using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard
{
    internal class AssessmentDashboardCommon : BasePage
    {
        protected AssessmentDashboardCommon(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By AddAnAssessmentButtonText = By.XPath("//div[@class='pg-title']//button");
        private readonly By AddAnAssesmentButton = By.XPath("*//*[text()='Add An Assessment']");
        private readonly By AssessmentToggle = By.CssSelector("#toggle-header a");
        private readonly By PulseToggle = By.CssSelector("#toggle-header-pulse a");
        private readonly By PendoCloseButton = By.Id("_pendo-close-guide_");
        private readonly By IndividualAssessmentSection = By.Id("individual-assessments");
        private readonly By AllClosedAssessments = By.XPath("//div[@title='Closed']");
        private readonly By ClosedAssessmentWithZeroRespondence = By.XPath("//div[@class='status'][@title='Closed']/preceding-sibling::div[2]/*[@class='red'][text()='0']");
        private readonly By AssessmentDashboardTabTitleText = By.Id("headingText");

        // Select Assessment Type Popup
        private readonly By AddAnAssessmentPopupTeamRadio = By.Id("team_btn");
        private readonly By AddAnAssessmentPopupPulseRadio = By.Id("pulse_btn");
        private readonly By AddAnAssessmentPopupIndividualRadio = By.Id("individual_btn");
        private readonly By AddAnAssessmentPopupAddAssessmentBtn = By.CssSelector("input[value='Add Assessment']");
        private readonly By AssessmentTypeCloseIconPopup = By.XPath("//span[@id='assessmentTypeWindow_wnd_title']/parent::div//span[normalize-space()='Close']");

        private readonly By PulseCheckSwimLane = By.XPath("//div[@class='dashboard_2 active_class swimlane']");

        public string GetAddAnAssessmentButtonText()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Get Add An Assessment Button Text");
            return Wait.UntilElementVisible(AddAnAssessmentButtonText).GetText();
        }

        private void ClosePendoPopup()
        {
            if (Wait.InCase(PendoCloseButton) == null) return;
            Wait.UntilElementVisible(PendoCloseButton).ClickOn(Driver);
            Wait.UntilJavaScriptReady();
        }

        public bool IsClosedAssessmentPresent()
        {
            return Driver.IsElementDisplayed(AllClosedAssessments);
        }

        public void AddAnAssessment(string type)
        {
            ClosePendoPopup();
            Log.Step(GetType().Name, $"Add an assessment type = {type}");
            Wait.UntilElementVisible(AddAnAssesmentButton);
            Wait.UntilElementClickable(AddAnAssesmentButton).ClickOn(Driver);

            if (type.ToLower().Equals("team"))
            {
                Wait.UntilElementVisible(AddAnAssessmentPopupTeamRadio);
                Wait.UntilElementClickable(AddAnAssessmentPopupTeamRadio).ClickOn(Driver);
            }
            else if (type.ToLower().Equals("individual"))
            {
                Wait.UntilElementVisible(AddAnAssessmentPopupIndividualRadio);
                Wait.UntilElementClickable(AddAnAssessmentPopupIndividualRadio).ClickOn(Driver);
            }
            else if (type.ToLower().Equals("pulse"))
            {
                Wait.UntilElementVisible(AddAnAssessmentPopupPulseRadio);
                Wait.UntilElementClickable(AddAnAssessmentPopupPulseRadio).ClickOn(Driver);
            }

            try
            {
                Wait.UntilElementVisible(AddAnAssessmentPopupAddAssessmentBtn);
                Wait.UntilElementClickable(AddAnAssessmentPopupAddAssessmentBtn).ClickOn(Driver);
            }
            catch (Exception e) //If page takes > 60 secs to load.Catching exception and doing futher action.
            {
                if (e.InnerException != null) Console.WriteLine("Exception occured - " + e.InnerException.Message);

                if (Driver.GetCurrentUrl().EndsWith("/create")) //If already navigated to Assessment Create page.
                {
                    Driver.RefreshPage();
                }
                else //If still on Assessment Dashboard page, then try again.
                {
                    Driver.RefreshPage();
                    Wait.UntilElementClickable(AddAnAssesmentButton).ClickOn(Driver);

                    if (type.ToLower().Equals("team"))
                    {
                        Wait.UntilElementClickable(AddAnAssessmentPopupTeamRadio).ClickOn(Driver);
                    }
                    else if (type.ToLower().Equals("individual"))
                    {
                        Wait.UntilElementClickable(AddAnAssessmentPopupIndividualRadio).ClickOn(Driver);
                    }

                    Wait.UntilElementClickable(AddAnAssessmentPopupAddAssessmentBtn).ClickOn(Driver);
                }
            }
        }

        public void ClickOnMtEtToggleButton()
        {
            ClosePendoPopup();
            Log.Step(GetType().Name, "Click on toggle button");
            if (!Wait.UntilElementVisible(AssessmentToggle).GetAttribute("class").Contains("on"))
            {
                Wait.UntilElementClickable(AssessmentToggle).ClickOn(Driver);
            }
        }

        public void SwitchToPulseAssessment()
        {
            Log.Step(GetType().Name, "Switch to pulse assessment");
            if (!Wait.UntilElementVisible(PulseToggle).GetAttribute("class").Contains("on"))
            {
                Wait.UntilElementClickable(PulseToggle).Click();
            }
        }

        public void SwitchToIndividualAssessmentView()
        {
            Log.Step(GetType().Name, "Switch the toggle to 'Individual' assessments");
            Wait.UntilElementClickable(AssessmentToggle);

            if (!Wait.InCase(IndividualAssessmentSection).Displayed)
            {
                Wait.UntilElementClickable(AssessmentToggle).ClickOn(Driver);
                Wait.UntilElementVisible(IndividualAssessmentSection);
            }

            Wait.UntilJavaScriptReady();
        }

        public string GetToggleClass()
        {
            return Wait.UntilElementVisible(AssessmentToggle).GetElementAttribute("class");
        }

        public string GetAssessmentDashboardTabTitleText()
        {
            Log.Step(GetType().Name, "Get 'Assessment Dashboard' tab title text");
            Driver.RefreshPage();
            return Wait.UntilElementVisible(AssessmentDashboardTabTitleText).GetText();
        }
        public bool IsAssessmentTypeToggleDisplayed()
        {
            if (Driver.IsElementPresent(AssessmentToggle))
                return Wait.UntilElementExists(AssessmentToggle).Displayed;
            return false;
        }

        public bool IsAddAssessmentButtonDisplayed()
        {
            if (Driver.IsElementPresent(AddAnAssesmentButton))
                return Wait.UntilElementExists(AddAnAssesmentButton).Displayed;
            return false;
        }

        public void NavigateToPage(int teamId)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/teams/{teamId}/assessments");
            Wait.UntilJavaScriptReady();
        }

        public bool IsClosedAssessmentWithZeroRespondencePresent()
        {
            return Driver.IsElementPresent(ClosedAssessmentWithZeroRespondence);
        }
        // Select Assessment Type Popup
        public void ClickOnAddAnAssessmentButton()
        {
            Log.Info("Click on the 'Add An Assessment' button on Team Assessment dashboard page");
            Wait.UntilElementClickable(AddAnAssesmentButton).Click();
        }

        public void ClickOnCloseIconPopup()
        {
            Log.Step(nameof(TeamAssessmentDashboardPage), "Click on Close Icon popup");
            Wait.UntilElementClickable(AssessmentTypeCloseIconPopup).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsAddAnAssessmentPulseRadioButtonDisplayed()
        {
            Wait.HardWait(500); // Wait until Add Assessment popup is fully loaded
            return Driver.IsElementDisplayed(AddAnAssessmentPopupPulseRadio);
        }

        public bool IsPulseCheckSwimLaneDisplayed()
        {
            //Wait until pulse is replicate on UI
            for (var i = 0; i < 20; i++)
            {
                if (!Driver.IsElementDisplayed(PulseCheckSwimLane))
                {
                    Driver.RefreshPage();
                }
                else
                {
                    break;
                }
            }
            return Driver.IsElementDisplayed(PulseCheckSwimLane);
        }
    }
}
