using System;
using System.Linq;
using OpenQA.Selenium;
using AngleSharp.Common;
using AtCommon.Utilities;
using System.Globalization;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class ReviewAndSubmitPage : ManageCampaignsCommonPage
    {
        public ReviewAndSubmitPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Locators
        private readonly By LaunchCampaignButton = By.XPath("//button/span[text()='Launch campaign']");
        private readonly By CampaignDetailsEditButton = By.XPath("//div[@id='campaign-details-summary-header']//button");
        private readonly By CampaignDetailsCampaignName = By.XPath("//strong[text()='Campaign Name:']/parent::p/parent::div//following-sibling::div/span");
        private readonly By CampaignDetailsRadarType = By.XPath("//strong[text()='Radar Type:']/parent::p/parent::div//following-sibling::div/span");
        private readonly By CampaignDetailsStartDate = By.XPath("//strong[text()='Start Date:']/parent::p/parent::div//following-sibling::div/span");
        private readonly By CampaignDetailsEndDate = By.XPath("//strong[text()='End Date:']/parent::p/parent::div//following-sibling::div/span");
        private readonly By CampaignDetailsFacilitatorMatchingStrategy = By.XPath("//strong[text()='Facilitator Matching Strategy:']/parent::p/parent::div//following-sibling::div/span");
        private readonly By FacilitationApproachEditButton = By.XPath("//div[@id='assessment-setup-summary-header']//button");
        private readonly By FacilitationApproachExpandMoreButton = By.XPath("//div[@id='assessment-setup-summary-header']//*[local-name()='svg' and @data-testid='ExpandMoreIcon']");
        private readonly By FacilitationApproachAssessmentName = By.XPath("//strong[text()='Assessment Name']/parent::p/parent::div//following-sibling::div/span");
        private readonly By FacilitationApproachCampaignStarts = By.XPath("//p[contains(text(),'Campaign Starts')]");
        private readonly By FacilitationApproachCampaignEnds = By.XPath("//p[contains(text(),'Campaign Ends')]");
        private readonly By FacilitationApproachTimelineLabels = By.XPath("//div[@id='assessment-setup-summary-content']//div[contains(@class,'MuiBox-root') and text()]");

        private readonly By FacilitationApproachStakeholderLaunchDate = By.XPath("//strong[text()='Stakeholder Launch Date']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Stakeholder Launch Date']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachTeamMemberLaunchDate = By.XPath("//strong[text()='Team Member Launch Date']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Team Member Launch Date']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachAssessmentStartDate = By.XPath("//strong[text()='Assessment Start Date']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Assessment Start Date']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachAssessmentCloseDate = By.XPath("//strong[text()='Assessment Close Date']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Assessment Close Date']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachRetrospectiveWindowStart = By.XPath("//strong[text()='Retrospective Window Start']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Retrospective Window Start']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachRetrospectiveWindowEnd = By.XPath("//strong[text()='Retrospective Window End']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Retrospective Window End']/ancestor::p/parent::div//following-sibling::div/p");

        private readonly By FacilitationApproachStakeholderWindowStart = By.XPath("//strong[text()='Stakeholder Window Start']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Stakeholder Window Start']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By FacilitationApproachStakeholderWindowEnd = By.XPath("//strong[text()='Stakeholder Window End']/parent::p/parent::div//following-sibling::div/p | //strong//font[text()='Stakeholder Window End']/ancestor::p/parent::div//following-sibling::div/p");
        private readonly By MatchmakingEditButton = By.XPath("//div[@id='matchmaking-result-summary-header']//button");
        private readonly By MatchmakingExpandMoreButton = By.XPath("//div[@id='matchmaking-result-summary-header']//*[local-name()='svg' and @data-testid='ExpandMoreIcon']");
        private readonly By MatchmakingTeamContactViewButton = By.Id("matchmaking-view-tab-0");
        private readonly By MatchmakingFacilitatorViewButton = By.Id("matchmaking-view-tab-1");
        private readonly By MatchmakingTeamContactViewTooltipIcon = By.XPath("//button[@id='matchmaking-view-tab-0']//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By MatchmakingFacilitatorViewTooltipIcon = By.XPath("//button[@id='matchmaking-view-tab-1']//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By TooltipMessage = By.XPath("//div[contains(@id,'mui-')]/div");

        private static By MatchmakingColumnValuesList(string columnValueDataField) => By.XPath($"//div[@id='matchmaking-result-summary-content']//div[contains(@class,'MuiDataGrid-virtualScrollerRenderZone')]//div[@data-id]/div[count(//div[@aria-rowindex='2']/div[@data-field='{columnValueDataField}']//preceding-sibling::div[@data-field] )+1]");

        private static By MatchmakingRowValuesList(int index, string columnValueDataField) => By.XPath($"//div[@id='matchmaking-result-summary-content']//div[contains(@class,'MuiDataGrid-virtualScrollerRenderZone')]//div[@data-id][{index}]/div[count(//div[@aria-rowindex='2']/div[@data-field='{columnValueDataField}']//preceding-sibling::div[@data-field] )+1]/*");

        private static By MatchmakingColumnName(string columnName) =>
            By.XPath($"//div[@id='matchmaking-result-summary-content']//div[@aria-rowindex='2']//div[@aria-label][text()='{columnName}']");
        private readonly By TeamSelectEditButton = By.XPath("//div[@id='team-selection-summary-header']//button");
        private readonly By TeamSelectExpandMoreButton = By.XPath("//div[@id='team-selection-summary-header']//*[local-name()='svg' and @data-testid='ExpandMoreIcon']");
        private readonly By TeamSelectCampaignHeaderText = By.XPath("//p[contains(text(),'Team Selected For Campaign')]");

        private static By TeamSelectColumnValuesList(string columnHeader) => By.XPath($"//div[@id='team-selection-summary-content']//table[contains(@class,'k-grid-tab')]//td[count(//div[@id='team-selection-summary-content']//table[contains(@class,'k-grid-header')]//th[@colspan='1']//span[text()='{columnHeader}']//ancestor::th//preceding-sibling::th)+1]");

        private static By TeamSelectRowValuesList(int index, string columnHeader) => By.XPath($"//div[@id='team-selection-summary-content']//table[contains(@class,'k-grid-tab')]//tr[{index}]/td[count(//div[@id='team-selection-summary-content']//table[contains(@class,'k-grid-header')]//th[@colspan='1']//span[text()='{columnHeader}']//ancestor::th//preceding-sibling::th)+1]/*");
        private readonly By FacilitatorSelectEditButton = By.XPath("//div[@id='facilitator-selection-summary-header']//button");
        private readonly By FacilitatorSelectExpandMoreButton = By.XPath("//div[@id='facilitator-selection-summary-header']//*[local-name()='svg' and @data-testid='ExpandMoreIcon']");

        private readonly By FacilitatorSelectCampaignHeader = By.XPath("//p[contains(text(),'Facilitator Selected For Campaign')]");

        private static By FacilitatorSelectColumnValuesList(string columnHeader) => By.XPath($"//div[@id='facilitator-selection-summary-content']//table[contains(@class,'k-grid-tab')]//td[count(//div[@id='selected-facilitators-grid']//table[contains(@class,'k-grid-header')]//th//span[text()='{columnHeader}']//ancestor::th//preceding-sibling::th)+1]");

        private static By FacilitatorSelectRowValuesList(int index, string columnHeader) => By.XPath($"//div[@id='facilitator-selection-summary-content']//table[contains(@class,'k-grid-tab')]//tr[{index}]/td[count(//div[@id='selected-facilitators-grid']//table[contains(@class,'k-grid-header')]//th//span[text()='{columnHeader}']//ancestor::th//preceding-sibling::th)+1]");

        private static By FacilitatorSelectColumnName(string columnName) =>
            By.XPath($"//div[@id='facilitator-selection-summary-content']//table[@role]//thead//th//span[text()='{columnName}']");
        private readonly By WarningMessages = By.XPath("//*[local-name()='svg' and @data-testid='WarningAmberOutlinedIcon']/parent::div/following-sibling::div/p");


        //Methods
        public void ClickOnLaunchCampaignButton()
        {
            Log.Step(GetType().Name, "Click on 'Launch Campaign' button");
            Driver.JavaScriptScrollToElement(Wait.UntilAllElementsLocated(LaunchCampaignButton).GetItemByIndex(1)).Click();
            Wait.HardWait(15000); //takes too much time to launch campaign
            WaitTillSpinnerNotExist();
        }

        public bool IsLaunchCampaignButtonDisplayed()
        {
            WaitTillSpinnerNotExist();
            return Driver.IsElementDisplayed(LaunchCampaignButton);
        }

        public void ClickOnCampaignDetailsEditButton()
        {
            Log.Step(GetType().Name, "Click on 'Campaign Details Edit' button");
            Wait.UntilElementClickable(CampaignDetailsEditButton).Click();
        }

        public string GetCampaignDetailsCampaignName()
        {
            return Wait.UntilElementVisible(CampaignDetailsCampaignName).GetText();
        }
        public string GetCampaignDetailsRadarType()
        {
            return Wait.UntilElementVisible(CampaignDetailsRadarType).GetText();
        }
        public string GetCampaignDetailsStartDate()
        {
            return Wait.UntilElementVisible(CampaignDetailsStartDate).GetText();
        }
        public string GetCampaignDetailsEndDate()
        {
            return Wait.UntilElementVisible(CampaignDetailsEndDate).GetText();
        }
        public string GetCampaignDetailsFacilitatorMatchingStrategy()
        {
            Log.Step(GetType().Name, "Get Campaign Details 'Facilitator Matching Strategy' value");
            return Wait.UntilElementVisible(CampaignDetailsFacilitatorMatchingStrategy).GetText();
        }

        public void ClickOnFacilitationApproachEditButton()
        {
            Log.Step(GetType().Name, "Click on 'Facilitation Approach  Edit' button");
            Wait.UntilElementClickable(FacilitationApproachEditButton).Click();
        }
        public void ClickOnFacilitationApproachExpandMoreButton()
        {
            Log.Step(GetType().Name, "Click on 'Facilitation Approach Expand More' button");
            Wait.UntilElementClickable(FacilitationApproachExpandMoreButton).Click();
        }

        public string GetFacilitationApproachAssessmentName()
        {
            return Wait.UntilElementVisible(FacilitationApproachAssessmentName).GetText();
        }

        public string GetFacilitationApproachCampaignStartsDate()
        {
            Log.Step(GetType().Name, "Get Campaign Starts Date");
            return Wait.UntilElementVisible(FacilitationApproachCampaignStarts).GetText().Replace("Campaign Starts ", "");
        }

        public string GetFacilitationApproachCampaignEndsDate()
        {
            Log.Step(GetType().Name, "Get Campaign Ends Date");
            return Wait.UntilElementVisible(FacilitationApproachCampaignEnds).GetText().Replace("Campaign Ends ", "");
        }

        public List<string> GetFacilitationApproachTimelineLabelsList()
        {
            Log.Step(GetType().Name, "Get list of all the Assessment Timeline Labels");
            return Driver.GetTextFromAllElements(FacilitationApproachTimelineLabels).ToList();
        }

        public string GetFacilitationApproachStakeholderLaunchDate()
        {
            return Wait.UntilElementVisible(FacilitationApproachStakeholderLaunchDate).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachTeamMemberLaunchDate()
        {
            return Wait.UntilElementVisible(FacilitationApproachTeamMemberLaunchDate).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachAssessmentCloseDate()
        {
            return Wait.UntilElementVisible(FacilitationApproachAssessmentCloseDate).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachRetrospectiveWindowStart()
        {
            return Wait.UntilElementVisible(FacilitationApproachRetrospectiveWindowStart).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachRetrospectiveWindowEnd()
        {
            return Wait.UntilElementVisible(FacilitationApproachRetrospectiveWindowEnd).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachStakeholderWindowStart()
        {
            return Wait.UntilElementVisible(FacilitationApproachStakeholderWindowStart).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachStakeholderWindowEnd()
        {
            return Wait.UntilElementVisible(FacilitationApproachStakeholderWindowEnd).GetText().Split(',')[0].Trim();
        }
        public string GetFacilitationApproachAssessmentStartDate()
        {
            return Wait.UntilElementVisible(FacilitationApproachAssessmentStartDate).GetText().Split(',')[0].Trim();
        }

        public List<string> GetMatchmakingColumnValuesList(string columnName)
        {
            Log.Step(GetType().Name, "Get list of Matchmaking table data by column name");
            Driver.JavaScriptScrollToElement(MatchmakingColumnValuesList(columnName));
            return Driver.GetTextFromAllElements(MatchmakingColumnValuesList(columnName)).ToList();
        }

        public List<string> GetMatchmakingRowValuesList(int index, string columnName)
        {
            Log.Step(GetType().Name, "Get list of Matchmaking table data by column name");
            Driver.JavaScriptScrollToElement(MatchmakingRowValuesList(index, columnName));
            return Driver.GetTextFromAllElements(MatchmakingRowValuesList(index, columnName)).ToList();
        }

        public string GetMatchmakingColumnName(string columnName)
        {
            Log.Step(GetType().Name, "Get matchmaking column name");
            return Wait.UntilElementVisible(MatchmakingColumnName(columnName)).GetText();
        }

        public void ClickOnMatchmakingEditButton()
        {
            Log.Step(GetType().Name, "Click on 'Matchmaking  Edit' button");
            Wait.UntilElementClickable(MatchmakingEditButton).Click();
        }

        public void ClickOnMatchmakingExpandMoreButton()
        {
            Log.Step(GetType().Name, "Click on 'Matchmaking  Expand More' button");
            Wait.UntilElementClickable(MatchmakingExpandMoreButton).Click();
        }

        public void ClickOnMatchmakingTeamContactViewButton()
        {
            Log.Step(GetType().Name, "Click on Matchmaking 'Team Contact View' button");
            Wait.UntilElementClickable(MatchmakingTeamContactViewButton).Click();
        }

        public void ClickOnMatchmakingFacilitatorViewButton()
        {
            Log.Step(GetType().Name, "Click on Matchmaking 'Facilitator View' button");
            Wait.UntilElementClickable(MatchmakingFacilitatorViewButton).Click();
        }

        public string GetMatchmakingTeamContactViewTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Team Contact View tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(MatchmakingTeamContactViewTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public string GetMatchmakingFacilitatorViewTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Facilitator View tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(MatchmakingFacilitatorViewTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public void ClickOnTeamSelectedForCampaignEditButton()
        {
            Log.Step(GetType().Name, "Click on 'Team Selected for Campaign Edit' button");
            Wait.UntilElementClickable(TeamSelectEditButton).Click();
        }

        public void ClickOnTeamSelectedForCampaignExpandMoreButton()
        {
            Log.Step(GetType().Name, "Click on 'Team Selected for Campaign Edit' button");
            Driver.JavaScriptScrollToElement(TeamSelectExpandMoreButton);
            Wait.HardWait(2000); //wait till scroll to element
            Wait.UntilElementClickable(TeamSelectExpandMoreButton).Click();
            Wait.HardWait(2000);// Wait till all teams displayed
        }

        public int GetNumberOfTeamsSelectedForCampaignCount()
        {
            Log.Step(GetType().Name, "Get numbers of team selected for Campaign");
            return Wait.UntilElementVisible(TeamSelectCampaignHeaderText).GetText().GetDigits();
        }

        public List<string> GetTeamSelectColumnValuesList(string columnName)
        {
            Log.Step(GetType().Name, "Get list of Team select table data by column name");
            Driver.JavaScriptScrollToElement(TeamSelectColumnValuesList(columnName));
            return Driver.GetTextFromAllElements(TeamSelectColumnValuesList(columnName)).ToList();
        }

        public List<string> GetTeamSelectRowValuesList(int index, string columnName)
        {
            Log.Step(GetType().Name, "Get list of Team select table data by column name");
            Driver.JavaScriptScrollToElement(TeamSelectRowValuesList(index, columnName));
            return Driver.GetTextFromAllElements(TeamSelectRowValuesList(index, columnName)).ToList();
        }

        public void ClickOnFacilitatorSelectedForCampaignEditButton()
        {
            Log.Step(GetType().Name, "Click on 'Facilitator Selected for Campaign Edit' button");
            Wait.UntilElementClickable(FacilitatorSelectEditButton).Click();
        }
        public void ClickOnFacilitatorSelectedForCampaignExpandMoreButton()
        {
            Log.Step(GetType().Name, "Click on 'Facilitator Selected for Campaign Expand More' button");
            Driver.JavaScriptScrollToElement(FacilitatorSelectExpandMoreButton);
            Wait.HardWait(2000); //wait till scroll to element
            Wait.UntilElementClickable(FacilitatorSelectExpandMoreButton).Click();
            Wait.HardWait(2000); //Wait till selected facilitator appear
        }
        public int GetNumberOfFacilitatorsSelectedForCampaignCount()
        {
            Log.Step(GetType().Name, "Get numbers of facilitators selected for Campaign");
            return Wait.UntilElementVisible(FacilitatorSelectCampaignHeader).GetText().GetDigits();
        }

        public List<string> GetFacilitatorSelectColumnValuesList(string columnName)
        {
            Log.Step(GetType().Name, "Get list of Facilitator select table data by column name");
            Driver.JavaScriptScrollToElement(FacilitatorSelectColumnValuesList(columnName));
            return Driver.GetTextFromAllElements(FacilitatorSelectColumnValuesList(columnName)).ToList();
        }

        public List<string> GetFacilitatorSelectRowValuesList(int index, string columnName)
        {
            Log.Step(GetType().Name, "Get list of Facilitator select table data by column name");
            Driver.JavaScriptScrollToElement(FacilitatorSelectRowValuesList(index, columnName));
            return Driver.GetTextFromAllElements(FacilitatorSelectRowValuesList(index, columnName)).ToList();
        }

        public string GetFacilitatorSelectColumnName(string columnName)
        {
            Log.Step(GetType().Name, "Get facilitator select column name");
            return Wait.UntilElementVisible(FacilitatorSelectColumnName(columnName)).GetText();
        }

        public List<string> GetWarningMessagesList()
        {
            Log.Step(GetType().Name, "Get list of all the Warning Messages");
            return Driver.GetTextFromAllElements(WarningMessages).ToList();
        }

        public string GetDateInMmDdYyyyFormat(DateTime dateTime)
        {
            return dateTime.ToString("M/d/yyyy", CultureInfo.InvariantCulture);
        }

    }
}