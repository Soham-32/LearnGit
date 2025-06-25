using System;
using System.Collections.Generic;
using AtCommon.Utilities;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.IO;

namespace AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams
{
    public class PortfolioTeamsFactory
    {
        public static Team GetValidPortfolioTeamInfo(string workType = SharedConstants.NewPortfolioTeamWorkType) => new Team
        {
            TeamName = $"PortfolioTeam_{RandomDataUtil.GetTeamName()}",
            WorkType = workType,
            ExternalIdentifier = "Test",
            DepartmentAndGroup = $"{RandomDataUtil.GetTeamDepartment()} Edited",
            TeamBioOrBackground = RandomDataUtil.GetTeamBio(),
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Coaching", SharedConstants.TeamTagCoaching)
            }
        };

        public static Team GetValidUpdatedPortfolioTeamInfo() => new Team
        {
            TeamName = $"PortfolioTeamEdited_{RandomDataUtil.GetTeamName()}",
            WorkType = "Enterprise Team",
            ExternalIdentifier = "Test",
            DepartmentAndGroup = "Test Department Edited",
            TeamBioOrBackground = $"{RandomDataUtil.GetTeamBio()} Edited",
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Strategic Objectives", "Objective 1")
            }
        };
    }
}
