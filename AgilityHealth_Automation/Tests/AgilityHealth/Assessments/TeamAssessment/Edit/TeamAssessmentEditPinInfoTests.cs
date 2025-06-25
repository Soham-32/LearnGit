using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Edit
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentEditPinInfoTests : BaseTest
    {
        private static bool _classInitFailed;
        private static SetUpMethods _setup;
        private static TeamHierarchyResponse _team;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName(), SharedConstants.Stakeholder2.FullName() }
        };
        private static readonly List<string> TeamMembersEmailList = new List<string> { SharedConstants.TeamMember1.Email, SharedConstants.TeamMember2.Email };
        private static readonly List<string> StakeHoldersEmailList = new List<string> { SharedConstants.Stakeholder1.Email, SharedConstants.Stakeholder2.Email };


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _setup = new SetUpMethods(testContext, TestEnvironment);
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")]
        [TestCategory("KnownDefectAsBL")]
        [TestCategory("KnownDefectAsOL")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditTeamMemberPinInfo()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamAssessmentPinInfoPage = new TeamAssessmentPinInfoPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Verify PinAccess popup is displayed for {TeamAssessment.AssessmentName} after clicking on Display Pin button ");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            var teamAssessmentEditUrl = Driver.GetCurrentUrl();
            teamAssessmentEditPage.ClickOnDisplayPinInfoButtonForTeamMember();
            Assert.IsTrue(teamAssessmentEditPage.IsPinAccessPopupDisplay(), "Pin Access popup is not displayed");

            var pin = teamAssessmentEditPage.GetPinFromPinAccessPopup();
            var assessmentUrl = teamAssessmentEditPage.GetUrlFromPinAccessPopup();
            teamAssessmentEditPage.ClickOnPinAccessPopupCloseButton();

            Log.Info($"Go to Team assessment and fill the survey for {Constants.TeamMemberName1} & {Constants.TeamMemberName2}");
            foreach (var teamMemberEmail in TeamMembersEmailList)
            {
                surveyPage.NavigateToUrl(assessmentUrl);
                var request = new AssessmentPinRequest()
                {
                    Pin = pin,
                    Email = teamMemberEmail
                };

                teamAssessmentPinInfoPage.FillAssessmentAccessDetails(request);
                surveyPage.ConfirmIdentity();
                surveyPage.ClickStartSurveyButton();
                surveyPage.SubmitRandomSurvey();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickFinishButton();
            }

            Log.Info("Verify that survey emails are received by TeamMembers");
            teamAssessmentDashboard.NavigateToUrl(teamAssessmentEditUrl);
            TeamMembersEmailList.ForEach(teamMemberEmail => Assert.IsTrue(teamAssessmentEditPage.IsTeamMembersReopenAssessmentIconDisplayed(teamMemberEmail), "'Team Member Reopen Assessment Icon' is not displayed for team member"));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessmentEditStakeHolderPinInfo()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var teamAssessmentEditPage = new TeamAssessmentEditPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var teamAssessmentPinInfoPage = new TeamAssessmentPinInfoPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Verify PinAccess popup is displayed for {TeamAssessment.AssessmentName} after clicking on Display Pin button ");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            var teamAssessmentEditUrl = Driver.GetCurrentUrl();
            teamAssessmentEditPage.ClickOnDisplayPinInfoButtonForStakeholder();
            Assert.IsTrue(teamAssessmentEditPage.IsPinAccessPopupDisplay(), "Pin Access popup is not displayed");

            var pin = teamAssessmentEditPage.GetPinFromPinAccessPopup();
            var assessmentUrl = teamAssessmentEditPage.GetUrlFromPinAccessPopup();
            teamAssessmentEditPage.ClickOnPinAccessPopupCloseButton();

            Log.Info($"Go to Team assessment and fill the survey {Constants.StakeholderName1} & {Constants.StakeholderName2}");
            foreach (var stakeholderEmail in StakeHoldersEmailList)
            {
                surveyPage.NavigateToUrl(assessmentUrl);
                var request = new AssessmentPinRequest()
                {
                    Pin = pin,
                    Email = stakeholderEmail
                };
                teamAssessmentPinInfoPage.FillAssessmentAccessDetails(request);
                surveyPage.ConfirmIdentity();
                surveyPage.ClickStartSurveyButton();
                surveyPage.SubmitRandomSurvey();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickFinishButton();
            }

            Log.Info("Verify that survey emails are received by StakeHolders");
            teamAssessmentDashboardPage.NavigateToUrl(teamAssessmentEditUrl);
            StakeHoldersEmailList.ForEach(stakeholderEmail => Assert.IsTrue(teamAssessmentEditPage.IsStakeHolderReopenAssessmentIconDisplayed(stakeholderEmail), "'Stakeholder Reopen Assessment Icon' is not displayed for StakeHolders"));
        }
    }
}