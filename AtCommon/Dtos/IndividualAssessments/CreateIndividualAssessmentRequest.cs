using System;
using System.Collections.Generic;
using AtCommon.Dtos.Companies;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class CreateIndividualAssessmentRequest
    {
        public CreateIndividualAssessmentRequest()
        {
            Members = new List<IndividualAssessmentMemberRequest>();
            IndividualViewers = new List<UserRequest>();
            AggregateViewers = new List<UserRequest>();
        }

        public int BatchId { get; set; }
        public string AssessmentName { get; set; }
        public string PointOfContact { get; set; }
        public string PointOfContactEmail { get; set; }
        public Guid TeamUid { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int SurveyTypeId { get; set; }
        public bool AllowInvite { get; set; }
        public bool AllowResultView { get; set; }
        public bool Published { get; set; }
        public bool ReviewEachOther { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<IndividualAssessmentMemberRequest> Members { get; set; }
        public List<UserRequest> IndividualViewers { get; set; }
        public List<UserRequest> AggregateViewers { get; set; }
    }
}
