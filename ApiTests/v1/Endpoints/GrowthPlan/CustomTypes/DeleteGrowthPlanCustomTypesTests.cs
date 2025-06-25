using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan.CustomTypes
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class DeleteGrowthPlanCustomTypesTests : BaseV1Test
    {
        private static int _customTypeId;
        private static SaveCustomGrowthPlanTypesRequest _customTypesCreateRequest;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void GetCustomTypeDetails(TestContext testContext)
        {
            var user = User;
            if (User.IsTeamAdmin() || User.IsBusinessLineAdmin() || User.IsOrganizationalLeader() || User.IsMember())
            {
                user = SiteAdminUser;
            }

            //Creating Growth Plan Custom Type & getting Id for newly created Custom Type
            _customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);
            new SetupTeardownApi(TestEnvironment).CreateGrowthItemCustomType(_customTypesCreateRequest, user);
            var getCustomTypesLists = new SetupTeardownApi(TestEnvironment).GetGrowthItemCustomType(Company.Id, user);
  
            var customText = _customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).First();
            _customTypeId = getCustomTypesLists.Result.First(x => x.CustomText.Equals(customText)).CompanyCustomListId;
        }

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Delete_Valid_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesDeleteRequest = GrowthPlanFactory.CustomTypesDeleteRequest(Company.Id, new List<int> { _customTypeId });

            // When
            var response = await client.PostAsync<string>(RequestUris.GrowthPlanGetCustomDelete(), customTypesDeleteRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match");
            Assert.IsTrue(response.Dto.Equals("true"), "Response is false");

            var customTypesLists = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));
            Assert.IsFalse(customTypesLists.Dto.Any(x => x.CompanyCustomListId.Equals(_customTypeId)), "Deleted Custom type is still present");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Delete_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesDeleteRequest = GrowthPlanFactory.CustomTypesDeleteRequest(0, new List<int> { _customTypeId });

            // When
            var response = await client.PostAsync(RequestUris.GrowthPlanGetCustomDelete(), customTypesDeleteRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Delete_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesDeleteRequest = GrowthPlanFactory.CustomTypesDeleteRequest(SharedConstants.FakeCompanyId, new List<int> { 0 });

            // When
            var response = await client.PostAsync(RequestUris.GrowthPlanGetCustomDelete(), customTypesDeleteRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task GrowthPlan_CustomTypes_Delete_By_NonAuthorized_User_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var customTypesDeleteRequest = GrowthPlanFactory.CustomTypesDeleteRequest(Company.Id, new List<int> { _customTypeId });

            // When
            var response = await client.PostAsync(RequestUris.GrowthPlanGetCustomDelete(), customTypesDeleteRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Delete_UnAuthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();
            var customTypesDeleteRequest =
                GrowthPlanFactory.CustomTypesDeleteRequest(Company.Id, new List<int> { _customTypeId });

            // When
            var response = await client.PostAsync(RequestUris.GrowthPlanGetCustomDelete(),
                customTypesDeleteRequest.ToStringContent());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match");
        }

    }
}