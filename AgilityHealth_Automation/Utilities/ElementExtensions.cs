using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AgilityHealth_Automation.Utilities
{
    public static class ElementExtensions
    {

        public static string GetText(this IWebElement element)
        {
            return element.TagName == "input" ? element.GetElementAttribute("value") : element.Text.Trim();
        }

        public static void Check(this IWebElement element, bool check = true)
        {
            if (check && !element.Selected || !check && element.Selected)
            {
                element.Click();
            }

        }
        /// <summary>
        /// Used for checkboxes that have 3 options: checked, unchecked, partially checked
        /// </summary>
        /// <param name="element">the input element checkbox</param>
        /// <param name="check">whether or not the checkbox should be checked or unchecked</param>
        public static void CheckIntermediate(this IWebElement element, bool check = true)
        {
            // determine if the checkbox is in the 'partial' state
            var partial = element.GetAttribute("data-indeterminate") == "true";
            // if you want it checked and it's not, or you don't want it checked and it is, or you don't 
            // want it checked and it's in the partial state
            if (check && !element.Selected || !check && element.Selected || !check && partial)
            {
                element.Click();
                // if you want it checked and it was in the partial state you must click it again
                if (check && partial) element.Click();
            }

        }


        public static void ClickOn(this IWebElement element, IWebDriver driver)
        {
            if (driver.IsInternetExplorer())
            {
                driver.JavaScriptClickOn(element);
            }
            else
            {
                element.Click();
            }
        }

        public static void DoubleClick(this IWebElement element, IWebDriver driver)
        {
            new Actions(driver).DoubleClick(element).Perform();
        }

        public static string GetElementAttribute(this IWebElement element, string attribute)
        {
            return element.GetAttribute(attribute).Trim();
        }

        public static IWebElement SetText(this IWebElement element, string text, bool clear = true, bool isReact = false, int time = 0)
        {
            if (clear && isReact)
            {
                element.ClearTextbox();
            }
            else if (clear)
            {
                element.Clear();
            }

            element.SendKeys(text);
            Thread.Sleep(TimeSpan.FromSeconds(time));
            return element;
        }


        //Dropdowns
        public static void SelectDropdownValueByVisibleText(this IWebElement element, string text)
        {
            new SelectElement(element).SelectByText(text);
        }

        public static DateTime? ToDateTime(this string text)
        {
            if (text == string.Empty) return null;
            return DateTime.Parse(text);
        }

        public static void ClearTextbox(this IWebElement element)
        {
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
        }

        public static void KeyUp(this IWebElement element)
        {
            element.SendKeys(Keys.Up);
        }
    }


}
