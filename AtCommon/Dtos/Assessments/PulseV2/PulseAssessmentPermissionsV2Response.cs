namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class PulseAssessmentPermissionsV2Response
    {
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }
}