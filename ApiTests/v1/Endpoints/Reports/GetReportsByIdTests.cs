using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Reports;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetReportsByIdTests : BaseV1Test
    {
        private const double RandomId = 9999999999;

        //200
        [TestMethod]
        [TestCategory("SiteAdmin")]

        public async Task Reports_Get_ReportById_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var report = ReportsFactory.GetReportById();

            //When
            var response = await client.GetAsync<IList<ReportsById>>(RequestUris.ReportsById(ReportsFactory.GetReportList().First().Id));

            var teamIds = response.Dto.Select(team => team.TeamId).ToList();
            var teamNames = response.Dto.Select(team => team.TeamName).ToList();
            var companyIds = response.Dto.Select(company => company.CompanyId).ToList();
            var externalIds = response.Dto.Select(team => team.ExternalId).ToList();

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(teamIds.Contains(report.TeamId), $" <{teamIds}> list does not contains <{report.TeamId}> ");
            Assert.IsTrue(teamNames.Contains(report.TeamName), $" <{teamNames}> list does not contains <{report.TeamName}> ");
            Assert.IsTrue(companyIds.Contains(report.CompanyId), $" <{companyIds}> list does not contains <{report.CompanyId}> ");
        }


        //401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"),
         TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Reports_Get_ReportById_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync(RequestUris.ReportsById(ReportsFactory.GetReportList().First().Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member"),
         TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Reports_Get_ReportById_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<IList<ReportsById>>(RequestUris.ReportsById(RandomId));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
