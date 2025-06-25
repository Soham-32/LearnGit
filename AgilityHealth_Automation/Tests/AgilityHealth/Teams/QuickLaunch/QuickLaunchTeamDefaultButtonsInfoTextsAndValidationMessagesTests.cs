using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.QuickLaunch;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.QuickLaunch
{
    [TestClass]
    [TestCategory("Team"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchTeamDefaultButtonsInfoTextsAndValidationMessagesTests : BaseTest
    {
        [TestMethod]
        public void QuickLaunchTeam_Verify_DefaultButtons_InfoTexts_ValidationMessages()
        {
            var driverObject = Driver;
            var login = new LoginPage(Driver, Log);
            var quickLaunchTeamPage = new QuickLaunchTeamPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchTeamInfo = QuickLaunchTeamFactory.GetValidQuickLaunchTeamInfo();
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA and open 'Quick Launch Team' popup and verify info texts");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            Assert.AreEqual("Create Team", quickLaunchTeamPage.GetTitleText(), "Quick launch team pop up Title text is not matched");
            Assert.AreEqual("To quickly create a team, please complete the details below.", quickLaunchTeamPage.GetInfoText(), "'Create Team' description is not matched");

            Log.Info("Verify default state of 'Generate Team Link' button, 'Team Name' textbox and 'Work Type' drop-down");
            Assert.IsTrue(quickLaunchTeamPage.IsGenerateTeamLinkButtonEnabled(), "'Generate Team Link' button is disabled");
            Assert.IsTrue(quickLaunchTeamPage.IsTeamNameTextBoxTextEmpty(), "Team name text is not empty");
            Assert.AreEqual("Select Work Type", quickLaunchTeamPage.GetSelectedWorkTypeText(), "Default option is not selected");

            Log.Info("Verify validation message for 'Team' and 'Work Type' after click on 'Generate Team Link' button without filling any data");
            quickLaunchTeamPage.ClickOnGenerateTeamLinkButton();
            Assert.AreEqual("The Team Name field is required.", quickLaunchTeamPage.GetTeamNameFieldValidationText(), "Validation message is not matched for Team Name field");
            Assert.AreEqual("The WorkType field is required.", quickLaunchTeamPage.GetWorkTypeSelectionFieldValidationText(), "Validation message is not matched for Select Work Type field");

            Log.Info("Click on 'Generate Team Link' button after select 'Work Type' and verify validation message");
            quickLaunchTeamPage.SelectWorkType(getValidQuickLaunchTeamInfo.WorkType);
            quickLaunchTeamPage.ClickOnGenerateTeamLinkButton();
            Assert.AreEqual("The Team Name field is required.", quickLaunchTeamPage.GetTeamNameFieldValidationText(), "Validation message is not matched for Select Team field");

            Log.Info("Select 'Team' option from 'Quick Launch' after refresh the page then enter team name and verify validation message after click on 'Generate Team Link' button");
            Driver.RefreshPage();
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchAssessmentPage.QuickLaunchOptions.Team.ToString());
            quickLaunchTeamPage.EnterTeamName(getValidQuickLaunchTeamInfo.TeamName);
            quickLaunchTeamPage.ClickOnGenerateTeamLinkButton();
            Assert.AreEqual("The WorkType field is required.", quickLaunchTeamPage.GetWorkTypeSelectionFieldValidationText(), "Validation message is not matched for Select Work Type field");

            Log.Info("Fill all the required information in 'Quick Launch Team' pop up and verify 'Team Created' text with 'Copy' link info text after click on 'Generate Team Link' button");
            quickLaunchTeamPage.EnterQuickLaunchTeamInfo(getValidQuickLaunchTeamInfo);
            quickLaunchTeamPage.ClickOnGenerateTeamLinkButton();
            Assert.AreEqual("Team Created", quickLaunchTeamPage.GetTeamCreatedText(), "Quick launch assessment is not created");
            Assert.AreEqual("Invite team members by sharing this link:", quickLaunchTeamPage.GetCopyLinkInfoText(), "Quick launch assessment copy link is not generated");

            Log.Info("Click on 'Copy' icon and verify tool tip message for 'Copy' icon");
            lock (ClipboardLock)
            {
                var quickLaunchTeamLink = quickLaunchTeamPage.GetCopiedLinkText();
                Assert.AreEqual("Copy Link", quickLaunchTeamPage.GetCopyIconTooltipMessage(), "Tooltip message is not matched for 'Copy' icon");

                Log.Info("Click on 'Done' button then copy & paste the generated team link in new tab and verify the page title and description text");
                quickLaunchTeamPage.ClickOnDoneButton();
                driverObject.SwitchTo().NewWindow(WindowType.Tab);
                driverObject.NavigateToPage(quickLaunchTeamLink);
            }

            Log.Info("Click on 'Done' button then copy & paste the generated team link in new tab and verify the page title and description text");
            Assert.AreEqual((getValidQuickLaunchTeamInfo.TeamName + " - " + "Team Member Details").ToLower(), quickLaunchMemberAccessPage.GetAssessmentAccessPageTitleText().ToLower(), "'Team Member Details' page header title is not matched");
            Assert.AreEqual("Submitting this form will add you as a team member to a team in AgilityHealth.", quickLaunchMemberAccessPage.GetAssessmentAccessPageDescriptionList().ListToString(), "Description is not matched");

            Log.Info("Verify 'Submit' button and tooltip message for 'Role(s)' & 'Participant Group(s)'");
            Assert.IsTrue(quickLaunchMemberAccessPage.IsSubmitButtonDisplayed(), "'Submit' button is not displayed");

            Assert.AreEqual("Select the role you play in the team.", quickLaunchMemberAccessPage.GetTooltipMessage("Role"), "Tooltip message is not matched for Role");
            Assert.AreEqual("Select the participant group you play in the team.", quickLaunchMemberAccessPage.GetTooltipMessage("Participant Group"), "Tooltip message is not matched for Participant Groups");

            Log.Info("Click on 'Submit' button without filling any data and verify the validation message for every mandatory fields");
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("The First Name field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("FirstName"), "Validation message is not matched for FirstName field");
            Assert.AreEqual("The Last Name field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("LastName"), "Validation message is not matched for FirstName field");
            Assert.AreEqual("The Company Email field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation message is not matched for FirstName field");

            Log.Info("Enter the invalid company email and verify validation message for 'Email'");
            getValidQuickLaunchMemberAccessInfo.Email = "invalidEmail";
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("This value should be a valid email.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation message is not matched for FirstName field");
        }
    }
}
