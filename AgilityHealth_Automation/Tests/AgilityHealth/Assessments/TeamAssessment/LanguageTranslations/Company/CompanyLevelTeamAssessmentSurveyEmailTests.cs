using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.LanguageTranslations.Company
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("LanguageTranslation")]
    public class CompanyLevelTeamAssessmentSurveyEmailTests : LanguageTranslationsBaseTest
    {
        private static User SiteAdmin => TestEnvironment.UserConfig.GetUserByDescription("translation");
        private static AddCompanyRequest _companyRequest;
        public static string NormalTeamName = "Team_" + RandomDataUtil.GetTeamName();
        public static string Language = ManageRadarFactory.SelectTranslatedLanguage(new List<string>() { "English", "Arabic", "Polish" });

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void TeamAssessment_SurveyEmail_AtCompanyLevel_ForDifferentLanguages()
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompanyPage2 = new AddCompany2RadarSelectionPage(Driver, Log);
            var addCompanyPage3 = new AddCompany3SubscriptionPage(Driver, Log);
            var addCompanyPage4 = new AddCompany4SecurityPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info($"Login to the application and create a new Company with {Language} language");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdmin.Username, SiteAdmin.Password);
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add9UI");
            _companyRequest.IsoLanguageCode = Language;

            addCompanyPage1.FillInCompanyProfileInfo(_companyRequest);
            if (SiteAdmin.IsSiteAdmin())
            {
                addCompanyPage1.FillInAdminInfo(_companyRequest);
            }

            addCompanyPage1.ClickNextButton();
            addCompanyPage2.WaitUntilLoaded();

            const string radarName = SharedConstants.TeamHealthRadarName;
            addCompanyPage2.SelectRadar(radarName);

            addCompanyPage2.ClickNextButton();
            addCompanyPage3.WaitUntilLoaded();

            addCompanyPage3.FillSubscriptionInfo(_companyRequest);

            addCompanyPage3.ClickNextButton();
            addCompanyPage4.WaitUntilLoaded();

            addCompanyPage4.FillSecurityInfo(_companyRequest, false);

            addCompanyPage4.ClickCreateCompanyButton();
            companyDashboardPage.WaitUntilLoaded();

            Log.Info($"Navigate to 'Manage Feature' and Turn ON 'Enable Language Selection' feature for {_companyRequest.Name} company");
            var companyId = setup.GetCompany(_companyRequest.Name).GetAwaiter().GetResult().Id;
            manageFeaturesPage.NavigateToPage(companyId);
            manageFeaturesPage.TurnOnEnableLanguageSelection();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate to Team dashboard page and create a team without 'Preferred Language' option then add member/stakeholder to the team");
            dashBoardPage.NavigateToPage(companyId);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            var today = DateTime.Now.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            var teamInfo = new TeamInfo()
            {
                TeamName = NormalTeamName,
                WorkType = SharedConstants.NewTeamWorkType,
                PreferredLanguage = null,
                Methodology = "Scrum",
                Department = "Test Department",
                DateEstablished = today,
                AgileAdoptionDate = today,
                Description = "Test Description",
                TeamBio = RandomDataUtil.GetTeamBio(),
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                Tags = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Business Lines", SharedConstants.TeamTag) }
            };
            var teamMemberInfo = new TeamMemberInfo
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = Constants.UserEmailPrefix + "member" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
            };
            var stakeHolderInfo = new StakeHolderInfo
            {
                FirstName = SharedConstants.Stakeholder2.FirstName,
                LastName = SharedConstants.Stakeholder2.LastName,
                Email = SharedConstants.Stakeholder2.Email,
                Role = SharedConstants.StakeholderRole
            };

            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();
            addTeamMemberPage.ClickAddNewTeamMemberButton();

            addTeamMemberPage.EnterTeamMemberInfo(teamMemberInfo);
            addTeamMemberPage.ClickSaveAndCloseButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();

            addStakeHolderPage.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(stakeHolderInfo);
            addStakeHolderPage.ClickSaveAndCloseButton();

            addStakeHolderPage.ClickReviewAndFinishButton();

            Log.Info($"Go to Team dashboard page and click on {teamInfo.TeamName} team");
            finishAndReviewPage.ClickOnGoToTeamDashboard();
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(teamInfo.TeamName);
            dashBoardPage.ClickOnTeamName(teamInfo.TeamName);

            //Convert Team Information into 'AddTeamWithMemberRequest' DTO
            var teamMemberDetails = new AddMemberRequest
            {
                FirstName = teamMemberInfo.FirstName,
                LastName = teamMemberInfo.LastName,
                Email = teamMemberInfo.Email
            };
            var stakeholderDetails = new AddStakeholderRequest
            {
                FirstName = stakeHolderInfo.FirstName,
                LastName = stakeHolderInfo.LastName,
                Email = stakeHolderInfo.Email
            };

            CreateAssessmentAndVerifyEmail(Language, _companyRequest.Name, teamInfo.TeamName, teamMemberDetails, stakeholderDetails);
        }


        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!SiteAdmin.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }
    }
}