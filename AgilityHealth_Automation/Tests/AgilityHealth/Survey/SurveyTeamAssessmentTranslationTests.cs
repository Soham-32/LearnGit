using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Survey.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Radar.RadarPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Survey
{
    [TestClass]
    [TestCategory("Survey"),TestCategory("LanguageTranslation")]
    public class SurveyTeamAssessmentTranslationTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static string _surveyLinkForFirstMember;
        private static readonly string FirstMemberEmail = SharedConstants.TeamMember2.Email;
        private static User FacilitatorUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");
        private static SetUpMethods _setup;
        private readonly List<string> FinishText = new List<string> { "Finish", "終了", "Finalizar" };

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamHealthRadarName,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            Facilitator = FacilitatorUser.FirstName + " " + FacilitatorUser.LastName,
            FacilitationDate = DateTime.Today.AddDays(1),
            TeamMembers = new List<string> { SharedConstants.TeamMember2.FullName() }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                // Set up the env
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.GiCopyFromPreviousAssessmentTeam);
                _setup = new SetUpMethods(_, TestEnvironment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_TeamAssessment_SurveyTranslation_English()
        {
            VerifySetup(_classInitFailed);
            Survey_TeamAssessment_SurveyTranslation(RadarLanguage.English);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_TeamAssessment_SurveyTranslation_Japanese()
        {
            VerifySetup(_classInitFailed);
            Survey_TeamAssessment_SurveyTranslation(RadarLanguage.Japanese);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_TeamAssessment_SurveyTranslation_Spanish()
        {
            VerifySetup(_classInitFailed);
            Survey_TeamAssessment_SurveyTranslation(RadarLanguage.Spanish);
        }

        private void Survey_TeamAssessment_SurveyTranslation(RadarLanguage language)
        {
            var surveyPage = new SurveyPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var editTeamBasePage = new EditTeamBasePage(Driver, Log);
            var editTeamProfilePage = new EditTeamProfilePage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Verify the team preferred language edit successfully");
            dashBoardPage.GridTeamView();

            dashBoardPage.SearchTeam(_team.Name);
            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);

            dashBoardPage.ClickTeamEditButton(_team.Name);

            editTeamBasePage.GoToTeamProfileTab();

            Log.Info("Verify the team info change the preferred language");
            var teamInfo = new TeamInfo
            {
                PreferredLanguage = language.ToString(),
            };

            editTeamProfilePage.EnterTeamInfo(teamInfo);
            editTeamProfilePage.ClickUpdateTeamProfileButton();

            Log.Info("Verify the updated team language are updated successfully");

            editTeamBasePage.GoToDashboard();

            editTeamBasePage.NavigateToPage(teamId);

            editTeamBasePage.GoToTeamProfileTab();

            TeamInfo actualTeamDetail = editTeamProfilePage.GetTeamInfo();
            Assert.AreEqual(teamInfo.PreferredLanguage, actualTeamDetail.PreferredLanguage, "Preferred Language doesn't match");

            Log.Info("Verify the create a team assessment");
            _setup.AddTeamAssessment(_team.TeamId, TeamAssessment);

            var surveyResponse = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\Survey\THAssessmentTranslation_{language.GetDescription()}.json").DeserializeJsonObject<SurveyTranslation>();

            var emailSubjects = surveyResponse.SurveyAssessmentEmailSubject.FirstOrDefault()?.Subject;

            string TeamAssessmentSubject(string teamName, string assessmentName) =>
            $"{teamName} | {assessmentName} - {emailSubjects}";

            _surveyLinkForFirstMember = GmailUtil.GetSurveyLink(TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName), FirstMemberEmail);

            surveyPage.NavigateToUrl(_surveyLinkForFirstMember);

            Log.Info("Verify The Confirm Identity Popup");
            var actualPopupTitle = surveyPage.GetConfirmIdentityPopupTitle();
            var expectedPopupTitle = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.TranslatedConfirmIdentityTitle;
            Assert.AreEqual(expectedPopupTitle, actualPopupTitle, "Confirm Identity title text is not matched");

            var actualPopupHeaderTitle = surveyPage.GetSurveyIdentity();
            var expectedPopupHeaderTitle = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.TranslatedHeaderTitle;
            Assert.AreEqual(expectedPopupHeaderTitle, actualPopupHeaderTitle, "Confirm Identity Header title text is not matched");

            var actualTranslatedHeaderTitleSubtext = surveyPage.GetConfirmIdentityText();
            var expectedTranslatedHeaderTitleSubtext = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.TranslatedHeaderTitleSubtext;
            Assert.AreEqual(expectedTranslatedHeaderTitleSubtext, actualTranslatedHeaderTitleSubtext, "Confirm Identity Subtitle text is not matched");

            var actualContinueButtonText = surveyPage.GetConfirmIdentityContinueButtonText();
            var expectedContinueButtonText = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.TranslatedContinueButtonText;
            Assert.AreEqual(expectedContinueButtonText, actualContinueButtonText, "Confirm Identity Continue button text is not matched");

            var actualThatsNotMeButtonText = surveyPage.GetConfirmIdentityThatsNotMeButtonText();
            var expectedThatsNotMeButtonText = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.TranslatedThatNotMeButtonText;
            Assert.AreEqual(expectedThatsNotMeButtonText, actualThatsNotMeButtonText, "Confirm Identity That's Not Me button text is not matched");

            var actualPopupFooterText = surveyPage.GetConfirmIdentityFooterText();
            var expectedPopupFooterText = surveyResponse.ConfirmIdentityPopup.FirstOrDefault()?.FooterText;
            Assert.AreEqual(expectedPopupFooterText, actualPopupFooterText, "Confirm Identity footer text is not matched");

            surveyPage.ConfirmIdentity();

            surveyPage.ClickOnWelcomePageStepper();

            //Verify the translation for Welcome Page
            Log.Info("Verify The language is spanish or other language");

            string actualWelcomePageMessage;
            if (language.ToString() == RadarLanguage.Spanish.ToString())
            {
                Log.Info("Verify The Welcome Page");
                actualWelcomePageMessage = surveyPage.GetWelcomePageMessageForSpanish();
            }
            else
            {
                Log.Info("Verify The Welcome Page");
                actualWelcomePageMessage = surveyPage.GetWelcomePageMessage();
            }
            var expectedWelcomePageMessage = surveyResponse.WelcomePage.FirstOrDefault()?.WelcomeMessage;
            Assert.AreEqual(expectedWelcomePageMessage, actualWelcomePageMessage, "Welcome Page message is not matched");

            var actualWelcomePageStartButtonText = surveyPage.GetWelcomePageSurveyStartButtonText();
            var expectedWelcomePageStartButtonText = surveyResponse.WelcomePage.FirstOrDefault()?.StartButtonText;
            Assert.AreEqual(expectedWelcomePageStartButtonText, actualWelcomePageStartButtonText, "Start button text is not matched");

            var actualLeftSideWelcomeMenu = surveyPage.GetWelcomePageStepperText();
            var expectedLeftSideWelcomeMenu = surveyResponse.WelcomePage.FirstOrDefault()?.LeftSidePanelText;
            Assert.AreEqual(expectedLeftSideWelcomeMenu, actualLeftSideWelcomeMenu, "Welcome text is not displayed in left side menu");

            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(5);

            //Verify the translation for dimension
            Log.Info("Verify The dimension name");
            var expectedDimensionsList = surveyResponse.Dimensions.Select(d => d.DimensionName).ToList();
            var actualDimensionsList = surveyPage.GetLeftNavDimensionsList();
            Assert.That.ListsAreEqual(expectedDimensionsList.ToList(), actualDimensionsList.ToList(), "Dimensions List is not matched.");

            Log.Info("Verify The all competency questions");
            foreach (var dimension in surveyResponse.Dimensions)
            {
                if (!FinishText.Contains(dimension.DimensionName))
                {
                    //Sub dimension
                    var expectedSubDimensions = dimension.SubDimension;

                    foreach (var subDimension in expectedSubDimensions)
                    {
                        var actualSubDimensionNames = surveyPage.GetAllSubDimensionsHeaderTextList().ToList();
                        Assert.That.ListContains(actualSubDimensionNames, subDimension.HeaderName, "Header name is not matched");
                    }

                    var expectedDimensionSubDimensionText = $"{expectedSubDimensions.FirstOrDefault().HeaderName}";

                    var dimensions = surveyResponse.Dimensions.Where(a => a.DimensionName.Equals(dimension.DimensionName)).ToList();

                    //Question List
                    var questionAllList = (from dimensionList in dimensions
                                        let subDimension = dimensionList.SubDimension.ToList()
                                        from subDim in subDimension
                                        let competency = subDim.Competencies.ToList()
                                        from competenciesList in competency
                                        let questionList = competenciesList.Question.ToList()
                                        from question in questionList
                                        let questions = question.Question
                                        select questions).ToList();

                    //Question Progress details
                    var questionProgressAnswerList = (from dimensionList in dimensions
                                                      let subDimension = dimensionList.SubDimension.ToList()
                                                      from subDim in subDimension
                                                      let competency = subDim.Competencies.ToList()
                                                      from competenciesList in competency
                                                      let questionList = competenciesList.Question.ToList()
                                                      from question in questionList
                                                      let questionProgress = question.QuestionFooterDetail
                                                      select questionProgress).ToList();

                    //Question N\A text
                    var questionNaCheckboxTextList = (from dimensionList in dimensions
                                                      let subDimension = dimensionList.SubDimension.ToList()
                                                      from subDim in subDimension
                                                      let competency = subDim.Competencies.ToList()
                                                      from competenciesList in competency
                                                      let questionList = competenciesList.Question.ToList()
                                                      from question in questionList
                                                      let questionNaText = question.NaText
                                                      select questionNaText).ToList();

                    //Question Notes Title
                    var questionNotesTitleList = (from dimensionList in dimensions
                                                  let subDimension = dimensionList.SubDimension.ToList()
                                                  from subDim in subDimension
                                                  let note = subDim.Notes.ToList()
                                                  from notes in note
                                                  let notesTitle = notes.NotesTitle
                                                  select notesTitle).ToList();

                    //Save button text
                    var expectedSaveButtonText = (from dimensionList in dimensions
                                                  let footerText = dimensionList.FooterButtonText.ToList()
                                                  from footer in footerText
                                                  let saveButtonText = footer.SaveButton
                                                  select saveButtonText).ToList();

                    //Question button text
                    var expectedQuestionLabelArrowText = (from dimensionList in dimensions
                                                          let footerText = dimensionList.FooterButtonText.ToList()
                                                          from footer in footerText
                                                          let questionButtonText = footer.QuestionLabel
                                                          select questionButtonText).ToList();

                    //Section button text
                    var expectedSectionLabelArrowText = (from dimensionList in dimensions
                                                         let footerText = dimensionList.FooterButtonText.ToList()
                                                         from footer in footerText
                                                         let sectionButtonText = footer.SectionLabel
                                                         select sectionButtonText).ToList();
                    //Question saved text
                    var expectedQuestionSavedText = (from dimensionList in dimensions
                                                     let footerText = dimensionList.FooterButtonText.ToList()
                                                     from footer in footerText
                                                     let saveMesText = footer.SaveMessage
                                                     select saveMesText).ToList();

                    //Competency description list
                    var expectedCompetencyDescriptionList = (from dimensionList in dimensions
                                                              let subDimensionList = dimensionList.SubDimension.ToList()
                                                              from subDimension in subDimensionList
                                                              let descriptionOne = subDimension.DescriptionOne
                                                              select descriptionOne).ToList();

                    //Maturity description list
                    var expectedMaturityDescriptionList = (from dimensionList in dimensions
                                                           let subDimensionList = dimensionList.SubDimension.ToList()
                                                           from subDimension in subDimensionList
                                                           let descriptionTwo = subDimension.DescriptionTwo
                                                           select descriptionTwo).ToList();

                    var actualCompetencyQuestionList = surveyPage.GetAllQuestionsBySubDimensionList(expectedDimensionSubDimensionText);
                    var actualCompetencyQuestionNaTextList = surveyPage.GetAllQuestionsByNaTextList(expectedDimensionSubDimensionText);
                    var actualCompetencyQuestionProgressDescriptionList = surveyPage.GetAllQuestionsByProgressDescriptionList(expectedDimensionSubDimensionText);
                    var actualSubDimensionNotesTitle = surveyPage.GetCompetencyNotesList(expectedDimensionSubDimensionText).ToList();
                    var actualCompetencyDescriptionList = surveyPage.GetCompetencyDescriptionList(expectedDimensionSubDimensionText).ToList();
                    var actualMaturityDescriptionList = surveyPage.GetMaturityDescriptionList(expectedDimensionSubDimensionText).ToList();

                    var actualSaveButtonText = surveyPage.GetSaveButtonText();
                    var actualQuestionLabelArrowText = surveyPage.GetQuestionLabelArrowText();
                    var actualSectionLabelArrowText = surveyPage.GetSectionLabelArrowText();


                    //Loop for competency description verify                    
                    expectedCompetencyDescriptionList.ForEach(competencyDescriptionText => Assert.IsTrue(actualCompetencyDescriptionList.Contains(competencyDescriptionText), ""));

                    //Loop for Maturity description verify
                    expectedMaturityDescriptionList.ForEach(maturityDescriptionText => Assert.IsTrue(actualMaturityDescriptionList.Contains(maturityDescriptionText), ""));

                    //Loop for Survey questions verify
                    Parallel.For(0, questionAllList.Count, n =>
                    {
                        var expectedSurveyQuestion = questionAllList[n].FormatSurveyQuestions();
                        var actualSurveyQuestion = actualCompetencyQuestionList[n].FormatSurveyQuestions();
                        Assert.AreEqual(expectedSurveyQuestion, actualSurveyQuestion, "Survey question is not matched");
                    });

                    //Loop for Survey Competency Progress question list verify
                    Parallel.For(0, questionProgressAnswerList.Count, n =>
                    {
                        var expectedQuestionProgressDescription = questionProgressAnswerList[n].FormatSurveyQuestions();
                        var actualQuestionProgressDescription = actualCompetencyQuestionProgressDescriptionList[n].FormatSurveyQuestions();
                        Assert.AreEqual(expectedQuestionProgressDescription, actualQuestionProgressDescription, "Survey Competency Progress question list is not matched");
                    });

                    //Loop for Survey questions N/A text verify
                    Parallel.For(0, questionNaCheckboxTextList.Count, n =>
                    {
                        var expectedQuestionNaText = questionNaCheckboxTextList[n].FormatSurveyQuestions();
                        var actualQuestionNaText = actualCompetencyQuestionNaTextList[n].FormatSurveyQuestions();
                        Assert.AreEqual(expectedQuestionNaText, actualQuestionNaText, "Survey question N/A text is not matched");
                    });

                    //Loop for Survey questions Notes verify
                    Parallel.For(0, questionNotesTitleList.Count, n =>
                    {
                        var expectedQuestionNotes = questionNotesTitleList[n].FormatSurveyQuestions();
                        var actualQuestionNotes = actualSubDimensionNotesTitle[n].FormatSurveyQuestions();
                        Assert.AreEqual(expectedQuestionNotes, actualQuestionNotes, "Notes title is not matched");
                    });


                    //Verify the footer buttons
                    Assert.AreEqual(expectedSaveButtonText.FirstOrDefault(), actualSaveButtonText, "Save button text is not matched");
                    Assert.AreEqual(expectedQuestionLabelArrowText.FirstOrDefault(), actualQuestionLabelArrowText, "Question label arrow text is not matched");
                    Assert.AreEqual(expectedSectionLabelArrowText.FirstOrDefault(), actualSectionLabelArrowText, "Section label arrow text is not matched");

                    surveyPage.ClickNextButton();

                    Log.Info("Verify The survey saved successfully");
                    var actualSurveySavedText = surveyPage.GetSurveySavedText(expectedQuestionSavedText.FirstOrDefault());
                    Assert.AreEqual(expectedQuestionSavedText.FirstOrDefault(), actualSurveySavedText, "Saved message text is not matched");
                }

                else
                {
                    Log.Info("Verify The Finish dimension details");
                    var expectedDimensions = dimension.Finish.FirstOrDefault().OpenEndQuestionList.ToList();
                    var actualOpenEndQuestion = surveyPage.GetFinishOpenEndedQuestionList().ToList();
                    var actualOpenEndQuestionProgress = surveyPage.GetFinishOpenEndedQuestionProgressList().ToList();
                    var actualSaveButtonText = surveyPage.GetSaveButtonText();
                    var actualQuestionLabelArrowText = surveyPage.GetQuestionLabelArrowText();
                    var actualSectionLabelArrowText = surveyPage.GetSectionLabelArrowText();
                    var actualFinishButtonText = surveyPage.GetFinishButtonText();

                    var dimensions = surveyResponse.Dimensions.Where(a => a.DimensionName.Equals(dimension.DimensionName)).ToList();

                    //Save button text
                    var expectedSaveButtonText = (from dimensionList in dimensions
                                                  let subDimension = dimensionList.Finish.ToList()
                                                  from subDim in subDimension
                                                  let footerButton = subDim.FooterButtonText.ToList()
                                                  from saveButton in footerButton
                                                  let saveButtonText = saveButton.SaveButton
                                                  select saveButtonText).ToList();

                    //Question label arrow text
                    var expectedQuestionLabelArrowText = (from dimensionList in dimensions
                                                          let subDimension = dimensionList.Finish.ToList()
                                                          from subDim in subDimension
                                                          let footerButton = subDim.FooterButtonText.ToList()
                                                          from questionButton in footerButton
                                                          let questionLabelText = questionButton.QuestionLabel
                                                          select questionLabelText).ToList();

                    //Section label arrow text
                    var expectedSectionLabelArrowText = (from dimensionList in dimensions
                                                         let subDimension = dimensionList.Finish.ToList()
                                                         from subDim in subDimension
                                                         let footerButton = subDim.FooterButtonText.ToList()
                                                         from sectionText in footerButton
                                                         let sectionLabelText = sectionText.SectionLabel
                                                         select sectionLabelText).ToList();

                    //Finish button text
                    var expectedFinishButtonText = (from dimensionList in dimensions
                                                    let subDimension = dimensionList.Finish.ToList()
                                                    from subDim in subDimension
                                                    let footerButton = subDim.FooterButtonText.ToList()
                                                    from finishBText in footerButton
                                                    let finishButtonText = finishBText.FinishButton
                                                    select finishButtonText).ToList();

                    Log.Info("Verify The openEnded question");
                    foreach (var subDimension in expectedDimensions)
                    {
                        Assert.That.ListContains(actualOpenEndQuestion, subDimension.OpenEndQuestion, "OpenEnded question is not matched");
                        Assert.That.ListContains(actualOpenEndQuestionProgress, subDimension.OpenEndQuestionDetail, "Progress question list is not matched");
                    }

                    //Verify the button
                    Assert.AreEqual(expectedSaveButtonText.FirstOrDefault(), actualSaveButtonText, "Save button text is not matched");
                    Assert.AreEqual(expectedQuestionLabelArrowText.FirstOrDefault(), actualQuestionLabelArrowText, "Question label arrow text is not matched");
                    Assert.AreEqual(expectedSectionLabelArrowText.FirstOrDefault(), actualSectionLabelArrowText, "Section label arrow text is not matched");
                    Assert.AreEqual(expectedFinishButtonText.FirstOrDefault(), actualFinishButtonText, "Finish button text is not matched");

                    surveyPage.ClickSurveyFinishButton();

                    //Finish Confirm Identity text
                    Log.Info("Verify The confirm identity popup text");
                    var expectedConfirmIdentityHeaderText = (from dimensionList in dimensions
                                                             let subDimension = dimensionList.Finish.ToList()
                                                             from subDim in subDimension
                                                             let confirmBox = subDim.ConfirmFinishingDialogBox.ToList()
                                                             from confirmFinish in confirmBox
                                                             let titleText = confirmFinish.Title
                                                             select titleText).ToList();

                    //Finish Confirm Identity description text
                    var expectedConfirmIdentityFinishDescriptionText = (from dimensionList in dimensions
                                                                        let subDimension = dimensionList.Finish.ToList()
                                                                        from subDim in subDimension
                                                                        let confirmBox = subDim.ConfirmFinishingDialogBox.ToList()
                                                                        from confirmFinish in confirmBox
                                                                        let descriptionText = confirmFinish.Description
                                                                        select descriptionText).ToList();

                    //Finish Confirm Identity ok button text
                    var expectedConfirmIdentityFinishOkButtonText = (from dimensionList in dimensions
                                                                     let subDimension = dimensionList.Finish.ToList()
                                                                     from subDim in subDimension
                                                                     let confirmBox = subDim.ConfirmFinishingDialogBox.ToList()
                                                                     from confirmFinish in confirmBox
                                                                     let okButtonText = confirmFinish.OkButton
                                                                     select okButtonText).ToList();

                    //Finish Confirm Identity cancel button text
                    var expectedConfirmIdentityFinishCancelButtonText = (from dimensionList in dimensions
                                                                         let subDimension = dimensionList.Finish.ToList()
                                                                         from subDim in subDimension
                                                                         let confirmBox = subDim.ConfirmFinishingDialogBox.ToList()
                                                                         from confirmFinish in confirmBox
                                                                         let cancelButtonText = confirmFinish.CancelButton
                                                                         select cancelButtonText).ToList();

                    var actualConfirmIdentityFinishHeaderText = surveyPage.GetConfirmIdentityFinishHeaderTitle();
                    var actualConfirmIdentityFinishDescriptionText = surveyPage.GetConfirmIdentityFinishDescription();
                    var actualConfirmIdentityFinishOkButtonText = surveyPage.GetConfirmIdentityFinishOkButtonText();
                    var actualConfirmIdentityFinishCancelButtonText = surveyPage.GetConfirmIdentityFinishCancelButtonText();

                    Assert.AreEqual(expectedConfirmIdentityHeaderText.FirstOrDefault(), actualConfirmIdentityFinishHeaderText, "Confirm Identity header text is not matched");
                    Assert.AreEqual(expectedConfirmIdentityFinishDescriptionText.FirstOrDefault(), actualConfirmIdentityFinishDescriptionText, "Confirm Identity description text is not matched");
                    Assert.AreEqual(expectedConfirmIdentityFinishOkButtonText.FirstOrDefault(), actualConfirmIdentityFinishOkButtonText, "Confirm Identity 'OK' button text is not matched");
                    Assert.AreEqual(expectedConfirmIdentityFinishCancelButtonText.FirstOrDefault(), actualConfirmIdentityFinishCancelButtonText, "Confirm Identity 'Cancel' button text is not matched");

                    surveyPage.ClickOnSurveyConfirmIdentityOkButton();

                    //Survey finish message
                    Log.Info("Verify the survey finish message");
                    var expectedSurveyFinishMessage = (from dimensionList in dimensions
                                                       let subDimension = dimensionList.Finish.ToList()
                                                       from subDim in subDimension
                                                       let lastPageMessage = subDim.LastPageMessage.ToList()
                                                       from finishMessage in lastPageMessage
                                                       let finishMessageText = finishMessage.SubmitMessage
                                                       select finishMessageText).ToList();

                    var actualSurveyFinishMessage = surveyPage.GetSurveyFinishMessage();
                    Assert.AreEqual(expectedSurveyFinishMessage.FirstOrDefault(), actualSurveyFinishMessage, "Confirm Identity description text is not matched");
                }
            }
        }


    }
}
