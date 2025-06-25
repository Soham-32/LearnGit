using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.BatchEdit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
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
    public class BatchEditHeaderDeleteButtonTest2 : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateIndividualAssessmentRequest _assessmentFactory;
        private static IndividualAssessmentResponse _assessment;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
               
                _team = GetTeamForBatchEdit(setup, "BatchEditDelete");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEditDeleteIA_");
                _assessmentFactory = individualDataResponse.Item2;
                _assessmentFactory.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = setup.CreateIndividualAssessment(_assessmentFactory, SharedConstants.IndividualAssessmentType)
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
        public void BatchEdit_Header_TrashButton_Delete()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);

            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickDeleteTrashButton();
            batchEditAssessmentPage.ClickPopupDeleteButton();
            iAssessmentDashboardPage.WaitUntilLoaded();

            //assert
            Log.Info("Assert: Verify delete button returns user to assessment dashboard with assessment not present");
            Assert.IsFalse(iAssessmentDashboardPage.IsAssessmentPresent($"{_assessment.AssessmentName} - Roll Up"));
        }

    }
}