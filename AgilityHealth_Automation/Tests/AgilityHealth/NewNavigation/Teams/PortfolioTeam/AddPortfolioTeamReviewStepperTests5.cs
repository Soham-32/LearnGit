using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamReviewStepperTests5 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Portfolio_Team_Verify_Expand_Collapse_All_Sections()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createPortfolioTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var reviewTeamStepperPage = new ReviewStepperPage(Driver, Log);

            var portfolioTeamInfo = PortfolioTeamsFactory.GetValidPortfolioTeamInfo();
            var leadersInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation then switch to grid view on team dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();
            teamDashboardPage.SwitchToGridView();

            Log.Info("Click on 'Add a New Team' button and select 'Portfolio Team' type to create a portfolio team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.PortfolioTeam);

            Log.Info("Enter portfolio team info and click on 'continue' button till 'Review' stepper");
            createPortfolioTeamStepperPage.EnterTeamInfo(portfolioTeamInfo);
            createPortfolioTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addSubTeamsStepperPage.AssignSubTeam(SharedConstants.MultiTeam);
            addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton();
            addLeadersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            addLeadersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            addLeadersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            addLeadersStepperPage.ClickOnContinueToReviewButton();

            //Click on 'v' collapse of Team Tag Section
            Log.Info("Click on portfolio team tags section and validate the portfolio team tags section is collapsed");
            var actualMultiTeamTagsText = reviewTeamStepperPage.GetTeamTagsText();
            reviewTeamStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsFalse(reviewTeamStepperPage.IsTeamTagsSectionExpanded(), "Portfolio team Tag section is not collapsed");

            //click on again 'v' expand of Team Tag Section 
            Log.Info("Click on portfolio team tags section and validate the portfolio team tags info on 'Review' stepper");
            reviewTeamStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsTrue(reviewTeamStepperPage.IsTeamTagsSectionExpanded(), "Portfolio team Tag section is collapsed");
            CollectionAssert.AreEquivalent(portfolioTeamInfo.Tags, actualMultiTeamTagsText, "Portfolio tags are not matching");

            //Click on 'v' collapse of Sub Teams Section
            Log.Info("Click on portfolio team tags section and validate the portfolio team tags section is collapsed");
            reviewTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsFalse(reviewTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Portfolio team Tag section is not collapsed");

            //click on again 'v' expand of Sub Teams Section 
            Log.Info("Click on portfolio team tags section and validate the portfolio team tags info on 'Review' stepper");
            reviewTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsTrue(reviewTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Portfolio team Tag section is collapsed");
            Assert.IsTrue(reviewTeamStepperPage.GetSubTeamsTextList().Contains(SharedConstants.MultiTeam), "Failure !! Sub-team: " + SharedConstants.MultiTeam + " does not display in Finish and Review page");

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
