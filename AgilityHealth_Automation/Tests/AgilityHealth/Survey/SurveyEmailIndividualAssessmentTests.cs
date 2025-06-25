using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Survey
{
    [TestClass]
    [TestCategory("Survey")]
    public class SurveyEmailIndividualAssessmentTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static ReviewerResponse _firstReviewerResponse, _secondReviewerResponse;
        private static readonly string RandomValidEmail = $"RandomMail+{RandomDataUtil.GetUserName()}@yopmail.com";
        private static readonly string RandomInvalidEmail = $"InvalidMail+{RandomDataUtil.GetUserName()}";
        private const string ExpectedValidationMessageInvalidEmailText = "Error : Please enter a valid email.";
        //private const string ExpectedValidationMessageBlankEmailText = "Error : Please enter your email address.";
        private static string _surveyLinkForFirstReviewer, _surveyLinkForSecondReviewer;
        private static string _firstReviewerEmail, _secondReviewerEmail;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);

                // Create individual assessment
                var firstReviewer = new CreateReviewerRequest
                {
                    FirstName = $"NewReviewer{Guid.NewGuid()}",
                    LastName = "Name",
                    Email = Constants.UserEmailPrefix + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                };
                var secondReviewer = new CreateReviewerRequest
                {
                    FirstName = $"NewReviewer{Guid.NewGuid()}",
                    LastName = "Name",
                    Email = Constants.UserEmailPrefix + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain,
                };
                
                _firstReviewerResponse = setupApi.CreateReviewer(firstReviewer).GetAwaiter().GetResult();
                _secondReviewerResponse = setupApi.CreateReviewer(secondReviewer).GetAwaiter().GetResult();
                

                _team = GetTeamForIndividualAssessment(setupApi, "ThatsNotMe");
                var individualDataResponse = GetIndividualAssessment(setupApi, _team, "ThatsNotMe_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(_firstReviewerResponse.ToAddIndividualMemberRequest());
                _assessmentRequest.Members.First().Reviewers.Add(_secondReviewerResponse.ToAddIndividualMemberRequest());
                setupApi.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

                // Get survey links for reviewers
                _firstReviewerEmail = _firstReviewerResponse.Email;
                _secondReviewerEmail = _secondReviewerResponse.Email;
                _surveyLinkForFirstReviewer = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject, _firstReviewerEmail, "unread", _assessmentRequest.PointOfContact);
                _surveyLinkForSecondReviewer = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject, _secondReviewerEmail, "unread", _assessmentRequest.PointOfContact);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_IndividualAssessment_ExistingReviewer()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            Log.Info($"Take email survey for {_secondReviewerEmail} and enter other reviewer's email which exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForSecondReviewer);
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(_firstReviewerEmail);
            var expectedEmailSentPopupMessage = Constants.EmailFoundPopupMessage(_firstReviewerEmail);
            Assert.AreEqual(expectedEmailSentPopupMessage, emailPage.GetEmailFoundPopupText(), "'Email Found' popup message is not match");
            emailPage.ClickCloseButtonOnEmailFoundPopup();

            Log.Info($"Go to received email for {_firstReviewerEmail} , take survey and verify that the Survey screen is displayed");
            var surveyLinkForReviewerResponse = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject, _firstReviewerEmail, "inbox");
            surveyPage.NavigateToUrl(surveyLinkForReviewerResponse);
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            var getSurveyIdentity = surveyPage.GetSurveyIdentity();
            Assert.AreEqual($"Hello, {_firstReviewerResponse.FullName()}", getSurveyIdentity, "Survey screen should display for the correct user.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_IndividualAssessment_RandomValidEmail()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            Log.Info($"Take email survey for {_firstReviewerEmail} and enter random email which is not exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForFirstReviewer);
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(RandomValidEmail);
            var expectedEmailSentPopupMessageForMotFound = Constants.EmailNotFoundPopupMessage(RandomValidEmail);
            Assert.AreEqual(expectedEmailSentPopupMessageForMotFound, emailPage.GetEmailNotFoundPopupText(), "'Email Not Found' popup message is not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Survey_ThatsNotMe_For_IndividualAssessment_ValidationMessageForEmail()
        {
            VerifySetup(_classInitFailed);

            var surveyPage = new SurveyPage(Driver, Log);
            var emailPage = new PersonalAssessmentLinkPage(Driver, Log);

            Log.Info($"Take email survey for {_secondReviewerEmail} and enter invalid email which is not exists in assessment then verify popup message");
            surveyPage.NavigateToUrl(_surveyLinkForSecondReviewer);
            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ClickThatsNotMe();
            emailPage.EnterEmail(RandomInvalidEmail);
            Assert.AreEqual(ExpectedValidationMessageInvalidEmailText, emailPage.GetValidationMessageForInvalidEmailText(), "Validation message is not matched");

            // Commenting below two lines due to Bug 39236: UI > That's Not Me Page > Issues with validation messages via mouse click
            //emailPage.EnterEmail("");
            //Assert.AreEqual(ExpectedValidationMessageBlankEmailText, emailPage.GetValidationMessageForBlankEmailText(), "Validation message is not matched");

        }
    }
}
