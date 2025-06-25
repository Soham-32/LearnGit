using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using OpenQA.Selenium;

namespace AtCommon.Utilities
{
    public class CSharpHelpers
    {
        private static readonly Random Rnd = new Random();

        //To generate random number
        public static int RandomNumber(int numberOfDigits = 6)
        {
            var max = 1;
            for (var i = 0; i < numberOfDigits; i++)
            {
                max *= 10;
            }
            return Rnd.Next(1, max);
        }
        public static string RandomString(int numberOfChar = 6)
        {
            var strBuild = new StringBuilder();

            for (var i = 0; i < numberOfChar; i++)
            {
                var flt = Rnd.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(25 * flt));
                var letter = Convert.ToChar(shift + 65);
                strBuild.Append(letter);
            }
            return strBuild.ToString();
        }

        public static string GenerateRandomNames()
        {
            // Lists of 4-letter adjectives and nouns
            var adjectives = new List<string> { "Bold", "Calm", "Dawn", "Epic", "Fast", "Glad", "Kind", "Wise", "Wild", "Cool", "Slow", "Soft", "Hard", "Angry", "Happy", "Dark" };
            var nouns = new List<string> { "Bats", "Lamb", "Cats", "Hawk", "Wolf", "Lynx", "Frog", "Bear", "Lion", "Swim", "Tiger", "Dogs", "Rabbits", "Zebras", "Turtles", "Horse" };

            // Create a Random instance to select random elements
            var random = new Random();

            // Select a random adjective from the list of adjectives
            var adjective = adjectives[random.Next(adjectives.Count)];

            // Select a random noun from the list of nouns
            var noun = nouns[random.Next(nouns.Count)];

            // Combine the randomly selected adjective and noun to create and return a team name
            return $"{adjective} {noun}";
        }

        public static int Random8Number()
        {
            return Rnd.Next(10000000, 99999999);
        }
        public static int RandomNumberBetween2Numbers(int num1, int num2)
        {
            return Rnd.Next(num1, num2);
        }

        public static int CompareTwoDate(string firstDate, string secondDate)
        {
            return DateTime.Compare(DateTime.Parse(firstDate), DateTime.Parse(secondDate));
        }


        /// <summary>
        /// Covert RGB to HEX
        /// </summary>
        /// <param name="rbg">Expected input examples are 'rgb(194, 24, 91)' or 'rgba(194, 24, 91)'</param>
        /// <returns>Equivalent HEX number</returns>
        public static string ConvertRgbToHex(string rbg)
        {
            rbg = rbg.Replace("rgba(", "").Replace("rgb(", "").Replace(")", "");
            var newRbg = rbg.Split(',');
            var myColor = Color.FromArgb(Convert.ToInt32(newRbg[0]), Convert.ToInt32(newRbg[1]),
                Convert.ToInt32(newRbg[2]));
            return "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
        }

        public static string CovertHexToRgb(string hexColor)
        {
            var color = ColorTranslator.FromHtml(hexColor);
            return $"rgb({color.R}, {color.G}, {color.B})";
        }
        public static string GetRandomColor(Random random)
        {
           // var random = new Random();
            var hexOutput = $"{random.Next(0, 0xFFFFFF):X}";
            while (hexOutput.Length < 6)
                hexOutput = "0" + hexOutput;
            return "#" + hexOutput;
        }

        public string ImageToBase64(string imagePath)
        {
            using var image = Image.FromFile(imagePath);
            using var m = new MemoryStream();
            image.Save(m, image.RawFormat);
            var imageBytes = m.ToArray();
            var base64String = Convert.ToBase64String(imageBytes);
            return "data:image/png;base64," + base64String;
        }

        public static void Base64ToImage(string base64String, string fileName)
        {
            var bytes = Convert.FromBase64String(base64String);

            Image image;
            using (var ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            var pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            image.Save($@"{Path.Combine(pathUser, "Downloads")}\{fileName}.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public IList<string> SortListAscending(IList<string> list) => list.OrderBy(n => n).ToList();

        public IList<string> SortListAscending(IList<int> list) =>
            list.OrderBy(n => n).Select(n => n.ToString()).ToList();


        public IList<string> SortListDescending(IList<string> list) => list.OrderByDescending(n => n).ToList();

        public IList<string> SortListDescending(IList<int> list) =>
            list.OrderByDescending(n => n).Select(n => n.ToString()).ToList();

        public static bool YesNoToBool(string text)
        {
            return text.ToLower() switch
            {
                "yes" => true,
                "no" => false,
                _ => throw new Exception($"Cannot parse the value of <{text}> to a bool.")
            };
        }

        public int GetIndexForPartialMatchingText(List<string> list, string matchingText)
        {
            return list.FindIndex(text => text.Contains(matchingText));
        }

        public int GetIndexForExactMatchingText(List<string> list, string matchingText)
        {
            return list.FindIndex(text => text.Equals(matchingText));
        }

        //Invoke Process
        public static void RunExternalExe(string filePath, string commandLineArgs)
        {
            try
            {
                var procStartInfo = new ProcessStartInfo
                {
                    FileName = filePath, Arguments = commandLineArgs
                };

                using var process = new Process {StartInfo = procStartInfo};
                process.Start();
                process.WaitForExit();
            }
            catch
            {
                // ignored
            }
        }

        public static string GetTimeZone(DateTime time)
        {
            var timeZoneFormat = TimeZoneInfo.Local.IsDaylightSavingTime(time)
                ? TimeZoneInfo.Local.DaylightName
                : TimeZoneInfo.Local.StandardName;

            return timeZoneFormat;
        }

        public static DateTime ChangeDateTimeFormat(DateTime? dateTime, string format = "M/d/yyyy h:mm:ss tt")
        {
            var published = DateTime.ParseExact(dateTime.ToString(), format, CultureInfo.InvariantCulture).ToString(format);
            var pulseRequestPublishedDateTime = Convert.ToDateTime(published);

            return pulseRequestPublishedDateTime;
        }

        public static byte[] GetImageHash(Bitmap image)
        {
            var bytes = new byte[1];
            bytes = (byte[]) (new ImageConverter()).ConvertTo(image, bytes.GetType());

            return (new MD5CryptoServiceProvider()).ComputeHash(bytes ?? throw new InvalidOperationException());
        }
        

        public static string GetClipboard()
        {
            string clipBoard = null;
            var thread = new Thread(() => clipBoard = Clipboard.GetText());
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
            return clipBoard;
        }

        public static T HandleStaleElement<T>(Func<T> action, int maxRetries = 2, int delayMilliseconds = 200)
        {
            for (var attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    return action();
                }
                catch (StaleElementReferenceException)
                {
                    if (attempt == maxRetries - 1)
                    {
                        throw;
                    }
                    Thread.Sleep(delayMilliseconds); // Wait before retrying
                }
            }

            throw new InvalidOperationException("Unable to complete the action due to stale element issues.");
        }
        public static int GetRandomEnumValue<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (int)values.GetValue(Rnd.Next(values.Length));
        }

        public static T GetRandomEnumKey<T>()
        {
            var enumValues = Enum.GetValues(typeof(T)); // Get all enum values
            return (T)enumValues.GetValue(Rnd.Next(enumValues.Length)); // Return a random value from the enum
        }

        public static string GetNormalizedText(string text)
        {
            return Regex.Replace(text ?? "", @"\s+", " ").Trim();
        }
    }


    public static class StringExtensions
    {
        private static readonly Regex WhiteSpaceRegex = new Regex(@"\s+");
        public static string RemoveWhitespace(this string stringWithSpaces) => WhiteSpaceRegex.Replace(stringWithSpaces, "");
        public static string HtmlToString(this string htmlText)
        {
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(htmlText, "");
        }
            
        public static string HtmlDecode(this string htmlText) => System.Net.WebUtility.HtmlDecode(htmlText);

        public static string ReplaceStringData(this string text)
        {
            var surveyQuestionTextReplaceInfo = new Dictionary<string, string>
            {
                { " ", "" },
                { "&gt;", ">" },
                { "&nbsp;", "" },
                { "\n", "" },
                { "&amp;", "&" },
                { "�", "’" },
                { "\r", "" },
                { "&rsquo;", "’"}
            };

            return surveyQuestionTextReplaceInfo.Aggregate(text, (current, keyValuePair) => current.Replace(keyValuePair.Key, keyValuePair.Value));
        }

        public static string FormatSurveyQuestions(this string text)
        {
            return text.HtmlToString().ReplaceStringData();
        }

        public static int ToInt(this string text)
        {
            return text == string.Empty ? 0 : int.Parse(text);
        }

        public static List<string> SplitLines(this string text)
        {
            return text.Split(new[] {'\r', '\n'},
                StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<string> StringToList(this string text, char splitBy = ',')
        {
            return text.Split(splitBy).Select(s => s.Trim()).ToList();
        }

        public static string ListToString(this IEnumerable<string> list, char joinBy = ',')
        {
            return string.Join($"{joinBy}", list.ToArray());
        }

        public static int GetDigits(this string text) 
        {
            var emptyString = string.Empty;
            return int.Parse(text.Where(t => char.IsDigit(t)).Aggregate(emptyString, (current, t) => current + t));
        }

        public static string NormalizeSpace(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var output = new StringBuilder();
            var skipped = false;

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (skipped) continue;
                    output.Append(' ');
                    skipped = true;
                }
                else
                {
                    skipped = false;
                    output.Append(c);
                }
            }

            return output.ToString();
        }

        public static DateTime GetTimeFromString(this string statement)
        {
            var regex = Regex.IsMatch(statement, @"\d{2}:\d{2}") ? new Regex(@"\d{2}:\d{2}") : new Regex(@"\d{1}:\d{2}");
            var match = regex.Match(statement);
            if (match.Success)
            {
                var matchString = match.ToString();
                return DateTime.Parse(matchString);
            }
            else
            {
                throw new FormatException("The statement does not contain a valid time format.");
            }
        }

        public static string RemoveTimeFromString(this string statement)
        {
            var regex = Regex.IsMatch(statement, @"\d{2}:\d{2}") ? new Regex(@"\d{2}:\d{2}") : new Regex(@"\d{1}:\d{2}");
            var match = regex.Match(statement);
            if (match.Success)
            {
                var matchString = match.ToString();
                return statement.Replace(matchString, "");
            }
            else
            {
                throw new FormatException("The statement does not contain a valid time format.");
            }
        }

    }

    public static class DateTimeExtensions
    {
        public static DateTime NextFiveMinutes(this DateTime date)
        {
            var remainder = date.Minute % 5;
            return remainder.Equals(0) ? date : date.AddMinutes(5 - remainder);
        }
    }

}