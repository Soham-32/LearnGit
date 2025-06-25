using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class IndividualAssessmentVerifyButtonsOnPageOne : IndividualAssessmentBaseTest
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
        public void IndividualAssessment_VerifyEnabledButtonsOnPage1()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);
            
            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"PublishIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            Log.Info("Assert: Verify buttons are not enabled when assessment fields are empty");
            createIndividualAssessment1.WaitUntilLoaded();
            var deleteButtonDisabled = createIndividualAssessment1.IsDeleteButtonEnabled();
            Assert.IsFalse(deleteButtonDisabled, "Delete button is enabled");
            var saveAsDraftButtonDisabled = createIndividualAssessment1.IsSaveAsDraftButtonEnabled();
            Assert.IsFalse(saveAsDraftButtonDisabled, "Save as draft button is enabled");
            var nextButtonDisabled = createIndividualAssessment1.IsNextButtonEnabled();
            Assert.IsFalse(nextButtonDisabled, "Next button is enabled");

            //fill out assessment
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            Log.Info("Assert: Verify buttons are enabled when assessment fields are filled");
            var deleteButtonEnabled = createIndividualAssessment1.IsDeleteButtonEnabled();
            Assert.IsFalse(deleteButtonEnabled, "Delete button is enabled");
            var saveAsDraftButtonEnabled = createIndividualAssessment1.IsSaveAsDraftButtonEnabled();
            Assert.IsFalse(saveAsDraftButtonEnabled, "Save as draft button is enabled");
            var nextButtonEnabled = createIndividualAssessment1.IsNextButtonEnabled();
            Assert.IsTrue(nextButtonEnabled, "Next button is disabled");
        }
    }
}