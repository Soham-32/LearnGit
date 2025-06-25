using System;
using System.Collections.Generic;
namespace AtCommon.Dtos.Radars
{
    public class RadarMemberResponse
    {
        public int SurveyId { get; set; }
        public int SurveyTypeId { get; set; }
        public int Scale { get; set; }
        public string ReviewerThankYouMessage { get; set; }
        public string RevieweeThankYouMessage { get; set; }
        public string ThankYouMessage { get; set; }
        public string ReviewerSurveyMessage { get; set; }
        public string RevieweeSurveyMessage { get; set; }
        public string SurveyMessage { get; set; }
        public bool Absolute { get; set; }
        public string CompanyName { get; set; }
        public bool IsMaster { get; set; }
        public string KioskProfileTitle { get; set; }
        public int? MethodologyId { get; set; }
        public int? WorkTypeId { get; set; }
        public string AssessmentLink { get; set; }
        public string Hash { get; set; }
        public int? KioskId { get; set; }
        public string RouteName { get; set; }
        public string StoreResultsUrl { get; set; }
        public string CompleteSurveyUrl { get; set; }
        public bool ShowKioskCompanyProfile { get; set; }
        public bool IsReviewee { get; set; }
        public bool IsIndividual { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int FinalStep { get; set; }
        public string TeamName { get; set; }
        public string TeamMemberName { get; set; }
        public string Notes { get; set; }
        public string Answers { get; set; }
        public double Results { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DimensionMemberResponse> Dimensions { get; set; }
    }
}
