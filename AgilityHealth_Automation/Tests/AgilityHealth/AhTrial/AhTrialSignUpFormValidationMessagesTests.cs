using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.ObjectFactories;
using AgilityHealth_Automation.Base;
using System.Collections.Generic;
using AtCommon.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.AhTrial;
using AgilityHealth_Automation.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial")]
    public class AhTrialSignUpFormValidationMessagesTests : BaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 45421
        public void AhTrial_SignUpForm_Verify_ValidationMessages()
        {
            var ahTrialSignUpPage = new AhTrialSignUpFormPage(Driver, Log);

            Log.Info("Navigate to Sign Up form, fill sign up form with invalid phone number and verify validation message");
            ahTrialSignUpPage.NavigateToPage();
            var getAhTrialSignUpForInvalidPhone = AhTrialFactory.GetValidAhTrialSignUpFormInfo();
            getAhTrialSignUpForInvalidPhone.PhoneNumber = "1";
            ahTrialSignUpPage.EnterSignUpInfo(getAhTrialSignUpForInvalidPhone);
            ahTrialSignUpPage.ClickOnSignUpButton();
            Assert.AreEqual("Must be valid and limited to numbers only. US numbers must be 10 digits.", ahTrialSignUpPage.GetValidationMessageList().ListToString(), "Validation message is not matched for invalid phone number");

            Log.Info("Fill sign up form with mismatched password and verify validation message");
            var getSignUpDataWitMisMatchedPassword = AhTrialFactory.GetValidAhTrialSignUpFormInfo();
            getSignUpDataWitMisMatchedPassword.ReEnterPassword = "abc";
            ahTrialSignUpPage.EnterSignUpInfo(getSignUpDataWitMisMatchedPassword);
            ahTrialSignUpPage.ClickOnSignUpButton();
            Assert.AreEqual("Password & Confirm Password do not match.", ahTrialSignUpPage.GetValidationMessageList().ListToString(), "Validation message is not matched for wrong re-enter password ");
            Driver.RefreshPage();

            Log.Info("Click on 'Sign Up' button without filling any field and verify validation messages for all the fields ");
            ahTrialSignUpPage.ClickOnAhLicenseAgreementCheckbox();
            ahTrialSignUpPage.ClickOnSignUpButton();
            var expectedValidationMessageList = new List<string>()
            {
                "Please enter your first name.",
                "Please enter your last name.",
                "Please enter your email address.",
                "Please enter your company name.",
                "Please enter your phone number.",
                "Please select your country.",
                "Please select your industry.",
                "A password must contain at least 8 characters, including an upper/lower case, a numeric and a special character.",
                "Please re-enter your password.",
                "reCAPTCHA verification is required."
            };
            Assert.That.ListsAreEqual(expectedValidationMessageList, ahTrialSignUpPage.GetValidationMessageList(), "Validation message list is not matched");

            Log.Info("Fill sign up form with valid data and verify that user navigate to Team dashboard");
            var getAhTrialSignUpInfo = AhTrialFactory.GetValidAhTrialSignUpFormInfo();
            ahTrialSignUpPage.EnterSignUpInfo(getAhTrialSignUpInfo);
            ahTrialSignUpPage.ClickOnSignUpButton();
            Assert.AreEqual("Teams Dashboard - AH", Driver.Title, "Window Title is incorrect after login");

            Log.Info("Navigate to Sign Up page and fill sign up form with existing 'CompanyName', 'CompanyEmail' & 'PhoneNumber'");
            ahTrialSignUpPage.NavigateToPage();
            ahTrialSignUpPage.EnterSignUpInfo(getAhTrialSignUpInfo);
            ahTrialSignUpPage.ClickOnSignUpButton();
            var expectedValidationMessageListForExistedUser = new List<string>()
            {
                "User already exists.",
                "Company already exists.",
                "Phone Number already exists."
            };
            Assert.That.ListsAreEqual(expectedValidationMessageListForExistedUser, ahTrialSignUpPage.GetValidationMessageList(), "Validation message list is not matched for existing user");
        }
    }

}

