using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Delete
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentDeleteTests : BaseTest
    {

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName() }
        };

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _team = new SetupTeardownApi(TestEnvironment)
                .GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
            TeamAssessment.StartDate = DateTime.UtcNow.ToLocalTime();
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Edit_DeleteAssessment()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");

            taEditPage.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();

            Log.Info("Verify assessment is deleted properly");
            Assert.IsFalse(teamAssessmentDashboard.DoesAssessmentExist(TeamAssessment.AssessmentName), 
                $"Assessment <{TeamAssessment.AssessmentName}> still shows on the Assessment Dashboard");
        }
    }
}