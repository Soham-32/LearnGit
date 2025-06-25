using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.Generic;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GrowthItemNotesTabAddEditDeleteTests : GrowthItemNotesBaseTests
    {
        private static TeamHierarchyResponse _team;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static TeamAssessmentInfo _teamAssessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var setup = new SetUpMethods(testContext, TestEnvironment);
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
            .GetTeamByName(SharedConstants.Team);

            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(), 
                TeamMembers = new List<string> { Constants.TeamMemberName1 },
                StakeHolders = new List<string> { Constants.StakeholderName1 }
            };

            setup.AddTeamAssessmentAndGi(_team.TeamId, _teamAssessment, new List<GrowthItem> { GrowthItemInfo });
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public void GrowthItem_AddEditDeleteNotes()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Create/Update growth item note via 'Save' button of popup and verify");
            GrowthItemAddEditNotesViaSaveButtonOfPopup(GrowthItemInfo.Title);

            Driver.RefreshPage();
           
            Log.Info("Create/Update/Delete growth item note via 'Save' button of note section and verify");
            GrowthItemAddEditDeleteNotes(GrowthItemInfo.Title);
        }
    }
}