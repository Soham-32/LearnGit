using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentAndEditUsingBackButtonTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA", 2);

                _reviewer = MemberFactory.GetReviewer();
                setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_EditAssessmentInfoUsingBackButton()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton(); //to 'Add Reviewers'

            createIndividualAssessment2.WaitUntilLoaded();
            var participantCountInitial = createIndividualAssessment2.GetCountOfParticipants();
            createIndividualAssessment2.ClickNextButton(); //to 'Invite Viewers'
            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickBackButton(); //back to 'Add Reviewers'

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.DeleteParticipant(assessment.Members[1].Email);
            createIndividualAssessment2.AcceptDeleting();
            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ClickAddReviewer(assessment.Members[0].Email);
            addReviewerModal.WaitUntilLoaded();
            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);

            createIndividualAssessment2.ClickNextButton(); //go to 'Invite Viewers' page
            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton(); //go to 'Review and Publish' page

            Log.Info("Assert: Verify information has been updated and shows on Review & Publish page");
            //get info from page
            createIndividualAssessment4.WaitUntilLoaded();
            var actualAssessmentInfo = createIndividualAssessment4.GetAssessmentInfo();
            var participantCountFinal = createIndividualAssessment2.GetCountOfParticipants();
            var doesReviewerExist = createIndividualAssessment4.HasReviewer(_reviewer.Email);

            //validate new info
            Assert.AreEqual(assessment.AssessmentName, actualAssessmentInfo.AssessmentName,
                "Assessment names are not the same.");
            Assert.IsTrue(participantCountInitial > participantCountFinal, "A participant was not removed.");
            Assert.IsTrue(doesReviewerExist, "Added reviewer does not show on 'Review and Publish' page");
            Assert.AreEqual(assessment.PointOfContact, actualAssessmentInfo.PointOfContact,
                "Point of contact names do not match");
            Assert.AreEqual(assessment.PointOfContactEmail, actualAssessmentInfo.PointOfContactEmail,
                "Point of contact emails do not match");

            //publish
            createIndividualAssessment4.ClickPublishTopButton();

            Log.Info("Assert: Verify individual assessment created with edited name");
            individualAssessmentDashboard.WaitUntilLoaded();
            Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent(
                    $"{assessment.AssessmentName} - Roll up"),
                $"Individual Roll up doesn't exists with name : {assessment.AssessmentName}");
        }
    }
}