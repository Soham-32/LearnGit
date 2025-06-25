using AtCommon.Dtos.Tags;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using AtCommon.Dtos.IndividualAssessments;

namespace AtCommon.Dtos.Teams
{
    public class AddMemberRequest
    {
        public AddMemberRequest()
        {
            Tags = new List<TeamMemberTagRequest>();
            Reviewers = new List<AddMemberRequest>();
        }
        [JsonIgnore]
        public Guid Uid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
        public List<RoleRequest> RoleTags { get; set; }
        public List<TeamMemberTagRequest> Tags { get; set; }
        public List<AddMemberRequest> Reviewers { get; set; }
    }

    public static class AddMemberRequestExtensions
    {
        public static string FullName(this AddMemberRequest response)
        {
            return $"{response.FirstName} {response.LastName}";
        }

        public static AddMemberRequest AddTags(this AddMemberRequest request, List<string> roles, List<string> participantGroups)
        {
            request.Tags = new List<TeamMemberTagRequest>
            {
                new TeamMemberTagRequest
                {
                    Category = "Role",
                    Tags = roles
                },
                new TeamMemberTagRequest
                {
                    Category = "Participant Group",
                    Tags = participantGroups
                }
            };

            return request;
        }

    }
}