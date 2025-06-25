using AtCommon.Dtos.Teams;

namespace AtCommon.Dtos.Bulk
{
    public class MemberTagRequest : TagRequest
    {
        public string TeamExternalIdentifier { get; set; }
        public string Email { get; set; }
    }
}