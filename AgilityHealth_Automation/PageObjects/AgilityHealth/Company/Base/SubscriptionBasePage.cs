using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Base
{
    internal class SubscriptionBasePage : BasePage
    {
        public SubscriptionBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        protected readonly By SubscriptionTypeListbox = AutomationId.Equals("subscriptionType", "div");

        protected static By SubscriptionListItem(string item) =>
            By.XPath($"//div[@id = 'menu-subscriptionType']//li[text() = '{item}']");
        protected readonly By ManagedSubscriptionCheckbox =
            AutomationId.Equals("managed-Subscription", "input");
        protected readonly By AssessmentLimitTextbox =
            AutomationId.Equals("assessment-limit", "input");
        protected readonly By TeamLimitTextbox = AutomationId.Equals("team-limit", "input");
        protected readonly By ContractSignedDate =
            AutomationId.Equals("dateContractSigned", "input");
        protected readonly By ContractEndDate = AutomationId.Equals("contractEndDate", "input");
        protected readonly By AccountManagerFirstNameTextbox =
            AutomationId.Equals("accountManagerFirstName", "input");
        protected readonly By AccountManagerLastNameTextbox =
            AutomationId.Equals("accountManagerLastName", "input");
        protected readonly By AccountManagerEmailTextbox =
            AutomationId.Equals("accountManagerEmail", "input");
        protected readonly By AccountManagerPhoneNumberTextbox =
            AutomationId.Equals("accountManagerPhone", "input");

        protected readonly By LicenseKeyAddButton = AutomationId.Equals("btnAddLicenseKey");

        protected static By SpecificLicenseKeyOriginTextBox(string origin) => By.XPath($"*//div/input[contains(@id, 'origin')][@value='{origin}']");
        protected static By SpecificLicenseKeyKeyTextBox(string key) => By.XPath($"*//div/input[contains(@id, 'key')][@value='{key}']");
        protected static By SpecificLicenseKeyQuantityTextBox(string quantity) => By.XPath($"*//div/input[contains(@id, 'quantity')][@value='{quantity}']");
        protected readonly By AllLicenseKeyOriginTextBoxes = By.XPath("*//div/input[contains(@id, 'origin')]");
        protected readonly By AllLicenseKeyKeyTextBoxes = By.XPath("*//div/input[contains(@id, 'key')]");
        protected readonly By AllLicenseKeyQuantityTextBoxes = By.XPath("*//div/input[contains(@id, 'quantity')]");
        protected readonly By FieldErrorMessage = By.XPath("//p[contains(@class,'Mui-error')]");
        protected static By ValidationText(string fieldName) => By.XPath($"//div[@automation-id='{FieldLocators[fieldName]}']//following-sibling::p");
        protected static readonly Dictionary<string, string> FieldLocators = new Dictionary<string, string>
        {
            { "Subscription Type", "subscriptionType" },
        };

        public void WaitUntilLoaded()
        {
            Wait.UntilElementVisible(SubscriptionTypeListbox);
        }


        public void FillSubscriptionInfo(AddCompanyRequest company)
        {
            Log.Step(GetType().Name, "Fill in Subscription Info");
            SelectItem(SubscriptionTypeListbox, SubscriptionListItem(company.SubscriptionType));
            Wait.UntilElementExists(ManagedSubscriptionCheckbox).Check(company.ManagedSubscription);

            if (company.AssessmentsLimit != 0)
                Wait.UntilElementClickable(AssessmentLimitTextbox).SetText(company.AssessmentsLimit.ToString(), isReact: true);
            if (company.TeamsLimit != 0)
                Wait.UntilElementClickable(TeamLimitTextbox).SetText(company.TeamsLimit.ToString(), isReact: true);
            if (company.DateContractSigned != null)
                Wait.UntilElementClickable(ContractSignedDate).SetText(company.DateContractSigned.Value.ToString("MM/dd/yyyy"));
            if (company.ContractEndDate != null)
                Wait.UntilElementClickable(ContractEndDate).SetText(company.ContractEndDate.Value.ToString("MM/dd/yyyy"));

        }

        public bool IsManagedSubscriptionCheckboxSelected()
        {
            return Wait.UntilElementExists(ManagedSubscriptionCheckbox).Selected;
        }


        public AddCompanyRequest GetSubscriptionInfo()
        {
            return new AddCompanyRequest
            {
                SubscriptionType = Wait.UntilElementClickable(SubscriptionTypeListbox).GetText(),
                ManagedSubscription = Wait.UntilElementExists(ManagedSubscriptionCheckbox).Selected,
                AssessmentsLimit = Wait.UntilElementClickable(AssessmentLimitTextbox).GetText().ToInt(),
                TeamsLimit = Wait.UntilElementClickable(TeamLimitTextbox).GetText().ToInt(),
                DateContractSigned = Wait.UntilElementClickable(ContractSignedDate).GetText().ToDateTime(),
                ContractEndDate = Wait.UntilElementClickable(ContractEndDate).GetText().ToDateTime()
            };
        }


        public void FillAccountManagerInfo(AddCompanyRequest company)
        {
            Log.Step(GetType().Name, "Fill in Account Manager Info");
            Wait.UntilElementClickable(AccountManagerFirstNameTextbox).SetText(company.AccountManagerFirstName, isReact: true);
            Wait.UntilElementClickable(AccountManagerLastNameTextbox).SetText(company.AccountManagerLastName, isReact: true);
            SetAccountManagerEmail(company.AccountManagerEmail);
            Wait.UntilElementClickable(AccountManagerPhoneNumberTextbox).SetText(company.AccountManagerPhone, isReact: true);
        }

        public void SetAccountManagerEmail(string email)
        {
            Log.Step(GetType().Name, "Set account manager email");
            Wait.UntilElementClickable(AccountManagerEmailTextbox).SetText(email, isReact: true);
        }

        public AddCompanyRequest GetAccountManagerInfo()
        {
            return new AddCompanyRequest
            {
                AccountManagerFirstName = Wait.UntilElementClickable(AccountManagerFirstNameTextbox).GetText(),
                AccountManagerLastName = Wait.UntilElementClickable(AccountManagerLastNameTextbox).GetText(),
                AccountManagerEmail = Wait.UntilElementClickable(AccountManagerEmailTextbox).GetText(),
                AccountManagerPhone = Wait.UntilElementClickable(AccountManagerPhoneNumberTextbox).GetText()
            };
        }

        public void FillLicenseKeyInfo(CompanyLicenseDto licenseInfo)
        {
            Log.Step(GetType().Name, "Fill in License Key Info");
            ClickLicenseKeyAddButton();
            SetLicenseKeyOriginText(licenseInfo);
            Wait.UntilElementClickable(SpecificLicenseKeyKeyTextBox("")).SetText(licenseInfo.Key);
            Wait.UntilElementClickable(SpecificLicenseKeyQuantityTextBox("25")).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(SpecificLicenseKeyQuantityTextBox("25")).SendKeys(licenseInfo.Quantity.ToString());
        }

        public void ClickLicenseKeyAddButton()
        {
            Log.Step(GetType().Name, "Click license key add button");
            Wait.UntilElementClickable(LicenseKeyAddButton).Click();
        }

        public void SetLicenseKeyOriginText(CompanyLicenseDto licenseInfo)
        {
            Log.Step(GetType().Name, "Input text in origin textbox");
            Wait.UntilElementClickable(SpecificLicenseKeyOriginTextBox("")).SetText(licenseInfo.Origin);
        }

        public bool DoesErrorMessageExist()
        {
            Log.Step(GetType().Name, "Check for error message/warning on a field on the page");
            return Driver.IsElementDisplayed(FieldErrorMessage);
        }

        public List<CompanyLicenseDto> GetLicenseKeyInfo()
        {
            var licenseKeyInfo = new List<CompanyLicenseDto>();
            var count = Driver.GetElementCount(AllLicenseKeyOriginTextBoxes);
            var origins = Driver.FindElements(AllLicenseKeyOriginTextBoxes);
            var keys = Driver.FindElements(AllLicenseKeyKeyTextBoxes);
            var quantities = Driver.FindElements(AllLicenseKeyQuantityTextBoxes);

            for (int i = 0; i < count; i++)
            {
                licenseKeyInfo.Add(new CompanyLicenseDto
                {
                    Origin = origins[i].GetText(),
                    Key = keys[i].GetText(),
                    Quantity = int.Parse(quantities[i].GetText())
                }
                );
            }
            return licenseKeyInfo;
        }

        public string GetFieldValidationMessage(string fieldName)
        {
            return Wait.UntilElementVisible(ValidationText(fieldName)).GetText();
        }
    }
}
