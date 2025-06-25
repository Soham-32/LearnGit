using System;
using System.IO;
using System.Linq;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GlobalTranslations
{
    [TestClass]
    [TestCategory("LanguageTranslation"), TestCategory("TeamAssessment")]
    public class GlobalTranslationsTests : BaseTest
    {
        private static User TranslationAdmin => TestEnvironment.UserConfig.GetUserByDescription("translation");
        private static readonly string Language = ManageRadarFactory.SelectTranslatedLanguage();

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")]
        [TestCategory("TeamAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"),
          TestCategory("Member")]
        public void GlobalTranslation_EndToEndTest()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            const string companyName = "Automation_CA";

            var translations = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\GlobalTranslation\GlobalTranslations.json").DeserializeJsonObject<AtCommon.Dtos.GlobalTranslation.GlobalTranslations>();
            var languageData = translations.Languages.FirstOrDefault(lang => lang.Language == Language);
            if (languageData == null)
            {
                throw new ArgumentNullException($"Language data for {Language} not found.");
            }

            Log.Info("Navigate to Login page and verify the language Dropdown list");
            login.NavigateToPage();

            var actualLanguageList = topNav.GetListOfLanguages();
            var expectedLanguageList = ManageRadarFactory.Languages();

            Log.Info("Verify the language Dropdown list");
            Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Language list doesn't match");

            Log.Info($"Select {Language} Language from the language dropdown and try to login without using credentials");
            topNav.SelectALanguage(Language);
            login.LoginToApplication("", "");

            var expectedLoginPageTitleText = languageData.LoginPageResponse.Title;
            var expectedLoginPageTitleDescriptionText = languageData.LoginPageResponse.TitleDescription;
            var expectedLoginPageEmailText = languageData.LoginPageResponse.Email;
            var expectedLoginPagePasswordText = languageData.LoginPageResponse.Password;
            var expectedLoginPageForgotPasswordText = languageData.LoginPageResponse.ForgotPassword;
            var expectedLoginPageKeepMeLoggedInTextText = languageData.LoginPageResponse.KeepMeLoggedIn;
            var expectedLoginPageDoNotHaveAccountText = languageData.LoginPageResponse.DoNotHaveAccount;
            var expectedLoginPageLoginButtonText = languageData.LoginPageResponse.LoginButton;
            var expectedLoginPageEmailValidation = languageData.LoginPageResponse.EmailValidation;
            var expectedLoginPagePasswordValidation = languageData.LoginPageResponse.PasswordValidation;

            var actualLoginPageTitleDescriptionText = login.GetPageTitleDescriptionText();
            var actualLoginPageTitleText = login.GetPageTitleText();
            var actualLoginPageEmailText = login.GetEmailText();
            var actualLoginPagePasswordText = login.GetPasswordText();
            var actualLoginPageKeepMeLoggedInTextText = login.GetKeepMeLoggedInText();
            var actualLoginPageDoNotHaveAccountText = login.GetDoNotHaveAnAccountText();
            var actualLoginPageForgotPasswordText = login.GetForgotPasswordText();
            var actualLoginPageEmailValidation = login.GetEmailValidationErrorText();
            var actualLoginPagePasswordValidation = login.GetPasswordValidationErrorText();
            var actualLoginPageLoginButtonText = login.GetLoginButtonText();

            Assert.AreEqual(expectedLoginPageTitleText, actualLoginPageTitleText, "Login page 'Title' text doesn't match.");
            Assert.AreEqual(expectedLoginPageTitleDescriptionText, actualLoginPageTitleDescriptionText, "Login page 'Title Description' text doesn't match.");
            Assert.AreEqual(expectedLoginPageEmailText, actualLoginPageEmailText, "Login page 'Email' text doesn't match.");
            Assert.AreEqual(expectedLoginPagePasswordText, actualLoginPagePasswordText, "Login page 'Password' text doesn't match.");
            Assert.AreEqual(expectedLoginPageForgotPasswordText, actualLoginPageForgotPasswordText, "Login page 'Forgot Password' text doesn't match.");
            Assert.AreEqual(expectedLoginPageKeepMeLoggedInTextText, actualLoginPageKeepMeLoggedInTextText, "Login page 'Keep Me logged In' text doesn't match.");
            Assert.AreEqual(expectedLoginPageDoNotHaveAccountText, actualLoginPageDoNotHaveAccountText, "Login page 'Don't have an Account' text doesn't match.");
            Assert.AreEqual(expectedLoginPageLoginButtonText, actualLoginPageLoginButtonText, "Login page 'Login Button' text doesn't match.");
            Assert.AreEqual(expectedLoginPageEmailValidation, actualLoginPageEmailValidation, "Login page 'Email validation' text doesn't match.");
            Assert.AreEqual(expectedLoginPagePasswordValidation, actualLoginPagePasswordValidation, "Login page 'Password validation' text doesn't match.");

            Log.Info("Login into the application");
            login.LoginToApplication(User.Username, User.Password);

            if (TranslationAdmin.IsSiteAdmin())
            {
                Log.Info("Wait until Company dashboard page loaded completely");
                companyDashboardPage.WaitUntilLoaded();
                Thread.Sleep(10000); // need to wait till language gets translate
                Driver.RefreshPage();
                companyDashboardPage.WaitUntilLoaded();

                var expectedCompanyPageTitleText = languageData.CompanyDashboardPageResponse.Title;
                var expectedCompanyPageAddACompanyButtonText = languageData.CompanyDashboardPageResponse.AddACompanyButton;
                var actualCompanyPageTitleText = companyDashboardPage.GetPageTitleText();
                var actualCompanyPageAddACompanyButtonText = companyDashboardPage.GetAddACompanyButtonText();
                var translatedCompanyName = languageData.CompanyDashboardPageResponse.Companyname;

                Log.Info("Verify 'Language Dropdown' list , 'Page title' and 'Add a company' button texts on company dashboard page");
                actualLanguageList = topNav.GetListOfLanguages(true);
                Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Company dashboard page Language list doesn't match");
                Assert.AreEqual(expectedCompanyPageTitleText, actualCompanyPageTitleText, "Company dashboard page 'Page Title' text doesn't match.");
                Assert.AreEqual(expectedCompanyPageAddACompanyButtonText, actualCompanyPageAddACompanyButtonText, "Company dashboard page 'Add a Company button' text doesn't match.");

                Log.Info($"Navigate to {translatedCompanyName} company on the company dashboard page");
                companyDashboardPage.Search(companyName);
                companyDashboardPage.ClickOnCompanyName(translatedCompanyName);
            }

            Log.Info("Verify 'Dashboard' list , 'Page title' and 'Add a Team' button texts on Team dashboard page");
            teamDashboardPage.GridTeamView();
            Thread.Sleep(10000); // need to wait till language gets translate
            Driver.RefreshPage();
            var expectedTeamDashboardPageAddATeamButtonText = languageData.TeamDashboardPageResponse.AddATeamButton;
            var expectedTeamDashboardPageDashboardList = languageData.TeamDashboardPageResponse.DashboardList;
            var actualTeamDashboardPageDashboardList = teamDashboardPage.GetDashboardsList();

            if (!(TranslationAdmin.IsOrganizationalLeader() || TranslationAdmin.IsMember()))
            {
                var actualTeamDashboardPageAddATeamButtonText = teamDashboardPage.GetAddATeamButtonText();
                Assert.AreEqual(expectedTeamDashboardPageAddATeamButtonText, actualTeamDashboardPageAddATeamButtonText, "Team dashboard page 'Add a Team button' text doesn't match.");
            }
            if (TranslationAdmin.IsSiteAdmin() || TranslationAdmin.IsCompanyAdmin())
            {
                Assert.That.ListsAreEqual(expectedTeamDashboardPageDashboardList, actualTeamDashboardPageDashboardList,
                    "Team dashboard page 'Dashboard List' doesn't match");
            }
            else
            {
                foreach (var dashboard in actualTeamDashboardPageDashboardList)
                {
                    Assert.That.ListContains(expectedTeamDashboardPageDashboardList, dashboard, "Team dashboard page 'Dashboard List' doesn't match");
                }
            }
            Log.Info($"Select {SharedConstants.RadarTeam} team and navigate to Team Assessment dashboard page");
            var radarName = languageData.TeamAssessmentDashboardPageResponse.ThRadar;
            teamDashboardPage.SearchTeam(SharedConstants.RadarTeam);
            teamDashboardPage.GoToTeamAssessmentDashboard(1);

            var expectedTeamAssessmentDashboardPageTitleText = languageData.TeamAssessmentDashboardPageResponse.Title;
            var actualTeamAssessmentDashboardPageTitleText = teamAssessmentDashboard.GetHeadingText();
            var expectedTeamAssessmentDashboardPageDashboardList = languageData.TeamAssessmentDashboardPageResponse.DashboardList;
            var actualTeamAssessmentDashboardPageDashboardList = teamAssessmentDashboard.GetDashboardsList();
            var expectedTeamAssessmentDashboardPageAddAnAssessmentButtonText = languageData.TeamAssessmentDashboardPageResponse.AddAnAssessmentButton;

            if (!TranslationAdmin.IsMember())
            {
                var actualTeamAssessmentDashboardPageAddAnAssessmentButtonText = teamAssessmentDashboard.GetAddAnAssessmentButtonText();
                Assert.AreEqual(expectedTeamAssessmentDashboardPageAddAnAssessmentButtonText, actualTeamAssessmentDashboardPageAddAnAssessmentButtonText, "Team Assessment dashboard page 'Add an Assessment button' text doesn't match.");
            }

            Assert.AreEqual(expectedTeamAssessmentDashboardPageTitleText, actualTeamAssessmentDashboardPageTitleText, "Team Assessment dashboard page 'Page title' text doesn't match.");
            Assert.That.ListsAreEqual(expectedTeamAssessmentDashboardPageDashboardList, actualTeamAssessmentDashboardPageDashboardList, "Team Assessment dashboard page 'Dashboard List' doesn't match.");

            Log.Info($"Click on {radarName} radar and navigate to the Assessment page");
            teamAssessmentDashboard.ClickOnRadar(radarName);
            Thread.Sleep(10000);// need to wait till language gets translate
            Driver.RefreshPage();

            Log.Info("Verify 'Page title' text on Assessment page");
            var expectedAssessmentPageTitleText = languageData.AssessmentPageResponse.Title;
            var actualAssessmentPageTitleText = assessmentDetailPage.GetAssessmentTitleText();
            Assert.AreEqual(expectedAssessmentPageTitleText, actualAssessmentPageTitleText, "Assessment page 'Page title' text doesn't match.");
        }
    }
}
