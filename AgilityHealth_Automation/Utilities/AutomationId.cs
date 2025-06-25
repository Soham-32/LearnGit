using OpenQA.Selenium;

namespace AgilityHealth_Automation.Utilities
{
    public static class AutomationId
    {
        public static By Equals(string automationId) => By.CssSelector($"[automation-id = '{automationId}']");
        public static By Equals(string automationId, string subElement) => By.CssSelector($"[automation-id = '{automationId}'] {subElement}");
        public static By StartsWith(string automationId) => By.CssSelector($"[automation-id^= '{automationId}']");
        public static By StartsWith(string automationId, string subElement) => By.CssSelector($"[automation-id^= '{automationId}'] {subElement}");
        public static By Contains(string automationId) => By.CssSelector($"[automation-id*= '{automationId}']");
        public static By EndsWith(string automationId) => By.CssSelector($"[automation-id$= '{automationId}']");
    }
}
