using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarDetailsBasePage : RadarHeaderBasePage
    {
        public RadarDetailsBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        //Radar details
        private readonly By RadarCompanyDropdown = By.XPath("//label[normalize-space()='Company']/parent::span/following-sibling::span/span");
        private static By SelectRadarCompanyName(string companyName) => By.XPath($"//div[@id='Survey_CompanyId-list'] | //ul[@id='CompanyId_listbox']//li[@role='option'][normalize-space()='{companyName}'] | //div[text()='{companyName}']");

        private readonly By RadarTypeDropdown = By.XPath("//label[normalize-space()='Radar Type']/parent::span/following-sibling::span/span");
        private static By SelectRadarTypeDropdown(string radar) => By.XPath($"//ul[@id='Survey_SurveyTypeId_listbox']//li[@role='option'][normalize-space()='{radar}']");
        private readonly By RadarNameTextbox = By.Id("Survey_Name");
        private readonly By RadarScaleDropdown = By.XPath("//label[normalize-space()='Scale']/parent::span/following-sibling::span/span");
        private static By SelectRadarScale(string value) => By.XPath($"//ul[@id='Survey_Scale_listbox']//li[@role='option'][normalize-space()='{value}']");
        private readonly By ShowAsAbsoluteCheckbox = By.Id("Survey_Absolute");
        private readonly By PublicCheckbox = By.Id("Survey_IsPublic");
        private readonly By Show1ResponseCheckbox = By.Id("Survey_ShowOneResponse");
        private readonly By IncludeInGlobalBenchmarkingCheckbox = By.Id("Survey_EnableBenchmark");
        private readonly By AppendStandardFooterCheckbox = By.Id("Survey_EmailMessageHasStandardFooter");
        private readonly By RadarLimitedToDropdown = By.XPath("//ul[@id='Survey_AvailableCompanies_taglist']//following-sibling::input");
        private static By SelectRadarLimitedToOptions(string value) => By.XPath($"//ul[@id='Survey_AvailableCompanies_listbox']//li[@role='option'][normalize-space()='{value}']");
        private readonly By AllLimitedToOptions = By.XPath("//ul[@id='Survey_AvailableCompanies_taglist']/li/span[not(@class)]");
        private readonly By RadarAvailableToDropdown = By.XPath("//ul[@id='Survey_AvailableUserRoles_taglist']//following-sibling::input");
        private static By SelectRadarAvailableToOptions(string value) => By.XPath($"//ul[@id='Survey_AvailableUserRoles_listbox']//li[@role='option'][normalize-space()='{value}']");
        private readonly By AllAvailableToOptions = By.XPath("//ul[@id='Survey_AvailableUserRoles_taglist']/li/span[not(@class)]");
        private readonly By RadarWorkTypeDropdown = By.XPath("//ul[@id='Survey_SurveyWorkTypes_taglist']//following-sibling::input");
        private static By SelectRadarWorkTypeOptions(string value) => By.XPath($"//ul[@id='Survey_SurveyWorkTypes_listbox']//li[@role='option'][normalize-space()='{value}']");
        private readonly By AllWorkTypeOptions = By.XPath("//ul[@id='Survey_SurveyWorkTypes_taglist']/li/span[not(@class)]");
        private readonly By RadarCopyrightTextbox = By.Id("Survey_Copyright");
        private readonly By DefaultRadar = By.Id("Survey_IsDefault");

        //Logo
        private readonly By RadarLogo = By.Id("avatar");
        private readonly By UploadDone = By.XPath("//strong[text()='Done']");
        private readonly By RadarImage = By.XPath("//div[@id='imgSrc']/img");

        //All Dropdown List
        private readonly By RadarCompanyAllValues = By.XPath("//ul[@id='Survey_CompanyId_listbox']//li[@role='option']");
        private readonly By RadarTypeAllValues = By.XPath("//ul[@id='Survey_SurveyTypeId_listbox']//li[@role='option']");
        private readonly By AvailableToAllValues = By.XPath("//ul[@id='Survey_AvailableUserRoles_listbox']//li[@role='option']");
        private readonly By LimitedToAllValues = By.XPath("//ul[@id='Survey_AvailableCompanies_listbox']//li[@role='option']");
        private readonly By WorkTypeAllValues = By.XPath("//ul[@id='Survey_SurveyWorkTypes_listbox']//li[@role='option']");
        private readonly By ScaleAllValues = By.XPath("//ul[@id='Survey_Scale_listbox']//li[@role='option']");

        //Email Messages

        //Default Language section
        private readonly By IframeBody = By.XPath("//body");
        private readonly By ActiveCheckbox = By.Id("Survey_Active");
        internal readonly By AssessmentWelcomeMessageIframe = By.XPath("//div[@id='teamSurveyMessage']//iframe");
        private readonly By SenderNameTextbox = By.Id("Survey_EmailMessageSenderName");
        private readonly By SubjectTextbox = By.Id("Survey_EmailMessageSubject");
        internal readonly By EmailWelcomeMessageIframe = By.XPath("//div[@id='teamEmailMessage']//iframe");
        internal readonly By ThankYouMessageIframe = By.XPath("//div[@id='teamThankYou']//iframe");

        //Translated Language section
        private readonly By TranslatedActiveCheckbox = By.Id("TranslatedActive");
        internal readonly By TranslatedAssessmentWelcomeMessageIframe = By.XPath("//div[@id='translatedTeamSurveyMessage']//iframe");
        private readonly By TranslatedSubjectTextbox = By.Id("Survey_TranslatedEmailMessageSubject");
        internal readonly By TranslatedEmailWelcomeMessageIframe = By.XPath("//div[@id='translatedTeamEmailMessage']//iframe");
        internal readonly By TranslatedThankYouMessageIframe = By.XPath("//div[@id='translatedTeamThankYou']//iframe");
        

        public void EnterRadarDetails(RadarDetails radarDetails)
        {
            Log.Step(nameof(RadarDetailsBasePage), "Enter Radar Details");

            //CompanyName Dropdown
            SelectRadarCompany(radarDetails.CompanyName);

            //RadarName Textbox
            if (!string.IsNullOrEmpty(radarDetails.Name))
            {
                Wait.UntilElementClickable(RadarNameTextbox).SetText(radarDetails.Name);
            }

            //Type Dropdown
            if (!string.IsNullOrEmpty(radarDetails.Type))
            {
                SelectRadarType(radarDetails.Type);
            }

            //RadarLogo
            UploadProfilePhoto(radarDetails.Logo);
            
            //Scale Dropdown
            SelectScale(radarDetails.Scale);

            //LimitedTo Dropdown
            if (Driver.IsElementDisplayed(RadarLimitedToDropdown) && radarDetails.LimitedTo.FirstOrDefault()!= null)
            {
                SelectLimitedTo(radarDetails.LimitedTo);
            }

            //AvailableTo Dropdown
            if (radarDetails.AvailableTo.FirstOrDefault() != null)
            {
                SelectAvailableTo(radarDetails.AvailableTo);
            }

            //WorkType Dropdown
            if(radarDetails.WorkType.FirstOrDefault() != null)
            {
                SelectWorkType(radarDetails.WorkType);
            }

            //Copyright Textbox
            Wait.UntilElementClickable(RadarCopyrightTextbox).SetText(radarDetails.CopyrightText);

            //ShowAsAbsoluteCheckbox
            Wait.UntilElementClickable(ShowAsAbsoluteCheckbox).Check(radarDetails.ShowAsAbsolute);
            
            //PublicCheckbox
            Wait.UntilElementClickable(PublicCheckbox).Check(radarDetails.Public);

            //Show1ResponseCheckbox
            Wait.UntilElementClickable(Show1ResponseCheckbox).Check(radarDetails.Show1Response);

            //IncludeInGlobalBenchmarkingCheckbox
            ClickOnGlobalBenchmarkingCheckbox(radarDetails.IncludeInGlobalBenchmarking);

            //ActiveCheckbox
            ClickOnActiveCheckboxButton(radarDetails.Active);

            //AppendStandardFooter
            Wait.UntilElementClickable(AppendStandardFooterCheckbox).Check(radarDetails.AppendStandardFooter);
        }
        public void ClickOnGlobalBenchmarkingCheckbox(bool select)
        {
            if (Driver.IsElementDisplayed(ActiveCheckbox))
            {
                Wait.UntilElementClickable(IncludeInGlobalBenchmarkingCheckbox).Check(select);
            }

        }
        public void ClickOnActiveCheckboxButton(bool select)
        {
            if (Driver.IsElementDisplayed(ActiveCheckbox))
            {
                Wait.UntilElementClickable(ActiveCheckbox).Check(select);
            }
        }

        public void EnterMessagesTextsDetails(RadarDetails radarDetails)
        {
            Log.Step(nameof(RadarDetailsBasePage), "Enter Messages Text Details");

            //AssessmentWelcomeMessage
            if (!string.IsNullOrEmpty(radarDetails.AssessmentWelcomeMessage))
            {
                Driver.SwitchToFrame(AssessmentWelcomeMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.AssessmentWelcomeMessage);
                Driver.SwitchTo().DefaultContent();
            }

            //EmailMessageSenderName
            if (!string.IsNullOrEmpty(radarDetails.EmailMessageSenderName))
            {
                Wait.UntilElementClickable(SenderNameTextbox).SetText(radarDetails.EmailMessageSenderName);
            }

            //EmailMessageSubject
            if (!string.IsNullOrEmpty(radarDetails.EmailMessageSubject))
            {
                Wait.UntilElementClickable(SubjectTextbox).SetText(radarDetails.EmailMessageSubject);
            }

            //EmailWelcomeMessage
            if (!string.IsNullOrEmpty(radarDetails.EmailWelcomeMessage))
            {
                Driver.SwitchToFrame(EmailWelcomeMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.EmailWelcomeMessage);
                Driver.SwitchTo().DefaultContent();
            }

            //ThankYouMessage
            if (!string.IsNullOrEmpty(radarDetails.ThankYouMessage))
            {
                Driver.SwitchToFrame(ThankYouMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.ThankYouMessage);
                Driver.SwitchTo().DefaultContent();
            }
        }

        public void EnterTranslations(RadarDetails radarDetails)
        {
            Log.Step(nameof(RadarDetailsBasePage),"Enter Translated Details");

            //TranslatedActiveCheckBox
            if (Driver.IsElementDisplayed(TranslatedActiveCheckbox))
            {
                Wait.UntilElementClickable(TranslatedActiveCheckbox).Check(radarDetails.TranslatedActive);
            }

            //TranslatedAssessmentWelcomeMessage
            if (!string.IsNullOrEmpty(radarDetails.TranslatedAssessmentWelcomeMessage))
            {
                Driver.SwitchToFrame(TranslatedAssessmentWelcomeMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.TranslatedAssessmentWelcomeMessage);
                Driver.SwitchTo().DefaultContent();
            }

            //TranslatedEmailMessageSubject
            if (!string.IsNullOrEmpty(radarDetails.TranslatedEmailMessageSubject))
            {
                Wait.UntilElementClickable(TranslatedSubjectTextbox).SetText(radarDetails.TranslatedEmailMessageSubject);
            }

            //TranslatedEmailWelcomeMessage
            if (!string.IsNullOrEmpty(radarDetails.TranslatedEmailWelcomeMessage))
            {
                Driver.SwitchToFrame(TranslatedEmailWelcomeMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.TranslatedEmailWelcomeMessage);
                Driver.SwitchTo().DefaultContent();
            }

            //TranslatedThankYouMessage
            if (!string.IsNullOrEmpty(radarDetails.TranslatedThankYouMessage))
            {
                Driver.SwitchToFrame(TranslatedThankYouMessageIframe);
                Wait.UntilElementExists(IframeBody).SetText(radarDetails.TranslatedThankYouMessage);
                Driver.SwitchTo().DefaultContent();
            }
        }

        public void SelectRadarCompany(string companyName)
        {
            if (companyName.Equals(SharedConstants.CompanyName))
            {
                if (GetRadarCompanyName().Equals(SharedConstants.CompanyName)) return;
            }
            Log.Step(nameof(RadarDetailsBasePage), $"Select the Radar Company Name : {companyName}");
            SelectItem(RadarCompanyDropdown, SelectRadarCompanyName(companyName));
        }

        private void SelectRadarType(string radarType)
        {
            Log.Step(nameof(RadarDetailsBasePage), $"Select the Radar Type : {radarType}");
            SelectItem(RadarTypeDropdown, SelectRadarTypeDropdown(radarType));
        }

        private void SelectScale(string scale)
        {
            Log.Step(nameof(RadarDetailsBasePage), $"Select the Radar Scale value : {scale}");
            SelectItem(RadarScaleDropdown, SelectRadarScale(scale));
        }

        private void SelectLimitedTo(List<string> value)
        {
            Log.Step(nameof(RadarDetailsBasePage), $"Select value {value} from 'Limited To' dropdown");
            foreach (var item in value)
            {
                SelectItem(RadarLimitedToDropdown, SelectRadarLimitedToOptions(item));
            }
        }

        private void SelectAvailableTo(List<string> value)
        {
            Log.Step(nameof(RadarDetailsBasePage), $"Select value {value} from 'Available To' dropdown");
            foreach (var item in value)
            {
                SelectItem(RadarAvailableToDropdown, SelectRadarAvailableToOptions(item));
            }
        }

        private void SelectWorkType(List<string> value)
        {
            Log.Step(nameof(RadarDetailsBasePage), $"Select value {value} from 'WorkType' dropdown");
            foreach (var item in value)
            {
                SelectItem(RadarWorkTypeDropdown, SelectRadarWorkTypeOptions(item));
            }
        }

        private void UploadProfilePhoto(string photoPath)
        {
            Log.Step(nameof(RadarDetailsBasePage), "Upload the Radar logo");
            Wait.UntilElementExists(RadarLogo).SetText(photoPath);
            Wait.UntilElementVisible(UploadDone);
        }

        public RadarDetails GetRadarDetails()
        {
            var radarCompanyName = GetRadarCompanyName();
            var radarType = Wait.UntilElementExists(RadarTypeDropdown).GetText().Replace("\r\nselect", "");
            var radarName = Wait.UntilElementVisible(RadarNameTextbox).GetText().Replace("\r\nselect", "");
            var scale = Wait.UntilElementExists(RadarScaleDropdown).GetText().Replace("\r\nselect", "");
            var showAsAbsolute = Wait.UntilElementVisible(ShowAsAbsoluteCheckbox).Selected;
            var publicCheckbox = Wait.UntilElementVisible(PublicCheckbox).Selected;
            var show1Response = Wait.UntilElementVisible(Show1ResponseCheckbox).Selected;
            var includeInGlobalBenchmarking = Driver.IsElementDisplayed(IncludeInGlobalBenchmarkingCheckbox) && Wait.UntilElementVisible(IncludeInGlobalBenchmarkingCheckbox).Selected;
            var active =Driver.IsElementDisplayed(ActiveCheckbox) && Wait.UntilElementVisible(ActiveCheckbox).Selected;
            var appendStandardFooter = Wait.UntilElementVisible(AppendStandardFooterCheckbox).Selected;
            const string radarLogo = "";
            var limitedTo = GetAllSelectedLimitedTo();
            var availableTo = GetAllSelectedAvailableTo();
            var workType = GetAllSelectedWorkType();
            var copyrightText = Wait.UntilElementVisible(RadarCopyrightTextbox).GetText().Replace("\r\nselect", "");
            var defaultAssessmentWelcomeMessage = GetDefaultAssessmentWelcomeMessage();
            var emailMessageSenderName = Driver.IsElementPresent(SenderNameTextbox) ? Wait.UntilElementVisible(SenderNameTextbox).GetText() : null;
            var defaultEmailMessageSubject = Driver.IsElementPresent(SubjectTextbox) ? Wait.UntilElementVisible(SubjectTextbox).GetText() : null;
            var defaultEmailWelcomeMessage = GetDefaultEmailWelcomeMessage(); 
            var defaultThankYouMessage = GetDefaultThankYouMessage();
            
            return new RadarDetails
            {
                CompanyName = radarCompanyName,
                ShowAsAbsolute = showAsAbsolute,
                Type = radarType,
                Name = radarName,
                Scale = scale,
                Public = publicCheckbox,
                Show1Response = show1Response,
                IncludeInGlobalBenchmarking = includeInGlobalBenchmarking,
                Active= active,
                AppendStandardFooter = appendStandardFooter,
                Logo= radarLogo,
                LimitedTo= limitedTo,
                AvailableTo= availableTo,
                WorkType= workType,
                CopyrightText= copyrightText,
                AssessmentWelcomeMessage= defaultAssessmentWelcomeMessage,
                EmailMessageSubject = defaultEmailMessageSubject,
                EmailWelcomeMessage= defaultEmailWelcomeMessage,
                ThankYouMessage= defaultThankYouMessage,
                EmailMessageSenderName= emailMessageSenderName,
            };
        }

        public RadarDetails GetTranslatedFields()
        {
            var radarLanguage = Wait.UntilElementExists(HeaderLanguageDropdown).GetText().Replace("\r\nselect", "");
            var translatedAssessmentWelcomeMessage = GetTranslatedAssessmentWelcomeMessage();
            var translatedEmailMessageSubject = Driver.IsElementPresent(TranslatedSubjectTextbox) ? Wait.UntilElementVisible(TranslatedSubjectTextbox).GetText() : null;
            var translatedEmailWelcomeMessage = GetTranslatedEmailWelcomeMessage();
            var translatedThankYouMessage = GetTranslatedThankYouMessage();
            
            return new RadarDetails()
            {
                Language= radarLanguage,
                TranslatedAssessmentWelcomeMessage = translatedAssessmentWelcomeMessage,
                TranslatedEmailWelcomeMessage = translatedEmailWelcomeMessage,
                TranslatedThankYouMessage = translatedThankYouMessage,
                TranslatedEmailMessageSubject = translatedEmailMessageSubject
            };
        }

        //All Dropdown List Get Methods
        public List<string> GetRadarCompanyDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Company' value from Company dropdown");
            Wait.UntilElementClickable(RadarCompanyDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getRadarCompanyAllValue = Driver.GetTextFromAllElements(RadarCompanyAllValues).ToList();
            Wait.UntilElementClickable(RadarCompanyDropdown).Click();
            return getRadarCompanyAllValue;
        }

        public List<string> GetRadarTypeDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Type' value from Radar Type dropdown");
            Wait.UntilElementClickable(RadarTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getRadarTypeAllValue = Driver.GetTextFromAllElements(RadarTypeAllValues).ToList();
            Wait.UntilElementClickable(RadarTypeDropdown).Click();
            return getRadarTypeAllValue;
        }

        public List<string> GetScaleDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Scale' value from Scale dropdown");
            Wait.UntilElementClickable(RadarScaleDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getScaleAllValue = Driver.GetTextFromAllElements(ScaleAllValues).ToList();
            Wait.UntilElementClickable(RadarScaleDropdown).SendKeys(Keys.Escape);
            return getScaleAllValue;
        }

        public List<string> GetLimitedToDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Limited To' value from Limited To dropdown");
            Wait.UntilElementClickable(RadarLimitedToDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getLimitedToAllValue = Driver.GetTextFromAllElements(LimitedToAllValues).ToList();
            Wait.UntilElementClickable(RadarLimitedToDropdown).SendKeys(Keys.Escape);
            return getLimitedToAllValue;
        }

        public List<string> GetAvailableToDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'Available To' value from Available To dropdown");
            Wait.UntilElementClickable(RadarAvailableToDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getAvailableToAllValue = Driver.GetTextFromAllElements(AvailableToAllValues).ToList();
            Wait.UntilElementClickable(RadarAvailableToDropdown).SendKeys(Keys.Escape);
            return getAvailableToAllValue;
        }

        public List<string> GetWorkTypeDropdownAllValues()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All 'WorkType' value from Work Type dropdown");
            Wait.UntilElementClickable(RadarWorkTypeDropdown).Click();
            Wait.UntilJavaScriptReady();
            var getWorkTypAllValue = Driver.GetTextFromAllElements(WorkTypeAllValues).ToList();
            GetScaleDropdownAllValues();
            return getWorkTypAllValue;
        }

        // Get Email Messages

        // Default Email Messages 
        private string GetDefaultAssessmentWelcomeMessage()
        {
             Log.Step(nameof(RadarDetailsBasePage), "Get default Assessment welcome message from 'Assessment welcome' textbox");
             Driver.SwitchToFrame(AssessmentWelcomeMessageIframe);
             var assessmentMessage = Wait.UntilElementExists(IframeBody).GetText();
             Driver.SwitchTo().DefaultContent();
             return assessmentMessage;
        }

        private string GetDefaultEmailWelcomeMessage()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get default Email welcome message from 'Email welcome' textbox");
            Driver.SwitchToFrame(EmailWelcomeMessageIframe);
            var emailMessage = Wait.UntilElementExists(IframeBody).GetText();
            Driver.SwitchTo().DefaultContent();
            return emailMessage;
        }

        private string GetDefaultThankYouMessage()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get default Thank you message from 'ThankYou message' textbox");
            Driver.SwitchToFrame(ThankYouMessageIframe);
            var thankYouMessage = Wait.UntilElementExists(IframeBody).GetText();
            Driver.SwitchTo().DefaultContent();
            return thankYouMessage;
        }

        // Translated Emails Messages
        private string GetTranslatedAssessmentWelcomeMessage()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get Translated Assessment welcome message from 'Assessment welcome' textbox");
            Driver.SwitchToFrame(TranslatedAssessmentWelcomeMessageIframe);
            var translatedAssessmentMessage = Wait.UntilElementExists(IframeBody).GetText();
            Driver.SwitchTo().DefaultContent();
            return translatedAssessmentMessage;
        }

        private string GetTranslatedEmailWelcomeMessage()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get Translated Email welcome message from 'Translated Email welcome' textbox");
            Driver.SwitchToFrame(TranslatedEmailWelcomeMessageIframe);
            var translatedEmailMessage = Wait.UntilElementExists(IframeBody).GetText().Replace("\r\nselect", "");
            Driver.SwitchTo().DefaultContent();
            return translatedEmailMessage;
        }

        private string GetTranslatedThankYouMessage()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get Translated Thank you message from 'Translated ThankYou message' textbox");
            Driver.SwitchToFrame(TranslatedThankYouMessageIframe);
            var translatedThankYouMessage = Wait.UntilElementExists(IframeBody).GetText();
            Driver.SwitchTo().DefaultContent();
            return translatedThankYouMessage;
        }

        //Get Radar Details
        public List<int> GetImageParameters()
        {
            var style = Wait.UntilElementVisible(RadarImage).GetAttribute("style").Split(';');
            var emptyString = string.Empty;
            var emptyList = new List<int>();
            foreach (var value in style)
            {
                emptyString = value.Where(t => char.IsDigit(t)).Aggregate(emptyString, (current, t) => current + t);
                if (emptyString.Length <= 0) continue;
                emptyList.Add(int.Parse(emptyString));
                emptyString = string.Empty;
            }
            return emptyList;
        }

        public string GetRadarCompanyName()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get Radar Company Name from 'Company' dropdown");
            return Wait.UntilElementExists(RadarCompanyDropdown).GetText().Replace("\r\nselect", "");
        }

        private List<string> GetAllSelectedLimitedTo()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All the selected value from 'Limited To' dropdown");
            if (Driver.IsElementDisplayed(RadarLimitedToDropdown) && Driver.GetTextFromAllElements(AllLimitedToOptions).ToList().Count == 0) return new List<string>() { null };
            return Driver.GetTextFromAllElements(AllLimitedToOptions).ToList();
        }

        private List<string> GetAllSelectedAvailableTo()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All the selected value from 'Available To' dropdown");
            return Driver.GetTextFromAllElements(AllAvailableToOptions).ToList().Count == 0 ? new List<string>() { null } : Driver.GetTextFromAllElements(AllAvailableToOptions).ToList();
        }

        private List<string> GetAllSelectedWorkType()
        {
            Log.Step(nameof(RadarDetailsBasePage), "Get All the selected value from 'WorkType' dropdown");
            return Driver.GetTextFromAllElements(AllWorkTypeOptions).ToList().Count == 0 ? new List<string>() { null } : Driver.GetTextFromAllElements(AllWorkTypeOptions).ToList();
        }

        // Is Email Messages
        public bool IsMessagesTextboxEnabled(By iFrameLocator)
        {
            Driver.SwitchToFrame(iFrameLocator);
            var messagesTextboxEnabled = Convert.ToBoolean(Wait.UntilElementClickable(IframeBody).GetAttribute("contenteditable"));
            Driver.SwitchTo().DefaultContent();
            return messagesTextboxEnabled;
        }

        // Default Emails Messages
        public bool IsSenderNameTextboxEnabled()
        {
            return Driver.IsElementEnabled(SenderNameTextbox);
        }

        public bool IsDefaultActiveCheckboxPresent()
        {
            return Driver.IsElementPresent(ActiveCheckbox);
        }

        public bool IsDefaultSubjectTextboxEnabled()
        {
            return Driver.IsElementEnabled(SubjectTextbox);
        }

        // Translated Emails Messages
        public bool IsTranslatedActiveCheckboxPresent()
        {
            return Driver.IsElementPresent(TranslatedActiveCheckbox);
        }

        public bool IsTranslatedSubjectTextboxEnabled()
        {
            return Driver.IsElementEnabled(TranslatedSubjectTextbox);
        }
        public bool IsLimitToFieldDisplayed()
        {
            return Driver.IsElementDisplayed(RadarLimitedToDropdown);
        }

        //Default Radar
        public void ClickOnDefaultRadarCheckbox(bool check = true)
        {
            Log.Step(nameof(RadarDetailsBasePage), "Click on 'Default Radar' checkbox");
            Wait.UntilElementClickable(DefaultRadar).Check(check);
        }
        public bool IsDefaultRadarFieldDisplayed()
        {
            return Driver.IsElementDisplayed(DefaultRadar);
        }
    }
}