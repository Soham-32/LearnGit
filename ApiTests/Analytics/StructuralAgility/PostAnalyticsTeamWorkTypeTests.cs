using System.Collections.Generic;
using System.Linq;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.StructuralAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsTeamWorkTypeTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamWorkType_Post_Success()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var teamWorkType = InsightsFactory.GetValidTeamWorkTypeRequest();
            var response =
                await client.PostAsync<IList<TeamWorkTypeResponse>>(RequestUris.AnalyticsTeamWorkType(Company.InsightsId), teamWorkType);

            var expectedResponse = new List<TeamWorkTypeResponse>()
            {
                new TeamWorkTypeResponse
                {
                    CompanyId = Company.InsightsId,
                    TeamId = 0,
                    WorkType = "Business Operations"
                },
                new TeamWorkTypeResponse
                {
                    CompanyId = Company.InsightsId,
                    TeamId = 0,
                    WorkType = "Service and Support"
                },
                new TeamWorkTypeResponse
                {
                    CompanyId = Company.InsightsId,
                    TeamId = 0,
                    WorkType = "Software Delivery"
                }
            };

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(expectedResponse.Count, response.Dto.Count, "Response list count does not match.");
            foreach (var expected in expectedResponse)
            {
                var actualResult = response.Dto.FirstOrDefault(r => r.WorkType == expected.WorkType)
                    .CheckForNull($"{expected.WorkType} was not found in the response.");
                Assert.AreEqual(expected.CompanyId, actualResult.CompanyId, $"CompanyId is not {expected.CompanyId}.");
                Assert.AreEqual(expected.TeamId, actualResult.TeamId, $"TeamId is not {expected.TeamId}.");
                Assert.AreEqual(expected.WorkType, actualResult.WorkType, $"WorkType is not {expected.WorkType}.");
            }
            
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamWorkType_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var teamWorkType = InsightsFactory.GetValidTeamWorkTypeRequest();
            var response =
                await client.PostAsync<TeamWorkTypeResponse>(RequestUris.AnalyticsTeamWorkType(Company.InsightsId), teamWorkType);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_TeamWorkType_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var teamWorkType = InsightsFactory.GetValidTeamWorkTypeRequest();
            var response =
                await client.PostAsync<TeamWorkTypeResponse>(RequestUris.AnalyticsTeamWorkType(SharedConstants.FakeCompanyId), teamWorkType);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }
    }
}
