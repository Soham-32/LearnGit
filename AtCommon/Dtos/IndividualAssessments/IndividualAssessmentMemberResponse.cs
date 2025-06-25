using System;
using System.Collections.Generic;


namespace AtCommon.Dtos.IndividualAssessments
{
    public class IndividualAssessmentMemberResponse
    {
        public IndividualAssessmentMemberResponse()
        {
            Members = new List<IndividualAssessmentMemberDetailsResponse>();
            IndividualViewers = new List<ViewerResponse>();
            AggregateViewers = new List<ViewerResponse>();
        }

        public int BatchId { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentTypeName { get; set; }
        public string PointOfContact { get; set; }
        public string PointOfContactEmail { get; set; }
        public DateTime? AssessmentStartDateTime { get; set; }
        public DateTime? AssessmentEndDateTime { get; set; }
        public bool AllowInvite { get; set; }
        public bool AllowResultView { get; set; }
        public bool ReviewEachOther { get; set; }
        public Guid TeamUid { get; set; }
        public bool Published { get; set; }
        public int SurveyTypeId { get; set; }
        public List<IndividualAssessmentMemberDetailsResponse> Members { get; set; }
        public List<ViewerResponse> IndividualViewers  { get; set; }
        public List<ViewerResponse> AggregateViewers { get; set; }

    }
}
