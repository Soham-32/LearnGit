using System.Data;
using System.Linq;
using OpenQA.Selenium;
using AngleSharp.Common;
using AtCommon.Utilities;
using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns
{
    public class AutoMatchMakingPage : ManageCampaignsCommonPage
    {
        public AutoMatchMakingPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region CommonLocators   
        
        private readonly By ContinueToSetUpAssessmentsButton = By.XPath("//button[text()='Continue to set up assessments']");
        private readonly By HeaderTitleMessageText = By.XPath("//h5[contains(@class,'MuiTypography-root')]");
        private readonly By NumberOfTeamsText = By.XPath("//p[contains(text(),'Number of Teams:')]");
        private readonly By NumberOfTeamsEditButton = By.XPath("//p[contains(text(),'Number of Teams:')]/following-sibling::button");

        private readonly By NumberOfFacilitatorsText = By.XPath("//p[contains(text(),'Number of Facilitators:')]");
        private readonly By NumberOfFacilitatorsEditButton = By.XPath("//p[contains(text(),'Number of Facilitators:')]/following-sibling::button");

        private readonly By TargetNumberOfTeamsPerFacilitatorText = By.XPath("//p[contains(text(),'Target Number of Teams Per Facilitator:')]");
        private readonly By TargetNumberOfTeamsPerFacilitatorEditButton = By.XPath("//p[contains(text(),'Target Number of Teams Per Facilitator:')]/following-sibling::button");

        #endregion
         
        #region EditTargetNumberOfTeamsPerFacilitatorPopup

        private readonly By EditTargetNumberOfTeamsPerFacilitatorPopupTitleText = By.XPath("//h2[text()='Edit Target Number of Teams Per Facilitator']");
        private readonly By EditTargetNumberOfTeamsPerFacilitatorPopupWarning = By.XPath("//form//p[contains(@class,'MuiTypography-root')]");
        private readonly By TargetedNumberTooltipIcon = By.XPath("//form//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By TargetedNumberDropdown = By.Id("maximumFacilitatorTeamAssignments");
        private static By TargetedNumberDropdownValue(string targetRatio) => By.XPath($"//ul[@role='listbox']/li[@name='{targetRatio}']");
        private readonly By TargetNumberDropdownList = By.XPath("//ul[@role='listbox']/li[contains(@name,'per Facilitator')]");
        private readonly By EditTargetNumberOfTeamsPerFacilitatorPopupCancelButton = By.XPath("//button[text()='Cancel']");
        private readonly By EditTargetNumberOfTeamsPerFacilitatorPopupUpdateButton = By.XPath("//button[text()='Update']");
        private readonly By EditTargetNumberOfTeamsPerFacilitatorPopupCloseButton = By.XPath("//button[@aria-label='close']");

        private readonly By TeamContactViewButton = By.Id("matchmaking-view-tab-0");
        private readonly By TeamContactViewTooltipIcon = By.XPath("//button[@id='matchmaking-view-tab-0']//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By FacilitatorViewButton = By.Id("matchmaking-view-tab-1");
        private readonly By FacilitatorViewTooltipIcon = By.XPath("//button[@id='matchmaking-view-tab-1']//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By ExportToExcelButton = By.XPath("//span[text()='Export to Excel']//parent::button");
        private readonly By CreateAutoMatchesButton = By.XPath("//button[text()='Create Auto Matches']");
        private readonly By ReCreateAutoMatchesButton = By.XPath("//button/span[text()='Re-create Auto Matches']");
        private readonly By RecreateAutoMatchesTooltipIcon = By.XPath("//button[@aria-label='Information']//*[local-name()='svg' and @data-icon='question-circle']");
        private readonly By WarningMessages = By.XPath("(//p[@class='MuiTypography-root MuiTypography-body1 css-1a1edml'])[position()>3]");
        private readonly By TooltipMessage = By.XPath("//div[contains(@id,'mui-')]/div");

        #endregion

        #region Re-create popup

        private readonly By AssignmentsRecreateConfirmationPopup = By.Id("alertDialogTitle");
        private readonly By AssignmentsRecreateConfirmationPopupCancelButton = By.XPath("//h2[@id='alertDialogTitle']/following-sibling::div/button[text()='Cancel']");
        private readonly By AssignmentsRecreateConfirmationPopupOkButton = By.XPath("//h2[@id='alertDialogTitle']/following-sibling::div/button[text()='Ok']");

        #endregion

        #region Grid data Assigned facilitator

        private static By EditFacilitatorIconForTeam(string teamName) => By.XPath($"//div[@title='{teamName}']/parent::div/following-sibling::div//button");
        private readonly By OpenIcon = By.XPath("//button[@title='Open']");
        private static By FacilitatorValue(string facilitatorName) => By.XPath($"//ul[contains(@id,'listbox')]/li[text()='{facilitatorName}']");
        private readonly By CorrectIcon = By.CssSelector("svg[data-testid='CheckOutlinedIcon']");
        private readonly By CancelIcon = By.CssSelector("svg[data-testid='CloseOutlinedIcon']");

        #endregion


        //Methods

        #region CommonLocators  

        public void ClickOnContinueToSetUpAssessmentsButton()
        {
            Log.Step(GetType().Name, "Click on 'Continue To SetUp Assessments' button");
            WaitTillSpinnerNotExist();
            Wait.UntilElementClickable(ContinueToSetUpAssessmentsButton).Click();
        }

        public bool IsContinueToSetUpAssessmentsButtonEnabled()
        {
            return Driver.IsElementEnabled(ContinueToSetUpAssessmentsButton);
        }

        public bool IsContinueToSetUpAssessmentsButtonDisplay()
        {
            WaitTillSpinnerNotExist();
            return Driver.IsElementDisplayed(ContinueToSetUpAssessmentsButton);
        }

        public string GetHeaderTitleMessageText()
        {
            Log.Step(GetType().Name, "Get the Title Message Text");
            return Wait.UntilElementVisible(HeaderTitleMessageText).GetText();
        }

        public int GetCountOfSelectedTeams()
        {
            Log.Step(GetType().Name, "Get the number of teams selected for the campaign");
            return Wait.UntilElementVisible(NumberOfTeamsText).GetText().GetDigits();
        }

        public void ClickOnNumberOfTeamsEditButton()
        {
            Log.Step(GetType().Name, "Click on Number of Teams 'Edit' button");
            Wait.UntilElementClickable(NumberOfTeamsEditButton).Click();
        }

        public int GetCountOfSelectedFacilitators()
        {
            Log.Step(GetType().Name, "Get the number of facilitators selected for the campaign");
            return Wait.UntilElementVisible(NumberOfFacilitatorsText).GetText().GetDigits();
        }

        public void ClickOnNumberOfFacilitatorsEditButton()
        {
            Log.Step(GetType().Name, "Click on Number of Facilitators 'Edit' button");
            Wait.UntilElementClickable(NumberOfFacilitatorsEditButton).Click();
        }

        public int GetTargetNumberOfTeamsPerFacilitatorCount()
        {
            Log.Step(GetType().Name, "Get the Target number of Teams Per Facilitator for the campaign");
            return Wait.UntilElementVisible(TargetNumberOfTeamsPerFacilitatorText).GetText().GetDigits();
        }

        public void ClickTargetTeamsEditButton()
        {
            Log.Step(GetType().Name, "Click on Target Number 'Edit' button");
            Wait.UntilElementClickable(TargetNumberOfTeamsPerFacilitatorEditButton).Click();
        }

        public bool IsTeamsEditPopupDisplayed()
        {
            return Driver.IsElementDisplayed(EditTargetNumberOfTeamsPerFacilitatorPopupTitleText);
        }

        public string GetTeamsEditPopupWarning()
        {
            Log.Step(GetType().Name, "Get Edit Target number of Teams Per Facilitator Popup Warning message");
            return Wait.UntilElementVisible(EditTargetNumberOfTeamsPerFacilitatorPopupWarning).GetText();
        }

        public string GetTargetNumberTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Target number tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(TargetedNumberTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public void SelectTargetedNumber(string targetNumber)
        {
            Log.Step(GetType().Name, "Select target number from Targeted Number dropdown");
            SelectItem(TargetedNumberDropdown, TargetedNumberDropdownValue(targetNumber));
        }

        public List<string> GetTargetedNumberList()
        {
            Log.Step(GetType().Name, "Get list of all the 'Targeted Number' options");
            return Driver.GetTextFromAllElements(TargetNumberDropdownList).ToList();
        }

        public void ClickTeamsEditPopupCancelButton()
        {
            Log.Step(GetType().Name, "Click on 'Cancel' button");
            Wait.UntilElementClickable(EditTargetNumberOfTeamsPerFacilitatorPopupCancelButton).Click();
        }

        public bool IsTeamsEditPopupUpdateButtonEnabled()
        {
            return Driver.IsElementEnabled(EditTargetNumberOfTeamsPerFacilitatorPopupUpdateButton);
        }

        public void ClickTeamsEditPopupUpdateButton()
        {
            Log.Step(GetType().Name, "Click on 'Update' button");
            Wait.UntilElementClickable(EditTargetNumberOfTeamsPerFacilitatorPopupUpdateButton).Click();
        }

        public void ClickTeamsEditPopupCloseButton()
        {
            Log.Step(GetType().Name, "Click on 'Close' button");
            Wait.UntilElementClickable(EditTargetNumberOfTeamsPerFacilitatorPopupCloseButton).Click();
        }

#endregion

        public void ClickOnTeamContactViewButton()
        {
            Log.Step(GetType().Name, "Click on 'Team Contact View' button");
            Wait.UntilElementClickable(TeamContactViewButton).Click();
            Wait.HardWait(2000); //wait till background colour changed
        }

        public string GetTeamContactViewTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Team Contact View tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(TeamContactViewTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public void ClickOnFacilitatorViewButton()
        {
            Log.Step(GetType().Name, "Click on 'Facilitator View' button");
            Wait.UntilElementClickable(FacilitatorViewButton).Click();
            Wait.HardWait(2000); //wait till background colour changed
        }

        public string GetButtonColorByName(string buttonName)
        {
            return Wait.UntilElementVisible(buttonName == "Team Contact View" ? TeamContactViewButton : FacilitatorViewButton)
                .GetCssValue("color");
        }

        public string GetFacilitatorViewTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Facilitator View tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(FacilitatorViewTooltipIcon));
            Wait.HardWait(2000);//Wait till tooltip message displayed.
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public void ClickOnExportToExcelButton()
        {
            Log.Step(GetType().Name, "Click on 'Export To Excel' button");
            Wait.UntilElementClickable(ExportToExcelButton).Click();
        }

        public bool IsExportToExcelButtonEnabled()
        {
            return Driver.IsElementEnabled(ExportToExcelButton);
        }

        public void ClickOnCreateAutoMatchesButton()
        {
            Log.Step(GetType().Name, "Click on 'Create Auto Matches' button");
            Wait.UntilElementClickable(CreateAutoMatchesButton).Click();
        }

        public void ClickOnReCreateAutoMatchesButton()
        {
            Log.Step(GetType().Name, "Click on 'ReCreate Auto Matches' button");
            Wait.UntilElementClickable(ReCreateAutoMatchesButton).Click();
        }

        public bool IsReCreateAutoMatchesButtonDisplayed()
        {
            return Driver.IsElementDisplayed(ReCreateAutoMatchesButton);
        }

        public string GetCreateRecreateAutoMatchesTooltipMessage()
        {
            Log.Step(GetType().Name, "Hover over Create/Recreate Auto matches tooltip icon and get the message");
            Driver.MoveToElement(Wait.UntilElementClickable(RecreateAutoMatchesTooltipIcon));
            return Wait.UntilElementVisible(TooltipMessage).GetText();
        }

        public List<string> GetWarningMessagesList()
        {
            Log.Step(GetType().Name, "Get list of all the Warning Messages");
            return Driver.GetTextFromAllElements(WarningMessages).ToList();
        }

        public bool IsAssignmentsRecreateConfirmationPopupDisplayed()
        {
            return Driver.IsElementDisplayed(AssignmentsRecreateConfirmationPopup);
        }

        public void ClickOnAssignmentsRecreateConfirmationPopupCancelButton() 
        {
            Log.Step(GetType().Name, "Click on Assignments Recreate Confirmation Popup 'Cancel' button");
            Wait.UntilElementClickable(AssignmentsRecreateConfirmationPopupCancelButton).Click();
        }

        public void ClickOnAssignmentsRecreateConfirmationPopupOkButton()
        {
            Log.Step(GetType().Name, "Click on Assignments Recreate Confirmation Popup 'Ok' button");
            Wait.UntilElementClickable(AssignmentsRecreateConfirmationPopupOkButton).Click();
        }

        public void ChangeFacilitatorOfTeam(string teamName, string facilitatorName)
        {
            Log.Step(GetType().Name, $"Assign {facilitatorName} facilitator to {teamName} team");
            Wait.UntilElementClickable(EditFacilitatorIconForTeam(teamName)).Click();
            SelectItem(OpenIcon, FacilitatorValue(facilitatorName));
            Wait.UntilAllElementsLocated(CorrectIcon).GetItemByIndex(1).Click();
        }

        public void ClickOnCancelIcon()
        {
            Log.Step(GetType().Name, "Click on 'cancel' icon");
            Wait.UntilElementClickable(CancelIcon).Click();
        }

        public List<string> GetExcelColumnData(DataTable table, string columnName)
        {
            return (from DataRow row in table.Rows select row[columnName].ToString()).ToList();
        }

        public List<string> GetMatchingColumns(List<string> excelColumns, List<string> uiColumns)
        {
            return uiColumns.Intersect(excelColumns).ToList();
        }
    }
}