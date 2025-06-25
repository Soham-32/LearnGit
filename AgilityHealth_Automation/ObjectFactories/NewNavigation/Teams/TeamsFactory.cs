using System;
using System.Collections.Generic;
using AtCommon.Utilities;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.Globalization;
using System.IO;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Radar.RadarPage;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams
{
    public class TeamsFactory
    {
        public static Team GetValidFullWizardTeamInfo(string workType = SharedConstants.NewTeamWorkType) => new Team
            {
                TeamName = $"fullWizardTeam_{RandomDataUtil.GetTeamName()}",
                WorkType = workType,
                PreferredLanguage = RadarLanguage.English.ToString(),
                Methodology = "Scrum",
                ExternalIdentifier = "Test",
                DepartmentAndGroup = $"{RandomDataUtil.GetTeamDepartment()} Edited",
            DateEstablished = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                TeamBioOrBackground = RandomDataUtil.GetTeamBio(),
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                Tags = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Coaching", SharedConstants.TeamTagCoaching)
                }
        };
        public static Team GetValidUpdatedFullWizardTeamInfo() => new Team
        {
            TeamName = $"TeamEditedFullWizardTeam_{RandomDataUtil.GetTeamName()}",
            WorkType = "Business Operations",
            PreferredLanguage = RadarLanguage.Spanish.ToString(),
            Methodology = "Kanban",
            ExternalIdentifier = "Test Edited",
            DepartmentAndGroup = $"{RandomDataUtil.GetTeamDepartment()} Edited",
            DateEstablished = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
            TeamBioOrBackground = $"{RandomDataUtil.GetTeamBio()} Edited",
            ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"),
            Tags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Strategic Objectives", "Objective 1"),
            }
        };

        public static Member GetTeamMemberInfo() => new Member
        {
            FirstName = "Member",
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "m" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain,
            Role = new List<string>(){"Designer", "Developer" },
            ParticipantGroup = new List<string>() { "Collocated", "Contractor" }
        };

        public static Member GetEditTeamMemberInfo() => new Member
        {
            FirstName = "EditMember",
            LastName = "EditedName",
            Email = Constants.UserEmailPrefix + "editm" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain,
            Role = new List<string>() { "QA Tester", "Scrum Master" },
            ParticipantGroup = new List<string>() { "Support", "FTE" }
        };

        public static Member GetStakeholderInfo() => new Member
        {
            FirstName = "Stake",
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "s" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain,
            Role = new List<string>() { "Sponsor", "Manager" }
        };

        public static Member GetEditStakeholderInfo() => new Member
        {
            FirstName = "EditStake",
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "s" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain,
            Role = new List<string>() { "Customer", "Executive" }
        };
    }
}
