using System;
using System.Linq;
using OpenQA.Selenium;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AtCommon.Dtos.CampaignsV2;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class CampaignDetailsPage : ManageCampaignsCommonPage
    {
        public CampaignDetailsPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By CreateCampaignHeaderText = By.XPath("//h1[contains(@class,'MuiTypography-root MuiTypography-h1')]");
        private readonly By LetsCreateYourCampaignText = By.XPath("//h5[contains(@class,'MuiTypography-root MuiTypography-h5')]");

        private readonly By CampaignNameTextBox = AutomationId.Equals("name", "input");
        private readonly By CampaignNameTextBoxError = AutomationId.Equals("name", "+p");

        private readonly By RadarTypeDropDown = By.XPath("//div[@automation-id='surveyId']");
        private readonly By RadarTypeDropDownError = AutomationId.Equals("surveyId", "+p");
        private static By RadarTypeOptions(string radarType) => By.XPath($"//div[@id='menu-surveyId']//li//span[text()='{radarType}'] | //div[@id='menu-surveyId']//li//font[text()='{radarType}']");
        private readonly By OptionsList = By.XPath("//ul[contains(@class,'MuiList-root MuiList-padding MuiMenu-list')]//li/span[not(@class)]");

        private const string StartDatePickerInputId = "startDate";
        private const string EndDatePickerInputId = "endDate";
        private readonly By StartDateTextbox = By.XPath("//div[@automation-id='startDate']//input");
        private readonly By EndDatePickerTextbox = By.XPath("//div[@automation-id='endDate']//input");
        private readonly By StartDateError = AutomationId.Equals("startDate", "+p");
        private readonly By EndDateError = AutomationId.Equals("endDate", "+p");
        private readonly By ParentTeamDropDown = AutomationId.Equals("parentTeamId", "input");
        private static By ParentTeamOptions(string teamName) => By.XPath($"//ul[@id='parentTeamId-listbox']/li[text()='{teamName}']");
        private readonly By ParentTeamOptionsList = By.XPath("//ul[@id='parentTeamId-listbox']/li");

        private static By FacilitatorMatchMakingStrategyRadioButton(string strategy) => By.XPath($"//input[contains(@value,'{strategy}') and @name='matchMakingStrategy']");
        private readonly By FacilitatorMatchMakingStrategyOption = By.XPath("//input[@name='matchMakingStrategy' and @checked]");
        private readonly By TargetNoOfTeamsPerFacilitatorDropdown = AutomationId.Equals("maximumFacilitatorTeamAssignments", "div");
        private static By TargetNoOfTeamsPerFacilitatorOptions(string noOfTeams) => By.XPath($"//div[@id='menu-maximumFacilitatorTeamAssignments']//li//span[contains(text(),'{noOfTeams}')] | //div[@id='menu-maximumFacilitatorTeamAssignments']//li//span//font[contains(text(),'{noOfTeams}')]");
        private readonly By TargetNoOfTeamsPerFacilitatorOption = By.XPath("//div[@id='maximumFacilitatorTeamAssignments']/span");
        private readonly By TargetNoOfTeamsPerFacilitatorDropdownError = AutomationId.Equals("maximumFacilitatorTeamAssignments", "+p");

        private readonly By CreateACampaignButton = By.XPath("//span[text()='Create a campaign']//parent::button");
        private readonly By UpdateCampaignButton = By.XPath("//span[text()='Update Campaign']//parent::button");

        //Methods      
        public string GetCreateCampaignHeaderTitle()
        {
            Log.Step(GetType().Name, "Get 'Create Campaign' page header title");
            WaitTillSpinnerNotExist();
            return Wait.UntilElementVisible(CreateCampaignHeaderText).GetText();
        }

        public string GetDescriptionText()
        {
            Log.Step(GetType().Name, "Get description text");
            return Wait.UntilElementVisible(LetsCreateYourCampaignText).GetText();
        }

        public void EnterCampaignName(string campaignName)
        {
            Log.Step(GetType().Name, "Enter the 'Campaign Name'");
            Wait.UntilElementVisible(CampaignNameTextBox).SetText(campaignName).SendKeys(Keys.Tab);
        }

        public string GetCampaignName()
        {
            Log.Step(GetType().Name, "Get the 'Campaign Name'");
            return Wait.UntilElementVisible(CampaignNameTextBox).GetText();
        }

        public string GetCampaignNameValidation()
        {
            Log.Step(GetType().Name, "Get 'Campaign Name' validation message");
            Wait.UntilElementVisible(CampaignNameTextBox).SendKeys(Keys.Tab + Keys.Tab + Keys.Tab);
            return Wait.UntilElementVisible(CampaignNameTextBoxError).GetText();
        }

        public string GetRadarType()
        {
            Log.Step(GetType().Name, "Get 'Radar Type' values");
            return Wait.UntilElementVisible(RadarTypeDropDown).GetText();
        }

        public void SelectRadarType(string radarType)
        {
            Log.Step(GetType().Name, "Select values for 'Radar type' dropdown");
            if (GetRadarType() == radarType) return;
            Driver.JavaScriptScrollToElementCenter(LetsCreateYourCampaignText);
            SelectItem(RadarTypeDropDown, RadarTypeOptions(radarType));
        }

        public List<string> GetRadarTypeList()
        {
            Log.Step(GetType().Name, "Get list of all the Radar types");
            Wait.UntilElementClickable(RadarTypeDropDown).Click();
            Wait.HardWait(1000);
            var dropdownValues = Driver.GetTextFromAllElements(OptionsList).ToList();
            Driver.ClickOnEscFromKeyboard();
            return dropdownValues;

        }

        public string GetRadarTypeValidationMessage()
        {
            Log.Step(GetType().Name, "Get 'Radar Type' validation message");
            Driver.ClickOnEscFromKeyboard();
            return Wait.UntilElementVisible(RadarTypeDropDownError).GetText();
        }

        public void SetStartAndEndDate(DateTime startDate, DateTime endDate)
        {
            Log.Step(GetType().Name, "Select values for 'Start Date'");
            if (startDate.CompareTo(new DateTime()) == 0) return;
            var calendar = new V2Calendar(Driver, StartDatePickerInputId);
            calendar.CampaignSetDateAndTime(startDate, false);

            if (endDate.CompareTo(new DateTime()) == 0) return;
            calendar = new V2Calendar(Driver, EndDatePickerInputId);
            calendar.CampaignSetDateAndTime(endDate, false);
        }

        public string GetStartDate()
        {
            Log.Step(GetType().Name, "Get 'Start Date' value");
            return Wait.UntilElementExists(StartDateTextbox).GetElementAttribute("value");
        }

        public string GetStartDateValidationMessage()
        {
            Log.Step(GetType().Name, "Get 'Start Date' validation message");
            Wait.UntilElementVisible(StartDateTextbox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(StartDateError).GetText();
        }

        public string GetEndDate()
        {
            Log.Step(GetType().Name, "Get 'End Date' value");
            return Wait.UntilElementExists(EndDatePickerTextbox).GetElementAttribute("value");
        }

        public string GetEndDateValidationMessage()
        {
            Log.Step(GetType().Name, "Get 'End Date' validation message");
            Wait.UntilElementVisible(EndDatePickerTextbox).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(EndDateError).GetText();
        }

        public string GetParentTeam()
        {
            Log.Step(GetType().Name, "Get 'Parent Team' values");
            return Wait.UntilElementVisible(ParentTeamDropDown).GetText();
        }

        public void SelectParentTeam(string teamName)
        {
            Log.Step(GetType().Name, "Select values for 'Parent Team' dropdown");
            if (GetParentTeam() == teamName) return;
            SelectItem(ParentTeamDropDown, ParentTeamOptions(teamName));
        }

        public List<string> GetParentTeamList()
        {
            Log.Step(GetType().Name, "Get list of all the Target Number of Teams Per Facilitator");
            Wait.UntilElementClickable(ParentTeamDropDown).Click();
            Wait.HardWait(2000); //wait till dropdown opens
            var dropdownList =  Driver.GetTextFromAllElements(ParentTeamOptionsList).ToList();
            Driver.ClickOnEscFromKeyboard();
            return dropdownList;
        }

        public void SelectFacilitatorsMatchMakingStrategyRadioButton(string strategy)
        {
            Log.Step(GetType().Name, "Select Facilitators Matchmaking Strategy RadioButtons");
            Wait.UntilElementClickable(FacilitatorMatchMakingStrategyRadioButton(strategy)).Check();
        }

        public string GetSelectedFacilitatorsMatchMakingStrategy()
        {
            Log.Step(GetType().Name, "Get Facilitators Matchmaking Strategy Option");
            return Wait.UntilElementVisible(FacilitatorMatchMakingStrategyOption).GetElementAttribute("value");
        }

        public void SelectNoOfTeamsPerFacilitator(string noOfTeams)
        {
            Log.Step(GetType().Name, "Select 'No. of Teams per Facilitator'");
            SelectItem(TargetNoOfTeamsPerFacilitatorDropdown, TargetNoOfTeamsPerFacilitatorOptions(noOfTeams));
            Wait.UntilJavaScriptReady();
        }

        public string GetNoOfTeamsPerFacilitator()
        {
            Log.Step(GetType().Name, "Get 'No. of Teams per Facilitator'");
            return Wait.UntilElementVisible(TargetNoOfTeamsPerFacilitatorOption).GetText();
        }

        public List<string> GetTargetNumberOfTeamsPerFacilitatorList()
        {
            Log.Step(GetType().Name, "Get list of all the Target Number of Teams Per Facilitator");
            Wait.UntilElementClickable(TargetNoOfTeamsPerFacilitatorDropdown).Click();
            Wait.HardWait(2000); //wait till dropdown opens
            var dropdownList = Driver.GetTextFromAllElements(OptionsList).ToList();
            Driver.ClickOnEscFromKeyboard();
            return dropdownList;
        }

        public string GetTargetNoOfTeamsPerFacilitatorValidation()
        {
            Log.Step(GetType().Name, "Get 'Target Number of Teams Per Facilitator' validation message");
            Wait.UntilElementVisible(TargetNoOfTeamsPerFacilitatorDropdown).SendKeys(Keys.Tab);
            return Wait.UntilElementVisible(TargetNoOfTeamsPerFacilitatorDropdownError).GetText();
        }
        public void ClickOnCreateACampaign()
        {
            Log.Step(GetType().Name, " Click on 'Create a Campaign' button");
            Wait.UntilElementClickable(CreateACampaignButton).Click();
            Wait.HardWait(4000);//wait till spinner displayed
        }

        public bool IsCreateACampaignButtonEnabled()
        {
            return Driver.IsElementEnabled(CreateACampaignButton);
        }

        public void ClickOnUpdateCampaign()
        {
            Log.Step(GetType().Name, "Click on 'Update Campaign' button");
            Wait.UntilElementClickable(UpdateCampaignButton).Click();
        }

        public void EnterCampaignDetailsInfo(CampaignDetails campaignDetails)
        {
            Log.Step(GetType().Name, "Enter Campaigns Details information");
            EnterCampaignName(campaignDetails.Name);
            SelectRadarType(campaignDetails.RadarType);
            SetStartAndEndDate(campaignDetails.StartDate, campaignDetails.EndDate);
            SelectParentTeam(campaignDetails.ParentTeam);
            SelectNoOfTeamsPerFacilitator(campaignDetails.TeamsPerFacilitator);
        }

        public CampaignDetails GetCampaignDetailsInfo()
        {
            Log.Step(GetType().Name, "Get Campaigns Details information");
            return new CampaignDetails
            {
                Name = GetCampaignName(),
                RadarType = GetRadarType(),
                StartDate = DateTime.Parse(GetStartDate()),
                EndDate = DateTime.Parse(GetEndDate()),
                FacilitatorsMatchmakingStrategy = GetSelectedFacilitatorsMatchMakingStrategy(),
                TeamsPerFacilitator = GetNoOfTeamsPerFacilitator(),
            };
        }
    }
}