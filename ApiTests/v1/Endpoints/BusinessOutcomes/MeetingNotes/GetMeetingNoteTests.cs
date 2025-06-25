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
    public class GetMeetingNoteTests : BaseV1Test
    {
        private static int _teamId;
        private static BusinessOutcomesMeetingNotesResponse _meetingCard;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;           
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            _meetingCard= new SetupTeardownApi(TestEnvironment).CreateBusinessOutcomesMeetingNotes(request, User);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Get_Success()
        {

            var client = await  GetAuthenticatedClient();

            var response = await client.GetAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeGetMeetingNote.AddQueryParameter("id",_meetingCard.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code didn't match");
            Assert.AreEqual(_meetingCard.Title, response.Dto.Title, "Title does not match");
            Assert.AreEqual(_meetingCard.DecisionsDescription, response.Dto.DecisionsDescription, "DecisionsDescription does not match");
            Assert.AreEqual(_meetingCard.AuthorName, response.Dto.AuthorName, "AuthorName does not match");
            Assert.AreEqual(_meetingCard.IsPrivate, response.Dto.IsPrivate, "IsPrivate flag does not match");
            Assert.AreEqual(_meetingCard.MeetingNoteType, response.Dto.MeetingNoteType, "MeetingNoteType does not match");
            Assert.AreEqual(_meetingCard.ScheduledAt.Date, response.Dto.ScheduledAt.Date, "ScheduledAt does not match"); 
            Assert.AreEqual(_meetingCard.MemberUsers.Count, response.Dto.MemberUsers.Count, "MemberUsers count mismatch");
            foreach (var reqMember in _meetingCard.MemberUsers)
            {
                Assert.IsTrue(response.Dto.MemberUsers.Any(m => m.MemberUserId == reqMember.MemberUserId),
                    $"MemberUser with ID '{reqMember.MemberUserId}' not found in response");
            }
       
            Assert.AreEqual(_meetingCard.ActionItems.Count, response.Dto.ActionItems.Count, "ActionItems count mismatch");
            foreach (var reqItem in _meetingCard.ActionItems)
            {
                Assert.IsTrue(response.Dto.ActionItems.Any(a =>
                    a.Description == reqItem.Description &&
                    a.OwnerUserId == reqItem.OwnerUserId &&
                    a.IsCompleted == reqItem.IsCompleted),
                    $"ActionItem with Description '{reqItem.Description}' and OwnerId '{reqItem.OwnerUserId}' not found or mismatched");
            }

            Assert.AreEqual(_meetingCard.Attachments.Count, response.Dto.Attachments.Count, "Attachments count mismatch");
            foreach (var reqAttachment in _meetingCard.Attachments)
            {
                Assert.IsTrue(response.Dto.Attachments.Any(att =>
                    att.LinkUrl == reqAttachment.LinkUrl &&
                    att.AuthorName == reqAttachment.AuthorName),
                    $"Attachment with URL '{reqAttachment.LinkUrl}' and Author '{reqAttachment.AuthorName}' not found or mismatched");
            }
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Get_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeGetMeetingNote.AddQueryParameter("id", _meetingCard.Id));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Get_BadRequest_NoId()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeGetMeetingNote); // No "id" query param

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Get_BadRequest_InvalidId()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeGetMeetingNote.AddQueryParameter("id", "00000000-0000-0000-0000-000000000000"));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
    }
}
