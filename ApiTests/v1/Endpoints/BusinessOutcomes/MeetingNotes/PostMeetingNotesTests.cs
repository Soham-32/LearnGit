using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using AtCommon.ObjectFactories.BusinessOutcomes.MeetingNotes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.Companies;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.MeetingNotes
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class PostMeetingNotesTests : BaseV1Test
    {
        private static int _teamId;
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Post_Success()
        {
            var client = await  GetAuthenticatedClient();
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);

            var response = await client.PostAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeCreateMeetingNotes, request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode,"Status code didn't match");
            Assert.AreEqual(request.Title, response.Dto.Title, "Title does not match");
            Assert.AreEqual(request.DecisionsDescription, response.Dto.DecisionsDescription, "DecisionsDescription does not match");
            Assert.AreEqual(request.AuthorName, response.Dto.AuthorName, "AuthorName does not match");
            Assert.AreEqual(request.TeamId, response.Dto.TeamId, "TeamId does not match");
            Assert.AreEqual(request.IsPrivate, response.Dto.IsPrivate, "IsPrivate flag does not match");
            Assert.AreEqual(request.MeetingNoteType, response.Dto.MeetingNoteType, "MeetingNoteType does not match");
            Assert.AreEqual(request.ScheduledAt.ToString("s"), response.Dto.ScheduledAt.ToString("s"), "ScheduledAt does not match");

            Assert.AreEqual(request.MemberUsers.Count, response.Dto.MemberUsers.Count, "MemberUsers count mismatch");
            foreach (var reqMember in request.MemberUsers)
            {
                Assert.IsTrue(response.Dto.MemberUsers.Any(m => m.MemberUserId == reqMember.MemberId),
                    $"MemberUser with ID '{reqMember.MemberId}' not found in response");
            }

            Assert.AreEqual(request.ActionItems.Count, response.Dto.ActionItems.Count, "ActionItems count mismatch");
            foreach (var reqItem in request.ActionItems)
            {
                Assert.IsTrue(response.Dto.ActionItems.Any(a =>
                    a.Description == reqItem.Description &&
                    a.OwnerUserId == reqItem.OwnerId &&
                    a.IsCompleted == reqItem.IsCompleted),
                    $"ActionItem with Description '{reqItem.Description}' and OwnerId '{reqItem.OwnerId}' not found or mismatched");
            }

            Assert.AreEqual(request.Attachments.Count, response.Dto.Attachments.Count, "Attachments count mismatch");
            foreach (var reqAttachment in request.Attachments)
            {
                Assert.IsTrue(response.Dto.Attachments.Any(att =>
                    att.LinkUrl == reqAttachment.LinkUrl &&
                    att.AuthorName == reqAttachment.AuthorName ),
                    $"Attachment with URL '{reqAttachment.LinkUrl}' and Author '{reqAttachment.AuthorName}' not found or mismatched");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Post_BadRequests_NoTitle()
        {
            var client = await GetAuthenticatedClient();
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            request.Title = null;

            var response = await client.PostAsync<List<string>>(RequestUris.BusinessOutcomeCreateMeetingNotes, request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Post_BadRequests_InValidDate()
        {
            var client = await GetAuthenticatedClient();
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            request.ScheduledAt = DateTime.UtcNow.AddMonths(-20);

            var response = await client.PostAsync<List<string>>(RequestUris.BusinessOutcomeCreateMeetingNotes, request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNote_Post_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient();
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);

            var response = await client.PostAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeCreateMeetingNotes, request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }
    }
}
