using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility
{
    internal class StructuralAgilityPage : InsightsDashboardPage
    {
        public StructuralAgilityPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By AllocationByRoleWorkTypeListbox = By.XPath(
            $"//div[@id = 'id_widget_{StructuralAgilityWidgets.AllocationByRole.Id}']//div[@automation-id='id-filter-worktype']");
        private readonly By TeamStabilityNumberOfTeam = By.XPath("//div[@id='id_widget_structuralagilityteamstability']//div[contains(@class,'highcharts-data-label-color-0')]//div[@style='font-size: 14px;']");
        private static By TeamItem(string item) => By.CssSelector($"div[title='{item}']");

        private readonly By ParticipantGroupFilterListbox = AutomationId.Equals("id-filter-participantgroup-child");
        private readonly By PeopleGroupFilterListbox =
            By.XPath("//div[@id = 'id_widgetGroup_People']//div[@automation-id = 'id-filter-worktype']/div");
        private readonly By TeamCategoriesFilterListbox = AutomationId.Equals("id-filter-teamcategories", "div");

        private static By ListItem(string item) => By.XPath($"//div[@id = 'menu-']//li[@data-value = '{item}']");
        private readonly By SettingsButton = By.XPath("//div[@id = 'id_tab_container']//button[@label = 'Settings']");
        private readonly By AskAgilityToggleButton = By.Id("main_AI");
        private readonly By AskAgilityInsightsDialogBoxTitle = By.Id("aiDialogTitle");
        private readonly By QuestionBankTab = By.XPath("//button[text()='Question Bank']");
        private readonly By QuestionInputBox = By.XPath("//div[@class='utteranceContent']");
        private readonly By AskAgilityInsightsTab = By.XPath("//button[contains(text(),'Ask AgilityInsights - AI')][@aria-selected='false']");
        private readonly By AllEntityQuestions = By.XPath("//li[contains(@class,'entityListItem')]/div/span[1]");
        private readonly By NlqIframe = By.XPath("//div[@class='aiPowerBIEmbed']/iframe[contains(@src,'https://app.powerbi.com/reportEmbed?')]");
        private readonly By BlankResponseText = By.CssSelector("div.visual-card svg[aria-label*='Blank']");
        private readonly By StructuralAgilityTabText = By.XPath("//button[text()='Structural Agility'] | //button//*[text()='Structural Agility']");

        // Methods
        public void SelectAllocationByRoleWorkType(string workType)
        {
            Log.Step(nameof(StructuralAgilityTab), $"Select Work Type item <{workType}> for widget '{StructuralAgilityWidgets.AllocationByRole.Title}");
            if (GetAllocationByRoleWorkType() != workType)
                SelectItem(AllocationByRoleWorkTypeListbox, ListItem(workType));
            WaitUntilWidgetsLoaded();
        }

        public string GetAllocationByRoleWorkType()
        {
            Log.Step(nameof(StructuralAgilityTab), $"Get AllocationByRole Work type for widget '{StructuralAgilityWidgets.AllocationByRole.Title}");
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(AllocationByRoleWorkTypeListbox));
            return Wait.UntilElementExists(AllocationByRoleWorkTypeListbox).GetText();
        }

        public void SelectParticipantGroupFilter(string item)
        {
            Log.Step(nameof(StructuralAgilityTab), $"Select item <{item}> in the Participant Group dropdown");
            if (GetParticipantGroupFilter() != item)
                SelectItem(ParticipantGroupFilterListbox, ListItem(item));
            var action = new Actions(Driver);
            action.SendKeys(Keys.Escape).Perform();
            WaitUntilWidgetsLoaded();
        }

        public string GetParticipantGroupFilter()
        {
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(ParticipantGroupFilterListbox));
            return Wait.UntilElementVisible(ParticipantGroupFilterListbox).GetText();
        }

        public void SelectPeopleFilter(string item)
        {
            Log.Step(nameof(StructuralAgilityTab), $"Select item <{item}> in the 'Work Type' dropdown");
            if (GetPeopleFilter() != item)
                SelectItem(PeopleGroupFilterListbox, ListItem(item));
            WaitUntilWidgetsLoaded();
        }

        public string GetPeopleFilter()
        {
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(PeopleGroupFilterListbox));
            return Wait.UntilElementExists(PeopleGroupFilterListbox).GetText();
        }

        public void SelectTeamCategories(string item)
        {
            Log.Step(nameof(StructuralAgilityTab), $"Select item <{item}> in the 'Category Name' dropdown");
            if (GetTeamCategoriesFilter() != item)
                SelectItem(TeamCategoriesFilterListbox, ListItem(item));
            WaitUntilWidgetsLoaded();
        }

        public string GetTeamCategoriesFilter()
        {
            Driver.JavaScriptScrollToElement(Wait.UntilElementExists(TeamCategoriesFilterListbox));
            return Wait.UntilElementExists(TeamCategoriesFilterListbox).GetText();
        }

        public string GetTeamStabilityNumberOfTeam()
        {
            Log.Step(nameof(StructuralAgilityTab), "Get team stability number of team from the 'Structural Agility' page");
            ScrollWidgetIntoView(StructuralAgilityWidgets.TeamStability);
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.HardWait(5000); //The widget takes time to load
            return Wait.UntilElementVisible(TeamStabilityNumberOfTeam).GetText();
        }

        public void SelectTeam(string team)
        {
            Log.Step(nameof(StructuralAgilityTab), $"In the left nav, click on the team name <{team}>");
            Wait.UntilElementVisible(TeamItem(team)).Click();
            WaitUntilWidgetsLoaded();
        }

        public void ClickSettingsButton()
        {
            Log.Step(nameof(StructuralAgilityTab), "Click on the 'Settings' button");
            Wait.UntilElementClickable(SettingsButton).Click();
            WaitUntilWidgetsLoaded();
        }

        public bool IsSettingsButtonVisible()
        {
            return Wait.InCase(SettingsButton)?.Displayed ?? false;
        }

        public string GetStructuralAgilityTabText()
        {
            return Wait.UntilElementVisible(StructuralAgilityTabText).GetText();
        }

        public void ClickOnAskAgilityToggleButton()
        {
            Log.Step(nameof(StructuralAgilityTab), "Click on the 'Ask Agility' button.");
            Wait.UntilElementClickable(AskAgilityToggleButton).Click();
        }

        public bool IsAskAgilityToggleButtonDisplayed()
        {
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.HardWait(9000); //Due to navigation it may take time to load. 
            return Driver.IsElementDisplayed(AskAgilityToggleButton);
        }

        public void ClickOnQuestionBankTab()
        {
            Log.Step(nameof(StructuralAgilityTab), "Click on the 'Question Bank' tab.");
            Wait.HardWait(1000);//Time takes to load Question bank
            Wait.UntilElementClickable(QuestionBankTab).Click();
        }

        public bool IsAskAgilityInsightsDialogBoxDisplayed()
        {
            Wait.HardWait(5000);//Ask Agility tab takes time to load
            return Driver.IsElementDisplayed(AskAgilityInsightsDialogBoxTitle,20);
        }

        public bool IsQuestionBankTabActive()
        {
            return Wait.UntilElementVisible(QuestionBankTab).GetAttribute("aria-selected").Equals("true");
        }

        public void ClickOnAskAgilityInsightsTab()
        {
            Log.Step(nameof(StructuralAgilityTab), "Click on the 'Ask Agility Insights' tab.");
            Wait.UntilElementClickable(AskAgilityInsightsTab).Click();
        }

        public string ClickAndGetFirstQuestion()
        {
            Log.Step(nameof(StructuralAgilityTab), "Click on random question from the displayed questions.");
            var copiedQuestionText = Wait.UntilElementVisible(AllEntityQuestions).GetText();
            Wait.UntilElementClickable(AllEntityQuestions).Click();
            return copiedQuestionText;
        }

        public void EnterCopiedQuestionInTheInputBox()
        {
            Log.Step(nameof(StructuralAgilityTab), "Enter the copied question in the input box.");
            var actions = new Actions(Driver);
            Wait.UntilElementClickable(QuestionInputBox).Click();
            actions.KeyDown(Keys.Control).SendKeys("v").KeyUp(Keys.Control).SendKeys(Keys.Enter).Perform();
        }

        public void SwitchToNlqIframe()
        {
            Driver.SwitchToFrame(NlqIframe);
        }
        public bool IsReceivedResponseBlank()
        {
            return Driver.IsElementDisplayed(BlankResponseText);
        }
        //Navigation
        public void NavigateToInsightsStructuralAgilityPageForProd(string env, int companyId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/V2/insights/company/{companyId}/parents/0/team/0/tab/structuralagility");
        }
    }

    public static class StructuralAgilityWidgets
    {
        public static InsightsWidget TeamStability = new InsightsWidget("Team Stability", "structuralagilityteamstability");
        public static InsightsWidget TeamStabilityDistribution = new InsightsWidget("Team Stability Distribution", "structuralagilityteamstabilitydistribution");
        public static InsightsWidget AllocationByRole = new InsightsWidget("Allocation by Role", "roleallocation");
        public static InsightsWidget AllocationPerTeam = new InsightsWidget("Allocation per Team", "teamroleallocation");
        public static InsightsWidget ParticipantGroup = new InsightsWidget("Participant Group", "structuralagilityparticipantgroup");
        public static InsightsWidget ParticipantGroupDistribution = new InsightsWidget("Participant Group Distribution", "structuralagilityparticipantgroupdistribution");
        public static InsightsWidget PeopleByRole = new InsightsWidget("Roles", "structuralagilitypeoplebyrole");
        public static InsightsWidget PeopleDistribution = new InsightsWidget("Roles Distribution", "structuralagilitypeoplebyroledistribution");
        public static InsightsWidget AgileAdoption = new InsightsWidget("Agile Adoption", "structuralagilityagilevsnonagileteams");
        public static InsightsWidget AgileAdoptionDistribution = new InsightsWidget("Agile Adoption Distribution", "structuralagilityagilevsnonagiledistribution");

        public static List<InsightsWidget> GetList()
        {
            return new List<InsightsWidget>
            {
                TeamStability,
                TeamStabilityDistribution,
                AllocationByRole,
                AllocationPerTeam,
                ParticipantGroup,
                ParticipantGroupDistribution,
                PeopleByRole,
                PeopleDistribution,
                AgileAdoption,
                AgileAdoptionDistribution
            };
        }
    }

}