using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Scheduler;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageFeatures
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageFeatures")]
    public class ManageFeaturesAssessmentsTests : BaseTest
    {
        private static User CompanyAdmin1 => TestEnvironment.UserConfig.GetUserByDescription("user 3");
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");
        private static User OrganizationalLeaderUser => SiteAdminUserConfig.GetUserByDescription("org leader 3");
        private static User BusinessLineAdminUser => SiteAdminUserConfig.GetUserByDescription("business line admin 3");
        private static User TeamAdminUser => SiteAdminUserConfig.GetUserByDescription("team admin 3");
        private static User CompanyAdmin => SiteAdminUserConfig.GetUserByDescription("company admin");
        private static User Member => SiteAdminUserConfig.GetUserByDescription("member 6");
        private static AtCommon.Dtos.Company SettingsCompany =>
            SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);
        private static CompanyHierarchyResponse _allTeamsList;
        private static TeamResponse _goiTeamResponse;
        private static AddTeamWithMemberRequest _teamRequest;
        private static readonly List<DimensionNote> TeamMemberAssessmentNotes = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = "Vision member 1" },
                new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = "Vision member 2" },
                new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = "Planning member 1" },
                new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = "Planning member 2" },
                new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = "Roles member 1" },
                new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = "Roles member 2" },
                new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence member 1" },
                new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence member 2" },
                new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = "Measurements member 1" },
                new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = "Measurements member 2" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = "Team Facilitator member 1" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = "Team Facilitator member 2" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = "Technical Lead(s) member 1" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = "Technical Lead(s) member 2" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = "Product Owner member 1" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = "Product Owner member 2" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = "Management member 1" },
                new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = "Management member 2" },
                new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = "Team Dynamics member 1" },
                new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = "Team Dynamics member 2" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = "Agility member 1" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = "Agility member 2" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = "Team Structure member 1" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = "Team Structure member 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = "Strengths member 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = "Strengths member 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Growth Opportunities", Note = "Improvements member 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Growth Opportunities", Note = "Improvements member 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = "Impediments member 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = "Impediments member 2" }
            };

        private static readonly List<DimensionNote> StackHolderAssessmentNotes = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Stakeholder 1" },
                new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Stakeholder 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = "Stakeholder 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = "Stakeholder 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Growth Opportunities", Note = "Stakeholder 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Growth Opportunities", Note = "Stakeholder 2" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = "Stakeholder 1" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = "Stakeholder 2" }
            };

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
            Campaign = "AT Campaign",
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };
        private const string WhiteColorHex = "#333333";
        private const string OrangeColorHex = "#d95f0e";
        private static TeamHierarchyResponse _team;
        private static RadarResponse _radar;
        private static SetupTeardownApi _setup;
        private static SetUpMethods _setupUi;

        private static readonly Dictionary<string, string> CompetencyDetails = new Dictionary<string, string>
        {
            //ID,Description
            { "4422", "Confidence" },
            { "4870", "PERFORMANCE" }
        };

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _teamRequest = TeamFactory.GetNormalTeam("TeamForMemberLogin", 2);

            _setup = new SetupTeardownApi(TestEnvironment);
            _setupUi = new SetUpMethods(_, TestEnvironment);

            _setup.CreateTeam(_teamRequest, CompanyAdmin1).GetAwaiter().GetResult();
            _allTeamsList = _setup.GetCompanyHierarchy(SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);
            _team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            _radar = _setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);

            _setupUi.AddTeamAssessment(_team.TeamId, TeamAssessment, SiteAdminUser);
        }

        //Notify Users Of Pending Assessments
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_NotifyUsersOfPendingAssessments_On()
        {
            var login = new LoginPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var companyAdmin2 = CompanyAdminUserConfig.GetUserByDescription("user 6");
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.TeamForNotification);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Notify Users Of Pending Assessments' feature");
            manageFeaturesPage.TurnOnNotifyUsersOfPendingAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate team assessment page and create the team assessment ");
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            //Delete all assessments as it might cause problem notification verification
            try
            {
                var links = teamAssessmentDashboard.GetAssessmentEditLinks();
                foreach (var link in links)
                {
                    Driver.NavigateToPage(link);
                    taEditPage.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();
                }
            }
            catch (Exception)
            {
                Log.Warning("Something went wrong while deleting assessments");
            }
            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName= RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Log out as {CompanyAdmin1.FullName} and login as {companyAdmin2.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(companyAdmin2.Username, companyAdmin2.Password);

            Log.Info("Verify that Open Assessment link & pop up should be displayed and pop up text");
            Assert.IsTrue(topNavigation.IsOpenAssessmentLinkPresent(), "Open Assessment link is not displayed");
            Assert.IsTrue(topNavigation.IsOpenAssessmentPopUpPresent(), "Open Assessment pop up not displayed");

            var expected = $"You have an assessment waiting for Automation Team for Notification : {assessmentName}\r\nPlease click the button below to take it.\r\nTake assessment";
            Assert.AreEqual(expected, topNavigation.GetOpenAssessmentPopUpText(), "Open Assessment pop up text doesn't match");

            topNavigation.ClickOnOpenAssessmentPopUpTakeAssessmentLink();
            Driver.SwitchToLastWindow();
            surveyPage.CloseDeploymentPopup();
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitRandomSurvey();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            Driver.SwitchToFirstWindow();
            topNavigation.LogOut();
            login.LoginToApplication(companyAdmin2.Username, companyAdmin2.Password);

            Log.Info("Verify that Open Assessment link & pop up should not be displayed and pop up text");
            Assert.IsFalse(topNavigation.IsOpenAssessmentLinkPresent(), "Open Assessment link is displayed");
            Assert.IsFalse(topNavigation.IsOpenAssessmentPopUpPresent(), "Open Assessment pop up is displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_NotifyUsersOfPendingAssessments_Off()
        {

            var login = new LoginPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var companyAdmin2 = CompanyAdminUserConfig.GetUserByDescription("user 6");
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(Constants.TeamForNotification);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Notify Users Of Pending Assessments' feature");
            manageFeaturesPage.TurnOffNotifyUsersOfPendingAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Navigate team assessment page and create the team assessment ");
            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName= RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Log out as {CompanyAdmin1.FullName} and login as {companyAdmin2.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(companyAdmin2.Username, companyAdmin2.Password);

            Log.Info("Verify that Open Assessment link & pop up should not be displayed");
            Assert.IsFalse(topNavigation.IsOpenAssessmentLinkPresent(), "Open Assessment link is displayed");
            Assert.IsFalse(topNavigation.IsOpenAssessmentPopUpPresent(), "Open Assessment pop up is displayed");

        }

        //Assessment Settings
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AssessmentSettings_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Assessment Settings' feature");
            manageFeaturesPage.TurnOnAssessmentSettings();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'Manage Radars' section should be displayed");
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("View Radars"), "On Settings page, 'Manage Radars' section is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AssessmentSettings_Off()
        {

            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Assessment Settings' feature");
            manageFeaturesPage.TurnOffAssessmentSettings();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that on Settings page, 'Manage Radars' section should not be displayed");
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("View Radars"), "On Settings page, 'Manage Radars' section is present");
        }

        //Individual Assessments
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_IndividualAssessments_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            var goiTeam = _setup.GetTeamProfileResponse(SharedConstants.GoiTeam, CompanyAdmin);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Individual Assessments' feature");
            manageFeaturesPage.TurnOnIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that assessment type toggle should be displayed after turning Individual Assessments on for the company");
            teamAssessmentDashboardPage.NavigateToPage(goiTeam.First().TeamId);
            Assert.IsTrue(teamAssessmentDashboardPage.IsAssessmentTypeToggleDisplayed(), "The Assessment Type Toggle is not displayed after turning Individual Assessments on for the company");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_IndividualAssessments_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            var goiTeam = _setup.GetTeamProfileResponse(SharedConstants.GoiTeam, CompanyAdmin);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Individual Assessments' feature");
            manageFeaturesPage.TurnOffIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that assessment type toggle should not be displayed after turning Individual Assessments on for the company");
            teamAssessmentDashboardPage.NavigateToPage(goiTeam.First().TeamId);
            Assert.IsFalse(teamAssessmentDashboardPage.IsAssessmentTypeToggleDisplayed(), "The Assessment Type Toggle is displayed after turning Individual Assessments on for the company");
        }

        //Hide Assessment Status Icon
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_HideAssessmentStatusIcons_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Hide Assessment Status Icon' feature");
            manageFeaturesPage.TurnOnHideAssessmentStatusIcons();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Create assessment with two reviewers");
            var setup = new SetupTeardownApi(TestEnvironment);
            var reviewer1 = setup.CreateReviewer(MemberFactory.GetReviewer(), CompanyAdmin1).GetAwaiter().GetResult();
            var reviewer2 = setup.CreateReviewer(MemberFactory.GetReviewer(), CompanyAdmin1).GetAwaiter().GetResult();
            var teamRequest = TeamFactory.GetGoiTeam("IAIcon", 1);
            var team = setup.CreateTeam(teamRequest, CompanyAdmin1).GetAwaiter().GetResult();

            var assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(SettingsCompany.Id, CompanyAdmin1.CompanyName, team.Uid, $"IAIcon_{Guid.NewGuid()}");
            assessmentRequest.Members = team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            assessmentRequest.Members.First().Reviewers.Add(reviewer1.ToAddIndividualMemberRequest());
            assessmentRequest.Members.First().Reviewers.Add(reviewer2.ToAddIndividualMemberRequest());

            var assessment = setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType, CompanyAdmin1).GetAwaiter().GetResult();
            assessmentRequest.BatchId = assessment.BatchId;
            setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType, CompanyAdmin1).GetAwaiter().GetResult();

            Log.Info($"Log out as {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNavigation.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Fill the survey of assessment");
            Driver.NavigateToPage(GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject, reviewer1.Email, "inbox"));
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            var assessmentUid = assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(SettingsCompany.Id, team.Uid, assessmentUid);

            Log.Info("Verify if all icons next to the reviewers are hidden");
            Assert.IsFalse(iaEditPage.DoesCheckMarkIconDisplay(reviewer1.Email), "Check Mark icon is displayed");
            Assert.IsFalse(iaEditPage.DoesLockIconDisplay(reviewer1.Email), "Lock icon is displayed");
            Assert.IsFalse(iaEditPage.DoesResendIconDisplay(), "Resend icon is displayed");
            Assert.IsFalse(iaEditPage.DoesAccessLinkIconDisplay(reviewer2.Email), "Access Link icon is displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_HideAssessmentStatusIcons_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffHideAssessmentStatusIcons();
            manageFeaturesPage.ClickUpdateButton();

            var setup = new SetupTeardownApi(TestEnvironment);
            var reviewer1 = setup.CreateReviewer(
                MemberFactory.GetReviewer(), CompanyAdmin1).GetAwaiter().GetResult();
            var reviewer2 = setup.CreateReviewer(
                MemberFactory.GetReviewer(), CompanyAdmin1).GetAwaiter().GetResult();
            var teamRequest = TeamFactory.GetGoiTeam("IAIcon", 1);
            var team = setup.CreateTeam(teamRequest, CompanyAdmin1).GetAwaiter().GetResult();

            var assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                SettingsCompany.Id, CompanyAdmin1.CompanyName, team.Uid, $"IAIcon_{Guid.NewGuid()}");
            assessmentRequest.Members = team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            assessmentRequest.Members.First().Reviewers.Add(reviewer1.ToAddIndividualMemberRequest());
            assessmentRequest.Members.First().Reviewers.Add(reviewer2.ToAddIndividualMemberRequest());

            var assessment = setup.CreateIndividualAssessment(assessmentRequest,
                SharedConstants.IndividualAssessmentType, CompanyAdmin1).GetAwaiter().GetResult();
            assessmentRequest.BatchId = assessment.BatchId;
            setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType, CompanyAdmin1)
                .GetAwaiter().GetResult();
            topNavigation.LogOut();

            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Driver.NavigateToPage(GmailUtil.GetSurveyLink(
                SharedConstants.IaEmailReviewerSubject, reviewer1.Email, "inbox"));
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            var assessmentUid = assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(SettingsCompany.Id, team.Uid, assessmentUid);

            Log.Info("Verify if all icons next to the reviewers are hidden");
            Assert.IsTrue(iaEditPage.DoesCheckMarkIconDisplay(reviewer1.Email), "Check Mark icon should display");
            Assert.IsTrue(iaEditPage.DoesLockIconDisplay(reviewer1.Email), "Lock icon should display");

            Assert.IsTrue(iaEditPage.DoesResendIconDisplay(), "Resend icon should display");
            Assert.IsTrue(iaEditPage.DoesAccessLinkIconDisplay(reviewer2.Email), "Access Link icon should display");
        }

        //Highlight Stakeholder Responses
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_HighlightStakeholderResponses_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Highlight Stakeholder Responses' feature");
            manageFeaturesPage.TurnOnHighlightStakeholderResponses();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the radar page");
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2ForStakeholder);

            foreach (var note in TeamMemberAssessmentNotes)
            {
                Assert.AreEqual(WhiteColorHex,
                    note.Dimension != "Open"
                        ? radarPage.GetSurveyNoteColor(note.SubDimension, note.Note)
                        : radarPage.GetSurveyOpenEndQuestionNoteColor(note.SubDimension, note.Note),
                    $"Team member note color doesn't match for '{note.SubDimension}' and '{note.Note}'");
            }

            foreach (var note in StackHolderAssessmentNotes)
            {
                Assert.AreEqual(OrangeColorHex,
                    note.Dimension != "Open"
                        ? radarPage.GetSurveyNoteColor(note.SubDimension, note.Note)
                        : radarPage.GetSurveyOpenEndQuestionNoteColor(note.SubDimension, note.Note),
                    $"Stakeholder note color doesn't match for '{note.SubDimension}' and '{note.Note}'");
            }

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_HighlightStakeholderResponses_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Highlight Stakeholder Responses' feature");
            manageFeaturesPage.TurnOffHighlightStakeholderResponses();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Navigate to the radar page");
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2ForStakeholder);

            foreach (var note in TeamMemberAssessmentNotes)
            {
                Assert.AreEqual(WhiteColorHex,
                    note.Dimension != "Open"
                        ? radarPage.GetSurveyNoteColor(note.SubDimension, note.Note)
                        : radarPage.GetSurveyOpenEndQuestionNoteColor(note.SubDimension, note.Note),
                    $"Team member note color doesn't match for '{note.SubDimension}' and '{note.Note}'");
            }

            foreach (var note in StackHolderAssessmentNotes)
            {
                Assert.AreEqual(WhiteColorHex,
                    note.Dimension != "Open"
                        ? radarPage.GetSurveyNoteColor(note.SubDimension, note.Note)
                        : radarPage.GetSurveyOpenEndQuestionNoteColor(note.SubDimension, note.Note),
                    $"Stakeholder note color doesn't match for '{note.SubDimension}' and '{note.Note}'");
            }
        }

        //Team vs Agile Coach Comparison
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamVSAgileCoachComparison_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Team vs Agile Coach Comparison' feature");
            manageFeaturesPage.TurnOnTeamVsAgileCoach();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2ForAgileCoach);
            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            radarPage.Filter_OpenFilterSidebar();
            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName("Agile Coach");

            Assert.IsTrue(assessmentDetailPage.DoesAgileCoachDotDisplay(), "Agile Coach dots is not displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamVSAgileCoachComparison_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Team vs Agile Coach Comparison' feature");
            manageFeaturesPage.TurnOffTeamVsAgileCoach();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2ForAgileCoach);
            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            Log.Info("verify that 'Agile Coach' checkbox should not displayed");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsFalse(assessmentDetailPage.DoesFilterItemDisplay("Agile Coach"),
                "Agile Coach filter item displayed in filter even the feature is turned off");
        }

        //Facilitator Assessment
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FacilitatorAssessment_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Facilitator Assessment' feature");
            manageFeaturesPage.TurnOnFacilitatorAssessment();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            dashBoardPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Verify that Facilitator Dashboard tab is present");
            Assert.IsTrue(dashBoardPage.IsFacilitatorDashboardDisplayed(), "Facilitator Dashboard tab is NOT present");

            assessmentProfile.NavigateToPage(team.TeamId);
            var name = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, name);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Go to created assessment review and finish page,verifying that 'Send post retrospective feedback Assessment?' text/checkbox is present");
            Assert.IsTrue(reviewAndLaunch.IsSendRetrospectiveSurveyCheckboxPresent(), "'Send post retrospective feedback Assessment?' checkbox is not present");
            Assert.IsTrue(reviewAndLaunch.IsSendRetrospectiveSurveyTextPresent(), "'Send post retrospective feedback Assessment?' text is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FacilitatorAssessment_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Facilitator Assessment' feature");
            manageFeaturesPage.TurnOffFacilitatorAssessment();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            dashBoardPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Verify that Facilitator Dashboard tab is not present");
            Assert.IsFalse(dashBoardPage.IsFacilitatorDashboardDisplayed(), "Facilitator Dashboard tab is present");

            assessmentProfile.NavigateToPage(team.TeamId);
            var name = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, name);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            Assert.IsFalse(reviewAndLaunch.IsSendRetrospectiveSurveyCheckboxPresent(), "'Send post retrospective feedback assessment?' checkbox is present");
            Assert.IsFalse(reviewAndLaunch.IsSendRetrospectiveSurveyTextPresent(), "'Send post retrospective feedback assessment?' text is present");
        }

        //Assessment Scheduler
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AssessmentScheduler_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Assessment Scheduler' feature");
            manageFeaturesPage.TurnOnAssessmentScheduler();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that Assessment Scheduler should be displayed");
            dashBoardPage.ClickAssessmentDashBoard();
            Assert.IsTrue(assessmentDashboardListTabPage.IsTabDisplayed(AssessmentDashboardBasePage.TabSelection.SchedulerTab), "Assessment Scheduler link is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AssessmentScheduler_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Assessment Scheduler' feature");
            manageFeaturesPage.TurnOffAssessmentScheduler();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Verify that Assessment Scheduler should not be displayed");
            dashBoardPage.ClickAssessmentDashBoard();
            Assert.IsFalse(assessmentDashboardListTabPage.IsTabDisplayed(AssessmentDashboardBasePage.TabSelection.SchedulerTab), "Assessment Scheduler link is displayed");

        }

        //Assessment Checklist and Custom Maturity Model
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMaturity_On()
        {
            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Assessment Checklist and Custom Maturity Model' feature");
            manageFeaturesPage.TurnOnTeamMaturity();
            manageFeaturesPage.ClickUpdateButton();

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Edit");

            Log.Info("Verify that Assessment Checklist showing");
            Assert.IsTrue(taEditPage.DoesAssessmentChecklistDisplay(), "Assessment Checklist tab should display");

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnSettingsLink();
            Assert.IsTrue(settingsPage.IsSettingOptionPresent("Manage Model and Checklist"), "On Settings page, 'Manage Model and Checklist' section is not present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMaturity_Off()
        {

            var login = new LoginPage(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);

            var radarTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Assessment Checklist and Custom Maturity Model' feature");
            manageFeaturesPage.TurnOffTeamMaturity();
            manageFeaturesPage.ClickUpdateButton();

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(radarTeam.TeamId);
            teamAssessmentDashboard.SelectRadarLink(SharedConstants.TeamHealth2Radar, "Edit");

            Log.Info("Verify that Assessment Checklist not showing");
            Assert.IsFalse(taEditPage.DoesAssessmentChecklistDisplay(), "Assessment Checklist tab shouldn't display");

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            topNav.ClickOnSettingsLink();
            Assert.IsFalse(settingsPage.IsSettingOptionPresent("Manage Model and Checklist"), "On Settings page, 'Manage Model and Checklist' section is present");
        }
        //Assessment PIN Access
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AssessmentPinAccess_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Assessment PIN Access' feature");
            manageFeaturesPage.TurnOnSurveyPinAccess();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName=RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");

            Assert.IsTrue(taEditPage.DoesDisplayPinButtonDisplay(), "Display PIN button is not displayed");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AssessmentPinAccess_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and Go to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Assessment PIN Access' feature");
            manageFeaturesPage.TurnOffSurveyPinAccess();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and Login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName= RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");

            Assert.IsFalse(taEditPage.DoesDisplayPinButtonDisplay(), "Display PIN button is displayed");
        }
        //Team Member Log In
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMemberLogin_On_SubFeature_TeamMemberLogInAllowAhfToOverWriteCheckbox_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} , Navigate to the 'Manage Feature' page and Turn On 'TeamMember Login' feature then Check sub feature 'Allow AHFs to overwrite company settings when launching assessments'");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamMemberLogin();
            manageFeaturesPage.TurnOnOffSubFeatureTeamMemberLogInAllowAhfToOverWriteCheckbox();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment, Verify that 'Account setup emails be sent to participants' section should be displayed on 'Review & Launch' page");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentProfile.NavigateToPage(team.TeamId);

            var assessmentName = $"TeamMemberLogInAllowAhfToOverWriteOn_{RandomDataUtil.GetAssessmentName()}";
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(_teamRequest.Members[0].FullName());
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            Assert.IsTrue(reviewAndLaunch.IsAccountSetupEmailSettingSectionPresent(), "'Account setup email setting' section is not displayed on 'Review & Launch' page");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMemberLogin_On_SubFeature_TeamMemberLogInAllowAhfToOverWriteCheckbox_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as  {User.FullName}, Navigate to the 'Manage Feature' page and Turn On 'TeamMember Login' feature then Uncheck sub feature 'Allow AHFs to overwrite company settings when launching assessments' checkbox");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamMemberLogin();
            manageFeaturesPage.TurnOnOffSubFeatureTeamMemberLogInAllowAhfToOverWriteCheckbox(false);
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment, Verify that 'Account setup emails be sent to participants' section should not be displayed on 'Review & Launch' page");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentProfile.NavigateToPage(team.TeamId);

            var assessmentName = $"TeamMemberLogInAllowAhfToOverWriteOff_{RandomDataUtil.GetAssessmentName()}";
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(_teamRequest.Members[0].FullName());
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            Assert.IsFalse(reviewAndLaunch.IsAccountSetupEmailSettingSectionPresent(), "'Account setup emails be sent to participants' section is displayed on 'Review & Launch' page");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMemberLogin_On_SubFeature_TeamMemberLogInAfterSubmitSurvey_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as  {User.FullName}, Navigate to the 'Manage Feature' page and Turn On 'TeamMember Login' feature then Select sub feature 'When the participants submit the assessment'");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamMemberLogin();
            manageFeaturesPage.TurnOnSubFeatureTeamMemberLogInAfterSubmitSurvey();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentProfile.NavigateToPage(team.TeamId);

            var assessmentName = $"TeamMemberLogInAfterSubmitSurvey_{RandomDataUtil.GetAssessmentName()}";
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(_teamRequest.Members[0].FullName());
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Navigate to the edit {assessmentName} team assessment page and Verify that 'Account Setup E-Mail' text should be matched");
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");
            Assert.AreEqual("When the participants submit the assessment", teamAssessmentEditPage.GetAccountSetupEmailText(), "'Account Setup E-Mail' text is not matched");

            Log.Info("Go to email page of Team Member, click on the Take Survey link");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(team.Name, assessmentName),
                To = _teamRequest.Members[0].Email,
                Labels = new List<string> { "Inbox" }
            };
            _setupUi.CompleteTeamMemberSurvey(emailSearch);

            Log.Info("Verifying that Confirmation email should be sent to team member after submitting survey");
            const string accountSubject = "Confirm your account";
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(accountSubject, _teamRequest.Members[0].Email, "Auto_User"), $"Email not received by {_teamRequest.Members[0].Email} with subject {accountSubject}");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMemberLogin_On_SubFeature_TeamMemberLogInAssessmentEndDate_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as  {User.FullName}, Navigate to the 'Manage Feature' page and Turn On 'TeamMember Login' feature then Select sub feature 'When the assessment end date has been reached'");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamMemberLogin();
            manageFeaturesPage.TurnOnSubFeatureTeamMemberLogInOnAssessmentEndDate();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentProfile.NavigateToPage(team.TeamId);

            var assessmentName = $"TeamMemberLogInOnAssessmentEndDate_{RandomDataUtil.GetAssessmentName()}";
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(_teamRequest.Members[0].FullName());
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Navigate to the edit {assessmentName} team assessment page and Verify that 'Account Setup E-Mail' text should be matched");
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");
            Assert.AreEqual("When the assessment end date has been reached", teamAssessmentEditPage.GetAccountSetupEmailText(), "'Account Setup E-Mail' text is not matched");

            Log.Info("Go to email page of Team Member, click on the Take Survey link");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(team.Name, assessmentName),
                To = _teamRequest.Members[0].Email,
                Labels = new List<string> { "Inbox" }
            };
            _setupUi.CompleteTeamMemberSurvey(emailSearch);

            Log.Info("Verifying that confirmation emails should not sent to Team Member after submitting survey");
            const string accountSubject = "Confirm your account";
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(accountSubject, _teamRequest.Members[0].Email, "", "UNREAD", 60), $"Email received by {_teamRequest.Members[0].Email} with subject {accountSubject}");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamMemberLogin_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(_teamRequest.Name);
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as  {User.FullName}, Navigate to the 'Manage Feature' page and Turn Off 'TeamMember Login' feature");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffTeamMemberLogin();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            assessmentProfile.NavigateToPage(team.TeamId);

            var assessmentName = $"TeamMemberLoginOff_{RandomDataUtil.GetAssessmentName()}";
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(_teamRequest.Members[0].FullName());
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            Assert.IsFalse(reviewAndLaunch.IsAccountSetupEmailSettingSectionPresent(), "'Account setup emails be sent to participants' section is displayed on 'Review & Launch' page");
            reviewAndLaunch.ClickOnPublish();

            Log.Info($"Navigate to the edit {assessmentName} team assessment page and Verify that 'Account Setup E-Mail' text should be matched");
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");
            Assert.AreEqual("Do not send setup e-mails", teamAssessmentEditPage.GetAccountSetupEmailText(), "'Account Setup E-Mail' text is not matched");

            Log.Info("Go to email page of Team Member, click on the Take Survey link");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(team.Name, assessmentName),
                To = _teamRequest.Members[0].Email,
                Labels = new List<string> { "Inbox" }
            };
            _setupUi.CompleteTeamMemberSurvey(emailSearch);

            Log.Info("Verifying that confirmation emails should not sent to Team Member after submitting survey");
            const string accountSubject = "Confirm your account";
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(accountSubject, _teamRequest.Members[0].Email, "", "UNREAD", 60), $"Email received by {_teamRequest.Members[0].Email} with subject {accountSubject}");
        }

        //Find A Facilitator
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FindAFacilitator_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var schedulerTabPage = new SchedulerTabPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"Login as {SiteAdminUser.FullName} and Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            //Pre-requisite: Turning on Assessment Scheduler for company.
            manageFeaturesPage.TurnOffAllowAssessmentCreationFromAhFs();
            manageFeaturesPage.TurnOnAssessmentScheduler();
            manageFeaturesPage.ClickUpdateButton();

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Log out as {SiteAdminUser.FullName} and Login as  {User.FullName} then navigate to the 'Manage Feature' page");
            topNavigation.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Find A Facilitator' feature");
            manageFeaturesPage.TurnOnFindAFacilitator();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info("Create team assessment and Verify that 'Find A Facilitator' checkbox should be displayed on 'Review & Launch' page ");
            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName= RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            Assert.IsTrue(reviewAndLaunch.IsFindAFacilitatorCheckboxVisible(), "The 'Find A Facilitator' checkbox is not visible");

            Log.Info("Navigate to the 'Assessment Scheduler' page and Click on 'Create Draft Assessment' button'");
            schedulerTabPage.NavigateToPage(SettingsCompany.Id);
            schedulerTabPage.ClickCreateDraftAssessment();

            Log.Info("Verify that 'Find A Facilitator' checkbox should be visible");
            Assert.IsTrue(schedulerTabPage.IsFindAFacilitatorCheckboxVisible(), "The 'Find A Facilitator' Checkbox is not visible.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_FindAFacilitator_Off()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var schedulerTabPage = new SchedulerTabPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"Login as {SiteAdminUser.FullName} and Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(SiteAdminUser.Username, SiteAdminUser.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            //Pre-requisite: Turning on Assessment Scheduler for company.
            manageFeaturesPage.TurnOffAllowAssessmentCreationFromAhFs();
            manageFeaturesPage.TurnOnAssessmentScheduler();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {SiteAdminUser.FullName} and Login as  {User.FullName} then navigate to the 'Manage Feature' page");
            topNavigation.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.CloseDeploymentPopup();
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Find A Facilitator' feature");
            manageFeaturesPage.TurnOffFindAFacilitator();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as {User.FullName} and login as {CompanyAdmin1.FullName} then create team assessment");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            assessmentProfile.NavigateToPage(team.TeamId);
            var assessmentName=RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(SharedConstants.TeamAssessmentType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectAllTeamMembers();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Verify that Find A Facilitator should not be visible");
            Assert.IsFalse(reviewAndLaunch.IsFindAFacilitatorCheckboxVisible(), "The 'Find A Facilitator' checkbox is visible");

            Log.Info("Navigate to the 'Assessment Scheduler' page and Click on 'Create Draft Assessment' button'");
            schedulerTabPage.NavigateToPage(SettingsCompany.Id);
            schedulerTabPage.ClickCreateDraftAssessment();

            Log.Info("Verify the 'Find A Facilitator' checkbox should not be visible");
            Assert.IsFalse(schedulerTabPage.IsFindAFacilitatorCheckboxVisible(), "The 'Find A Facilitator' Checkbox is visible.");

        }
        //Enable Maturity Calculations
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnableMaturityCalculations_OnOff()
        {
            var topNav = new TopNavigation(Driver, Log);
            var loginPage = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfilePage = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembersPage = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolderPage = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunchPage = new ReviewAndLaunchPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            const string expectedTeamMaturityTitle = "Team Maturity";

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page then Turn on 'Enable Maturity Calculations' features");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnEnableMaturityCalculations();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin.FullName} then verify 'Team Maturity' section should be displayed");
            topNav.LogOut();
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(CompanyAdmin.Username, CompanyAdmin.Password);

            Log.Info($"Navigate to the {SharedConstants.Team} and create the team assessment");
            assessmentProfilePage.NavigateToPage(team.TeamId);
            var assessmentName = $"Team Maturity Assessment{RandomDataUtil.GetAssessmentName()}";
            assessmentProfilePage.FillDataForAssessmentProfile(SharedConstants.TeamHealthRadarName, assessmentName);
            assessmentProfilePage.ClickOnNextSelectTeamMemberButton();
            selectTeamMembersPage.SelectTeamMemberByName(Constants.TeamMemberName1);
            selectTeamMembersPage.ClickOnNextSelectStakeholdersButton();
            selectStakeHolderPage.ClickOnReviewAndFinishButton();
            reviewAndLaunchPage.ClickOnPublish();

            Log.Info($"Get the survey link of {Constants.TeamMemberName1} from gmail and fill the survey");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(team.Name, assessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            _setupUi.CompleteTeamMemberSurvey(emailSearch);

            Log.Info($"Go to radar page of {assessmentName} and verify that 'Team Maturity' section should be displayed");
            assessmentDashboardPage.NavigateToPage(team.TeamId);
            assessmentDashboardPage.ClickOnRadar(assessmentName);
            Assert.IsTrue(radarPage.GetTeamMaturityTitle().StartsWith(expectedTeamMaturityTitle), "On radar page 'Team Maturity' Section is not displayed");

            Log.Info("Logout as company admin and login as site admin and navigate to the 'Manage Feature' page then Turn off 'Enable Maturity Calculations' features");
            topNav.LogOut();
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffEnableMaturityCalculations();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Logout as a SA user and login as CA user then verify 'Team Maturity' section should not be displayed");
            topNav.LogOut();
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(CompanyAdmin.Username, CompanyAdmin.Password);

            Log.Info($"Go to radar page of {assessmentName}");
            assessmentDashboardPage.NavigateToPage(team.TeamId);
            assessmentDashboardPage.ClickOnRadar(assessmentName);
            Assert.IsFalse(radarPage.GetTeamMaturityTitle().StartsWith(expectedTeamMaturityTitle), "On radar page 'Team Maturity' Section is displayed");
        }

        //Pre-Populate Growth Item Description
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_PrePopulateGrowthItemDescription_On()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On 'Pre-Populate Growth Item Description' feature");
            manageFeaturesPage.TurnOnPrePopulateGrowthItemDescription();
            manageFeaturesPage.TurnOnAssessmentSettings();
            manageFeaturesPage.TurnOnIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info($"Navigate to the {TeamAssessment.AssessmentName} radar page and click on the competency");
            assessmentDashboard.NavigateToPage(_team.TeamId);
            assessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            radarPage.ClickCompetency(CompetencyDetails.Keys.First());

            Log.Info("Click on the Growth plan item icon and Verify that pre-populate growth item description should be displayed");
            radarPage.ClickGrowthPlanAddGiButton();
            var getPrePopulateGrowthPlanDescription = addGrowthItemPopup.GetDescription();
            Assert.IsTrue(getPrePopulateGrowthPlanDescription.Contains(CompetencyDetails.Values.First()), "Pre-populate GI Description is not matched");

            Log.Info($"Create Individual assessment in {Constants.GoiTeam} team");
            var team = _setup.GetTeamResponse(Constants.GoiTeam, CompanyAdmin1);
            dashboardPage.NavigateToPage(Company.Id);

            var teamId = dashboardPage.GetTeamIdFromLink(team.Name).ToInt();
            var assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(SettingsCompany.Id, CompanyAdmin1.CompanyName, team.Uid, $"Pre-population_GI_Description_{Guid.NewGuid()}");
            assessmentRequest.Members = team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            _setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType, CompanyAdmin1).GetAwaiter().GetResult();

            Log.Info($"Fill the survey of {assessmentRequest.Members.First().FirstName}");
            _setupUi.CompleteIndividualSurvey(assessmentRequest.Members.First().Email, assessmentRequest.PointOfContact);

            Log.Info($"Navigate to the {Constants.GoiTeam} and Click on the Growth plan item icon then Verify that pre-populate GI description should be displayed");
            iAssessmentDashboardPage.NavigateToPage(teamId);
            iAssessmentDashboardPage.ClickOnRadar($"{assessmentRequest.AssessmentName} - {assessmentRequest.Members.First().FirstName} {assessmentRequest.Members.First().LastName}");
            radarPage.ClickCompetency(CompetencyDetails.Keys.Last());
            radarPage.ClickGrowthPlanAddGiButton();
            var getGrowthPlanDescriptionIndividual = addGrowthItemPopup.GetDescription();
            Assert.IsTrue(getGrowthPlanDescriptionIndividual.Contains(CompetencyDetails.Values.Last()), "Pre-populate GI Description is not matched");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_PrePopulateGrowthItemDescription_Off()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            const string expectedGiDescription = "";

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn Off 'Pre-Populate Growth Item Description' feature");
            manageFeaturesPage.TurnOffPrePopulateGrowthItemDescription();
            manageFeaturesPage.TurnOnAssessmentSettings();
            manageFeaturesPage.TurnOnIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            Log.Info($"Navigate to the {TeamAssessment.AssessmentName} radar page and click on the competency");
            assessmentDashboard.NavigateToPage(_team.TeamId);
            assessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            radarPage.ClickCompetency(CompetencyDetails.Keys.First());

            Log.Info("Click on the Growth plan item icon and Verify that pre-populate GI description should not be displayed");
            radarPage.ClickGrowthPlanAddGiButton();
            Assert.AreEqual(expectedGiDescription, addGrowthItemPopup.GetDescription(), "GI description is not matched");

            Log.Info($"Create Individual assessment in {Constants.GoiTeam} team");
            var team = _setup.GetTeamResponse(Constants.GoiTeam, CompanyAdmin1);
            dashboardPage.NavigateToPage(Company.Id);

            var teamId = dashboardPage.GetTeamIdFromLink(team.Name).ToInt();
            var assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(SettingsCompany.Id, CompanyAdmin1.CompanyName, team.Uid, $"Pre-population_GI_Description_{Guid.NewGuid()}");
            assessmentRequest.Members = team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            _setup.CreateIndividualAssessment(assessmentRequest, SharedConstants.IndividualAssessmentType, CompanyAdmin1).GetAwaiter().GetResult();

            Log.Info($"Fill the survey of {assessmentRequest.Members.First().FirstName}");
            _setupUi.CompleteIndividualSurvey(assessmentRequest.Members.First().Email, assessmentRequest.PointOfContact);

            Log.Info($"Navigate to the {Constants.GoiTeam} and Click on the Growth plan item icon then Verify that pre-populate GI description should not be displayed");
            iAssessmentDashboardPage.NavigateToPage(teamId);
            iAssessmentDashboardPage.ClickOnRadar($"{assessmentRequest.AssessmentName} - {assessmentRequest.Members.First().FirstName} {assessmentRequest.Members.First().LastName}");
            radarPage.ClickCompetency(CompetencyDetails.Keys.Last());
            radarPage.ClickGrowthPlanAddGiButton();
            Assert.AreEqual(expectedGiDescription, addGrowthItemPopup.GetDescription(), "GI description is not matched");
        }

        //Standard Deviation Model
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_StandardDeviationModel_On()
        {

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);
            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeam);
            const string expectedTitle = "in percent standard deviation";

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            var usersList = new List<User>
            {
                CompanyAdmin1,
                OrganizationalLeaderUser,
                BusinessLineAdminUser,
                TeamAdminUser
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn On 'Standard Deviation Model' features");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnStandardDeviationModel();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Logout as a SA user and login as CA,  OL, BL ,TA user then verify 'Highest Consensus Competencies Standard Deviation' title and 'Lowest Consensus Competencies Standard Deviation' title should be display on Team, Multi-team, Enterprise level");
            topNav.LogOut();
            foreach (var user in usersList)
            {
                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);
                assessmentDashboard.NavigateToPage(team.TeamId);
                assessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on team level");

                radarPage.NavigateToPage(multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on multi-team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on multi-team level");

                radarPage.NavigateToPage(enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on enterprise-team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on enterprise-team level");

                topNav.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_StandardDeviationModel_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);
            var enterpriseTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.EnterpriseTeam);
            const string expectedTitle = "";

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            var usersList = new List<User>
            {
                CompanyAdmin1,
                OrganizationalLeaderUser,
                BusinessLineAdminUser,
                TeamAdminUser
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Manage Feature' page and Turn Off 'Standard Deviation Model' features");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffStandardDeviationModel();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Logout as a SA user and login as CA,  OL, BL ,TA user then verify 'Highest Consensus Competencies Standard Deviation' title and 'Lowest Consensus Competencies Standard Deviation' title should not be displayed on Team, Multi-team and Enterprise level");
            topNav.LogOut();
            foreach (var user in usersList)
            {
                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);
                assessmentDashboard.NavigateToPage(team.TeamId);
                assessmentDashboard.ClickOnRadar(SharedConstants.TeamHealth2Radar);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on team level");

                mtDashboardPage.NavigateToPage(multiTeam.TeamId);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on multi-team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on multi-team level");

                mtDashboardPage.NavigateToPage(enterpriseTeam.TeamId, true);
                Assert.AreEqual(expectedTitle, radarPage.GetHighestConsensusCompetenciesStandardDeviationTitle(), "'Highest Consensus Competencies Standard Deviation' title is not match on enterprise-team level");
                Assert.AreEqual(expectedTitle, radarPage.GetLowestConsensusCompetenciesStandardDeviationTitle(), "'Lowest Consensus Competencies Standard Deviation' title is not match on enterprise-team level");

                topNav.LogOut();
            }
        }

        //Allow Assessment Creation From AHFs Only
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AllowAssessmentCreationFromAHFsOnly_On()
        {

            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            var taNoAhf = SiteAdminUserConfig.GetUserByDescription("team admin no ahf");
            var taAhf = SiteAdminUserConfig.GetUserByDescription("team admin ahf");

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            manageFeaturesPage.TurnOnAllowAssessmentCreationFromAhFs();

            manageFeaturesPage.ClickUpdateButton();
            topNavigation.LogOut();

            login.LoginToApplication(taAhf.Username, taAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            Assert.IsTrue(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(),
                "'Add an Assessment button is not visible on Team Assessment Dashboard for Team Admin w/AHF'");

            batchesTabPage.NavigateToPage(SettingsCompany.Id);

            Assert.IsTrue(batchesTabPage.IsPlusButtonDisplayed(),
                "'Plus Button' is not visible on the Batches Dashboard");

            topNavigation.LogOut();

            login.LoginToApplication(taNoAhf.Username, taNoAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            Assert.IsFalse(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(),
                "'Add an Assessment' button should not be visible on Team Assessment Dashboard for Team Admin w/AHF'");

            batchesTabPage.NavigateToPage(SettingsCompany.Id);

            Assert.IsFalse(batchesTabPage.IsPlusButtonDisplayed(),
                "'Plus Button' should not be displayed on Batches Dashboard");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_AllowAssessmentCreationFromAHFsOnly_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);
            var taNoAhf = SiteAdminUserConfig.GetUserByDescription("team admin no ahf");
            var taAhf = SiteAdminUserConfig.GetUserByDescription("team admin ahf");

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            manageFeaturesPage.TurnOffAllowAssessmentCreationFromAhFs();
            manageFeaturesPage.ClickUpdateButton();

            topNavigation.LogOut();

            login.LoginToApplication(taAhf.Username, taAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            Assert.IsTrue(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(),
                "'Add an Assessment button is not visible on Team Assessment Dashboard for Team Admin w/AHF'");

            batchesTabPage.NavigateToPage(SettingsCompany.Id);

            Assert.IsTrue(batchesTabPage.IsPlusButtonDisplayed(),
                "'Plus Button' is not visible on the Batches Dashboard");

            topNavigation.LogOut();

            login.LoginToApplication(taNoAhf.Username, taNoAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            Assert.IsTrue(teamAssessmentDashboard.IsAddAssessmentButtonDisplayed(),
                "'Add an Assessment button is not visible on Team Assessment Dashboard for Team Admin w/AHF'");

            batchesTabPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("check that the 'Plus' button shows on batches dashboard");
            Assert.IsTrue(batchesTabPage.IsPlusButtonDisplayed(),
                "'Plus Button' is not visible on the Batches Dashboard");
        }

        //Team Comments
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamCommentsReport_On_SubFeature_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboard = new MtEtDashboardPage(Driver, Log);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Turn on 'Team Comments Report' and it's sub feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamCommentsReport();
            manageFeaturesPage.TeamCommentsReport_TurnOnSubFeature("Company Admin");
            manageFeaturesPage.TeamCommentsReport_TurnOnSubFeature("Organizational Leader");
            manageFeaturesPage.TeamCommentsReport_TurnOnSubFeature("Business Line Admin");
            manageFeaturesPage.TeamCommentsReport_TurnOnSubFeature("Team Admin");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as CA , OL, BL, TA users then verify that team comments excel icon should be displayed");
            topNavigation.LogOut();
            var usersList = new List<User>
            {
                CompanyAdmin1,
                OrganizationalLeaderUser,
                BusinessLineAdminUser,
                TeamAdminUser
            };
            foreach (var user in usersList)
            {
                login.LoginToApplication(user.Username, user.Password);
                mtDashboard.NavigateToPage(multiTeam.TeamId);
                mtDashboard.ClickOnRadar(SharedConstants.TeamAssessmentType);
                Assert.IsTrue(radarPage.IsTeamCommentsExcelButtonDisplayed(), "Team Comments Excel icon is not displayed");
                topNavigation.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamCommentsReport_On_SubFeature_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboard = new MtEtDashboardPage(Driver, Log);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Turn on 'Team Comments Report' and turn off it's sub feature");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOnTeamCommentsReport();
            manageFeaturesPage.TeamCommentsReport_TurnOffSubFeature("Company Admin");
            manageFeaturesPage.TeamCommentsReport_TurnOffSubFeature("Organizational Leader");
            manageFeaturesPage.TeamCommentsReport_TurnOffSubFeature("Business Line Admin");
            manageFeaturesPage.TeamCommentsReport_TurnOffSubFeature("Team Admin");
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as CA , OL, BL, TA then verify that team comments excel icon should not be displayed");
            topNavigation.LogOut();
            var usersList = new List<User>
            {
                CompanyAdmin1,
                OrganizationalLeaderUser,
                BusinessLineAdminUser,
                TeamAdminUser
            };
            foreach (var user in usersList)
            {
                login.LoginToApplication(user.Username, user.Password);
                mtDashboard.NavigateToPage(multiTeam.TeamId);
                mtDashboard.ClickOnRadar(SharedConstants.TeamAssessmentType);
                Assert.IsFalse(radarPage.IsTeamCommentsExcelButtonDisplayed(), "Team Comments Excel icon is displayed");
                topNavigation.LogOut();
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_TeamCommentsReport_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var topNavigation = new TopNavigation(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboard = new MtEtDashboardPage(Driver, Log);
            var multiTeam = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.MultiTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName}, Navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Turn off 'Team Comments Report'");
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);
            manageFeaturesPage.TurnOffTeamCommentsReport();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Log out as {User.FullName} and login as CA , OL, BL, TA then verify that team comments excel icon should not be displayed");
            topNavigation.LogOut();
            var usersList = new List<User>
            {
                CompanyAdmin1,
                OrganizationalLeaderUser,
                BusinessLineAdminUser,
                TeamAdminUser
            };
            foreach (var user in usersList)
            {
                login.LoginToApplication(user.Username, user.Password);
                mtDashboard.NavigateToPage(multiTeam.TeamId);
                mtDashboard.ClickOnRadar(SharedConstants.TeamAssessmentType);
                Assert.IsFalse(radarPage.IsTeamCommentsExcelButtonDisplayed(), "Team Comments Excel icon is displayed");
                topNavigation.LogOut();
            }
        }

        //Enable Share Assessment Results
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableShareAssessmentResult_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable Share Assessment Results' features ");
            manageFeaturesPage.TurnOnEnableShareAssessmentResult();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.SelectRadarLink(SharedConstants.ProgramHealthRadar, "Edit");
            Assert.IsTrue(taEditPage.IsStartSharingAssessmentButtonDisplayed(), "'Start Sharing Assessment Result' button is not displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableShareAssessmentResult_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable Share Assessment Results' features ");
            manageFeaturesPage.TurnOffEnableShareAssessmentResult();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            assessmentDashboard.NavigateToPage(team.TeamId);
            assessmentDashboard.SelectRadarLink(SharedConstants.ProgramHealthRadar, "Edit");
            Assert.IsFalse(taEditPage.IsStartSharingAssessmentButtonDisplayed(), "'Start Sharing Assessment Result' button is not displayed");
        }

        //Enable Hide Assessment Comments
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableHideAssessmentComments_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable Hide Assessment Comments' feature");
            manageFeaturesPage.TurnOnEnableHideAssessmentComments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.AssessmentHideUnHideCommentsRadar);

            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is not displayed");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Comments' button is not displayed");
            Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All Stakeholder Comments' button is not displayed");
            Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note), "'Hide' button is not displayed ");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void ManageFeatures_EnableHideAssessmentComments_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamsDashboard = new TeamDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.RadarTeam);

            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable Hide Assessment Comments' feature");
            manageFeaturesPage.TurnOffEnableHideAssessmentComments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName}");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamsDashboard.NavigateToPage(Company.Id);
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.AssessmentHideUnHideCommentsRadar);

            Assert.IsFalse(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is displayed");
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Comments' button is displayed");
            Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All Stakeholder Comments' button is displayed");
            Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note), "'Hide' button is displayed");
        }

        //Enable Pulse Assessments 
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnablePulseAssessment_On()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn on 'Enable Pulse Assessments' feature");
            manageFeaturesPage.TurnOnEnablePulseAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} user and login as {CompanyAdmin1.FullName} then verify pulse assessment link");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), "Pulse radio button is not displayed");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_EnablePulseAssessment_Off()
        {
            var login = new LoginPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var team = _allTeamsList.CheckForNull($"<{nameof(_allTeamsList)}> is null.").GetTeamByName(SharedConstants.Team);

            Log.Info($"login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn off 'Enable Pulse Assessments' feature");
            manageFeaturesPage.TurnOffEnablePulseAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info($"Logout as a {User.FullName} and login as {CompanyAdmin1.FullName} then verify pulse assessment link");
            topNav.LogOut();
            login.NavigateToPage();
            login.LoginToApplication(CompanyAdmin1.Username, CompanyAdmin1.Password);
            
            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            Assert.IsFalse(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), "Pulse radio button is displayed");
        }

        //Participant can view Talent Dev Aggregate Results
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AllParticipantsCanViewTalentDevAggregateResults_On()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var team = TeamFactory.GetGoiTeam("IndividualRadar");
            team.Members.Add(new AddMemberRequest
            {
                FirstName = Member.FirstName,
                LastName = Member.LastName,
                Email = Member.Username
            });
            _goiTeamResponse = _setup.CreateTeam(team, CompanyAdmin1).GetAwaiter().GetResult();

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            login.NavigateToPage();
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn On 'All Participants Can View Talent Dev Aggregate Results' feature");
            manageFeaturesPage.TurnOnAllParticipantsCanViewTalentDevAggregateResults();
            manageFeaturesPage.TurnOnNotifyUsersOfPendingAssessments();
            manageFeaturesPage.TurnOnIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Create an individual assessment");
            var assessment = IndividualAssessmentBaseTest.GetIndividualAssessment(_setup, _goiTeamResponse, "AllParticipantsCanViewTalentDevAggregateResults_", SiteAdminUser);
            dashboardPage.NavigateToPage(Company.Id);
            var teamId = dashboardPage.GetTeamIdFromLink(_goiTeamResponse.Name).ToInt();

            Log.Info($"Logout as {User.FullName} and Login as {Member.Username}");
            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(Member.Username, Member.Password);

            Log.Info($"Navigate to the {Constants.GoiTeam} Team");
            iAssessmentDashboardPage.NavigateToPage(teamId);

            Log.Info($"Verify that {assessment.Item2.AssessmentName}-Roll up radar and  {assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName} participant radar is present on Individual Assessment dashboard page");
            Assert.IsTrue(iAssessmentDashboardPage.IsRadarPresent(assessment.Item2.AssessmentName + " - Roll up"), $"Individual Roll up radar doesn't exists with name : {assessment.Item2.AssessmentName} - Roll up");
            Assert.IsTrue(iAssessmentDashboardPage.IsRadarPresent(assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName), $"Individual participant radar doesn't exists with name : {assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName}");

            Log.Info($"Click on {assessment.Item2.AssessmentName + " - Roll up"} radar and verify title");
            iAssessmentDashboardPage.ClickOnRadar(assessment.Item2.AssessmentName + " - Roll up");
            Assert.AreEqual(assessment.Item2.AssessmentName.ToLower(), radarPage.GetRadarTitle().ToLower(), "Roll up radar title didn't match");

            Log.Info($"Navigate back to Assessment dashboard page then click on {assessment.Item2.AssessmentName} - {Member.FirstName} {Member.LastName} radar and verify title");
            iAssessmentDashboardPage.NavigateToPage(teamId);
            iAssessmentDashboardPage.ClickOnRadar(assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName);
            Assert.AreEqual($"{assessment.Item2.AssessmentName} - {Member.FirstName} {Member.LastName} - {SharedConstants.IndividualAssessmentType}".ToLower(), radarPage.GetRadarTitle().ToLower(), "Participant radar title didn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public void ManageFeatures_AllParticipantsCanViewTalentDevAggregateResults_Off()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var manageFeaturesPage = new ManageFeaturesPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            // Create GOI team
            var team = TeamFactory.GetGoiTeam("IndividualRadar");
            team.Members.Add(new AddMemberRequest
            {
                FirstName = Member.FirstName,
                LastName = Member.LastName,
                Email = Member.Username
            });
            _goiTeamResponse = _setup.CreateTeam(team, CompanyAdmin1).GetAwaiter().GetResult();

            Log.Info($"Login as {User.FullName} and navigate to the 'Manage Feature' page");
            if (User.IsCompanyAdmin())
            {
                User = CompanyAdmin1;
            }
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageFeaturesPage.NavigateToPage(SettingsCompany.Id);

            Log.Info("Turn Off 'All Participants Can View Talent Dev Aggregate Results' feature");
            manageFeaturesPage.TurnOffAllParticipantsCanViewTalentDevAggregateResults();
            manageFeaturesPage.TurnOnNotifyUsersOfPendingAssessments();
            manageFeaturesPage.TurnOnIndividualAssessments();
            manageFeaturesPage.ClickUpdateButton();

            Log.Info("Create an Individual assessment");
            var assessment = IndividualAssessmentBaseTest.GetIndividualAssessment(_setup, _goiTeamResponse, "AllParticipantsCanViewTalentDevAggregateResults_", User);
            dashboardPage.NavigateToPage(Company.Id);
            var teamId = dashboardPage.GetTeamIdFromLink(_goiTeamResponse.Name).ToInt();

            Log.Info($"Logout as {User.FullName} and Login as {Member.Username}");
            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(Member.Username, Member.Password);

            Log.Info($"Navigate to the {Constants.GoiTeam} Team");
            iAssessmentDashboardPage.NavigateToPage(teamId);

            Log.Info($"Verify that {assessment.Item2.AssessmentName}-Roll up radar and  {assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName} participant radar is present on Individual Assessment dashboard page");
            Assert.IsFalse(iAssessmentDashboardPage.IsRadarPresent(assessment.Item2.AssessmentName + " - Roll up"), $"Individual Roll up radar exists with name : {assessment.Item2.AssessmentName} - Roll up");
            Assert.IsTrue(iAssessmentDashboardPage.IsRadarPresent(assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName), $"Individual participant radar doesn't exists with name : {assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName}");

            Log.Info($"Click on {assessment.Item2.AssessmentName} - {Member.FirstName} {Member.LastName} radar and verify title");
            iAssessmentDashboardPage.ClickOnRadar(assessment.Item2.AssessmentName + " - " + Member.FirstName + " " + Member.LastName);
            Assert.AreEqual($"{assessment.Item2.AssessmentName} - {Member.FirstName} {Member.LastName} - {SharedConstants.IndividualAssessmentType}".ToLower(), radarPage.GetRadarTitle().ToLower(), "Participant radar title didn't match");
        }
    }
}
