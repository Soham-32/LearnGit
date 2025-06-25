using System.Text.RegularExpressions;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    internal class IaSelectIaTeamMembersPage : BasePage
    {
        public IaSelectIaTeamMembersPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By IaSelectTmIntro = By.ClassName("intro");
        private readonly By AllCheckbox = By.Id("selectall");
        private readonly By NextSelectOtherReviewers = By.CssSelector("input[type='submit']");

        public void SelectAllTeamMembers()
        {
            Log.Step(nameof(IaSelectIaTeamMembersPage), "Click on Select All Team Members checkbox");
            Wait.UntilElementClickable(AllCheckbox).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickNextSelectOtherReviewers()
        {
            Log.Step(nameof(IaSelectIaTeamMembersPage), "Click on Next, Select Other Reviewers button");
            Wait.UntilElementClickable(NextSelectOtherReviewers).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesIntroTextDisplayCorrectly(string expected)
        {
            var introText = Wait.UntilElementVisible(IaSelectTmIntro).GetText();
            introText = Regex.Replace(introText, @"\r\n?|\n", "");
            return expected.Trim().Equals(introText);
        }
    }
}