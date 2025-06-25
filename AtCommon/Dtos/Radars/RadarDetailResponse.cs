using System;
using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class RadarDetailResponse
    {
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public int OriginalVersion { get; set; }
        public bool Active { get; set; }
        public string SurveyType { get; set; }
        public int SurveyTypeId { get; set; }
        public int Scale { get; set; }
        public string Copyright { get; set; }
        public string DateActivated { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public bool IsPublic { get; set; }
        public int LinkedAppId { get; set; }
        public string ReviewerThankYouMessage { get; set; }
        public string RevieweeThankYouMessage { get; set; }
        public string ThankYouMessage { get; set; }
        public string ReviewerSurveyMessage { get; set; }
        public string RevieweeSurveyMessage { get; set; }
        public string SurveyMessage { get; set; }
        public string ReviewerEmailMessage { get; set; }
        public string RevieweeEmailMessage { get; set; }
        public string EmailMessage { get; set; }
        public int SourceAppId { get; set; }
        public string Logo { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsBeta { get; set; }
        public int Order { get; set; }
        public bool ShowOneResponse { get; set; }
        public string EmailSender { get; set; }
        public string EmailSubject { get; set; }
        public string ReviewerEmailSender { get; set; }
        public string ReviewerEmailSubject { get; set; }
        public bool EmailAppendStandardFooter { get; set; }
        public bool Absolute { get; set; }
        public string ReminderMessage { get; set; }
        public string RulesDefinitions { get; set; }
        public string RadarDisplayMessage { get; set; }
        public string UserRoles { get; set; }
        public bool? IsDefault { get; set; }
        public string DefaultDescription { get; set; }
        public string DefaultThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DimensionDetailResponse> Dimensions { get; set; }
    }
}
