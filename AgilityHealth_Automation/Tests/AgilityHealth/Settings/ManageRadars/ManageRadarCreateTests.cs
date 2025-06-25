using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos.Radars.Custom;
using AgilityHealth_Automation.Utilities;
using System;
using AtCommon.Api;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Radar.RadarPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    [TestCategory("Settings"),TestCategory("LanguageTranslation")]
    [TestCategory("SiteAdmin")]
    public class ManageRadarCreateTests : ManageRadarBaseTests
    {
        private const string SelectAssessment = SharedConstants.TeamAssessmentType;
        private static readonly RadarDetails RadarInfo = ManageRadarFactory.GetValidRadarDetails();

        [TestMethod]
        public void ManageRadar_Create_New_Radar()
        {
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var addRadarDetailsPage = new AddRadarDetailsPage(Driver, Log);

            Log.Info("Login to the application and navigate to 'Manage Radar' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Click on the 'Create New Radar' button and Verify whether the 'Create Radar' button is enabled or disabled on the 'Create Radar' pop-up when selecting the 'Create Radar' radio button.");
            manageRadarPage.ClickOnCreateNewRadarButton();
            Assert.IsFalse(manageRadarPage.IsCreateRadarPopupCreateRadarButtonEnabled(), " The 'Create Radar' Button on 'Create Radar' pop-up is enabled ");
            manageRadarPage.CreateRadarPopupClickOnCreateRadarRadioButton();
            Assert.IsTrue(manageRadarPage.IsCreateRadarPopupCreateRadarButtonEnabled(), "The 'Create Radar' button on 'Create Radar' pop-up is disabled after clicking on 'Crete Radar' radio button.");
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

            Log.Info("Enter All the Radar Details and Click 'Save and Continue' button on 'Create Radar' page.");
            addRadarDetailsPage.EnterRadarDetails(RadarInfo);
            addRadarDetailsPage.EnterMessagesTextsDetails(RadarInfo);
            addRadarDetailsPage.ClickOnSaveAndContinueButton();

            Log.Info("Click on 'Back' button and Verify All Radar Details"); 
            addRadarDetailsPage.ClickOnBackToButton();
            var actualRadarDetails = addRadarDetailsPage.GetRadarDetails();
            var imageWidth = addRadarDetailsPage.GetImageParameters().FirstOrDefault();
            var imageHeight= addRadarDetailsPage.GetImageParameters().LastOrDefault();

            //CompanyName, Type, RadarName, and Scale Assertion
            Assert.AreEqual(RadarInfo.CompanyName,actualRadarDetails.CompanyName, "Radar Company Name doesn't match");
            Assert.AreEqual(RadarInfo.Type, actualRadarDetails.Type, "Radar Type doesn't match");
            Assert.AreEqual(RadarInfo.Name, actualRadarDetails.Name, "Radar Name doesn't match");
            Assert.AreEqual(RadarInfo.Scale, actualRadarDetails.Scale, "Radar Scale doesn't match");

            //Logo Assertion
            Assert.AreEqual(300, imageWidth, "Radar Image width doesn't match");
            Assert.AreEqual(90, imageHeight, "Radar Image height doesn't match");

            //Dropdowns Assertion
            Assert.That.ListsAreEqual(RadarInfo.LimitedTo, actualRadarDetails.LimitedTo, "'Limited To' value doesn't match");
            Assert.That.ListsAreEqual(RadarInfo.AvailableTo, actualRadarDetails.AvailableTo, "'Available To' values doesn't match.");
            Assert.That.ListsAreEqual(RadarInfo.WorkType, actualRadarDetails.WorkType, "'WorkType' value doesn't match.");

            //Copyright Assertion
            Assert.AreEqual(RadarInfo.CopyrightText, actualRadarDetails.CopyrightText, "Radar Copyright Text doesn't match");

            //Checkboxes Assertion
            Assert.AreEqual(RadarInfo.ShowAsAbsolute,actualRadarDetails.ShowAsAbsolute,"'Show As Absolute' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Public, actualRadarDetails.Public, "'Public' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Show1Response, actualRadarDetails.Show1Response, "'Show 1 Response' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.IncludeInGlobalBenchmarking, actualRadarDetails.IncludeInGlobalBenchmarking, "'Include In GlobalBenchmarking' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Active, actualRadarDetails.Active, "'Active' Checkbox value doesn't match");

            //Default Messages Assertion
            Assert.AreEqual(RadarInfo.AssessmentWelcomeMessage, actualRadarDetails.AssessmentWelcomeMessage, "The Default 'Assessment Welcome' Message doesn't match.");
            Assert.AreEqual(RadarInfo.EmailMessageSenderName, actualRadarDetails.EmailMessageSenderName, "The Default 'Sender Name' doesn't match.");
            Assert.AreEqual(RadarInfo.EmailMessageSubject, actualRadarDetails.EmailMessageSubject, "The Default 'Subject' doesn't match.");
            Assert.AreEqual(RadarInfo.EmailWelcomeMessage, actualRadarDetails.EmailWelcomeMessage, "The Default 'Email welcome' Message doesn't match.");
            Assert.AreEqual(RadarInfo.ThankYouMessage, actualRadarDetails.ThankYouMessage, " The Default 'Thank You' Message doesn't match.");
            Assert.AreEqual(RadarInfo.AppendStandardFooter, actualRadarDetails.AppendStandardFooter, "The 'Append Standard Footer' Checkbox doesn't match.");

            //Clean-Up Method
            DeleteRadar(RadarInfo.Name);
        }

        [TestMethod]
        public void ManageRadar_Create_CopyExisting_Radar()
        {
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);

            Log.Info("Log in to the application and Navigate to 'Manage Radar' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Click on the 'Create New Radar' button and Verify whether the 'Create Radar' button is enabled or disabled on the 'Create Radar' pop-up when selecting the 'Copy Existing Radar' radio button.");
            manageRadarPage.ClickOnCreateNewRadarButton();
            Assert.IsFalse(manageRadarPage.IsCreateRadarPopupCreateRadarButtonEnabled(), "The 'Create Radar' button on 'Create Radar' pop-up is enabled ");
            manageRadarPage.CreateRadarPopupClickOnCopyExistingRadarRadioButton();
            Assert.IsFalse(manageRadarPage.IsCreateRadarPopupCreateRadarButtonEnabled(), "The 'Create Radar' button on 'Create Radar' pop-up is enabled after clicking on 'Copy Existing Radar' radio button.");
            manageRadarPage.CreateRadarPopupSelectAssessment(SelectAssessment);
            Assert.IsTrue(manageRadarPage.IsCreateRadarPopupCreateRadarButtonEnabled(), "The 'Create Radar' button on 'Create Radar' pop-up is disabled after selecting 'Assessment' type.");
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

            Log.Info(" Enter All the Radar Details and Click 'Update' button on 'Edit Radar' page.");
            editRadarDetailsPage.EnterRadarDetails(RadarInfo);
            editRadarDetailsPage.EnterMessagesTextsDetails(RadarInfo);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Click on 'Back' button and Verify All Radar Details");
            editRadarDetailsPage.ClickOnBackToButton();
            var actualEditedRadarDetails=editRadarDetailsPage.GetRadarDetails();
            var imageWidth = editRadarDetailsPage.GetImageParameters().FirstOrDefault();
            var imageHeight = editRadarDetailsPage.GetImageParameters().LastOrDefault();

            //CompanyName, Type, RadarName, and Scale Assertion
            Assert.AreEqual(RadarInfo.CompanyName, actualEditedRadarDetails.CompanyName, "Radar Company Name doesn't match");
            Assert.AreEqual(RadarInfo.Type, actualEditedRadarDetails.Type, "Radar Type doesn't match");
            Assert.AreEqual(RadarInfo.Name, actualEditedRadarDetails.Name, "Radar Name doesn't match");
            Assert.AreEqual(RadarInfo.Scale, actualEditedRadarDetails.Scale, "Radar Scale doesn't match");

            //Logo Assertion
            Assert.AreEqual(300, imageWidth, "Radar Image width doesn't match");
            Assert.AreEqual(90, imageHeight, "Radar Image height doesn't match");

            //Dropdowns Assertion
            Assert.That.ListsAreEqual(RadarInfo.LimitedTo, actualEditedRadarDetails.LimitedTo, "''Limited To' value doesn't match");
            Assert.That.ListsAreEqual(RadarInfo.AvailableTo, actualEditedRadarDetails.AvailableTo, "'Available To' values doesn't match.");
            Assert.That.ListsAreEqual(RadarInfo.WorkType, actualEditedRadarDetails.WorkType, "'WorkType' value doesn't match.");

            //Copyright Assertion
            Assert.AreEqual(RadarInfo.CopyrightText, actualEditedRadarDetails.CopyrightText, "Radar Copyright Text doesn't match");

            //Checkboxes Assertion
            Assert.AreEqual(RadarInfo.ShowAsAbsolute, actualEditedRadarDetails.ShowAsAbsolute, "'Show As Absolute' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Public, actualEditedRadarDetails.Public, "'Public' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Show1Response, actualEditedRadarDetails.Show1Response, "'Show 1 Response' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.IncludeInGlobalBenchmarking, actualEditedRadarDetails.IncludeInGlobalBenchmarking, "'Include In GlobalBenchmarking' Checkbox value doesn't match");
            Assert.AreEqual(RadarInfo.Active, actualEditedRadarDetails.Active, "'Active' Checkbox value doesn't match");

            //Default Messages Assertion
            Assert.AreEqual(RadarInfo.AssessmentWelcomeMessage, actualEditedRadarDetails.AssessmentWelcomeMessage, "The Default 'Assessment Welcome' Message doesn't match.");
            Assert.AreEqual(RadarInfo.EmailMessageSenderName, actualEditedRadarDetails.EmailMessageSenderName, "The Default 'Sender Name' doesn't match.");
            Assert.AreEqual(RadarInfo.EmailMessageSubject, actualEditedRadarDetails.EmailMessageSubject, "The Default 'Subject' doesn't match.");
            Assert.AreEqual(RadarInfo.EmailWelcomeMessage, actualEditedRadarDetails.EmailWelcomeMessage, "The Default 'Email welcome' Message doesn't match.");
            Assert.AreEqual(RadarInfo.ThankYouMessage, actualEditedRadarDetails.ThankYouMessage, " The Default 'Thank You' Message doesn't match.");
            Assert.AreEqual(RadarInfo.AppendStandardFooter, actualEditedRadarDetails.AppendStandardFooter, "The 'Append Standard Footer' Checkbox doesn't match.");


            //Clean-Up Method
            Log.Info("Navigate to 'Manage Radar' page and Delete the Radar");
            DeleteRadar(RadarInfo.Name);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 44637 , 44845
        public void ManageRadar_Create_Radar_With_DifferentLanguage()
        {
            var login = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);

            Log.Info("Log in to the application and navigate to 'Manage Radar' page.");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Navigate to the 'Edit radar details' page");
            manageRadarPage.ClickOnCreateNewRadarButton();
            manageRadarPage.CreateRadarPopupClickOnCopyExistingRadarRadioButton();
            manageRadarPage.CreateRadarPopupSelectAssessment(SelectAssessment);
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

            Log.Info("Select all languages and verify that the URL updates accordingly");
            var enumValues = System.Enum.GetValues(typeof(RadarLanguage));
            foreach (RadarLanguage enumValue in enumValues)
            {
                var lang = enumValue.ToString();
                editRadarDetailsPage.SelectRadarLanguage(lang);
                var getCurrentUrl = Driver.GetCurrentUrl();
                Assert.IsTrue(getCurrentUrl.Contains(enumValue.GetDescription()), $"The redirected url doesn't comprises of '{enumValue.GetDescription()}'");
            }

            Log.Info($"Select {RadarInfo.Language} Language from Header language dropdown on 'Edit Radar details' page.");
            editRadarDetailsPage.SelectRadarLanguage(RadarInfo.Language);

            Log.Info(" Verify 'Assessment welcome', 'Email welcome', 'ThankYou' Message Text fields and Subject Textbox are disabled on 'Edit Radar details' page.");
            Assert.IsTrue(editRadarDetailsPage.IsDefaultActiveCheckboxPresent(),"'Active' checkbox is not present");
            Assert.IsFalse(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.AssessmentWelcomeMessageIframe), " The Default 'Assessment Welcome Message' Textbox is enabled");
            Assert.IsFalse(editRadarDetailsPage.IsSenderNameTextboxEnabled(),"The Default 'SenderName' Textbox is enabled");
            Assert.IsFalse(editRadarDetailsPage.IsDefaultSubjectTextboxEnabled(), " The Default 'Subject' Textbox is enabled");
            Assert.IsFalse(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.EmailWelcomeMessageIframe), " The Default 'Email Welcome Message' Textbox is enabled");
            Assert.IsFalse(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.ThankYouMessageIframe), "The Default 'Thank You Message' Textbox is enabled");

            Log.Info(" Verify Translated 'Assessment welcome', 'Email welcome', 'ThankYou' Message Text fields and Subject Textbox are enabled on 'Edit Radar details' page.");
            Assert.IsTrue(editRadarDetailsPage.IsTranslatedActiveCheckboxPresent(),"'Translated Active' checkbox is not present");
            Assert.IsTrue(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.TranslatedAssessmentWelcomeMessageIframe), "The Translated 'Assessment Welcome Message' Textbox is disabled");
            Assert.IsTrue(editRadarDetailsPage.IsTranslatedSubjectTextboxEnabled(), "The Translated 'Subject' Textbox is disabled ");
            Assert.IsTrue(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.TranslatedEmailWelcomeMessageIframe), "The Translated 'Email Welcome Message' Textbox is disabled");
            Assert.IsTrue(editRadarDetailsPage.IsMessagesTextboxEnabled(editRadarDetailsPage.TranslatedThankYouMessageIframe), "The Translated 'Thank You Message' Textbox is disabled");

            Log.Info(" Enter Translated All the Radar Details for Different Language and Click 'Update' button on 'Edit Radar details' page.");
            editRadarDetailsPage.EnterRadarDetails(RadarInfo);
            editRadarDetailsPage.EnterTranslations(RadarInfo);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Click on 'Back' button and Verify All Radar Details");
            var expectedLanguageList = ManageRadarFactory.Languages();
            var expectedRadarTypeList = ManageRadarFactory.RadarTypes();
            var expectedWorkTypeList = ManageRadarFactory.WorkTypes();
            var expectedAvailableToList = ManageRadarFactory.AvailableTo();
            var expectedScaleList = ManageRadarFactory.Scale();
            editRadarDetailsPage.ClickOnBackToButton();
            var actualRadarInfo = editRadarDetailsPage.GetTranslatedFields();
            var actualLanguageList = editRadarDetailsPage.GetHeaderLanguageDropdownAllValues();
            var actualHeaderCompanyList = editRadarDetailsPage.GetHeaderCompanyDropdownAllValues();
            var actualRadarTypeList = editRadarDetailsPage.GetRadarTypeDropdownAllValues();
            var actualScaleList = editRadarDetailsPage.GetScaleDropdownAllValues();
            var actualAvailableToList = editRadarDetailsPage.GetAvailableToDropdownAllValues();
            var actualWorkTypeList = editRadarDetailsPage.GetWorkTypeDropdownAllValues();
            var actualCompanyList = editRadarDetailsPage.GetRadarCompanyDropdownAllValues();

            //Verify all Translated radar details
            Assert.AreEqual(RadarInfo.Language,actualRadarInfo.Language, "'Radar Language' doesn't match.");
            Assert.AreEqual(RadarInfo.TranslatedActive, actualRadarInfo.TranslatedActive, "The 'Translated Active' checkbox doesn't match");
            Assert.AreEqual(RadarInfo.TranslatedAssessmentWelcomeMessage, actualRadarInfo.TranslatedAssessmentWelcomeMessage, "The 'Translated Assessment Welcome' Message doesn't match");
            Assert.AreEqual(RadarInfo.TranslatedEmailMessageSubject, actualRadarInfo.TranslatedEmailMessageSubject, "The 'Translated Subject' Message doesn't match");
            Assert.AreEqual(RadarInfo.TranslatedEmailWelcomeMessage, actualRadarInfo.TranslatedEmailWelcomeMessage, "The 'Translated Email Welcome' Message doesn't match");
            Assert.AreEqual(RadarInfo.TranslatedThankYouMessage, actualRadarInfo.TranslatedThankYouMessage, "The 'Translated Thank You' Message doesn't match");

            Log.Info("Verify All Dropdown value");
            Assert.That.ListsAreEqual(expectedLanguageList, actualLanguageList, "Language list doesn't match");
            Assert.That.ListsAreEqual(actualCompanyList, actualHeaderCompanyList, "Companies value doesn't match");
            Assert.That.ListsAreEqual(expectedRadarTypeList, actualRadarTypeList, "'Radar Type' value doesn't match.");
            actualCompanyList.Remove(SharedConstants.CompanyName);
            Assert.That.ListsAreEqual(actualCompanyList, editRadarDetailsPage.GetLimitedToDropdownAllValues(), "Companies value doesn't match");
            Assert.That.ListsAreEqual(expectedAvailableToList, actualAvailableToList, "'Available To' value doesn't match.");
            Assert.That.ListsAreEqual(expectedWorkTypeList, actualWorkTypeList, "'WorkType' value doesn't match.");
            Assert.That.ListsAreEqual(expectedScaleList, actualScaleList, "'Radar scale' value doesn't match.");

            // Comparing Company and HeaderCompany name are similar
            Assert.AreEqual(editRadarDetailsPage.GetRadarCompanyName(),editRadarDetailsPage.GetHeaderCompanyName()," 'Company' name doesn't match");

            if(editRadarDetailsPage.GetRadarCompanyName().Equals(SharedConstants.CompanyName))
            {
                Assert.IsTrue(editRadarDetailsPage.IsHeaderCompanyDropdownEnabled(), "The Header Company dropdown is disabled");
                editRadarDetailsPage.SelectRadarCompany(User.CompanyName);
                Assert.IsFalse(editRadarDetailsPage.IsHeaderCompanyDropdownEnabled(), "The Header Company dropdown is enabled");
            }
            else
            {
                Assert.IsFalse(editRadarDetailsPage.IsHeaderCompanyDropdownEnabled(), "The Header Company dropdown is enabled");
                editRadarDetailsPage.SelectRadarCompany(SharedConstants.CompanyName);
                Assert.IsTrue(editRadarDetailsPage.IsHeaderCompanyDropdownEnabled(), "The Header Company dropdown is disabled");
            }

            //Clean-Up Method
            Log.Info("Navigate to 'Manage Radar' page and Delete the Radar");
            DeleteRadar(RadarInfo.Name);
        }
    }
}
