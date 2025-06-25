using AtCommon.Dtos.Teams;
using System;
using System.Collections.Generic;
using AtCommon.Dtos.IndividualAssessments;

namespace AtCommon.Dtos.Assessments
{
    public class ReviewerResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } 
        public DateTime? EmailSentAt { get; set; }
        public DateTime? AssessmentCompleteDateTime { get; set; }
        public bool MemberExists { get; set; }
        public Guid AssessmentUid { get; set; }
        public string AssessmentUrl { get; set; }
        public List<TagResponse> Tags { get; set; }
        public List<RoleResponse> RoleTags { get; set; }
        public Guid Uid { get; set; }
    }

    public static class ReviewerExtensions
    {
        public static AddMemberRequest ToAddMemberRequest(this ReviewerResponse response)
        {
            return new AddMemberRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Uid = response.Uid
            };
        }

        public static IndividualAssessmentMemberRequest ToAddIndividualMemberRequest(this ReviewerResponse response)
        {
            return new IndividualAssessmentMemberRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Uid = response.Uid
            };
        }

        public static string FullName(this ReviewerResponse response)
        {
            return $"{response.FirstName} {response.LastName}";
        }
    }
}
