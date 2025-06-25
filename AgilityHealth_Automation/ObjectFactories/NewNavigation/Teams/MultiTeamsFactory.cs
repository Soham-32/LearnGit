using System;
using System.Collections.Generic;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.IO;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams
{
    public class MultiTeamsFactory
    {
        public static Team GetValidMultiTeamInfo(string workType = SharedConstants.NewMultiTeamWorkType) => new Team
        {
            TeamName = $"multiTeam_{RandomDataUtil.GetTeamName()}",
            WorkType = workType,
            ExternalIdentifier = "Test",
            DepartmentAndGroup = "Test Department",
            TeamBioOrBackground = RandomDataUtil.GetTeamBio(),
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Coaching", SharedConstants.TeamTagCoaching)
            }
        };

        public static Team GetValidUpdatedMultiTeamInfo() => new Team
        {
            TeamName = $"MultiTeamEdited_{RandomDataUtil.GetTeamName()}",
            WorkType = "Chapter",
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