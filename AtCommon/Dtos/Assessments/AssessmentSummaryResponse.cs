namespace AtCommon.Dtos.Assessments
{
    public class AssessmentSummaryResponse : BaseDto
    {
        public string AssessmentName { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string SurveyType { get; set; }
        public string SurveyName { get; set; }
    }
}