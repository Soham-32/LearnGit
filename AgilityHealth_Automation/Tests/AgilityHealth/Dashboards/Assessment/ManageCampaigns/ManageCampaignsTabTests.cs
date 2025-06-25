using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.ManageCampaigns
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class ManageCampaignsTabTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageCampaignsTab_UiVerification()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            const string expectedManageCampaignsTabColor = Constants.BlueColorHexValue;

            Log.Info("Login to the application and Navigate to the Assessment dashboard page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashBoardPage.ClickAssessmentDashBoard();

            Log.Info("Verify that 'Manage Campaigns' Tab is present on 'Assessment dashboard' page");
            var assessmentManagementTabs = assessmentDashboardListTabPage.GetAllTabs();
            Assert.That.ListContains(assessmentManagementTabs, "Manage Campaigns", "Manage Campaigns tab is not present in Assessment Management List");

            Log.Info("Verify that Manage Campaigns tab is underlined and color changed to blue color when user hover on it from the assessment list tab");
            assessmentDashboardListTabPage.HoverOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            var actualManageCampaignsTabColor = assessmentDashboardListTabPage.GetColorOfTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            Assert.AreEqual(expectedManageCampaignsTabColor, actualManageCampaignsTabColor, "Color of Manage Campaigns Tab is not in blue color");
            Assert.IsTrue(assessmentDashboardListTabPage.IsTabUnderlined(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab), "Manage Campaigns Tab is not underlined");

            Log.Info("Click on 'Manage Campaigns' Tab present on 'Assessment dashboard' page");
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);

            Log.Info("Verify that Manage Campaigns tab is underlined and color changed to blue color when user hover on it from the assessment list tab");
            assessmentDashboardListTabPage.HoverOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            actualManageCampaignsTabColor = assessmentDashboardListTabPage.GetColorOfTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            Assert.AreEqual(expectedManageCampaignsTabColor, actualManageCampaignsTabColor, "Color of Manage Campaigns Tab is not in blue color");
            Assert.IsTrue(assessmentDashboardListTabPage.IsTabUnderlined(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab), "Manage Campaigns Tab is not underlined");
        }


        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void ManageCampaignsTab_DetailedPageVerification()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageCampaignTabPage = new ManageCampaignsTabPage(Driver, Log);

            var expectedGridHeaderTextList = new List<string>()
            {
               "Campaign Name", "Creation Date/Time", "Start Date", "End Date", "Status", "Commands"
            };

            Log.Info("Login to the application and Navigate to the Assessment dashboard page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashBoardPage.ClickAssessmentDashBoard();

            Log.Info("Click on 'Manage Campaigns' Tab present on 'Assessment dashboard' page");
            manageCampaignTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);

            Log.Info("Verify that page title is 'Manage Campaigns'");
            var pageHeaderTitle = manageCampaignTabPage.GetManageCampaignHeaderTitleText();
            Assert.AreEqual("Manage Campaigns", pageHeaderTitle, "'Manage campaigns' title is not matched");

            Log.Info("Verify the 'Create New Campaign' button text");
            var createNewCampaignsButtonText = manageCampaignTabPage.GetCreateNewCampaignsButtonText();
            Assert.AreEqual("Create New Campaign", createNewCampaignsButtonText, "'Create New Campaigns' button text is not matched");

            Log.Info("Click on 'Create New Campaign' button and verify the current url");
            manageCampaignTabPage.ClickOnCreateNewCampaignsButton();
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/v2/campaign/company/{Company.Id}/create", Driver.GetCurrentUrl(), "Campaign Details page url is not matched");

            Log.Info("Navigate back and verify all the table header text on the manage campaigns page");
            Driver.Navigate().Back();
            var actualGridHeaderText = manageCampaignTabPage.GetAllColumnHeaderTextList().ToList();
            Assert.That.ListsAreEqual(expectedGridHeaderTextList, actualGridHeaderText, "Grid header text doesn't match");

            Log.Info("Click on first Index Campaign 'Edit' button and verify that user redirect on the edit page");
            manageCampaignTabPage.ClickOnEditCampaignsButtonByIndex(1);
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/wizard"), "URL does not contains 'wizard'");
        }
    }
}