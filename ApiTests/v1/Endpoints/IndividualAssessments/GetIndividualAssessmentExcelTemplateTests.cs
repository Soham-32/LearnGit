using System.IO;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class GetIndividualAssessmentExcelTemplateTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Get_ExcelTemplate_Success()
        {
            var client = await GetAuthenticatedClient();
            const string fileName = "TalentDevImportTemplate.xlsx";
            var fileUtil = new FileUtil();

            var response = await client.GetAsync(RequestUris.AssessmentIndividualExcelTemplate()
                .AddQueryParameter("companyId", Company.Id));

            fileUtil.DeleteFilesInDownloadFolder(fileName);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fs = new FileStream($@"{fileUtil.GetDownloadPath()}\{fileName}", FileMode.Create);
            await stream.CopyToAsync(fs);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
            Assert.IsTrue(fileUtil.IsFileDownloaded(fileName),
                $"{fileName} not downloaded successfully");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Get_ExcelTemplate_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync(RequestUris.AssessmentIndividualExcelTemplate()
                .AddQueryParameter("companyId", Company.Id));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Response status code does not match. Received a {response.StatusCode}.");
        }
    }
}
