using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Survey
{
    [TestClass]
    [TestCategory("Survey")]
    public class SurveyEmailTeamAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _teamLevel;
        private static string _surveyLinkForFirstMember, _surveyLinkForSecondMember;
        private static readonly string FirstMemberEmail = SharedConstants.TeamMember1.Email;
        private static readonly string SecondMemberEmail = SharedConstants.TeamMember3.Email;
        private static readonly string RandomValidEmail = $"RandomMail+{CSharpHelpers.RandomNumber()}@yopmail.com";
        private static readonly string RandomInvalidEmail = $"InvalidMail+{CSharpHelpers.RandomNumber()}";
        private const string ExpectedValidationMessageInvalidEmailText = "Error : Please enter a valid email.";
        //private const string ExpectedValidationMessageBlankEmailText = "Error : Please enter your email address.";
        private static User FacilitatorUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");
        private static SetUpMethods _setup;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            Facilitator = FacilitatorUser.FirstName + " " + FacilitatorUser.LastName,
            FacilitationDate = DateTime.Today.AddDays(1),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember3.FullName() }

        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                // Create team assessment 
                _teamLevel = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _setup = new SetUpMethods(_, TestEnvironment);
                _setup.AddTeamAssessment(_teamLevel.TeamId, TeamAssessment);

                // Get survey links for members
                _surveyLinkForFirstMember = GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_teamLevel.Name, TeamAssessment.AssessmentName), FirstMemberEmail); 
                _surveyLinkForSecondMember = GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_teamLevel.Name, TeamAssessment.AssessmentName), SecondMemberEmail);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_TeamAssessment_ExistingMember()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            Log.Info($"Take email survey for {SecondMemberEmail} and enter another member's email which exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForSecondMember);
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(FirstMemberEmail);
            var expectedEmailSentPopupMessage = Constants.EmailFoundPopupMessage(FirstMemberEmail);
            Assert.AreEqual(expectedEmailSentPopupMessage, emailPage.GetEmailFoundPopupText(), "'Email Found' popup message is not match");
            emailPage.ClickCloseButtonOnEmailFoundPopup();

            Log.Info($"Go to received email for {FirstMemberEmail} , take survey and verify that the Survey screen is displayed");
            var newSurveyLinkForFirstMember = GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_teamLevel.Name, TeamAssessment.AssessmentName), FirstMemberEmail);
            surveyPage.NavigateToUrl(newSurveyLinkForFirstMember);
            var getSurveyIdentity = surveyPage.GetSurveyIdentity();
            Assert.AreEqual($"Hello, {SharedConstants.TeamMember1.FullName()}", getSurveyIdentity, "Survey screen should display for the correct user.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_TeamAssessment_RandomValidMember()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            var expectedFacilitatorEmailMessage = $"{RandomValidEmail} is attempting to access an assessment on {_teamLevel.Name} team for their {TeamAssessment.AssessmentName} assessment";

            const string expectedSubject = "Assessment Requested";
            var facilitatorEmail = FacilitatorUser.Username;

            Log.Info($"Take email survey for {FirstMemberEmail} and enter random valid email which is not exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForFirstMember);
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(RandomValidEmail);
            var expectedEmailSentPopupMessageForNotFound = Constants.EmailNotFoundPopupMessage(RandomValidEmail);
            Assert.AreEqual(expectedEmailSentPopupMessageForNotFound, emailPage.GetEmailNotFoundPopupText(), "'Email Not Found' popup message is not match");
            emailPage.ClickCloseButtonOnEmailSentPopupNotFound();

            // Check Facilitator email
            Log.Info($"Verify a {facilitatorEmail} Facilitator email for random valid member");
            var facilitatorEmailBody = GmailUtil.GetAccountManagerEmailBody(expectedSubject, facilitatorEmail, "", "UNREAD");
            Assert.IsTrue(facilitatorEmailBody != null, "Member haven't received any Facilitator email");
            Assert.IsTrue((facilitatorEmailBody.Contains(expectedFacilitatorEmailMessage)), $"Actual - {facilitatorEmailBody} is not contains expected - {expectedFacilitatorEmailMessage}");

          
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_TeamAssessment_ValidationMessageForEmail()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            Log.Info($"Take email survey for {SecondMemberEmail} and enter random invalid email which is not exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForSecondMember);
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(RandomInvalidEmail);
            Assert.AreEqual(ExpectedValidationMessageInvalidEmailText, emailPage.GetValidationMessageForInvalidEmailText(), "Validation message is not matched");

        // Commenting below two lines due to Bug 39236: UI > That's Not Me Page > Issues with validation messages via mouse click
        //    emailPage.EnterEmail("");
        //    Assert.AreEqual(ExpectedValidationMessageBlankEmailText, emailPage.GetValidationMessageForBlankEmailText(), "Validation message is not matched");
        }
    }
}
