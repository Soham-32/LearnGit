using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.DataObjects;
using AtCommon.Api;
using AtCommon.Utilities;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch;
using System.Data;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.Dashboards
{
    [TestClass]
    [TestCategory("HeartBeatChecks"), TestCategory("OE_Dashboard")]
    public class TeamsDashboardTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyQuickLaunchTeamPopupInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var quickLaunchAssessment = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchTeam = new QuickLaunchTeamPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Navigate to the Teams Dashboard page of " + companyId + " successfully");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamDashboard.png");
            quickLaunchAssessment.ClickOnQuickLaunchOptionsLink("Team");

            Log.Info("Selected Team Option in Quick Launch Dropdown");
            Assert.AreEqual("Create Team", quickLaunchTeam.GetTitleText(), "Failure !! Quick Create Team Popup text could not be verified");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyBulkDataManagementInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Navigate to the Teams Dashboard page of " + companyId + " successfully");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamDashboard.png");

            if (teamDashboardPage.IsBulkManagementButtonDisplayed() == true){
                teamDashboardPage.MoveToBulkDataMgmtButton();
                Log.Info("Moved to Bulk Data Management Dropdown");
                List<string> BulkDataMgmtMenuList = new List<string>() { "Download Template", "Import", "Export", "Generate Ext. Identifiers", "Help" };
                Assert.That.ListsAreEqual(BulkDataMgmtMenuList, teamDashboardPage.GetBulkDataMgmtMenuList(), "Failure !! Quick Create Team Popup text could not be verified");
            }else { Log.Info("This company does not have Bulk Data Management access"); }
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyViewOptionsOnTeamDashInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Navigate to the Teams Dashboard page of " + companyId + " successfully");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamDashboard.png");
            teamDashboardPage.MoveToViewButton();
            Assert.IsTrue(teamDashboardPage.DoesViewItemExist("Grid"), "Failure !! Unable to see Grid View Option");
            Assert.IsTrue(teamDashboardPage.DoesViewItemExist("Swim Lanes"), "Failure !! Unable to see Grid View Option");

            Log.Info("Switching to Swim Lane View");
            teamDashboardPage.GoToSwimLaneView();
            Assert.IsTrue(teamDashboardPage.DoesExpandButtonExist(), "Failure !! Unable to see Grid View Option");
        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyStateOnTeamDashInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Navigate to the Teams Dashboard page of " + companyId + " successfully");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamDashboard.png");

            teamDashboardPage.MoveToStateButton();
            Log.Info("Moved to State Dropdown");
            Assert.IsTrue(teamDashboardPage.DoesStateItemExist("Active"), "Failure !! Unable to see Active State Option");
            Log.Info("Active Button verified Successfully");
            Assert.IsTrue(teamDashboardPage.DoesStateItemExist("Archived"), "Failure !! Unable to see Archived State Option");

        }

        [TestMethod]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void VerifyAddTeamFromTeamDashInProd(string env)
        {
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Navigate to the Teams Dashboard page of " + companyId + " successfully");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\TeamDashboard.png");

            Log.Info("Add A Team from team dashboard");
            teamDashboardPage.ClickAddATeamButton();
            teamDashboardPage.ClickTeamType();
            teamDashboardPage.ClickAddTeamButton();
            var ExpectedCreateTeamStepperTitle = "Follow the wizard to create a new team";
            Assert.AreEqual(ExpectedCreateTeamStepperTitle, createTeamStepperPage.GetCreateTeamStepperInfo(), " Failure !! Create Team stepper info header is not matched");

            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Add A Multi Team from team dashboard");
            teamDashboardPage.ClickAddATeamButton();
            teamDashboardPage.ClickMultiteamType();
            teamDashboardPage.ClickAddTeamButton();
            var ExpectedCreateMultiTeamStepperTitle = "Follow the wizard to create a new multi-team.";
            Assert.AreEqual(ExpectedCreateMultiTeamStepperTitle, createTeamStepperPage.GetCreateTeamStepperInfo(), " Failure !! Create Team stepper info header is not matched");
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            Log.Info("Add A Enterprise Team from team dashboard");
            teamDashboardPage.ClickAddATeamButton();
            teamDashboardPage.ClickEnterpriseTeamType();
            teamDashboardPage.ClickAddTeamButton();
            var ExpectedCreateEnterpriseTeamStepperTitle = "Follow the wizard to create an enterprise team.";
            Assert.AreEqual(ExpectedCreateEnterpriseTeamStepperTitle, createTeamStepperPage.GetCreateTeamStepperInfo(), " Failure !! Create Team stepper info header is not matched");
            teamDashboardPage.NavigateToTeamDashboardPageForProd(env, companyId);

            if(teamDashboardPage.AddTeam_DoesRadioButtonExist(TeamType.NTier)==true)
            {
            Log.Info("Add N-Tier Team from team dashboard");
            teamDashboardPage.ClickAddATeamButton();
            teamDashboardPage.ClickNTierTeamType();
            teamDashboardPage.ClickAddTeamButton();
            var ExpectedCreateNTierTeamStepperTitle = "Follow the wizard to create an N-Tier team.";
            Assert.AreEqual(ExpectedCreateNTierTeamStepperTitle, createTeamStepperPage.GetCreateTeamStepperInfo(), " Failure !! Create Team stepper info header is not matched");
            }else{Log.Info("This company does not have N-Tier Team creation access");}
}
    }
}