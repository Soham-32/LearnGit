using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics.StructuralAgility
{
    public class PeopleByRoleResponse
    {
        public bool HasResults { get; set; }
        public RequestParameters Parameters { get; set; }
        public int TotalPeople { get; set; }
        public List<PeopleByRoleData> Data { get; set; }
    }

    public class PeopleByRoleData
    {
        public string WidgetType { get; set; }
        public int? ChildTeamId { get; set; }
        public string ChildTeamName { get; set; }
        public string MemberRole { get; set; }
        public int? MemberCount { get; set; }
        public long MemberRoleRank { get; set; }
    }

    public class RequestParameters
    {
        public string WidgetType { get; set; }
        public int TeamId { get; set; }
        public int WorkTypeId { get; set; }
        public string AuthorizedteamList { get; set; }
    }
}
