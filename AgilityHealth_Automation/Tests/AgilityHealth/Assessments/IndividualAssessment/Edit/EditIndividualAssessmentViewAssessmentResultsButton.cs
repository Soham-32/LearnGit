using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentViewAssessmentResultsButton : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static IndividualAssessmentResponse _assessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "EditIA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "TakeIA_");
                var assessment = individualDataResponse.Item2;
                assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                _assessment = setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
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
        public void IndividualAssessment_Edit_ViewAssessmentResults()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull("assessmentUid is null.").AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);

            iaEditPage.WaitForAssessmentTitle($"{_team.Members.First().FullName()}'s Talent Development Assessment");
            iaEditPage.ClickViewAssessmentResults();

            Log.Info("Assert: Verify that 'View assessment results' button takes user to radar");
            Driver.SwitchToLastWindow();
            Assert.IsTrue(iaEditPage.GetAssessmentTitle().Contains(SharedConstants.IndividualAssessmentType),$"User should be taken to radar. Actual {SharedConstants.IndividualAssessmentType}");
        }
    }
}
