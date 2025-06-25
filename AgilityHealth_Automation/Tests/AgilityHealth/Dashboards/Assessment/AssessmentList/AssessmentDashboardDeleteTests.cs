using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.AssessmentList
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class AssessmentDashboardDeleteTests : BaseTest
    {
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            var setup = new SetUpMethods(testContext, TestEnvironment);
            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                TeamMembers = new List<string> { Constants.TeamMemberName1 }
            };

            setup.AddTeamAssessment(_team.TeamId, _teamAssessment);
            
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void AssessmentDashboard_AssessmentList_DeleteAssessment()
        {
            
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ResetAllFilters();

            assessmentDashboardListTabPage.DeleteAssessment(_teamAssessment.AssessmentName);

            Log.Info("Verify the assessment is no longer displayed");
            Assert.AreEqual("No assessment selected.", assessmentDashboardListTabPage.GetAssessmentFilterMessage(),
                "Message should show after assessment deleted");

        }
    }
}
