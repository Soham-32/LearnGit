using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch
{
    public class QuickLaunchMemberAccess
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> ParticipantGroups { get; set; }
    }

    public static class QuickLaunchMemberAccessFormExtensions
    {

        public static string FullName(this QuickLaunchMemberAccess response)
        {
            return $"{response.FirstName} {response.LastName}";
        }
    }
}