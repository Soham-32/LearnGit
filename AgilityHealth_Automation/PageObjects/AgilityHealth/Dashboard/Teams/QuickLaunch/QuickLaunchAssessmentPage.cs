using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom.QuickLaunch;
using AtCommon.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch
{
    internal class QuickLaunchAssessmentPage : TeamDashboardPage
    {
        public QuickLaunchAssessmentPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Quick Launch
        private readonly By QuickLaunchButton = By.XPath("//div[@id='divQuickAssessment']/div/button");
        private static By QuickLaunchOption(string option) => By.XPath($"//div[@id='divQuickAssessment']//span[text()='{option}']");
        private readonly By QuickLaunchAssessmentPopupTitleText = By.XPath("//div[@id='assessment_window']/h3");
        private readonly By QuickLaunchAssessmentPopupCreateAssessmentInfoText = By.XPath("//div[@id='assessment_create']/preceding-sibling::div");

        private readonly By QuickLaunchAssessmentPopupSelectTeamDropDown = By.XPath("//span[@aria-owns='assessmentTeams_listbox']");
        private static By QuickLaunchAssessmentPopupSelectTeam(string team) => By.XPath($"//ul[@id='assessmentTeams_listbox']/li[contains(text(), '{team}')]");
        private readonly By QuickLaunchAssessmentPopupCreateNewTeamCheckBox = By.Id("create_newTeam_btn");
        private readonly By QuickLaunchAssessmentPopupTeamNameTextbox = By.Id("assessmentQuickTeamname");
        private readonly By QuickLaunchAssessmentPopupSelectRadarDropdown = By.Id("divDDLassessmentRadar");
        private readonly By QuickLaunchAssessmentPopupRadarList = By.XPath("//ul[@id='assessmentRadar_listbox']/li");
        private static By QuickLaunchAssessmentPopupSelectRadar(string radar) => By.XPath($"//ul[@id='assessmentRadar_listbox']/li[contains(normalize-space(), '{radar}')]");
        private readonly By QuickLaunchAssessmentPopupGenerateAssessmentLinkButton = By.Id("quickAssessment");
        private readonly By QuickLaunchAssessmentPopupAssessmentCreatedText = By.XPath("//div[@id='assessment_success']/div/span[contains(text(),'Assessment Created')]");
        private readonly By QuickLaunchAssessmentPopupCopyLinkInfoText = By.XPath("//div[@id='assessment_success']/div/span[contains(text(),'Share this link with your team members to complete the assessment.')]");

        private readonly By QuickLaunchAssessmentPopupCopyIcon = By.Id("btnCopyToClipboard");
        private readonly By QuickLaunchAssessmentPopupDoneButton = By.Id("assessment_done");

        //Validation 
        private readonly By TeamNameFieldValidationText = By.Id("teamValidateQuickAssessment");
        private readonly By RadarSelectionFieldValidationText = By.Id("radarValidateQuickAssessment");


        //Quick Launch
        public bool IsQuickLaunchButtonDisplayed()
        {
            return Driver.IsElementDisplayed(QuickLaunchButton);
        }

        public bool IsQuickLaunchOptionDisplayed(string option)
        {
            return Driver.IsElementDisplayed(QuickLaunchOption(option));
        }
        public string GetQuickLaunchAssessmentPopupTitleText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get title text from Quick Launch Assessment Popup");
            return Wait.UntilElementVisible(QuickLaunchAssessmentPopupTitleText).GetText();

        }
        public string GetQuickLaunchAssessmentPopupInfoText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get text from Quick Launch Assessment Popup Create Assessment Info");
            return Wait.UntilElementVisible(QuickLaunchAssessmentPopupCreateAssessmentInfoText).GetText();
        }

        public bool IsGenerateAssessmentLinkButtonEnabled()
        {
            return Wait.UntilElementExists(QuickLaunchAssessmentPopupGenerateAssessmentLinkButton).Displayed;
        }

        public void ClickOnQuickLaunchOptionsLink(string option)
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Quick Launch Assessment'");
            MoveToQuickLaunchButton();
            Wait.UntilElementClickable(QuickLaunchOption(option)).Click();
        }

        public List<string> GetQuickLaunchAssessmentPopupRadarList()
        {
            Log.Step(nameof(TeamDashboardPage), "Get 'Radar' List");
            return Wait.UntilAllElementsLocated(QuickLaunchAssessmentPopupRadarList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public void EnterQuickLaunchAssessmentInfo(QuickLaunchAssessment quickLaunchAssessmentInfo)
        {
            Log.Step(nameof(TeamDashboardPage), "Enter 'Quick Launch Assessment' info for existing team");

            if (quickLaunchAssessmentInfo.CreateNewTeam)
            {
                QuickLaunchAssessmentPopupClickOnCreateNewTeamCheckBox();
                QuickLaunchAssessmentPopupEnterTeamName(quickLaunchAssessmentInfo.NewTeamName);
            }
            else
            {
                QuickLaunchAssessmentPopupSelectTeamName(quickLaunchAssessmentInfo.ExistingTeamName);
            }

            QuickLaunchAssessmentPopupSelectRadarName(quickLaunchAssessmentInfo.RadarName);
        }
        public void QuickLaunchAssessmentPopupSelectTeamName(string team)
        {
            Log.Step(nameof(TeamDashboardPage), "Select existing team from 'Select Team' dropdown");
            SelectItem(QuickLaunchAssessmentPopupSelectTeamDropDown, QuickLaunchAssessmentPopupSelectTeam(team));
        }
        public void QuickLaunchAssessmentPopupClickOnCreateNewTeamCheckBox()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Create New Team' checkBox");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(QuickLaunchAssessmentPopupCreateNewTeamCheckBox).Check();
        }
        public void QuickLaunchAssessmentPopupEnterTeamName(string newTeamName)
        {
            Log.Step(nameof(TeamDashboardPage), "Enter 'New Team' name in textbox");
            Wait.UntilElementEnabled(QuickLaunchAssessmentPopupTeamNameTextbox).SetText(newTeamName);
        }
        public void ClickOnSelectRadarDropdown()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Select Radar' dropdown");
            Wait.UntilElementClickable(QuickLaunchAssessmentPopupSelectRadarDropdown).Click();
            Wait.UntilJavaScriptReady();
        }
        public void QuickLaunchAssessmentPopupSelectRadarName(string radar)
        {
            Log.Step(nameof(TeamDashboardPage), "Select Radar from 'Select Radar' dropdown");
            SelectItem(QuickLaunchAssessmentPopupSelectRadarDropdown, QuickLaunchAssessmentPopupSelectRadar(radar));
        }
        public string QuickLaunchAssessmentPopupCopyAssessmentAccessLink()
        {
            QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            var quickLaunchAssessmentPopupCopiedLink = GetQuickLaunchAssessmentPopupCopyIconLink();
            QuickLaunchAssessmentPopupClickOnDoneButton();
            return quickLaunchAssessmentPopupCopiedLink;
        }
        public void QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Generate Assessment Link' button");
            Wait.UntilElementClickable(QuickLaunchAssessmentPopupGenerateAssessmentLinkButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public string GetQuickLaunchAssessmentPopupAssessmentCreatedText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get the Quick launch assessment popup 'Assessment Created' text");
            return Wait.UntilElementVisible(QuickLaunchAssessmentPopupAssessmentCreatedText).GetText();
        }
        public string GetQuickLaunchAssessmentPopupCopyLinkInfoText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get the Quick launch assessment popup 'Copy' link title text");
            return Wait.UntilElementVisible(QuickLaunchAssessmentPopupCopyLinkInfoText).GetText();
        }
        public string GetQuickLaunchAssessmentPopupCopyIconLink()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Copy' icon for generated assessment link");
            Wait.UntilElementClickable(QuickLaunchAssessmentPopupCopyIcon).Click();
            var quickLaunchAssessmentPopupCopiedLink = CSharpHelpers.GetClipboard();
            return quickLaunchAssessmentPopupCopiedLink;
        }
        public void QuickLaunchAssessmentPopupClickOnDoneButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Click on 'Done' button");
            Wait.UntilElementClickable(QuickLaunchAssessmentPopupDoneButton).Click();
        }

        public void MoveToQuickLaunchButton()
        {
            Log.Step(nameof(TeamDashboardPage), "Move to 'Quick Launch' button");
            Driver.MoveToElement(Wait.UntilElementExists(QuickLaunchButton));
        }

        //tooltip
        public string GetQuickLaunchAssessmentPopupCopyIconTooltipMessage()
        {
            Log.Step(nameof(TeamDashboardPage), "Get the tooltip text for the 'Copy' icon");
            return Wait.UntilElementVisible(QuickLaunchAssessmentPopupCopyIcon).GetAttribute("title");
        }

        //Validation
        public string GetQuickLaunchAssessmentPopupTeamNameFieldValidationText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get text of team validation text");
            return Wait.UntilElementExists(TeamNameFieldValidationText).GetText();
        }

        public string GetQuickLaunchAssessmentPopupRadarSelectionFieldValidationText()
        {
            Log.Step(nameof(TeamDashboardPage), "Get text of team validation text");
            return Wait.UntilElementExists(RadarSelectionFieldValidationText).GetText();
        }

        public enum QuickLaunchOptions
        {
            Assessment,
            Team
        }

    }
}
