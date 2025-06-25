using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.AhTrial;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial")]
    public class AhTrialCompanyManageSubscriptionAndAccountManagerDetailTests : BaseTest
    {
        private readonly SetupTeardownApi SetupApi = new SetupTeardownApi(TestEnvironment);

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void AhTrial_AccountManagerDetails_ValidationEmail_For_EnterpriseTrial_Type()
        {
            const int enterpriseTeamsLimit = 10;
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany();
            var ahTrialBaseCompanyResponse = SetupApi.CreateAhTrialCompany(ahTrialCompanyRequest);

            AhTrial_CheckManageSubscription_And_Verify_AccountManager_Details(ahTrialBaseCompanyResponse, ahTrialCompanyRequest, enterpriseTeamsLimit);
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void AhTrial_AccountManagerDetails_ValidationEmail_For_SmallBusinessTrial_Type()
        {
            const int smallBusinessTeamsLimit = 5;
            var ahTrialCompanyRequest = AhTrialFactory.GetValidAhTrialCompany("SmallBusiness");
            var ahTrialBaseCompanyResponse = SetupApi.CreateAhTrialCompany(ahTrialCompanyRequest);

            AhTrial_CheckManageSubscription_And_Verify_AccountManager_Details(ahTrialBaseCompanyResponse, ahTrialCompanyRequest, smallBusinessTeamsLimit);
        }

        public void AhTrial_CheckManageSubscription_And_Verify_AccountManager_Details(AhTrialBaseCompanyResponse ahTrialBaseCompanyResponse, AhTrialCompanyRequest ahTrialCompanyRequest, int teamLimit)
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var editSubscriptionPage = new EditCompanySubscriptionPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createTeamPage = new CreateTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);

            var user = new User(ahTrialCompanyRequest.Email, ahTrialCompanyRequest.Password);

            Log.Info($"Create {teamLimit} teams in {ahTrialBaseCompanyResponse.Name} company");
            for (var i = 1; i < teamLimit; i++)
            {
                SetupApi.CreateTeam(TeamFactory.GetNormalTeam("TestQuickLaunchAssessment"), user).GetAwaiter().GetResult();
            }

            Log.Info("Login as SA user and search on company dashboard then verify that newly created company displayed or not");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.Search(ahTrialBaseCompanyResponse.Name);
            Assert.IsTrue(companyDashboardPage.IsCompanyPresent(ahTrialBaseCompanyResponse.Name), $"Company {ahTrialBaseCompanyResponse.Name} is not present");

            Log.Info("Edit newly created company and enter mandatory data");
            companyDashboardPage.ClickEditIconByCompanyName(ahTrialBaseCompanyResponse.Name);
            var editedCompany = CompanyFactory.GetCompanyForUpdate();
            editCompanyProfilePage.SelectCompanySize(editedCompany.Size);
            editCompanyProfilePage.SelectLifeCycleStage(editedCompany.LifeCycleStage);
            editCompanyProfilePage.SelectCompanyType(editedCompany.CompanyType);

            Log.Info("Go to 'Subscription' tab and verify that 'Managed Subscription' checkbox checked or not");
            editSubscriptionPage.Header.ClickOnSubscriptionTab();
            Assert.IsTrue(editSubscriptionPage.IsManagedSubscriptionCheckboxSelected(), "Managed Subscription checkbox info is not matched");

            Log.Info("Verify account manager info then update account manager email and update company");
            var actualAccountManager = editSubscriptionPage.GetAccountManagerInfo();
            Assert.AreEqual("AH", actualAccountManager.AccountManagerFirstName, "AccountManagerFirstName  is not matched");
            Assert.AreEqual("Sales", actualAccountManager.AccountManagerLastName, "AccountManagerLastName  is not matched");
            Assert.AreEqual("sales@agilityinsights.ai", actualAccountManager.AccountManagerEmail, "AccountManagerEmail is not matched");
            Assert.AreEqual("4028580829", actualAccountManager.AccountManagerPhone, "AccountManagerPhoneNumber is not matched");

            var updatedAccountManagerEmail = Constants.UserEmailPrefix + "_AccountManager_" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain;
            editSubscriptionPage.SetAccountManagerEmail(updatedAccountManagerEmail);
            editSubscriptionPage.Header.ClickSaveButton();

            Log.Info("Go to team dashboard for newly created company and create a new team then verify that 'reaching team limits' validation email is received by account manager");
            dashBoardPage.NavigateToPage(ahTrialBaseCompanyResponse.Id);
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
            finishAndReviewPage.ClickOnGoToTeamDashboard();

            var expectedSubject = SharedConstants.AccountManagerEmailSubject(ahTrialBaseCompanyResponse.Name);
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, updatedAccountManagerEmail, "Inbox"), $"Email NOT received <{expectedSubject}> sent to {updatedAccountManagerEmail}");

            var emailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, updatedAccountManagerEmail, "Inbox");
            Assert.IsTrue(emailBody.Contains($"Number of teams:   {teamLimit}"), $"Number of teams:  {teamLimit} - message is not present in email body");

            Log.Info("Create a new team and verify 'Upgrade Your Subscription' popup");
            dashBoardPage.ClickAddATeamButton();
            Assert.AreEqual("Upgrade Your Subscription", createTeamPage.GetUpgradeYourSubscriptionPopupHeaderText(), "'Upgrade Your Subscription' is not displayed");
        }
    }
}

