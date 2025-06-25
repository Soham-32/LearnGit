using System;
using System.IO;
using System.Linq;
using AtCommon.Dtos.Email;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Utilities;
using Newtonsoft.Json.Linq;

namespace AtCommon.ObjectFactories
{
    public static class EmailFactory
    {
        public static JObject Json { get; set; }

        public static I360EmailRequest GetValidEmailBody(string username)
        {
            var unique = Guid.NewGuid().ToString();

            return new I360EmailRequest()
            {
                From = username,
                To = username,
                Subject = $"Automation {unique}",
                PlainTextContent = "Testing Email from AT",
                HtmlContent = null,
                ScheduledDateTime = null
            };
        }

        public static SurveyEmailTranslations GetSurveyEmailBodyByLanguage(string languageName)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/TestData/LanguageTranslation/SurveyEmailTranslations.json";
            var text = File.ReadAllText(filePath);
            Json = JObject.Parse(text);

            var languageData = (Json["Languages"])!
                .FirstOrDefault(language => language["language"].ToString() == languageName)
                .CheckForNull($"No language found in the file with description <{languageName}>");

            return new SurveyEmailTranslations()
            {
                Language = languageData["language"]?.ToString(),
                SurveyEmailBody = languageData["surveyEmailBody"]?.ToString(),
                SurveyEmailMessageSubject = languageData["surveyEmailMessageSubject"]?.ToString(),
            };
        }

        public static SetupEmailTranslations GetSetupEmailBodyByLanguage(string languageName)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/TestData/LanguageTranslation/SetupEmailTranslations.json";
            var text = File.ReadAllText(filePath);
            Json = JObject.Parse(text);

            var languageData = (Json["Languages"])!
                .FirstOrDefault(language => language["language"].ToString() == languageName)
                .CheckForNull($"No language found in the file with description <{languageName}>");

            return new SetupEmailTranslations()
            {
                Language = languageData["language"]?.ToString(),
                SetupEmailBody = languageData["SetupEmailBody"]?.ToString(),
                SetupEmailMessageSubject = languageData["SetupEmailMessageSubject"]?.ToString(),
            };
        }
    }
}