using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.GrowthPlanV2.Dashboard;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.GrowthPlanV2.Add
{
    [TestClass]
    [TestCategory("GrowthPlanV2"), TestCategory("GrowthPlan")]
    public class GrowthPlanAddItemTests2 : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly GrowthItem GrowthItemInfo = GrowthPlanFactory.GetValidGrowthItem();

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.TurnOnOffGrowthItemTypeFieldRequiredFeature(Company.Id, false);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GPV2_Add_MandatoryFieldsVerification()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthPlanDashboard = new GrowthPlanDashboardPage(Driver, Log);
            var growthPlanAddItemPage = new GrowthPlanAddItemPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            growthPlanDashboard.NavigateToPage(Company.Id);
            growthPlanDashboard.ClickOnAddGrowthItemButton();

            growthPlanAddItemPage.FillForm(GrowthItemInfo);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");

            Log.Info("Verify 'Save' button enabled or not, while 'Category' selected / deselected");
            Driver.RefreshPage();
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.ClickOnOwnerDropDown();
            growthPlanAddItemPage.SelectOwner(GrowthItemInfo.Owner);
            growthPlanAddItemPage.ClickOnSelectAssessmentDropDown();
            growthPlanAddItemPage.SelectAssessmentType(GrowthItemInfo.RadarType);
            growthPlanAddItemPage.ClickOnTargetCompetenciesDropDown();
            var targetCompetencies = growthPlanAddItemPage.GetTargetCompetenciesList();
            growthPlanAddItemPage.SelectTargetCompetencies(targetCompetencies[0]);
            growthPlanAddItemPage.EnterGrowthPlanTitle(GrowthItemInfo.Title);
            growthPlanAddItemPage.ClickOnTypesDropDown();
            growthPlanAddItemPage.SelectType(GrowthItemInfo.Type);
            growthPlanAddItemPage.SetStartDate(DateTime.Parse(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy")));
            growthPlanAddItemPage.EnterDescription(GrowthItemInfo.Description);
            Assert.IsFalse(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is enabled");
            growthPlanAddItemPage.ClickOnCategoryDropDown();
            growthPlanAddItemPage.SelectCategory(GrowthItemInfo.Category);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");

            Log.Info("Verify 'Save' button enabled or not, when 'Title' field blank");
            Driver.RefreshPage();
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.ClickOnOwnerDropDown();
            growthPlanAddItemPage.SelectOwner(GrowthItemInfo.Owner);
            growthPlanAddItemPage.ClickOnSelectAssessmentDropDown();
            growthPlanAddItemPage.SelectAssessmentType(GrowthItemInfo.RadarType);
            growthPlanAddItemPage.ClickOnTargetCompetenciesDropDown();
            growthPlanAddItemPage.SelectTargetCompetencies(targetCompetencies[0]);
            growthPlanAddItemPage.ClickOnCategoryDropDown();
            growthPlanAddItemPage.SelectCategory(GrowthItemInfo.Category);
            growthPlanAddItemPage.ClickOnTypesDropDown();
            growthPlanAddItemPage.SelectType(GrowthItemInfo.Type);
            growthPlanAddItemPage.SetStartDate(DateTime.Parse(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy")));
            growthPlanAddItemPage.EnterDescription(GrowthItemInfo.Description);
            Assert.IsFalse(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is enabled");
            growthPlanAddItemPage.EnterGrowthPlanTitle(GrowthItemInfo.Title);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");


            Log.Info("Verify 'Save' button enabled or not, while 'Team Assessment Type' selected / deselected");
            Driver.RefreshPage();
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.ClickOnOwnerDropDown();
            growthPlanAddItemPage.SelectOwner(GrowthItemInfo.Owner);
            growthPlanAddItemPage.ClickOnCategoryDropDown();
            growthPlanAddItemPage.SelectCategory(GrowthItemInfo.Category);
            growthPlanAddItemPage.EnterGrowthPlanTitle(GrowthItemInfo.Title);
            growthPlanAddItemPage.ClickOnTypesDropDown();
            growthPlanAddItemPage.SelectType(GrowthItemInfo.Type);
            growthPlanAddItemPage.SetStartDate(DateTime.Parse(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy")));
            growthPlanAddItemPage.EnterDescription(GrowthItemInfo.Description);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");
            growthPlanAddItemPage.ClickOnSelectAssessmentDropDown();
            growthPlanAddItemPage.SelectAssessmentType(GrowthItemInfo.RadarType);
            Assert.IsFalse(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is enabled");

            Log.Info("Verify 'Save' button enabled or not, while 'Competency Targets' selected / deselected");
            Driver.RefreshPage();
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.ClickOnOwnerDropDown();
            growthPlanAddItemPage.SelectOwner(GrowthItemInfo.Owner);
            growthPlanAddItemPage.ClickOnSelectAssessmentDropDown();
            growthPlanAddItemPage.SelectAssessmentType(GrowthItemInfo.RadarType);
            growthPlanAddItemPage.ClickOnCategoryDropDown();
            growthPlanAddItemPage.SelectCategory(GrowthItemInfo.Category);
            growthPlanAddItemPage.EnterGrowthPlanTitle(GrowthItemInfo.Title);
            growthPlanAddItemPage.ClickOnTypesDropDown();
            growthPlanAddItemPage.SelectType(GrowthItemInfo.Type);
            growthPlanAddItemPage.SetStartDate(DateTime.Parse(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy")));
            growthPlanAddItemPage.EnterDescription(GrowthItemInfo.Description);
            Assert.IsFalse(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is enabled");
            growthPlanAddItemPage.ClickOnTargetCompetenciesDropDown();
            growthPlanAddItemPage.SelectTargetCompetencies(targetCompetencies[0]);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");

            Log.Info("Verify that 'Save' button enabled when optional fields selected / deselected");
            Driver.RefreshPage();
            growthPlanDashboard.ClickOnAddGrowthItemButton();
            growthPlanAddItemPage.ClickOnSelectAssessmentDropDown();
            growthPlanAddItemPage.SelectAssessmentType(GrowthItemInfo.RadarType);
            growthPlanAddItemPage.ClickOnTargetCompetenciesDropDown();
            growthPlanAddItemPage.SelectTargetCompetencies(targetCompetencies[0]);
            growthPlanAddItemPage.ClickOnCategoryDropDown();
            growthPlanAddItemPage.SelectCategory(GrowthItemInfo.Category);
            growthPlanAddItemPage.EnterGrowthPlanTitle(GrowthItemInfo.Title);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");
            growthPlanAddItemPage.ClickOnOwnerDropDown();
            growthPlanAddItemPage.SelectOwner(GrowthItemInfo.Owner);
            growthPlanAddItemPage.ClickOnTypesDropDown();
            growthPlanAddItemPage.SelectType(GrowthItemInfo.Type);
            growthPlanAddItemPage.SetStartDate(DateTime.Parse(GrowthItemInfo.TargetDate?.ToString("MM/dd/yyyy")));
            growthPlanAddItemPage.EnterDescription(GrowthItemInfo.Description);
            Assert.IsTrue(growthPlanAddItemPage.IsSaveButtonEnabled(), "'Save' button is disabled");
        }
    }
}
