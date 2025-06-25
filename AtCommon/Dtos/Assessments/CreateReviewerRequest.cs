using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments
{
    public class CreateReviewerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<RoleResponse> RoleTags { get; set; }
        public Guid MemberUid { get; set; }
    }

    public static class CreateReviewerRequestExtensions
    {
        public static string FullName(this CreateReviewerRequest response)
        {
            return $"{response.FirstName} {response.LastName}";
        }
    }
}