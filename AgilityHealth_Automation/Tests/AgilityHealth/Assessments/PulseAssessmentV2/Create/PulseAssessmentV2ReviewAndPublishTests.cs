using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2ReviewAndPublishTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static int _teamId;
        private static IList<TeamV2Response> _teamMemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("PulseV2Team", 1);

                var teamResponse = setupApi.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;

                //Add teamMember
                var member = PulseV2Factory.GetValidTeamMemberWithRole("Team Member");
                setupApi.AddTeamMembers(teamResponse.Uid, member);
                _teamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_ReviewAndPublish_EditStepperIcons()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var expectedQuestionCountByCompetency = new List<string>();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Team Assessment dashboard' and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            Log.Info("Filling the valid data in stepper 1(Create Pulse Check page)");
            var pulseData = PulseV2Factory.GetPulseAddData();
            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Filling the valid data in stepper 2(Select Questions page)");
            selectQuestionPage.FillForm(pulseData);
            var dimensionName = pulseData.Questions.First().DimensionName;
            selectQuestionPage.Header.ClickOnSelectRecipientsStepper();

            Log.Info("Get all the teams from 'Select Recipients page'");
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            var allTeams = selectRecipientsPage.GetListOfTeams();
            selectRecipientsPage.Header.ClickOnNextButton();

            Log.Info("Verify that 'Publish' and 'Save as Draft' button is displayed on 'Review and Publish' page");
            Assert.IsTrue(reviewAndPublishPage.IsPublishButtonDisplayed(), $"Publish button is not displayed on {reviewAndPublishPage} page");
            Assert.IsTrue(reviewAndPublishPage.IsSaveAsDraftButtonDisplayed(), $"SaveAsDraft button is not displayed on {reviewAndPublishPage} page");

            Log.Info("Click on edit icon of 'Assessment Details' section, Navigate to 'Create Pulse Check' page and verify page title");
            reviewAndPublishPage.ClickOnAssessmentDetailsPencilIcon();
            Assert.IsTrue(createPulseCheckPage.IsCreatePulseCheckTitleDisplayed(), "User is not navigated to stepper 1 - Create Pulse check page");

            var assessmentName = createPulseCheckPage.GetAssessmentName();
            var assessmentType = createPulseCheckPage.GetAssessmentType();
            var actualAssessmentDetails = createPulseCheckPage.GetScheduleSectionData();

            createPulseCheckPage.Header.ClickOnNextButton();
            selectQuestionPage.Header.ClickOnNextButton();
            selectRecipientsPage.Header.ClickOnNextButton();

            Log.Info("Verify all the details of stepper 1 and stepper 4");
            Assert.AreEqual(assessmentName, reviewAndPublishPage.GetAssessmentName(), "Assessment type does not match");
            Assert.AreEqual(assessmentType, reviewAndPublishPage.GetAssessmentType(), "Assessment Type doesn't match");
            Assert.AreEqual(actualAssessmentDetails.StartDate.ToString("MM/dd/yyyy hh:mm tt"), reviewAndPublishPage.GetAssessmentStartDate(), "Assessment StartDateTime doesn't match");
            Assert.AreEqual(actualAssessmentDetails.Period, reviewAndPublishPage.GetAssessmentPeriod(), "Assessment Period doesn't match");
            Assert.AreEqual(actualAssessmentDetails.RepeatInterval.Type, reviewAndPublishPage.GetAssessmentFrequency(), "Assessment RepeatInterval doesn't match");

            Log.Info("Click on edit icon of 'Questions selected' section, Navigate to 'Select Questions' page and verify page title");
            reviewAndPublishPage.ClickOnQuestionsSelectedPencilIcon();
            Assert.IsTrue(selectQuestionPage.IsSelectQuestionsTitleDisplayed(), "User is not navigate to the stepper 2 - Select Questions page");

            pulseData.NumberOfSelectedQuestions = PulseV2Factory.GetTh25SurveyResponse().Dimensions
                .Where(s => s.Name.Equals(pulseData.Questions[0].DimensionName)).SelectMany(d => d.SubDimensions).SelectMany(z => z.Competencies).SelectMany(c => c.Questions).ToList().Count;

            Log.Info("Go to 'Review and Publish' page and verify selected dimension, subDimensions and competency");
            selectQuestionPage.Header.ClickOnNextButton();
            selectRecipientsPage.Header.ClickOnNextButton();

            var actualTotalQuestionsCount = reviewAndPublishPage.GetTheCountOfSelectedQuestions().ToInt();

            var surveyResponse = PulseV2Factory.GetTh25SurveyResponse();
            var actualQuestionsCount = reviewAndPublishPage.GetTheSelectedQuestions();

            var subDimensionList = surveyResponse.Dimensions.Where(s => s.Name.Equals(dimensionName)).SelectMany(d => d.SubDimensions.Select(x => x.Name)).ToList();

            foreach (var subDimension in subDimensionList)
            {
                var competencies = surveyResponse.Dimensions
                .Where(s => s.Name.Equals(dimensionName)).SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Select(c => c.Name))).ToList();

                expectedQuestionCountByCompetency.AddRange(from competency in competencies
                                                           let expectedCompetencyQuestionsCount = surveyResponse.Dimensions.Where(s => s.Name.Equals(dimensionName))
                                                               .SelectMany(s => s.SubDimensions.Where(x => x.Name.Equals(subDimension)).SelectMany(z => z.Competencies.Where(c => c.Name.Equals(competency)).SelectMany(c => c.Questions.Select(rq => rq.Text.FormatSurveyQuestions()))))
                                                               .ToList()
                                                               .Count()
                                                           select dimensionName + " /" + subDimension + competency + " " + "(" + expectedCompetencyQuestionsCount + ")");
            }
            Assert.That.ListsAreEqual(expectedQuestionCountByCompetency, actualQuestionsCount, "Questions are not matched");
            Assert.AreEqual(pulseData.NumberOfSelectedQuestions, actualTotalQuestionsCount, "Total Questions count is not matched");

            Log.Info("Click on edit icon of 'Team/TeamMembers' section, Navigate to the 'Select Recipients' page and verify page title");
            reviewAndPublishPage.ClickOnTeamPencilIcon();
            Assert.IsTrue(selectRecipientsPage.IsSelectRecipientsTitleDisplayed(), "User is not navigate to the stepper 3 - Select Recipients page");
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(allTeams.First()), "Team is not selected");

            var expectedTeamMemberList = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team.Name);

            Log.Info("Go to 'Review and publish' page and verify team members");
            selectRecipientsPage.Header.ClickOnNextButton();
            reviewAndPublishPage.ClickOnTeam();
            var actualTeamMembersList = reviewAndPublishPage.GetListOfTeamMembersEmail(_team.Name);
            Assert.That.ListsAreEqual(expectedTeamMemberList, actualTeamMembersList, "Team Members list does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_ReviewAndPublish_SaveAsDraft()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();

            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();
            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            selectRecipientsPage.Header.ClickOnNextButton();

            Log.Info("Click on 'Save As Draft' button and verify pulse is created as draft on team assessment dashboard");
            reviewAndPublishPage.ClickOnSaveAsDraftButton();
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseData.Name), "Draft Assessment is not exist");

            Log.Info("Go to edit of draft assessment and verify publish button on 'Edit Pulse Check' page");
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);
            Assert.IsTrue(editPulseCheckPage.Header.IsPublishButtonDisplayed(), "Publish button is not displayed on  page");

            editPulseCheckPage.Header.ClickOnEditRecipientsTab();
            editPulseCheckPage.Header.ClickOnPublishButton();
            editPulseCheckPage.Header.ClickOnPublishPopupSaveAsDraftButton();

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);
            Assert.IsTrue(editPulseCheckPage.Header.IsPublishButtonDisplayed(), "Publish button is not displayed on  page");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_ReviewAndPublish_Close()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();

            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to team dashboard and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();
            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            selectRecipientsPage.Header.ClickOnNextButton();

            Log.Info("Click on 'Cancel' button of close popup and verify that publish button is displayed");
            reviewAndPublishPage.Header.ClickOnCloseIcon();
            reviewAndPublishPage.Header.ClickOnCancelButtonOfExitPulseAssessmentPopup();
            Assert.IsTrue(reviewAndPublishPage.IsPublishButtonDisplayed(), "Publish button is not displayed on review page");

            Log.Info("Click on 'Exit' button of close popup and verify that pulse assessment is not created");
            reviewAndPublishPage.Header.ClickOnCloseIcon();
            reviewAndPublishPage.Header.ClickOnExitButtonOfExitPulseAssessmentPopup();
            Assert.IsFalse(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseData.Name), "Draft Assessment is not exist");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_ReviewAndPublish_VerifyGrayedOutMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse Assessment");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();

            Log.Info("Select Specific competency which is excluded by role");
            pulseData.Questions.First().QuestionSelectionPref = QuestionSelectionPreferences.Competency;
            selectQuestionPage.FillForm(pulseData);

            Log.Info("Verify the team and teamMembers under 'Select Recipients' page");
            selectQuestionPage.Header.ClickOnNextButton();
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team.Name), "Team is not selected");

            var listOfTeamMembers = selectRecipientsPage.GetTeamMembersEmailByTeamNames(_team.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamMemberEnabled(_team.Name, listOfTeamMembers.Last()), "TeamMember is grayed out");
            Assert.IsFalse(selectRecipientsPage.IsTeamMemberEnabled(_team.Name, listOfTeamMembers.First()), "TeamMember is not grayed out");

            Log.Info("Go to review and publish page and verify team members");
            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            selectRecipientsPage.Header.ClickOnNextButton();
            reviewAndPublishPage.ClickOnTeam();
            var actualTeamMembersList = reviewAndPublishPage.GetListOfTeamMembersEmail(_team.Name);
            Assert.That.ListContains(_teamMemberResponses.First().SelectedParticipants.Select(a => a.Email).ToList(), actualTeamMembersList.First(), "Team member does not match");
            Assert.IsFalse(reviewAndPublishPage.IsMemberDisplayed(listOfTeamMembers.Last()), "Team Member is displayed");

            var expectedTeamCount = reviewAndPublishPage.GetListOfTeams().Count.ToString();
            var actualTeamCount = reviewAndPublishPage.GetCountOfTeams();
            Assert.AreEqual(expectedTeamCount, actualTeamCount, "Team Count does not match");

            var actualTeamMemberCount = reviewAndPublishPage.GetCountOfTeamMembers();
            Assert.AreEqual(reviewAndPublishPage.GetListOfTeamMembersEmail(_team.Name).Count.ToString(), actualTeamMemberCount, "Team Member count does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_ReviewAndPublish_SteppersClickable()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to 'Team Assessment dashboard' and create 'Pulse Assessment'");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");


            Assert.IsFalse(createPulseCheckPage.Header.IsSelectQuestionsStepperEnabled(), "'Select question' stepper is enabled");

            Assert.IsFalse(createPulseCheckPage.Header.IsSelectRecipientsStepperEnabled(), "'Select recipients' stepper is enabled");

            Assert.IsFalse(createPulseCheckPage.Header.IsSelectReviewAndPublishStepperEnabled(), "'Review and Publish' stepper is enabled");

            Log.Info("Filling the valid data in stepper 1(Create Pulse Check page)");
            var pulseData = PulseV2Factory.GetPulseAddData();
            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);

            createPulseCheckPage.Header.ClickOnSelectQuestionsStepper();
            Assert.IsTrue(selectQuestionPage.IsSelectQuestionsTitleDisplayed(), "'Select question' is not displayed");

            Assert.IsFalse(createPulseCheckPage.Header.IsSelectRecipientsStepperEnabled(), "'Select recipients' stepper is enabled");

            Assert.IsFalse(createPulseCheckPage.Header.IsSelectReviewAndPublishStepperEnabled(), "'Review and Publish' stepper is enabled");

            //Back to stepper 1
            selectQuestionPage.Header.ClickOnCreatePulseCheckStepper();
            Assert.IsTrue(createPulseCheckPage.IsCreatePulseCheckTitleDisplayed(), "'Select question' is not displayed");

            createPulseCheckPage.Header.ClickOnSelectQuestionsStepper();

            Log.Info("Filling the valid data in stepper 2(Select Questions page)");
            selectQuestionPage.FillForm(pulseData);

            selectQuestionPage.Header.ClickOnSelectRecipientsStepper();
            Assert.IsTrue(selectRecipientsPage.IsSelectRecipientsTitleDisplayed(), "'Select Recipients' is not displayed");

            selectRecipientsPage.ClickOnSelectTeamCheckbox(_team.Name, false);
            Assert.IsFalse(selectRecipientsPage.IsTeamSelected(_team.Name), "Team is selected");

            Assert.IsFalse(createPulseCheckPage.Header.IsSelectReviewAndPublishStepperEnabled(), "'Review and Publish' stepper is enabled");

            selectQuestionPage.Header.ClickOnCreatePulseCheckStepper();
            Assert.IsTrue(createPulseCheckPage.IsCreatePulseCheckTitleDisplayed(), "'Select question' is not displayed");

            selectRecipientsPage.Header.ClickOnSelectQuestionsStepper();
            Assert.IsTrue(selectQuestionPage.IsSelectQuestionsTitleDisplayed(), "'Select question' is not displayed");

            selectQuestionPage.Header.ClickOnSelectRecipientsStepper();
            Assert.IsTrue(selectRecipientsPage.IsSelectRecipientsTitleDisplayed(), "'Select Recipients' is not displayed");

            Log.Info("Get all the teams from 'Select Recipients page'");
            selectRecipientsPage.ClickOnSelectTeamCheckbox(_team.Name);
            Assert.IsTrue(selectRecipientsPage.IsTeamSelected(_team.Name), "Team is not selected");

            selectRecipientsPage.Header.ClickOnReviewAndPublishStepper();
            Assert.IsTrue(reviewAndPublishPage.IsPublishButtonDisplayed(),
                $"Publish button is not displayed on {reviewAndPublishPage} page");

            reviewAndPublishPage.Header.ClickOnSelectRecipientsStepper();
            Assert.IsTrue(selectRecipientsPage.IsSelectRecipientsTitleDisplayed(), "'Select Recipients' is not displayed");

            selectRecipientsPage.Header.ClickOnSelectQuestionsStepper();
            Assert.IsTrue(selectQuestionPage.IsSelectQuestionsTitleDisplayed(), "'Select question' is not displayed");

            selectQuestionPage.Header.ClickOnCreatePulseCheckStepper();
            Assert.IsTrue(createPulseCheckPage.IsCreatePulseCheckTitleDisplayed(), "'Select question' is not displayed");
        }
    }
}