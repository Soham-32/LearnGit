using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base
{
    public class RadarSelectionBase : BasePage
    {
        public RadarSelectionBase(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        protected readonly By RadarSelectionHeaderText = By.XPath("//form//p[text()='Radar Selection'] | //form//p//font[text()='Radar Selection']");

        protected static By RadarCheckbox(string radarName) => By.XPath($"//label/span[text() = '{radarName}']/parent::label//input");

        public void SelectRadar(string radarName)
        {
            Log.Step(GetType().Name, $"Check radar type <{radarName}> checkbox");
            Wait.UntilElementEnabled(RadarCheckbox(radarName)).Check();
        }

        public void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(RadarSelectionHeaderText);
        }

        public bool IsRadarSelectionTextVisible()
        {
            return Driver.GetElementCount(RadarSelectionHeaderText) > 0 && Wait.UntilElementExists(RadarSelectionHeaderText).Displayed;
        }

        public bool IsRadarChecked(string radarName)
        {
            var element = Wait.UntilElementClickable(By.XPath($"//span[text() = '{radarName}']/preceding-sibling::span"));

            return element.GetElementAttribute("class").Contains("Mui-checked");
        }

        public bool IsRadarPresent(string radarName)
        {
            return Driver.IsElementPresent(RadarCheckbox(radarName));
        }
    }
}