using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories.BusinessOutcomes.MeetingNotes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.MeetingNotes
{

    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class PatchMeetingNoteActionItemStatusByOwner : BaseV1Test
    {
        private static int _teamId;
        private static BusinessOutcomesMeetingNotesResponse _meetingCard;
        private static int _actionItemId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;           
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            _meetingCard= new SetupTeardownApi(TestEnvironment).CreateBusinessOutcomesMeetingNotes(request, User);
            _actionItemId = _meetingCard.ActionItems.FirstOrDefault()!.Id;
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_ActionItem_Patch_Success()
        {
            var client = await  GetAuthenticatedClient();
            Assert.IsFalse(_meetingCard.ActionItems.FirstOrDefault() is { IsCompleted: true }, "By Default status is true");

            var request = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = _actionItemId,
                IsCompleted = true
            };

            var response = await client.PatchAsync<bool>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,"Status code didn't match");
            Assert.IsTrue(response.Dto, "Response is not updated");
        }

        //[TestMethod] Bug
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_ActionItem_Patch_Success_InComplete()
        {
            var client = await GetAuthenticatedClient();

            // Mark it completed first
            var completeRequest = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = _actionItemId,
                IsCompleted = true
            };
            await client.PatchAsync<bool>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, completeRequest);

            // Call again with same status
            var repeatRequest = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = _actionItemId,
                IsCompleted = false
            };

            var response = await client.PatchAsync<bool>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, repeatRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code didn't match");
            Assert.IsFalse(response.Dto, "Action item should remain completed");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_ActionItem_Patch_NotFound_InvalidActionItemId()
        {
            var client = await GetAuthenticatedClient();
            var request = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = -9999,
                IsCompleted = true
            };

            var response = await client.PatchAsync<object>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code didn't match");
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_ActionItem_Patch_NotFound_NoActionItemId()
        {
            var client = await GetAuthenticatedClient();
            var request = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = 0,
                IsCompleted = true
            };

            var response = await client.PatchAsync<object>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_ActionItem_Patch_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient();
            var request = new UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
            {
                ActionItemId = _actionItemId,
                IsCompleted = true
            };

            var response = await client.PatchAsync<string>(RequestUris.BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner, request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }
    }
}
