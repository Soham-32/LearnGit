using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit
{
    internal class EditCompanySubscriptionPage : SubscriptionBasePage
    {
        public EditCompanySubscriptionPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditCompanyHeaderWidget(driver, log);
        }

        public EditCompanyHeaderWidget Header { get; set; }

        // Locators
        private readonly By CurrentUsageCompanyName =
            AutomationId.StartsWith("current-usage-companyName");
        private readonly By CurrentUsageNumberOfTeams =
            AutomationId.StartsWith("current-usage-numberOfTeams");
        private readonly By CurrentUsageNumberOfMultiTeams =
            AutomationId.StartsWith("current-usage-numberOfMultiTeams");
        private readonly By CurrentUsageNumberOfEnterpriseTeams =
            AutomationId.StartsWith("current-usage-numberOfEnterpriseTeams");
        private readonly By CurrentUsageNumberOfTeamAssessments =
            AutomationId.StartsWith("current-usage-numberOfTeamAssessments");
        private readonly By CurrentUsageNumberOfIndividualAssessments =
            AutomationId.StartsWith("current-usage-numberOfIndividualAssessments");
        private readonly By CurrentUsageCompanyType =
            AutomationId.StartsWith("current-usage-companyType");

        private readonly By HistoryRows =
            By.XPath("//th[starts-with(@automation-id, 'history-table-header-Number')]/../following-sibling::tr");
        private readonly By HistoryId = AutomationId.StartsWith("history-table-id");
        private readonly By HistorySubscriptionType = AutomationId.StartsWith("history-table-subscriptionType");
        private readonly By TeamsLimitType = AutomationId.StartsWith("history-table-teams-limit");
        private readonly By AssessmentLimitType = AutomationId.StartsWith("history-table-assessments-limit");
        private readonly By HistoryDateContractSigned = AutomationId.StartsWith("history-table-dateContractSigned");
        private readonly By HistoryContractEndDate = AutomationId.StartsWith("history-table-contractEndDate");

        //Key-customer verification
        #region Elements
        private readonly By SubscriptionsSectionText = By.XPath("//p[text()='Subscriptions']");
        #endregion

        // Methods

        public CurrentUsageResponse GetCurrentUsage()
        {
            return new CurrentUsageResponse
            {
                CompanyName = Wait.UntilElementVisible(CurrentUsageCompanyName).GetText(),
                NumberOfTeams = Wait.UntilElementVisible(CurrentUsageNumberOfTeams).GetText().ToInt(),
                NumberOfMultiTeams = Wait.UntilElementVisible(CurrentUsageNumberOfMultiTeams).GetText().ToInt(),
                NumberOfEnterpriseTeams = Wait.UntilElementVisible(CurrentUsageNumberOfEnterpriseTeams).GetText().ToInt(),
                NumberOfTeamAssessments = Wait.UntilElementVisible(CurrentUsageNumberOfTeamAssessments).GetText().ToInt(),
                NumberOfIndividualAssessments = Wait.UntilElementVisible(CurrentUsageNumberOfIndividualAssessments).GetText().ToInt(),
                CompanyType = Wait.UntilElementVisible(CurrentUsageCompanyType).GetText()
            };
        }


        public List<SubscriptionHistoryItem> GetSubscriptionHistory()
        {
            var rows = Wait.UntilAllElementsLocated(HistoryRows);

            return rows.Select(row => new SubscriptionHistoryItem
            {
                Id = row.FindElement(HistoryId).GetText().ToInt(),
                SubscriptionType = row.FindElement(HistorySubscriptionType).GetText(),
                TeamsLimit = row.FindElement(TeamsLimitType).GetText().ToInt(),
                AssessmentsLimit = row.FindElement(AssessmentLimitType).GetText().ToInt(),
                DateContractSigned = row.FindElement(HistoryDateContractSigned).GetText(),
                ContractEndDate = row.FindElement(HistoryContractEndDate).GetText()
            }).ToList();
        }

        //Key-customer verification
        #region Elements
        public string GetSubscriptionsSectionText()
        {
            Log.Step(nameof(SubscriptionBasePage), "Get the 'Subscriptions' section text");
            return Wait.UntilElementVisible(SubscriptionsSectionText).GetText();
        }
        #endregion
    }

    internal class SubscriptionHistoryItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SubscriptionType { get; set; }
        public string DateContractSigned { get; set; }
        public string ContractEndDate { get; set; }
        public int TeamsLimit { get; set; }
        public int AssessmentsLimit { get; set; }
        public string ManagedSubscription { get; set; }
    }
}
