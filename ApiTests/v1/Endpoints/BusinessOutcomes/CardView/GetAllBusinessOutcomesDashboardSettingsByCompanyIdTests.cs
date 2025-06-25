using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class GetAllBusinessOutcomesDashboardSettingsByCompanyIdTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_DashboardSettings_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync<IList<DashboardSettingResponse>>(
                RequestUris.BusinessOutcomeGetAllBusinessOutcomeDashboardSettingsByCompanyId(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code didn't match");
            Assert.IsTrue(response.Dto.Any(), "The response body is empty");
            Assert.AreEqual(3, response.Dto.Count, "Unexpected number of settings returned");

            var expectedSettings = new[] {

             new { Name = "DisplayCompanyLevelOutcomes", DisplayName = "Display Company Level Outcomes on Every Level", Description = "Company Level 3 Year and 1 Year outcomes display at the Teams level as View Only Items", InputType = "bool", Value = "1" },
             new { Name = "FilterDataEitherByEitherAndOrOr", DisplayName = "Display the business outcome filtered by either \"And\" (combines all filters) or \"Or\" (either filters are applied)", Description = "Display the business outcome filtered by either \"And\" (combines all filters) or \"Or\" (either filters are applied)", InputType = "bool", Value = "0" },
             new { Name = "EnableQualityAlignmentScore", DisplayName = "Enable Alignment and Quality Score", Description = "Enable Alignment and Quality Score", InputType = "bool", Value = "0" }
              };

            for (var i = 0; i < response.Dto.Count; i++)
            {
                var actual = response.Dto[i];
                var expected = expectedSettings[i];
                Assert.AreEqual(Company.Id, actual.CompanyId, $"CompanyId does not match at index {i}");
                Assert.AreEqual(expected.Name, actual.Name, $"Name does not match at index {i}");
                Assert.AreEqual(expected.DisplayName, actual.DisplayName, $"DisplayName does not match at index {i}");
                Assert.AreEqual(expected.Description, actual.Description, $"Description does not match at index {i}");
                Assert.AreEqual(expected.InputType, actual.InputType, $"InputType does not match at index {i}");
                Assert.AreEqual(expected.Value, actual.Value, $"Value does not match at index {i}");
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_DashboardSettings_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.BusinessOutcomeGetAllBusinessOutcomeDashboardSettingsByCompanyId(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_DashboardSettings_Get_Forbidden()
        {

            //arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.BusinessOutcomeGetAllBusinessOutcomeDashboardSettingsByCompanyId(SharedConstants.FakeCompanyId));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code didn't match");
        }
    }
}