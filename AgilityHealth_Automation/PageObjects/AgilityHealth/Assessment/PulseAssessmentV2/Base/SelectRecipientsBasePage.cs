using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Base
{
    public class SelectRecipientsBasePage : BasePage
    {
        public SelectRecipientsBasePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators 
        //Headers
        private readonly By SelectRecipientsPageTitle = By.XPath("//button[@automation-id='stepNext']//parent::div//parent::div//preceding-sibling::div//p[contains(normalize-space(),'Select Recipients')]");

        //Team Locators
        private static readonly By SelectDeselectAllTeams = By.XPath("//span[contains(@automation-id, 'selectDeselectAll')]//input");

        public static By SearchTeamTextBox = By.XPath("//div[@automation-id='searchTeamsBox']//input");
        private readonly By LimitToTheseRolesDropDown =
            By.XPath("//span[@automation-id='companyRoles']//input[@aria-autocomplete='list']");

        private readonly By ListOfRoles = AutomationId.Equals("companyRoles", "div[tabindex]");
        private static By SelectRole(string role) => By.XPath($"//span[@automation-id='companyRoles']//*[text()='{role}']");
        private static By RemoveSelectedRole(string role) => By.XPath($"//span[@automation-id='companyRoles']//div[contains(normalize-space(),'{role}')]//following-sibling::div");

        public static By TeamCheckBox(string teamName) => By.XPath($"//span[text()='{teamName}']//preceding-sibling::span//input | //*[text()='{teamName}']//ancestor::span//preceding-sibling::span//input");
        public static By TeamExpandCollapseIcon(string teamName) => By.XPath($"//span[text()='{teamName}']//*[local-name()='svg'] | //span//font[text()='{teamName}']//../..//*[local-name()='svg']");

        public static By TeamMemberExpandCollapseIcon(string teamMemberName) => By.XPath($"//span/p[@title='{teamMemberName}']");
        public readonly By LoadingMembersText = By.XPath("//*[text()='Loading members...']");
        public static By TeamMemberDetailsRow(string teamMemberEmail) => By.XPath($"//div[@title='{teamMemberEmail}']/parent::div");

        private readonly By ListOfTeams = By.XPath("//div[@role='button']//span[2]");
        public static By ListOfTeamMembersEmailByTeamName(string teamName) => By.XPath(
            $"//*[text()='{teamName}']//ancestor::div[@role='button']//following-sibling::div//div[@automation-id='participantName']//following-sibling::div[1]");
        public static By ListOfTeamMembersRoleByTeamName(string teamName) => By.XPath(
            $"//*[text()='{teamName}']//ancestor::div[@role='button']//following-sibling::div//div[@automation-id='participantName']//following-sibling::div[2]");


        //Methods
        public bool IsSelectRecipientsTitleDisplayed()
        {
            return Driver.IsElementDisplayed(SelectRecipientsPageTitle);
        }

        public void SelectDeselectAllTeamsCheckBox(bool select = true)
        {
            Log.Step(nameof(SelectRecipientsPage), "Select/Deselect All Teams check box");
            Driver.JavaScriptScrollToElement(SelectDeselectAllTeams, false).Check(select);
            if (!Driver.IsElementSelected(SelectDeselectAllTeams))
            {
                Driver.JavaScriptScrollToElement(SelectDeselectAllTeams, false).Check(select);
            }
            Wait.HardWait(2000);// wait needed to load next button
        }

        public void SearchWithTeamName(string teamName)
        {
            Log.Step(nameof(SelectRecipientsPage), "Search team from text box");
            Wait.UntilElementVisible(SearchTeamTextBox).SetText(teamName);
            Wait.HardWait(3000);// need to wait till search items shows in grid
        }

        public void RemoveSearchedText()
        {
            Log.Step(nameof(SelectRecipientsPage), "Remove searched text from text box");
            Wait.UntilElementClickable(SearchTeamTextBox).ClearTextbox();
        }

        public void SelectDeselectRoleFromFilterDropDown(string role, bool isSelect = true)
        {
            if (isSelect)
            {
                Log.Step(nameof(SelectRecipientsPage), $"Select {role} from filter");
                SelectItem(LimitToTheseRolesDropDown, SelectRole(role));
                Wait.HardWait(2000);// need to wait till the selected role is added to the filter
                Wait.UntilElementExists(LimitToTheseRolesDropDown).SendKeys(Keys.Tab);
            }
            else
            {
                Log.Step(nameof(SelectRecipientsPage), $"Remove {role} from filter");
                Wait.UntilElementClickable(RemoveSelectedRole(role)).Click();
            }
        }

        public bool IsSelectedRoleDisplayed(string role)
        {
            return Driver.IsElementDisplayed(SelectRole(role));
        }

        public bool IsTeamDisplayed(string teamName)
        {
            return Driver.IsElementDisplayed(TeamExpandCollapseIcon(teamName));
        }

        public List<string> GetListOfRoles()
        {
            Log.Step(nameof(SelectRecipientsPage), "Get list of roles");
            return Wait.UntilAllElementsLocated(ListOfRoles).Select(a => a.GetText()).ToList();
        }


        public bool IsTeamSelected(string teamName)
        {
            return Driver.IsElementSelected(TeamCheckBox(teamName));
        }

        public void ClickOnSelectTeamCheckbox(string teamName, bool check = true)
        {
            Log.Step(nameof(SelectRecipientsPage), $"Click on '{teamName}' check box");
            Driver.JavaScriptScrollToElement(TeamCheckBox(teamName), false).Check(check);
            Wait.HardWait(2000);// Adding wait because it takes some time to get the status of check box
        }

        public List<string> GetListOfTeams()
        {
            Log.Step(nameof(SelectRecipientsPage), "Get list of teams");
            return Wait.UntilAllElementsLocated(ListOfTeams).Select(a => a.GetText()).ToList();
        }

        public void ClickOnTeamExpandIcon(string teamName)
        {
            Log.Step(nameof(SelectRecipientsPage), "Click on Team expand/collapse icon");
            if (Driver.JavaScriptScrollToElement(TeamExpandCollapseIcon(teamName)).GetAttribute("data-testid") == "KeyboardArrowDownIcon")
            {
                Wait.HardWait(2000); //Wait till teams appear.
                Wait.UntilElementClickable(TeamExpandCollapseIcon(teamName)).Click();
            }
            Wait.UntilElementNotExist(LoadingMembersText);
        }

        public bool IsTeamMemberEnabled(string teamName, string teamMemberName)
        {
            ClickOnTeamExpandIcon(teamName);
            return Wait.UntilElementExists(TeamMemberDetailsRow(teamMemberName)).GetAttribute("automation-id") == "enabled ";
        }

        public List<string> GetTeamMembersEmailByTeamNames(string teamName)
        {
            Log.Step(nameof(SelectRecipientsPage), "Get team member emails by team name");
            ClickOnTeamExpandIcon(teamName);
            Wait.HardWait(1000);// Taking sometime to get the teamMember emails
            return Wait.UntilAllElementsLocated(ListOfTeamMembersEmailByTeamName(teamName)).Select(a => a.GetAttribute("title"))
                .ToList();
        }

        public List<string> GetTeamMemberRoleByTeamName(string teamName)
        {
            Log.Step(nameof(SelectRecipientsPage), "Get team member roles by team name");
            ClickOnTeamExpandIcon(teamName);
            return Wait.UntilAllElementsLocated(ListOfTeamMembersRoleByTeamName(teamName)).Select(a => a.GetText())
                .ToList();
        }

        public void ExpandCollapseTeamAndTeamMember(string teamName, string teamMemberName)
        {
            Log.Step(nameof(SelectRecipientsPage), "Click on team/ team member to expand and collapse and get the Survey completed tag");
            ClickOnTeamExpandIcon(teamName);
            Wait.UntilElementClickable(TeamMemberExpandCollapseIcon(teamMemberName)).Click();
            Wait.HardWait(2000);// Taking sometime to expand the team member
        }

    }
}
