using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsPeopleByRoleTests : BaseV1Test
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
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_PeopleByRole_Post_Company_Average_Success()
        {
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 0);
            var expectedResponse = InsightsFactory.GetPeopleByRoleAverageResponse(0);

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_PeopleByRole_Post_Company_Distribution_Success()
        {

            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Distribution, 0);
            var expectedResponse = InsightsFactory.GetPeopleByRoleDistributionResponse(
                0, new List<TeamHierarchyResponse>{ _enterpriseTeam });

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Enterprise_Average_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 
                _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleAverageResponse(_enterpriseTeam.TeamId);

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Enterprise_Distribution_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Distribution, 
                _enterpriseTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleDistributionResponse(
                _enterpriseTeam.TeamId, _enterpriseTeam.Children);

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_MultiTeam_Average_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleAverageResponse(_multiTeam.TeamId);

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_MultiTeam_Distribution_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Distribution, _multiTeam.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleDistributionResponse(
                _multiTeam.TeamId, new List<TeamHierarchyResponse> { _team });

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Team_Average_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, _team.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleAverageResponse(_team.TeamId);

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Team_Distribution_Success()
        {
            VerifySetup(_classInitFailed);
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Distribution, _team.TeamId);
            var expectedResponse = InsightsFactory.GetPeopleByRoleDistributionResponse(
                _team.TeamId, new List<TeamHierarchyResponse> { _team });

            await PeopleByRoleValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 0);
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsPeopleByRole(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 0);
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsPeopleByRole(SharedConstants.FakeCompanyId), request);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_PeopleByRole_Post_BadRequest()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var request = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 0);
            request.WidgetType = StructuralAgilityWidgetType.BadRequest;

            // when
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsPeopleByRole(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual($"'Widget Type' has a range of values which does not include '{StructuralAgilityWidgetType.BadRequest:D}'.",
                response.Dto.FirstOrDefault(), "Error Message does not match.");
        }

        private async Task PeopleByRoleValidator(PeopleByRoleRequest request, PeopleByRoleResponse expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            var response = await client.PostAsync<PeopleByRoleResponse>(
                RequestUris.AnalyticsPeopleByRole(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.HasResults, response.Dto.HasResults, "HasResults does not match.");
            Assert.AreEqual(expectedResponse.Parameters.WidgetType, response.Dto.Parameters.WidgetType, 
                "Parameters.WidgetType does not match.");
            Assert.AreEqual(expectedResponse.Parameters.TeamId, response.Dto.Parameters.TeamId, 
                "Parameters.TeamId does not match.");
            Assert.AreEqual(expectedResponse.Parameters.WorkTypeId, response.Dto.Parameters.WorkTypeId, 
                "Parameters.WorkTypeId does not match.");
            Assert.AreEqual(expectedResponse.TotalPeople, response.Dto.TotalPeople, "TotalPeople does not match.");
            Assert.AreEqual(expectedResponse.Data.Count, response.Dto.Data.Count, 
                "The PeopleByRoleResponse List does not have the correct count.");
            foreach (var item in expectedResponse.Data)
            {
                var actualResult = response.Dto.Data.FirstOrDefault(
                        r => r.MemberRole == item.MemberRole && r.ChildTeamName == item.ChildTeamName)
                    .CheckForNull($"{item.MemberRole} MemberRole was not found in the response.");
                Assert.AreEqual(item.WidgetType, actualResult.WidgetType, 
                    $"ChildTeamName does not match for <{item.MemberRole}>.");
                Assert.AreEqual(item.MemberCount, actualResult.MemberCount, 
                    $"ChildTeamName does not match for <{item.MemberRole}>.");
                Assert.AreEqual(item.MemberRoleRank, actualResult.MemberRoleRank, 
                    $"ChildTeamName does not match for <{item.MemberRole}>.");
                Assert.AreEqual(item.ChildTeamId, actualResult.ChildTeamId, 
                    $"ChildTeamName does not match for <{item.MemberRole}>.");
                Assert.AreEqual(item.ChildTeamName, actualResult.ChildTeamName, 
                    $"ChildTeamName does not match for <{item.MemberRole}>.");
            }
        }
    }
}
