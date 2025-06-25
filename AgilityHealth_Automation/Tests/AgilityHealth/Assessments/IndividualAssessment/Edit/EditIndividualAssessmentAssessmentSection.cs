using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentAssessmentSection : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "EditIA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "EditIASection_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_AssessmentSection()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            var assessmentType = iaEditPage.GetAssessmentType();
            var poc = iaEditPage.GetPoc();
            var pocEmail = iaEditPage.GetPocEmail();
            var name = iaEditPage.GetAssessmentName();

            var actualAssessmentInfo = iaEditPage.GetAssessmentInfo();
            var timeDifferenceStart = _assessmentRequest.Start.Subtract(actualAssessmentInfo.Start);
            var timeDifferenceEnd = _assessmentRequest.End.Subtract(actualAssessmentInfo.End);

            Assert.AreEqual(assessmentType, SharedConstants.IndividualAssessmentType, "Assessment types do not match");
            Assert.IsTrue(timeDifferenceStart < TimeSpan.Parse("00:02:00") && timeDifferenceStart > TimeSpan.Parse("-00:02:00"), 
                $"Start Date displays incorrectly - Expected:<{_assessmentRequest.Start}>. Actual:<{actualAssessmentInfo.Start}>.");
            Assert.IsTrue(timeDifferenceEnd < TimeSpan.Parse("00:02:00") && timeDifferenceEnd > TimeSpan.Parse("-00:02:00"), 
                $"Start Date displays incorrectly - Expected:<{_assessmentRequest.Start}>. Actual:<{actualAssessmentInfo.Start}>.");
            Assert.AreEqual(poc, _assessmentRequest.PointOfContact, "Point of contact names do not match");
            Assert.AreEqual(pocEmail, _assessmentRequest.PointOfContactEmail, "Point of contact emails do not match");
            Assert.AreEqual(name, _assessment.AssessmentName, "Assessment names do not match");
        }
    }
}