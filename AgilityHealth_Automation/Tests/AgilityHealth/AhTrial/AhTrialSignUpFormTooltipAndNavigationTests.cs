using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.AhTrial;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial")]
    public class AhTrialSignUpFormTooltipAndNavigationTests : BaseTest
    {
        [TestMethod]
        public void AhTrial_SignUpForm_Verify_NavigationLinks()
        {
            var ahTrialSignUpPage = new AhTrialSignUpFormPage(Driver, Log);

            Log.Info("Navigate to the 'Sign Up' form");
            ahTrialSignUpPage.NavigateToPage();

            Log.Info("Click on 'Login Here' link and verify user should be navigating to the 'Login' page");
            ahTrialSignUpPage.ClickOnLoginHereLink();
            Assert.AreEqual("https://atqa.agilityinsights.ai/account/login", Driver.GetCurrentUrl(), "Login url is not matched"); //to be done when developer fixes environment to qa

            Log.Info("Navigate to 'SignUp' form");
            ahTrialSignUpPage.NavigateToPage();

            Log.Info("Click on 'Ah license Agreement' link and verify user should be navigating to the 'Ah License Agreement' page");
            ahTrialSignUpPage.ClickOnAhLicenseAgreementLink();
            Driver.SwitchToLastWindow();
            Assert.AreEqual("https://agilityinsights.ai/software-licensing-agreement/", Driver.GetCurrentUrl(), "Ah License Agreement url is not matched");
        }

        [TestMethod]
        public void AhTrial_SignUpForm_Verify_TooltipMessagesAndDefaultSignUpButton()
        {
            var ahTrialSignUpPage = new AhTrialSignUpFormPage(Driver, Log);

            Log.Info("Navigate to the 'Sign Up' form, verify 'Sign Up' button is disable and verify tool tip message for 'Enterprise Trial', 'Small Business' & 'Company Name' fields");
            ahTrialSignUpPage.NavigateToPage();
            Assert.IsTrue(ahTrialSignUpPage.IsTitleTextDisplayed(), "'Sign Up' form title is not displayed");
            Assert.IsFalse(ahTrialSignUpPage.IsSignUpButtonEnabled(), "'Sign Up' button is enabled");
            Assert.AreEqual("For larger enterprises who want to run a pilot or proof of concept.", ahTrialSignUpPage.GetEnterpriseTrialTooltipMessage(), "Tooltip message is not matched for Enterprise Trial");
            Assert.AreEqual("For independent consultants or small company.", ahTrialSignUpPage.GetSmallBusinessTooltipMessage(), "Tooltip message is not matched for Small Business");
            Assert.AreEqual("This is what your company will be called in AgilityHealth.", ahTrialSignUpPage.GetCompanyNameTooltipMessage(), "Tooltip message is not matched for Company Name");
        }
    }
}


