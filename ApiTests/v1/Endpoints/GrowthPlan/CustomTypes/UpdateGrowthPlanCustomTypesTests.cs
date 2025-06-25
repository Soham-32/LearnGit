using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan.CustomTypes
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class UpdateGrowthPlanCustomTypesTests : BaseV1Test
    {
        //204
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Put_Valid_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // Create a new custom type and getting a list of custom types
            var customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);
            var response = await client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), customTypesCreateRequest.ToStringContent());
            response.EnsureSuccessStatusCode();

            var getCustomTypesLists = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));
            getCustomTypesLists.EnsureSuccess();

            //Data for updating custom type
            var firstCustomTypeText = customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).First();
            var firstCustomTypeDetails = getCustomTypesLists.Dto.First(x => x.CustomText.Equals(firstCustomTypeText));
            var customTypesUpdateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id, 1, firstCustomTypeDetails.CompanyCustomListId);
            var updatedCustomText = customTypesUpdateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText = "updatedCustomType"+ RandomDataUtil.GetGrowthPlanTitle()).First();

            //when
            var updatedResponse = await client.PutAsync<string>(RequestUris.GrowthPlanCustomTypesSave(), customTypesUpdateRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.OK, updatedResponse.StatusCode, "Status code doesn't match");
            Assert.IsTrue(updatedResponse.Dto.Equals("true"), "Response is false");
            
            var customTypesLists = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));
            getCustomTypesLists.EnsureSuccess();
            Assert.IsTrue(customTypesLists.Dto.Any(x => x.CustomText.Equals(updatedCustomText)), $"Updated Custom type {updatedCustomText} is not present");
            Assert.IsFalse(customTypesLists.Dto.Any(x => x.CustomText.Equals(firstCustomTypeText)), $"First Custom type {firstCustomTypeText} is present");
        }

    }
}