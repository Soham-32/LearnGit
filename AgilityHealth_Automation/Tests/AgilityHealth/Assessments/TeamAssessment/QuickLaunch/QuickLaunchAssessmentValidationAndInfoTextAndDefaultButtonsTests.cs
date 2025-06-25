using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey.QuickLaunch;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories.QuickLaunch;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams.QuickLaunch.QuickLaunchAssessmentPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.QuickLaunch
{
    [TestClass]
    [TestCategory("Assessments"), TestCategory("QuickLaunch")]
    [TestCategory("CompanyAdmin")]
    public class QuickLaunchAssessmentValidationAndInfoTextAndDefaultButtonsTests : BaseTest
    {
        public static string TeamNameValidation = "The Team Name field is required.";
        public static string RadarSelectionValidation = "The Radar field is required.";

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 44309
        public void QuickLaunchAssessment_Verify_Validation_InfoText_DefaultButtons()
        {
            var login = new LoginPage(Driver, Log);
            var quickLaunchAssessmentPage = new QuickLaunchAssessmentPage(Driver, Log);
            var quickLaunchMemberAccessPage = new QuickLaunchMemberAccessPage(Driver, Log);

            var getValidQuickLaunchAssessmentInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchAssessmentInfo();
            var getValidQuickLaunchMemberAccessInfo = QuickLaunchAssessmentFactory.GetValidQuickLaunchMemberAccessInfo();

            Log.Info("Login as CA and verify that 'Quick Launch' button is displayed");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            Assert.IsTrue(quickLaunchAssessmentPage.IsQuickLaunchButtonDisplayed(), "'Quick Launch' button is not displayed");

            Log.Info("Hover over the 'Quick Launch' button and verify 'Assessment' and 'Team' links are displayed");
            quickLaunchAssessmentPage.MoveToQuickLaunchButton();
            Assert.IsTrue(quickLaunchAssessmentPage.IsQuickLaunchOptionDisplayed(QuickLaunchOptions.Assessment.ToString()), "'Assessment' link is not displayed");
            Assert.IsTrue(quickLaunchAssessmentPage.IsQuickLaunchOptionDisplayed(QuickLaunchOptions.Team.ToString()), "'Team' link is not displayed");

            Log.Info("Verify Quick launch assessment popup title with description after click on 'Assessment' from 'Quick Launch' options");
            quickLaunchAssessmentPage.ClickOnQuickLaunchOptionsLink(QuickLaunchOptions.Assessment.ToString());
            Assert.AreEqual("Create Assessment", quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupTitleText(), "Quick launch assessment pop up Title text is not matched");
            Assert.AreEqual("Select or create a new team, then choose the radar for your assessment", quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupInfoText(), "'Create Assessment' description is not matched");

            Log.Info("Verify that 'Generate Assessment Link' button is enabled");
            Assert.IsTrue(quickLaunchAssessmentPage.IsGenerateAssessmentLinkButtonEnabled(), "'Generate Assessment Link' button is disabled");

            Log.Info("Verify validation message for 'Team' and 'Radar' after click on 'Generate Assessment Link' button without filling any data");
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            Assert.AreEqual(TeamNameValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupTeamNameFieldValidationText(), "Validation message is not matched for Select Team field");
            Assert.AreEqual(RadarSelectionValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupRadarSelectionFieldValidationText(), "Validation message is not matched for Select Radar field");

            Log.Info("Click on 'Generate Assessment Link' after select 'Radar' and verify validation message");
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupSelectRadarName(getValidQuickLaunchAssessmentInfo.RadarName);
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            Assert.AreEqual(TeamNameValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupTeamNameFieldValidationText(), "Validation message is not matched for Select Team field");

            Log.Info("Click on 'Generate Assessment Link' button after Check 'Create New Team' checkbox and verify validation message");
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnCreateNewTeamCheckBox();
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            Assert.AreEqual(TeamNameValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupTeamNameFieldValidationText(), "Validation message is not matched for Select Team field");
            Assert.AreEqual(RadarSelectionValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupRadarSelectionFieldValidationText(), "Validation message is not matched for Select Radar field");

            Log.Info("Click on 'Generate Assessment Link' button after enter team name in 'Create New Team' textbox and verify validation message");
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupEnterTeamName(getValidQuickLaunchAssessmentInfo.NewTeamName);
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            Assert.AreEqual(RadarSelectionValidation, quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupRadarSelectionFieldValidationText(), "Validation message is not matched for Select Radar field");

            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupSelectRadarName(getValidQuickLaunchAssessmentInfo.RadarName);
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnGenerateAssessmentLinkButton();
            Assert.IsFalse(quickLaunchAssessmentPage.IsGenerateAssessmentLinkButtonEnabled(), "'Generate Assessment Link' button is enable");

            Log.Info("Verify 'Assessment Created' text with 'Copy' link info text");
            Assert.AreEqual("Assessment Created", quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupAssessmentCreatedText(), "Quick launch assessment is not created");
            Assert.AreEqual("Share this link with your team members to complete the assessment.", quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupCopyLinkInfoText(), "Quick launch assessment copy link is not generated");

            Log.Info("Click on 'Copy' icon and verify tool tip message for 'Copy' icon");
            var quickLaunchAssessmentLink = quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupCopyIconLink();
            Assert.AreEqual("Copy Link", quickLaunchAssessmentPage.GetQuickLaunchAssessmentPopupCopyIconTooltipMessage(), "Tooltip message is not matched for 'Copy' icon");

            Log.Info("Click on 'Done' button");
            quickLaunchAssessmentPage.QuickLaunchAssessmentPopupClickOnDoneButton();

            Log.Info("Copy and paste the generated assessment link and verify the page title text and description");
            Driver.SwitchTo().NewWindow(WindowType.Tab);
            Driver.NavigateToPage(quickLaunchAssessmentLink);
            Assert.AreEqual((getValidQuickLaunchAssessmentInfo.NewTeamName + " - " + "Assessment Access").ToLower(), quickLaunchMemberAccessPage.GetAssessmentAccessPageTitleText().ToLower(), "'Assessment Access' page header title doesn't match");

            var expectedDescriptionList = new List<string>{
               "Before you begin, please fill out the information below and select the role(s) and participant group(s) you belong to.",
               "This information will be used to add you to the team but will not be shared with the assessment results.",
               "Assessment results remain anonymous at all times."
            };
            Assert.That.ListsAreEqual(expectedDescriptionList, quickLaunchMemberAccessPage.GetAssessmentAccessPageDescriptionList(), "Description list is not matched");

            Log.Info("Verify 'Submit' button is displayed");
            Assert.IsTrue(quickLaunchMemberAccessPage.IsSubmitButtonDisplayed(), "'Submit' button is not displayed");

            Log.Info("Verify Tooltip message for 'Role(s)' & 'Participant Group(s)' and Information text");
            Assert.AreEqual("Select the role you play in the team.", quickLaunchMemberAccessPage.GetTooltipMessage("Role"), "Tooltip message is not matched for Role");
            Assert.AreEqual("Select the participant group you play in the team.", quickLaunchMemberAccessPage.GetTooltipMessage("Participant Group"), "Tooltip message is not matched for Participant Groups");

            Log.Info("Click on 'Submit' button and verify the validation message for every mandatory fields");
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("The First Name field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("FirstName"), "Validation messae is not matched for FirstName field");
            Assert.AreEqual("The Last Name field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("LastName"), "Validation messae is not matched for FirstName field");
            Assert.AreEqual("The Company Email field is required.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation messae is not matched for FirstName field");
            Assert.AreEqual("The Role field is required.", quickLaunchMemberAccessPage.GetRoleValidationMessageText(), "Validation message is not matched for Role field");

            Log.Info("Enter the invalid company email and verify validation message for 'Email'");
            getValidQuickLaunchMemberAccessInfo.Email = "invalidEmail";
            quickLaunchMemberAccessPage.EnterQuickLaunchAssessmentAccessInfo(getValidQuickLaunchMemberAccessInfo);
            quickLaunchMemberAccessPage.ClickOnSubmitButton();
            Assert.AreEqual("This value should be a valid email.", quickLaunchMemberAccessPage.GetValidationMessageText("EmailAddress"), "Validation messae is not matched for FirstName field");
        }
    }
}
