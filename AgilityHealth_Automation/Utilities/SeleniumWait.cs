using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using System;
using System.Threading;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Utilities
{
    public class SeleniumWait
    {
        private readonly IWebDriver Driver;
        private readonly WebDriverWait Wait;
        public int TimeOut { get; set; }

        public SeleniumWait(IWebDriver driver, int timeOut = 60)
        {
            TimeOut = timeOut;
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
        }


        /// <summary>
        ///  Waits until an element can be found with the supplied locator. The element is not necessarily displayed.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementExists(By locator)
        {
            var element = Wait.Until(webDriver => webDriver.FindElement(locator));

            return element;
        }

        /// <summary>
        /// Waits until an element can't be located any longer
        /// </summary>
        /// <param name="locator">By locator used to find the element</param>
        /// <param name="timeOut">Amount of time to wait before throwing exception</param>
        public void UntilElementNotExist(By locator, int? timeOut = null)
        {
            timeOut ??= TimeOut;
            new WebDriverWait(Driver, TimeSpan.FromSeconds(timeOut.Value))
                .Until(d => d.FindElements(locator).Count == 0);
            Thread.Sleep(500);
        }

        /// <summary>
        /// Waits until the element is displayed and enabled
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementClickable(IWebElement element)
        {
            Wait.Until(d =>
            {
                try
                {
                    return element.Displayed && element.Enabled;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            Thread.Sleep(500);

            return element;
        }

        /// <summary>
        /// Waits until the element can be located with the supplied locator, and that the element is displayed and enabled.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementClickable(By locator)
        {
            var element = Wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            return element;
        }

        /// <summary>
        /// Locate all the elements that match the locator and return them in a list
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeOut"></param>
        /// <returns>IList of elements that were located</returns>
        public IList<IWebElement> UntilAllElementsLocated(By locator, int timeOut = 60)
        {
            return new WebDriverWait(Driver, TimeSpan.FromSeconds(timeOut)).Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(locator));
        }

        /// <summary>
        /// Waits until an element can be located with the supplied locator, and that the element is displayed.
        /// </summary>
        /// <param name="by"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementVisible(By by)
        {

            var displayedElement = Wait.Until(d =>
            {
                try
                {
                    var element = Driver.FindElement(by);
                    return element.Displayed ? element : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
            HardWait(500);

            return displayedElement;
        }

        /// <summary>
        /// Waits until the element is displayed
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementVisible(IWebElement element)
        {
            Wait.Until(d =>
            {
                try
                {
                    return element.Displayed;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            HardWait(500);

            return element;
        }

        /// <summary>
        /// Waits until the element is no longer displayed
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>The located element</returns>
        public IWebElement UntilElementInvisible(By locator)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return !e.Displayed ? e : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Waits until the element is no longer displayed
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>returns true if the element is not displayed</returns>
        //public bool UntilElementInvisible(By locator)
        //{
        //    return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        //}

        public IWebElement UntilElementEnabled(By locator)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return e.Enabled ? e : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Waits until JQuery has loaded and JavaScript ready state is 'complete'
        /// </summary>
        public void UntilJavaScriptReady(TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(TimeOut);

            HardWait(500);
            try
            {
                if (Driver.HasJquery() && !Driver.IsJqueryReady())
                {
                    var driverWait = new WebDriverWait(Driver, timeout.Value);
                    driverWait.Until((d) => Driver.IsJqueryReady());
                }

                if (!Driver.IsJsReadyStateComplete())
                {
                    var driverWait = new WebDriverWait(Driver, timeout.Value);
                    driverWait.Until((d) => Driver.IsJsReadyStateComplete());
                }
            }
            catch
            {
                //At times, on IE 11 ,with no reason IsJqueryReady,IsJSReadyStateComplete calls gets timeout or getting : 'jQuery' is undefined message . To avoid that.
            }

            HardWait(500);
        }

        /// <summary>
        /// This waits for a specific amount of time. This should be used only if necessary.
        /// </summary>
        /// <param name="milliseconds"></param>
        public void HardWait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Waits until an element is found with the by given locator. Returns the element if found.
        /// If the element is not found, it returns null instead of an exception. Should be used to handle if an element may come up and need to be delat with.
        /// </summary>
        /// <param name="byLocator"></param>
        /// <param name="timeout"></param>
        /// <returns>The located element or null if the element was not found</returns>
        public IWebElement InCase(By byLocator, int timeout = 5)
        {
            IWebElement element = null;
            var waitInCase = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            try
            {
                element = waitInCase.Until((d) =>
                {
                    var elements = d.FindElements(byLocator);
                    return elements.Count > 0 ? elements[0] : null;
                });
            }
            catch (Exception)
            {
                // ignored
            }

            return element;
        }
        /// <summary>
        /// returns a list of elements. If the specified timeout has been reached and no elements found,
        /// an empty list will be returned.
        /// </summary>
        /// <param name="byLocator"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IList<IWebElement> InCases(By byLocator, int timeout = 5)
        {
            try
            {
                return new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout))
                    .Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(byLocator));
            }
            catch (Exception)
            {
                return new List<IWebElement>();
            }
        }

        public IWebElement UntilCssValueEquals(By locator, string cssProperty, string cssValue)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return e.GetCssValue(cssProperty) == cssValue ? e : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public IWebElement UntilAttributeEquals(By locator, string attributeName, string attributeValue)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return e.GetAttribute(attributeName) == attributeValue ? e : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Locates an element and waits for the specified attribute to either contain or not contain the specified value.
        /// </summary>
        /// <remarks>You can invert the condition by passing contains = false, so that it waits for the attribute to NOT contain the value.</remarks>
        /// <returns>The element that was located</returns>
        public IWebElement UntilAttributeContains(By locator, string attributeName, string attributeValue, bool contains = true)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return e.GetAttribute(attributeName).Contains(attributeValue) == contains ? e : null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public IWebElement UntilAttributeNotEquals(By locator, string attributeName, string attributeValue)
        {
            return Wait.Until(d =>
            {
                try
                {
                    var e = d.FindElement(locator);
                    return e.GetAttribute(attributeName) == attributeValue ? null : e;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public void UntilLoadingDone()
        {
            UntilElementInvisible(By.Id("loading"));
        }

        public IWebElement ForSubElement(IWebElement parentElement, By subElementLocator)
        {
            return Wait.Until(d =>
            {
                try
                {
                    return parentElement.FindElement(subElementLocator);
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public void UntilTextToBePresent(IWebElement element, string expectedText)
        {
            Wait.Until(ExpectedConditions.TextToBePresentInElement(element, expectedText));
        }
    }
}