using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AngleSharp.Text;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class BaseTabPage : BasePage
    {
        public BaseTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        private readonly By SelectChildCardButton = By.XPath("//button[contains(text(),'Select Child Cards')]");
        private static By ActionKebabIcon(string title) => By.XPath($"//*[text()='{title}']//ancestor::tr//*[local-name()='svg' and @data-testid='MoreVertIcon']");

        private readonly By ActionDeleteButton = By.XPath("//li[@role='menuitem']");

        private readonly By RemoveButton = By.XPath("//button[text()='Remove']");


        #region Weight
        private readonly By WeightButton = By.Id("kr-weights-button");
        private readonly By WeightTableHeader = By.XPath("//table[@aria-label='Key results weights table']//*[contains(@class,'MuiTableCell-head')]");
        private readonly By ImpactIcon = By.XPath("//table[@aria-label='Key results weights table']//*[@data-icon='bullseye-arrow']");
        private readonly By WeightTableKeyResultTitles =
            By.XPath("//table[@aria-label='Key results weights table']//*[local-name()='svg']/parent::span//following-sibling::span");
        private readonly By WeightTableKeyResultDistributedWeightValues =
            By.XPath("//table[@aria-label='Key results weights table']//input[not(@aria-invalid='false')]");
        private readonly By WeightTableSubHeaderTooltip =
            By.XPath("//*[contains(@class,'info-circle')]//following-sibling::p");
        private readonly By WeightTableTotalWeightValue = By.XPath("//*[contains(text(),'Total Weight')]/parent::tr//input");
        private readonly By WeightTableTotalWeightText = By.XPath("//*[contains(text(),'Total Weight')]");
        private readonly By WeightTableCancelButton = By.XPath("//*[contains(text(),'Cancel')]");
        private readonly By WeightTableApplyButton = By.XPath("//*[contains(text(),'Apply')]");
        #endregion

        private static By ProjectOutcomeLink(string projectCardName) => By.XPath(
            $"//table//a//span[text()='{projectCardName}' or //font[text()='{projectCardName}']]");


        public void ClickOnSelectChildCardsButton()
        {
            Log.Step(nameof(BaseTabPage), "Click on Select child card button");
            Wait.UntilElementExists(SelectChildCardButton).Click();
        }

        public void ClickOnActionsKebabIcon(string title)
        {
            Log.Step(nameof(BaseTabPage), "Click on Actions Kebab Icon ");
            Wait.UntilElementClickable(ActionKebabIcon(title)).Click();
        }

        public void ClickOnRemoveButton()
        {
            Log.Step(nameof(BaseTabPage), "Click on Remove button ");
            Wait.UntilElementClickable(RemoveButton).Click();
        }

        public void ClickOnActionsDeleteOption()
        {
            Log.Step(nameof(BaseTabPage), "Click on Actions Delete button");
            Wait.UntilElementExists(ActionDeleteButton).Click();
        }

        public bool IsProjectOutcomeDisplayed(string projectOutcomeName)
        {
            return Wait.UntilElementExists(ProjectOutcomeLink(projectOutcomeName)).Displayed;
        }

        public void ClickOnProjectOutcomeNameLink(string projectOutcomeName)
        {
            Log.Step(nameof(BaseTabPage), "Click on project outcome name link");
            Wait.UntilElementClickable(ProjectOutcomeLink(projectOutcomeName)).Click();
        }
        #region Weight
        public bool IsWeightButtonDisplayed()
        {
            return Driver.IsElementDisplayed(WeightButton);
        }

        public bool IsWeightButtonEnabled()
        {
            return Driver.IsElementEnabled(WeightButton);
        }

        public void ClickOnWeightButton()
        {
            Log.Step(nameof(BaseTabPage), "Click on Weight button");
            Wait.UntilElementClickable(WeightButton).Click();
        }

        public List<string> GetWeightTableHeader()
        {
            return Driver.GetTextFromAllElements(WeightTableHeader).ToList();
        }

        public string GetWeightTooltipValue()
        {
            Log.Step(nameof(BaseTabPage), "Get the Weight Tooltip Value");
            return Wait.UntilElementVisible(WeightTableSubHeaderTooltip).GetText();
        }

        public List<KeyResult> GetKeyResultsWeightTableRow()
        {
            Log.Step(nameof(BaseTabPage), "Get Key Results Weight Table Values");
            var keyResults = new List<KeyResult>();

            // Find all key result titles
            var keyResultElements = Wait.UntilAllElementsLocated(WeightTableKeyResultTitles);

            // Find all Impact icons
            var impactIconElements = Wait.UntilAllElementsLocated(ImpactIcon);

            // Find all the weight values
            var weightElements = Driver.FindElements(WeightTableKeyResultDistributedWeightValues);

            // Ensure the sizes of keyResultElements, impactIconElements, and weightElements match
            if (keyResultElements.Count == impactIconElements.Count && keyResultElements.Count == weightElements.Count)
            {
                for (var i = 0; i < keyResultElements.Count; i++)
                {
                    Log.Step(nameof(BaseTabPage), $"Get the title of Key Results for row {i}th row");
                    var keyResultTitle = keyResultElements[i].Text;

                    Log.Step(nameof(BaseTabPage), $"Get the impact icon is displayed{i}th row");
                    var isImpactIconDisplayed = impactIconElements[i].Displayed;

                    Log.Step(nameof(BaseTabPage), $"Get the weight value for {i}th row");
                    var weightValue = weightElements[i].GetElementAttribute("value").ToDouble();  // You'll need to extract this correctly based on the HTML structure

                    // Add this string to the results list
                    keyResults.Add(new KeyResult
                    {
                        Title = keyResultTitle,
                        Weight = weightValue,
                        IsImpact = isImpactIconDisplayed
                    });
                }
            }
            // Return the list of actual results
            return keyResults;
        }

        public void ClickOnWeightTablePopupCancelButton()
        {
            Log.Step(nameof(BaseTabPage), "Click on Cancel Button");
            Wait.UntilElementClickable(WeightTableCancelButton).Click();
        }

        public void ClickOnApplyButton()
        {
            Log.Step(nameof(BaseTabPage), "Click on Apply Button");
            Wait.UntilElementClickable(WeightTableApplyButton).Click();
        }

        public bool IsWeightTableCancelButtonDisplayed()
        {
            Driver.JavaScriptScrollToElement(WeightTableCancelButton);
            return Driver.IsElementDisplayed(WeightTableCancelButton);
        }

        public bool IsWeightTableApplyButtonDisplayed()
        {
            Driver.JavaScriptScrollToElement(WeightTableApplyButton);
            return Driver.IsElementDisplayed(WeightTableApplyButton);
        }

        public bool IsTotalWeightTextDisplayed()
        {
            return Driver.IsElementDisplayed(WeightTableTotalWeightText);
        }

        public string GetTotalWeightValue()
        {
            return Wait.UntilElementVisible(WeightTableTotalWeightValue).GetText();
        }
        #endregion

    }
}