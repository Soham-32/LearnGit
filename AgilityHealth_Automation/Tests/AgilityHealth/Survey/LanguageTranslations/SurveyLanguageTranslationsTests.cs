using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Survey.LanguageTranslations
{
    [TestClass]
    [TestCategory("Survey")]
    public class SurveyLanguageTranslationsTests : BaseTest
    {
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamHealthRadarName,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName() }
        };

        private static TeamHierarchyResponse _team;
        public static string Language = ManageRadarFactory.SelectTranslatedLanguage();

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment)
                .GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            var setup = new SetUpMethods(_, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"),TestCategory("SiteAdmin"),TestCategory("TeamAdmin"),TestCategory("Member")] 
        public void VerifySurveyPage_LanguageTranslations()
        {

                var login = new LoginPage(Driver, Log);
                var previewAssessmentPage = new PreviewAssessmentPage(Driver, Log);

                //Deserialize 
                var translations = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\SurveyTranslations\SurveyTranslation.json").DeserializeJsonObject<SurveyTranslations>();
                var languageData = translations.Languages.FirstOrDefault(lang => lang.Language == Language);
                if (languageData == null)
                {
                    throw new ArgumentNullException($"Language data for {Language} not found.");
                }

                Log.Info($"Login as {User.FullName} into the application");
                login.NavigateToPage();
                login.LoginToApplication(User.Username, User.Password);

                Log.Info("Navigate to the Survey page");
                var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name,TeamAssessment.AssessmentName), SharedConstants.TeamMember1.Email, "unread");
                previewAssessmentPage.NavigateToUrl(surveyLink);
                previewAssessmentPage.ConfirmIdentity();

                Log.Info("Confirm the identity and verify Survey parameters as per selected language");
                previewAssessmentPage.SelectLanguageDropdown(Language);
                if (!Language.Equals("English")) previewAssessmentPage.ConfirmIdentity();
                previewAssessmentPage.SubmitRandomSurvey();
                var expectedWelcomeText = languageData.Welcome;
                var expectedFinishText = languageData.Finish;
                var expectedStartButtonText = languageData.StartButton;
                var expectedDimensionName = languageData.DimensionName;
                var expectedSubDimensionName = languageData.SubDimensionName;
                var expectedSubDimensionDescription = languageData.SubDimensionDescription;
                var expectedCompetency = languageData.CompetencyName;
                var expectedCompetencyTooltipMessage = languageData.CompetencyTooltipMessage;
                var expectedQuestion = languageData.Question;
                var expectedNaText = languageData.NA;
                var expectedProgressAnswerText = languageData.ProgressAnswer;
                var expectedFinishButtonText = languageData.FinishButton;

                //Welcome text, Dimension , Finish and Survey button
                var dimensionList = previewAssessmentPage.GetLeftNavDimensionsList();
                var actualWelcomeText = previewAssessmentPage.GetWelcomePageStepperText();
                var actualDimensionName = dimensionList.FirstOrDefault();
                var actualFinishText = dimensionList.LastOrDefault();
                var actualStartButtonText = previewAssessmentPage.GetWelcomePageSurveyStartButtonText();
                Assert.AreEqual(expectedWelcomeText,actualWelcomeText,"Welcome text does not match");
                Assert.AreEqual(expectedFinishText,actualFinishText, "Finish text does not match");
                Assert.AreEqual(expectedStartButtonText,actualStartButtonText, "Start button text does not match");
                Assert.AreEqual(expectedDimensionName,actualDimensionName, "Dimension Name text does not match");

                // Sub Dimension , Competency and Question
                var dimensionParameters = previewAssessmentPage.GetAssessmentDetailsForDimension(actualDimensionName);
                var actualSubDimensionName = dimensionParameters["SubDimensions"].FirstOrDefault();
                var actualCompetency = dimensionParameters["Competencies"].FirstOrDefault();
                var firstQuestion = dimensionParameters["Questions"].FirstOrDefault();
                var actualQuestion = firstQuestion?.Replace("\r\n\r\n", "");
                Assert.AreEqual(expectedSubDimensionName, actualSubDimensionName, "SubDimension Name text does not match");
                Assert.AreEqual(expectedCompetency, actualCompetency, "Competency Name text does not match");
                Assert.AreEqual(expectedQuestion, actualQuestion, "Question text does not match");

                //SubDimension Description ,Finish button, Tooltip and Progress Text
                previewAssessmentPage.ConfirmIdentity();
                var actualFinishButtonText = previewAssessmentPage.GetFinishButtonText();
                previewAssessmentPage.ClickOnDimension(actualDimensionName);
                var subDimensionDescriptionList = previewAssessmentPage.GetCompetencyDescriptionList(actualSubDimensionName);
                var actualSubDimensionDescription = (subDimensionDescriptionList[0] + subDimensionDescriptionList[2]).Replace("\r\n\r\n\r\n","").Replace("\r\n\r\n","");
                var actualCompetencyTooltipMessage = previewAssessmentPage.GetCompetencyTooltipMessage(actualCompetency);
                var actualNaText = previewAssessmentPage.GetNA_Text(actualCompetency);
                var actualProgressAnswerText = previewAssessmentPage.GetProgressAnswerText(actualCompetency);
                Assert.AreEqual(expectedFinishButtonText, actualFinishButtonText, "Finish Button text does not match");
                Assert.AreEqual(expectedSubDimensionDescription, actualSubDimensionDescription, "Sub Dimension description text does not match");
                Assert.AreEqual(expectedCompetencyTooltipMessage, actualCompetencyTooltipMessage, "Competency Tooltip text does not match");
                Assert.AreEqual(expectedNaText, actualNaText, "'N/A' text does not match");
                Assert.AreEqual(expectedProgressAnswerText, actualProgressAnswerText, "Progress Answer text does not match");
        }
    }
}
