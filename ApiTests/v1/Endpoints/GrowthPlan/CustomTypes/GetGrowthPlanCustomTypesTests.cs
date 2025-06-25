using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan.CustomTypes
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GetGrowthPlanCustomTypesTests : BaseV1Test
    {
        private static Guid _companyUid;
        private static SaveCustomGrowthPlanTypesRequest _customTypesCreateRequest;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void GetCompanyDetails(TestContext _)
        {
            var user = SiteAdminUser;

            //Getting Company Uid
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var companyResponse =
                client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(Company.Id)).GetAwaiter().GetResult();
            companyResponse.EnsureSuccess();
            _companyUid = companyResponse.Dto.Uid;

            if (!User.IsTeamAdmin() || User.IsBusinessLineAdmin() || User.IsOrganizationalLeader() || User.IsMember())
            {
                user = SiteAdminUser;
            }

            // Create a new custom Type
            _customTypesCreateRequest = GrowthPlanFactory.CustomTypesCreateRequest(Company.Id);
            new SetupTeardownApi(TestEnvironment).CreateGrowthItemCustomType(_customTypesCreateRequest, user);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Get_By_CompanyId_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.All(e => e.CompanyCustomListId >= 0), "Company Custom List Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(e => !string.IsNullOrEmpty(e.CustomText)), "Custom Text is null or empty");

            // Verify custom type 
            var expectedCustomText = _customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).First();
            Assert.IsTrue(response.Dto.Any(x => x.CustomText.Equals(expectedCustomText)), $"Custom Type {expectedCustomText} is not present in list");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Get_By_CompanyId_And_CompanyUid_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();
            var queryString = new Dictionary<string, object>
            {
                {"companyId",  Company.Id},
                {"companyUid",  _companyUid}
            };

            // when
            var response = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter(queryString));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.All(e => e.CompanyCustomListId >= 0), "Company Custom List Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(e => !string.IsNullOrEmpty(e.CustomText)), "Custom Text is null or empty");

            // Verify custom type
            var expectedCustomText = _customTypesCreateRequest.CustomGrowthPlanTypes.Select(a => a.CustomText).First();
            Assert.IsTrue(response.Dto.Any(x => x.CustomText.Equals(expectedCustomText)), $"Custom Type {expectedCustomText} is not present in list");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_CustomTypes_Get_By_CompanyId_InValid_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", 0));

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_Items_Get_By_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }


        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task GrowthPlan_Items_Get_By_NonAuthorized_User_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task GrowthPlan_Items_Get_By_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }
    }
}