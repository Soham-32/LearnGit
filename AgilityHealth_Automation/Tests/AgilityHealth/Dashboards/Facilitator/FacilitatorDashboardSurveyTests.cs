using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Facilitator;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Facilitator
{
    [TestClass]
    [TestCategory("Dashboard"), TestCategory("FacilitatorDashboard")]
    public class FacilitatorDashboardSurveyTests : BaseTest
    {
        private static readonly User Facilitator = TestEnvironment.UserConfig.GetUserByDescription("facilitator");
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StartDate = DateTime.Today,
            SendRetroSurvey = true,
            SendRetroSurveyOption = "Launch after Facilitation Date",
            Facilitator = Facilitator.FullName,
            FacilitationDate = DateTime.Today,
        };
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {

            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            // add an assessment
            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, TeamAssessment);

            // complete a survey
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            setup.CompleteTeamMemberSurvey(emailSearch, ansValue: 5);
            TeamAssessment.EndDate = TeamAssessment.StartDate.AddDays(7);

            // launch the survey
            setup.LaunchAhfSurvey(_team.TeamId, TeamAssessment.AssessmentName);

        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("CompanyAdmin")]
        public void FacilitatorDashboard_SurveyCounts()
        {
            var login = new LoginPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var facilitatorDashboard = new FacilitatorDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.WaitForAssessmentToLoad(TeamAssessment.Facilitator, TeamAssessment.AssessmentName);
            
            var surveyCount = int.Parse(facilitatorDashboard.GetAssessmentColumnValue(TeamAssessment.Facilitator,
                TeamAssessment.AssessmentName, "No Of Responses"));

            Assert.AreEqual(0, surveyCount, $"Number of Responses value is incorrect for {TeamAssessment.AssessmentName}");

            var surveyLink = GmailUtil.GetSurveyLink("AHF's Assessment - Please Respond", Constants.TeamMemberEmail1,
                keyword: TeamAssessment.AssessmentName);

            surveyPage.NavigateToUrl(surveyLink);

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton();

            Driver.NavigateToPage(ApplicationUrl);

            teamDashboardPage.ClickFacilitatorDashboard();
            facilitatorDashboard.ActiveInactiveFacilitatorToggleButton(true);

            facilitatorDashboard.ExpandFacilitator(TeamAssessment.Facilitator);

            surveyCount = int.Parse(facilitatorDashboard.GetAssessmentColumnValue(TeamAssessment.Facilitator,
                TeamAssessment.AssessmentName, "No Of Responses"));

            Assert.AreEqual(1, surveyCount, $"Number of Responses value is incorrect for {TeamAssessment.AssessmentName}");
        }
        
    }
}