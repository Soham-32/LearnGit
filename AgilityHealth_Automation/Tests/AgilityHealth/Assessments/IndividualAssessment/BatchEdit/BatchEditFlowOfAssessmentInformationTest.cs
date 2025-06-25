using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.BatchEdit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class BatchEditFlowOfAssessmentInformationTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentResponse;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "BatchEdit");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditIAViewer_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessment = setup.CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BatchEdit_AssessmentTab_VerifyIAInfo()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            
            //assert
            Log.Info("Assert: Verify assessment info shown is the same as the information used to create the individual assessment");
            var assessmentName = batchEditAssessmentPage.GetAssessmentName();
            var pointOfContact = batchEditAssessmentPage.GetPointOfContact();
            var pointOfContactEmail = batchEditAssessmentPage.GetPointOfContactEmail();
            var assessmentStart = batchEditAssessmentPage.GetAssessmentStartDateTime();
            var assessmentEnd = batchEditAssessmentPage.GetAssessmentEndDateTime();

            Assert.AreEqual(_assessmentResponse.AssessmentName, assessmentName, "Assessment names do not match");
            Assert.AreEqual(_assessmentResponse.PointOfContact, pointOfContact, "The point of contact names do not match"); 
            Assert.AreEqual(_assessmentResponse.PointOfContactEmail, pointOfContactEmail, "The point of contact emails do not match");
            Assert.AreEqual(_assessmentResponse.Start.ToString("MM/dd/yyyy hh:mm tt"), assessmentStart, "The assessment start date and time do not match");
            Assert.AreEqual(_assessmentResponse.End.ToString("MM/dd/yyyy hh:mm tt"), assessmentEnd, "The assessment start date and time do not match");
        }
    }
}