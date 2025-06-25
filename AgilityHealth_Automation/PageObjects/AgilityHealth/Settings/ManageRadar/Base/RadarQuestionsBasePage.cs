using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarQuestionsBasePage : RadarGridBasePage
    {
        public RadarQuestionsBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By ExportToExcelButton = By.XPath("//button[text()='Export to Excel']");
        private readonly By AddEditQuestionPopupCompetencyDropdownValidationMessage = By.Id("competencies_validationMessage");
        private readonly By AddEditQuestionPopupCompetencyDropdown = By.XPath("//input[@id='competencies']//preceding-sibling::span");
        private static By AddEditSelectQuestionPopupCompetencyDropdown(string competency) => By.XPath($"//ul[@id='competencies_listbox']//li[@role='option'][normalize-space()='{competency}']");
        private readonly By AddEditQuestionPopupScaleOverrideDropdown = By.XPath("//span[@aria-owns='ScaleOverride_listbox']");
        private static By AddEditQuestionPopupSelectScaleOverrideDropdown(string scale) => By.XPath($"//ul[@id='ScaleOverride_listbox']//li[@role='option'][normalize-space()='{scale}']");
        private readonly By AddEditQuestionPopupQuantitativeMetricCheckbox = By.Id("Quantitative");
        private static By AddEditQuestionPopupQuestionTextIframe => By.XPath("//div[normalize-space()='Question Text']//following-sibling::div//iframe");
        private static By AddEditQuestionPopupBodyIframe => By.XPath("//body");
        private static By AddEditQuestionPopupQuestionHelpIframe => By.XPath("//div[normalize-space()='Question Help']//following-sibling::div//iframe");

        private static readonly By AddEditQuestionPopupAdvanceOptionsButton = By.Id("openAdvancedOptions");
        private readonly By AddEditQuestionPopupQuestionTagsDropdown = By.XPath("//input[@aria-owns='SelectedOptions_taglist SelectedOptions_listbox']");
        private static By AddEditQuestionPopupSelectQuestionTagsDropdown(string questionTag) => By.XPath($"//ul[@id='SelectedOptions_listbox']//li[@role='option'][normalize-space()='{questionTag}']");
        private readonly By AddEditQuestionPopupExcludeRolesDropdown = By.XPath("//input[@aria-owns='ExcludeRoleList_taglist ExcludeRoleList_listbox']");
        private static By AddEditQuestionPopupSelectExcludeRolesDropdown(string excludeRole) => By.XPath($"//ul[@id='ExcludeRoleList_listbox']//li[@role='option'][normalize-space()='{excludeRole}']");
        private readonly By AddEditQuestionPopupParticipantTagsDropdown = By.XPath("//span[@aria-owns='ExcludeByTag_listbox']");
        private static By AddEditQuestionPopupSelectParticipantTagsDropdown(string option) => By.XPath($"//ul[@id='ExcludeByTag_listbox']//li[@role='option'][normalize-space()='{option}']");
        private readonly By AddEditQuestionPopupParticipantTagsOptionsDropdown = By.XPath("//input[@aria-owns='ParticipantTags_taglist ParticipantTags_listbox']");
        private static By AddEditQuestionPopupSelectParticipantTagsOptionsDropdown(string participantTag) => By.XPath($"//ul[@id='ParticipantTags_listbox']//li[@role='option'][normalize-space()='{participantTag}']");

        private readonly By TranslatedAddEditQuestionPopupCompetencyDropdown = By.Id("TranslatedCompetencyName");
        private static By TranslatedAddEditQuestionPopupQuestionTextIframe => By.XPath("//div[normalize-space()='Question Text' and contains(@class,'translated')]//following-sibling::div//iframe");
        private static By TranslatedAddEditQuestionPopupQuestionHelpIframe => By.XPath("//div[normalize-space()='Question Help' and contains(@class,'translated')]//following-sibling::div//iframe");
        private static By QuestionEditIconByIndex(int index) => By.XPath($"//tbody/tr[{index}]//td//span[contains(@class,'k-edit')]");
        private readonly By QuestionTextCleanFormattingButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//span[@class = 'k-tool-icon k-cleanFormatting']");
        private readonly By TranslatedTextCleanFormattingButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//span[@class = 'k-tool-icon k-cleanFormatting']");
        private readonly By UpdateButton = By.XPath("//a[contains(normalize-space(), 'Update')]");
        private readonly By QuestionEditPopupTranslatedQuestionText = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//following::tr[1]/td/iframe");
        private readonly By QuestionEditPopupQuestionText = By.XPath("//ul[contains(@aria-controls, 'Text')]//following::tr[1]/td/iframe[contains(@class, 'k-content')]");
        private readonly By QuestionEditPopupQuestionTextItalicButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//a[contains(@title,'Italic')]");
        private readonly By QuestionEditPopupTranslatedTextItalicButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//a[contains(@title,'Italic')]");
        private readonly By QuestionEditPopupQuestionTextBoldButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//a[contains(@title,'Bold')]");
        private readonly By QuestionEditPopupTranslatedTextBoldButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//a[contains(@title,'Bold')]");
        private readonly By QuestionEditPopupQuestionTextUnderlineButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//a[contains(@title,'Underline')]");
        private readonly By QuestionEditPopupTranslatedTextUnderlineButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//a[contains(@title,'Underline')]");
        private readonly By QuestionEditPopupQuestionTextAlignLeftButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//a[contains(@title,'Align text left')]");
        private readonly By QuestionEditPopupTranslatedTextAlignLeftButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//a[contains(@title,'Align text left')]");
        private readonly By QuestionEditPopupQuestionTextUnorderedListButton = By.XPath("//ul[contains(@aria-controls, 'Text')]//a[contains(@title,'Insert unordered list')]");
        private readonly By QuestionEditPopupTranslatedTextUnorderedListButton = By.XPath("//ul[contains(@aria-controls, 'TranslatedText')]//a[contains(@title,'Insert unordered list')]");
        private readonly By QuestionEditPopupQuestionDescriptionText = By.XPath("//textarea[@id = 'Text']/preceding-sibling::iframe");
        private readonly By QuestionEditPopupTranslatedQuestionDescriptionText = By.XPath("//textarea[@id = 'TranslatedText']/preceding-sibling::iframe");
        private readonly By UnderlinePresent = By.XPath("//body/ul//*[contains(@style, 'text-decoration:underline;')]");
        private readonly By AlignLeftTextPresent = By.XPath("//body//li");
        private readonly By UnorderedListTextPresent = By.XPath("//body/ul/li");


        internal void EnterQuestionInfo(RadarQuestionResponse radarQuestion)
        {
            Log.Step(nameof(RadarCompetenciesBasePage), "Enter Questions Info");

            //Dimension 
            SelectDimension(radarQuestion.Dimension);

            //Sub Dimension 
            SelectSubDimension(radarQuestion.SubDimension);

            //Competency 
            SelectItem(AddEditQuestionPopupCompetencyDropdown, AddEditSelectQuestionPopupCompetencyDropdown(radarQuestion.Competency));
            
            //Scale Override
            if (!string.IsNullOrEmpty(radarQuestion.ScaleOverride))
                SelectItem(AddEditQuestionPopupScaleOverrideDropdown, AddEditQuestionPopupSelectScaleOverrideDropdown(radarQuestion.ScaleOverride));

            //Quantitative Metric
            Wait.UntilElementClickable(AddEditQuestionPopupQuantitativeMetricCheckbox).Check();

            //Question Text
            EnterQuestionTextDescription(radarQuestion.QuestionText);

            //Question Help
            if (!string.IsNullOrEmpty(radarQuestion.QuestionHelp))
                EnterQuestionHelpDescription(radarQuestion.QuestionHelp);

            //Advance Options
            Wait.UntilElementClickable(AddEditQuestionPopupAdvanceOptionsButton).Click();

            //WorkType
            if (!string.IsNullOrEmpty(radarQuestion.WorkType))
            {
                SelectWorkTypeFilter(radarQuestion.WorkTypeFilter);
                SelectWorkType(radarQuestion.WorkType);
            }

            //Methodology
            if (!string.IsNullOrEmpty(radarQuestion.Methodology))
            {
                SelectMethodologyFilter(radarQuestion.MethodologyFilter);
                SelectMethodology(radarQuestion.Methodology);
            }

            //Company
            if (!string.IsNullOrEmpty(radarQuestion.Company))
            {
                SelectCompanyFilter(radarQuestion.CompanyFilter);
                SelectCompany(radarQuestion.Company);
            }

            //Question Tags
            if (!string.IsNullOrEmpty(radarQuestion.QuestionTags))
                SelectItem(AddEditQuestionPopupQuestionTagsDropdown, AddEditQuestionPopupSelectQuestionTagsDropdown(radarQuestion.QuestionTags));

            //Exclude Roles
            if (!string.IsNullOrEmpty(radarQuestion.ExcludeRoles))
                SelectItem(AddEditQuestionPopupExcludeRolesDropdown, AddEditQuestionPopupSelectExcludeRolesDropdown(radarQuestion.ExcludeRoles));

            //Participant Tags
            if (string.IsNullOrEmpty(radarQuestion.ParticipantTagsFilter)) return;
            SelectItem(AddEditQuestionPopupParticipantTagsDropdown, AddEditQuestionPopupSelectParticipantTagsDropdown(radarQuestion.ParticipantTagsFilter));
            SelectItem(AddEditQuestionPopupParticipantTagsOptionsDropdown, AddEditQuestionPopupSelectParticipantTagsOptionsDropdown(radarQuestion.ParticipantTags));
        }
        
        internal void EnterTranslatedQuestionInfo(Question radarQuestion)
        {
            Log.Step(nameof(RadarCompetenciesBasePage), "Enter Translated Questions Info");
            EnterTranslatedQuestionTextDescription(radarQuestion.QuestionText);
            EnterTranslatedQuestionHelpDescription(radarQuestion.QuestionHelp);
        }

        public bool IsTranslatedCompetencyNameEnabled()
        {
            return Driver.IsElementEnabled(TranslatedAddEditQuestionPopupCompetencyDropdown);
        }
        
        public void ClickOnExportToExcelButton()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Click on Export To Excel Button");
            Wait.UntilElementClickable(ExportToExcelButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ClickOnQuestionEditButtonByIndex(int index)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Click on Question 'Edit' Button");
            Wait.UntilElementClickable(QuestionEditIconByIndex(index)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectAllQuestionTextAndApplyAllStyles(string language)
        {
            if (language.Equals("English"))
            {
                ClickOnQuestionTextCleanFormattingButton();
                SelectAllTheQuestionTextForBold();
                SelectAllTheQuestionTextForItalic();
                SelectAllTheQuestionTextForUnderline();
                SelectAllTheQuestionForApplyUnorderedList();
                SelectAllTheQuestionForAlignLeftText();
            }
            else
            {
                ClickOnTranslatedQuestionTextCleanFormattingButton();
                SelectTheTranslatedQuestionTextForBold();
                SelectTheTranslatedQuestionTextForItalic();
                SelectAllTheTranslatedQuestionTextForUnderline();
                SelectAllTheTranslatedQuestionForApplyUnorderedList();
                SelectAllTheTranslatedQuestionForAlignLeftText();
            }
        }

        private void SelectAllTheQuestionTextForBold()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the question text for applied bold style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupQuestionTextBoldButton).Click();
        }

        private void SelectAllTheQuestionTextForItalic()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the question text for applied italic style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupQuestionTextItalicButton).Click();
        }

        private void SelectTheTranslatedQuestionTextForBold()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select the translated question text for applied bold  style");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedTextBoldButton).Click();
        }
        private void SelectTheTranslatedQuestionTextForItalic()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select the translated question text for applied italic style");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedTextItalicButton).Click();
        }

        private void SelectAllTheQuestionTextForUnderline()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the question text for applied underline style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionDescriptionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupQuestionTextUnderlineButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SelectAllTheTranslatedQuestionTextForUnderline()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the translated question text for applied underline style");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedTextUnderlineButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SelectAllTheQuestionForAlignLeftText()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the question text for applied align left text style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupQuestionTextAlignLeftButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SelectAllTheTranslatedQuestionForAlignLeftText()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the translated question text for applied align left text style");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedTextAlignLeftButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SelectAllTheQuestionForApplyUnorderedList()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the question text for applied unordered list style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupQuestionTextUnorderedListButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void SelectAllTheTranslatedQuestionForApplyUnorderedList()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Select all the translated question text for apply unordered list");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedTextUnorderedListButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnCleanFormattingButton(string language)
        {
            (language.Equals("English") ? (Action)ClickOnQuestionTextCleanFormattingButton : ClickOnTranslatedQuestionTextCleanFormattingButton)();
        }

        public void ClickOnQuestionTextCleanFormattingButton()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Click on question text 'Clean Formatting Button' for remove applied style");
            Wait.UntilElementClickable(QuestionEditPopupQuestionDescriptionText).Click();
            Wait.UntilElementClickable(QuestionEditPopupQuestionDescriptionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(QuestionTextCleanFormattingButton).DoubleClick(Driver);
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnTranslatedQuestionTextCleanFormattingButton()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Click on translated question text 'Clean Formatting Button' for remove applied style");
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionDescriptionText).Click();
            Wait.UntilElementClickable(QuestionEditPopupTranslatedQuestionDescriptionText).SendKeys(Keys.Control + "a");
            Wait.UntilElementClickable(TranslatedTextCleanFormattingButton).DoubleClick(Driver);
            Wait.UntilJavaScriptReady();
        }

        public bool IsQuestionTextContainsBoldTag(string language)
        {
            Driver.SwitchToFrame(language.Equals("English") ? QuestionEditPopupQuestionText : QuestionEditPopupTranslatedQuestionText);
            var isStrongTagPresent = Driver.IsElementPresent(By.TagName("strong"));
            Driver.SwitchTo().DefaultContent();
            return isStrongTagPresent;
        }

        public bool IsQuestionTextContainsItalicTag(string language)
        {
            Driver.SwitchToFrame(language.Equals("English") ? QuestionEditPopupQuestionText : QuestionEditPopupTranslatedQuestionText);
            var isItalicTagPresent = Driver.IsElementPresent(By.TagName("em"));
            Driver.SwitchTo().DefaultContent();
            return isItalicTagPresent;
        }

        public bool DoesQuestionTextUnderlineTag(string language)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Does Question text contains the Underline tag");
            Driver.SwitchToFrame(language.Equals("English") ? QuestionEditPopupQuestionText : QuestionEditPopupTranslatedQuestionText);
            var underlinePresentLists = Driver.FindElements(UnderlinePresent);
            if (underlinePresentLists.Count.Equals(0))
            {
                var underlineList = Driver.IsElementPresent(By.TagName("ul"));
                Driver.SwitchTo().DefaultContent();
                return underlineList;
            }
            else
            {
                var underlineList = underlinePresentLists.All(u => u.GetAttribute("style").Contains("text-decoration: underline;"));
                Driver.SwitchTo().DefaultContent();
                return underlineList;
            }
        }

        public bool DoesQuestionTextContainAlignLeftText(string language)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Does question text contains align left text");
            Driver.SwitchToFrame(language.Equals("English") ? QuestionEditPopupQuestionText : QuestionEditPopupTranslatedQuestionText);
            var textAlignLeftLists = Driver.FindElements(AlignLeftTextPresent);
            if (textAlignLeftLists.Count.Equals(0))
            {
                var textAlignLeft = Driver.IsElementPresent(By.TagName("ul"));
                Driver.SwitchTo().DefaultContent();
                return textAlignLeft;
            }
            else
            {
                var textAlignLeft = textAlignLeftLists.All(w => w.GetAttribute("style").Contains("text-align: left;"));
                Driver.SwitchTo().DefaultContent();
                return textAlignLeft;
            }
        }

        public bool DoesQuestionTextContainsUnorderedList(string language)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Does question text contains unordered list");
            Driver.SwitchToFrame(language.Equals("English") ? QuestionEditPopupQuestionText : QuestionEditPopupTranslatedQuestionText);
            var unorderedListTexts = Driver.FindElements(UnorderedListTextPresent);
            if (unorderedListTexts.Count.Equals(0))
            {
                var unorderedList = Driver.IsElementPresent(By.TagName("ul"));
                Driver.SwitchTo().DefaultContent();
                return unorderedList;
            }
            else
            {
                var unorderedList = unorderedListTexts.All(w => w.TagName.Contains("li"));
                Driver.SwitchTo().DefaultContent();
                return unorderedList;
            }
        }

        public void ClickOnQuestionEditUpdateButton()
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Click on question edit 'Update' button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }

        private void EnterQuestionTextDescription(string text)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Enter value in 'Question Text' Description");
            Driver.SwitchToFrame(AddEditQuestionPopupQuestionTextIframe);
            Wait.UntilElementClickable(AddEditQuestionPopupBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }

        private void EnterQuestionHelpDescription(string text )
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Enter value in 'Question Help' Description");
            Driver.SwitchToFrame(AddEditQuestionPopupQuestionHelpIframe);
            Wait.UntilElementClickable(AddEditQuestionPopupBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }

        public bool IsCompetencyDropdownEnabled()
        {
            return !Wait.UntilElementClickable(AddEditQuestionPopupCompetencyDropdown).GetAttribute("class")
                .Contains("disabled");
        }

        public bool IsCompetencyDropdownValidationMessageDisplayed()
        {
            return Driver.IsElementDisplayed(AddEditQuestionPopupCompetencyDropdownValidationMessage);
        }

        private void EnterTranslatedQuestionTextDescription(string text)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Enter value in 'Question Text' Description");
            Driver.SwitchToFrame(TranslatedAddEditQuestionPopupQuestionTextIframe);
            Wait.UntilElementClickable(AddEditQuestionPopupBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }

        private void EnterTranslatedQuestionHelpDescription(string text)
        {
            Log.Step(nameof(RadarQuestionsBasePage), "Enter value in 'Question Help' Description");
            Driver.SwitchToFrame(TranslatedAddEditQuestionPopupQuestionHelpIframe);
            Wait.UntilElementClickable(AddEditQuestionPopupBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }
    }
}