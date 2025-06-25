using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.EditTeams
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_EditTeams")]
    public class EditNTierTeamTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyThatEditEnterpriseTeamAllSteppersLoadedSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);          
            var editProfileBasePage = new EditProfileBasePage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var memberCommonPage = new MemberCommon(Driver, Log);
            var editEtMtSubTeamBasePage = new EditEtMtSubTeamBasePage(Driver, Log);

            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            LoginToProductionEnvironment(env);

            Log.Info($"Navigate to Company dashboard page for company");
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Navigate to Team dashboard page");
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);

            Log.Info("Verify that user is navigated on the Team Profile tab successfully");           
            var teamNames = teamDashboardPage.GetAllTeamsNames("N-Tier Teams");
            if (teamDashboardPage.DoesAnyTeamDisplay()) { return; }
            var firstTeamName = teamNames.First();
            teamDashboardPage.ClickTeamEditButton(firstTeamName);
            var actualTeamProfileTabHeaderTitle = editProfileBasePage.GetTeamProfilePageTitle();
            Assert.AreEqual("Edit " + firstTeamName, actualTeamProfileTabHeaderTitle, $"The Edit Team Profile tab header title does not contain the expected team name. Actual: '{actualTeamProfileTabHeaderTitle}' - {env}");

            Log.Info("Verify that user is navigated on the Sub-Teams tab successfully");
            editTeamBasePage.GoToSubTeamsTab();           
            Assert.AreEqual("Time to edit some sub-teams.", memberCommonPage.GetPageHeaderTitle(), $"The Sub-Teams tab does not loaded successfully - {env}");

            Log.Info("Verify that user is navigated on the Growth Team tab successfully");
            editTeamBasePage.GoToGrowthTeamTab();            
            Assert.AreEqual("Time to edit the growth team.", memberCommonPage.GetPageHeaderTitle(), $"The Growth Team tab does not loaded successfully - {env}");
        }
    }
}
