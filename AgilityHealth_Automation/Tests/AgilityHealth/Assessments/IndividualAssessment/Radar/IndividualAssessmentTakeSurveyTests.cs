using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Radar
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("IARadars"), TestCategory("Assessments")]
    public class IndividualAssessmentTakeSurveyTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateIndividualAssessmentRequest _assessmentRequest, _assessmentRequest1, _assessmentRequest2;
        private static readonly List<DimensionNote> NotesForParticipant = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Influencing Change", Note = $"Influencing Change participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Coaching Impact", Note = $"Coaching Impact participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Coaching Style", Note = $"Coaching Style participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Growth", Note = $"Growth participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Skills", Note = $"Skills participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments participant {RandomDataUtil.GetUserName()}" }
            };
        private static readonly List<DimensionNote> NotesForReviewer1 = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Influencing Change", Note = $"Influencing Change Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Coaching Impact", Note = $"Coaching Impact Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Coaching Style", Note = $"Coaching Style Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Growth", Note = $"Growth Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Skills", Note = $"Skills Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements Reviewer {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments Reviewer {RandomDataUtil.GetUserName()}" }
            };
        private static readonly List<DimensionNote> NotesForReviewer2 = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Influencing Change", Note = $"Influencing Change participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Value Delivered", SubDimension = "Coaching Impact", Note = $"Coaching Impact participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Coaching Style", Note = $"Coaching Style participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Leadership & Style", SubDimension = "Growth", Note = $"Growth participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Foundation", SubDimension = "Skills", Note = $"Skills participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements participant {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments participant {RandomDataUtil.GetUserName()}" }
        };

        public static readonly List<string> Dimension = new List<string> { "Value Delivered", "Leadership & Style", "Foundation" };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                //Create Individual assessment with one participant
                var admin = User.IsMember() ? TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;
                _team = GetTeamForIndividualAssessment(setup, "TakeSurveyTests",1,admin);
                _assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(Company.Id, User.CompanyName, _team.Uid, "TakeParticipantSurveyTests_");
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType,admin).GetAwaiter().GetResult();

                //Create Individual assessment with one participant and one reviewer
                _assessmentRequest1 = IndividualAssessmentFactory.GetPublishedIndividualAssessment(Company.Id, User.CompanyName, _team.Uid, "TakeReviewerSurveyTests_");
                _assessmentRequest1.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                var reviewer = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessmentRequest1.Members.First().Reviewers.Add(reviewer.ToAddIndividualMemberRequest());
                setup.CreateIndividualAssessment(_assessmentRequest1, SharedConstants.IndividualAssessmentType, admin).GetAwaiter().GetResult();

                //Create Individual assessment with one participant and two reviewers
                _assessmentRequest2 = IndividualAssessmentFactory.GetPublishedIndividualAssessment(Company.Id, User.CompanyName, _team.Uid, "TakeMultiReviewerSurveyTests_");
                _assessmentRequest2.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                var reviewer1 = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessmentRequest2.Members.First().Reviewers.Add(reviewer1.ToAddIndividualMemberRequest());
                var reviewer2 = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessmentRequest2.Members.First().Reviewers.Add(reviewer2.ToAddIndividualMemberRequest());
                setup.CreateIndividualAssessment(_assessmentRequest2, SharedConstants.IndividualAssessmentType, admin).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("KnownDefect")] // Bug Id : 43871
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Single_Participant_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Participant' {_assessmentRequest.Members.First().Email}");
            var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.IaEmailParticipantSubject, _assessmentRequest.Members.First().Email, "unread", _assessmentRequest.PointOfContact);
            surveyPage.NavigateToUrl(surveyLink);

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimension, NotesForParticipant);

            surveyPage.EnterOpenEndedNotes(NotesForParticipant, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name).ToInt();
            iaDashboardPage.NavigateToPage(teamId);
            iaDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);

            var participant = iaDashboardPage.GetAssessmentParticipant(
                    $"{_assessmentRequest.AssessmentName} - {_assessmentRequest.Members.First().FullName()}");

            Log.Info("Verify the number of participants and reviewers should display correctly ");
            Assert.AreEqual("Completed by 1 out of 1 Participant", participant);

            iaDashboardPage.ClickOnRadar($"{_assessmentRequest.AssessmentName} - {_assessmentRequest.Members.First().FullName()}");

            Log.Info("Verify notes for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open end question for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Single_ParticipantAndReviewer_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Participant' {_assessmentRequest1.Members.First().Email}");
            var surveyLinkForParticipant = GmailUtil.GetSurveyLink(SharedConstants.IaEmailParticipantSubject,
                 _assessmentRequest1.Members.First().Email, "unread", _assessmentRequest1.PointOfContact);
            surveyPage.NavigateToUrl(surveyLinkForParticipant);

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimension, NotesForParticipant);

            surveyPage.EnterOpenEndedNotes(NotesForParticipant, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Reviewer' {_assessmentRequest1.Members.First().Reviewers.First().Email}");
            var surveyLinkForReviewer = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject,
                _assessmentRequest1.Members.First().Reviewers.First().Email, "unread", _assessmentRequest1.PointOfContact);
            surveyPage.NavigateToUrl(surveyLinkForReviewer);

            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimension, NotesForReviewer1);

            surveyPage.EnterOpenEndedNotes(NotesForReviewer1, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name).ToInt();
            iaDashboardPage.NavigateToPage(teamId);
            iaDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);
            iaDashboardPage.WaitUntilLoaded();

            var reviewer = iaDashboardPage.GetAssessmentReviewer(
                $"{_assessmentRequest1.AssessmentName} - {_assessmentRequest1.Members.First().FullName()}");
            Log.Info("Verify the number of reviewers should display correctly ");
            Assert.AreEqual("Completed by 1 out of 1 Reviewer", reviewer);

            iaDashboardPage.ClickOnRadar($"{_assessmentRequest1.AssessmentName} - {_assessmentRequest1.Members.First().FullName()}");

            Log.Info("Verify notes for Reviewer display correctly");
            NotesForReviewer1.Where(n => n.Dimension != "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open end question for Reviewer display correctly");
            NotesForReviewer1.Where(n => n.Dimension == "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify notes for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open end question for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public void IndividualAssessment_MultipleReviewersAndParticipant_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var iaDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Particiapant' {_assessmentRequest2.Members.First().Email}");
            var surveyLinkForParticipant = GmailUtil.GetSurveyLink(SharedConstants.IaEmailParticipantSubject,
                 _assessmentRequest2.Members.First().Email, "unread", _assessmentRequest2.PointOfContact);
            surveyPage.NavigateToUrl(surveyLinkForParticipant);

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();

            surveyPage.SubmitSurveyNotes(Dimension, NotesForParticipant);

            surveyPage.EnterOpenEndedNotes(NotesForParticipant, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Reviewer1' {_assessmentRequest2.Members.Last().Reviewers.First().Email}");
            var surveyLinkForReviewer1 = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject,
                _assessmentRequest2.Members.Last().Reviewers.First().Email, "unread", _assessmentRequest2.PointOfContact);
            surveyPage.NavigateToUrl(surveyLinkForReviewer1);

            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimension, NotesForReviewer1);

            surveyPage.EnterOpenEndedNotes(NotesForReviewer1, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Individual assessment and fill the survey with notes for 'Reviewer2' {_assessmentRequest2.Members.First().Reviewers.Last().Email}");
            var surveyLinkForReviewer2 = GmailUtil.GetSurveyLink(SharedConstants.IaEmailReviewerSubject,
               _assessmentRequest2.Members.First().Reviewers.Last().Email, "unread", _assessmentRequest2.PointOfContact);
            surveyPage.NavigateToUrl(surveyLinkForReviewer2);

            surveyPage.SelectReviewerRole(new List<string> { "Reviewer" });
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimension, NotesForReviewer2);

            surveyPage.EnterOpenEndedNotes(NotesForReviewer2, AssessmentType.Individual);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name).ToInt();
            iaDashboardPage.NavigateToPage(teamId);
            iaDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);
            iaDashboardPage.WaitUntilLoaded();

            iaDashboardPage.ClickOnRadar($"{_assessmentRequest2.AssessmentName} - {_assessmentRequest2.Members.First().FullName()}");

            Log.Info("Verify notes for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension != "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify notes for Reviewer1 display correctly");
            NotesForReviewer1.Where(n => n.Dimension != "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify notes for Reviewer2 display correctly");
            NotesForReviewer2.Where(n => n.Dimension != "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetDimensionNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open ended question for Participant display correctly");
            NotesForParticipant.Where(n => n.Dimension == "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open ended question for Reviewer1 display correctly");
            NotesForReviewer1.Where(n => n.Dimension == "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));

            Log.Info("Verify open ended question for Reviewer2 display correctly");
            NotesForReviewer2.Where(n => n.Dimension == "Open").ToList().ForEach(n => Assert.That.ListContains(assessmentDetailPage.GetOpenEndedNotes(n.SubDimension), n.Note,
                    $"Note not present for sub-dimension {n.SubDimension}"));
        }
    }
}