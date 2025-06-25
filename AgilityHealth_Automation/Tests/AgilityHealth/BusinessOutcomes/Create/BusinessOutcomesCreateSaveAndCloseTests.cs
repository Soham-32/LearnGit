using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Create
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCreateSaveAndCloseTests : BusinessOutcomesBaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_BusinessOutcome_Add_SaveAndClose()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            var expected = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id,
                SwimlaneType.StrategicIntent);
            expected.Tags = new List<BusinessOutcomeTagRequest>
            {
                new BusinessOutcomeTagRequest { Name = "Tag 1" }, 
                new BusinessOutcomeTagRequest { Name = "Tag 2" }
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnPlusButton(expected.SwimlaneType.GetDescription());

            addBusinessOutcomePage.FillForm(expected);

            var keyResult1 = BusinessOutcomesFactory.GetKeyResultRequest(Company.Id);
            addBusinessOutcomePage.KeyResultsTab.AddKeyResult(keyResult1);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(expected.Title), 
                $"New created business outcome with title {expected.Title} is not displayed on the dashboard.");
            
            var expectedPercentage = GetKeyResultsProgressPercentage(new List<KeyResultRequest> { keyResult1 });
            Assert.AreEqual($"{expectedPercentage}%", 
                businessOutcomesDashboard.GetOverallPercentageValue(expected.SwimlaneType.GetDescription(), expected.Title),
                "Overall percentage doesn't match");

            //Verifying overall progress indicator
            var expectedOverallProgress = $"transform: translateX({float.Parse(expectedPercentage) - 100}%);";

            //Verifying Overall progress bar color
            var overallProgressColor = businessOutcomesDashboard.GetOverallProgressBarColor(expected.SwimlaneType.GetDescription(), expected.Title);
            Assert.AreEqual(expected.CardColor, 
                CSharpHelpers.ConvertRgbToHex(overallProgressColor), 
                $"Color doesn't match for '{expected.Title}' overall progress bar");

            expected.Tags.ForEach(tag => Assert.IsTrue(
                businessOutcomesDashboard.IsBusinessOutcomeCategoryPresent(expected.SwimlaneType.GetDescription(), 
                    expected.Title, tag.Name),
                $"Business outcome- {expected.Title} doesn't have category - {tag}"));

            //Expanding Card
            businessOutcomeBasePage.CardSearch(expected.Title);
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(expected.Title);

            //Key Result Name & Percentage
            Assert.AreEqual($"{expectedPercentage}%", 
                businessOutcomesDashboard.GetKeyResultPercentageValue(expected.Title, keyResult1.Title), 
                $"Key Result - {keyResult1.Title}  percentage doesn't match");

            //Key Result progress indication
            Assert.AreEqual(expectedOverallProgress, 
                businessOutcomesDashboard.GetKeyResultProgressIndication(expected.Title, keyResult1.Title), 
                $"Key Result - {keyResult1.Title}  progress indication doesn't match");

            //Verifying key result progress bar color
            var keyResultProgressColor = businessOutcomesDashboard.GetKeyResultProgressBarColor(expected.Title, keyResult1.Title);
            Assert.AreEqual(expected.CardColor, CSharpHelpers.ConvertRgbToHex(keyResultProgressColor), 
                $"Color doesn't match for '{keyResult1.Title}' key result progress bar");

            //Verifying color left vertical stripe
            Assert.AreEqual($"background: {CSharpHelpers.CovertHexToRgb(expected.CardColor)};", 
                businessOutcomesDashboard.GetColorInfoForLeftVerticalStripe(expected.Title), 
                $"Color doesn't match for '{expected.Title} left vertical stripe'");

        }
    }
}