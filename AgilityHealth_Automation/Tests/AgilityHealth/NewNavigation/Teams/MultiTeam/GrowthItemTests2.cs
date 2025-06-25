using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("GrowthItem"), TestCategory("NewNavigation")]
    public class GrowthItemTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] // Bug Id : 47413
        public void GrowthItem_MultiTeam_GridView_All_Mandatory_Fields()
        {
            var loginPage = new LoginPage(Driver, Log);
            var leftNavPage = new LeftNavPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewPage(Driver, Log);

            Log.Info("Navigate to the application and login");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(User.Username, User.Password);
            teamDashboardPage.NavigateToPage(Company.Id);
            SwitchToNewNav();

            Log.Info("Click on Measure tab");
            teamDashboardPage.ClickOnMeasureTab();

            Log.Info("Expand Enterprise, Portfolio");
            leftNavPage.ExpandTeam(SharedConstants.EnterpriseTeam);
            leftNavPage.ExpandTeam(SharedConstants.PortfolioTeam);

            Log.Info("Click on a multi team");
            leftNavPage.ClickOnATeam(SharedConstants.MultiTeam);

            Log.Info("Switch to Grid view");
            growthItemGridViewPage.SwitchToGridView();

            Log.Info("Click on Add Growth Item button");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();

            var expectedList = new List<string>() {"Title", "Priority", "Category", "UpdatedSurveyId", "Type"};
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var giField in expectedList)
            {
                Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(giField), $"Validation message is not displayed for GI '{giField}' field");
            }

            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = "Other",
                Status = "Not Started",
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "Low"
            };

            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Title' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("Title"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field ");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Priority' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("Priority"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Category' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("Category"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Type' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("Type"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Radar Type' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("UpdatedSurveyId"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'CompetencyTargets' field blank");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.ClickOnSaveButton();
            expectedList.Add("CompetencyTargets");

            foreach (var expectedField in expectedList)
            {
                if (expectedField.Equals("CompetencyTargets"))
                {
                    Assert.IsTrue(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            growthItemGridViewPage.ClickOnCancelButton();

            Log.Info("Verify that validation message is not displayed after click on 'Save' button, when every mandatory fields are filled");
            growthItemGridViewPage.ClickOnAddGrowthItemButton();
            growthItemGridViewPage.SelectCategory(growthItemInfo.Category);
            growthItemGridViewPage.SetTitle(growthItemInfo.Title);
            growthItemGridViewPage.SelectType(growthItemInfo.Type);
            growthItemGridViewPage.SelectStatus(growthItemInfo.Status);
            growthItemGridViewPage.SelectRadarType(growthItemInfo.RadarType);
            growthItemGridViewPage.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            growthItemGridViewPage.SelectPriority(growthItemInfo.Priority);
            growthItemGridViewPage.ClickOnSaveButton();

            foreach (var expectedField in expectedList)
            {
                Assert.IsFalse(growthItemGridViewPage.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
            }
            Assert.IsTrue(growthItemGridViewPage.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");
        }
    }
}
