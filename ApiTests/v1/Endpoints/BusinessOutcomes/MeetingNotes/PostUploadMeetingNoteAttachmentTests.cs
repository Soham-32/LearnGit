using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
    public class UploadMeetingNoteAttachmentTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_UploadMeetingNoteAttachment_Success()
        {
            var client = await GetAuthenticatedClient();
            var fileName = "TalentDevImport.xlsx";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "TestData", fileName);
            Assert.IsTrue(File.Exists(filePath), $"Test file not found at: {filePath}");

            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
           
            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "File", Path.GetFileName(filePath));
            formData.Add(new StringContent(User.FullName), "Identity.Name");
            formData.Add(new StringContent("true"), "Identity.IsAuthenticated");
            formData.Add(new StringContent(Company.Id.ToString()), "CompanyId");    

            var response = await client.PostAsync(RequestUris.BusinessOutcomeUploadMeetingNotesAttachments, formData);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BusinessOutcomeMeetingNoteAttachmentResponse>(jsonString);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code didn't match");
            Assert.IsNotNull(result, "Response DTO is null");
            Assert.IsTrue(result.Id > 0, "Attachment ID is not valid");
            Assert.AreEqual(User.FullName, result.AuthorName, "AuthorName mismatch");
            Assert.AreEqual(".xlsx", result.FileExtension, "File extension mismatch");
            Assert.AreEqual(fileName, result.FileName, "FileName does not match");
            Assert.IsTrue(result.FileSize > 0, "FileSize is invalid");
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_UploadMeetingNoteAttachment_BadRequests_NoFile()
        {
            var client = await GetAuthenticatedClient();

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(User.FullName), "Identity.Name");
            formData.Add(new StringContent("true"), "Identity.IsAuthenticated");
            formData.Add(new StringContent(Company.Id.ToString()), "CompanyId");

            var response = await client.PostAsync(RequestUris.BusinessOutcomeUploadMeetingNotesAttachments, formData);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_UploadMeetingNoteAttachment_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient();
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "TestData", "TalentDevImport.xlsx");

            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "File", Path.GetFileName(filePath));
            formData.Add(new StringContent(User.FullName), "Identity.Name");
            formData.Add(new StringContent("true"), "Identity.IsAuthenticated");
            formData.Add(new StringContent(Company.Id.ToString()), "CompanyId");

            var response = await client.PostAsync(RequestUris.BusinessOutcomeUploadMeetingNotesAttachments, formData);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }
    }
}