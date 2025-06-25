using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using static System.Threading.Thread;

namespace AgilityHealth_Automation.Utilities
{
    public static class DriverExtensions
    {
        public static void RefreshPage(this IWebDriver driver)
        {
            try
            {
                driver.Navigate().Refresh();
            }
            catch //If there is a WebDriverException, attempt refreshing page again.
            {
                driver.Navigate().Refresh();
            }
        }

        public static void Back(this IWebDriver driver)
        {
            driver.Navigate().Back();
        }

        public static void BackAndWait(this IWebDriver driver)
        {
            driver.Navigate().Back();
            SeleniumWait wait = new SeleniumWait(driver);
            wait.UntilJavaScriptReady();
        }

        public static void NavigateToPage(this IWebDriver driver, String url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
            }
            catch
            {
                if (!driver.IsInternetExplorer()) 
                {
                    driver.Navigate().GoToUrl(url);
                }
            }
        }

        public static string GetCurrentUrl(this IWebDriver driver)
        {
            return driver.Url;
        }
        public static void ClickOnEscFromKeyboard(this IWebDriver driver)
        {
            var actions = new Actions(driver);
            actions.SendKeys(Keys.Escape).Perform();
        }

        public static void TakeScreenShot(this IWebDriver driver, string filepath)
        {
            var ssDriver = (ITakesScreenshot)driver;
            if (ssDriver == null) return;
            var screenshots = ssDriver.GetScreenshot();
            screenshots.SaveAsFile(filepath);
        }

        public static IWebDriver SwitchToFrame(this IWebDriver driver, By by)
        {
            driver.SwitchTo().Frame(driver.FindElement(by));
            return driver;
        }

        public static IWebDriver SwitchToFrame(this IWebDriver driver, IWebElement element)
        {
            driver.SwitchTo().Frame(element);
            return driver;
        }
        public static void SwitchToDefaultIframe(this IWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
        }

        public static IWebDriver SelectWindowByTitle(this IWebDriver driver, string title)
        {
            foreach (var item in driver.WindowHandles.Where(item => driver.SwitchTo().Window(item).Title.Equals(title)))
            {
                driver.SwitchTo().Window(item);
                break;
            }

            return driver;
        }

        public static void SwitchToLastWindow(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }

        public static void SwitchToFirstWindow(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.First());
        }

        public static void SelectWindowByIndex(this IWebDriver driver, int index)
        {
            var windows = driver.WindowHandles;
            driver.SwitchTo().Window(windows[index]);
        }



        public static int GetElementCount(this IWebDriver driver, By byLocator)
        {
            return driver.FindElements(byLocator).Count;
        }

        public static bool IsElementPresent(this IWebDriver driver, By byLocator, int waitTimeInSeconds = 5)
        {
            return new SeleniumWait(driver).InCase(byLocator, waitTimeInSeconds) != null;
        }

        public static bool IsChildElementPresent(this IWebDriver driver, IWebElement parentElement, By childLocator, int waitTimeInSeconds=1)
        {
            new SeleniumWait(driver).HardWait(waitTimeInSeconds * 1000);

            return parentElement.FindElements(childLocator).Count > 0;
        }

        //Javascript methods
        public static void ExecuteJavaScript(this IWebDriver driver, String javaScript)
        {
            driver.JsExecutor().ExecuteScript(javaScript);
        }

        public static void ExecuteJavaScript(this IWebDriver driver, string javaScript, params Object[] args)
        {
            driver.JsExecutor().ExecuteScript(javaScript, args);
        }

        public static void JavaScriptClickOn(this IWebDriver driver, IWebElement element)
        {
            driver.JsExecutor().ExecuteScript("arguments[0].click();", element);
        }

        public static void JavaScriptScroll(this IWebDriver driver, string horizontal, string vertical)
        {
            string script = "scroll(" + horizontal + "," + vertical + ")";
            driver.JsExecutor().ExecuteScript(script);
        }

        public static IWebElement JavaScriptScrollToElement(this IWebDriver driver, IWebElement element, bool top = true)
        {
            driver.JsExecutor().ExecuteScript($"arguments[0].scrollIntoView({top.ToString().ToLower()});", element);

            return element;
        }

        public static IWebElement JavaScriptScrollToElement(this IWebDriver driver, By locator, bool top = true)
        {
            var element = new SeleniumWait(driver).UntilElementExists(locator);
            driver.JsExecutor().ExecuteScript($"arguments[0].scrollIntoView({top.ToString().ToLower()});", element);

            return element;
        }

        public static IWebElement JavaScriptScrollToElementCenter(this IWebDriver driver, By locator)
        {
            var element = new SeleniumWait(driver).UntilElementExists(locator);
            driver.JsExecutor().ExecuteScript("arguments[0].scrollIntoView({behavior: 'auto', block: 'center', inline: 'center'});", element);

            return element;
        }

        public static void JavaScriptSetAttribute(this IWebDriver driver, IWebElement element,string name, string value)
        {
            driver.JsExecutor().ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", element, name, value);
        }

        public static void JavaScriptSetValue(this IWebDriver driver, IWebElement element,string value)
        {
            var script = $"arguments[0].value='{value}';";
            driver.JsExecutor().ExecuteScript(script, element);
        }

        public static IJavaScriptExecutor JsExecutor(this IWebDriver driver)
        {
            return driver as IJavaScriptExecutor;
        }

        public static bool IsJqueryReady(this IWebDriver driver)
        {
            bool ready = (bool) driver.JsExecutor().ExecuteScript("return jQuery.active==0");
            return ready;
        }

        public static bool HasJquery(this IWebDriver driver)
        {
            bool jqueryDefined = (bool) driver.JsExecutor().ExecuteScript("return typeof jQuery != 'undefined'");
            return jqueryDefined;
        }

        public static bool IsJsReadyStateComplete(this IWebDriver driver)
        {
            bool ready = false;
            try
            {
                ready = driver.JsExecutor().ExecuteScript("return document.readyState").ToString().Equals("complete");
            }
            catch (WebDriverException)
            {
            }
            return ready;
        }

        public static void StopBrowserPageLoad(this IWebDriver driver)
        {
            ExecuteJavaScript(driver, "return window.stop");
        }

        //Alert methods
        public static void AcceptAlert(this IWebDriver driver)
        {
            IAlert alert = driver.SwitchTo().Alert();
            alert.Accept();
        }

        public static string GetAlertMessage(this IWebDriver driver)
        {
            return driver.SwitchTo().Alert().Text;
        }


        //Drag and drop the object
        public static IWebDriver DragAndDrop(this IWebDriver driver, IWebElement element, int x, int y)
        {
            Actions builder = new Actions(driver);
            builder.DragAndDropToOffset(element, x, y);

            return driver;
        }

        public static IWebDriver DragElementToElement(this IWebDriver driver, IWebElement fromElement,
            IWebElement toElement)
        {
            var builder = new Actions(driver);
            builder.ClickAndHold(fromElement).MoveToElement(toElement).Release().Build().Perform();

            return driver;
        }

        public static void DragElementToElement(this IWebDriver driver, IWebElement fromElement,
            IWebElement toElement, int x, int y)
        {
            var builder = new Actions(driver);
            builder.ClickAndHold(fromElement).MoveToElement(toElement).MoveByOffset(x, y).Release().Build().Perform();

            Sleep(1000);
        }

        public static void DragElementToElement(this IWebDriver driver, By fromElement, By toElement, int x = 0, int y = 0)
        {
            var wait = new SeleniumWait(driver);
            var fromEl = wait.UntilElementExists(fromElement);
            var toEl = wait.UntilElementExists(toElement);
            driver.DragElementToElement(fromEl, toEl, x, y);
        }

        public static IWebElement MoveToElement(this IWebDriver driver, IWebElement element)
        {
            Actions action = new Actions(driver);
            action.MoveToElement(element).Perform();

            return element;
        }

        public static IWebElement MoveToElement(this IWebDriver driver, By locator)
        {
            return driver.MoveToElement(new SeleniumWait(driver).UntilElementExists(locator));
        }

        //Move Slider
        public static void Moveslider(this IWebDriver driver, IWebElement widget, int x, int y)
        {
            Actions actions = new Actions(driver);
            IAction action = actions.ClickAndHold(widget).MoveByOffset(x, y).Release().Build();
            action.Perform();
        }

        public static IList<string> GetTextFromAllElements(this IWebDriver driver, By locator)
        {
            IList<IWebElement> elements = driver.FindElements(locator);

            return elements.Select(e => e.GetText()).ToList();
            
        }
        public static bool IsInternetExplorer(this IWebDriver driver)
        {
            return driver.GetType() == typeof(InternetExplorerDriver);
        }

        public static bool IsChrome(this IWebDriver driver)
        {
            return driver.GetType() == typeof(ChromeDriver);
        }

        public static bool IsFirefox(this IWebDriver driver)
        {
            return driver.GetType() == typeof(FirefoxDriver);
        }

        public static IList<string> GetAttributeFromAllElements(this IWebDriver driver, By locator, string attribute)
        {
            IList<IWebElement> elements = driver.FindElements(locator);

            return (from element in elements select element.GetAttribute(attribute)).ToList();

        }

        public static bool IsElementDisplayed(this IWebDriver driver, By locator, int timeout = 5)
        {
            return new SeleniumWait(driver).InCase(locator, timeout)?.Displayed ?? false;
        }

        public static bool IsElementSelected(this IWebDriver driver, By locator, int timeout = 5)
        {
            return new SeleniumWait(driver).InCase(locator, timeout)?.Selected ?? false;
        }

        public static bool IsElementEnabled(this IWebDriver driver, By locator, int timeOut = 5)
        {
            TimeSpan to = TimeSpan.FromSeconds(timeOut);
            try
            {
                new WebDriverWait(driver, to).Until(d =>
                {
                    var e = d.FindElement(locator);
                    return e.Enabled ? e : null;
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}