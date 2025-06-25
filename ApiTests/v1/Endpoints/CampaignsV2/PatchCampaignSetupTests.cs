using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class PatchCampaignSetupTests : BaseV1Test
    {
        private static CreateCampaignRequest _request;
        private static CreateCampaignResponse _campaignResponse;
        private static MatchmakingResponse _autoMatchmakingResponse;
        private static MatchmakingRequest _autoMatchmakingRequest;
        public static int NumberOfTeams = 4;
        public static int NumberOfFacilitators = 2;
        public static int TargetRatio = 2;

        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {

            // Create new campaign and Auto matches
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateCampaign(Company.Id, _request, SiteAdminUser);

            _autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams,
                NumberOfFacilitators, TargetRatio);
            _autoMatchmakingResponse = setupApi.AutoMatchmakingResponse(Company.Id,
                _campaignResponse.Id, _autoMatchmakingRequest, SiteAdminUser);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_CampaignSetUp_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId, _autoMatchmakingRequest.TeamIds
                , _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<SaveAsDraftResponse>(RequestUris.CampaignsV2Setup(Company.Id, _autoMatchmakingResponse.CampaignId), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, setupResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(_campaignResponse.Id, setupResponse.Dto.CampaignId, "Campaign Id doesn't match");
            Assert.AreEqual(saveAsDraftRequest.MatchmakingAssignmentsState, setupResponse.Dto.MatchmakingAssignmentsState, "Matchmaking Assignments State doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.AssessmentType, setupResponse.Dto.CampaignAssessment.AssessmentType, "Assessment Type doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.Name, setupResponse.Dto.CampaignAssessment.Name, "Campaign Assessment Name doesn't match");

            //Single Combined meeting
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowStart, setupResponse.Dto.CampaignAssessment.SingleCombinedMeetingStakeholderWindowStart, " Single Combined Meeting Stakeholder Window Start doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowEnd, setupResponse.Dto.CampaignAssessment.SingleCombinedMeetingStakeholderWindowEnd, " Single Combined Meeting Stakeholder Window End doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleCombinedMeetingTeamMemberLaunchDate, setupResponse.Dto.CampaignAssessment.SingleCombinedMeetingTeamMemberLaunchDate, " Single Combined Meeting Team Member Launch Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowStart, setupResponse.Dto.CampaignAssessment.SingleCombinedMeetingRetrospectiveWindowStart,
                "Single Combined Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowEnd, setupResponse.Dto.CampaignAssessment.SingleCombinedMeetingRetrospectiveWindowEnd, " Single Combined Meeting Retrospective Window End doesn't match");

            //Two meeting
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.TwoMeetingsTeamMemberLaunchDate, setupResponse.Dto.CampaignAssessment.TwoMeetingsTeamMemberLaunchDate, "Two Meetings Team Member Launch Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.TwoMeetingsStakeholderLaunchDate, setupResponse.Dto.CampaignAssessment.TwoMeetingsStakeholderLaunchDate, "Two Meetings Stakeholder Launch Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.TwoMeetingsCloseDate, setupResponse.Dto.CampaignAssessment.TwoMeetingsCloseDate, "Two Meetings Close Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowStart, setupResponse.Dto.CampaignAssessment.TwoMeetingsRetrospectiveWindowStart, "Two Meetings Retrospective Window Start doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowEnd, setupResponse.Dto.CampaignAssessment.TwoMeetingsRetrospectiveWindowEnd, "Two Meetings Retrospective Window End doesn't match");

            //Single Retro meeting
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleRetroMeetingAssessmentStartDate, setupResponse.Dto.CampaignAssessment.SingleRetroMeetingAssessmentStartDate, "Single Retro Meeting Assessment Start Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleRetroMeetingAssessmentCloseDate, setupResponse.Dto.CampaignAssessment.SingleRetroMeetingAssessmentCloseDate, "Single Retro Meeting Assessment Close Date doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowStart, setupResponse.Dto.CampaignAssessment.SingleRetroMeetingRetrospectiveWindowStart, "Single Retro Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(saveAsDraftRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowEnd, setupResponse.Dto.CampaignAssessment.SingleRetroMeetingRetrospectiveWindowEnd, "Single Retro Meeting Retrospective Window End doesn't match");

            Assert.That.ListsAreEqual(saveAsDraftRequest.SelectedFacilitators, setupResponse.Dto.SelectedFacilitators, "Facilitators list doesn't match");
            Assert.That.ListsAreEqual(saveAsDraftRequest.SelectedTeams.Select(x => x.ToString()).ToList(), setupResponse.Dto.SelectedTeams.Select(x => x.ToString()).ToList(), "Teams list doesn't match");
            CollectionAssert.AreEquivalent(saveAsDraftRequest.TeamFacilitatorMap, setupResponse.Dto.TeamFacilitatorMap,
                "Team Facilitator Map doesn't match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Patch_CampaignSetUp_NullRequestParameter_OK()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var updatedCampaignSetupRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId, null, null, null);
            updatedCampaignSetupRequest.MatchmakingAssignmentsState = "";
            updatedCampaignSetupRequest.AssessmentSettings.Name = "";
            updatedCampaignSetupRequest.AssessmentSettings.AssessmentType = "";
            updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsStakeholderLaunchDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsTeamMemberLaunchDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsCloseDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowStart = null;
            updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowEnd = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingAssessmentStartDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingAssessmentCloseDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowStart = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowEnd = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowStart = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowEnd = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowStart = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingTeamMemberLaunchDate = null;
            updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowEnd = null;

            //When
            var updatedCampaignResponse = await client.PatchAsync<EmptyAndNullErrorList>(RequestUris.CampaignsV2Setup(Company.Id, _autoMatchmakingResponse.CampaignId), updatedCampaignSetupRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, updatedCampaignResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(null, updatedCampaignResponse.Dto.AssessmentSettingsassessmentType, "Assessment Type doesn't match");
            Assert.AreEqual(null, updatedCampaignResponse.Dto.AssessmentSettingsname, "Assessment Name doesn't match");
            Assert.AreEqual(null, updatedCampaignResponse.Dto.MatchmakingAssignmentsState, "Match making Assignments State doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.SelectedTeams, updatedCampaignResponse.Dto.SelectedTeams, "Selected Teams doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.SelectedFacilitators, updatedCampaignResponse.Dto.SelectedFacilitators, "Selected Facilitators doesn't match");

            //Single Combined Meetings
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowStart, updatedCampaignResponse.Dto.AssessmentSettingssingleCombinedMeetingRetrospectiveWindowStart, "Single Combined Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowEnd, updatedCampaignResponse.Dto.AssessmentSettingssingleCombinedMeetingRetrospectiveWindowEnd, "Single Combined Meeting Retrospective Window End doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowStart, updatedCampaignResponse.Dto.AssessmentSettingssingleCombinedMeetingStakeholderWindowStart, "Single Combined Meeting Stakeholder Window Start doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingStakeholderWindowEnd, updatedCampaignResponse.Dto.AssessmentSettingssingleCombinedMeetingStakeholderWindowEnd, "Single Combined Meeting Stakeholder Window End doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleCombinedMeetingTeamMemberLaunchDate, updatedCampaignResponse.Dto.AssessmentSettingssingleCombinedMeetingTeamMemberLaunchDate, "Single Combined Meeting Team Member Launch Date doesn't match");

            //Two Meetings
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsStakeholderLaunchDate, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsStakeholderLaunchDate, "Two Meetings Stakeholder Launch Date doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsCloseDate, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsCloseDate, "Two Meetings Close Date doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowStart, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsRetrospectiveWindowStart, "Two Meetings Retrospective Window Start doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsRetrospectiveWindowEnd, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsRetrospectiveWindowEnd, "Two Meetings Retrospective Window End doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.TwoMeetingsTeamMemberLaunchDate, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsTeamMemberLaunchDate, "Two Meetings Team Member Launch Date doesn't match");

            //Single Retro Meetings
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingAssessmentStartDate, updatedCampaignResponse.Dto.AssessmentSettingssingleRetroMeetingAssessmentStartDate, "Single Retro Meeting Assessment Start Date doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingAssessmentCloseDate, updatedCampaignResponse.Dto.AssessmentSettingssingleRetroMeetingAssessmentCloseDate, "Single Retro Meeting Assessment Close Date doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowStart, updatedCampaignResponse.Dto.AssessmentSettingstwoMeetingsRetrospectiveWindowStart, "Single Retro Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(updatedCampaignSetupRequest.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowEnd, updatedCampaignResponse.Dto.AssessmentSettingssingleRetroMeetingRetrospectiveWindowEnd, "Single Retro Meeting Retrospective Window End doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignSetUp_WithOut_CampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
                {
                    "'Campaign Id' must not be empty.",
                    "CampaignId is not found."
                };

            var updatedCampaignSetupRequest = CampaignFactory.SaveAsDraftCampaignData(0,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<IList<string>>(RequestUris.CampaignsV2Setup(Company.Id, 0), updatedCampaignSetupRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, setupResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, setupResponse.Dto.ToList(), "Error message does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignSetUp_Invalid_CampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<string>(RequestUris.CampaignsV2Setup(Company.Id, -1), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, setupResponse.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("Invalid Request", setupResponse.Dto, "Invalid message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Patch_CampaignSetUp_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {  
                "CompanyId is not found"
            };

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<IList<string>>(RequestUris.CampaignsV2Setup(SharedConstants.FakeCompanyId, _campaignResponse.Id), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, setupResponse.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, setupResponse.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignSetUp_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<SaveAsDraftResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.Id), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, setupResponse.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignSetUp_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<IList<string>>(RequestUris.CampaignsV2Setup(SharedConstants.FakeCompanyId, _campaignResponse.Id), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, setupResponse.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Patch_CampaignSetUp_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);

            //When
            var setupResponse = await client.PatchAsync<SaveAsDraftResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.Id), saveAsDraftRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, setupResponse.StatusCode, "Status Code doesn't match.");
        }

    }
}
