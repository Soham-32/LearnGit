using OpenQA.Selenium;
using AgilityHealth_Automation.Utilities;
using AgilityHealth_Automation.Base;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.AhTrial.Custom;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.AhTrial
{
    internal class AhTrialSignUpFormPage : BasePage
    {
        public AhTrialSignUpFormPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Header
        private readonly By TitleText = By.XPath("//div[contains(@class,'login-page-title')]/h1");
        private readonly By LoginHereLink = By.XPath("//a[contains(text(),'Login here')]");

        //Form
        private readonly By FirstNameTextbox = By.Id("FirstName");
        private readonly By LastNameTextbox = By.Id("LastName");
        private readonly By CompanyEmailTextbox = By.Id("Email");
        private readonly By CompanyNameTextbox = By.Id("Company");
        private readonly By PhoneNumberTextbox = By.Id("Phone");
        private readonly By CountryDropdown = By.XPath("//span[@aria-owns='Country_listbox']");
        private static By CountryListItem(string country) => By.XPath($"//ul[@id='Country_listbox']//li[text()='{country}']");
        private readonly By IndustryDropdown = By.XPath("//span[@aria-owns='Industry_listbox']");
        private static By IndustryListItem(string industry) => By.XPath($"//ul[@id='Industry_listbox']//li[text()='{industry}']");
        private readonly By PasswordTextbox = By.Id("Password");
        private readonly By ReEnterPasswordTextbox = By.Id("ReEnterPassword");
        private readonly By CaptchaCheckboxIframe = By.XPath("//iframe[@title='reCAPTCHA']");
        private readonly By CaptchaCheckbox = By.Id("recaptcha-anchor");
        private readonly By AhLicenseAgreementCheckbox = By.Id("RememberMe");
        private readonly By AhLicenseAgreementLink = By.XPath("//a[contains(text(),'AgilityHealth Software Licensing Agreement')]");
        private readonly By SignUpButton = By.Id("submit");

        //Tooltip
        private readonly By EnterpriseTrialTooltip = By.Id("adduserTooltipE");
        private readonly By SmallBusinessTooltip = By.Id("adduserTooltip");
        private readonly By CompanyNameTooltip = By.Id("addCompanyTooltipE");

        private readonly By EnterpriseTrialTooltipMessage = By.XPath("//div[@class='k-tooltip-content' and contains(text(),'For larger enterprises')]");
        private readonly By SmallBusinessTooltipMessage = By.XPath("//div[@class='k-tooltip-content' and contains(text(),'For independent')]");
        private readonly By CompanyNameTooltipMessage = By.XPath("//div[@class='k-tooltip-content' and contains(text(),'This is what your company')]");

        //Validation  message
        private readonly By ValidationMessageList = By.XPath("//span[contains(@id,'Validate') and (@style='font-size: 12px;' or @style='') or @style='display: inline;']");
        
        public void EnterSignUpInfo(AhTrialSignUpForm signUpForm)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter Sign up info");

            //FirstName
            EnterFirstName(signUpForm.FirstName);

            //LastName         
            EnterLastName(signUpForm.LastName);

            //Email
            EnterCompanyEmail(signUpForm.Email);

            //CompanyName
            EnterCompanyName(signUpForm.CompanyName);

            //PhoneNumber
            EnterPhoneNumber(signUpForm.PhoneNumber);

            //Select Country
            SelectCountry(signUpForm.Country);

            //Select Industry
            SelectIndustry(signUpForm.Industry);

            //Password
            EnterPassword(signUpForm.Password);

            //reEnterPassword
            ReEnterPassword(signUpForm.ReEnterPassword);

            //CAPTCHA             
            ClickOnCAPTCHACheckbox();

            //'AgilityHealth Software Licensing Agreement' checkbox
            ClickOnAhLicenseAgreementCheckbox();
        }

        //Header
        public bool IsTitleTextDisplayed()
        {
            return Driver.IsElementDisplayed(TitleText);
        }
        public void ClickOnLoginHereLink()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Click on 'Login here' link");
            Wait.UntilElementClickable(LoginHereLink).Click();
        }

        //Form
        public void EnterFirstName(string firstName)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'FirstName' in textbox");
            Wait.UntilElementClickable(FirstNameTextbox).SetText(firstName);
        }
        public void EnterLastName(string lastName)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'LastName' in textbox");
            Wait.UntilElementClickable(LastNameTextbox).SetText(lastName);
        }
        public void EnterCompanyEmail(string email)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'CompanyEmail' in textbox");
            Wait.UntilElementClickable(CompanyEmailTextbox).SetText(email);
        }
        public void EnterCompanyName(string companyName)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'CompanyName' in textbox");
            Wait.UntilElementClickable(CompanyNameTextbox).SetText(companyName);
        }
        public void EnterPhoneNumber(string phoneNumber)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'PhoneNumber' in textbox");
            Wait.UntilElementClickable(PhoneNumberTextbox).SetText(phoneNumber);
        }
        public void SelectCountry(string country)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Select country from 'Country' dropdown list");
            SelectItem(CountryDropdown, CountryListItem(country));
        }
        public void SelectIndustry(string industry)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Select industry from 'Industry' dropdown list");
            SelectItem(IndustryDropdown, IndustryListItem(industry));
        }
        public void EnterPassword(string password)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'Password' in textbox");
            Wait.UntilElementClickable(PasswordTextbox).SetText(password);
        }
        public void ReEnterPassword(string reEnterPassword)
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Enter 'ReEnterPassword' in textbox");
            Wait.UntilElementClickable(ReEnterPasswordTextbox).SetText(reEnterPassword);
            Wait.HardWait(2000); //Need to wait until captcha loaded
        }
        public void ClickOnCAPTCHACheckbox()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Click on 'CAPTCHA' checkbox button");
            Driver.SwitchToFrame(CaptchaCheckboxIframe);
            Wait.UntilElementClickable(CaptchaCheckbox).Check();
            Driver.SwitchTo().DefaultContent();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnAhLicenseAgreementCheckbox()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Click on 'AhLicenseAgreement' checkbox button");
            Wait.UntilElementClickable(AhLicenseAgreementCheckbox).Check();
        }
        public void ClickOnAhLicenseAgreementLink()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Click on 'AhLicenseAgreement' link");
            Wait.UntilElementClickable(AhLicenseAgreementLink).Click();
        }
        public bool IsSignUpButtonEnabled()
        {
            return Driver.IsElementEnabled(SignUpButton);
        }
        public void ClickOnSignUpButton()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Click on 'SignUp' button");
            Wait.UntilElementClickable(SignUpButton).Click();
            Wait.UntilJavaScriptReady();
        }

        //Tooltip
        public string GetEnterpriseTrialTooltipMessage()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Hover over the 'Enterprise Trial' tooltip and get tooltip message");
            Driver.MoveToElement(Wait.UntilElementClickable(EnterpriseTrialTooltip));
            return Wait.UntilElementVisible(EnterpriseTrialTooltipMessage).GetText();
        }
        public string GetSmallBusinessTooltipMessage()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Hover over the 'Small Business' tooltip and get the text of tooltip");
            Driver.MoveToElement(Wait.UntilElementClickable(SmallBusinessTooltip));
            return Wait.UntilElementVisible(SmallBusinessTooltipMessage).GetText();
        }
        public string GetCompanyNameTooltipMessage()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Hover over the'Company Name' tooltip and get the text of tooltip");
            Driver.MoveToElement(Wait.UntilElementClickable(CompanyNameTooltip));
            return Wait.UntilElementVisible(CompanyNameTooltipMessage).GetText();
        }

        //Validation Error message
        public List<string> GetValidationMessageList()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Get the list of validation messages");
            return Wait.UntilAllElementsLocated(ValidationMessageList).Select(a => Driver.JavaScriptScrollToElement(a).GetText()).ToList();
        }

        public void NavigateToPage()
        {
            Log.Step(nameof(AhTrialSignUpFormPage), "Navigate to SignUp form");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/agilityhealth/signup");
        }
        public void NavigateToAhTrialPageForProd(string env)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/agilityhealth/signup");
        }
    }
}
