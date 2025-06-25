using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentResultShareTest2 : BaseTest
    {
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void TA_Result_Share_BL_TA_CanNotSeeButton()
        {

            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam);

            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);

            manageFeaturesPage.NavigateToPage(Company.Id);
            manageFeaturesPage.TurnOnEnableShareAssessmentResult();
            manageFeaturesPage.ClickUpdateButton();

            topNav.LogOut();

            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.DevOpsHealthRadar, "Edit");
            Assert.IsFalse(taEditPage.IsStartSharingAssessmentButtonDisplayed(), "'Start Sharing Assessment Result' button is not displayed");
        }
    }
}