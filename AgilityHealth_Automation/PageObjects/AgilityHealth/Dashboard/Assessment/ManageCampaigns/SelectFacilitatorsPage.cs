using OpenQA.Selenium;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class SelectFacilitatorsPage : ManageCampaignsCommonPage
    {
        public SelectFacilitatorsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators

        private readonly By ContinueToMatchmakingButton = By.XPath("//button[text()='Continue to matchmaking']");
        private readonly By AddFacilitatorsButton = By.XPath("//button[text()='Add Facilitators']");
        private readonly By SearchNameTextBox = By.XPath("//p[contains(text(),'Search:')]//following::div/div/input[@placeholder='Search'] | //p//font[contains(text(),'Search:')]//following::div/div/input[@placeholder='Search']");
        private By ParentTeamDropDown = By.XPath("//p[text()='Parent Team:']/following::input[@role='combobox'] | //p//font[text()='Parent Team:']/following::input[@role='combobox'] ");
        private static By DropDownItem(string option) => By.XPath($"//div//ul//li[text()='{option}'] | //div//ul//li//font[text()='{option}']");
        private static By FirstNameCheckBox(string firstName) => By.XPath($"//td[text()='{firstName}']/ancestor::tr//td//input | //td//font[text()='{firstName}']/ancestor::tr//td//input");
        private By AllFacilitatorsCheckBox = By.XPath("//span[text()='First Name']/ancestor::tr/th//input | //span//font[text()='First Name']/ancestor::tr/th//input");


        //Methods
        public void ClickOnContinueToMatchmakingButton()
        {
            Log.Step(GetType().Name, "Click on 'Continue to Matchmaking' button");
            Wait.UntilElementClickable(ContinueToMatchmakingButton).Click();
            Wait.HardWait(2000); // Wait till description message got changed
        }

        public bool IsContinueToMatchmakingButtonEnabled()
        {
            return Driver.IsElementEnabled(ContinueToMatchmakingButton);
        }

        public void ClickOnAddFacilitatorsButton()
        {
            Log.Step(GetType().Name, "Click on 'Add Facilitators' button to select teams");
            Wait.UntilElementClickable(AddFacilitatorsButton).Click();
        }

        public bool IsAddFacilitatorsButtonDisplayed()
        {
            WaitTillSpinnerNotExist();
            return Driver.IsElementDisplayed(AddFacilitatorsButton);
        }

        public void SearchWithFacilitatorName(string name, bool isPopUp)
        {
            Log.Step(GetType().Name, $"Search with '{name}' Facilitator name");
            var searchTextBox = isPopUp ? PrefixXPath(SearchNameTextBox, SelectFacilitatorsPrefixText) : SearchNameTextBox;

            Wait.UntilElementVisible(searchTextBox).SetText(name).SendKeys(Keys.Tab);
            WaitTillSpinnerNotExist();
        }

        public void RemoveSearchText()
        {
            Log.Step(GetType().Name, "Remove values from the 'Search' textbox");
            Wait.UntilElementExists(SearchNameTextBox).ClearTextbox();
            Wait.UntilElementNotExist(ProgressBar);
        }

        public void SelectParentTeamDropDown(string radarType, bool isPopUp)
        {
            Log.Step(GetType().Name, "Select values for 'Work Type' dropdown");
            if (isPopUp)
            {
                ParentTeamDropDown = PrefixXPath(ParentTeamDropDown, SelectFacilitatorsPrefixText);
            }
            SelectItem(ParentTeamDropDown, DropDownItem(radarType));
        }

        public void SelectFirstNameCheckBox(List<string> facilitatorList)
        {
            Log.Step(GetType().Name, "Select Facilitator check box");
            foreach (var name in facilitatorList)
            {
                Wait.HardWait(3000); //wait till state change of checkbox
                Wait.UntilElementClickable(FirstNameCheckBox(name)).Check();
                Wait.HardWait(5000); //wait till state change of checkbox
            }
        }

        public void SelectAllFacilitatorsCheckBox(bool isPopUp)
        {
            Log.Step(GetType().Name, "Select all Facilitators checkbox");
            if (isPopUp)
            {
                AllFacilitatorsCheckBox = PrefixXPath(AllFacilitatorsCheckBox, SelectTeamsPrefixText);
            }
            Wait.UntilElementClickable(AllFacilitatorsCheckBox).Check();
        }

    }
}