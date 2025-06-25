using System.Collections.Generic;
using AtCommon.Dtos.Companies;

namespace AtCommon.Dtos.Radars
{
    public class QuestionMemberResponse
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string AlternativeText { get; set; }
        public int ScaleOverride { get; set; }
        public string HelpText { get; set; }
        public bool ExcludeCompany { get; set; }
        public bool ExcludeMethodology { get; set; }
        public bool ExcludeByTag { get; set; }
        public bool ExcludeWorkType { get; set; }
        public int? SurveyGizmoId { get; set; }
        public bool External { get; set; }
        public string ExcludeRole { get; set; }
        public bool? Quantitative { get; set; }
        public int Methodology { get; set; }
        public int WorkType { get; set; }
        public List<CompanyResponse> Companies { get; set; }
        public List<TagInfoResponse> Methodologies { get; set; }
        public List<TagInfoResponse> WorkTypes { get; set; }
        public List<TagInfoResponse> ParticipantTags { get; set; }
        public bool IsExcluded { get; set; }
        public int Number { get; set; }
        public int UserCompanyId { get; set; }
    }
}
