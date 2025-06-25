using AtCommon.Utilities;
using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch;

namespace AtCommon.ObjectFactories.QuickLaunch
{
    public static class QuickLaunchAssessmentFactory
    {
        public static QuickLaunchAssessment GetValidQuickLaunchAssessmentInfo(string teamName = null, bool isCreateNewTeam = true) => new QuickLaunchAssessment
        {
            RadarName = SharedConstants.TeamHealthRadarName,
            ExistingTeamName = teamName,
            CreateNewTeam = isCreateNewTeam,
            NewTeamName = $"Quick_Launch{RandomDataUtil.GetTeamName()}"
        };

        public static QuickLaunchMemberAccess GetValidQuickLaunchMemberAccessInfo() => new QuickLaunchMemberAccess
        {
            FirstName = $"QuickLaunch_Mem_{RandomDataUtil.GetFirstName()}",
            LastName = SharedConstants.TeamMemberLastName,
            Email = $"ah_automation+quicklaunch_mem{Guid.NewGuid():D}@agiletransformation.com",
            Roles = new List<string> { "Agile Coach", "Architect", "Business Analyst", "Chief Product Owner", "Designer" },
            ParticipantGroups = new List<string> { "Collocated", "Contractor", "Distributed", "FTE", "Leadership Team" }
        };

        public static List<string> GetExpectedAhTrialRadarList()
        {
            return new List<string>()
            {
                "DevOps Health V2.0",
                "Enterprise Business Agility 2.0",
                "SAFe Business Agility",
                "SAFe Team and Technical Agility (NEW)",
                "Team Culture 2.0",
                "TeamHealth 4.0"
            };
        }

        public static List<string> GetExpectedAhTrialWorkTypeList()
        {
            return new List<string>()
            {
                "Business Operations",
                "Feature Team",
                "Kiosk",
                "SAFe - Release Management",
                "Service and Support",
                "Software Delivery",
                "Transformation"
            };
        }
    }
}