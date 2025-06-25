using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsRoleAllocationAverageTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var user = User.IsSiteAdmin() || User.IsPartnerAdmin() ? User : InsightsUser;
                var companyHierarchy = setup.GetCompanyHierarchy(Company.InsightsId, user);

                _enterpriseTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsEnterpriseTeam1);
                _multiTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam1);
                _team = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_Company_Success()
        {
            var request = InsightsFactory.GetRoleAllocationAverageRequest(0, new List<int> { 0 });
            var expectedResponse = InsightsFactory.GetRoleAllocationAverageResponse();
            expectedResponse.Table.ForEach(m =>
            {
                m.TeamsSupportedCount *= 2;
            });

            await RoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_Enterprise_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetRoleAllocationAverageRequest(_enterpriseTeam.TeamId, new List<int> { 0 });
            var expectedResponse = InsightsFactory.GetRoleAllocationAverageResponse();
            expectedResponse.Table.ForEach(m =>
            {
                m.TeamsSupportedCount *= 2;
            });

            await RoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_MultiTeam_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetRoleAllocationAverageRequest(_multiTeam.TeamId, _multiTeam.ParentIds);
            var expectedResponse = InsightsFactory.GetRoleAllocationAverageResponse();

            await RoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_Team_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetRoleAllocationAverageRequest(_team.TeamId, _team.ParentIds);
            var expectedResponse = InsightsFactory.GetRoleAllocationAverageResponse();

            await RoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var roleAverageRequest = InsightsFactory.GetRoleAllocationAverageRequest(0, new List<int> { 0 });
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsRoleAllocationAverage(Company.InsightsId), roleAverageRequest);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationAverage_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var roleAverageRequest = InsightsFactory.GetRoleAllocationAverageRequest(0, new List<int> { 0 });
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsRoleAllocationAverage(SharedConstants.FakeCompanyId), roleAverageRequest);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        private async Task RoleAllocationAverageValidator(RoleAllocationRequest request,
            AnalyticsTableResponse<RoleAllocationAverageResponse> expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.PostAsync<AnalyticsTableResponse<RoleAllocationAverageResponse>>(
                RequestUris.AnalyticsRoleAllocationAverage(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.Table.Count, response.Dto.Table.Count, "The RoleAllocationAverageResponse List does not have the correct count.");
            foreach (var item in expectedResponse.Table)
            {
                var actualResult = response.Dto.Table.FirstOrDefault(r => r.MemberName == item.MemberName) ??
                                   throw new Exception($"{item.MemberName} MemberName was not found in the response.");
                Assert.AreEqual(item.WorkType, actualResult.WorkType, $"{item.MemberRole} WorkType does not match.");
                Assert.AreEqual(item.MemberRole, actualResult.MemberRole, $"{item.MemberRole} TeamsSupported does not match.");
                Assert.AreEqual(item.MemberName, actualResult.MemberName, $"{item.MemberRole} TeamCount does not match.");
                Assert.AreEqual(item.TeamsSupportedCount, actualResult.TeamsSupportedCount, $"{item.MemberRole} Teams Supported Count does not match.");
                Assert.AreEqual(item.TeamsSupportedTarget, actualResult.TeamsSupportedTarget, $"{item.MemberRole} TeamsSupportedTarget does not match.");
            }
        }
    }
}
