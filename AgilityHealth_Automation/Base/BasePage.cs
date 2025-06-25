using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;


namespace AgilityHealth_Automation.Base
{
    public class BasePage
    {
        protected IWebDriver Driver;
        protected SeleniumWait Wait;
        protected ILogger Log;

        public BasePage(IWebDriver driver, ILogger log = null)
        {
            Driver = driver;
            Wait = new SeleniumWait(driver);
            Log = log ?? new ConsoleLogger();
        }

        protected readonly By PendoDeploymentPopup = By.Id("pendo-guide-container");
        protected readonly By PendoDeploymentCloseButton = By.CssSelector("button._pendo-close-guide");
        private readonly By NewNavCustomIframe = By.Id("customFrame");

        /// <summary>
        /// Selects an Item in a Kendo dorp down list. Optional Parameter for can be passed to filter the list
        /// </summary>
        /// <param name="listLocator">Element that is clicked on to open the list</param>
        /// <param name="itemLocator">Element that is to be selected</param>
        /// <param name="searchText">filter the list. useful for long lists where the item is not visible. The 'listLocator' must be an input.</param>
        public void SelectItem(By listLocator, By itemLocator, string searchText = "")
        {
            var listBox = Wait.UntilElementClickable(listLocator);

            var found = false;
            var originalException = "";
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    listBox.Click();
                    Wait.UntilJavaScriptReady();
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        listBox.SendKeys(searchText);
                        Wait.UntilJavaScriptReady();
                    }
                    var item = Wait.UntilElementExists(itemLocator);
                    if (!item.Displayed)
                    {
                        Driver.JavaScriptScrollToElement(item); //When list items are present but hidden 
                    }
                    if (!item.Displayed) continue;
                    item.Click();
                    found = true;
                    break;
                }
                catch (System.Exception e)
                {
                    originalException = e.Message;
                }
            }

            if (!found)
            {
                throw new System.Exception($"Unable to select item in list: listbox locator = {listLocator}, item locator = {itemLocator}. \nInner Exception: {originalException}");
            }

            Wait.UntilJavaScriptReady();
        }

        public void CloseDeploymentPopup()
        {
            Log.Step(GetType().Name, "if the Deployment popup appears click on the close button");
            if (!Driver.IsElementDisplayed(PendoDeploymentPopup)) return;
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(PendoDeploymentCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SwitchToIframeForNewNav()
        {
            Wait.UntilJavaScriptReady();
            Driver.SwitchToFrame(NewNavCustomIframe);
            Wait.HardWait(5000); //Wait till iframe loads
        }

        public void RemovePendoHelp()
        {
            Driver.ExecuteJavaScript("var element = document.getElementById('launcher'); if (element) {element.remove()};");
        }

        public void NavigateToUrl(string url, By elementToWait = null)
        {
            Log.Step(GetType().Name, $"Navigating to URL: <{url}>");
            Driver.NavigateToPage(url);

            for (var i = 1; i < 5; i++)
            {
                if (Driver.GetCurrentUrl() != url)
                {
                    Driver.NavigateToPage(url);
                }
                else
                {
                    if (Driver.IsElementDisplayed(elementToWait, 10) || elementToWait == null)
                    {
                        break;
                    }
                    Driver.RefreshPage();
                }
            }
        }
    }
}
