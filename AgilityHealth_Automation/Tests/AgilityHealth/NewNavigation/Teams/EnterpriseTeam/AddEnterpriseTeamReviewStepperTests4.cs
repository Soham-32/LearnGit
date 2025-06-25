using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamReviewStepperTests4 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Enterprise_Team_Verify_Expand_Collapse_All_Sections()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var reviewTeamStepperPage = new ReviewStepperPage(Driver, Log);

            var enterpriseTeamInfo = EnterpriseTeamsFactory.GetValidEnterpriseTeamInfo();
            var leadersInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();


            Log.Info("Click on 'Add a New Team' button and select 'Portfolio Team' type to create a portfolio team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.EnterpriseTeam);

            Log.Info("Enter portfolio team info and click on 'continue' button till 'Review' stepper");
            createEnterpriseTeamStepperPage.EnterTeamInfo(enterpriseTeamInfo);
            createEnterpriseTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addSubTeamsStepperPage.AssignSubTeam(SharedConstants.PortfolioTeam);
            addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton();
            addLeadersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            addLeadersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            addLeadersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            addLeadersStepperPage.ClickOnContinueToReviewButton();

            //Click on 'v' collapse of Sub Teams Section
            Log.Info("Click on enteprise sub team section and validate the enteprise sub team section is collapsed");
            reviewTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsFalse(reviewTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Enteprise sub team section is not collapsed");

            //click on again 'v' expand of Sub Teams Section 
            Log.Info("Click on enteprise sub team section and validate the enteprise sub team info on 'Review' stepper");
            reviewTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsTrue(reviewTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Enteprise sub teamg section is collapsed");
            Assert.IsTrue(reviewTeamStepperPage.GetSubTeamsTextList().Contains(SharedConstants.PortfolioTeam), "Failure !! Sub-team: " + SharedConstants.MultiTeam + " does not display in Finish and Review page");

            //Click on 'v' collapse of Leader Section
            Log.Info("Click on leaders section and validate the leaders section is collapsed");
            reviewTeamStepperPage.ClickOnLeadersSection();
            Assert.IsFalse(reviewTeamStepperPage.IsLeadersSectionExpanded(), "Leader section is not collapsed");

            //Click on again 'v' collapse of Leader Section
            Log.Info("Click on leaders section and validate the leaders info on 'Review' stepper");
            reviewTeamStepperPage.ClickOnLeadersSection();
            var memberInfo = reviewTeamStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.IsTrue(reviewTeamStepperPage.IsLeadersSectionExpanded(), "Leader section is collapsed");
            Assert.AreEqual(leadersInfo.FirstName, memberInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, memberInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, memberInfo.Email, "Email doesn't match");
        }
    }
}
