using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Teams.Custom.QuickLaunch;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch
{
    public class QuickLaunchTeamPage : BasePage
    {
        public QuickLaunchTeamPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By TitleText = By.XPath("//div[@id='team_window']/h3");
        private readonly By CreateTeamInfoText = By.XPath("//div[@id='team_create']/preceding-sibling::div");
        private readonly By TeamNameTextbox = By.Id("assessmentQuickTeamname");
        private readonly By SelectWorkTypeDropdown = By.XPath("//div[@id='divDDLWorkType']//span[@class='k-input']");
        private readonly By WorkTypeList = By.XPath("//ul[@id='teamsWorkType_listbox']/li");
        private static By WorkTypeItem(string workType) => By.XPath($"//ul[@id='teamsWorkType_listbox']/li[contains(normalize-space(), '{workType}')]");
        private readonly By GenerateTeamLinkButton = By.Id("quickTeam");
        private readonly By TeamCreatedText = By.XPath("//div[@id='team_success']/div[1]/span");
        private readonly By CopyLinkInfoText = By.XPath("//div[@id='team_success']/div[2]/span");
        private readonly By CopyIcon = By.Id("btnCopyToClipboardTeam");
        private readonly By DoneButton = By.Id("team_done");

        //Validation
        private readonly By TeamNameFieldValidationText = By.Id("teamValidateQuickTeam");
        private readonly By WorkTypeSelectionFieldValidationText = By.Id("radarValidateWorkType");

        public string GetTitleText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get title text");
            return Wait.UntilElementVisible(TitleText).GetText();

        }
        public string GetInfoText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get info text");
            return Wait.UntilElementVisible(CreateTeamInfoText).GetText();
        }
        public bool IsGenerateTeamLinkButtonEnabled()
        {
            return Driver.IsElementEnabled(GenerateTeamLinkButton);
        }
        public bool IsTeamNameTextBoxTextEmpty()
        {
            return string.IsNullOrWhiteSpace(Wait.UntilElementVisible(TeamNameTextbox).GetText());
        }
        public string GetSelectedWorkTypeText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get selected work type text");
            return Wait.UntilElementVisible(SelectWorkTypeDropdown).GetText();
        }

        public void EnterQuickLaunchTeamInfo(QuickLaunchTeam quickLaunchTeamInfo)
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Enter 'Quick Launch Assessment' info for team");
            EnterTeamName(quickLaunchTeamInfo.TeamName);
            SelectWorkType(quickLaunchTeamInfo.WorkType);
        }
        public List<string> GetWorkTypeList()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get 'Work Type' List");
            return Wait.UntilAllElementsLocated(WorkTypeList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public void EnterTeamName(string teamName)
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Enter team name");
            Wait.UntilElementEnabled(TeamNameTextbox).SetText(teamName);
        }

        public void ClickOnWorkTypeDropdown()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Click on 'Select Work Type' dropdown");
            Wait.UntilElementClickable(SelectWorkTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectWorkType(string workType)
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Select work type from 'Select Work Type' dropdown");
            SelectItem(SelectWorkTypeDropdown, WorkTypeItem(workType));
        }

        public string CopyTeamAccessLink()
        {
            ClickOnGenerateTeamLinkButton();
            var copiedLink = GetCopiedLinkText();
            ClickOnDoneButton();
            return copiedLink;
        }

        public void ClickOnGenerateTeamLinkButton()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Click on 'Generate Team Link' button");
            Wait.UntilElementClickable(GenerateTeamLinkButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public string GetTeamCreatedText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get 'Team Created' text");
            return Wait.UntilElementVisible(TeamCreatedText).GetText();
        }

        public string GetCopyLinkInfoText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get 'Copy' link info text");
            return Wait.UntilElementVisible(CopyLinkInfoText).GetText();
        }

        public string GetCopiedLinkText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Click on 'Copy' icon");
            Wait.UntilElementClickable(CopyIcon).Click();
            var copiedLink = CSharpHelpers.GetClipboard();
            return copiedLink;
        }

        public void ClickOnDoneButton()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Click on 'Done' button");
            Wait.UntilElementClickable(DoneButton).Click();
        }

        //tooltip
        public string GetCopyIconTooltipMessage()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get the tooltip text for the 'Copy' icon");
            return Wait.UntilElementVisible(CopyIcon).GetAttribute("title");
        }

        //Validation
        public string GetTeamNameFieldValidationText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get text of team validation text");
            return Wait.UntilElementExists(TeamNameFieldValidationText).GetText();
        }

        public string GetWorkTypeSelectionFieldValidationText()
        {
            Log.Step(nameof(QuickLaunchTeamPage), "Get text of work type validation text");
            return Wait.UntilElementExists(WorkTypeSelectionFieldValidationText).GetText();
        }
    }
}
