using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
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
    public class AddIndividualAssessmentDeleteReviewerTest : IndividualAssessmentBaseTest
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
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_DeleteReviewer()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);
            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.ClickAddReviewer(_team.Members[0].Email);
            addReviewerModal.WaitUntilLoaded();

            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);
            createIndividualAssessment2.DeleteReviewer(_reviewer.Email);
            createIndividualAssessment2.AcceptDeleting();

            Assert.AreEqual(_team.Members[0].FullName() + " (0)", createIndividualAssessment2.GetParticipantNameByEmail(_team.Members[0].Email), "Participant name should be displayed properly with count of reviewer");

            Log.Info("Assert: Verify that deleted reviewer doesn't display in reviewers list");
            Assert.IsFalse(createIndividualAssessment2.DoesReviewerDisplay(_reviewer.Email), 
                $"Deleted reviewer {_reviewer.Email} still displays in reviewers list");
        }
    }
}