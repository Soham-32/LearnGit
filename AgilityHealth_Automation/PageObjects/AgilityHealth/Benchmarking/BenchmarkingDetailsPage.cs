using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Benchmarking
{
    internal class BenchmarkingDetailsPage : BasePage
    {
        public BenchmarkingDetailsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Title
        private readonly By Title = By.CssSelector("div.pg-title h1");

        //Legends
        private readonly By BaseTeamLegendText = By.CssSelector("#legend tbody tr td");
        private readonly By BenchmarkAgainstLegendText = By.Id("benchmarkLabel");


        //Title
        public string GetTitle()
        {
            return Wait.UntilElementVisible(Title).GetText();
        }
        
        //Legends
        public string GetBaseTeamLegendText()
        {
            return Wait.UntilElementVisible(BaseTeamLegendText).GetText();
        }

        public string GetBenchmarkAgainstLegendText()
        {
            return Wait.UntilElementVisible(BenchmarkAgainstLegendText).GetText();
        }
    }
}