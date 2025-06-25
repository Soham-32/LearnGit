using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class TeamMembersExcludedFromQuestionsResponse
    {
        public int TeamId { get; set; }
        public Guid TeamUid { get; set; }
        public List<TeamMemberExclusion> TeamMembers { get; set; }
    }
    public class TeamMemberExclusion
    {
        public int MemberId { get; set; }
        public string MemberUid { get; set; }
        public bool IsDisabled { get; set; }
    }
}
