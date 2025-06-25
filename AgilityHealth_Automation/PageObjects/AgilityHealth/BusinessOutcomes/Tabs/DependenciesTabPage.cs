using System;
using System.Collections.Generic;
using System.Globalization;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class DependenciesTabPage : BaseTabPage
    {
        public DependenciesTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        #region Dependenies Tab

        private readonly By DependencyAddButton = By.XPath("//button[contains(text(),'Add A Dependency')]//*[local-name()='svg' and @data-testid='AddCircleIcon']");
        private readonly By SelectDependencyCardTextBox = By.XPath("//textarea[@placeholder='Select Dependent Card']");
        private static By DependencyCardTitle(int rowIndex) => By.XPath($"(//textarea[@placeholder='Select Dependent Card'])[{rowIndex}]");
        private static By DependencyCardOwnerName(int rowIndex) => By.XPath($"(//textarea[@placeholder='Select Dependent Card'])[{rowIndex}]//ancestor::tr//td[7]");
        private static By DynamicCardTitleName(string cardTitle) => By.XPath($"//li[text()='{cardTitle}']");
        private readonly By GetDependencyListCount = By.XPath("//div[@data-rbd-droppable-id='dependencies']//tr[@role='button']");
        private readonly By DependencyProgressPercentage = By.XPath("//p[text()='Dependency Progress']/parent::div/div/p");

        #endregion

        //Methods

        #region Dependencies Tab

        public void ClickOnDependencyAddButton()
        {
            Log.Step(nameof(DependenciesTabPage), "Click on 'Add Dependencies' button");
            Driver.JavaScriptScrollToElement(DependencyAddButton).Click();
        }

        public void AddDependencyCard(string cardTitle)
        {
            Log.Step(nameof(DependenciesTabPage), $"Add a dependency card <{cardTitle}>");
            Wait.UntilElementClickable(SelectDependencyCardTextBox).Click();
            Wait.HardWait(2000); //Wait until dependency column loaded
            Wait.UntilElementVisible(SelectDependencyCardTextBox).SetText(cardTitle);
            if (!Driver.IsElementDisplayed(DependencyCardTitle(1)))
            {
                Wait.UntilElementClickable(SelectDependencyCardTextBox).Click(); 
                Wait.UntilElementVisible(SelectDependencyCardTextBox).SetText(cardTitle);
            }
            Wait.UntilElementVisible(DynamicCardTitleName(cardTitle)).Click();
        }

        public List<string> GetDependencyInfo()
        {
            Log.Step(nameof(CheckListTabPage), "Get the created Dependency response");
            var dependencyListCount = Wait.UntilAllElementsLocated(GetDependencyListCount).Count;
            var dependencyList = new List<string>();

            for (var i = 1; i <= dependencyListCount; i++)
            {
                Driver.JavaScriptScrollToElement(DependencyCardTitle(i));
                var dependencyTitle = Wait.UntilElementVisible(DependencyCardTitle(i)).GetText();
                var owner = Wait.UntilElementExists(DependencyCardOwnerName(i)).GetText();
                dependencyList.Add($"{dependencyTitle} - {owner}");
            }

            return dependencyList;
        }

        public string GetDependencyProgressPercentage()
        {
            var progressText = Driver.JavaScriptScrollToElement(DependencyProgressPercentage).Text.Replace("%", "").Trim();

            return Math.Round(double.TryParse(progressText, out var progress) ? progress : 0,
                MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }


}
