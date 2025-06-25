using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    [TestCategory("Settings")]
    [TestCategory("SiteAdmin")]
    public class ManageRadarCleanFormatButtonTests : ManageRadarBaseTests
    {
        private const string SelectAssessment = SharedConstants.TeamHealthRadarName;
        private static readonly RadarDetails RadarInfo = ManageRadarFactory.GetValidRadarDetails();

        [TestMethod]
        public void ManageRadar_Verify_CleanFormat_Button()
        {
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);
            var radarQuestionsBasePage = new RadarQuestionsBasePage(Driver, Log);
            var addRadarReviewAndFinishBasePage = new RadarReviewAndFinishBasePage(Driver, Log);

            Log.Info("Login to the application and Navigate to 'Manage Radar' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Click on the 'Create New Radar' button and create a radar");
            manageRadarPage.ClickOnCreateNewRadarButton();
            manageRadarPage.CreateRadarPopupClickOnCopyExistingRadarRadioButton();
            manageRadarPage.CreateRadarPopupSelectAssessment(SelectAssessment);
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

            Log.Info("Enter All the Radar Details and Click 'Update' button on 'Edit Radar' page.");
            editRadarDetailsPage.EnterRadarDetails(RadarInfo);
            editRadarDetailsPage.EnterMessagesTextsDetails(RadarInfo);
            editRadarDetailsPage.ClickOnUpdateButton();
            editRadarDetailsPage.ClickOnBackToButton();

            Log.Info("Select Language from Header language dropdown, navigate to Questions page and edit the questions.");
            editRadarDetailsPage.SelectRadarLanguage(RadarInfo.Language);
            addRadarReviewAndFinishBasePage.SelectWizardStep("Questions");
            radarQuestionsBasePage.ClickOnQuestionEditButtonByIndex(1);

            Log.Info("Select all the question text for applied all style");
            radarQuestionsBasePage.SelectAllQuestionTextAndApplyAllStyles(RadarInfo.Language);

            Log.Info("Click on 'Update' button on question edit popup");
            radarQuestionsBasePage.ClickOnQuestionEditUpdateButton();

            Log.Info("Click on 'Edit' button in question list");
            radarQuestionsBasePage.ClickOnQuestionEditButtonByIndex(1);

            Log.Info("Click on update button and verify the question text contains 'Italic' and 'Bold' tag");
            Assert.IsTrue(radarQuestionsBasePage.IsQuestionTextContainsItalicTag(RadarInfo.Language), "Italic tag is not applied");
            Assert.IsTrue(radarQuestionsBasePage.IsQuestionTextContainsBoldTag(RadarInfo.Language), "Bold tag is not applied");

            Log.Info("Verify the Underline style is applied on question text");
            Assert.IsTrue(radarQuestionsBasePage.DoesQuestionTextUnderlineTag(RadarInfo.Language), "Underline list is not applied");

            Log.Info("Verify the align left text style is applied on question text");
            Assert.IsTrue(radarQuestionsBasePage.DoesQuestionTextContainAlignLeftText(RadarInfo.Language), "Align left text is not applied");

            Log.Info("Verify the text contains the unordered list style");
            Assert.IsTrue(radarQuestionsBasePage.DoesQuestionTextContainsUnorderedList(RadarInfo.Language), "Unordered list is not applied");

            Log.Info("Click on clean formatting button icon for clear the text formatting");
            radarQuestionsBasePage.ClickOnCleanFormattingButton(RadarInfo.Language);

            Log.Info("Click on 'Update' button on question edit popup");
            radarQuestionsBasePage.ClickOnQuestionEditUpdateButton();

            Log.Info("Click on 'Edit' icon in question list");
            
            Driver.RefreshPage();// Due to Bug: 40716, refreshing page and one verification
            radarQuestionsBasePage.ClickOnQuestionEditButtonByIndex(1);

            Log.Info("Verify the question text contains 'Italic' and 'Bold' tag");
            Assert.IsFalse(radarQuestionsBasePage.IsQuestionTextContainsItalicTag(RadarInfo.Language), "Italic tag is present");
            if (RadarInfo.Language == "English")
            {
                Assert.IsFalse(radarQuestionsBasePage.IsQuestionTextContainsBoldTag(RadarInfo.Language), "Bold tag is present");
            }
            
            Log.Info("Verify the Underline style is not present on question text or not");
            Assert.IsFalse(radarQuestionsBasePage.DoesQuestionTextUnderlineTag(RadarInfo.Language), "Underline list is applied");

            Log.Info("Verify the align text left side is not present on question text");
            Assert.IsFalse(radarQuestionsBasePage.DoesQuestionTextContainAlignLeftText(RadarInfo.Language), "Align left text is applied");

            Log.Info("Verify the text is not contains the Unordered list");
            Assert.IsFalse(radarQuestionsBasePage.DoesQuestionTextContainsUnorderedList(RadarInfo.Language), "Unordered list is applied");

            //Clean-Up Method
            Log.Info("Navigate to 'Manage Radar' page and Delete the Radar");
            DeleteRadar(RadarInfo.Name);
        }
    }
}
