using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Members;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentAddNewMemberAndStakeholderTests : BaseTest
    {
        public static bool ClassInitFailed;
        private static int _teamId;
        private static readonly TeamMemberInfo TeamMemberInfo1 = new TeamMemberInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "member1" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
        };
        private static readonly TeamMemberInfo TeamMemberInfo2 = new TeamMemberInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "member2" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
        };
        private static readonly StakeHolderInfo StakeholderInfo1 = new StakeHolderInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "stakeholder1" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
            Role = SharedConstants.StakeholderRole
        };
        private static readonly StakeHolderInfo StakeholderInfo2 = new StakeHolderInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "stakeholder2" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
            Role = SharedConstants.StakeholderRole
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team");
                var teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;

            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }



        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_AddNewMemberAndStakeholder()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var teamMemberCommon = new TeamMemberCommon(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var stakeholderCommon = new StakeHolderCommon(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            // Verify 'Cancel' button from 'Add New Team Member' popup
            Log.Info("Enter members details, click on 'Cancel' button, verify that member is not added");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo1);
            selectTeamMembers.AddNewMembersPopupClickOnCancelButton();

            var expectedTeamMember1 = TeamMemberInfo1.Email;
            Assert.IsFalse(addTeamMemberPage.IsTeamMemberDisplayed(expectedTeamMember1), $"Member {expectedTeamMember1} is added");

            // Verify 'Save and Add New' button from 'Add New Team Member' popup
            Log.Info("Enter members details, click on 'Save and Add New' button, verify title of popup, close the popup, verify added member in grid");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo1);
            selectTeamMembers.AddNewMembersPopupClickOnSaveAndAddNewButton();
            Assert.IsTrue(teamMemberCommon.IsAddNewTeamMembersPopupDisplayed(), "'Add New Team Member' popup is not displayed");

            selectTeamMembers.AddNewMembersPopupClickOnCloseIcon();
            Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(expectedTeamMember1), $"Team member {expectedTeamMember1} is not added");

            // Verify 'Save and Close' button from 'Add New Team Member' popup
            Log.Info("Enter members details, click on 'Save and Close' button, verify added member in grid");
            teamMemberCommon.ClickAddNewTeamMemberButton();
            addTeamMemberPage.EnterTeamMemberInfo(TeamMemberInfo2);
            selectTeamMembers.ClickSaveAndCloseButton();

            selectTeamMembers.SelectTeamMemberByName(TeamMemberInfo1.FirstName + " " + TeamMemberInfo1.LastName);

            var expectedTeamMember2 = TeamMemberInfo2.Email;
            Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(expectedTeamMember2), $"Team member {expectedTeamMember2} is not added");

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            // Verify 'Cancel' button from 'Add New Stakeholder' popup
            Log.Info("Enter stakeholders details, click on 'Cancel' of popup, verify that stakeholder is not added");
            stakeholderCommon.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(StakeholderInfo1);
            selectStakeHolder.AddNewMembersPopupClickOnCancelButton();

            var expectedStakeholder1 = StakeholderInfo1.Email;
            Assert.IsFalse(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholder1), $"Stakeholder {expectedStakeholder1} is added");

            // Verify 'Save and Add New' button from 'Add New Stakeholder' popup
            Log.Info("Enter stakeholders details, click on 'Save and Add New' button, verify title of popup, close the popup, verify added stakeholder in grid");
            stakeholderCommon.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(StakeholderInfo1);
            selectStakeHolder.AddNewMembersPopupClickOnSaveAndAddNewButton();
            Assert.IsTrue(addStakeHolderPage.IsAddNewStakeholderPopupDisplayed(), "'Add New Team Member' popup is not displayed");

            selectStakeHolder.AddNewMembersPopupClickOnCloseIcon();
            Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholder1), $"Team member {expectedStakeholder1} is not added");

            // Verify 'Save and Close' button from 'Add New Stakeholder' popup
            Log.Info("Enter stakeholders details, click on 'Save and Close' button, verify added stakeholder in grid");
            stakeholderCommon.ClickAddNewStakeHolderButton();
            addStakeHolderPage.EnterStakeHolderInfo(StakeholderInfo2);
            selectStakeHolder.ClickSaveAndCloseButton();
            selectStakeHolder.SelectStakeHolderByName(StakeholderInfo1.FirstName + " " + StakeholderInfo1.LastName);

            var expectedStakeholder2 = StakeholderInfo2.Email;
            Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholder2), $"Stakeholder {expectedStakeholder2} is not added");

            Log.Info("Go to 'Review & Launch' page and verify 'Team Members & Stakeholders'");
            selectStakeHolder.ClickOnReviewAndFinishButton();
            var expectedMembers = new Dictionary<string, string>
            {
                { TeamMemberInfo1.FirstName + " " + TeamMemberInfo1.LastName, TeamMemberInfo1.Email},
                { TeamMemberInfo2.FirstName + " " + TeamMemberInfo2.LastName, TeamMemberInfo2.Email},
            };
            var expectedStakeholders = new Dictionary<string, string>
            {
                { StakeholderInfo1.FirstName + " " + StakeholderInfo1.LastName, StakeholderInfo1.Email},
                { StakeholderInfo2.FirstName + " " + StakeholderInfo2.LastName, StakeholderInfo2.Email},
            };

            var actualMembers = reviewAndLaunch.GetTeamMembers();
            var actualStakeholders = reviewAndLaunch.GetStakeholders();

            Assert.That.ListsAreEqual(expectedMembers.Keys.ToList(), actualMembers.Keys.ToList());
            Assert.That.ListsAreEqual(expectedMembers.Values.ToList(), actualMembers.Values.ToList());
            Assert.That.ListsAreEqual(expectedStakeholders.Keys.ToList(), actualStakeholders.Keys.ToList());
            Assert.That.ListsAreEqual(expectedStakeholders.Values.ToList(), actualStakeholders.Values.ToList());
        }
    }
}