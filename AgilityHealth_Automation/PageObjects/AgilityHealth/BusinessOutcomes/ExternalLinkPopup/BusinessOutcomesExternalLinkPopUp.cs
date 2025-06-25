using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.ExternalLinkPopup
{
    internal class BusinessOutcomesExternalLinkPopUp : BasePage
    {
        public BusinessOutcomesExternalLinkPopUp(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By TitleTextbox = By.Id("externallinks__titleField");
        private readonly By UrlTextbox = By.Id("externallinks__urlField");
        private readonly By AddButton = By.Id("externallinks__addLink");
        private readonly By SaveButton = By.Id("externallinks__saveBtn");
        private static By ExternalLinkValidationMessage(string fieldName) => By.XPath($"//label[@id='externallinks__{fieldName}Field-label']//parent::div//following-sibling::small//span");

        private static By ExternalLinkDeleteButton(string linkText) =>
            By.XPath($"//a[text() = '{linkText}']/following-sibling::button");

        // Methods
        public void AddExternalLink(BusinessOutcomesLinkRequest request)
        {
            Log.Step(nameof(BusinessOutcomesExternalLinkPopUp), $"Add External link with title <{request.Title}> and URL <{request.ExternalUrl}>");
            Wait.UntilElementClickable(TitleTextbox).SetText(request.Title);
            Wait.UntilElementClickable(UrlTextbox).SetText(request.ExternalUrl);
            Wait.UntilElementClickable(AddButton).Click();
            Wait.UntilJavaScriptReady();
            ClickOnSaveButton();
        }

        public void ClickOnAddButton()
        {
            Log.Step(nameof(BusinessOutcomesExternalLinkPopUp), "Click on 'Add' button on the External Link popup");
            Wait.UntilElementClickable(AddButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnSaveButton()
        {
            Log.Step(nameof(BusinessOutcomesExternalLinkPopUp), "Click on 'Save' button");
            Wait.UntilElementClickable(SaveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnDeleteButton(string linkText)
        {
            Log.Step(nameof(BusinessOutcomesExternalLinkPopUp), $"Click on Delete button with the title {linkText} on the External Link popup");
            Wait.UntilElementClickable(ExternalLinkDeleteButton(linkText)).Click();
        }

        public string GetValidationMessage(string fieldName)
        {
            return Wait.UntilElementVisible(ExternalLinkValidationMessage(fieldName)).GetText();
        }

    }
}