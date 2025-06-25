using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Account
{
    internal class LoginPage : BasePage
    {
        public LoginPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private readonly By PageTitle = By.XPath("//div[contains(@class,'login-page-title')]/h1");
        private readonly By PageTitleDescription = By.XPath("//div[contains(@class,'login-page-title')]/p");
        private readonly By EmailText = By.XPath("//label[@for='Email']");
        private readonly By PasswordText = By.XPath("//label[@for='Password']");
        private readonly By ForgotPasswordLink = By.XPath("//div[contains(@class,'forgot-password')]");
        private readonly By KeepMeLoggedInText = By.XPath("//label[@class='remember-me-label']");
        private readonly By DoNotHaveAnAccountText = By.XPath("//div[contains(@class,'login-page-title') and @style]");

        //Location Popup
        private readonly By CountryMessage = By.Id("languageAutodectionCountryMessage");
        private readonly By LanguageMessage = By.Id("languageAutodectionLanguageMessage");

        private readonly By ConfirmPassword = By.Id("ConfirmPassword");

        private readonly By Email = By.XPath("//input[contains(@name,'Email')]");
        private readonly By LoginButton = By.XPath("//button[@type='submit'] | //input[@type='submit']");
        private readonly By Password = By.CssSelector("input[type='Password']");
        private readonly By SetPasswordButton = By.Id("set_password");
        private readonly By ValidationErrors = By.ClassName("validation-summary-errors");
        private readonly By EmailValidationErrors = By.Id("Email-error");
        private readonly By PasswordValidationErrors = By.Id("Password-error");

        //Location Popup
        public string GetCountryName()
        {
            Log.Step(nameof(LoginPage), "Get Country name");
            var completeMessage = Wait.UntilElementExists(CountryMessage).GetText();
            if (!completeMessage.Contains("South Korea"))
                return completeMessage.Replace(".", "").Split(' ').ToList().LastOrDefault();
            var messageList = completeMessage.Replace(".", "").Split(' ').ToList();
            return $"{messageList[messageList.Count - 2]} {messageList[messageList.Count - 1]}";
        }

        public string GetLanguage()
        {
            Log.Step(nameof(LoginPage), "Get Language");
            var completeMessage = Wait.UntilElementVisible(LanguageMessage).GetText();
            return completeMessage.Replace("?", "").Split(' ').ToList().LastOrDefault();
        }

        public string GetPageTitleText()
        {
            Log.Step(nameof(LoginPage), "Get page title");
            return Wait.UntilElementVisible(PageTitle).GetText();
        }

        public string GetPageTitleDescriptionText()
        {
            Log.Step(nameof(LoginPage), "Get page title text");
            return Wait.UntilElementVisible(PageTitleDescription).GetText();
        }

        public string GetEmailText()
        {
            Log.Step(nameof(LoginPage), "Get Email text");
            return Wait.UntilElementVisible(EmailText).GetText();
        }

        public string GetPasswordText()
        {
            Log.Step(nameof(LoginPage), "Get Password text");
            return Wait.UntilElementVisible(PasswordText).GetText();
        }

        public string GetLoginButtonText()
        {
            Log.Step(nameof(LoginPage), "Get Login Button text");
            return Wait.UntilElementVisible(LoginButton).GetAttribute("value");
        }

        public string GetForgotPasswordText()
        {
            Log.Step(nameof(LoginPage), "Get Forgot Password text");
            return Wait.UntilElementVisible(ForgotPasswordLink).GetText();
        }

        public string GetKeepMeLoggedInText()
        {
            Log.Step(nameof(LoginPage), "Get Keep me logged In text");
            return Wait.UntilElementVisible(KeepMeLoggedInText).GetText();
        }
        public string GetDoNotHaveAnAccountText()
        {
            Log.Step(nameof(LoginPage), "Get Don't Have An Account text");
            return Wait.UntilElementVisible(DoNotHaveAnAccountText).GetText();
        }

        //Login to application
        public void LoginToApplication(string username, string password)
        {
            Log.Step(nameof(LoginPage), $"Login to application: Username = <{username}>, Password = <{password}>");
            Wait.UntilElementVisible(Email).SetText(username);
            Wait.UntilElementVisible(Password).SetText(password);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(LoginButton).Click();
            Wait.HardWait(5000); //Wait till homepage is loading due to SSO 
        }

        public void SetUserPassword(string password)
        {
            Log.Step(nameof(LoginPage), $"Set and confirm password: <{password}>");
            Wait.UntilElementVisible(Password).SetText(password);
            Wait.UntilElementVisible(ConfirmPassword).SetText(password);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SetPasswordButton).Click();
        }

        public string GetValidationErrorText()
        {
            return Wait.InCase(ValidationErrors)?.GetText() ?? "";
        }

        public string GetEmailValidationErrorText()
        {
            return Wait.InCase(EmailValidationErrors)?.GetText() ?? "";
        }
        public string GetPasswordValidationErrorText()
        {
            return Wait.InCase(PasswordValidationErrors)?.GetText() ?? "";
        }

        public void WaitForPageToLoad()
        {
            Log.Step(nameof(LoginPage), "Wait for log in page to load");
            Wait.UntilElementClickable(Email);
            Wait.UntilElementClickable(Password);
        }

        public bool AreOnLoginPage()
        {
            Log.Step(nameof(LoginPage), "Verify on login page");
            return Driver.IsElementDisplayed(Email);
        }

        public void NavigateToPage()
        {
            var envName = BaseTest.TestEnvironment.EnvironmentName;

            if (envName == "riyadh" || envName == "mecca" || envName == "chennai" || envName == "ahmedabad" || envName == "omaha" || envName == "texas")
            {
                NavigateToUrl($"{BaseTest.ApplicationUrl}/account/remotelogin");
            }
            else
            {
                NavigateToUrl($"{BaseTest.ApplicationUrl}/account/login");
            }
        }
        public void NavigateToLoginPage(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/account/login");
        }
        public void NavigateToRemoteLoginPage(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/account/remotelogin");
        }
        public void NavigateToSaDomainLoginPage(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.sa/account/login");
        }
        public void NavigateToSaDomainRemoteLoginPage(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.sa/account/remotelogin");
        }
    }
}