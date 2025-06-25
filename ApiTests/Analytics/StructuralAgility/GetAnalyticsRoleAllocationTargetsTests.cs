using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class GetAnalyticsRoleAllocationTargetsTests : BaseV1Test
    {
        private const int TeamId = 0;

        [TestMethod]
        [TestCategory("KnownDefect")]  // Bug Id: 38148
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Get_Success()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();


            // when
            var response =
                await client.GetAsync<IList<RoleAllocationTargetsResponse>>(
                    RequestUris.AnalyticsRoleAllocationTargets(Company.InsightsId, TeamId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");

            var workTypes = InsightsFactory.InsightsTeams.Select(t => t.WorkType).Distinct().ToList();

            foreach (var item in response.Dto)
            {
                Assert.IsTrue(item.Uid.CompareTo(new Guid()) != 0, "A valid Guid does not exist");
                Assert.IsTrue(item.RoleAllocationTargetId > 0,
                    $"RoleAllocationTargetId is not > 0.");
                Assert.AreEqual(Company.InsightsId, item.CompanyId, $"CompanyId is not {item.CompanyId}.");
                Assert.AreEqual(TeamId, item.TeamId, $"TeamId is not {item.TeamId}.");
                Assert.IsTrue(workTypes.Contains(item.WorkType), $"<{item.WorkType}> is not used by this company.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(item.Role), $"Role is empty or null");
                Assert.AreEqual(1,item.Allocation, $"Allocation is not {item.Allocation}.");
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response =
                await client.GetAsync(RequestUris.AnalyticsRoleAllocationTargets(Company.InsightsId, TeamId));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_RoleAllocationTargets_Get_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.GetAsync(
                RequestUris.AnalyticsRoleAllocationTargets(SharedConstants.FakeCompanyId, TeamId));
            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }
    }
}
