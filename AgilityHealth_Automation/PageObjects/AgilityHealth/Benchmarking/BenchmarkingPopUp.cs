using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Benchmarking
{
    internal class BenchmarkingPopUp : BasePage
    {
        public BenchmarkingPopUp(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Benchmark Against
        private readonly By AgainstCompanyRadioButton = By.Id("myCompanyScope");
        private readonly By AgainstAgilityHealthRadioButton = By.Id("agilityhealthScope");

        //Benchmarking Option
        private readonly By BenchMarkingOptionDropdownArrow = By.CssSelector("span[aria-owns='BenchmarkingOption_listbox']");
        private By BenchMarkingOptionItem(string item) =>
            By.XPath($"//ul[@id='BenchmarkingOption_listbox']/li[text()='{item}'] | //ul[@id='BenchmarkingOption_listbox']/li//font[text()='{item}']");

        //Work Type
        private readonly By BenchMarkingWorkTypeDropdownArrow = By.CssSelector("span[aria-owns='BenchmarkingWorkType_listbox']");
        private By BenchMarkingWorkTypeItem(string item) =>
            By.XPath($"//ul[@id='BenchmarkingWorkType_listbox']/li[text()='{item}'] | //ul[@id='BenchmarkingWorkType_listbox']/li//font[text()='{item}']");


        //Roles
        private readonly By BenchMarkingRoleDropdownArrow = By.CssSelector("span[aria-owns='BenchmarkingRole_listbox']");
        private By BenchMarkingRoleItem(string item) =>
           By.XPath($"//ul[@id='BenchmarkingRole_listbox']/li[text()='{item}']");

        //Buttons
        private readonly By SelectButton = By.Id("selectBenchmarking");


        //Benchmark Against
        public void SelectCompanyRadioButton()
        {
            Wait.UntilElementClickable(AgainstCompanyRadioButton).Click();
        }

        public void SelectAgilityHealthRadioButton()
        {
            Log.Step(nameof(BenchmarkingPopUp), "Select Agility Health radio button");
            Wait.UntilElementClickable(AgainstAgilityHealthRadioButton).Click();
        }

        //Benchmarking Option
        public void SelectBenchMarkingOption(string option)
        {
            Log.Step(nameof(BenchmarkingPopUp), $"Select Bench Marking option <{option}>");
            SelectItem(BenchMarkingOptionDropdownArrow, BenchMarkingOptionItem(option));
        }

        //Role
        public void SelectBenchMarkingRole(string role)
        {
            SelectItem(BenchMarkingRoleDropdownArrow, BenchMarkingRoleItem(role));
        }

        //Work Type
        public void SelectBenchMarkingWorkType(string worktype)
        {
            Log.Step(nameof(BenchmarkingPopUp), $"Select Bench Marking work type <{worktype}>");
            SelectItem(BenchMarkingWorkTypeDropdownArrow, BenchMarkingWorkTypeItem(worktype));
        }

        //Button
        public void ClickSelectButton()
        {
            Log.Step(nameof(BenchmarkingPopUp), "Click on Select button");
            Wait.UntilElementExists(SelectButton);
            Wait.UntilElementClickable(SelectButton).Click();
            Wait.UntilJavaScriptReady();
        }
    }
}