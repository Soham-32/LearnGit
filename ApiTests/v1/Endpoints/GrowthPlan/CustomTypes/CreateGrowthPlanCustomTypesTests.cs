using AtCommon.Utilities;
using System.Linq;
using System.Net;
using AtCommon.Api;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using System.Collections.Generic;
using AtCommon.Dtos.GrowthPlan;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan.CustomTypes
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class CreateGrowthPlanCustomTypesTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_Valid_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);

            // When
            var response = await client.PutAsync<string>(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match");
            Assert.IsTrue(response.Dto.Equals("true"), "Response is false");

            var customText = customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).First();
            var getGrowthItemLists = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));
            getGrowthItemLists.EnsureSuccess();
            Assert.IsTrue(getGrowthItemLists.Dto.Any(x => x.CustomText.Equals(customText)), $"Custom Type {customText} is not present");
        }

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_Multiple_CustomTypes_Put_Valid_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id, 4);

            // When
            var response = await client.PutAsync<string>(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match");
            Assert.IsTrue(response.Dto.Equals("true"), "Response is false");

            var expectedGrowthItemCustomTexts = customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).ToList();
            var actualGrowthItemTypes = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));
            actualGrowthItemTypes.EnsureSuccess();
            var actualGrowthItemCustomTexts = actualGrowthItemTypes.Dto.Select(a => a.CustomText).ToList();
            foreach (var customType in expectedGrowthItemCustomTexts)
            {
                Assert.That.ListContains(actualGrowthItemCustomTexts, customType, $"{customType} item is not present in custom types list");
            }
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(0);

            // When
            var response = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(SharedConstants.FakeCompanyId);

            // When
            var response = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task GrowthPlan_CustomTypes_Put_NonAuthorized_User_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);

            // When
            var response = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_Conflict()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);
            var firstResponse = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());
            firstResponse.EnsureSuccessStatusCode();
            // When 
            var secondResponse = await client.PutAsync<string>(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest);
            // Then
            Assert.AreEqual(HttpStatusCode.Conflict, secondResponse.StatusCode, "Status code doesn't match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_UnAuthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);

            // When
            var response = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match");
        }
    }
}   