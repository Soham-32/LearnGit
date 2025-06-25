using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class TeamMemberResponse : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<TagResponse> Tags { get; set; }
    }

    public static class TeamMemberResponseExtensions
    {
        public static string FullName(this TeamMemberResponse response)
        {
            return $"{response.FirstName} {response.LastName}";
        }
    }
}