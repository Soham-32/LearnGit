using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.LanguageTranslations.Team
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("LanguageTranslation")]
    public class TeamLevelTeamAssessmentSurveyEmailTests : LanguageTranslationsBaseTest
    {
        private static User TranslationAdmin => TestEnvironment.UserConfig.GetUserByDescription("translation");
        private static AddTeamWithMemberRequest _team;
        private static bool _classInitFailed;
        public static int TeamId;
        public static string Language = ManageRadarFactory.SelectTranslatedLanguage(new List<string>() { "English", "Arabic", "Polish" });

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                _team = TeamFactory.GetNormalTeam("EditTeam");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = RandomDataUtil.GetFirstName(),
                    LastName = SharedConstants.TeamMemberLastName,
                    Email = Constants.UserEmailPrefix + "member" + _team.Members.FirstOrDefault().FirstName + Constants.UserEmailDomain,
                });
                _team.Stakeholders.Add(new AddStakeholderRequest
                {
                    FirstName = SharedConstants.Stakeholder2.FirstName,
                    LastName = SharedConstants.Stakeholder2.LastName,
                    Email = SharedConstants.Stakeholder2.Email
                });
                setupApi.CreateTeam(_team, TranslationAdmin).GetAwaiter().GetResult();
                TeamId = setupApi.GetCompanyHierarchy(Company.Id, TranslationAdmin).GetTeamByName(_team.Name).TeamId;

            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_SurveyEmail_AtTeamLevel_ForDifferentLanguages()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            Log.Info("Login to the application");
            login.NavigateToPage();
            login.LoginToApplication(TranslationAdmin.Username, TranslationAdmin.Password);

            Log.Info($"Search {_team.Name} team and click on it's Edit button on the Team dashboard page");
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.ClickTeamEditButton(_team.Name);

            Log.Info($"Update Team information for {Language} language");
            editTeamBasePage.GoToTeamProfileTab();
            editTeamProfilePage.SelectPreferredLanguage(Language);
            editTeamProfilePage.ClickUpdateTeamProfileButton();

            Log.Info($"Navigate to the Assessment dashboard page for {_team.Name} team");
            teamAssessmentDashboard.NavigateToPage(TeamId);
            CreateAssessmentAndVerifyEmail(Language, TranslationAdmin.CompanyName, _team.Name, _team.Members.FirstOrDefault(), _team.Stakeholders.FirstOrDefault());
        }
    }
}
