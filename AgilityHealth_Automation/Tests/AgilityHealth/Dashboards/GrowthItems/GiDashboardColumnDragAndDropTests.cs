using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.GrowthItems;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.GrowthItems
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GiDashboardColumnDragAndDropTests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo2 = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo3 = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo4 = GrowthPlanFactory.GetValidGrowthItem();
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
            GrowthItemInfo2.Priority = GrowthPlanFactory.GetGrowthPlanPriority()[2];
            GrowthItemInfo3.Priority = GrowthPlanFactory.GetGrowthPlanPriority()[3];
            GrowthItemInfo4.Priority = GrowthPlanFactory.GetGrowthPlanPriority()[4];
            setup.AddTeamAssessmentAndGi(_team.TeamId, _teamAssessment, new List<GrowthItem> { GrowthItemInfo, GrowthItemInfo2, GrowthItemInfo3, GrowthItemInfo4 });
        }

        [TestMethod]
        [TestCategory("GrowthItemDashboard")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void GrowthItemDashboard_DragDrop_Column()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var giDashboardGridView = new GiDashboardGridWidgetPage(Driver, Log);
            const string columnName = "Priority";

            Log.Info($"Login as {User.FullName} and go to growth item dashboard");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.ClickGrowthItemDashBoard();

            Log.Info($"Select the {columnName} column from the column filter");
            giDashboardGridView.ClearFilter();
            giDashboardGridView.ClickColumnMenu();
            giDashboardGridView.ClickColumnsMenuItem();
            giDashboardGridView.AddColumns(new List<string> { columnName });
            giDashboardGridView.ClickColumnMenu();

            Log.Info($"Drag and drop {columnName} column and verify data should be filtered");
            giDashboardGridView.DragAndDropColumn(columnName, 0, -10);
            var groupRowHeaderValueList = giDashboardGridView.GetGroupRowHeaderValues(columnName);
            var allRowValueList = giDashboardGridView.GetAllRawValues();

            var growthItemRowList = new List<List<string>>();
            var previousIndex = 0;
            for (var i = 1; i <= groupRowHeaderValueList.Count ; i++)
            {
                var currentIndex = groupRowHeaderValueList.Count != i ? allRowValueList.FindIndex(a => a.Contains(groupRowHeaderValueList[i])) : allRowValueList.Count;
                var length = currentIndex - previousIndex;
                growthItemRowList.Add(allRowValueList.GetRange(previousIndex, length));
                previousIndex = currentIndex;
            }

            for (var i = 0; i < groupRowHeaderValueList.Count; i++)
            {
                foreach (var growthItemRow in growthItemRowList[i])
                {
                    Assert.IsTrue(growthItemRow.Contains(groupRowHeaderValueList[i]), $"{growthItemRow} growth item does not contains {groupRowHeaderValueList[i]}");
                }
            }
        }
    }
}

