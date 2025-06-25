using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AtCommon.Api.Enums;
using AtCommon.Api;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.Financials
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesFinancialAddTests1 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse  _deliverableCardResponse, _initiativeCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var deliverable = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription()).Tags;
            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            _deliverableCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, deliverable);
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Deliverables_Financials_Add()
        {
            AddAndValidateFinancialRows(_deliverableCardResponse, BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Initiatives_Checklist_Add()
        {
            AddAndValidateFinancialRows(_initiativeCardResponse, BusinessOutcomesCardType.AnnualView.GetDescription(), BusinessOutcomesCardTypeTags.AnnualView.GetDescription());
        }
    
        public void AddAndValidateFinancialRows(BusinessOutcomeResponse createdCardResponse, string cardType, string cardTypeTag)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);
            var financialsTab = new FinancialsTabPage(Driver, Log);

            var financialBudget = BusinessOutcomesFactory.GenerateFinancial();
            var spends = BusinessOutcomesFactory.GenerateTargetCurrentSpends(
                Company.Id,
                financialBudget.ApprovedBudget,
                new DateTime(2024, 1, 1)
            );

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the selected card type as {cardType} ");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.SelectCardType(cardType);
            businessOutcomesDashboard.SelectCardTypeFromDropdown(cardTypeTag);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely(createdCardResponse.sourceCategoryName ?? SwimlaneType.StrategicIntent.GetDescription());

            Log.Info($"Click on the created Card for the {cardType} and navigate to Financials");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Financials");
            financialsTab.ClickOnTableViewTab();
            financialsTab.AddFinancialBudgetData(financialBudget);
            foreach (var spend in spends)
            {
                financialsTab.AddFinancialSpendData(spend);
            }
         
            businessOutcomeBasePage.ClickOnSaveAndCloseButton();

            Log.Info("Reopen the Financial Tab and validate the added rows");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(createdCardResponse.Title);
            businessOutcomeBasePage.ClickOnTab("Financials");
            financialsTab.ClickOnTableViewTab();
            

            Log.Info("Validating the financial Budget");
            var actualFinancialRows = financialsTab.GetFinancialSpendDataRows();
            var actualFinancial = financialsTab.GetFinancialBudgetData();

            Assert.AreEqual(financialBudget.RequestedBudget, actualFinancial.RequestedBudget, "Mismatch in RequestedBudget");
            Assert.AreEqual(financialBudget.ApprovedBudget, actualFinancial.ApprovedBudget, "Mismatch in ApprovedBudget");
            Assert.AreEqual(financialBudget.BudgetCategory, actualFinancial.BudgetCategory, "Mismatch in BudgetCategory");
            Assert.AreEqual(spends.Sum(s => s.CurrentSpent), actualFinancial.TotalSpent, "Mismatch in TotalSpent");
            Assert.AreEqual(financialBudget.CalculationMethod, actualFinancial.CalculationMethod, "Mismatch in CalculationMethod");


            Log.Info("Validating the financial Spend Data");
            Assert.AreEqual(spends.Count, actualFinancialRows.Count, "Financial row count does not match.");
            for (var i = 0; i < spends.Count; i++)
            {
                Assert.AreEqual(spends[i].SpendingTarget.ToString(), actualFinancialRows[i]["TargetSpend"], $"Mismatch at row {i} - TargetSpend");
                Assert.AreEqual(spends[i].CurrentSpent.ToString(), actualFinancialRows[i]["CurrentSpend"], $"Mismatch at row {i} - CurrentSpend");
                Assert.AreEqual(spends[i].FinancialAsOfDate?.ToString("MMMM yyyy"), actualFinancialRows[i]["Date"], $"Mismatch at row {i} - Date");
            }
        }
    }
}