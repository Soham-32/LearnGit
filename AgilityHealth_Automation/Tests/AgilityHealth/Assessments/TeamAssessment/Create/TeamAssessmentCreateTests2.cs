using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateTests2 : BaseTest
    {

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_SendToEveryone()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            Log.Info("Test: Verify that user can add an assessment [Send To Everyone]");
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.AddAnAssessment("Team");

            var assessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName1);
            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName2);

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.SelectStakeHolderByName(Constants.StakeholderName2);
            selectStakeHolder.SelectStakeHolderByName(Constants.StakeholderName3);

            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("verifying Assessment Name/Team members/Stakeholders info ");
            //verifying assessment name
            var actualText = reviewAndLaunch.GetAssessmentName();
            Assert.IsTrue(actualText.Contains(assessmentName), 
                $"Assessment name does not match. Expected: <{assessmentName}>, Actual: <{actualText}>");

            //Verifying team members info
            var expectedMembers = new Dictionary<string, string>
            {
                { Constants.TeamMemberName1, Constants.TeamMemberEmail1 },
                { Constants.TeamMemberName2, Constants.TeamMemberEmail2 }
            };

            var actualMembers = reviewAndLaunch.GetTeamMembers();

            foreach (var expected in expectedMembers)
            {
                Assert.IsTrue(actualMembers.ContainsKey(expected.Key), 
                    $"Could not find team member with name {expected.Key}");
                Assert.AreEqual(expected.Value, actualMembers[expected.Key], 
                    $"Email address does not match for <{expected.Key}>");
            }
            
            //Verifying stakeholder info
            var expectedStakeholders = new Dictionary<string, string>
            {
                { Constants.StakeholderName2, Constants.StakeholderEmail2 },
                { Constants.StakeholderName3, Constants.StakeholderEmail3 }
            };
            
            var actualStakeholders = reviewAndLaunch.GetStakeholders();

            foreach (var expected in expectedStakeholders)
            {
                Assert.IsTrue(actualStakeholders.ContainsKey(expected.Key), 
                    $"Could not find stakeholder with name {expected.Key}");
                Assert.AreEqual(expected.Value, actualStakeholders[expected.Key], 
                    $"Email address does not match for <{expected.Key}>");
            }

            reviewAndLaunch.SelectSendToEveryone();

            reviewAndLaunch.ClickOnPublish();

            Log.Info("Verify that Assessment is launched and displayed correctly on Dashboard ");

            var data = teamAssessmentDashboard.GetAssessmentStatus(assessmentName);
            Assert.AreEqual("Open", data[0],"Assessment status is incorrect");
            Assert.AreEqual("disc green", data[1],"Assessment indicator is incorrect");

            teamAssessmentDashboard.ClickOnRadar(assessmentName);

            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/radar"), 
                $"Not Navigated to Assessment radar page i.e. Assessment is NOT lunched successfully. Navigated to {Driver.GetCurrentUrl()}");

            Log.Info("Verifying where assessments are received by Team Members");
            var expectedSubject = SharedConstants.TeamAssessmentSubject(_team.Name, assessmentName);
            foreach (var teamMember in expectedMembers)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember.Value), 
                    $"Email NOT received <{expectedSubject}> sent to {teamMember.Value}");
            }

            Log.Info("Verifying where assessments are received by Stake Holder");
            foreach (var stakeholder in expectedStakeholders)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, stakeholder.Value), 
                    $"Email NOT received <{expectedSubject}> sent to {stakeholder.Value}");
            }

            Log.Info("Navigate back to Assessment dashboard and verifying Pulse radio is displayed or not after the assessment is published but not completed");
            Driver.Back();
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), "Pulse radio button is not displayed");
            teamAssessmentDashboard.ClickOnCloseIconPopup();
        }
    }
}