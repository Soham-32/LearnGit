using AtCommon.Dtos.Teams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtCommon.Dtos.IndividualAssessments
{
    public class IndividualAssessmentMemberRequest
    {
        public IndividualAssessmentMemberRequest()
        {
            Tags = new List<TagRequest>();
            RoleTags = new List<RoleRequest>();
            ParticipantGroups = new List<RoleRequest>();
            Reviewers = new List<IndividualAssessmentMemberRequest>();
        } 

        public Guid Uid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
        public List<TagRequest> Tags { get; set; }
        public List<RoleRequest> RoleTags { get; set; }
        public List<RoleRequest> ParticipantGroups { get; set; }
        public List<IndividualAssessmentMemberRequest> Reviewers { get; set; }
    }

    public static class AddIndividualMemberRequestExtensions
    {
        public static string FullName(this IndividualAssessmentMemberRequest response)
        {
            return $"{response.FirstName} {response.LastName}";
        }

        public static IndividualAssessmentMemberRequest AddTag(this IndividualAssessmentMemberRequest request, string category, string value)
        {
            var existingTag = request.Tags.FirstOrDefault(t => t.Category == category);
            if (existingTag == null)
            {
                request.Tags.Add(new TagRequest
                {
                    Category = category,
                    Tags = new List<string> { value }
                });
            }
            else
            {
                existingTag.Tags.Add(value);
            }

            return request;
        }

    }
}
