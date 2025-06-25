using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.OrgStructure
{
    public class OrgStructureDashboardPage : BasePage
    {
        public OrgStructureDashboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        
        private readonly By ExportToPdfButton = By.Id("pdfImage");
        private readonly By CreatePdfButton = By.Id("pdf_submit");

        public void ExportToPdf()
        {
            Wait.UntilElementClickable(ExportToPdfButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(CreatePdfButton).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}
