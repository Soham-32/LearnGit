using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class TeamMemberV2Response
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsPulseAssessmentCompleted { get; set; }
        public bool IsExcludedForQuestions { get; set; }
        public bool IsFilteredByRole { get; set; }
        public List<RoleResponse> Tags { get; set; }
        public Guid Uid { get; set; }
    }
    
    public static class TeamMemberResponseExtensions    
    {
        public static string FullName(this TeamMemberV2Response response)
        {
            return $"{response.FirstName} {response.LastName}";
        }
    }
}
