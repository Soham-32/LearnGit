using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanySubscriptionEmailNotificationTests : CompanyEditBaseTest
    {
        private static AddCompanyRequest _companyRequest;
        private static CompanyResponse _companyResponse;
        public static int NumberOfTeamCount;
        public static int NumberOfTeamAssessmentCount;

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CompanyFactory.GetCompany(partnerReferralCompany: User.CompanyName);
            _companyRequest.IsDraft = false;
            _companyRequest.AssessmentsLimit = 4;
            _companyRequest.TeamsLimit = 1;
            var setUp = new SetupTeardownApi(TestEnvironment);
            _companyResponse = setUp.CreateCompany(_companyRequest).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin")]
        public void Company_Subscription_DraftAssessment_NotCounted()
        {
            var expectedSubject = SharedConstants.AccountManagerEmailSubject(_companyRequest.Name);
            const string teamRadarName = SharedConstants.TeamHealthRadarName;
            const string individualRadarName = SharedConstants.IndividualAssessmentTypeAccessibleToAll;

            var login = new LoginPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var radarSelectionPage = new EditRadarSelectionPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            // TODO: When v2 end point is ready to send email notification, replace below action to add radar via API call.
            Log.Info($"Edit {_companyRequest.Name} company and select radars");
            companyDashboardPage.Search(_companyRequest.Name);
            companyDashboardPage.ClickEditIconByCompanyName(_companyRequest.Name);
            editCompanyProfilePage.Header.ClickOnRadarSelectionTab();
            radarSelectionPage.WaitUntilLoaded();
            radarSelectionPage.SelectRadar(teamRadarName);
            radarSelectionPage.SelectRadar(individualRadarName);
            radarSelectionPage.Header.ClickSaveButton();
            companyDashboardPage.WaitUntilLoaded();

            // TODO: When v2 end point is ready to send email notification, replace below action to sign up as CA user (via UI) / add team via API call
            Log.Info("Create a team and verify account manager email for reaching team limits and assessment limits");
            dashBoardPage.NavigateToPage(_companyResponse.Id);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.Team);
            dashBoardPage.ClickAddTeamButton();
            var teamInfo = new TeamInfo()
            {
                TeamName = "Team_" + RandomDataUtil.GetTeamName(),
                WorkType = SharedConstants.NewTeamWorkType
            };
            createTeamPage.EnterTeamInfo(teamInfo);
            createTeamPage.ClickCreateTeamAndAddTeamMembers();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToAssessmentDashboard();

            NumberOfTeamCount++;

            var emailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, _companyRequest.AccountManagerEmail, "", "UNREAD");
            Assert.IsTrue(emailBody!=null, "Account manager haven't received any email");
            Assert.IsTrue(emailBody.Contains($"Number of teams:   {NumberOfTeamCount}"), $"Number of teams:  {NumberOfTeamCount} - message is not present in email body");
            Assert.IsTrue(emailBody.Contains($"Number of team assessments: {NumberOfTeamAssessmentCount}"),$"Number of team assessments: {NumberOfTeamAssessmentCount} - message is not present in email body");

            Log.Info("Create a team assessment & publish and verify account manager email for reaching team limits and assessment limits");
            teamAssessmentDashboard.AddAnAssessment("Team");
            var assessmentName = "TeamAssessment_" + RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(teamRadarName, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();
            
            NumberOfTeamAssessmentCount++;
            emailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, _companyRequest.AccountManagerEmail, "", "UNREAD");
            Assert.IsTrue(emailBody != null, "Account manager haven't received any email");
            Assert.IsTrue(emailBody.Contains($"Number of teams:   {NumberOfTeamCount}"), $"Number of teams:  {NumberOfTeamCount} - message is not present in email body");
            Assert.IsTrue(emailBody.Contains($"Number of team assessments: {NumberOfTeamAssessmentCount}"), $"Number of team assessments: {NumberOfTeamAssessmentCount} - message is not present in email body");

            Log.Info("Create a team assessment & save as a draft and verify email is not sent to account manager");
            teamAssessmentDashboard.AddAnAssessment("Team");
            assessmentName = "DraftAssessment_" + RandomDataUtil.GetAssessmentName();
            assessmentProfile.FillDataForAssessmentProfile(teamRadarName, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnSaveAsDraft();
           
            emailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, _companyRequest.AccountManagerEmail, "", "UNREAD");
            Assert.IsFalse(emailBody != null, "Account manager received an email");
           
            Log.Info("Publish draft assessment and verify account manager email for reaching team limits and assessment limits");
            teamAssessmentDashboard.ClickOnRadar(assessmentName);
            reviewAndLaunch.ClickOnPublish();

            NumberOfTeamAssessmentCount++;
            emailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, _companyRequest.AccountManagerEmail, "", "UNREAD");
            Assert.IsTrue(emailBody != null, "Account manager haven't received any email");
            Assert.IsTrue(emailBody.Contains($"Number of teams:   {NumberOfTeamCount}"), $"Number of teams:  {NumberOfTeamCount} - message is not present in email body");
            Assert.IsTrue(emailBody.Contains($"Number of team assessments: {NumberOfTeamAssessmentCount}"), $"Number of team assessments: {NumberOfTeamAssessmentCount} - message is not present in email body");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }
    }
}