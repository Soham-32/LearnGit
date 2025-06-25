using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesGrowthPortalTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static AtCommon.Dtos.Company SettingsCompany =>
            SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);


        //Growth Portal
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthPortal_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Growth Portal' feature and turn off 'Default (Master) Content' and 'Custom Content' sub-feature ");
            manageFeaturesPage.TurnOnGrowthPortal();
            manageFeaturesPage.TurnOffGrowthPortalDefaultContent();
            manageFeaturesPage.TurnOffGrowthPortalCustomContent();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("'Growth Portal' link should be displayed");
            Assert.IsFalse(topNav.DoesGrowthPortalLinkDisplay(), "On top nav, Growth Portal link is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthPortal_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Growth Portal' feature");
            manageFeaturesPage.TurnOffGrowthPortal();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("'Growth Portal' link should not be displayed");
            Assert.IsFalse(topNav.DoesGrowthPortalLinkDisplay(), "On top nav, Growth Portal link is displayed");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug : 47508
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthPortal_DefaultContent_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Growth Portal' feature and turn on 'Default (Master) Content' sub-feature");
            manageFeaturesPage.TurnOnGrowthPortal();
            manageFeaturesPage.TurnOnGrowthPortalDefaultContent();
            manageFeaturesPage.TurnOffGrowthPortalCustomContent();

            Log.Info("Turn on 'Default (Master) Content' sub-features");
            manageFeaturesPage.TurnOnDefaultContentSubFeature("Videos");
            manageFeaturesPage.TurnOnDefaultContentSubFeature("Resources");
            manageFeaturesPage.TurnOnDefaultContentSubFeature("Training");
            manageFeaturesPage.TurnOnDefaultContentSubFeature("Coaches");
            manageFeaturesPage.TurnOnDefaultContentSubFeature("Recommendations");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");
            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);
            growthPortalPage.ClickSelectButton();
            growthPortalPage.TreeViewExpandNode("Leadership");
            growthPortalPage.TreeViewExpandNode("Team Facilitator");
            growthPortalPage.ClickCompetency("Effective Facilitation");

            Assert.IsFalse(growthPortalPage.DoesEditButtonDisplay(), "Edit button is displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Health), "The 'Health' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Recommendations), "The 'Recommendations' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Videos), "The 'Videos' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Resources), "The 'Resources' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Coaching), "The 'Coaching' section is not displayed");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug : 47508
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthPortal_CustomContent_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Growth Portal' feature and turn on 'Custom Content' sub-feature");
            manageFeaturesPage.TurnOnGrowthPortal();
            manageFeaturesPage.TurnOnGrowthPortalCustomContent();
            manageFeaturesPage.TurnOffGrowthPortalDefaultContent();

            Log.Info("Turn on 'Custom Content' sub-features");
            manageFeaturesPage.TurnOnCustomContentSubFeature("Videos");
            manageFeaturesPage.TurnOnCustomContentSubFeature("Resources");
            manageFeaturesPage.TurnOnCustomContentSubFeature("Training");
            manageFeaturesPage.TurnOnCustomContentSubFeature("Coaches");
            manageFeaturesPage.TurnOnCustomContentSubFeature("Recommendations");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");
            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);
            growthPortalPage.ClickSelectButton();
            growthPortalPage.TreeViewExpandNode("Leadership");
            growthPortalPage.TreeViewExpandNode("Team Facilitator");
            growthPortalPage.ClickCompetency("Effective Facilitation");

            Assert.IsTrue(growthPortalPage.DoesEditButtonDisplay(), "Edit button is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Health), "The 'Health' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Recommendations), "The 'Recommendations' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Videos), "The 'Videos' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Resources), "The 'Resources' section is not displayed");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Coaching), "The 'Coaching' section is not displayed");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug : 47508
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_GrowthPortal_DefaultAndCustomContent_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            manageFeaturesPage.TurnOnGrowthPortal();
            manageFeaturesPage.TurnOffGrowthPortalDefaultContent();
            manageFeaturesPage.TurnOnGrowthPortalDefaultContent();
            manageFeaturesPage.TurnOffGrowthPortalCustomContent();
            manageFeaturesPage.TurnOnGrowthPortalCustomContent();

            manageFeaturesPage.TurnOffDefaultContentSubFeature("Videos");
            manageFeaturesPage.TurnOffDefaultContentSubFeature("Resources");
            manageFeaturesPage.TurnOffDefaultContentSubFeature("Training");
            manageFeaturesPage.TurnOffDefaultContentSubFeature("Coaches");
            manageFeaturesPage.TurnOffDefaultContentSubFeature("Recommendations");
            manageFeaturesPage.TurnOffCustomContentSubFeature("Videos");
            manageFeaturesPage.TurnOffCustomContentSubFeature("Resources");
            manageFeaturesPage.TurnOffCustomContentSubFeature("Training");
            manageFeaturesPage.TurnOffCustomContentSubFeature("Coaches");
            manageFeaturesPage.TurnOffCustomContentSubFeature("Recommendations");

            manageFeaturesPage.ClickUpdateButton();
            topNav.LogOut();

            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            Assert.IsTrue(growthPortalPage.DoesEditButtonDisplay(), "Edit button didn't display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Health), "The 'Health' section did not display");

        }
    }
}
