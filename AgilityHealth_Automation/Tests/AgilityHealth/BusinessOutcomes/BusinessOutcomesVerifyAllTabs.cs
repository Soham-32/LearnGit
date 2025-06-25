using System.Collections.Generic;
using System.IO;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes
{
    
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesVerifyAllTabs : BaseTest
    {
        public static BusinessOutcomeTabs CardTypeTabs = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/BusinessOutcomes/BusinessOutcomeCardTabs.json").DeserializeJsonObject<BusinessOutcomeTabs>();

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_VerifyAllTabsForBusinessOutcomeCardType()
        {
            VerifyAllTabs(CardTypeTabs.BusinessOutcome.BusinessOutcomes ,BusinessOutcomesCardType.BusinessOutcomes.GetDescription(), TimeFrameTags.Annually.GetDescription(),SwimlaneType.StrategicIntent.GetDescription());
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_VerifyAllTabsForDeliverableCardType()
        {
            VerifyAllTabs(CardTypeTabs.BusinessOutcome.Deliverables, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription(),"Sprint 1");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_VerifyAllTabsForInitiativeCardType()
        {
            VerifyAllTabs(CardTypeTabs.BusinessOutcome.Initiatives, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription(), "2024");
        }

        private void VerifyAllTabs(List<string> expectedTabs,string cardType,string labelName,string columnName)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
          
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(labelName);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(columnName);

            Log.Info($"Verify all tabs for the selected {cardType}");
            businessOutcomesDashboard.ClickOnPlusButton(columnName);
            var actualTabs = businessOutcomeBasePage.GetAllTabsName();
            Assert.That.ListsAreEqual(expectedTabs, actualTabs, $"All tabs are not displayed for selected {cardType}");
        }
    }
}