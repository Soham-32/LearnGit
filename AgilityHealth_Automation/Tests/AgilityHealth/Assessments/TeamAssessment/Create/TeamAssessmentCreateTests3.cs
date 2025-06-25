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
    public class TeamAssessmentCreateTests3 : BaseTest
    {

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_SendToStakeHolder()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            Log.Info("Test: User is able to send assessment to Stakeholders");
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

            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamAssessmentType;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.SelectStakeHolderByName(Constants.StakeholderName2);
            selectStakeHolder.SelectStakeHolderByName(Constants.StakeholderName3);

            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Verify that user is able to see the emails of stakeholders as selected");

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

            reviewAndLaunch.SelectSendToStakeHolder();

            reviewAndLaunch.ClickOnPublish();
            
            var expectedSubject = SharedConstants.TeamAssessmentSubject(_team.Name, assessmentName);
            Log.Info("Verifying where assessments are received by Stakeholders");
            foreach (var stakeholder in expectedStakeholders)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, stakeholder.Value), 
                    $"Email NOT received <{expectedSubject}> sent to {stakeholder.Value}");
            }
        }
    }
}