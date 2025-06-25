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
    public class PostAnalyticsTeamStabilityTests : BaseV1Test
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
                //_multiTeam.Children.RemoveAt(0);
                _team = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug: 36943
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_TeamStability_Post_Company_Average_OK()
        {
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> {0},
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Summary",
                    TeamId = 0,
                    TeamName = Company.InsightsCompany,
                    TeamType = "Company",
                    TeamCount = 1,
                    OriginalMemberCount = 20,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_TeamStability_Post_Company_Distribution_OK()
        {
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> {0},
                SelectedTeamParents = "0"
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = _enterpriseTeam.TeamId,
                    TeamName = _enterpriseTeam.Name,
                    TeamType = _enterpriseTeam.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 20,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Enterprise_Average_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> {_enterpriseTeam.TeamId},
                SelectedTeamParents = _enterpriseTeam.ParentId.ToString()
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Summary",
                    TeamId = _enterpriseTeam.TeamId,
                    TeamName = _enterpriseTeam.Name,
                    TeamType = _enterpriseTeam.Type,
                    TeamCount = _enterpriseTeam.Children.Select(a=>a.Children.Count).Sum(),
                    OriginalMemberCount = 20,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Enterprise_Distribution_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> {_enterpriseTeam.TeamId},
                SelectedTeamParents = _enterpriseTeam.ParentId.ToString()
            };

            var subTeam1 = _enterpriseTeam.Children.SingleOrDefault(c => c.Name == SharedConstants.InsightsMultiTeam1)
                .CheckForNull($"<{SharedConstants.InsightsMultiTeam1}> was not found in the response.");
            var subTeam2 = _enterpriseTeam.Children.SingleOrDefault(c => c.Name == SharedConstants.InsightsMultiTeam2)
                .CheckForNull($"<{SharedConstants.InsightsMultiTeam2}> was not found in the response.");

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = subTeam1.TeamId,
                    TeamName = subTeam1.Name,
                    TeamType = subTeam1.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 10,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                },
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = subTeam2.TeamId,
                    TeamName = subTeam2.Name,
                    TeamType = subTeam2.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 10,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_MultiTeam_Average_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> {_multiTeam.TeamId},
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Summary",
                    TeamId = _multiTeam.TeamId,
                    TeamName = _multiTeam.Name,
                    TeamType = _multiTeam.Type,
                    TeamCount = _multiTeam.Children.Count,
                    OriginalMemberCount = 10,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_MultiTeam_Distribution_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> {_multiTeam.TeamId},
                SelectedTeamParents = _multiTeam.ParentId.ToString()
            };

            var subTeam1 = _multiTeam.Children.SingleOrDefault(c => c.Name == SharedConstants.InsightsIndividualTeam1)
                .CheckForNull($"<{SharedConstants.InsightsIndividualTeam1}> was not found in the response.");
            var subTeam2 = _multiTeam.Children.SingleOrDefault(c => c.Name == SharedConstants.InsightsIndividualTeam2)
                .CheckForNull($"<{SharedConstants.InsightsIndividualTeam2}> was not found in the response.");

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = subTeam1.TeamId,
                    TeamName = subTeam1.Name,
                    TeamType = subTeam1.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 5,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                },
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = subTeam2.TeamId,
                    TeamName = subTeam2.Name,
                    TeamType = subTeam2.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 5,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Team_Average_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> {_team.TeamId},
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Summary",
                    TeamId = _team.TeamId,
                    TeamName = _team.Name,
                    TeamType = _team.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 5,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Team_Distribution_OK()
        {
            VerifySetup(_classInitFailed);
            var request = new TeamStabilityRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> {_team.TeamId},
                SelectedTeamParents = _team.ParentId.ToString()
            };

            var expectedResponse = new List<TeamStabilityResponse>
            {
                new TeamStabilityResponse
                {
                    WidgetType = "Detail",
                    TeamId = _team.TeamId,
                    TeamName = _team.Name,
                    TeamType = _team.Type,
                    TeamCount = 1,
                    OriginalMemberCount = 5,
                    AddedMemberCount = 0,
                    RemovedMemberCount = 0,
                    TeamStabilityPercentage = 100,
                    StabilityTarget = 80
                }
            };

            await TeamStabilityValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var teamStability = InsightsFactory.GetTeamStabilityRequest();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsTeamStability(Company.InsightsId), teamStability);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var teamStability = InsightsFactory.GetTeamStabilityRequest();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsTeamStability(SharedConstants.FakeCompanyId), teamStability);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamStability_Post_BadRequest()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var teamStability = InsightsFactory.GetTeamStabilityRequest();
            teamStability.WidgetType = StructuralAgilityWidgetType.BadRequest;
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsTeamStability(Company.InsightsId), teamStability);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual($"'Widget Type' has a range of values which does not include '{StructuralAgilityWidgetType.BadRequest:D}'.",
                response.Dto.FirstOrDefault(), "Response error message does not match.");
        }

        private async Task TeamStabilityValidator(TeamStabilityRequest request,
            ICollection<TeamStabilityResponse> expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.PostAsync<IList<TeamStabilityResponse>>(
                RequestUris.AnalyticsTeamStability(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.Count, response.Dto.Count,
                "expectedResponse list count does not match.");

            foreach (var item in expectedResponse)
            {
                var actualResult = response.Dto.FirstOrDefault(r => r.TeamName == item.TeamName)
                    .CheckForNull($"<{item.TeamName}> was not found in the response.");
                Assert.AreEqual(item.TeamId, actualResult.TeamId, "TeamId does not match.");
                Assert.AreEqual(item.TeamType, actualResult.TeamType, "TeamType does not match.");
                Assert.AreEqual(item.TeamCount, actualResult.TeamCount, "TeamCount does not match.");
                Assert.AreEqual(item.OriginalMemberCount, actualResult.OriginalMemberCount,
                    "OriginalMemberCount does not match.");
                Assert.AreEqual(item.AddedMemberCount, actualResult.AddedMemberCount,
                    "AddedMemberCount does not match.");
                Assert.AreEqual(item.RemovedMemberCount, actualResult.RemovedMemberCount,
                    "RemovedMemberCount does not match.");
                Assert.AreEqual(item.TeamStabilityPercentage, actualResult.TeamStabilityPercentage,
                    "TeamStability does not match.");
                Assert.AreEqual(item.StabilityTarget, actualResult.StabilityTarget,
                    "StabilityTarget does not match.");
            }
        }
    }
}