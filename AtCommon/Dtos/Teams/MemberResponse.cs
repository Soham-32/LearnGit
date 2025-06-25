using System;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;

namespace AtCommon.Dtos.Teams
{
    public class MemberResponse : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime? EmailSentAt { get; set; }
    }

    public static class MemberResponseExtensions
    {
        public static string FullName(this MemberResponse response)
        {
            return $"{response.FirstName} {response.LastName}";
        }

        public static AddMemberRequest ToAddMemberRequest(this MemberResponse response)
        {
            return new AddMemberRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                ExternalIdentifier = response.ExternalIdentifier,
                Uid = response.Uid
            };
        }

        public static IndividualAssessmentMemberRequest ToAddIndividualMemberRequest(this MemberResponse response)
        {
            return new IndividualAssessmentMemberRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                ExternalIdentifier = response.ExternalIdentifier,
                Uid = response.Uid
            };
        }

        public static UserRequest ToAddUserRequest(this MemberResponse response)
        {
            return new UserRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
            };
        }
    }
}
