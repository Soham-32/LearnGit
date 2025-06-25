using AgilityHealth_Automation.Enum.NewNavigation;
using AgilityHealth_Automation.ObjectFactories.NewNavigation.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class AddMultiTeamReviewStepperTests5 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48260
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Multi_Team_Verify_Expand_Collapse_All_Sections()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var createMultiTeamStepperPage = new CreateTeamStepperPage(Driver, Log);
            var addSubTeamsStepperPage = new AddTeamSubTeamStepperPage(Driver, Log);
            var addLeadersStepperPage = new AddTeamMembersStepperPage(Driver, Log);
            var reviewMultiTeamStepperPage = new ReviewStepperPage(Driver, Log);

            var multiTeamInfo = MultiTeamsFactory.GetValidMultiTeamInfo();
            var leadersInfo = TeamsFactory.GetTeamMemberInfo();

            Log.Info("Login to the application and switch to new navigation");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            SwitchToNewNav();

            Log.Info("Click on 'Add a New Team' button and select 'Multi-Team' type to create a multi team");
            teamDashboardPage.ClickOnAddNewTeamButtonAndSelectTeamType(TeamType.MultiTeam);

            Log.Info("Enter multi team info and click on 'continue' button till 'Review' stepper");
            createMultiTeamStepperPage.EnterTeamInfo(multiTeamInfo);
            createMultiTeamStepperPage.ClickOnContinueToSubTeamOrTeamMemberButton();
            addSubTeamsStepperPage.AssignSubTeam(SharedConstants.Team);
            addSubTeamsStepperPage.ClickOnContinueToAddLeadersButton();
            addLeadersStepperPage.OpenAddTeamMembersOrLeadersPopup();
            addLeadersStepperPage.PopupBase.EnterTeamMemberOrLeadersInfo(leadersInfo);
            addLeadersStepperPage.PopupBase.ClickOnCreateAndCloseButton();
            addLeadersStepperPage.ClickOnContinueToReviewButton();

            //Click on 'v' collapse of Team Tag Section
            Log.Info("Click on multi team tags section and validate the multi team tags section is collapsed");
            var actualMultiTeamTagsText = reviewMultiTeamStepperPage.GetTeamTagsText();
            reviewMultiTeamStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsFalse(reviewMultiTeamStepperPage.IsTeamTagsSectionExpanded(), "Multi team Tag section is expanded");

            //click on again 'v' expand of Team Tag Section 
            Log.Info("Click on multi team tags section and validate the multi team tags info on 'Review' stepper");
            reviewMultiTeamStepperPage.ClickOnTeamTagsSectionHeader();
            Assert.IsTrue(reviewMultiTeamStepperPage.IsTeamTagsSectionExpanded(), "Multi team Tag section is not expanded");
            CollectionAssert.AreEquivalent(multiTeamInfo.Tags, actualMultiTeamTagsText, "Multi tags are not matching");

            //Click on 'v' collapse of Sub Teams Section
            Log.Info("Click on multi team tags section and validate the multi team tags section is collapsed");
            reviewMultiTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsFalse(reviewMultiTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Multi team Tag section is not collapsed");

            //click on again 'v' expand of Sub Teams Section 
            Log.Info("Click on multi team tags section and validate the multi team tags info on 'Review' stepper");
            reviewMultiTeamStepperPage.ClickOnSubTeamsSection();
            Assert.IsTrue(reviewMultiTeamStepperPage.IsMultiSubTeamsSectionExpanded(), "Multi team Tag section is collapsed");
            Assert.IsTrue(reviewMultiTeamStepperPage.GetSubTeamsTextList().Contains(SharedConstants.Team), "Failure !! Sub-team: " + SharedConstants.Team + " does not display in Finish and Review page");

            //Click on 'v' collapse of Leader Section
            Log.Info("Click on leaders section and validate the leaders section is collapsed");
            reviewMultiTeamStepperPage.ClickOnLeadersSection();
            Assert.IsFalse(reviewMultiTeamStepperPage.IsLeadersSectionExpanded(), "Leader section is not collapsed");

            //Click on again 'v' collapse of Leader Section
            Log.Info("Click on leaders section and validate the leaders info on 'Review' stepper");
            reviewMultiTeamStepperPage.ClickOnLeadersSection();
            var memberInfo = reviewMultiTeamStepperPage.GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(leadersInfo.Email);
            Assert.IsTrue(reviewMultiTeamStepperPage.IsLeadersSectionExpanded(), "Leader section is collapsed");
            Assert.AreEqual(leadersInfo.FirstName, memberInfo.FirstName, "First name doesn't match");
            Assert.AreEqual(leadersInfo.LastName, memberInfo.LastName, "Last name doesn't match");
            Assert.AreEqual(leadersInfo.Email, memberInfo.Email, "Email doesn't match");
        }
    }
}
