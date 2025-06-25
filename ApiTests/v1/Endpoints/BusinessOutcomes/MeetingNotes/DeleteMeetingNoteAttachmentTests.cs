using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories.BusinessOutcomes.MeetingNotes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.MeetingNotes
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class DeleteMeetingNoteAttachmentTests : BaseV1Test
    {
        private static int _teamId;
        private static BusinessOutcomesMeetingNotesResponse _meetingCard;
        private static int _noteId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;

            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            _meetingCard = new SetupTeardownApi(TestEnvironment).CreateBusinessOutcomesMeetingNotes(request, User);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "TestData", "TalentDevImport.xlsx");
            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "File", Path.GetFileName(filePath));
            formData.Add(new StringContent(User.FullName), "Identity.Name");
            formData.Add(new StringContent("true"), "Identity.IsAuthenticated");
            formData.Add(new StringContent(Company.Id.ToString()), "CompanyId");
            _noteId = new SetupTeardownApi(TestEnvironment).CreateBusinessOutcomeMeetingNoteAttachment(formData, User).Id;
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNoteAttachment_Delete_Success()
        {
            var client = await GetAuthenticatedClient();
            var response = await client.DeleteAsync<bool>(RequestUris.BusinessOutcomeDeleteMeetingNoteAttachments.AddQueryParameter("ids", _noteId));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(response.Dto, "Delete is not successful");
        }

        //[TestMethod] //Bug
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNoteAttachment_Delete_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var firstDelete = await client.DeleteAsync<bool>(RequestUris.BusinessOutcomeDeleteMeetingNoteAttachments.AddQueryParameter("ids", _noteId));
            Assert.AreEqual(HttpStatusCode.OK, firstDelete.StatusCode, "Status Code does not match.");
            Assert.IsTrue(firstDelete.Dto, "Initial delete result was false");

            var secondDelete = await client.DeleteAsync<bool>(RequestUris.BusinessOutcomeDeleteMeetingNoteAttachments.AddQueryParameter("ids", _noteId));
            Assert.AreEqual(HttpStatusCode.NotFound, secondDelete.StatusCode, "MeetingNote Attachment is still present");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNoteAttachment_Delete_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.DeleteAsync<string>(RequestUris.BusinessOutcomeDeleteMeetingNoteAttachments.AddQueryParameter("ids", _noteId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }
    }
}