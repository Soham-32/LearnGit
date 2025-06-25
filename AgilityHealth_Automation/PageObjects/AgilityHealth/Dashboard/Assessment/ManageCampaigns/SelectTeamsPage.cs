using System.Linq;
using OpenQA.Selenium;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class SelectTeamsPage : ManageCampaignsCommonPage
    {
        public SelectTeamsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By ContinueToFacilitatorsButton = By.XPath("//button[text()='Continue to facilitators']");
        private readonly By AddTeamsButton = By.XPath("//button[text()='Add Teams']");
        private readonly By SearchTeamTextBox = By.XPath("//input[@name='keywordFilter']");
        private readonly By SearchByTagTextBox = By.XPath("//input[@name='tagFilter']");
        private readonly By FilterByWorkTypeDropDown = By.XPath("//div[@id='mui-component-select-workTypeFilter']");
        private readonly By TeamContactIsAhfDropDown = By.XPath("//div[@aria-haspopup='listbox' and not(@id)]");
        private readonly By SelectTeamPopupDescription = By.XPath("//div//p[contains(text(),'Search for a team')]");
        private static By TeamCheckBox(string teamName) => By.XPath($"//td[contains(text(),'{teamName}')]/parent::tr/td//span | //td//font[contains(text(),'{teamName}')]//ancestor::tr//td//span");
        private By AllTeamsCheckBox = By.XPath("//table[@role='presentation']//thead//input");
        private readonly By DropDownItemList = By.XPath("//div[@data-testid='sentinelStart']//following-sibling::div//ul//li/span[not(@class)]");
        private static By DropDownItem(string option) => By.XPath($"//div[@data-testid='sentinelStart']//following-sibling::div//ul//li//span[text()='{option}'] | //div[@data-testid='sentinelStart']//following-sibling::div//ul//li//span//font[text()='{option}']");

        private readonly By IsFacilitatorList = By.XPath("//table[contains(@class,'k-grid-tab')]//td[count(//table[contains(@class,'k-grid-header')]//th//span[text()='Is Facilitator']//ancestor::th//preceding-sibling::th)+5]");
        private static By TagNamesList(string tagName) => By.XPath($"//table[contains(@class,'k-grid-tab')]//td[count(//table[contains(@class,'k-grid-header')]//th[@colspan='1']//span[text()='Tags']//ancestor::th//preceding-sibling::th)+1]/div/div[contains(text(),'{tagName}')] | //table[contains(@class,'k-grid-tab')]//td[count(//table[contains(@class,'k-grid-header')]//th[@colspan='1']//span//font[text()='Tags']//ancestor::th//preceding-sibling::th)+1]/div/div//font[contains(text(),'{tagName}')]");
        private readonly By TeamContactIsAhfInfoIcon = By.XPath("//button[@aria-label='Information']//*[local-name() = 'svg']");
        private readonly By TooltipOfTeamContactIsAhf = By.XPath("//div[@data-foo='bar']");


        //Methods
        public void ClickOnContinueToFacilitatorButton()
        {
            Log.Step(GetType().Name, "Click on 'Continue to facilitator' button");
            Wait.HardWait(7000);//wait till button enabled/disabled
            Wait.UntilElementClickable(ContinueToFacilitatorsButton).Click();
        }

        public bool IsContinueToFacilitatorButtonEnabled()
        {
            return Driver.IsElementEnabled(ContinueToFacilitatorsButton);
        }

        public void ClickOnAddTeamsButton()
        {
            Log.Step(GetType().Name, "Click on 'Add Teams' button to select teams");
            WaitTillSpinnerNotExist();
            Wait.UntilElementClickable(AddTeamsButton).Click();
            Wait.HardWait(5000); //wait till team added
        }
        public bool IsAddTeamsButtonDisplayed()
        {
            WaitTillSpinnerNotExist();
            return Driver.IsElementDisplayed(AddTeamsButton);
        }

        public void SearchWithTeamName(string teamName, bool isPopUp)
        {
            Log.Step(GetType().Name, $"Search with '{teamName}' team name");
            var searchTextBox = isPopUp ? PrefixXPath(SearchTeamTextBox, SelectTeamsPrefixText) : SearchTeamTextBox;
            Wait.UntilElementVisible(searchTextBox).SetText(teamName).SendKeys(Keys.Tab);
            WaitTillSpinnerNotExist();
            Wait.UntilJavaScriptReady();
            Wait.HardWait(3000);//wait till search reflected
        }

        public void RemoveSearchText()
        {
            Log.Step(GetType().Name, "Remove values from the 'Search' textbox");
            Wait.UntilElementExists(SearchTeamTextBox).ClearTextbox();
            Wait.UntilElementNotExist(ProgressBar);
        }

        public void SearchWithTagName(string tagName, bool isPopUp)
        {
            Log.Step(GetType().Name, $"Search with '{tagName}' team name");
            var tagTextBox = isPopUp ? PrefixXPath(SearchByTagTextBox, SelectTeamsPrefixText) : SearchByTagTextBox;
            Wait.UntilElementVisible(tagTextBox).SetText(tagName).SendKeys(Keys.Tab);
            WaitTillSpinnerNotExist();
            Wait.HardWait(4000);//wait till search reflected
        }

        public void SelectFilterByWorkType(string radarType, bool isPopUp)
        {
            Log.Step(GetType().Name, "Select values for 'Work Type' dropdown");
            var workTypeDropDown = isPopUp ? PrefixXPath(FilterByWorkTypeDropDown, SelectTeamsPrefixText) : FilterByWorkTypeDropDown;
            SelectItem(workTypeDropDown, DropDownItem(radarType));
            WaitTillSpinnerNotExist();
        }

        public List<string> GetWorkTypeDropdownList()
        {
            Log.Step(GetType().Name, "Get list of all the Work types");
            return Driver.GetTextFromAllElements(DropDownItemList).ToList();
        }

        public void SelectTeamContactIsAhf(string option, bool isPopUp)
        {
            Log.Step(GetType().Name, "Select values for 'is AHF' dropdown");
            var teamContactIsAhfDropDown = isPopUp ? PrefixXPath(TeamContactIsAhfDropDown, SelectTeamsPrefixText) : TeamContactIsAhfDropDown;
            SelectItem(teamContactIsAhfDropDown, DropDownItem(option));
            WaitTillSpinnerNotExist();
        }

        public string GetSelectTeamDescription()
        {
            return Wait.UntilElementVisible(SelectTeamPopupDescription).GetText();
        }
        public List<string> GetIsAhfList()
        {
            Log.Step(GetType().Name, "Get list of all the 'Is AHF' options");
            return Driver.GetTextFromAllElements(DropDownItemList).ToList();
        }

        public List<string> GetListOfTeams()
        {
            Log.Step(GetType().Name, "Get list of all the Teams");
            return Driver.GetTextFromAllElements(ColumnValuesList("name")).ToList();
        }

        public void SelectTeamCheckBox(List<string> teamsList)
        {
            Log.Step(GetType().Name, "Select team check box");
            foreach (var team in teamsList)
            {
                Wait.HardWait(2000);//wait till checkbox state changed
                Wait.UntilElementClickable(TeamCheckBox(team)).Check();
                Wait.HardWait(4000);//wait till checkbox state changed
            }
        }

        public void SelectAllTeamCheckBox(bool isPopUp)
        {
            Log.Step(GetType().Name, "Select all teams checkbox");
            if (isPopUp)
            {
                AllTeamsCheckBox = PrefixXPath(AllTeamsCheckBox, SelectTeamsPrefixText);
            }
            Wait.UntilElementClickable(AllTeamsCheckBox).Check();
        }

        public bool IsAddToCampaignButtonEnabled()
        {
            return Driver.IsElementEnabled(AddToCampaignButton);
        }

        public List<string> GetIsFacilitatorColumnList(bool isPopUp)
        {
            Log.Step(GetType().Name, "Get list of 'Is Facilitator' column");
            var itemsList = isPopUp ? Driver.GetTextFromAllElements(PrefixXPath(IsFacilitatorList, SelectTeamsPrefixText)).ToList() : Driver.GetTextFromAllElements(IsFacilitatorList).ToList();
            return itemsList;
        }

        public List<string> GetTagNameColumnList(string tagName, bool isPopUp)
        {
            Log.Step(GetType().Name, "Get list of 'Team Name'");
            var itemsList = isPopUp ? Driver.GetTextFromAllElements(PrefixXPath(TagNamesList(tagName), SelectTeamsPrefixText)).ToList() : Driver.GetTextFromAllElements(TagNamesList(tagName)).ToList();
            return itemsList;
        }

        public void HoverOverOnTeamContactIsAHFInfoIcon()
        {
            Driver.MoveToElement(Wait.UntilElementVisible(TeamContactIsAhfInfoIcon));
        }

        public string GetTooltipMessageOfTeamContactIsAhf()
        {
            return Wait.UntilElementVisible(TooltipOfTeamContactIsAhf).GetText();
        }
    }
}