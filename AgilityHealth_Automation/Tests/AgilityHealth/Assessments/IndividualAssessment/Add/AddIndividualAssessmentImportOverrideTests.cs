﻿using System;
using System.Data;
using System.IO;
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
    public class AddIndividualAssessmentImportOverrideTests : IndividualAssessmentBaseTest
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
        public void IndividualAssessment_OverrideImport()
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
            
            createIndividualAssessment2.WaitUntilLoaded();
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TD_Import.xlsx");
            var tbl = ExcelUtil.GetExcelData(filePath);
            createIndividualAssessment2.ImportExcelFile(filePath);
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.WaitUntilLoaded();
            foreach (DataRow row in tbl.Rows)
            {
                var participantEmail = row["ParticipantsEmail"].ToString();
                if (participantEmail != "Participants Email Address" && participantEmail != "Required")
                {
                    Assert.IsTrue(createIndividualAssessment2.DoesParticipantDisplay(participantEmail), $"Participant {participantEmail} is not imported properly");
                    var reviewName = row["ReviewerFirstName"] + " " + row["ReviewerLastName"];
                    var reviewEmail = row["ReviewerEmail"].ToString();
                    var reviewerRoles = row["ReviewerRole"].ToString();
                    Assert.IsTrue(createIndividualAssessment2.DoesReviewerDisplayReviewerScreen(reviewName, reviewEmail, reviewerRoles), $"Reviewer with email {reviewEmail} is not showing correctly");
                }
            }

            var overrideFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TD_Import_Override.xlsx");
            var overrideTbl = ExcelUtil.GetExcelData(overrideFilePath);
            createIndividualAssessment2.ImportExcelFile(overrideFilePath);
            createIndividualAssessment2.WaitUntilLoaded();
            foreach (DataRow row in overrideTbl.Rows)
            {
                var participantEmail = row["ParticipantsEmail"].ToString();
                if (participantEmail != "Participants Email Address" && participantEmail != "Required")
                {
                    Assert.IsTrue(createIndividualAssessment2.DoesParticipantDisplay(participantEmail), $"Participant {participantEmail} is not imported properly");
                    var reviewName = row["ReviewerFirstName"] + " " + row["ReviewerLastName"];
                    var reviewEmail = row["ReviewerEmail"].ToString();
                    var reviewerRoles = row["ReviewerRole"].ToString();
                    Assert.IsTrue(createIndividualAssessment2.DoesReviewerDisplayReviewerScreen(reviewName, reviewEmail, reviewerRoles), $"Reviewer with email {reviewEmail} is not showing correctly");
                }
            }
        }
    }
}