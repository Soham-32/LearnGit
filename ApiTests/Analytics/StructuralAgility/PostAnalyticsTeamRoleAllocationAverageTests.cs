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
    public class PostAnalyticsTeamRoleAllocationAverageTests : BaseV1Test
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
        public async Task Analytics_TeamRoleAllocationAverage_Post_Company_Success()
        {
            var request = InsightsFactory.GetTeamRoleAllocationAverageRequest(0, new List<int> { 0 });
            var expectedResponse = InsightsFactory.GetTeamRoleAllocationAverageResponse();

            await TeamRoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamRoleAllocationAverage_Post_Enterprise_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetTeamRoleAllocationAverageRequest(_enterpriseTeam.TeamId, new List<int> { 0 });
            var expectedResponse = InsightsFactory.GetTeamRoleAllocationAverageResponse();

            await TeamRoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamRoleAllocationAverage_Post_MultiTeam_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetTeamRoleAllocationAverageRequest(_multiTeam.TeamId, _multiTeam.ParentIds);
            var expectedResponse = InsightsFactory.GetTeamRoleAllocationAverageTeamResponse();

            await TeamRoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamRoleAllocationAverage_Post_Team_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetTeamRoleAllocationAverageRequest(_team.TeamId, _team.ParentIds);
            var expectedResponse = InsightsFactory.GetTeamRoleAllocationAverageTeamResponse();

            await TeamRoleAllocationAverageValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamRoleAllocationAverage_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var roleAverageRequest = InsightsFactory.GetTeamRoleAllocationAverageRequest(0, new List<int> { 0 });
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsRoleAllocationAverage(Company.InsightsId), roleAverageRequest);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamRoleAllocationAverage_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var roleAverageRequest = InsightsFactory.GetTeamRoleAllocationAverageRequest(0, new List<int> { 0 });
            var response = await client.PostAsync<IList<string>>(RequestUris.AnalyticsRoleAllocationAverage(SharedConstants.FakeCompanyId), roleAverageRequest);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        private async Task TeamRoleAllocationAverageValidator(TeamRoleAllocationAverageRequest request,
            AnalyticsTableResponse<TeamRoleAllocationAverageResponse> expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.PostAsync<AnalyticsTableResponse<TeamRoleAllocationAverageResponse>>(
                RequestUris.AnalyticsTeamRoleAllocationAverage(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.Table.Count, response.Dto.Table.Count, "The RoleAllocationAverageResponse List does not have the correct count.");
            foreach (var item in expectedResponse.Table)
            {
                var actualResult = response.Dto.Table.FirstOrDefault(r => r.MemberRole == item.MemberRole && r.TeamName == item.TeamName) ??
                                   throw new Exception($"{item.MemberRole} MemberRole was not found in the response.");
                Assert.AreEqual(item.WorkType, actualResult.WorkType, $"{item.WorkType} WorkType does not match.");
                Assert.AreEqual(item.MemberRole, actualResult.MemberRole, $"{item.MemberRole} MemberRole does not match.");
                Assert.AreEqual(item.TeamName, actualResult.TeamName, $"{item.TeamName} TeamName does not match.");
                Assert.AreEqual(item.TargetAllocation, actualResult.TargetAllocation, $"{item.TargetAllocation} TargetAllocation does not match.");
                Assert.AreEqual(item.ActualAllocation, actualResult.ActualAllocation, $"{item.ActualAllocation} ActualAllocation does not match.");
            }
        }
    }
}
