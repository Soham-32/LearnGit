using System;
using System.Collections.Generic;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.IO;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams
{
    public class EnterpriseTeamsFactory
    {
        public static Team GetValidEnterpriseTeamInfo() => new Team
        {
            TeamName = $"EnterpriseTeam_{RandomDataUtil.GetTeamName()}",
            ExternalIdentifier = "Test",
            DepartmentAndGroup = "Test Department",
            TeamBioOrBackground = RandomDataUtil.GetTeamBio(),
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Coaching", SharedConstants.TeamTagCoaching)
            }
        };

        public static Team GetValidUpdatedEnterpriseTeamInfo() => new Team
        {
            TeamName = $"EntepriseTeamEdited_{RandomDataUtil.GetTeamName()}",
            ExternalIdentifier = "Test",
            DepartmentAndGroup = $"{RandomDataUtil.GetTeamDepartment()} Edited",
            TeamBioOrBackground = $"{RandomDataUtil.GetTeamBio()} Edited",
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Strategic Objectives", "Objective 1")
            }
        };
    }
}