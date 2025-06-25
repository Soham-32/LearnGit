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
    public class AddIndividualAssessmentDownloadTemplateTests : IndividualAssessmentBaseTest
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
        public void IndividualAssessment_DownloadExcelTemplate()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
             
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);
            
            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"CreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);
            createIndividualAssessment1.ClickNextButton();

            var fileName = "TalentDevImportTemplate.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            createIndividualAssessment2.ClickOnDownloadExcelTemplate();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");
        }
    }
}