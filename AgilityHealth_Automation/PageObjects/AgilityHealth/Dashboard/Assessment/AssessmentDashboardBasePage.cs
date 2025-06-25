using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment
{
    public class AssessmentDashboardBasePage : BasePage
    {
        public AssessmentDashboardBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        public By AllTabsList = By.XPath("//div[@id='bottomNavBarForAssessmentManagement']/a");
        private readonly By SubTabName = By.XPath("//div[@id='headerScheduler']/h1 | //div[@id='headerScheduler']/h1//font");
        private static By SpecificTab(string tab) => By.Id($"{tab}");

        public enum TabSelection
        {
            SchedulerTab,
            BatchesTab,
            AssessmentListTab,
            ManageCampaignsTab
        }

        public readonly Dictionary<TabSelection, string> AllTabs = new Dictionary<TabSelection, string>()
        {
            {TabSelection.SchedulerTab, "SchedulerDashbaord"},
            {TabSelection.BatchesTab, "batches"},
            {TabSelection.AssessmentListTab, "assessmentListDashbaord"},
            {TabSelection.ManageCampaignsTab, "manageCampaigns"}
        };
        public string GetSubTabName()
        {
            Log.Step(nameof(AssessmentDashboardBasePage), $"On assessment dashboard page , Get '{SubTabName}' tab.");
            return Wait.UntilElementExists(SubTabName).GetText();
        }
        public void SelectSubTab(TabSelection tab)
        {
            Log.Step(nameof(AssessmentDashboardBasePage), $"On assessment dashboard page , Select '{tab}' & Verify breadcrumb navigation.");
            switch (tab)
            {
                case TabSelection.AssessmentListTab:
                    ClickOnTab(TabSelection.AssessmentListTab);
                    break;
                case TabSelection.SchedulerTab:
                    ClickOnTab(TabSelection.SchedulerTab);
                    break;
                case TabSelection.BatchesTab:
                    ClickOnTab(TabSelection.BatchesTab);
                    break;
                case TabSelection.ManageCampaignsTab:
                    ClickOnTab(TabSelection.ManageCampaignsTab);
                    break;
            }
        }
        public void ClickOnTab(TabSelection tab)
        {
            Log.Step(nameof(AssessmentDashboardBasePage), $"Click on {tab} Tab");
            Wait.UntilElementClickable(SpecificTab(AllTabs[tab])).Click();
        }

        public void HoverOnTab(TabSelection tab)
        {
            Log.Step(nameof(AssessmentDashboardBasePage), $"Hover on {tab} Tab");
            Driver.MoveToElement(Wait.UntilElementEnabled(SpecificTab(AllTabs[tab])));
        }

        public List<string> GetAllTabs()
        {
            Log.Step(nameof(AssessmentDashboardBasePage), "Get All Assessment Management Tabs");
            return Driver.GetTextFromAllElements(AllTabsList).ToList();
        }

        public string GetColorOfTab(TabSelection tab)
        {
            Log.Step(nameof(AssessmentDashboardBasePage), $"Get color of the {tab} Tab");
            return CSharpHelpers
                .ConvertRgbToHex(Wait.UntilElementVisible(SpecificTab(AllTabs[tab])).GetCssValue("color"))
                .ToLower();
        }

        public bool IsTabUnderlined(TabSelection tab)
        {
            var borderBottomValue = Wait.UntilElementVisible(SpecificTab(AllTabs[tab])).GetCssValue("border-bottom");
            return (borderBottomValue == "1px solid rgb(101, 161, 223)");
        }

        public bool IsTabDisplayed(TabSelection tab)
        {
            return Wait.InCase(SpecificTab(AllTabs[tab])) != null && Wait.InCase(SpecificTab(AllTabs[tab])).Displayed;
        }

    }
}

