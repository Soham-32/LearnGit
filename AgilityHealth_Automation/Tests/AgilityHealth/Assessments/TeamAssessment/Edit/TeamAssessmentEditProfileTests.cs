using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Edit
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentEditProfileTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly User Facilitator = TestEnvironment.UserConfig.GetUserByDescription("facilitator");
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            LeadershipReadOutDate = DateTime.Today.AddDays(1),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName() }
        };

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment)
                        .GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
                TeamAssessment.StartDate = DateTime.UtcNow.ToLocalTime();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Edit_Profile()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            Log.Info($"Login as '{User.Username}' then navigate to assessment 'Edit' page and edit assessment details");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            taEditPage.ClickEditDetailButton();

            var editedAssessment = new TeamAssessmentInfo
            {
                AssessmentName = TeamAssessment.AssessmentName + "updated",
                Facilitator = Facilitator.FullName,
                FacilitationDate = DateTime.Today.AddDays(1).AddHours(8),
                FacilitationDuration = 1,
                LeadershipReadOutDate = DateTime.Today.AddDays(2),
                Location = "Whatevs",
                EndDate = DateTime.Today.AddDays(1).AddHours(12)
            };
            if (User.IsCompanyAdmin())
            {
                editedAssessment.StartDate = DateTime.Today.AddDays(-1).AddHours(7);
            }
            taEditPage.FillDataForAssessmentProfile(editedAssessment);
            taEditPage.EditPopup_ClickUpdateButton();
            Driver.RefreshPage();

            Log.Info("Verify updated assessment name ,facilitator, Leadership Readuout date display correctly");
            var actualProfile = taEditPage.GetAssessmentProfile();
            Assert.AreEqual(editedAssessment.AssessmentName, actualProfile.AssessmentName, 
                "Assessment Name doesn't match");
            TeamAssessment.AssessmentName = editedAssessment.AssessmentName;
            Assert.AreEqual(editedAssessment.Facilitator.Replace(" (ATI Facilitator)", ""),
                actualProfile.Facilitator, "Facilitator name doesn't match");
            Assert.AreEqual(editedAssessment.FacilitationDate, actualProfile.FacilitationDate, 
                "Facilitation Date doesn't match");
            Assert.AreEqual(editedAssessment.FacilitationDuration, actualProfile.FacilitationDuration, 
                "Facilitation Duration doesn't match");
            Assert.AreEqual(editedAssessment.Location, actualProfile.Location, 
                "Location doesn't match");
            Assert.AreEqual(editedAssessment.EndDate, actualProfile.EndDate, "End Date doesn't match");
            if (User.IsCompanyAdmin())
            {
                Assert.AreEqual(editedAssessment.StartDate, actualProfile.StartDate, 
                    "Start Date doesn't match");
            }
            else
            {
                Assert.That.TimeIsClose(TeamAssessment.StartDate, actualProfile.StartDate);
            }
            Assert.AreEqual(editedAssessment.LeadershipReadOutDate,actualProfile.LeadershipReadOutDate,"Leadership Readout Date doesn't match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Edit_Notes()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            const string assessmentNote = "Test Note";
            const string noteDescription = "Test Description";
            taEditPage.EnterDataForAssessmentNotesSection(assessmentNote, "Run", noteDescription);

            Log.Info("Verify Assessment Note is saved properly");
            Assert.AreEqual(assessmentNote, taEditPage.GetAssessmentNotes(), "Assessment notes do not match");
            var notes = taEditPage.GetNoteDescription();
            Assert.IsTrue(notes.Contains(noteDescription), $"Expected: <{noteDescription}>, Actual: <{notes}>");
        }
    }
}