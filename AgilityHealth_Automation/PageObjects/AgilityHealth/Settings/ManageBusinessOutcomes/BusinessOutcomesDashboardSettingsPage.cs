using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes
{
    internal class BusinessOutcomesDashboardSettingsPage : BusinessOutcomesSettingsPage
    {
        public BusinessOutcomesDashboardSettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Card Type

        private static By OutcomeEditLabelLink(string label) => By.XPath($"//p[contains(text(),'{label}')]//following-sibling::a");
        private static By InitiativeDeliverableEditLabelLink(string label) => By.XPath($"//div[text()='{label}']//following-sibling::a");
        private static By EditLabelTextbox(string label) => By.XPath($"//input[contains(@value,'{label}')]");
        private readonly By SaveLabel = By.XPath("//*[local-name()='svg' and @data-testid='CheckIcon']");
        private readonly By OutcomeLabelNames = By.XPath("//p[text()='Outcomes']/..//div/p");
        private static By InitiativesDeliverableLabelNames(string cardType) =>
            By.XPath($"//p[text()='Card Type']//following::div//div[text()='{cardType}']");

        #endregion

        #region Toggles

        private readonly By DisplayAtEveryLevelToggle = By.XPath("//input[@id = 'DisplayCompanyLevelOutcomes']/..");
        private readonly By BusinessOutcomeFilterToggle = By.XPath("//input[@id = 'FilterDataEitherByEitherAndOrOr']/..");
        private readonly By DashboardSettingsHeading = By.XPath("//h2[text() = 'Dashboard Settings'] | //h2//font[text() = 'Dashboard Settings']");

        #endregion

        #endregion

        #region Methods

        // Methods
        public void DisplayOnEveryLevel(bool on)
        {
            Log.Info("Turn display level outcomes on every level toggle On/Off");
            var toggle = Wait.UntilElementClickable(DisplayAtEveryLevelToggle);
            var isChecked = toggle.GetAttribute("class").Contains("Mui-checked");
            if (on && isChecked || !on && !isChecked) return;

            toggle.Click();
            ClickOnSaveButton();
        }

        public void BusinessOutcomeFilterOrAndToggle(bool and = true)
        {
            Log.Info("Turn business outcome filter toggle 'AND/OR'");
            var toggle = Wait.UntilElementClickable(BusinessOutcomeFilterToggle);
            var isChecked = toggle.GetAttribute("class").Contains("Mui-checked");
            if (and && isChecked || !and && !isChecked) return;

            toggle.Click();
            ClickOnSaveButton();
        }

        public bool IsDashboardSettingsHeadingDisplayed()
        {
            return Driver.IsElementDisplayed(DashboardSettingsHeading);
        }

        public void UpdateOutcomesLabel(string existingLabel, string newLabel)
        {
            Log.Info("Click on edit label and update with new label name");
            Wait.UntilElementExists(OutcomeEditLabelLink(existingLabel)).Click();
            Wait.UntilElementExists(EditLabelTextbox(existingLabel)).SetText(newLabel);
            Wait.UntilElementExists(SaveLabel).Click();
        }

        public List<string> GetListOfOutcomesLabels()
        {
            Log.Info("Get the list of outcome labels");
            var a = Wait.UntilAllElementsLocated(OutcomeLabelNames).Select(a => a.GetText()).ToList();
            return a;
        }

        public void UpdateInitiativesAndDeliverableLabels(string existingLabel, string newLabel)
        {
            Log.Info("Click on edit label of initiatives and deliverable, update them with new label name");
            Wait.UntilElementExists(InitiativeDeliverableEditLabelLink(existingLabel)).Click();
            Wait.UntilElementExists(EditLabelTextbox(existingLabel)).SetText(newLabel);
            Wait.UntilElementExists(SaveLabel).Click();
            Wait.HardWait(1000);
        }

        public bool IsCardTypePresent(string cardType)
        {
            return Driver.IsElementPresent(InitiativesDeliverableLabelNames(cardType));
        }

        #endregion
    }
}