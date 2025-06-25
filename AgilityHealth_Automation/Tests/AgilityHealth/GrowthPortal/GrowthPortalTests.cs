using System;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPortal;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPortal
{
    [TestClass]
    [TestCategory("GrowthPortal")]
    public class GrowthPortalTests : BaseTest
    {

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug:- 47508
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void GrowthPortal_ViewContent()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Health), "The 'Health' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Recommendations), "The 'Recommendations' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Videos), "The 'Videos' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Resources), "The 'Resources' section did not display");
            Assert.IsTrue(growthPortalPage.DoesSectionDisplay(CompetencySection.Coaching), "The 'Coaching' section did not display");

        }
        

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug:- 47508
        [TestCategory("CompanyAdmin")]
        public void GrowthPortal_Resources_Publish_LearnMore()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            growthPortalPage.ClickEditPortalButton();

            growthPortalPage.ClickAddResourceButton();

            var resourceTitle = "Resource" + RandomDataUtil.GetTeamDescription();
            growthPortalPage.EnterResourceTitle(resourceTitle);

            growthPortalPage.SelectResourceCompetency("Impediment Management");

            const string resourceLink = "https://agilityinsights.ai/";
            growthPortalPage.EnterResourceLink(resourceLink);

            growthPortalPage.ClickSaveResourceButton();

            growthPortalPage.ClickPublishButton();

            Driver.Close();
            Driver.SwitchToFirstWindow();

            topNav.ClickOnGrowthPortalLink();
            
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            growthPortalPage.ClickResourceLink(resourceTitle);

            Driver.SelectWindowByIndex(2);
            Assert.AreEqual(resourceLink, Driver.Url);

            try
            {
                Driver.Close();
                Driver.SelectWindowByTitle("Growth Portal - AH");

                growthPortalPage.ClickEditPortalButton();
                growthPortalPage.DeleteAllResources();

                growthPortalPage.ClickPublishButton();
            }
            catch (Exception)
            {
                Log.Info("Resource cleanup failed");
            }

        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug:- 47508
        [TestCategory("CompanyAdmin")]
        public void GrowthPortal_Resources_Publish_Download()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            growthPortalPage.ClickEditPortalButton();

            growthPortalPage.ClickAddResourceButton();

            var resourceTitle = "Resource" + RandomDataUtil.GetTeamDescription();
            growthPortalPage.EnterResourceTitle(resourceTitle);

            growthPortalPage.SelectResourceCompetency("Impediment Management");

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\testfile.txt");
            growthPortalPage.UploadResourceFile(filePath);

            growthPortalPage.ClickSaveResourceButton();

            growthPortalPage.ClickPublishButton();

            Driver.Close();
            Driver.SwitchToFirstWindow();

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            FileUtil.DeleteFilesInDownloadFolder("testfile.txt");
            growthPortalPage.ClickResourceFile(resourceTitle);

            Assert.IsTrue(FileUtil.IsFileDownloaded("testfile.txt"),
                "Failure !! Resource file is downloaded properly");

            try
            {
                growthPortalPage.ClickEditPortalButton();
                growthPortalPage.DeleteAllResources();

                growthPortalPage.ClickPublishButton();
            }
            catch (Exception)
            {
                Log.Info("Resource cleanup failed");
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug:- 47508
        [TestCategory("CompanyAdmin")]
        public void GrowthPortal_Resources_Publish_Image()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var growthPortalPage = new GrowthPortalPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            growthPortalPage.ClickEditPortalButton();

            growthPortalPage.ClickAddResourceButton();

            var resourceTitle = "Resource" + RandomDataUtil.GetTeamDescription();
            growthPortalPage.EnterResourceTitle(resourceTitle);

            growthPortalPage.SelectResourceCompetency("Impediment Management");

            const string resourceLink = "https://agilityinsights.ai/";
            growthPortalPage.EnterResourceLink(resourceLink);

            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg");
            growthPortalPage.UploadResourceThumbnail(imagePath);

            growthPortalPage.ClickSaveResourceButton();

            growthPortalPage.ClickPublishButton();

            Driver.Close();
            Driver.SwitchToFirstWindow();

            topNav.ClickOnGrowthPortalLink();
            Driver.SelectWindowByTitle("Growth Portal - AH");

            growthPortalPage.SelectAssessment(SharedConstants.TeamAssessmentType);

            growthPortalPage.ClickSelectButton();

            growthPortalPage.TreeViewExpandNode("Leadership");

            growthPortalPage.TreeViewExpandNode("Team Facilitator");

            growthPortalPage.ClickCompetency("Effective Facilitation");

            Assert.IsTrue(growthPortalPage.DoesResourceThumbnailDisplay(resourceTitle), $"Thumbnail image for <{resourceTitle}> is not displayed");
            
            growthPortalPage.ClickResourceThumbnail(resourceTitle);

            Driver.SelectWindowByIndex(2);
            Assert.AreEqual(resourceLink, Driver.Url);

            try
            {
                Driver.Close();
                Driver.SelectWindowByTitle("Growth Portal - AH");

                growthPortalPage.ClickEditPortalButton();
                growthPortalPage.DeleteAllResources();

                growthPortalPage.ClickPublishButton();
            }
            catch (Exception)
            {
                Log.Info("Resource cleanup failed");
            }
        }
    }
}
