using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class CampaignFactory
    {
        public static readonly string FacilitatorId = "c041460c-7839-4dd2-abc9-96e801490dae"; //Company Admin User

        public static List<string> MatchMakingStrategyList = new List<string> { "AutoAssignFacilitators", "ManuallyAssignFacilitators", "TeamFindFacilitator" };
        public static List<string> CreateAssessmentTypeList = new List<string> { "AutoCreateAssessment", "TeamFindFacilitator" };

        public static List<int> TeamIdsList = new List<int> { 7130, 6349, 40375, 9765 };
        public static List<string> FacilitatorIdsList = new List<string> { "45771fa1-ec5e-4410-a13c-385b8c186af2", "c041460c-7839-4dd2-abc9-96e801490dae" };
        public static List<string> AssessmentTypesList = new List<string> { "TwoSeparateMeetings", "SingleRetroMeeting", "SingleCombinedMeeting" };
        public static List<string> SearchTagList = new List<string> { "Software Delivery", "Scrum", "Automation" };
        public static List<string> AssessmentStatusList = new List<string> { "PreAssessment", "Open", "AssessmentComplete", "RetroComplete", "LeaderReadoutComplete" };

        public static CreateCampaignRequest GetCampaign()
        {
            var random = new Random();
            var guid = Guid.NewGuid();

            return new CreateCampaignRequest
            {
                Name = $"AT_Campaign{RandomDataUtil.GetCompanyZipCode()}",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(15),
                SurveyId = SharedConstants.AtTeamHealth3SurveyId,
                MatchMakingStrategy = MatchMakingStrategyList.OrderBy(x => random.Next()).FirstOrDefault(),
                MaximumFacilitatorTeamAssignments = 1,
                CreateAssessment = CreateAssessmentTypeList.First(),
                Status = "Draft"
            };
        }

        public static UpdateCampaignRequest GetUpdatedCampaign(int id)
        {
            var random = new Random();
            var guid = Guid.NewGuid();

            return new UpdateCampaignRequest
            {
                Name = $"AT_UpdateCampaign{RandomDataUtil.GetCompanyZipCode()}",
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(15),
                SurveyId = SharedConstants.AtTeamHealth3SurveyId,
                MatchMakingStrategy = MatchMakingStrategyList.OrderBy(x => random.Next()).FirstOrDefault(),
                MaximumFacilitatorTeamAssignments = 2,
                CreateAssessment = CreateAssessmentTypeList.First(),
                Status = "Draft",
                CampaignId = id
            };
        }

        public static CreateFacilitatorRequest GetCompanyFacilitator(string searchName)
        {
            return new CreateFacilitatorRequest
            {
                SearchName = searchName,
                CurrentPage = 1,
                PageSize = 1,
                FacilitatorIds = new List<string> { FacilitatorId }
            };
        }

        public static CreateTeamRequest GetCompanyTeam()
        {
            var random = new Random();

            return new CreateTeamRequest
            {
                SearchTeam = SharedConstants.RadarTeam,
                SearchWorkType = SharedConstants.NewTeamWorkType,
                CurrentPage = 1,
                PageSize = 1,
                TeamIds = TeamIdsList.GetRange(0, 1),
                ParentTeamId = 7132,
                ExcludeTeamIds = TeamIdsList.GetRange(1, 3),
                OrderByColumn = "",
                OrderByDirection = "",
                IsAhf = false,
                SearchTag = SearchTagList.OrderBy(x => random.Next()).FirstOrDefault()
            };
        }

        public static GetAllCampaignTeamIdsRequest GetCompanyTeamsId()
        {
            var random = new Random();

            return new GetAllCampaignTeamIdsRequest
            {
                SearchTeam = SharedConstants.RadarTeam,
                SearchTag = SearchTagList.OrderBy(x => random.Next()).FirstOrDefault(),
                SearchWorkType = SharedConstants.NewTeamWorkType,
                ParentTeamId = 7132,
                ExcludeTeamIds = TeamIdsList.GetRange(1, 3),
                IsAhf = false,
                TeamIds = TeamIdsList.GetRange(0, 1)
            };
        }

        public static GetTeamAssessmentsAllIdsRequest GetCampaignTeamsIds()
        {
            return new GetTeamAssessmentsAllIdsRequest()
            {
                SearchText = SharedConstants.RadarTeam,
                FacilitatorId = FacilitatorId,
                AssessmentStatus = AssessmentStatusList.First()
            };
        }

        public static MatchmakingRequest AutoMatchmakingCampaignData(int campaignId, int numberOfTeams, int noOfFacilitators, int targetRatio, int index = 0)
        {
            var teamIds = TeamIdsList.GetRange(0, numberOfTeams);
            var facilitatorIds = FacilitatorIdsList.GetRange(index, noOfFacilitators);

            return new MatchmakingRequest
            {
                CampaignId = campaignId,
                TeamIds = teamIds,
                FacilitatorIds = facilitatorIds,
                TargetRatio = targetRatio,
                MatchmakingType = "AutoAssignFacilitators"
            };
        }

        public static AddFacilitatorsToCampaignRequest AddFacilitatorsToPublishedCampaign(int campaignId)
        {
            var facilitatorIds = FacilitatorIdsList.GetRange(1, 1);

            return new AddFacilitatorsToCampaignRequest()
            {
                CampaignId = campaignId,
                FacilitatorIds = facilitatorIds
            };
        }

        public static SaveAsDraftRequest SaveAsDraftCampaignData(int campaignId, List<int> numberOfTeams, List<string> noOfFacilitators, Dictionary<string, string> teamFacilitatorMap)
        {
            var guid = Guid.NewGuid();
            var random = new Random();

            return new SaveAsDraftRequest
            {
                CampaignId = campaignId,
                SelectedTeams = numberOfTeams,
                SelectedFacilitators = noOfFacilitators,
                MatchmakingAssignmentsState = "AutoGenerated",
                TeamFacilitatorMap = teamFacilitatorMap,
                AssessmentSettings = new AssessmentSettings
                {
                    Name = $"AT_Campaign_{RandomDataUtil.GetAssessmentName()}",
                    AssessmentType = AssessmentTypesList.OrderBy(x => random.Next()).FirstOrDefault(),

                    TwoMeetingsStakeholderLaunchDate = DateTime.UtcNow.AddDays(1), //One day after the campaign starts
                    TwoMeetingsTeamMemberLaunchDate = DateTime.UtcNow.AddDays(2),
                    TwoMeetingsCloseDate = DateTime.UtcNow.AddDays(3),
                    TwoMeetingsRetrospectiveWindowStart = DateTime.UtcNow.AddDays(5),
                    TwoMeetingsRetrospectiveWindowEnd = DateTime.UtcNow.AddDays(14),  //One day before the campaign ends

                    SingleRetroMeetingAssessmentStartDate = DateTime.UtcNow.AddDays(1), //One day after the campaign starts
                    SingleRetroMeetingAssessmentCloseDate = DateTime.UtcNow.AddDays(4),
                    SingleRetroMeetingRetrospectiveWindowStart = DateTime.UtcNow.AddDays(5),
                    SingleRetroMeetingRetrospectiveWindowEnd = DateTime.UtcNow.AddDays(14),  //One day before the campaign ends

                    SingleCombinedMeetingStakeholderWindowStart = DateTime.UtcNow.AddDays(1), //One day after the campaign starts
                    SingleCombinedMeetingStakeholderWindowEnd = DateTime.UtcNow.AddDays(3),
                    SingleCombinedMeetingRetrospectiveWindowStart = DateTime.UtcNow.AddDays(4),
                    SingleCombinedMeetingTeamMemberLaunchDate = DateTime.UtcNow.AddDays(10),
                    SingleCombinedMeetingRetrospectiveWindowEnd = DateTime.UtcNow.AddDays(14) //One day before the campaign ends
                }
            };
        }

        public static LaunchCampaignRequest LaunchCampaign(int campaignId)
        {
            return new LaunchCampaignRequest
            {
                CampaignId = campaignId,
            };
        }

        public static UpdateAssessmentNameRequest UpdateAssessmentName(string assessmentName,List<int> teamsIds)
        {
            return new UpdateAssessmentNameRequest
            {
                AssessmentName =$"Updated_{assessmentName}", 
                TeamIds = teamsIds
            };
        }
    }
}