using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration
{
    public class LinkTeamBasePage : BasePage
    {
        public LinkTeamBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Link Team page 
        private readonly By LinkTeamPageTitleText = By.XPath("//h2[text() = 'Link Team']");

        //Jira Cloud Credentials
        private readonly By AuthenticationPopupTitleText = By.Id("alertDialogTitle");
        private readonly By InstanceNameTextBox = By.Id("instanceName");
        private readonly By InstanceNameValidationText = By.Id("instanceName-helper-text");
        private readonly By UrlTextBox = By.Id("url");
        private readonly By UrlValidationText = By.Id("url-helper-text");
        private readonly By AuthenticationPopupOkButton = By.XPath("//button[text()='Ok']");

        //Loading indicator
        private readonly By LoadingSpinner = By.XPath("//p[contains(text(), 'Loading')]//img[contains(@src, 'load') and contains(@src, '.gif')]");

        //Link Team page title
        private readonly By LinkTeamTitleText = By.XPath("//h2[text() = 'Link Team']");

        //Jira Instance dropdown                
        private readonly By JiraInstanceArrowDropdownIcon = By.Id("instance-select");
        private readonly By JiraInstanceNameList = By.XPath("//ul[@role = 'listbox']//li[not(text()='Select Instance')]");
        //Add new instance
        private readonly By AddNewInstancePlusIcon = By.XPath("//*[local-name()='svg' and @data-icon = 'plus-circle']");

        //Link Teams to Jira Boards
        //Jira Project dropdown
        private readonly By JiraProjectDropdownIcon = By.Id("project-autocomplete");
        private static By JiraProjectDropdownValue(string projectName) =>
            By.XPath($"//ul[@id='project-autocomplete-listbox']//li[text()='{projectName}']");

        private readonly By TeamDropDown = By.Id("team-autocomplete");
        private readonly By TeamDropdownValues = By.XPath("//ul[@role='listbox']/li");
        private static By TeamDropdownSelectValue(string teamName) => By.XPath($"//ul[@id='team-autocomplete-listbox']/li[text()='{teamName}']");
        private readonly By SelectTeamValidationMessage = By.Id("Teams_validationMessage");
        private readonly By JiraBoardDropDown = By.Id("board-autocomplete");
        private readonly By JiraBoardDropdownValues = By.XPath("//ul[@id='board-autocomplete-listbox']/li");
        private static By JiraBoardDropdownSelectValue(string boardName) => By.XPath($"//ul[@id='board-autocomplete-listbox']/li[text()='{boardName}']");
        private readonly By SelectJiraBoardValidationMessage = By.Id("Boards_validationMessage");
        private readonly By LinkButton = By.XPath("//button[text()='Link']");

        //Linked Teams to Boards table
        private static By LinkedTeamName(string teamName) => By.XPath($"//div[@role='rowgroup']//div/a[contains(text(), '{teamName}')]");
        private static By LinkedJiraBoardName(string jiraBoardName) => By.XPath($"//div[@role='rowgroup']//div[contains(text(),'{jiraBoardName}')]");
        private static By LinkedProjectName(string projectName) => By.XPath($"//div[@role='rowgroup']//div[contains(text(),'{projectName}')]");

        private static By SelectTeam(string jiraBoardName) => By.XPath($"//div[@role='presentation']//div[text()='{jiraBoardName}']//parent::div//preceding-sibling::div//input");
        private readonly By UnLinkButton = By.XPath("//button[text()='Unlink']");
        private readonly By UnlinkTeamsYesButton = By.XPath("//button[text()='Yes']");

        //Delete Instance        
        private readonly By DeleteInstanceTitleText = By.XPath("//span[text() = 'Delete Instance']");
        private static By DeleteInstanceCheckBox(string instanceName) => By.XPath($"//div[text()='{instanceName}']//ancestor::div[@role = 'rowgroup']//div[@data-field='__check__']");
        private readonly By DeleteButton = By.XPath("//button[text() = 'Delete']");
        private readonly By WarningPopup = By.Id("alertDialogTitle");
        private readonly By WarningPopupYesButton = By.XPath("//button[text() = 'Yes']");

        //Jira Cloud Credentials popup
        public bool IsAuthenticationPopupPresent()
        {
            return Driver.IsElementPresent(AuthenticationPopupTitleText);
        }

        public string GetInstanceNameMandatoryValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the mandatory field validation message for the 'Instance Name' text field");
            Wait.UntilElementClickable(InstanceNameTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(InstanceNameValidationText).GetText();
        }

        public string GetUrlMandatoryValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the mandatory field validation message for the 'Url' text field");
            Wait.UntilElementClickable(UrlTextBox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(UrlValidationText).GetText();
        }

        public void EnterInstanceName(string instanceName)
        {
            Log.Step(nameof(LinkTeamBasePage), "Enter the 'Instance name'.");
            Wait.UntilElementVisible(InstanceNameTextBox).SetText(instanceName, isReact: true);
        }

        public void EnterUrl(string url)
        {
            Log.Step(nameof(LinkTeamBasePage), "Enter the 'Url'.");
            Wait.UntilElementVisible(UrlTextBox).SetText(url, isReact: true);
        }

        public string GetInstanceNameAlreadyExistValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the instance name already exist validation message");
            return Wait.UntilElementVisible(InstanceNameValidationText).GetText();
        }

        public string GetUrlAlreadyExistValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the url already exist validation message");
            return Wait.UntilElementVisible(UrlValidationText).GetText();
        }

        public void ClickOnAuthenticationPopupOkButton()
        {
            Log.Step(nameof(LinkTeamBasePage), "Click on the Jira Cloud Credentials - 'Ok' button.");
            Wait.UntilElementClickable(AuthenticationPopupOkButton).Click();
        }

        //Link Team page
        public void WaitUntilLinkTeamPageLoaded()
        {
            Log.Step(nameof(LinkTeamBasePage), "Wait until Link team page is loaded");
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.UntilElementVisible(LinkTeamPageTitleText);
        }

        public bool IsLinkTeamTitleDisplayed()
        {
            Wait.UntilElementVisible(LinkTeamTitleText);
            return Driver.IsElementPresent(LinkTeamTitleText);
        }

        public List<string> GetJiraInstanceNameLists()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the jira instance name list from the 'Jira Instance' dropdown.");
            Wait.UntilElementClickable(JiraInstanceArrowDropdownIcon).Click();
            Wait.UntilJavaScriptReady();
            var instanceNamesList = Driver.GetTextFromAllElements(JiraInstanceNameList).ToList();
            var actions = new Actions(Driver);
            actions.SendKeys(Keys.Escape).Perform();
            return instanceNamesList;
        }

        public void ClickOnAddNewInstancePlusIcon()
        {
            Log.Step(nameof(LinkTeamBasePage), "Click on the 'Add New Instance Plus' icon");
            Wait.UntilElementClickable(AddNewInstancePlusIcon).Click();
        }

        public void SelectJiraProject(string projectName)
        {
            Log.Step(nameof(LinkTeamBasePage), $"Select the Jira Project - '{projectName}' from the 'Jira Project' dropdown.");
            SelectItem(JiraProjectDropdownIcon, JiraProjectDropdownValue(projectName));
            Wait.HardWait(2000);//
        }

        public List<string> GetAgilityHealthTeamsList()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get list of AH Teams.");
            Wait.UntilElementClickable(TeamDropDown).Click();
            return Wait.UntilAllElementsLocated(TeamDropdownValues).Select(x => x.GetText()).ToList();
        }

        public void SelectAgilityHealthTeam(string teamName)
        {
            Log.Step(nameof(LinkTeamBasePage), $"Select a Team -{teamName}");
            SelectItem(TeamDropDown, TeamDropdownSelectValue(teamName));
        }

        public string GetSelectTeamValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the 'Select Team' validation message.");
            return Wait.UntilElementVisible(SelectTeamValidationMessage).GetText();
        }

        public List<string> GetJiraBoardList()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the list of Jira Boards.");
            Wait.UntilElementClickable(JiraBoardDropDown).Click();
            return Wait.UntilAllElementsLocated(JiraBoardDropdownValues).Select(x => x.GetText()).ToList();
        }

        public void SelectJiraBoard(string boardName)
        {
            Log.Step(nameof(LinkTeamBasePage), "Select a 'Jira Board'.");
            SelectItem(JiraBoardDropDown, LinkTeamBasePage.JiraBoardDropdownSelectValue(boardName));
        }

        public string GetSelectBoardValidationMessage()
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the 'Link' validation message.");
            return Wait.UntilElementVisible(SelectJiraBoardValidationMessage).GetText();
        }

        public void ClickOnLinkButton()
        {
            Log.Step(nameof(LinkTeamBasePage), "Click on the 'Link' button.");
            Wait.UntilElementClickable(LinkButton).Click();
            Wait.HardWait(2000);//It takes time to link Team to Board.
        }

        //Linked Teams to Boards table
        public bool IsUnlinkButtonEnabled()
        {
            return Driver.IsElementEnabled(UnLinkButton);
        }

        public void UnlinkAlreadyLinkedTeam(string jiraBoardName, bool select = true)
        {
            Log.Step(nameof(LinkTeamBasePage), "Unlink the Jira Board if it is already linked with another team.");
            if (!IsLinkedBoardNamePresent(jiraBoardName)) return;
            Driver.JavaScriptScrollToElement(SelectTeam(jiraBoardName), false).Check(select);
            Driver.JavaScriptScrollToElement(UnLinkButton);
            Wait.UntilElementClickable(UnLinkButton).Click();
            Wait.UntilElementClickable(UnlinkTeamsYesButton).Click();
            Wait.HardWait(5000); //It takes some time to unlink team with board
        }

        public void LinKTeam(string projectName, string boardName, string teamName)
        {

            Log.Step(nameof(LinkTeamBasePage), "Link an AH team with the Jira Board.");
            SelectItem(JiraProjectDropdownIcon, JiraProjectDropdownValue(projectName));
            Wait.HardWait(2000);
            SelectItem(JiraBoardDropDown, LinkTeamBasePage.JiraBoardDropdownSelectValue(boardName));
            SelectItem(TeamDropDown, TeamDropdownSelectValue(teamName));
            Wait.UntilElementClickable(LinkButton).Click();
        }

        public string GetLinkedTeamName(string teamName)
        {
            return Wait.UntilElementVisible(LinkedTeamName(teamName)).GetText();
        }

        public string GetLinkedJiraBoardName(string jiraBoardName)
        {
            Log.Step(nameof(LinkTeamBasePage), "Get the linked 'Board Name'.");
            return Wait.UntilElementVisible(LinkedJiraBoardName(jiraBoardName)).GetText();
        }

        public bool IsLinkedBoardNamePresent(string jiraBoardName)
        {
            return Driver.IsElementPresent(LinkedJiraBoardName(jiraBoardName));
        }

        public string GetLinkedJiraProjectName(string jiraProjectName)
        {
            return Wait.UntilElementVisible(LinkedProjectName(jiraProjectName)).GetText();
        }

        public void ClickOnDeleteInstanceTitle()
        {
            Log.Step(nameof(ManageIntegrationsPage), "Click on the 'Delete Instance' title");
            Wait.UntilElementClickable(DeleteInstanceTitleText).Click();
        }

        public void ClickOnDeleteInstanceCheckBox(string instanceName)
        {
            Log.Step(nameof(ManageIntegrationsPage), $"Click on the 'Delete Instance' check box for the {instanceName}");
            Wait.UntilElementClickable(DeleteInstanceCheckBox(instanceName)).Click();
        }

        public void ClickOnDeleteButton()
        {
            Log.Step(nameof(ManageIntegrationsPage), "Click on the 'Delete' button");
            Wait.UntilElementClickable(DeleteButton).Click();
        }

        public bool IsWarningPopupPresent()
        {
            return Driver.IsElementPresent(WarningPopup);
        }

        public void ClickOnYesButtonOnWarningPopup()
        {
            Log.Step(nameof(ManageIntegrationsPage), "Click on the 'Yes' button.");
            Wait.UntilElementVisible(WarningPopupYesButton);
            Wait.UntilElementClickable(WarningPopupYesButton).Click();
        }

        public void DeleteInstance(string instanceName)
        {
            Log.Step(nameof(ManageIntegrationsPage), $"Delete the instance name:'{instanceName}'");
            ClickOnDeleteInstanceCheckBox(instanceName);
            ClickOnDeleteButton();
            ClickOnYesButtonOnWarningPopup();
        }
    }
}