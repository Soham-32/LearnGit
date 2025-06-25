using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class ManageCampaignsV2Factory
    {
        public static List<string> FacilitatorsMatchmakingStrategyList()
        {
            return new List<string>()
            {
                "ManuallyAssignFacilitators",
                "TeamFindFacilitator",
                "AutoAssignFacilitators"
            };
        }

        public static List<string> RadarTypeList()
        {
            return new List<string>()
            {
                "AT - TH3.0 - DO NOT USE",
                "Program Health",
                "DevOps Assessment",
                "Technical Health",
                "DevOps Health",
                "TeamHealth 3.0",
                "Maturity Testing [DoNotDelete]",
                "AT - TH2 - DO NOT USE",
                "AT - TH2.5 - DO NOT USE",
            };
        }

        public static List<string> ParentTeamList()
        {
            return new List<string>()
            {
                "Entire company",
                "Automation Enterprise Team",
                "Automation Multi Team",
                "Automation Enterprise Team for Radar",
                "Automation Multi Team for Radar 2",
                "Automation MultiTeam For Radar",
                "Automation ET for Growth Journey",
                "Automation MT for Growth Journey",
                "Automation ET for MTET GI",
                "Automation MT for MTET GI",
                "Automation ET Growth Journey 2",
                "Automation MT Growth Journey 2",
                "Automation MT for GI",
                "Automation Multi Team For GI Tab",
                "Automation MultiTeam For Benchmarking"
            };
        }

        public static List<string> TargetNoPerFacilitatorList()
        {
            return new List<string>()
            {
                "1 Team per Facilitator",
                "2 Teams per Facilitator",
                "3 Teams per Facilitator",
                "4 Teams per Facilitator"
            };
        }

        public static List<string> TeamNamesList()
        {
            return new List<string>()
            {
                "Automation Normal Team"
            };
        }
        public static List<string> FacilitatorFirstNamesList()
        {
            return new List<string>()
            {
                "Team"
            };
        }

        public static CampaignDetails GetCampaignDetails()
        {
            return new CampaignDetails
            {
                Name = "ATCampaign" +RandomDataUtil.GetAssessmentName(),
                RadarType = SharedConstants.TeamHealthRadarName,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(30),
                ParentTeam = ParentTeamList().FirstOrDefault(),
                FacilitatorsMatchmakingStrategy = FacilitatorsMatchmakingStrategyList().LastOrDefault(),
                TeamsPerFacilitator = "2 Teams per Facilitator"
            };
        }

        public static SelectTeamsDetails GetSelectTeamsDetails()
        {
            return new SelectTeamsDetails
            {
                TeamName = "Automation Normal Team",
                Tag = "Kanban",
                WorkType = "Software Delivery",
                TeamContactIsAhf = "No"
            };
        }

        public static SelectFacilitatorsDetails GetSelectFacilitatorsDetails()
        {
            return new SelectFacilitatorsDetails
            {
                FirstName = "Team",
                LastName = "Admin",
                ParentTeam = "Automation Multi Team"
            };
        }

        public static SetupAssessmentsDetails GetSetupAssessmentDetails(FacilitationApproach facilitationApproach)
        {
            return new SetupAssessmentsDetails
            {
                Name = "ATAssessment" + RandomDataUtil.GetAssessmentName(),
                FacilitationApproach = facilitationApproach,
                StakeholderWindowStart = DateTime.Today.AddDays(1),
                StakeholderWindowEnd = DateTime.Today.AddDays(2),
                StakeholderLaunchDate = DateTime.Today.AddDays(2),
                TeamMemberLaunchDate = DateTime.Today.AddDays(5),
                TeamMemberLaunchDateForOneMeeting = DateTime.Today.AddDays(17),
                AssessmentStartDate = DateTime.Today.AddDays(2),
                AssessmentCloseDate = DateTime.Today.AddDays(10),
                RetrospectiveWindowStart = DateTime.Today.AddDays(15),
                RetrospectiveWindowEnd = DateTime.Today.AddDays(25)
            };
        }
    }
}
