using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.BusinessOutcomes;
using System.Globalization;
using System;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class StoriesTabPage : BaseTabPage
    {
        public StoriesTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Stories
        private readonly By ProgressBar = By.XPath("//p[text()='Checklist Progress']/parent::div/div/p");

        #endregion

        #region Methods

        public string GetStoriesOverallProgressInfo()
        {

            Log.Step(nameof(ObstaclesTabPage), "Click on 'Select Owners'");
            var progressText = Driver.JavaScriptScrollToElement(ProgressBar).GetText().Trim();

            // Remove the '%' character and try parsing
            progressText = progressText.Replace("%", "");

            return Math.Round(double.TryParse(progressText, out var progress) ? progress : 0,
                MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
        }


        #endregion
    }
}