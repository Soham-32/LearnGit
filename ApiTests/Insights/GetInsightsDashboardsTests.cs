using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos.Insights;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiTests.Insights
{
    [TestClass]
    [TestCategory("Insights")]
    public class GetInsightsDashboardsTests : v1.Endpoints.BaseV1Test
    {
        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_Dashboards_Get_Success()
        {
            //given
            var client = await GetInsightsAuthenticatedClient();

            //when
            var response = await client.GetAsync<IList<DashboardResponse>>(RequestUris.InsightsDashboards());

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match.");
            var teamAgility = response.Dto.FirstOrDefault(d => d.Title == "Team Agility") ?? throw new Exception("Team Agility Dashboard was not found in the response.");
            Assert.IsTrue(teamAgility.Uid.CompareTo(new Guid())!=0, "A valid Guid does not exist");
            Assert.AreEqual(12, teamAgility.Widgets.Count, "The widget count does not match");
            //Widget 1
            var expectedWidgets = new List<WidgetResponse>
            {
                new WidgetResponse
                {
                    Title = "Agility Dimensions",
                    Type = "agilitydimensions"
                },
                new WidgetResponse
                {
                    Title = "Performance Metrics",
                    Type = "performancemetrics"
                },
                new WidgetResponse
                {
                    Title = "Performance Over Time",
                    Type = "performanceovertime"
                },
                new WidgetResponse
                {
                    Title = "Agility Index Over Time",
                    Type = "agilityovertime"
                },
                new WidgetResponse
                {
                    Title = "Performance Index",
                    Type = "performanceindex"
                },
                new WidgetResponse
                {
                    Title = "Maturity Over Time",
                    Type = "maturityovertime"
                },
                new WidgetResponse
                {
                    Title = "Maturity Dimensions",
                    Type = "maturitydimensions"
                },
                new WidgetResponse
                {
                    Title = "Maturity Index",
                    Type = "maturityindex"
                },
                new WidgetResponse
                {
                    Title = "Organizational Growth Items",
                    Type = "organizationgi"
                },
                new WidgetResponse
                {
                    Title = "Agility Index",
                    Type = "agilityindex"
                },
                new WidgetResponse
                {
                    Title = "Team Growth Items",
                    Type = "teamgi"
                },
                new WidgetResponse
                {
                    Title = "Enterprise Growth Items",
                    Type = "enterprisegi"
                }
            };

            foreach (var widget in expectedWidgets)
            {
                var actualWidget =  teamAgility.Widgets.FirstOrDefault(w => w.Type == widget.Type) ?? throw new Exception($"WidgetType {widget.Type} is not found");
                Assert.IsTrue(actualWidget.Uid.CompareTo(new Guid()) != 0, "A valid Guid does not exist");
                Assert.IsTrue(actualWidget.WidgetUid.CompareTo(new Guid()) != 0, "A valid Guid does not exist");
                Assert.AreEqual(widget.Title, actualWidget.Title, $"The Title does not match for {widget.Type}");
                Regex.Matches(actualWidget.SubTitle, @"[a-zA-Z]");
                Regex.Matches(actualWidget.Tooltip, @"[a-zA-Z]");
                Assert.IsTrue(actualWidget.DefaultWidth.ToString().Any(char.IsDigit));
                Assert.IsTrue(actualWidget.DefaultHeight.ToString().Any(char.IsDigit));
                Assert.IsTrue(actualWidget.DefaultRow.ToString().Any(char.IsDigit));
                Assert.IsTrue(actualWidget.DefaultColumn.ToString().Any(char.IsDigit));
            }                      
        }

        [TestMethod]
        [TestCategory("Avengers")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Insights_Dashboards_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.InsightsDashboards());

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }
    }
}
