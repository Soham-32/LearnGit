using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Create
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentCreateTests6 : BaseTest
    {

        private static TeamHierarchyResponse _team;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName() },
            StartDate = DateTime.Today.AddDays(3)
    };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
            var setup = new SetUpMethods(_, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
        }
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_FutureStartDate()
        {
            _team.CheckForNull($"<{nameof(_team)}> is null. Aborting test.");

            Log.Info("Test: Verify that assessment status should be shown as Pending when start date is in future");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            Log.Info("Verify that Assessment is saved as Pending on Dashboard ");
            var data = teamAssessmentDashboard.GetAssessmentStatus(TeamAssessment.AssessmentName);
            Assert.AreEqual("Pending", data[0],"Assessment status is incorrect");
            Assert.AreEqual("disc orange", data[1], "Assessment indicator is incorrect");
        }
    }
}