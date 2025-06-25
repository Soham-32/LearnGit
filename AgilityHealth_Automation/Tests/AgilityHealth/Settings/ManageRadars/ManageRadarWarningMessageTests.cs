using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    [TestCategory("Settings"),TestCategory("LanguageTranslation")]
    [TestCategory("SiteAdmin")]
    public class ManageRadarWarningMessageTests : ManageRadarBaseTests
    {
        private static readonly RadarDetails RadarInfo = ManageRadarFactory.GetValidRadarDetails();
        [TestMethod]
        public void ManageRadar_Verify_Warning_Message_While_Creating_New_Radar()
        {
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);
            var editRadarDimensionsPage = new EditRadarDimensionsPage(Driver, Log);
            var editRadarSubDimensionsPage = new EditRadarSubDimensionsPage(Driver, Log);
            var editRadarCompetenciesPage = new EditRadarCompetenciesPage(Driver, Log);
            var editRadarQuestionsPage = new EditRadarQuestionsPage(Driver, Log);
            var editRadarOpenEndedPage = new EditRadarOpenEndedPage(Driver, Log);
            var editRadarReviewAndFinishPage = new EditRadarReviewAndFinishPage(Driver, Log);
            var addRadarDetailsPage = new AddRadarDetailsPage(Driver, Log);
            var addRadarReviewAndFinishBasePage = new RadarReviewAndFinishBasePage(Driver, Log);
            const string expectedWarningMessage = "Radar is ACTIVE proceed with caution";
            
            Log.Info("Login to the application and navigate to 'Manage Radar' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Click on the 'Create New Radar' button and Verify whether the 'Create Radar' button is enabled or disabled on the 'Create Radar' pop-up when selecting the 'Create Radar' radio button.");
            manageRadarPage.ClickOnCreateNewRadarButton();
            manageRadarPage.CreateRadarPopupClickOnCreateRadarRadioButton();
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

            Log.Info("Enter All the Radar Details and Click 'Save and Continue' button on 'Create Radar' page.");
            addRadarDetailsPage.EnterRadarDetails(RadarInfo);
            addRadarDetailsPage.EnterMessagesTextsDetails(RadarInfo);
            addRadarDetailsPage.ClickOnSaveAndContinueButton();
            addRadarReviewAndFinishBasePage.SelectWizardStep("Review & Finish");
            addRadarReviewAndFinishBasePage.SelectActionType(RadarReviewAndFinishBasePage.SelectAction.IamDone);

            Log.Info($"Click on '{RadarInfo.Name}' Radar 'Edit' button.");
            manageRadarPage.ClickOnRadarEditIcon(RadarInfo.Name);

            Log.Info("Click on 'Active' Checkbox button and 'Update' button.");
            editRadarDetailsPage.ClickOnActiveCheckboxButton(true);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Click on 'Back' button and verify that Warning message is present on 'Radar Detail' page.");
            editRadarDetailsPage.ClickOnBackToButton();
            Assert.IsTrue(editRadarDetailsPage.IsWarningMessagePresent(), "Warning message is not present on 'Radar Detail' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarDimensionsPage.GetWarningMessage(),"Warning Message doesn't match");

            Log.Info("Click on 'Continue to Dimensions' button and verify that Warning message is present on 'Dimension' page.");
            editRadarDimensionsPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarDimensionsPage.IsWarningMessagePresent(), "Warning message is not present on 'Dimension' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarDimensionsPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on 'Continue to SubDimensions' button and verify that Warning message is present on 'Sub-Dimensions' page.");
            editRadarSubDimensionsPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarSubDimensionsPage.IsWarningMessagePresent(), "Warning message is not present on 'Sub-Dimensions' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarSubDimensionsPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on 'Continue to Competencies' button and verify that Warning message is present on 'Competencies' page.");
            editRadarCompetenciesPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarCompetenciesPage.IsWarningMessagePresent(), "Warning message is not present on 'Competencies' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarCompetenciesPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on 'Continue to Questions' button and verify that Warning message is present on 'Questions' page.");
            editRadarQuestionsPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarQuestionsPage.IsWarningMessagePresent(), "Warning message is not Present on 'Questions' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarQuestionsPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on 'Continue to Open-Ended Section' button and verify that Warning message is present on 'Open-Ended' page.");
            editRadarOpenEndedPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarOpenEndedPage.IsWarningMessagePresent(), "Warning message is not present on 'Open-Ended' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarOpenEndedPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on 'Review & Finish' button and verify that Warning message is present on 'Review & Finish' page.");
            editRadarReviewAndFinishPage.ClickOnContinueToButton();
            Assert.IsTrue(editRadarReviewAndFinishPage.IsWarningMessagePresent(), "Warning message is not present on 'Review & Finish' page.");
            Assert.AreEqual(expectedWarningMessage, editRadarReviewAndFinishPage.GetWarningMessage(), "Warning Message doesn't match");

            Log.Info("Click on \"I'm Done\" button");
            editRadarReviewAndFinishPage.SelectActionType(RadarReviewAndFinishBasePage.SelectAction.IamDone);

            Log.Info($"Click on '{RadarInfo.CompanyName}' Radar 'Edit' button.");
            manageRadarPage.ClickOnRadarEditIcon(RadarInfo.Name);

            Log.Info("Click on 'Active' Checkbox button and 'Update' button.");
            editRadarDetailsPage.ClickOnActiveCheckboxButton(false);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Click on 'Back' button and Navigate to 'Radar Detail' page and verify that Warning message is not present on 'Radar Detail' page.");
            editRadarDetailsPage.ClickOnBackToButton();
            Assert.IsFalse(editRadarDetailsPage.IsWarningMessagePresent(), "Warning message is present on 'Radar Detail' page.");

            Log.Info("Click on 'Continue to Dimensions' button and verify that Warning message is not present on 'Dimension' page.");
            editRadarDimensionsPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarDimensionsPage.IsWarningMessagePresent(), "Warning message is present on 'Dimension' Page.");

            Log.Info("Click on 'Continue to SubDimensions' button and verify that Warning message is not present on 'Sub-Dimensions' page.");
            editRadarSubDimensionsPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarSubDimensionsPage.IsWarningMessagePresent(), "Warning message is present on 'Sub-Dimensions' page.");

            Log.Info("Click on 'Continue to Competencies' button and verify that Warning message is not present on 'Competencies' page.");
            editRadarCompetenciesPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarCompetenciesPage.IsWarningMessagePresent(), "Warning message is present on 'Competencies' page.");

            Log.Info("Click on 'Continue to Questions' button and verify that Warning message is not present on 'Questions' page.");
            editRadarQuestionsPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarQuestionsPage.IsWarningMessagePresent(), "Warning message is present on 'Questions' page.");

            Log.Info("Click on 'Continue to Open-Ended Section' button and verify that Warning message is not present on 'Open-Ended' page.");
            editRadarOpenEndedPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarOpenEndedPage.IsWarningMessagePresent(), "Warning message is present on 'Open-Ended' page.");

            Log.Info("Click on 'Review & Finish' button and verify that Warning message is not present on 'Review & Finish' page.");
            editRadarReviewAndFinishPage.ClickOnContinueToButton();
            Assert.IsFalse(editRadarReviewAndFinishPage.IsWarningMessagePresent(), "Warning message is present on 'Review & Finish' page.");

            Log.Info("Click on \"I'm Done\" button");
            editRadarReviewAndFinishPage.SelectActionType(RadarReviewAndFinishBasePage.SelectAction.IamDone);

            //Clean-Up Method
            Log.Info("Navigate to 'Manage Radar' page and Delete the Radar");
            DeleteRadar(RadarInfo.Name);
        }
    }
}
