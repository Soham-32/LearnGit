using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using AtCommon.Dtos.BusinessOutcomes.Custom;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class DeliverablesTabPage : BaseTabPage
    {
        public DeliverablesTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }


        private static By ParentOutcomeKeyResultsCheckbox(string keyResult) => By.XPath($"//*[text()='{keyResult}']//ancestor::td//preceding-sibling::td//input");
        private readonly By LinkParentOutcomeKrPopupLinkButton = By.XPath("//button//span[text()='Link'] | //button//span//font[text()='Link']");

        #region Initiatives

        private readonly By SelectChildCardButton = By.XPath("//*[text()='Select Child Cards']");
        private readonly By SelectChildCardSearch =
            By.XPath("//span[text()='Select Child Cards']/following-sibling::input");
        public readonly By ChildCardItems = By.XPath("//div[@data-rbd-droppable-id='childCards']//tbody//tr");

        public static By ChildCardItemTitle(int rowIndex) => By.XPath($"(//img[@alt='Open Parent Url']//parent::a/span)[{rowIndex}]");

        private static By DeliverableChildCardProgress(int rowIndex) => By.XPath($"(//div[@data-rbd-droppable-id='childCards']//div[contains(@role,'progressbar')])[{rowIndex}]");
        private readonly By DeliverableChildCardProgress1 = By.XPath("//div[@data-rbd-droppable-id='childCards']//tr[@class='k-table-row k-master-row']/td[10]//div//span");

        public readonly By DeliverableProgressPercentage = By.XPath("(//p[contains(normalize-space(),'Progress')]/parent::div/div/p)[2]");

        private readonly By DeliverableProgressIndication = By.XPath("//p[text()='Progress']/parent::div//div[@role='progressbar']/span");
        private readonly By TosterPopup = By.ClassName("Toastify__toast-body");

        #endregion
        public List<DeliverableTabChildCard> GetDeliverableChildCardResponse()
        {
            Log.Step(nameof(DeliverablesTabPage), "Get the Deliverable Child Card Response");
            var childCards = new List<DeliverableTabChildCard>();
            var rowIndex = Driver.IsElementPresent(ChildCardItems) ? Driver.GetElementCount(ChildCardItems) : 0;

            for (var i = 1; i <= rowIndex; i++)
            {
                var childCardTitle = Wait.UntilElementVisible(ChildCardItemTitle(i)).GetText();

                // Check if the first locator has the attribute "aria-valuenow"
                var progressElement = Driver.IsElementPresent(DeliverableChildCardProgress(i)) ?
                                      DeliverableChildCardProgress(i) : DeliverableChildCardProgress1;

                var progressIndicationElement = Wait.UntilElementExists(progressElement);
                string progressIndicationValue;

                if (progressIndicationElement.GetAttribute("aria-valuenow") != null)
                {
                    // If the element has the "aria-valuenow" attribute, use it
                    progressIndicationValue = progressIndicationElement.GetAttribute("aria-valuenow");
                }
                else
                {
                    // Fallback in case the attribute isn't present or the element is not found using the first locator
                    progressIndicationValue = progressIndicationElement.GetText().Replace("%", ""); // or handle this case as needed
                }

                childCards.Add(new DeliverableTabChildCard()
                {
                    Title = childCardTitle,
                    Progress = progressIndicationValue,
                });
            }

            return childCards;
        }

        public string GetDeliverableProgressPercentage()
        {
            return Wait.UntilElementVisible(DeliverableProgressPercentage).GetText();
        }

        public void SelectChildCards(List<string> childCards)
        {
            Log.Info("Select 'Child Card' from the Popup ");
            foreach (var childCard in childCards)
            {
                Wait.UntilElementExists(SelectChildCardSearch).SetText(childCard);
                Wait.HardWait(2000);// need to wait till load the data
                Wait.UntilElementClickable(ParentOutcomeKeyResultsCheckbox(childCard)).Click();
                Wait.UntilJavaScriptReady();
            }
            Wait.UntilElementClickable(LinkParentOutcomeKrPopupLinkButton).Click();
            Wait.HardWait(3000); // Need to wait till KRs are selected
            Wait.UntilElementNotExist(TosterPopup);
        }

        public string GetDeliverableProgressIndication()
        {
            return Wait.UntilElementVisible(DeliverableProgressIndication).GetElementAttribute("style");
        }

        public void ClickOnChildCardTitle()
        {
            Log.Info("Click on Child Card Title");
            var rowIndex = Driver.IsElementPresent(ChildCardItems) ? Driver.GetElementCount(ChildCardItems) : 0;
            Wait.UntilElementClickable(ChildCardItemTitle(rowIndex)).Click();
            Wait.HardWait(2000);   // Wait until all child card display
        }

        public void ClickOnSelectChildCardButton()
        {
            Log.Step(nameof(DeliverablesTabPage), "Click on Select child card button");
            Wait.UntilElementExists(SelectChildCardButton).Click();
        }

    }

}
