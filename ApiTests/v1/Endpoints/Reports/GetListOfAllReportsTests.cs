using AtCommon.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Dtos.Reports;
using AtCommon.ObjectFactories;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetListOfAllReportsTests : BaseV1Test
    {
        //200
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Reports_Get_AllReportsList_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var reports = ReportsFactory.GetReportList();

            //When
            var response = client.GetAsync<IList<ReportListResponse>>(RequestUris.ReportsList()).GetAwaiter().GetResult();

            var reportsIds = response.Dto.Select(report => report.Id).ToList();
            var reportsNames = response.Dto.Select(report => report.Name).ToList();

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");

            foreach (var report in reports)
            {
                Assert.IsTrue(reportsIds.Contains(report.Id), $"Report list does not contain <{report.Id}>.");
                Assert.IsTrue(reportsNames.Contains(report.Name), $"Report list does not contain <{report.Name}>.");
            }
        }

        //200
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), 
         TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Reports_Get_AllReportsList_NotAssignedToUser_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = client.GetAsync<IList<ReportListResponse>>(RequestUris.ReportsList()).GetAwaiter().GetResult();

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.All(report => string.IsNullOrEmpty(report.Name)), "'Name' field does not empty or null.");
        }

        //401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"),
         TestCategory("SiteAdmin"),TestCategory("PartnerAdmin")]
        public async Task Reports_Get_AllReportsList_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync(RequestUris.ReportsList());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}

