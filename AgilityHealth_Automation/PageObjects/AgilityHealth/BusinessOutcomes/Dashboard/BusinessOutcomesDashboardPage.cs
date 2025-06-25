using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard
{
    public class BusinessOutcomesDashboardPage : BasePage
    {
        public BusinessOutcomesDashboardPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Locators
        #region ColumnSection
        private readonly By ColumnNames = By.XPath("//button[contains(@class, 'add__businessoutcome')]/ancestor::div/h1");
        private static By DynamicPlusButton(string columnName) => By.XPath($"//h1[@title='{columnName}']/preceding-sibling::button");
        private readonly By PlusButton = By.XPath("//button[@id='0']//*[local-name()='svg' and @data-icon='plus-circle' ]");
        private static By DynamicBusinessOutcomeNameLink(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']");
        private static By DynamicBusinessOutcomeId(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']/ancestor::p/preceding-sibling::div//p");
        private static By DynamicBusinessOutcomeCollapseIcon(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']/ancestor::div[contains(@class,'businessoutcomes__card')]//div[contains(@class,'expand__button')]//*[contains(@data-icon,'angle')]");
        private static By DynamicBusinessOutcomeLeftVerticalStripe(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']/ancestor::div[contains(@class,'businessoutcomes__card')]/div");
        private static By DynamicOverallPercentageValue(string columnName, string businessOutcomeName) => By.XPath($"//*[@title='{columnName}']/ancestor::div//*[text()='{businessOutcomeName}']/ancestor::p/../div[3]/p");
        private static By DynamicOverallProgressBar(string columnName, string businessOutcomeName) => By.XPath($"//*[@title='{columnName}']/ancestor::div//*[text()='{businessOutcomeName}']/ancestor::p/parent::span//div[@role='progressbar']/div");
        private static By BusinessOutcomeNames(string columnName) => By.XPath($"//h1[contains(normalize-space(),'{columnName}')]/parent::div/parent::div//p[contains(@class,'edit__businessoutcome')]");

        private static By CardTitleBySwimLane(string swimLane, string title) => By.XPath(
            $"//div[@id = 'swimlanesWrapper']/div//h1[@title = '{swimLane}']/../..//*[text() = '{title}']");
        private static By DynamicKeyResultPercentageValue(string businessOutcomeName, string keyResultName) => By.XPath($"//*[text()='{businessOutcomeName}']//ancestor::div[contains(@class,'businessoutcomes__card')]/div[2]/div[2]/div[contains(normalize-space(),'{keyResultName}')]//p");
        private static By DynamicKeyResultPercentageValue(string keyResultName) => By.XPath($"//div[@id='swimlanesWrapper']//div[(text()='{keyResultName}' or font//font[text()='{keyResultName}'])]//p");
        private static By DynamicKeyResultImpactIcon(string businessOutcomeName, string keyResultName) => By.XPath($"//*[contains(normalize-space(),'{businessOutcomeName}')]//ancestor::div[contains(@class,'businessoutcomes__card')]//div[contains(normalize-space(),'{keyResultName}')]//span//*[@data-icon='bullseye-arrow']");
        private static By DynamicKeyResultProgressBar(string businessOutcomeName, string keyResultName) => By.XPath($"//*[text()='{businessOutcomeName}']//ancestor::div[contains(@class,'businessoutcomes__card')]/div[2]/div[2]/div[contains(normalize-space(),'{keyResultName}')]//span[@role='progressbar']/span");

        private static By DynamicBusinessOutcomeExternalLinkButton(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']//ancestor::div[contains(@class,'businessoutcomes__card')]//button[contains(@id,'linkingStart')]");
        private static By DynamicBusinessOutcomeExternalLinkBadgeIndicator(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']//ancestor::div[contains(@class,'businessoutcomes__card')]//button[contains(@id,'linkingStart')]//div[contains(@class,'external__links__badge__indicator')]");
        private static By ExternalLinkFlyoutLink(string linkText) => By.XPath($"//div[@role = 'presentation']//a[contains(normalize-space(), '{linkText}')]");

        private static By DynamicBusinessOutcomeCopyButton(string businessOutcomeName) => By.XPath($"//*[text()='{businessOutcomeName}']//ancestor::div[contains(@class,'businessoutcomes__card')]//button[contains(@id,'copy')]");

        private static By DynamicBusinessOutcomeCategory(string columnName, string businessOutcomeName, string categoryName) =>
            By.XPath($"//h1[contains(normalize-space(),'{columnName}')]/parent::div/following-sibling::div[3]//p/div[contains(normalize-space(),'{businessOutcomeName}')]//ancestor::div[contains(@class,'businessoutcomes__card')]//div/span[text()='{categoryName}']");



        #endregion

        #region SearchFilter
        private readonly By SearchCardsTextbox = By.XPath("//input[@placeholder='Search Cards']");
        private readonly By FilterDropdown = By.Id("businessoutcomes__categoryFilterChevron");
        private static By CategoryValueCheckbox(string categoryName) =>
            By.XPath($"//div[@id='businessoutcomes__categoriesFilterDropdownList']//p[contains(normalize-space(), '{categoryName}')]/preceding-sibling::span//input");
        private readonly By FilterOkButton = By.Id("businessoutcomes__categoryOkButton");
        private readonly By FilterClearButton = By.Id("businessoutcomes__categoryClearButton");


        #endregion

        #region ColorFilter

        //Color Filter
        private readonly By ColorFilterButton = By.Id("color__filter__button");
        private static By ColorFilterColorSelection(string colorHexValue) => By.CssSelector($"div[title='Select {colorHexValue}']");
        private static By ColorFilterColorSelected(string colorHexValue) => By.XPath($"//div//div[@title='Select {colorHexValue}']//div/*[local-name()='svg' and @data-icon='check']");
        private readonly By ColorFilterClearButton = By.CssSelector("div[title*='Clear']");

        #endregion

        #region TagsView

        //Tags View
        private readonly By TagsViewTagDropdown = By.Id("businessoutcomes__view__selector");
        private readonly By TagsViewAllTagsList = By.XPath("//ul[@role='listbox']//li[@role='option']");
        private static By TagListItem(string item) =>
            By.XPath($"//li[@role='option'][text() = '{item}'] | //li[@role='option']//font[text() = '{item}']");

        #endregion

        #region CardTypeDropdown
        private readonly By CardTypeDropdown = By.XPath("//div[@id='filterContainer']//div[(@aria-haspopup='listbox') and not (@id='businessoutcomes__view__selector')]");
        private readonly By CardTypeTagDropdown = By.XPath("//div[@id='businessoutcomes__view__selector']");
        private readonly By CardTypeList = By.XPath("//ul[@role='listbox']//li[@role='option']");

        private static By CardTypeOption(string cardType) => By.XPath(
            $"//li[@role='option'][text() = '{cardType}'] | //li[@role='option']//font[text() = '{cardType}']");

        #endregion

        #region ExportToExcelPDF
        //Export To Excel
        private readonly By ExportToExcelButton = By.CssSelector("button.export__excel");

        //Export To PDF
        private readonly By ExportToPdfButton = By.CssSelector("button.export__pdf");


        #endregion

        #region SettingIcon
        private readonly By SettingsIcon = By.XPath("//div[@id='filterContainer']//button[@type='button']//*[name()='svg' and @name='Settings']");
        #endregion

        #region DuplicateCardPopup
        //Duplicate Card Pop up
        private readonly By DuplicateCardTeamTextBox = By.XPath("//div[@id='copycard_select_team']//div[text()='Search Team']");
        private readonly By DuplicateCardTeamInput = By.XPath("//div[@id='copycard_select_team']//input");
        private static By DuplicateCardTeamName(string teamName) =>
            By.XPath($"//div[@id='copycard_select_team']//div[text()='{teamName}']");
        private readonly By DuplicateCardColumnInput = By.XPath("//div[@id='copycard_select_column']//input");
        private readonly By DuplicateCardSaveButton = By.Id("copycard_saveBtn");

        private readonly By LoadingIcon = By.CssSelector("MuiCircularProgress-root");

        #endregion
        #endregion


        #region Methods

        #region ColumnSection
        //Column section
        public List<string> GetAllColumnNames()
        {
            return Wait.UntilAllElementsLocated(ColumnNames).Select(column => column.GetText()).ToList();
        }

        public void ClickOnPlusButton(string columnName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on the '+' button for column <{columnName}>");
            Wait.UntilElementClickable(DynamicPlusButton(columnName)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsPageLoadedCompletely(string columnName = "3 Year Outcomes")
        {
            if (!Driver.IsElementDisplayed(DynamicPlusButton(columnName), 60))
            {
                //If page not loaded, refreshing page and giving it a second chance
                Driver.RefreshPage();
            }

            Wait.HardWait(2000); // Waiting for label to display
            return Driver.IsElementDisplayed(DynamicPlusButton(columnName), 60);
        }

        public void WaitTillBoPageLoadedCompletely(string columnName = "3 Year Outcomes")
        {
            var element = Wait.InCase(DynamicPlusButton(columnName), 5);
            WaitForReload();
            if (element != null) return;

            //If page not loaded, refreshing page and giving it a second chance
            Driver.RefreshPage();
            Wait.InCase(DynamicPlusButton(columnName), 5);
            WaitForReload();
        }


        public void ClickOnBusinessOutcomeLink(string businessOutcomeName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on Business Outcome <{businessOutcomeName}>");
            Wait.UntilElementVisible(PlusButton);
            Wait.UntilElementClickable(SearchCardsTextbox).ClearTextbox();
            Wait.UntilElementVisible(SearchCardsTextbox).SetText(businessOutcomeName).SendKeys(Keys.Enter);
            Wait.UntilElementVisible(DynamicBusinessOutcomeNameLink(businessOutcomeName));
            Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeNameLink(businessOutcomeName), false);
            Wait.UntilElementVisible(DynamicBusinessOutcomeNameLink(businessOutcomeName)).Click();
            Wait.HardWait(3000); // need to wait till all data loads
        }

        public void ClickOnCardExpandCollapseButton(string businessOutcomeName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on Expand/Collapse button for Business Outcome <{businessOutcomeName}>");
            Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeCollapseIcon(businessOutcomeName)).Click();
            // the bottom may now have expanded below the visible area, so scroll it into view again
            Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeCollapseIcon(businessOutcomeName), false);
            Wait.UntilElementVisible(DynamicBusinessOutcomeCollapseIcon(businessOutcomeName));
        }

        public bool IsExpandCollapseIconVisible(string businessOutcomeName)
        {
            return Wait.InCase(DynamicBusinessOutcomeCollapseIcon(businessOutcomeName)) != null && Wait.InCase(DynamicBusinessOutcomeCollapseIcon(businessOutcomeName)).Displayed;
        }

        public string GetColorInfoForLeftVerticalStripe(string businessOutcomeName)
        {
            return Driver.JavaScriptScrollToElement(Wait.UntilElementClickable(DynamicBusinessOutcomeLeftVerticalStripe(businessOutcomeName))).GetElementAttribute("style");
        }

        public bool IsBusinessOutcomePresent(string businessOutcomeName)
        {
            if (Wait.InCase(DynamicBusinessOutcomeNameLink(businessOutcomeName)) != null)
            {
                return Driver.JavaScriptScrollToElement(Wait.InCase(DynamicBusinessOutcomeNameLink(businessOutcomeName))).Displayed;
            }
            return false;
        }

        public bool IsCardPresentInSwimLane(string swimLane, string cardTitle)
        {
            return Driver.IsElementPresent(CardTitleBySwimLane(swimLane, cardTitle));
        }

        public string GetBusinessOutcomeIdText(string businessOutcomeExactName)
        {
            return Wait.UntilElementVisible(Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeId(businessOutcomeExactName))).GetText();
        }

        public string GetOverallPercentageValue(string columnName, string businessOutcomeName)
        {
            Driver.JavaScriptScrollToElement(DynamicOverallPercentageValue(columnName, businessOutcomeName), false);
            return Wait.UntilElementVisible(DynamicOverallPercentageValue(columnName, businessOutcomeName)).GetText();
        }

        public string GetOverallProgressIndication(string columnName, string businessOutcomeName)
        {
            Driver.JavaScriptScrollToElement(DynamicOverallProgressBar(columnName, businessOutcomeName), false);
            return Wait.UntilElementVisible(DynamicOverallProgressBar(columnName, businessOutcomeName)).GetElementAttribute("style");
        }

        public string GetOverallProgressBarColor(string columnName, string businessOutcomeName)
        {
            Driver.JavaScriptScrollToElement(DynamicOverallProgressBar(columnName, businessOutcomeName));
            return Wait.UntilElementVisible(DynamicOverallProgressBar(columnName, businessOutcomeName)).GetCssValue("background-color");
        }

        public void DragCardToCard(string columnName, string cardTitle1, string cardTitle2, int xOffset, int yOffset)
        {
            var initialOrder = GetAllBusinessOutcomeNamesByColumn(columnName);
            var initialIndex1 = initialOrder.IndexOf(cardTitle1);
            var initialIndex2 = initialOrder.IndexOf(cardTitle2);

            for (var attempt = 0; attempt < 5; attempt++)
            {
                var card1 = Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeNameLink(cardTitle1));
                var card2 = Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeNameLink(cardTitle2));
                Driver.DragElementToElement(card1, card2, xOffset, yOffset);

                Thread.Sleep(1000); // Small wait to allow DOM update

                var updatedOrder = GetAllBusinessOutcomeNamesByColumn(columnName);
                if (updatedOrder.IndexOf(cardTitle1) == initialIndex2 &&
                    updatedOrder.IndexOf(cardTitle2) == initialIndex1)
                {
                    return; // success
                }
            }
        }

        public List<string> GetAllBusinessOutcomeNamesByColumn(string columnName)
        {
            Wait.UntilJavaScriptReady();
            var boItemPresent = Driver.IsElementPresent(BusinessOutcomeNames(columnName));
            return boItemPresent ? Wait.UntilAllElementsLocated(BusinessOutcomeNames(columnName)).Select(e => Driver.JavaScriptScrollToElement(e).GetText()).ToList() : new List<string>();
        }

        public string GetKeyResultPercentageValue(string businessOutcomeName, string keyResultName)
        {
            return Driver.JavaScriptScrollToElement(DynamicKeyResultPercentageValue(businessOutcomeName, keyResultName)).GetText();
        }

        public string GetKeyResultPercentageValue(string keyResultName)
        {
            return Driver.JavaScriptScrollToElement(DynamicKeyResultPercentageValue(keyResultName), false).GetText();
        }

        public string GetKeyResultProgressIndication(string businessOutcomeName, string keyResultName)
        {
            return Wait.UntilElementVisible(DynamicKeyResultProgressBar(businessOutcomeName, keyResultName)).GetElementAttribute("style");
        }

        public string GetKeyResultProgressBarColor(string businessOutcomeName, string keyResultName)
        {
            return Wait.UntilElementVisible(DynamicKeyResultProgressBar(businessOutcomeName, keyResultName)).GetCssValue("background-color");
        }

        public bool IsKeyResultVisible(string businessOutcomeName, string keyResultName)
        {
            if (Wait.InCase(DynamicKeyResultPercentageValue(businessOutcomeName, keyResultName)) != null)
            {
                return Driver.JavaScriptScrollToElement(Wait.InCase(DynamicKeyResultPercentageValue(businessOutcomeName, keyResultName))).Displayed;
            }
            return false;
        }

        public bool IsKeyResultImpactIconVisible(string businessOutcomeName, string keyResultName)
        {
            return Wait.InCase(DynamicKeyResultImpactIcon(businessOutcomeName, keyResultName)) != null && Wait.InCase(DynamicKeyResultImpactIcon(businessOutcomeName, keyResultName)).Displayed;
        }

        public void ClickOnExternalLinkButton(string businessOutcomeName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on 'External Link' button for Business Outcome <{businessOutcomeName}>");
            Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeExternalLinkButton(businessOutcomeName), false).Click();
        }

        public void ClickOnCopyButton(string businessOutcomeName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on 'Copy' button for Business Outcome <{businessOutcomeName}>");
            Wait.UntilElementVisible(SearchCardsTextbox).SetText(businessOutcomeName).SendKeys(Keys.Enter);
            Driver.JavaScriptScrollToElement(DynamicBusinessOutcomeCopyButton(businessOutcomeName)).Click();
        }

        public void MouseHoverOnExternalLinkButton(string businessOutcomeName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Hover on 'External Link' button for Business Outcome <{businessOutcomeName}>");
            Driver.MoveToElement(Wait.UntilElementClickable(DynamicBusinessOutcomeExternalLinkBadgeIndicator(businessOutcomeName)));
        }

        public string GetTotalExternalLinksCount(string businessOutcomeName)
        {
            return Wait.InCase(DynamicBusinessOutcomeExternalLinkBadgeIndicator(businessOutcomeName))?.GetText() ?? "0";
        }

        public bool IsExternalLinkPresent(string externalLinkName)
        {
            return Driver.IsElementDisplayed(By.LinkText(externalLinkName));
        }

        public void ClickOnExternalLink(string linkText)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on external link <{linkText}>");
            Wait.UntilElementClickable(ExternalLinkFlyoutLink(linkText)).Click();
        }

        public bool IsBusinessOutcomeCategoryPresent(string columnName, string businessOutcomeName, string categoryName)
        {
            if (Wait.InCase(DynamicBusinessOutcomeCategory(columnName, businessOutcomeName, categoryName)) != null)
            {
                return Driver.JavaScriptScrollToElement(Wait.InCase(DynamicBusinessOutcomeCategory(columnName, businessOutcomeName, categoryName))).Displayed;
            }
            return false;
        }


        #endregion

        #region SearchFilters
        //Search Filter
        public void Filter_SelectCategories(string[] categories)
        {
            if (categories.Length > 0)
            {
                Filter_ClickOnFilterDropdown();
                Filter_ClickOnClearButton();

                foreach (var category in categories)
                {
                    Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on checkbox for category <{category}>");
                    Wait.UntilElementExists(CategoryValueCheckbox(category)).Click();
                }
            }
            Filter_ClickOnOkButton();
        }

        public void SelectFilterTags(List<string> tags)
        {

            Filter_ClickOnFilterDropdown();
            Filter_ClickOnClearButton();

            foreach (var tag in tags)
            {
                Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on checkbox for category <{tag}>");
                Wait.UntilElementExists(CategoryValueCheckbox(tag)).Click();
            }

            Filter_ClickOnOkButton();
            Filter_ClickOnFilterDropdown();
            WaitForReload();
        }

        public bool AreFilterTagsSelected(List<string> tags)
        {
            Filter_ClickOnFilterDropdown();
            var selectedTagsList = new List<bool>();

            foreach (var tag in tags)
            {
                selectedTagsList.Add(Driver.IsElementSelected(CategoryValueCheckbox(tag), 15));
            }
            var isTagsSelected = selectedTagsList.All(a => true);
            Filter_ClickOnFilterDropdown();
            return isTagsSelected;
        }

        public void Filter_ClickOnClearButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Clear' button on the filter");
            Wait.UntilElementClickable(FilterClearButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void Filter_ClickOnFilterDropdown()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on Filter dropdown>");
            Wait.UntilElementClickable(FilterDropdown).Click();
            Wait.UntilJavaScriptReady();
        }

        public void Filter_ClickOnOkButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'OK' button on the filter");
            Wait.UntilElementClickable(FilterOkButton).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region ColorFilters
        //Color Filter
        public void ColorFilterClickOnFilterButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Color Filter' button");
            Wait.UntilElementClickable(ColorFilterButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ColorFilterSelectColor(string colorHexValue)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on <{colorHexValue}> in the color picker");
            Wait.UntilElementVisible(ColorFilterColorSelection(colorHexValue)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool IsColorFilterColorSelected(string colorHexValue)
        {
            return Driver.IsElementDisplayed(ColorFilterColorSelected(colorHexValue));
        }

        public void ColorFilterClickOnClearButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Clear' button on the filter");
            Wait.UntilElementClickable(ColorFilterClearButton).Click();
            Wait.UntilJavaScriptReady();
        }
        public void ColorFilterClose()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Close' button on the filter");
            Wait.UntilElementExists(ColorFilterClearButton).SendKeys(Keys.Escape);
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region CardType
        public void CardTypeExpandCollapseDropdown()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Expand/Collapse' button on the Card type dropdown");
            if (Wait.UntilElementExists(CardTypeDropdown).GetAttribute("aria-expanded") != "false") return;
            Wait.UntilElementClickable(CardTypeDropdown).Click();
            Wait.UntilJavaScriptReady();

        }

        public void SelectCardTypeFromDropdown(string cardType)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Select card type from dropdown");
            SelectItem(CardTypeTagDropdown, CardTypeOption((cardType)));
        }


        public List<string> CardTypeGetAllTypeList()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Get list of Card types");
            return Wait.UntilAllElementsLocated(CardTypeList).Select(tag => tag.GetText()).ToList();
        }

        public void SelectCardType(string cardType)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Select card type from the dropdown");
            CardTypeExpandCollapseDropdown();
            Wait.UntilElementClickable(CardTypeOption(cardType)).Click();
            Wait.HardWait(2000); //Need to wait till cards loaded
        }


        #endregion

        #region TagsView

        //Tags View
        public void TagsViewExpandCollapseTagDropdown()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Expand/Collapse' button on the Tag dropdown");
            Wait.UntilElementClickable(TagsViewTagDropdown).Click();
            Wait.UntilJavaScriptReady();
        }
        public List<string> TagsViewGetAllTagsList()
        {
            return Wait.UntilAllElementsLocated(TagsViewAllTagsList).Select(tag => tag.GetText()).ToList();
        }

        public void TagsViewSelectTag(string tagName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Click on tag <{tagName}> on the Tag dropdown");
            SelectItem(TagsViewTagDropdown, TagListItem(tagName));
            WaitForReload();
        }

        public void SelectCardTypeTag(string tagName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Select card type tag");
            Wait.UntilElementClickable(TagListItem(tagName)).Click();
            Wait.HardWait(2000); //It takes time to load the dashboard
        }

        public string GetSelectedLabelFromDropDown()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Get selected tag from 'Outcomes' dropdown");
            return Wait.UntilElementExists(TagsViewTagDropdown).GetText();
        }

        public string GetTagFromDropDown()
        {
            return Wait.UntilElementExists(TagsViewTagDropdown).GetText();
        }


        #endregion

        #region ExportToExcelAndPDF

        //Export To Excel
        public void ClickOnExportToExcelButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Export to Excel' button");
            Wait.UntilElementClickable(ExportToExcelButton).Click();
            Wait.UntilJavaScriptReady();
        }

        //Export To PDF
        public void ClickOnExportToPdfButton()
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), "Click on 'Export to PDF' button");
            Wait.UntilElementClickable(ExportToPdfButton).Click();
            Wait.UntilJavaScriptReady();
        }

        #endregion

        #region DuplicateCardPopup

        //Duplicate Card Pop up
        public void DuplicateCardCreate(string teamName, string columnName)
        {
            Log.Step(nameof(BusinessOutcomesDashboardPage), $"Create duplicate card on <{teamName},{columnName}>");
            Wait.UntilElementClickable(DuplicateCardTeamTextBox).Click();
            Wait.UntilElementClickable(DuplicateCardTeamInput).SetText(teamName).SendKeys(Keys.Tab);
            Wait.UntilElementVisible(DuplicateCardTeamName(teamName));
            Wait.UntilElementClickable(DuplicateCardColumnInput).SetText(columnName).SendKeys(Keys.Tab); ;
            Wait.UntilElementClickable(DuplicateCardSaveButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void WaitForReload()
        {
            Wait.InCase(LoadingIcon, 3);
            Wait.UntilElementNotExist(LoadingIcon);
        }

        #endregion

        #region Navigation

        //Navigation
        public void NavigateToPage(int companyId, List<int> parentIds = null, int teamId = 0)
        {
            parentIds ??= new List<int> { 0 };
            var parentString = string.Join("&", parentIds);
            NavigateToUrl($"{BaseTest.ApplicationUrl}/V2/outcomes/company/{companyId}/parents/{parentString}/team/{teamId}");
            WaitTillBoPageLoadedCompletely();
        }
        public string GetPageUrl(int companyId, List<int> parentIds = null, int teamId = 0, int cardTypeId = 1, int categoryLabelUid = 1)
        {
            parentIds ??= new List<int> { 0 };
            var parentString = string.Join("&", parentIds);
            return $"{BaseTest.ApplicationUrl}/V2/outcomes/company/{companyId}/parents/{parentString}/team/{teamId}/tab/kanban?cardTypeId={cardTypeId}&categoryLabelUid={categoryLabelUid}";
        }

        #endregion
        public bool AreBusinessOutcomeSubHeadersDisplayed()
        {
            var businessOutcomeSubHeaders = new List<By>
            {
                SearchCardsTextbox,
                FilterDropdown,
                ColorFilterButton,
                ExportToExcelButton,
                ExportToPdfButton,
                SettingsIcon
            };
            // Check if all icons are displayed.
            return businessOutcomeSubHeaders.All(locator => Driver.IsElementDisplayed(locator));
        }
    }

    public enum TimeFrameTags
    {
        [Description("Annually")]
        Annually,
        [Description("Quarterly")]
        Quarterly
    }
}

#endregion
