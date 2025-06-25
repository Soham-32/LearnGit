using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit
{
    public class EditRecipientsPage : SelectRecipientsBasePage
    {
        public EditHeaderPage Header { get; set; }
        public EditRecipientsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
            Header = new EditHeaderPage(driver, log);
        }

        //Locators
        public static By SurveyCompletedTagByTeamMember(string teamMemberName) => By.XPath(
            $"//p[@title='{teamMemberName}']//ancestor::div[@role='button']/following-sibling::div//*[local-name()='svg' and @data-testid='DoneIcon']/..");
        private static By TeamAssessmentStatus(string teamName) => By.XPath($"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//*[(@aria-label='Information') or (local-name()='svg' and @data-testid='DoneIcon')]/..");
        public static By ClusterEnvelopeIcon = By.XPath("//div[@role='region']//div//span[@aria-label='Send Reminder Email to Team']");
        public static By SurveyLinkIcon(string teamName, string teamMemberName) => By.XPath($"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//following-sibling::div//div[@title='{teamMemberName}']//following-sibling::div//button[@automation-id='linkBtn']");
        public static By IndividualEnvelopeIcon(string teamName, string teamMemberName) => By.XPath($"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//following-sibling::div//div[@title='{teamMemberName}']//following-sibling::div//span//button");
        public static By SendEmailToasterMessage = By.XPath("//*[text()='Reminder email has been sent.']");
        private readonly By StatusDropdown = AutomationId.Equals("statusFilter");
        private static By SelectStatusOption(string status) => By.XPath($"*//span[@automation-id='statusFilter']//div[contains(@id,'react-select') and contains(normalize-space(), '{status}')]");
        private static By InformationIcon(string teamName) => By.XPath(
            $"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//*[local-name()='svg' and @data-icon='circle-info']");
        private readonly By InformationIconTooltipMessage = By.XPath("//div[@role='tooltip']//div[text()][1] | //div[@role='tooltip']//div//font[text()][1]");
        private static By BulkEnvelopeButton(string teamName) => By.XPath(
            $"//span[contains(normalize-space(),'{teamName}')]//ancestor::div[@role='button']//span//*[local-name()='svg' and @data-icon='envelopes-bulk']");

        private readonly By BulkEnvelopIconForAllIncompleteTeams =
            By.XPath("//*[text()='EMAIL ALL INCOMPLETE TEAMS']//ancestor::span//span//button");
        private readonly By BulkEnvelopeIconTooltipMessage = By.XPath("//div[@role='tooltip']//div[text()] | //div[@role='tooltip']//div//font[text()]");
        private readonly By TeamCompletionCount = By.XPath("//div[@automation-id='searchTeamsBox']/parent::div/following-sibling::div");



        //Methods
        public void SelectStatusOptionFromStatusDropdown(string status)
        {
            Log.Info("Click on status dropdown and select option");
            SelectItem(StatusDropdown, SelectStatusOption(status));
        }

        public void ClickOnBulkEnvelopeIcon(string teamName)
        {
            Log.Info("Click on 'Bulk Envelope' icon");
            Wait.UntilElementClickable(BulkEnvelopeButton(teamName)).Click();
            Wait.HardWait(3000);
        }

        public bool IsBulKEnvelopeButtonDisplayed(string teamName)
        {
            return Driver.IsElementPresent(BulkEnvelopeButton(teamName));
        }

        public string GetTooltipMessageOfBulkEnvelope(string teamName)
        {
            Log.Info("Hover on the 'Bulk Envelope' icon");
            Driver.MoveToElement(Wait.UntilElementVisible(BulkEnvelopeButton(teamName)));
            Wait.HardWait(2000);// need to wait till message appears 
            return Wait.UntilElementVisible(BulkEnvelopeIconTooltipMessage).GetText();
        }

        public string GetTeamMemberCompletionText(string teamName)
        {
            Log.Info("Hover on the 'Information Icon'");
            Driver.MoveToElement(Wait.UntilElementVisible(InformationIcon(teamName)));
            return Wait.UntilElementVisible(InformationIconTooltipMessage).GetText();
        }

        public string GetAssessmentStatusOfTeam(string teamName)
        {
            Log.Step(nameof(EditRecipientsPage), "Get assessment status of team");
            return Wait.UntilElementVisible(TeamAssessmentStatus(teamName)).GetText();
        }

        public string GetSurveyCompletedTag(string teamName, string teamMemberName)
        {
            Log.Step(nameof(EditRecipientsPage), "Get the Survey completed tag");
            ExpandCollapseTeamAndTeamMember(teamName, teamMemberName);
            return Wait.UntilElementExists(SurveyCompletedTagByTeamMember(teamMemberName)).GetText();
        }

        public bool IsSurveyCompletedTagDisplayed(string teamName, string teamMemberName)
        {
            ExpandCollapseTeamAndTeamMember(teamName, teamMemberName);
            return Driver.IsElementDisplayed(SurveyCompletedTagByTeamMember(teamMemberName));
        }

        public void ClickOnClusterEnvelopeIcon()
        {
            Log.Step(nameof(EditRecipientsPage), "Click on 'Cluster Envelope' icon");
            Wait.UntilElementClickable(ClusterEnvelopeIcon).Click();
            Wait.UntilElementVisible(SendEmailToasterMessage);
        }

        public bool IsBatchEmailDisplayed()
        {
            return Driver.IsElementDisplayed(ClusterEnvelopeIcon);
        }

        public string ClickOnSurveyLinkIcon(string teamName, string teamMemberName)
        {
            Log.Step(nameof(EditRecipientsPage), "Click on 'Survey Link' icon");
            new Actions(Driver).SendKeys(Keys.Tab).Perform();
            Wait.UntilElementClickable(SurveyLinkIcon(teamName, teamMemberName)).Click();
            var link = CSharpHelpers.GetClipboard();
            return link;
        }

        public bool IsSurveyLinkIconPresent(string teamName, string teamMemberName)
        {
            return Driver.IsElementPresent(SurveyLinkIcon(teamName, teamMemberName));
        }

        public void ClickOnIndividualEnvelopeIcon(string teamName, string teamMemberName)
        {
            Log.Step(nameof(EditRecipientsPage), "Click on 'Individual Envelope' icon ");
            new Actions(Driver).SendKeys(Keys.Tab).Perform();
            Wait.UntilElementClickable(IndividualEnvelopeIcon(teamName, teamMemberName)).Click();
            Wait.UntilElementVisible(SendEmailToasterMessage);
        }

        public void ClickOnEmailAllInCompleteTeamsEnvelopeIcon()
        {
            Log.Step(nameof(EditRecipientsPage), "Click on 'Email All Incomplete Teams' icon ");
            Wait.UntilElementClickable(BulkEnvelopIconForAllIncompleteTeams).Click();
            Wait.UntilElementVisible(SendEmailToasterMessage);
        }

        public bool IsIndividualEnvelopePresent(string teamName, string teamMemberName)
        {
            return Driver.IsElementPresent(IndividualEnvelopeIcon(teamName, teamMemberName));
        }

        public bool IsEmailSentToasterMessageDisplayed()
        {
            return Driver.IsElementDisplayed(SendEmailToasterMessage, 18);
        }

        public string GetTeamCompletionCount()
        {
            var teamCompletionCount = Wait.UntilElementExists(TeamCompletionCount).GetText().Split(':', '(');
            return teamCompletionCount[1].RemoveWhitespace();
        }

        public string GetTeamCompletionPercentage()
        {
            var teamCompletionPercentage = Wait.UntilElementExists(TeamCompletionCount).GetText().Split('(', ')');
            return teamCompletionPercentage[1];
        }
    }
}
