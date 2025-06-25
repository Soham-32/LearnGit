using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentAutoCheckWithUpdatedReviewerTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA");
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_AutoCheckVerification_NewReviewer_Updated()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid); 

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"NewReviewerIA_{Guid.NewGuid()}");

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.ClickAddReviewer(_team.Members[0].Email);
            addReviewerModal.WaitUntilLoaded();

            var reviewer = MemberFactory.GetReviewer();
            reviewer.FirstName = $"NewReviewer{Guid.NewGuid()}";
            addReviewerModal.CreateNewReviewer(reviewer);
            var isChecked = addReviewerModal.IsReviewerChecked(reviewer.FirstName, reviewer.LastName);

            Log.Info("Assert: Check if the adding a new reviewer was auto-checked");
            Assert.IsTrue(isChecked, "The checkbox was not auto-checked");

            addReviewerModal.AddReviewersByScrollingInModal(reviewer);
            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            createIndividualAssessment4.WaitUntilLoaded();
            createIndividualAssessment4.ClickPublishBottomButton();

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);
            assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"UpdatedReviewerIA_{Guid.NewGuid()}");

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.ClickAddReviewer(_team.Members[0].Email);
            addReviewerModal.WaitUntilLoaded();

            reviewer.FirstName = $"UpdatedReviewerName{Guid.NewGuid()}";
            addReviewerModal.CreateNewReviewer(reviewer);
            addReviewerModal.MemberExistsModal_UpdateMember();
            isChecked = addReviewerModal.IsReviewerChecked(reviewer.FirstName, reviewer.LastName);

            Log.Info("Assert: Check if the adding an existing reviewer was auto-checked");
            Assert.IsTrue(isChecked, "The checkbox was not auto-checked");
        }
    }
}