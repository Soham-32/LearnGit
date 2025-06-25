using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class GetAllCustomFieldsByCompanyIdTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_CustomFields_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // Create Custom Field
            var request = new List<CustomFieldRequest> { BusinessOutcomesFactory.GetCustomFieldRequest() };
            var customFieldResponse = await client.PostAsync<IList<CustomFieldResponse>>(
                RequestUris.BusinessOutcomesCreateCustomFields(Company.Id), request);
            // assert
            Assert.AreEqual(HttpStatusCode.OK, customFieldResponse.StatusCode, "Status code didn't match");

            // act
            var response = await client.GetAsync<IList<CustomFieldResponse>>(RequestUris.BusinessOutcomeGetAllCustomFieldsByCompanyId(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code didn't match");
            Assert.IsTrue(response.Dto.Any(), "The response body is empty");

            var actual = response.Dto[0];

            Assert.AreEqual(Company.Id, actual.CompanyId, "CompanyId mismatch");
            Assert.AreEqual(request.First().Name, actual.Name, "Name mismatch");
            Assert.IsNotNull(response.Dto.First().Uid, "Uid is null.");
            Assert.AreEqual(0, response.Dto.First().Order, "Order does not match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_CustomFields_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.BusinessOutcomeGetAllCustomFieldsByCompanyId(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_CustomFields_Get_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(
                RequestUris.BusinessOutcomeGetAllCustomFieldsByCompanyId(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }
    }
}