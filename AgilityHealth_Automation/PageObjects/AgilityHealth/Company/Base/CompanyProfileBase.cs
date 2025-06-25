using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Users;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base
{
    public class CompanyProfileBase : BasePage
    {
        public CompanyProfileBase(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        internal readonly By LogoChooseFileButton = By.CssSelector("input[type='file']");
        internal readonly By LogoImage = AutomationId.Equals("logourl");
        internal readonly By CompanyNameTextBox = AutomationId.Equals("companyName", "input");
        internal readonly By CountryOfHeadquartersListBox = AutomationId.Equals("companyhq", "div");
        internal readonly By PreferredLanguage = AutomationId.Equals("companyLanguage", "div");
        internal static By CountryOfHeadquartersListItem(string item) =>
            By.XPath($"//div[@id = 'menu-country']//li[text() = '{item}']");

        internal readonly By CompanySizeListBox = AutomationId.Equals("companySize", "div");
        internal static By CompanySizeListItem(string item) =>
            By.XPath($"//div[@id = 'menu-size']//li[text() = '{item}']");

        internal readonly By IndustryListBox = AutomationId.Equals("companyIndustry", "div");

        internal static By IndustryListItem(string item) =>
            By.XPath($"//div[@id = 'menu-industryId']//*[text() = '{item}']");

        internal static By PreferredLanguageListItem(string item) =>
            By.XPath($"//div[@id='menu-isoLanguageCode']//li[text()='{item}']");
        private readonly By AllLanguagesList = By.XPath("//div[@id='menu-isoLanguageCode']//ul/li[@role='option']");

        internal readonly By PreferredTimezoneListBox = AutomationId.Equals("companyTimezone", "div");
        internal static By PreferredTimezoneListItem(string item) =>
            By.XPath($"//div[@id = 'menu-timeZoneInfoId']//li[@data-value = '{item}']");

        internal readonly By LifeCycleStageListBox = AutomationId.Equals("companyLifeCycle", "div");
        internal static By LifeCycleStageListItem(string item) => By.XPath($"//div[@id = 'menu-lifeCycleStage']//li[contains(normalize-space(), '{item}')]");

        internal readonly By CompanyAdminFirstNameTextBox = AutomationId.Equals("adminFirstName", "input");
        internal readonly By CompanyAdminLastNameTextBox = AutomationId.Equals("adminLastName", "input");
        internal readonly By CompanyAdminEmailTextBox = AutomationId.Equals("adminEmail", "input");
        internal readonly By CompanyAdminEmailValidationMessage = By.XPath("//*[@automation-id='adminEmail']/following-sibling::p");
        internal readonly By CompanyTypeListBox = AutomationId.Equals("companyType", "div");
        internal static By CompanyTypeListItem(string item) => By.XPath($"//div[@id = 'menu-companyType']//li[contains(normalize-space(), '{item}')]");


        internal readonly By AddToWatchlistCheckbox = By.Name("watchList");
        internal readonly By SelectPartnerReferralCheckbox = By.Name("hasPartnerReferral");
        internal readonly By ReferralTypeCompanyRadiobutton = By.CssSelector("input[value = 'company']");
        internal readonly By ReferralTypeIndividualRadiobutton = By.CssSelector("input[value = 'individual']");
        internal readonly By PartnerReferralListbox = AutomationId.Equals("companyPartnerReferral-input");

        internal static By PartnerReferralListItem(string item) => By.XPath($"//div[@id = 'menu-companyPartnerReferral']//li[contains(normalize-space(), '{item}')]");
        internal readonly By IndividualReferralTextbox = By.Id("individualPartnerReferral");
        internal static By ValidationText(string fieldName) => By.XPath($"//div[@automation-id='{FieldLocators[fieldName]}']//following-sibling::p");
        internal static readonly Dictionary<string, string> FieldLocators = new Dictionary<string, string>
        {
            { "Company Name", "companyName" },
            { "Country of Headquarters", "companyhq" },
            { "Company Size", "companySize" },
            { "Industry", "companyIndustry" },
            { "Preferred Timezone", "companyTimezone" },
            { "Life Cycle Stage", "companyLifeCycle" },
            { "First Name", "adminFirstName" },
            { "Last Name", "adminLastName" },
            { "Email", "adminEmail" },
            { "Company Type", "companyType" },
            { "Subscription Type", "subscriptionType" },
        };

        public void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(CompanyNameTextBox);
        }

        public void FillInCompanyProfileInfo(AddCompanyRequest company)
        {
            Log.Step(GetType().Name, "Enter Company info");
            if (!string.IsNullOrEmpty(company.Logourl))
            {
                Wait.UntilElementExists(LogoChooseFileButton).SetText(company.Logourl, clear: false);
            }

            Wait.UntilElementClickable(CompanyNameTextBox).SetText(company.Name, isReact: true);
            SelectCountry(company.Country);
            SelectCompanySize(company.Size);
            SelectItem(IndustryListBox, IndustryListItem(company.Industry));
            SelectItem(PreferredTimezoneListBox, PreferredTimezoneListItem(company.TimeZoneInfoId));
            SelectItem(PreferredLanguage, PreferredLanguageListItem(company.IsoLanguageCode));
            if (!string.IsNullOrEmpty(company.LifeCycleStage))
                SelectLifeCycleStage(company.LifeCycleStage);
            Wait.UntilElementClickable(CompanyAdminFirstNameTextBox)
                .SetText(company.CompanyAdminFirstName, isReact: true);
            Wait.UntilElementClickable(CompanyAdminLastNameTextBox)
                .SetText(company.CompanyAdminLastName, isReact: true);
            Wait.UntilElementClickable(CompanyAdminEmailTextBox)
                .SetText(company.CompanyAdminEmail, isReact: true);
        }

        public void SelectCompanySize(string companySize)
        {
            Log.Step(GetType().Name, $"Select company size <{companySize}>");
            SelectItem(CompanySizeListBox, CompanySizeListItem(companySize));
        }

        public void EnterCompanyAdminEmailInfo(string email, int waitInSeconds)
        {
            Log.Step(GetType().Name, $"Enter Company Admin Email <{email}>");
            Wait.UntilElementClickable(CompanyAdminEmailTextBox)
                .SetText(email, isReact: true).SendKeys(Keys.Tab);
            Wait.HardWait(waitInSeconds * 1000); //as API takes time to return validation message
        }

        public string GetCompanyAdminEmailValidationMessage()
        {
            return Wait.UntilElementVisible(CompanyAdminEmailValidationMessage).GetText();
        }

        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(CompanyProfileBase), "Get All Languages from Language dropdown");
            Driver.JavaScriptScrollToElement(PreferredLanguage);
            Wait.UntilElementClickable(PreferredLanguage).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(PreferredLanguageListItem("English")).Click();
            return getHeaderLanguageAllValue;
        }

        public void SelectLifeCycleStage(string lifeCycle)
        {
            Log.Step(GetType().Name, $"Select Life Cycle stage <{lifeCycle}>");
            SelectItem(LifeCycleStageListBox, LifeCycleStageListItem(lifeCycle));
        }

        public void FillInAdminInfo(AddCompanyRequest company)
        {
            Log.Step(GetType().Name, "Enter Admin info");
            SelectCompanyType(company.CompanyType);
            Wait.UntilElementEnabled(AddToWatchlistCheckbox).Check(company.WatchList);

            if (string.IsNullOrWhiteSpace(company.ReferralType)) return;
            Driver.JavaScriptScrollToElement(Wait.UntilElementEnabled(SelectPartnerReferralCheckbox)).Check();
            if (company.ReferralType.ToLower() == "company")
            {
                Wait.UntilElementEnabled(ReferralTypeCompanyRadiobutton).Click();
                if (!string.IsNullOrWhiteSpace(company.CompanyPartnerReferral))
                    SelectItem(PartnerReferralListbox, PartnerReferralListItem(company.CompanyPartnerReferral));
            }
            else if (company.ReferralType.ToLower() == "individual")
            {
                Wait.UntilElementEnabled(ReferralTypeIndividualRadiobutton).Click();
                if (!string.IsNullOrWhiteSpace(company.IndividualPartnerReferral))
                    Wait.UntilElementClickable(IndividualReferralTextbox).SetText(company.IndividualPartnerReferral);
            }
        }

        public void SelectCompanyType(string companyType)
        {
            Log.Step(GetType().Name, $"Select company type <{companyType}>");
            SelectItem(CompanyTypeListBox, CompanyTypeListItem(companyType));
        }

        public void SelectCountry(string country)
        {
            Log.Step(GetType().Name, $"Select country <{country}>");
            SelectItem(CountryOfHeadquartersListBox, CountryOfHeadquartersListItem(country));
        }

        public CompanyResponse GetCompanyProfile()
        {
            var name = Wait.UntilElementVisible(CompanyNameTextBox).GetText();
            var country = Wait.UntilElementVisible(CountryOfHeadquartersListBox).GetText();
            var size = Wait.UntilElementVisible(CompanySizeListBox).GetText();
            var industry = Wait.UntilElementVisible(IndustryListBox).GetText();
            var timeZoneInfoId = Wait.UntilElementVisible(PreferredTimezoneListBox).GetText();
            var lifeCycleStage = Wait.UntilElementVisible(LifeCycleStageListBox).GetText();
            var logoUrl = Wait.UntilElementExists(LogoImage).GetAttribute("src");
            var preferredLanguage = Wait.UntilElementExists(PreferredLanguage).GetText();
            return new CompanyResponse
            {
                Name = name,
                Country = country,
                Size = size,
                Industry = industry,
                TimeZoneInfoId = timeZoneInfoId,
                LifeCycleStage = lifeCycleStage,
                Logourl = logoUrl,
                IsoLanguageCode = preferredLanguage
            };
        }

        public UserResponse GetCompanyAdmin()
        {
            return new UserResponse
            {
                FirstName = Wait.UntilElementVisible(CompanyAdminFirstNameTextBox).GetText(),
                LastName = Wait.UntilElementVisible(CompanyAdminLastNameTextBox).GetText(),
                Email = Wait.UntilElementVisible(CompanyAdminEmailTextBox).GetText()
            };

        }


        public CompanyResponse GetWatchListInfo()
        {
            var company = new CompanyResponse
            {
                Type = Wait.UntilElementVisible(CompanyTypeListBox).GetText(),
                WatchList = Wait.UntilElementExists(AddToWatchlistCheckbox).Selected
            };

            var selectPartnerReferral = Wait.UntilElementExists(SelectPartnerReferralCheckbox).Selected;
            if (!selectPartnerReferral) return company;

            company.ReferralType = Wait.InCase(By.XPath(
                                           "//input[@name = 'referralType']//ancestor::span[contains(@class, 'Mui-checked')]//input"))
                                       ?.GetAttribute("value") ?? string.Empty;
            switch (company.ReferralType)
            {
                case "company":
                    company.CompanyPartnerReferral = Wait.UntilElementVisible(PartnerReferralListbox).GetText();
                    break;
                case "individual":
                    company.IndividualPartnerReferral = Wait.UntilElementVisible(IndividualReferralTextbox).GetText();
                    break;
            }
            return company;
        }

        public bool IsPreferredLanguageDisplayed()
        {
            return Driver.IsElementDisplayed(PreferredLanguage);
        }

        public string GetFieldValidationMessage(string fieldName)
        {
            return Wait.UntilElementVisible(ValidationText(fieldName)).GetText();
        }
    }
}
