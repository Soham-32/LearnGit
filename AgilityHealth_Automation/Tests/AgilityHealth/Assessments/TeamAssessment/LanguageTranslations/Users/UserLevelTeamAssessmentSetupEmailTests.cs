using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.LanguageTranslations.Users
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("LanguageTranslation")]
    public class UserLevelTeamAssessmentSetupEmailTests : LanguageTranslationsBaseTest
    {
        private static User TranslationAdmin => TestEnvironment.UserConfig.GetUserByDescription("translation");
        private static AddTeamWithMemberRequest _team;
        private static bool _classInitFailed;
        private const string Password = SharedConstants.CommonPassword;
        public static readonly string Language = ManageRadarFactory.SelectTranslatedLanguage(new List<string> { "Arabic", "Polish" });
        public static string OtherLanguage = ManageRadarFactory.SelectTranslatedLanguage(new List<string>() { Language, "Arabic", "Polish" });
        public static int TeamId;


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("EditTeam", 1);
                _team.Members.First().FirstName = "Member999";
                setupApi.CreateTeam(_team, TranslationAdmin).GetAwaiter().GetResult();

                //Give access to the newly created member
                TeamId = setupApi.GetCompanyHierarchy(Company.Id, TranslationAdmin).GetTeamByName(_team.Name).TeamId;
                setupUi.TeamMemberAccessAtTeamLevel(TeamId, _team.Members.First().Email, TranslationAdmin);

                //create account for member and set password
                setupUi.SetUserPassword(_team.Members.First().Email, Password, "Inbox");
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51053
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_SetupEmail_AtUserLevel_ForDifferentLanguages()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var header = new TopNavigation(Driver, Log);
            var accountSettingsPage = new AccountSettingsPage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);

            Log.Info("Login to the application as a Member");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(_team.Members.First().Email, Password);

            Log.Info($"Navigate to the 'My Profile' page and select {Language} Language");
            header.HoverOnNameRoleSection();
            header.ClickOnMyProfile();
            accountSettingsPage.SelectLanguage(Language);
            accountSettingsPage.ClickUpdateButton();

            Log.Info("Log out from the application as a Member and Re-Login as Admin");
            header.LogOut();
            login.LoginToApplication(TranslationAdmin.Username, TranslationAdmin.Password);

            Log.Info($"Select {_team.Name} team from Team dashboard page");
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.ClickTeamEditButton(_team.Name);

            Log.Info($"Update Team information for {OtherLanguage} language");
            editTeamBasePage.GoToTeamProfileTab();
            editTeamProfilePage.SelectPreferredLanguage(OtherLanguage);
            editTeamProfilePage.ClickUpdateTeamProfileButton();

            Log.Info($"Navigate to the Assessment dashboard page for {_team.Name} team");
            teamAssessmentDashboard.NavigateToPage(TeamId);
            CreateAssessmentAndVerifySetupEmail(Language, TranslationAdmin.CompanyName, _team.Name, _team.Members.FirstOrDefault());
        }
    }
}