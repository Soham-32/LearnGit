using System;
using System.Data;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class BatchEditImportOverrideTests : IndividualAssessmentBaseTest
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
                var individualDataResponse = GetIndividualAssessment(setup, _team, "BatchEdit_");
                _assessmentResponse = individualDataResponse.Item2;
                _assessmentResponse.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                _assessment = setup
                    .CreateIndividualAssessment(_assessmentResponse, SharedConstants.IndividualAssessmentType)
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
        public void BatchEdit_OverrideImport()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var batchEditAssessmentPage = new BatchEditAssessmentPage(Driver, Log);
            var batchEditParticipantReviewerPage = new BatchEditParticipantReviewerPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            batchEditAssessmentPage.NavigateToPage(Company.Id, _team.Uid, _assessment.BatchId, teamId);
            batchEditAssessmentPage.WaitForAssessmentDataLoaded();
            batchEditAssessmentPage.ClickParticipantsReviewersTab();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TD_Import.xlsx");
            var tbl = ExcelUtil.GetExcelData(filePath);
            batchEditParticipantReviewerPage.ImportExcelFile(filePath);
            batchEditParticipantReviewerPage.ExpandCollapseParticipantsAndReviewers();
            batchEditParticipantReviewerPage.WaitUntilLoaded();
            foreach (DataRow row in tbl.Rows)
            {
                var participantEmail = row["ParticipantsEmail"].ToString();
                if (participantEmail != "Participants Email Address" && participantEmail != "Required")
                {
                    Assert.IsTrue(batchEditParticipantReviewerPage.DoesParticipantDisplay(participantEmail), $"Participant {participantEmail} is not imported properly");
                    var reviewName = row["ReviewerFirstName"] + " " + row["ReviewerLastName"];
                    var reviewEmail = row["ReviewerEmail"].ToString();
                    var reviewerRoles = row["ReviewerRole"].ToString();
                    Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplayReviewerScreen(reviewName, reviewEmail, reviewerRoles), $"Reviewer with email {reviewEmail} is not showing correctly");
                }
            }

            var overrideFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TD_Import_Override.xlsx");
            var overrideTbl = ExcelUtil.GetExcelData(overrideFilePath);
            batchEditParticipantReviewerPage.ImportExcelFile(overrideFilePath);
            batchEditParticipantReviewerPage.WaitUntilLoaded();
            foreach (DataRow row in overrideTbl.Rows)
            {
                var participantEmail = row["ParticipantsEmail"].ToString();
                if (participantEmail != "Participants Email Address" && participantEmail != "Required")
                {
                    Assert.IsTrue(batchEditParticipantReviewerPage.DoesParticipantDisplay(participantEmail), $"Participant {participantEmail} is not imported properly");
                    var reviewName = row["ReviewerFirstName"] + " " + row["ReviewerLastName"];
                    var reviewEmail = row["ReviewerEmail"].ToString();
                    var reviewerRoles = row["ReviewerRole"].ToString();
                    Assert.IsTrue(batchEditParticipantReviewerPage.DoesReviewerDisplayReviewerScreen(reviewName, reviewEmail, reviewerRoles), $"Reviewer with email {reviewEmail} is not showing correctly");
                }
            }
        }
    }
}