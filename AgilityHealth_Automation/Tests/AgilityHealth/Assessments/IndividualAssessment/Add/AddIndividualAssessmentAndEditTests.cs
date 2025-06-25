using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentAndEditTests : IndividualAssessmentBaseTest
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
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_EditAssessmentInfoOnCreate()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            individualAssessmentDashboard.NavigateToPage(teamId.ToInt());
            individualAssessmentDashboard.AddAnAssessment("Talent Development");

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            createIndividualAssessment2.WaitUntilLoaded();
            
            createIndividualAssessment2.ClickNextButton();
            createIndividualAssessment3.WaitUntilLoaded();

            //get info from page
            createIndividualAssessment3.ClickNextButton();
            createIndividualAssessment4.WaitUntilLoaded();
            var actualAssessmentInfo = createIndividualAssessment4.GetAssessmentInfo();

            var timeDifferenceStart = assessment.Start.Subtract(actualAssessmentInfo.Start);
            var timeDifferenceEnd = assessment.End.Subtract(actualAssessmentInfo.End);

            
            Log.Info("Assert: Validate info shows correctly on the Review & Finish page");
            Assert.AreEqual(assessment.AssessmentName, actualAssessmentInfo.AssessmentName, 
                "Assessment names do not match");
            Assert.AreEqual(assessment.PointOfContact, actualAssessmentInfo.PointOfContact, 
                "Point of contact names do not match");
            Assert.AreEqual(assessment.PointOfContactEmail, actualAssessmentInfo.PointOfContactEmail, 
                "Point of contact emails do not match");
            Assert.IsTrue(timeDifferenceStart < TimeSpan.Parse("00:02:00") && timeDifferenceStart > TimeSpan.Parse("-00:02:00"), 
                $"Start Date displays incorrectly - Expected:<{assessment.Start}>. Actual:<{actualAssessmentInfo.Start}>.");
            Assert.IsTrue(timeDifferenceEnd < TimeSpan.Parse("00:02:00") && timeDifferenceEnd > TimeSpan.Parse("-00:02:00"), 
                $"Start Date displays incorrectly - Expected:<{assessment.Start}>. Actual:<{actualAssessmentInfo.Start}>.");

            createIndividualAssessment4.ClickAssessmentEditIcon();

            var editedAssessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditedCreateIA_{Guid.NewGuid()}");
            editedAssessment.PointOfContactEmail = assessment.PointOfContactEmail;
            editedAssessment.Start = DateTime.Today.AddDays(1);
            editedAssessment.End = DateTime.Today.AddDays(8);

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(editedAssessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ClickNextButton();
            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();
            createIndividualAssessment4.WaitUntilLoaded();

            Log.Info("Assert: Verify information has been updated and shows on Review & Publish page");
            //get info from page
            var newActualAssessmentInfo = createIndividualAssessment4.GetAssessmentInfo();

            var newTimeDifferenceStart = editedAssessment.Start.Subtract(actualAssessmentInfo.Start);
            var newTimeDifferenceEnd = editedAssessment.End.Subtract(actualAssessmentInfo.End);

            //validate new info
            Assert.AreEqual(editedAssessment.AssessmentName, newActualAssessmentInfo.AssessmentName, 
                "Assessment names do not match");
            Assert.AreEqual(assessment.PointOfContactEmail, editedAssessment.PointOfContactEmail, 
                "Point of contact email is not the same");
            Assert.AreNotEqual(assessment.AssessmentName, editedAssessment.AssessmentName, 
                "Assessment name is not different.");
            Assert.AreNotEqual(timeDifferenceStart, newTimeDifferenceStart, "Start time is not different.");
            Assert.AreNotEqual(timeDifferenceEnd, newTimeDifferenceEnd, "End time is not different.");

            //publish
            createIndividualAssessment4.ClickPublishTopButton();

            Log.Info("Assert: Verify individual assessment created with edited name");
            individualAssessmentDashboard.WaitUntilLoaded();
            Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent(
                    $"{editedAssessment.AssessmentName} - Roll up"),
                $"Individual Roll up doesn't exists with name : {editedAssessment.AssessmentName}");
        }
    }
}