using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams
{
    internal class ImportTeamsPopup : BasePage
    {
        public ImportTeamsPopup(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Locators
        private readonly By FileInput = By.Id("files");
        private readonly By ImportStatusResult = By.Id("statusResult");
        private readonly By ImportPopupIframe = By.XPath("//iframe[@title = 'Import Teams']");
        private readonly By UploadDone = By.CssSelector(".k-upload-status .k-i-tick");
        private readonly By CloseButton = By.Id("closeBulkImportCompleteWindow");
            
        // Methods
        public void UploadFile(string filePath)
        {
            Log.Step(nameof(ImportTeamsPopup), $"Uploading file at <{filePath}>");
            Wait.UntilJavaScriptReady();
            Driver.SwitchToFrame(ImportPopupIframe);
            Wait.UntilElementExists(FileInput).SetText(filePath);
            Wait.UntilElementExists(UploadDone);
            Wait.UntilElementVisible(CloseButton);
            Driver.SwitchTo().DefaultContent();
        }

        public string GetStatusResult()
        {
            Wait.UntilJavaScriptReady();
            Driver.SwitchToFrame(ImportPopupIframe);
            var result = Wait.UntilElementExists(ImportStatusResult).GetText();
            Driver.SwitchTo().DefaultContent();
            return result;
        }

        public void ClickCloseButton()
        {
            Log.Step(nameof(ImportTeamsPopup), "Click on the 'Close' button");
            Wait.UntilJavaScriptReady();
            Driver.SwitchToFrame(ImportPopupIframe);
            Wait.UntilElementClickable(CloseButton).Click();
            Driver.SwitchTo().DefaultContent();
        }
    }
}