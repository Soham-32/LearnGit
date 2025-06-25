using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Add
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class GrowthPlanAddItemTests1 : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IList<TeamsWithMembersAndSurveysResponse> _teamWithMemberCountResponse;
        private static IList<GrowthPlanStatusResponse> _growthPlanStatusResponse;
        private static IList<RadarCompetenciesResponse> _targetCompetenciesOfFirstSurvey;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team", 2);
                _teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamResponse.TeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _teamWithMemberCountResponse = setup.GetTeamWithMemberCountResponse(Company.Id, _teamResponse.TeamId).GetAwaiter().GetResult();
                _growthPlanStatusResponse = setup.GetStatusResponse().GetAwaiter().GetResult();
                _targetCompetenciesOfFirstSurvey = setup.GetRadarCompetenciesResponse(Company.Id, _teamWithMemberCountResponse[0].Surveys[0].SurveyId).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Add_VerifyValuesForFields()
        {
            VerifySetup(_classInitFailed);

            Log.Info("Test : Verify the values for fields in growthItem");
            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var addGrowthItems = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            growthPlanDashboard.NavigateToPage(Company.Id, _teamResponse.TeamId);

            Log.Info("Click on 'Add Growth Item' button and verify Tooltip text is matched");
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            var defaultDescriptionPlaceholderText = addGrowthItems.GetDefaultDescriptionPlaceholderText();
            var tooltipDescriptionText = addGrowthItems.GetTooltipDescriptionText();
            Assert.IsTrue(tooltipDescriptionText.StartsWith(defaultDescriptionPlaceholderText), $"Tooltip text not matched. Expected : {tooltipDescriptionText} , Actual : {defaultDescriptionPlaceholderText}");
            Assert.IsTrue(addGrowthItems.IsSupportArticleButtonDisplayed(), "'Support Article' Button is not displayed on tooltip");

            Log.Info("Click on 'Support Article' button and verify user is navigated to Support Center page ");
            addGrowthItems.ClickOnSupportArticleButton();
            Driver.SwitchToLastWindow();
            Assert.AreEqual("https://support.agilityinsights.ai/hc/en-us/articles/360025556634-How-do-I-create-Growth-Items-and-build-a-team-Growth-Plan-", Driver.GetCurrentUrl(), $"User isn't navigated to Support Center page, instead navigated to {Driver.GetCurrentUrl()}");
            Driver.SwitchToFirstWindow();

            Log.Info("Click on 'Owner' dropdown verify that user is able to select multiple owners and verify the owner List");
            addGrowthItems.ClickOnOwnerDropDown();
            var actualOwnerList = new List<string>(addGrowthItems.GetOwnerList());
            var expectedOwners = _teamWithMemberCountResponse.SelectMany(z => z.TeamMembers.Select(c => (c.FirstName + " " + c.LastName))).ToList();
            Assert.That.ListsAreEqual(expectedOwners, actualOwnerList, "Owner List not matched");

            addGrowthItems.DoubleClickOnOwnerDropDown();
            var ownerList = _teamWithMemberCountResponse.SelectMany(q => q.TeamMembers).ToList();

            var recordToMatch = 2;
            for (var i = 0; i < recordToMatch; i++)
            {
                addGrowthItems.ClickOnOwnerDropDown();
                var ownerFullName = ownerList[i].FirstName + " " + ownerList[i].LastName;
                addGrowthItems.SelectOwner(ownerFullName);
                Assert.IsTrue(addGrowthItems.IsOwnerSelected(expectedOwners[i]), $"Owner {ownerFullName} is not selected");
            }

            var actualSelectedOwnersList = addGrowthItems.GetSelectedOwnersList();
            var expectedSelectedOwnersList = expectedOwners.GetRange(0, recordToMatch);
            Assert.That.ListsAreEqual(expectedSelectedOwnersList, actualSelectedOwnersList.ToList(), "Selected Owners are not matched");

            for (var i = 0; i < recordToMatch; i++)
            {
                var ownerFullName = ownerList[i].FirstName + " " + ownerList[i].LastName;
                addGrowthItems.DeselectOwner(ownerFullName);
                Assert.IsFalse(addGrowthItems.IsOwnerSelected(expectedOwners[i]), $"Owner {ownerFullName} is still selected");
            }

            Log.Info("Click on 'Status' button and verify the status list");
            addGrowthItems.ClickOnStatusDropDown();
            var actualStatusList = new List<string>(addGrowthItems.GetStatusList());
            var expectedStatus = _growthPlanStatusResponse.Select(c => c.Status).ToList();
            Assert.That.ListsAreEqual(expectedStatus, actualStatusList, "Status List doesn't match");
            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            addGrowthItems.SelectStatus(growthItemInfo.Status);

            Log.Info("Click on 'Priority' button and verify the Priority list");
            addGrowthItems.ClickOnPriorityDropDown();
            var actualPriorityList = new List<string>(addGrowthItems.GetPriorityList());
            var expectedPriority = GrowthPlanFactory.GetGrowthPlanPriority();
            Assert.That.ListsAreEqual(expectedPriority, actualPriorityList, "Priority List doesn't match");
            addGrowthItems.SelectPriority(growthItemInfo.Priority);

            Log.Info("Click on 'Category' button and verify the Category list");
            addGrowthItems.ClickOnCategoryDropDown();
            var actualCategoryList = new List<string>(addGrowthItems.GetCategoryList());
            var expectedCategory = GrowthPlanFactory.GetGrowthPlanCategory();
            Assert.That.ListsAreEqual(expectedCategory, actualCategoryList, "Category List doesn't match");
            addGrowthItems.SelectCategory(growthItemInfo.Category);

            Log.Info("Click on 'Assessment type' button and verify the Assessment type list");
            addGrowthItems.ClickOnSelectAssessmentDropDown();
            var actualAssessmentTypeList = new List<string>(addGrowthItems.GetAssessmentTypeList());
            var expectedAssessmentTypeList = _teamWithMemberCountResponse.SelectMany(z => z.Surveys.Select(c => (c.Name).Trim())).ToList();
            Assert.That.ListsAreEqual(expectedAssessmentTypeList, actualAssessmentTypeList, "Assessment type List is not matched");
            addGrowthItems.DoubleClickOnOwnerDropDown();

            Log.Info("Click on 'Target Competencies' button and verify the Target Competencies list");
            addGrowthItems.ClickOnSelectAssessmentDropDown();
            addGrowthItems.ClickOnAssessmentTypeSelect(expectedAssessmentTypeList[0]);
            addGrowthItems.ClickOnTargetCompetenciesDropDown();
            var actualTargetCompetencyListProgramHealth = new List<string>(addGrowthItems.GetTargetCompetenciesList());
            var expectedTargetCompetenciesListProgramHealth = _targetCompetenciesOfFirstSurvey.Select(c => c.Name).ToList();
            Assert.That.ListsAreEqual(expectedTargetCompetenciesListProgramHealth, actualTargetCompetencyListProgramHealth, "Target Competencies List for Program Health doesn't match");
            addGrowthItems.DoubleClickOnOwnerDropDown();

            recordToMatch = 3;
            for (var i = 0; i < recordToMatch; i++)
            {
                addGrowthItems.ClickOnTargetCompetenciesDropDown();
                var competency = _targetCompetenciesOfFirstSurvey[i].Name;
                addGrowthItems.SelectTargetCompetencies(competency);
                Assert.IsTrue(addGrowthItems.IsTargetCompetenciesItemSelected(competency), $"TargetCompetencies {competency} is not selected");
            }

            var actualSelectedTargetCompetenciesList = addGrowthItems.GetSelectedTargetCompetenciesList();
            var expectedTargetCompetencyListAt25ThDoNotUse = _targetCompetenciesOfFirstSurvey.Select(a => a.Name).ToList().GetRange(0, recordToMatch);
            Assert.That.ListsAreEqual(expectedTargetCompetencyListAt25ThDoNotUse, actualSelectedTargetCompetenciesList.ToList(), "Selected TargetCompetencies are not matched");

            for (var i = 0; i < recordToMatch; i++)
            {
                var competency = _targetCompetenciesOfFirstSurvey[i].Name;
                addGrowthItems.DeselectTargetCompetency(competency);
                Assert.IsFalse(addGrowthItems.IsTargetCompetenciesItemSelected(competency), $"TargetCompetencies {competency} is still selected");
            }

            Log.Info("Click on 'Types' button and verify the Types list");
            addGrowthItems.ClickOnTypesDropDown();
            var expectedTypeList = GrowthPlanFactory.GetNewGrowthPlanTypes();
            var actualTypeList = new List<string>(addGrowthItems.GetTypesList());
            Assert.That.ListsAreEqual(expectedTypeList, actualTypeList, "Types List doesn't match");
        }
    }
}
