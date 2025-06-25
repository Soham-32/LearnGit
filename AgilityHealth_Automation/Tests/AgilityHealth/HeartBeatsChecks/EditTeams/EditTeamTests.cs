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
    public class EditTeamTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyThatEditTeamAllSteppersLoadedSuccessfully(string env)
        {
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);                       
            var editProfileBasePage = new EditProfileBasePage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var memberCommonPage = new MemberCommon(Driver, Log);           

            var companyName = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyName).ToList().FirstOrDefault();

            LoginToProductionEnvironment(env);

            Log.Info($"Navigate to Company dashboard page for company");
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Navigate to Team dashboard page");
            companyDashboardPage.Search(companyName);
            companyDashboardPage.ClickOnCompanyName(companyName);

            Log.Info("Verify that user is navigated on the Team Profile tab successfully");                                    
            var teamNames = teamDashboardPage.GetAllTeamsNames("Team");
            var SecondTeamName = teamNames[0];
            teamDashboardPage.ClickTeamEditButton(SecondTeamName);
            var actualTeamProfileTabHeaderTitle = editProfileBasePage.GetTeamProfilePageTitle();            
            Assert.AreEqual("Edit " + SecondTeamName, actualTeamProfileTabHeaderTitle, $"The Edit Team Profile tab header title does not contain the expected team name. Actual: '{actualTeamProfileTabHeaderTitle}' - {env}");

            Log.Info("Verify that user is navigated on the Edit Team Members tab successfully");
            editTeamBasePage.GoToTeamMembersTab();            
            Assert.AreEqual("Time to edit some team members.", memberCommonPage.GetPageHeaderTitle(), $"The Team Members tab does not loaded successfully - {env}");

            Log.Info("Verify that user is navigated on the Edit Stakeholders tab successfully");
            editTeamBasePage.GoToStakeHoldersTab();            
            Assert.AreEqual("Time to edit some stakeholders.", memberCommonPage.GetPageHeaderTitle(), $"The Edit Stakeholders tab does not loaded successfully - {env}");

            Log.Info("Verify that user is navigated on the Edit Team Members tab successfully");
            editTeamBasePage.IsMetricsTabPresent();
            editTeamBasePage.GoToMetricsTab();
            var expectedPageHeaderTitle = "A high performing healthy team uses both subjective measures (Team Health Assessment) and hard metrics to understand their performance. Enter the data below to unlock your metrics Dashboard!";
            Assert.AreEqual(expectedPageHeaderTitle, memberCommonPage.GetPageHeaderTitle(), $"The Metrics tab does not loaded successfully - {env}");
        }
    }
}