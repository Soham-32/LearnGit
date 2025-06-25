using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class AdditionalDetailsPage : BaseTabPage
    {
        public AdditionalDetailsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By SelectedTags = By.XPath("//div[@id='businessoutcometags']//span[text() or //font//font[text()]]");
        private static By RemoveTagIcon(string tag) => By.XPath($"//div[@id='businessoutcometags']//span[(text()='{tag}' or font//font[text()='{tag}'])]//following-sibling::*[local-name()='svg']");
        private readonly By RemoveTagToasterMessage = By.XPath("//div[text()= 'The last tag cannot be deleted. Please ensure that at least one tag is retained for this card.']");

        //Methods
        public void RemoveSelectedTag(List<string> tags)
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Remove tags from the card");
            foreach (var tag in tags)
            {
                Wait.UntilElementVisible(RemoveTagIcon(tag)).Click();
            }
            Wait.HardWait(2000);// need to wait till toaster message comes
        }

        public string GetLastTagCannotBeDeletedToasterMessage()
        {
            return Wait.UntilElementExists(RemoveTagToasterMessage).GetText();
        }
        public List<string> GetSelectedTag()
        {
            Log.Step(nameof(BusinessOutcomeCardPage), "Get selected tags from the card");
            return Wait.UntilAllElementsLocated(SelectedTags).Select(a => a.Text).ToList();
        }
    }
}
