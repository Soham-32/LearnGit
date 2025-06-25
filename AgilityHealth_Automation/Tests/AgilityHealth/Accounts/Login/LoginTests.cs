using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Accounts.Login
{
    [TestClass]
    [TestCategory("Accounts"), TestCategory("Login")]
    public class LoginTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Login_SuccessfulLogin()
        {
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();

            Assert.AreEqual(ApplicationUrl + "/account/login", Driver.GetCurrentUrl(), "Incorrect URL after login");

            login.LoginToApplication(User.Username, User.Password);

            Assert.AreEqual("Teams Dashboard", Driver.Title, "Window Title is incorrect after login");

        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Login_FailedLogin_And_ValidationMessages()
        {
            var login = new LoginPage(Driver, Log);

            const string invalidEmail = "fake@selenium.com";
            const string invalidPassword = "incorrect";

            login.NavigateToPage();
            Assert.AreEqual(ApplicationUrl + "/account/login", Driver.GetCurrentUrl(), "Incorrect URL after login");

            login.LoginToApplication(invalidEmail, "");
            Assert.AreEqual(Constants.PasswordValidationMessage, login.GetPasswordValidationErrorText(), "Validation text incorrect for blank password field");

            login.LoginToApplication(" ", invalidPassword);
            Assert.AreEqual(Constants.EmailValidationMessage, login.GetEmailValidationErrorText(), "Validation text incorrect for blank email field");

            login.LoginToApplication(" ", " ");
            Assert.AreEqual(Constants.EmailValidationMessage, login.GetEmailValidationErrorText(), "Validation text incorrect for blank email field");
            Assert.AreEqual(Constants.PasswordValidationMessage, login.GetPasswordValidationErrorText(), "Validation text incorrect for blank password field");

            login.LoginToApplication("test", invalidPassword);
            Assert.AreEqual("Error: The Email field is not a valid e-mail address.", login.GetEmailValidationErrorText(), "Validation text incorrect for invalid email");

            login.LoginToApplication(invalidEmail, invalidPassword);
            Assert.AreEqual(Constants.InvalidEmailAndPasswordValidationMessage, login.GetValidationErrorText(), "Validation text incorrect for invalid login");
        }

    }
}
