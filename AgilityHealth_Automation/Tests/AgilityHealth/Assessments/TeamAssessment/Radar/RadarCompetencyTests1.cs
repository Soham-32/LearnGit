using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class RadarCompetencyTests1 : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 },
            StakeHolders = new List<string> { Constants.StakeholderName1, Constants.StakeholderName2 }
        };

        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void RadarCompetency_SummaryView_AddNewGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            growthItemGridView.DeleteAllGIs();

            radarPage.ClickCompetency("4422");

            radarPage.ClickGrowthPlanAddGiButton();

            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Type = "Agile Enablement",
                Title = $"growthitem_{RandomDataUtil.GetGrowthPlanTitle()}",
                Status = "Not Started",
                TargetDate = DateTime.Now,
                Priority = "Low",
                Size = "2",
                Description = RandomDataUtil.GetGrowthPlanDescription()
            };
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo, false);
            addGrowthItemPopup.ClickSaveButton();

            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");
            if (actualGrowthItem.TargetDate != null)
                Assert.AreEqual((object)growthItemInfo.TargetDate.Value.Date, actualGrowthItem.TargetDate.Value.Date,
                    "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual("Team Confidence", actualGrowthItem.CompetencyTargets[0], "Competency doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void RadarCompetency_DetailView_AddNewGrowthItem()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);
            assessmentDetailPage.RadarSwitchView(ViewType.Detail);
            growthItemGridView.DeleteAllGIs();

            radarPage.ClickCompetency("4422");

            radarPage.ClickGrowthPlanAddGiButton();

            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Type = "Agile Enablement",
                Title = "growthitem" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                TargetDate = DateTime.Now,
                Priority = "Low",
                Size = "2",
                Description = RandomDataUtil.GetGrowthPlanDescription()
            };
            addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo, false);
            addGrowthItemPopup.ClickSaveButton();

            var actualGrowthItem = growthItemGridView.GetGrowthItemFromGrid(1);
            Assert.AreEqual(growthItemInfo.Category, actualGrowthItem.Category, "Category doesn't match");
            Assert.AreEqual(growthItemInfo.Type, actualGrowthItem.Type, "Type doesn't match");
            Assert.AreEqual(growthItemInfo.Title, actualGrowthItem.Title, "Title doesn't match");
            Assert.AreEqual(growthItemInfo.Status, actualGrowthItem.Status, "Status doesn't match");
            Assert.AreEqual(growthItemInfo.Priority, actualGrowthItem.Priority, "Priority doesn't match");

            // ReSharper disable once PossibleInvalidOperationException
            Assert.AreEqual((object)growthItemInfo.TargetDate.Value.Date, actualGrowthItem.TargetDate.Value.Date, "TargetDate doesn't match");
            Assert.AreEqual(growthItemInfo.Size, actualGrowthItem.Size, "Size doesn't match");
            Assert.AreEqual(growthItemInfo.Description, actualGrowthItem.Description, "Description doesn't match");
            Assert.AreEqual("Team Confidence", actualGrowthItem.CompetencyTargets[0], "Competency doesn't match");
        }
    }
}
