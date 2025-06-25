using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.Dtos;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class GetCampaignSetupTests : BaseV1Test
    {
        private static CreateCampaignRequest _request;
        private static CreateCampaignResponse _campaignResponse;
        private static MatchmakingResponse _autoMatchmakingResponse;
        private static MatchmakingRequest _autoMatchmakingRequest;
        private static SaveAsDraftRequest _saveAsDraftRequest;
        private static SaveAsDraftResponse _saveAsDraftResponse;
        public static int NumberOfTeams = 4;
        public static int NumberOfFacilitators = 2;
        public static int TargetRatio = 2;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            // Create new Campaign and Assessment
            var setupApi = new SetupTeardownApi(TestEnvironment);

            _request = CampaignFactory.GetCampaign();
            _campaignResponse = setupApi.CreateCampaign(Company.Id, _request, SiteAdminUser);

            _autoMatchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(_campaignResponse.Id, NumberOfTeams, NumberOfFacilitators, TargetRatio);
            _autoMatchmakingResponse = setupApi.AutoMatchmakingResponse(Company.Id, _campaignResponse.Id,
                _autoMatchmakingRequest, SiteAdminUser);

            _saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(_autoMatchmakingResponse.CampaignId,
                _autoMatchmakingRequest.TeamIds, _autoMatchmakingRequest.FacilitatorIds, _autoMatchmakingResponse.TeamFacilitatorMap);
            _saveAsDraftResponse = setupApi.SetupCampaignResponse(Company.Id, _campaignResponse.Id, _saveAsDraftRequest, SiteAdminUser);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_CampaignSetUp_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.Id));


            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual(_campaignResponse.Id, response.Dto.CampaignId, "Campaign Id doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.MatchmakingAssignmentsState, response.Dto.MatchmakingAssignmentsState, "Matchmaking Assignments State doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.AssessmentType, response.Dto.AssessmentSettings.AssessmentType, "Assessment Type doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.Name, response.Dto.AssessmentSettings.Name, "Campaign Assessment Name doesn't match");

            //Single Combined meeting
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleCombinedMeetingStakeholderWindowStart, response.Dto.AssessmentSettings.SingleCombinedMeetingStakeholderWindowStart, "Single Combined Meeting Stakeholder Window Start doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleCombinedMeetingStakeholderWindowEnd, response.Dto.AssessmentSettings.SingleCombinedMeetingStakeholderWindowEnd, "Single Combined Meeting Stakeholder Window End doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleCombinedMeetingTeamMemberLaunchDate, response.Dto.AssessmentSettings.SingleCombinedMeetingTeamMemberLaunchDate, "Single Combined Meeting Team Member Launch Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleCombinedMeetingRetrospectiveWindowStart, response.Dto.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowStart,
                "Single Combined Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleCombinedMeetingRetrospectiveWindowEnd, response.Dto.AssessmentSettings.SingleCombinedMeetingRetrospectiveWindowEnd, "Single Combined Meeting Retrospective Window End doesn't match");

            //Two meeting
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.TwoMeetingsTeamMemberLaunchDate, response.Dto.AssessmentSettings.TwoMeetingsTeamMemberLaunchDate, "Two Meetings Team Member Launch Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.TwoMeetingsStakeholderLaunchDate, response.Dto.AssessmentSettings.TwoMeetingsStakeholderLaunchDate, "Two Meetings Stakeholder Launch Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.TwoMeetingsCloseDate, response.Dto.AssessmentSettings.TwoMeetingsCloseDate, "Two Meetings Close Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.TwoMeetingsRetrospectiveWindowStart, response.Dto.AssessmentSettings.TwoMeetingsRetrospectiveWindowStart, "Two Meetings Retrospective Window Start doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.TwoMeetingsRetrospectiveWindowEnd, response.Dto.AssessmentSettings.TwoMeetingsRetrospectiveWindowEnd, "Two Meetings Retrospective Window End doesn't match");

            //Single Retro meeting
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleRetroMeetingAssessmentStartDate, response.Dto.AssessmentSettings.SingleRetroMeetingAssessmentStartDate, "Single Retro Meeting Assessment Start Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleRetroMeetingAssessmentCloseDate, response.Dto.AssessmentSettings.SingleRetroMeetingAssessmentCloseDate, "Single Retro Meeting Assessment Close Date doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleRetroMeetingRetrospectiveWindowStart, response.Dto.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowStart, "Single Retro Meeting Retrospective Window Start doesn't match");
            Assert.AreEqual(_saveAsDraftResponse.CampaignAssessment.SingleRetroMeetingRetrospectiveWindowEnd, response.Dto.AssessmentSettings.SingleRetroMeetingRetrospectiveWindowEnd, "Single Retro Meeting Retrospective Window End doesn't match");

            Assert.That.ListsAreEqual(_saveAsDraftResponse.SelectedFacilitators, response.Dto.SelectedFacilitators, "Facilitators list doesn't match");
            Assert.That.ListsAreEqual(_saveAsDraftResponse.SelectedTeams.Select(x => x.ToString()).ToList(), response.Dto.SelectedTeams.Select(x => x.ToString()).ToList(), "Teams list doesn't match");

            CollectionAssert.AreEquivalent(_saveAsDraftResponse.TeamFacilitatorMap, response.Dto.TeamFacilitatorMap,
                "Team Facilitator Map doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_CampaignSetUp_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Setup(SharedConstants.FakeCompanyId, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_CampaignSetUp_WithFakeCampaignId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CampaignId is not found"
            };

            //When
            var response = await client.GetAsync<IList<string>>(RequestUris.CampaignsV2Setup(Company.Id, SharedConstants.FakeCampaignId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignSetUp_Unauthorized()
        {

            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignSetUp_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(SharedConstants.FakeCompanyId, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_CampaignSetUp_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<CampaignSetupResponse>(RequestUris.CampaignsV2Setup(Company.Id, _campaignResponse.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }
    }
}
