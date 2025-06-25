using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AtCommon.Utilities
{
    public class GmailUtil
    {
        private static readonly string[] Scopes = { GmailService.Scope.GmailModify };
        private const string ApplicationName = "at-auto";
        private const int WaitInterval = 5000;
        public const string MemberEmailLabel = "Auto_Member";
        public const string UserEmailLabel = "Auto_User";

        private static GmailService InitGmailService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "secret_gmail.json"), FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-api.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "admin",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            return service;
        }

        private static UsersResource.MessagesResource.ListRequest GenerateEmailRequest(GmailService service, EmailSearch emailSearch)
        {
            var request = service.Users.Messages.List("me");

            request.Q = $"newer_than:1d subject:\"{emailSearch.Subject.Replace("|", "")}\" deliveredto:{emailSearch.To}";
            foreach (var label in emailSearch.Labels)
            {
                request.Q += $" label:{label}";
            }

            request.Q += $" {string.Join(" ", emailSearch.Keywords)}";

            return request;
        }

        private static Message GetMessageDetail(GmailService service, Message message)
        {
            var emailInfoReq = service.Users.Messages.Get("me", message.Id);
            return emailInfoReq.Execute();
        }

        private static string GetEmailBody(Message emailInfoResponse)
        {
            string body;
            if (emailInfoResponse.Payload.Parts == null &&
                    emailInfoResponse.Payload.Body != null)
                body = DecodeBase64String(emailInfoResponse.Payload.Body.Data);
            else
                body = GetNestedBodyParts(emailInfoResponse.Payload.Parts, "");
            return body;
        }

        private static List<LinkItem> GetEmailLinks(Message emailInfoResponse, string linkMatcher)
        {
            // parse the body and get all the links that contain the matcher
            var body = GetEmailBody(emailInfoResponse);
            return Find(body).Where(i => i.Href.Contains(linkMatcher) | i.Text.Contains(linkMatcher)).ToList();

        }

        private static TimeSpan GetElapsedTime(DateTime start)
        {
            var now = DateTime.Now;
            return now.Subtract(start);
        }

        private static Message FindEmail(EmailSearch emailSearch)
        {
            // create new gmail service
            var service = InitGmailService();
            // configure the request to send to gmail
            var messageListRequest = GenerateEmailRequest(service, emailSearch);
            // used to determine timeout
            var start = DateTime.Now;
            TimeSpan span;

            // keep looping until we find the email or run out of time
            do
            {
                // send the request and get the response
                var messageListResponse = messageListRequest.Execute();

                if (messageListResponse.Messages != null)
                {

                    // email detail request based on the Id
                    var emailInfoResponse = GetMessageDetail(service, messageListResponse.Messages.FirstOrDefault());

                    if (emailInfoResponse != null)
                        return emailInfoResponse;

                }
                System.Threading.Thread.Sleep(WaitInterval);

                span = GetElapsedTime(start);

            } while (span < emailSearch.Timeout);

            throw new Exception($"Could not find email with subject: <{emailSearch.Subject}> \nsent to: <{emailSearch.To}> \nkeywords: <{string.Join(",", emailSearch.Keywords)}>");
        }

        public static string GetLink(string linkMatcher, EmailSearch emailSearch)
        {
            var email = FindEmail(emailSearch);

            var body = GetEmailBody(email);

            if (string.IsNullOrEmpty(body))
            {
                throw new Exception($"Email body is empty for email with subject <{emailSearch.Subject}>.");
            }

            var links = Find(body);

            // Mark email as read
            MarkAsRead(email.Id);

            foreach (var link in links)
            {
                try
                {
                    var finalUrl = GetRedirectedUrl(link.Href);

                    if (finalUrl.Contains(linkMatcher))
                    {
                        return finalUrl;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error following URL {link.Href}: {ex.Message}");
                }
            }

            if (!links.Any()) throw new Exception(
                $"No links with <{linkMatcher}> were found in the email sent to <{emailSearch.To}> with subject <{emailSearch.Subject}>.");
            // some emails contain multiple survey links, but in this case we only want the first one
            return links.First().Href;
        }

        public static string GetLink(string linkMatcher, string expectedSubject, string sendTo, string filterLabel = "", string keyword = "")
        {
            if (filterLabel == "") { filterLabel = MemberEmailLabel; }
            var emailSearch = new EmailSearch
            {
                To = sendTo,
                Subject = expectedSubject,
                Labels = new List<string> { filterLabel },
                Keywords = new List<string> { keyword }
            };

            return GetLink(linkMatcher, emailSearch);
        }
        private static string GetRedirectedUrl(string initialUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(initialUrl);
            request.AllowAutoRedirect = false; // Prevent automatic redirection

            using HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check for redirection responses (e.g., 301, 302)
            if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently)
            {
                // Follow the redirect URL
                string redirectUrl = response.Headers["Location"];
                return GetRedirectedUrl(new Uri(new Uri(initialUrl), redirectUrl).ToString());
            }

            // Return the final URL if no redirection
            return response.ResponseUri.ToString();
        }

        public static string GetSurveyLink(string expectedSubject, string sendTo, string filterLabel = "", string keyword = "")
        {
            return GetLink("survey/assessment", expectedSubject, sendTo, filterLabel, keyword);
        }

        public static string GetPulseSurveyLink(string expectedSubject, string sendTo, string filterLabel = "", string keyword = "")
        {
            return GetLink("survey/pulse", expectedSubject, sendTo, filterLabel, keyword);
        }

        public static string GetFacilitatorOptInLink(string expectedSubject, string sendTo, string keyword = "")
        {
            return GetLink("Assessments/FacilitatorSignup", expectedSubject, sendTo, "inbox", keyword);
        }

        public static string GetSharedAssessmentLink(string expectedSubject, string sendTo, string filterLabel = "", string keyword = "")
        {
            return GetLink("Click here", expectedSubject, sendTo, filterLabel, keyword);
        }

        public static bool DoesMemberEmailExist(string expectedSubject, string sendTo, string filterLabel = "", string keyWord = "", int timeoutInSeconds=200)
        {
            if (filterLabel == "") { filterLabel = MemberEmailLabel; }
            var emailSearch = new EmailSearch
            {
                To = sendTo,
                Subject = expectedSubject,
                Labels = new List<string> { filterLabel },
                Keywords = new List<string> { keyWord },
                Timeout = new TimeSpan(0, 0, timeoutInSeconds)
            };

            return DoesMemberEmailExist(emailSearch);
        }

        public static bool DoesMemberEmailExist(EmailSearch emailSearch)
        {
            try
            {
                var email = FindEmail(emailSearch);
                MarkAsRead(email.Id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DoesAhfSurveyEmailExist(string email, string assessmentName, int timeOut=5)
        {
            var emailSearch = new EmailSearch
            {
                To = email,
                Subject = "AHF's Assessment - Please Respond",
                Labels = new List<string> { MemberEmailLabel },
                Keywords = new List<string> { assessmentName },
                Timeout = new TimeSpan(0,timeOut,0)
            };

            try
            {
                FindEmail(emailSearch);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DoesMeetingNotes(string subject, string email, string meetingTitle, string desc = "", int timeOut = 5)
        {
            var emailSearch = new EmailSearch
            {
                To = email,
                Subject = $"{subject} Meeting Minutes: {meetingTitle}",
                Labels = new List<string> { MemberEmailLabel },
                Keywords = string.IsNullOrEmpty(desc) ? new List<string>() : new List<string> { desc },
                Timeout = new TimeSpan(0, timeOut, 0)
            };
            try
            {
                FindEmail(emailSearch);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetAccountManagerEmailBody(string expectedSubject, string sendTo, string filterLabel = "", string keyword = "", int timeOut = 20)
        {
            var emailSearch = new EmailSearch
            {
                To = sendTo,
                Subject = expectedSubject,
                Labels = new List<string> { filterLabel },
                Keywords = new List<string> { keyword },
                Timeout = new TimeSpan(0, 0, timeOut)
            };
            try
            {
                var email = FindEmail(emailSearch);
                MarkAsRead(email.Id);
                return GetEmailBody(email).HtmlToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetUserActivationLink(string expectedSubject, string sendTo, string label = UserEmailLabel)
        {
            return GetLink("account/confirmemail", expectedSubject, sendTo, label);
        }

        public static string GetUserCreateAccountLink(string expectedSubject, string sendTo, string label = UserEmailLabel)
        {
            return GetLink("Create your account", expectedSubject, sendTo, label);
        }

        public static string GetLoginLink(string expectedSubject, string sendTo,
            string filterLabel = "", string keyword = "")
        {
            return GetLink("/login", expectedSubject, sendTo, filterLabel, keyword);
        }

        public static List<LinkItem> GetAllSurveyLinks(string expectedSubject, string sentTo,
            string keyword = "")
        {
            var emailSearch = new EmailSearch
            {
                To = sentTo,
                Subject = expectedSubject,
                Labels = new List<string> { "UNREAD" },
                Keywords = new List<string> {keyword}
            };
            var email = FindEmail(emailSearch);

            MarkAsRead(email.Id);
            return GetEmailLinks(email, "survey/assessment");

        }

        public static string GetPasswordResetLink(string email)
        {
            var emailSearch = new EmailSearch
            {
                Subject = "Reset your AgilityInsights Password",
                To = email,
                Labels = new List<string> { "Inbox", "UNREAD" }
            };
            return GetLink("/password/reset", emailSearch).Replace("&amp;", "&");
        }

        /// <summary>
        /// Modify the Labels a Message is associated with.
        /// </summary>
        /// <param name="messageId">ID of Message to modify.</param>
        public static Message MarkAsRead(string messageId)
        {
            var service = InitGmailService();
            var mods = new ModifyMessageRequest
            {
                RemoveLabelIds = new List<string> { "UNREAD" }
            };

            try
            {
                return service.Users.Messages.Modify(mods, "me", messageId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
        }


        private static IEnumerable<LinkItem> Find(string file)
        {
            var list = new List<LinkItem>();

            var m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            foreach (Match m in m1)
            {
                var value = m.Groups[1].Value;
                var i = new LinkItem();

                var m2 = Regex.Match(value, @"href='(.*?)'",
                    RegexOptions.Singleline);

                if (m2.Length == 0)
                {
                    m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                }

                if (m2.Success)
                {
                    i.Href = m2.Groups[1].Value;
                }

                var t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                    RegexOptions.Singleline);
                i.Text = t;

                list.Add(i);
            }
            return list;
        }

        public struct LinkItem
        {
            public string Href;
            public string Text;

            public override string ToString()
            {
                return Href + "\n\t" + Text;
            }
        }

        private static string GetNestedBodyParts(IList<MessagePart> part, string curr)
        {
            var str = curr;
            if (part == null)
            {
                return str;
            }
            foreach (var parts in part)
            {
                if (parts.Parts == null)
                {
                    if (parts.Body?.Data == null) continue;
                    var ts = DecodeBase64String(parts.Body.Data);
                    str += ts;
                }
                else
                {
                    return GetNestedBodyParts(parts.Parts, str);
                }
            }

            return str;
        }

        private static string DecodeBase64String(string s)
        {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }

    }

    public class EmailSearch
    {
        public EmailSearch()
        {
            Labels = new List<string>();
            Keywords = new List<string>();
            Timeout = new TimeSpan(0, 4, 0);
        }
        public string To { get; set; }
        public string Subject { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Keywords { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}