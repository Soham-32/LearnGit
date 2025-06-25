using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class Gpv2AddedFromV1Tests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo2 = GrowthPlanFactory.GetValidGrowthItem();
        private static readonly GrowthItem GrowthItemInfo3 = GrowthPlanFactory.GetValidGrowthItem();
        private static TeamAssessmentInfo _teamAssessment;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var setup = new SetUpMethods(testContext, TestEnvironment);
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
            _multiTeam = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            _enterpriseTeam = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam);

            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(), 
                TeamMembers = new List<string> { Constants.TeamMemberName1 },
                StakeHolders = new List<string> { Constants.StakeholderName1 }
            };

            setup.AddTeamAssessmentAndGi(_team.TeamId, _teamAssessment, new List<GrowthItem> { GrowthItemInfo });

            GrowthItemInfo2.Category = "Enterprise";
            GrowthItemInfo2.AffectedTeams = SharedConstants.Team;
            setup.AddGiForMTeam(_multiTeam.TeamId, GrowthItemInfo2);

            GrowthItemInfo3.Category = "";
            GrowthItemInfo3.AffectedTeams = SharedConstants.MultiTeam;
            setup.AddGiForETeam(_enterpriseTeam.TeamId, GrowthItemInfo3);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2Dashboard_VerifyItemAddedFromV1()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _team.TeamId);
            growthPlanDashboard.WaitUntilGrowthPlanLoaded();

            var expectedColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(expectedColumn);

            var titleName = GrowthItemInfo.Title;
            var actualTeamTitleNames = growthPlanDashboard.GetAllColumnValues("Title");
            Assert.That.ListContains(actualTeamTitleNames, titleName, "Growth Item not found");

            Assert.AreEqual(GrowthItemInfo.Priority, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Priority"), "'Priority' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Status, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Status"), "'Status' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Category, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Category"), "'Category' doesn't match");
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Created Date"), "'Created Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Owner, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Owner(s)"), "'Owner' doesn't match");
            Assert.AreEqual(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Target Date"), "'Target Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Size, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Size"), "'Size' doesn't match");
            Assert.AreEqual(GrowthItemInfo.Type, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Type"), "'Type' doesn't match");
            Assert.AreEqual(SharedConstants.Team, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Team"), "'Team' doesn't match'");
            Assert.AreEqual(_teamAssessment.AssessmentType, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Radar Type"), "'Radar type' doesn't match");
            Assert.AreEqual(User.Type.ToString(), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Updated By").RemoveWhitespace(), "'Updated By' doesn't match");
            Assert.AreEqual(_teamAssessment.AssessmentName, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Assessment"), "'Assessment doesn't match'");
            Assert.AreEqual("Team", growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Location"), "Location doesn't match");
            Assert.AreEqual(SharedConstants.Team , growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Origination"), "Origination doesn't match");
            Assert.IsTrue(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Id").ToInt() > 0, "Id is <= 0");
            Assert.That.ListsAreEqual(GrowthItemInfo.CompetencyTargets, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Competency Target").Split(','), "'Competency Target' doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2Dashboard_MTeam_VerifyItemAddedFromV1()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _multiTeam.TeamId);
            growthPlanDashboard.WaitUntilGrowthPlanLoaded();

            var expectedColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(expectedColumn);

            var titleName = GrowthItemInfo2.Title;
            var actualTitleNames = growthPlanDashboard.GetAllColumnValues("Title");
            Assert.That.ListContains(actualTitleNames, titleName, "Growth Item not found");

            Assert.AreEqual(GrowthItemInfo2.Priority, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Priority"), "'Priority' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Status, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Status"), "'Status' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Category, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Category"), "'Category' doesn't match");
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Created Date"), "'Created Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Owner, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Owner(s)"), "'Owner' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.TargetDate?.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Target Date"), "'Target Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Size, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Size"), "'Size' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.Type, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Type"), "'Type' doesn't match");
            Assert.AreEqual(GrowthItemInfo2.AffectedTeams, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Affected Teams"), "'Affected Teams' doesn't match");
            Assert.AreEqual(SharedConstants.MultiTeam, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Team"), "'Team' doesn't match'");
            Assert.AreEqual(SharedConstants.TeamAssessmentType, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Radar Type"), "'Radar type' doesn't match");
            Assert.AreEqual(User.Type.ToString(), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Updated By").RemoveWhitespace(), "'Updated By' doesn't match");
            Assert.AreEqual("MultiTeam", growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Location"), "Location doesn't match");
            Assert.AreEqual(SharedConstants.MultiTeam, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Origination"), "Origination doesn't match");
            Assert.IsTrue(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Id").ToInt() > 0, "Id is <= 0");
            Assert.That.ListsAreEqual(GrowthItemInfo.CompetencyTargets, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Competency Target").Split(','), "'Competency Target' doesn't match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2Dashboard_ETeam_VerifyItemAddedFromV1()
        {
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id, _enterpriseTeam.TeamId);
            growthPlanDashboard.WaitUntilGrowthPlanLoaded();

            var expectedColumn = GrowthPlanDashboardPage.ColumnLocators.Keys.ToList();
            growthPlanDashboard.AddColumns(expectedColumn);

            var titleName = GrowthItemInfo3.Title;
            var actualTitleNames = growthPlanDashboard.GetAllColumnValues("Title");
            Assert.That.ListContains(actualTitleNames, titleName, "Growth Item not found");

            Assert.AreEqual(GrowthItemInfo3.Priority, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Priority"), "'Priority' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.Status, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Status"), "'Status' doesn't match");
            Assert.AreEqual("Enterprise", growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Category"), "'Category' doesn't match");
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Created Date"), "'Created Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.Owner, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Owner(s)"), "'Owner' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.TargetDate?.ToString("MM/dd/yyyy"), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Target Date"), "'Target Date' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.Size, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Size"), "'Size' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.Type, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Type"), "'Type' doesn't match");
            Assert.AreEqual(GrowthItemInfo3.AffectedTeams, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Affected Teams"), "'Affected Teams' doesn't match");
            Assert.AreEqual(SharedConstants.EnterpriseTeam, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Team"), "'Team doesn't match'");
            Assert.AreEqual(SharedConstants.TeamAssessmentType, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Radar Type"), "'Radar type' doesn't match");
            Assert.AreEqual(User.Type.ToString(), growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Updated By").RemoveWhitespace(), "'Updated By' doesn't match");
            Assert.AreEqual("Enterprise", growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Location"), "Location doesn't match");
            Assert.AreEqual(SharedConstants.EnterpriseTeam , growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Origination"), "Origination doesn't match");
            Assert.IsTrue(growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Id").ToInt() > 0, "Id is <= 0");
            Assert.That.ListsAreEqual(GrowthItemInfo.CompetencyTargets, growthPlanDashboard.GetColumnValueByGiTitleName(titleName, "Competency Target").Split(','), "'Competency Target' doesn't match");
        }
    }
}