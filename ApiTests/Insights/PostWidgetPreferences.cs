using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Insights;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Insights
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostWidgetPreferences : v1.Endpoints.BaseV1Test
    {

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_WidgetPreferences_Post_Success()
        {
            //given
            var client = await GetInsightsAuthenticatedClient();
            var dashboardResponse = await client.GetAsync<IList<DashboardResponse>>(RequestUris.InsightsDashboards());
            dashboardResponse.EnsureSuccess();
            var teamAgility = dashboardResponse.Dto.FirstOrDefault(d => d.Title == "Team Agility")
                .CheckForNull("Team Agility Dashboard was not found in the response.");

            var preferenceWidget = new WidgetResponse
            {
                Title = "Performance Metrics",
                Type = "performancemetrics"
            };

            var actualWidget = teamAgility.Widgets.FirstOrDefault(w => w.Type == preferenceWidget.Type)
                .CheckForNull($"WidgetType {preferenceWidget.Type} is not found");

            //when
            var updateWidgetPreference = InsightsFactory.GetValidWidgetPreferenceRequest(actualWidget.Uid);
            var response = await client.PostAsync(
                RequestUris.WidgetPreferences(Company.InsightsId), updateWidgetPreference.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match");

        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_WidgetPreferences_Post_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            
            //when
            var updateWidgetPreference = InsightsFactory.GetValidWidgetPreferenceRequest(Guid.Empty);
            var response = await client.PostAsync(
                RequestUris.WidgetPreferences(Company.InsightsId), updateWidgetPreference.ToStringContent());

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match");
        }
    }
}
