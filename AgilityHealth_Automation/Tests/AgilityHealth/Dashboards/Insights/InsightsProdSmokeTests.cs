using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using AtCommon.Dtos;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("Dashboard")]
    public class InsightsProdSmokeTests : BaseTest
    {

        private static string Group => TestEnvironment.Parameters["ProdGroup"].ToString().ToUpper();
        private static string AzureDevOpsApiKey => TestEnvironment.Parameters["AzureDevOpsApiKey"].ToString();
        private static User ProdUser => TestEnvironment.UserConfig.GetUserByDescription("insights prod");

        public static IEnumerable<object[]> FilteredEnvironments
        {
            get
            {
                return AzureDevOpsApi
                    .GetProductionEnvironments(AzureDevOpsApiKey).GetAwaiter().GetResult()
                    .Where(e => e.IsActive && e.Group.ToUpper() == Group && e.IsInsightsEnabled)
                    .Select(e => new object[] { e.Name, e });
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(FilteredEnvironments))]
        public void Insights_Ui_ProductionSmokeTest(string _, ProductionEnvironment environment)
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboard = new CompanyDashboardPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);
            var structuralAgility = new StructuralAgilityPage(Driver, Log);

            login.NavigateToUrl(environment.LoginUrl.AbsoluteUri);
            login.LoginToApplication(ProdUser.Username, ProdUser.Password);
            companyDashboard.WaitUntilLoaded();

            teamAgilityPage.NavigateToUrl($"https://{environment.Environment}.agilityinsights.ai/V2/insights");
            teamAgilityPage.WaitUntilWidgetsLoaded();
            
            teamAgilityPage.ClickOnStructuralAgilityTab(120);

            foreach (var widget in StructuralAgilityWidgets.GetList())
            {
                Assert.IsTrue(structuralAgility.IsWidgetDisplayed(widget), $"<{widget.Title}> is not displayed");
            }

        }

    }
}