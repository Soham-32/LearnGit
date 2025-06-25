using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.AhTrial;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.AhTrial
{
    [TestClass]
    [TestCategory("AhTrial")]
    public class AhTrialSignUpFormCompanyCreationTests : BaseTest
    {
        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 47507
        public void SignUp_Form_Verify_Successfully_SignUp()
        {
            var ahTrialSignUpFormPage = new AhTrialSignUpFormPage(Driver, Log);
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info("Navigate to the 'Sign Up' form and verify title");
            ahTrialSignUpFormPage.NavigateToPage();
            Assert.IsTrue(ahTrialSignUpFormPage.IsTitleTextDisplayed(), "'Sign up' page title is not displayed");

            Log.Info("Fill sign up form with valid data and verify that user navigate to Team dashboard");
            var signUpData = AhTrialFactory.GetValidAhTrialSignUpFormInfo();
            ahTrialSignUpFormPage.EnterSignUpInfo(signUpData);
            ahTrialSignUpFormPage.ClickOnSignUpButton();
            Assert.AreEqual("Teams Dashboard - AH", Driver.Title, "Window Title is incorrect after login");

            Log.Info("Logout and navigate to Login as a SA user");
            topNav.LogOut();
            Assert.AreEqual(ApplicationUrl + "/account/login", Driver.GetCurrentUrl(), "Login url is not matched");
            login.LoginToApplication(User.Username, User.Password);
            companyDashboardPage.WaitUntilLoaded();

            Log.Info("Search for the created Company and verify that it is displayed");
            companyDashboardPage.Search(signUpData.CompanyName);
            Assert.IsTrue(companyDashboardPage.IsCompanyPresent(signUpData.CompanyName), $"Company {signUpData.CompanyName} is not present");
            Assert.AreEqual("Trial", companyDashboardPage.GetCompanyDetail(signUpData.CompanyName, "Subscript"), "Subscript does not match");
            Assert.AreEqual(signUpData.FirstName + " " + signUpData.LastName, companyDashboardPage.GetCompanyDetail(signUpData.CompanyName, "Comp Admin"), "Company Admin does not match");
            Assert.AreEqual(signUpData.Industry, companyDashboardPage.GetCompanyDetail(signUpData.CompanyName, "Industry"), "Industry does not match");
        }
    }
}

