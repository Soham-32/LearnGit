using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Radar
{
    internal enum Users
    {
        Participant,
        Viewer
    }

    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class IndividualAssessmentParticipantAndViewerAccessTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _createTeamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static User _viewer;
        private const string Password = SharedConstants.CommonPassword;
        private const int TeamMemberAnswer = 5;
        private static string _firstParticipant;
        private static string _secondParticipant;
        private static readonly GrowthItem CreateGi = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem UpdatedGi = GrowthPlanFactory.GetValidUpdatedGrowthItem();

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                _viewer = TestEnvironment.UserConfig.GetUserByDescription("member");

                var viewer = new MemberResponse()
                {
                    FirstName = _viewer.FirstName,
                    LastName = _viewer.LastName,
                    Email = _viewer.Username
                };

                _team = TeamFactory.GetGoiTeam("ia", 2);
                _firstParticipant = _team.Members.First().FullName();
                _secondParticipant = _team.Members.Last().FullName();
                _createTeamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();

                var individualDataResponse = GetIndividualAssessment(setup, _createTeamResponse, "IA_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _createTeamResponse.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.IndividualViewers.Add(viewer.ToAddUserRequest());

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

                //Fill the survey for team participant
                setupUi.CompleteIndividualSurvey(_assessmentRequest.Members.First().Email, _assessmentRequest.PointOfContactEmail, TeamMemberAnswer);

                //Set the password of newly created participant
                setupUi.SetupAccountPassword(_assessmentRequest.Members.First().Email, Password, GmailUtil.UserEmailLabel);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48956
        [TestCategory("Critical")] 
        [TestCategory("CompanyAdmin")]
        public void IndividualAssessment_Participant_AccessVerification()
        {
            VerifySetup(_classInitFailed);
            Participant_Viewer_AccessVerification(Users.Participant);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48956
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("CompanyAdmin")]
        public void IndividualAssessment_Viewer_AccessVerification()
        {
            VerifySetup(_classInitFailed);
            Participant_Viewer_AccessVerification(Users.Viewer);
        }

        private void Participant_Viewer_AccessVerification(Users user)
        {
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var teamDashboard = new TeamDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            var firstParticipantAssessment = _assessment.AssessmentName + " - " + _assessmentRequest.Members.First().FullName();
            var secondParticipantAssessment = _assessment.AssessmentName + " - " + _assessmentRequest.Members.Last().FullName();

            var userEmail = user switch
            {
                Users.Participant => _assessmentRequest.Members.First().Email,
                Users.Viewer => _viewer.Username,
                _ => null
            };

            login.NavigateToPage();
            login.LoginToApplication(userEmail, SharedConstants.CommonPassword);

            Log.Info($"Verify that {_team.Name} team is displayed on Team Dashboard");
            dashboardPage.GridTeamView();
            Assert.IsTrue(teamDashboard.DoesTeamDisplay(_team.Name), $"{_team.Name} is not display on team Dashboard");

            Log.Info($"Navigate to the individual assessment dashboard and verify {_firstParticipant} accesses");
            var teamId = dashboardPage.GetTeamIdFromLink(_createTeamResponse.Name).ToInt();
            individualAssessmentDashboard.NavigateToPage(teamId);

            if (user == Users.Participant)
            {
                Log.Info($"Verify that {_firstParticipant} should be able to see the Individual assessments of {_firstParticipant} but should not be able to see the Individual assessment for {_secondParticipant}");
                Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent($"{_assessment.AssessmentName} - {_firstParticipant}"), $"Individual radar doesn't exists with name : {_assessment.AssessmentName}");
                Assert.IsFalse(individualAssessmentDashboard.IsAssessmentPresent($"{_assessment.AssessmentName} - {_secondParticipant}"), $"Individual radar does exists with name : {_assessment.AssessmentName}");

                Log.Info($"Verify that {_firstParticipant} should not able to add/edit of the Individual assessments");
                Assert.IsFalse(individualAssessmentDashboard.IsAddAssessmentButtonDisplayed(), "'Add Assessment' button is displayed");
                Assert.IsFalse(individualAssessmentDashboard.IsAssessmentEditIconDisplayed(_assessment.AssessmentName), $"{_firstParticipant} is able to edit the assessment {_assessment.AssessmentName}");
            }
            else
            {
                Log.Info("Navigate to the individual assessment dashboard and verify Both participant's radar");
                Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent($"{_assessment.AssessmentName} - {_firstParticipant}"), $"Individual radar doesn't exists with name : {_assessment.AssessmentName}");
                Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent($"{_assessment.AssessmentName} - {_secondParticipant}"), $"Individual radar doesn't exists with name : {_assessment.AssessmentName}");

                Log.Info("Verify that viewer should not be able to add the Individual assessments");
                Assert.IsFalse(individualAssessmentDashboard.IsAddAssessmentButtonDisplayed(), "'Add Assessment' button is displayed");

                Log.Info("Verify that viewer should be able to edit the Individual assessments");
                Assert.IsTrue(individualAssessmentDashboard.IsAssessmentEditIconDisplayed(firstParticipantAssessment), $"Viewer is able to edit the assessment {_assessment.AssessmentName}");
                Assert.IsTrue(individualAssessmentDashboard.IsAssessmentEditIconDisplayed(secondParticipantAssessment), $"Viewer is able to edit the assessment {_assessment.AssessmentName}");
            }

            Log.Info("Click on the participant radar and verify the URL/title");
            individualAssessmentDashboard.ClickOnRadar($"{_assessment.AssessmentName} - {_firstParticipant}");
            var radarTitle = (firstParticipantAssessment + " - " + SharedConstants.IndividualAssessmentType);
            Assert.AreEqual(radarTitle.ToLower(), radarPage.GetRadarTitle().ToLower(), "RadarTitle doesn't match");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("/radar"), "Radar page URL is not matched");

            Log.Info("On Radar page, verify Export PDF, Excel and Questions buttons");
            Assert.IsTrue(radarPage.IsExportPdfButtonDisplayed(), "Export pdf button is not displayed");
            Assert.IsFalse(radarPage.IsExportExcelButtonDisplayed(), "Export excel button is displayed");
            Assert.IsFalse(radarPage.IsExportQuestionsButtonDisplayed(), "Export Questions button is displayed");

            Log.Info("Click on the pdf button and verify that pdf should be downloaded");
            var fileName = $"{_team.Name} {_assessmentRequest.AssessmentName} - {_firstParticipant} {_assessmentRequest.End:MM-dd-yyyy}.pdf";
            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} is not downloaded");

            Log.Info("Click on the 'Add New Item' and verify that growth item should be added");
            growthItemGridView.ClickAddNewGrowthItem();
            CreateGi.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().First();
            CreateGi.Owner = null;
            CreateGi.CompetencyTargets = new List<string> { SharedConstants.DimensionClarity };
            addGrowthItemPopup.EnterGrowthItemInfo(CreateGi);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(CreateGi.Title), $"Growth item {CreateGi.Title} is not added");

            Log.Info($"'Edit' {CreateGi.Title} GI and verify that growth item should be edited");
            growthItemGridView.ClickGrowthItemEditButton(CreateGi.Title);
            UpdatedGi.Category = GrowthPlanFactory.GetIaParticipantGrowthPlanCategories().Last();
            UpdatedGi.Owner = null;
            UpdatedGi.CompetencyTargets = new List<string> { SharedConstants.DimensionFoundation };
            addGrowthItemPopup.EnterGrowthItemInfo(UpdatedGi);
            addGrowthItemPopup.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(UpdatedGi.Title), $"Growth Item {UpdatedGi.Title} is not present");

            //Copy Bug id: 35461
            //Log.Info($"Copy the GI and verify that growth item should be copied");
            //growthItemGridView.ClickCopyGrowthItemButton(EditGrowthItemInfo.Title);
            //Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(EditGrowthItemInfo.Title), $"Growth item {EditGrowthItemInfo.Title} isn't copied");

            Log.Info("Click on the export button and verify that excel should be downloaded");
            fileName = $"GrowthPlanFor{_team.Name}Assessment{_assessmentRequest.AssessmentName} - {_firstParticipant}.xlsx";
            growthItemGridView.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");

            Log.Info($"Delete {UpdatedGi.Title} and verify that growth item should be deleted");
            growthItemGridView.DeleteGrowthItem(UpdatedGi.Title);
            Assert.IsFalse(growthItemGridView.IsGiPresent(UpdatedGi.Title), $"Growth item {UpdatedGi.Title} is not deleted");

        }
    }
}