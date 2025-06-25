using AtCommon.Api;
using AtCommon.Dtos.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Dtos.Companies;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetGrowthPlanItemsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentsGrowthPlanItems_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id.ToString();

            //act
            var response = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(
                RequestUris.ReportsGrowthPlanItems().AddQueryParameter("companyId", companyId));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto != null, "Response body should not be empty");
            Assert.IsTrue(response.Dto.Any(dto => dto.CompanyId.ToString() == companyId), "The company ids do not match");
            Assert.IsTrue(response.Dto.Any(dto => dto.Email != null), "Email is null");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Reports_Get_IndividualAssessmentsGrowthPlanItems_ForbiddenCompany()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            const string companyId = "115";

            //act
            var response = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(
                RequestUris.ReportsGrowthPlanItems().AddQueryParameter("companyId", companyId));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto == null, "Response body should be empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Reports_Get_IndividualAssessmentsGrowthPlanItems_ForbiddenUser()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id.ToString();

            //act
            var response = await client.GetAsync<IList<IndividualAssessmentGrowthPlanItemsSummaryResponse>>(
                RequestUris.ReportsGrowthPlanItems().AddQueryParameter("companyId", companyId));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto == null, "Response body should be empty");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Reports_Get_IndividualAssessmentsGrowthPlanItems_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            
            //act
            var response = await client.GetAsync(
                RequestUris.ReportsGrowthPlanItems().AddQueryParameter("companyId", Company.Id));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
