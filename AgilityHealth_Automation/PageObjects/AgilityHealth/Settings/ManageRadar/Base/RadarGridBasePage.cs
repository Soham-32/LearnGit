using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarGridBasePage: RadarHeaderBasePage
    {

        public RadarGridBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        // Radar Grid Section
        //Add new Dimension, Sub-dimension, Competency, Question and Open-Ended Button
        private readonly By AddNewButton = By.XPath("//a[contains(text(),'Add')]");
        private static By EditButton(string metrics) => By.XPath($"//tr/td[normalize-space()='{metrics}']/parent::tr/td/a/span[@class='k-icon k-edit']");
        private static By DeleteButton(string metrics) => By.XPath($"//tr/td[normalize-space()='{metrics}']/parent::tr/td/a/span[@class='k-icon k-delete']");
        private readonly By DeleteConfirmationYesButton = By.XPath("//div[@id='deleteConfirmation']/span[normalize-space()='Yes']");

        //Column Section
        private readonly By GridHeaderColumnsList = By.CssSelector("ul[style*='display: block'] span.k-link");
        private readonly By GridHeaderVisibleColumn = By.XPath("//th[@data-role='columnsorter'][(@style='')or(@style='color: rgb(0, 0, 0);')or not (@style)][not(@data-field='Action')]");
        private readonly By GridNumberRows = By.CssSelector("div[id$='-grid'] tbody>tr");

        private static By ColumnMenu(string arrowIcon) => By.XPath($"//th[@data-title='{arrowIcon}']//a/span[contains(@class,'k-i-arrowhead-s')]");
        private readonly By GridHeaderColumnMenuSortAscending = By.CssSelector(".k-sprite.k-i-sort-asc");
        private readonly By GridHeaderColumnMenuSortDescending = By.CssSelector(".k-sprite.k-i-sort-desc");
        private readonly By GridHeaderColumnMenuColumnsItem = By.CssSelector(".k-sprite.k-i-columns");
        private static By GridHeaderColumnListCheckbox(string columnsCheckboxes) => By.XPath($"//ul[contains(@style, 'display: block')]//span[@class='k-link'][text()='{columnsCheckboxes}']/input");
        private static By ColumnValuesList(string columnHeader) => By.XPath($"//div[contains(@id,'-grid')]//tbody//tr/td[count(//th[@data-role='columnsorter'][@data-title='{columnHeader}']//preceding-sibling::th) + 1]");
        private static By RowValues(int index, string columnHeader) => By.XPath($"//div[contains(@id,'-grid')]//tbody//tr[{index}]/td[count(//th[@data-role='columnsorter'][@style='' or not(@style)]//a[@class='k-link'][text()='{columnHeader}']/../preceding-sibling::th | //th[text() = '{columnHeader}']/preceding-sibling::th) + 1]");

        //Add/Edit Common Parameter Popup
        private readonly By NameTextbox = By.Id("Name");
        private readonly By ColorDropdownArrow =
            By.XPath("//input[@id='Color']/preceding-sibling::span//span[contains(@class,'arrow')]");
        private static By ColorPickerInput => By.XPath($"//div[@data-role='colorpicker']//input");
        private static By ColorPickerApplyButton => By.XPath($"//div[@data-role='colorpicker']//button[text()='Apply'] | //div[@data-role='colorpicker']//button//font[text()='Apply']");
        private readonly By RadarOrderTextbox = By.Id("RadarOrder");
        private readonly By FontDropdown = By.XPath("//span[@aria-owns='Font_listbox']");
        private static By SelectFontDropdown(string font) => By.XPath($"//ul[@id='Font_listbox']//li[@role='option'][normalize-space()='{font}']");
        private readonly By FontSizeDropdown = By.XPath("//span[@aria-owns='FontSize_listbox']");
        private static By SelectFontSizeDropdown(string size) => By.XPath($"//ul[@id='FontSize_listbox']//li[@role='option'][normalize-space()='{size}']");
        private readonly By LetterSpacingDropdown = By.XPath("//span[@aria-owns='LetterSpacing_listbox']");
        private static By SelectLetterSpacingDropdown(string space) => By.XPath($"//ul[@id='LetterSpacing_listbox']//li[@role='option'][normalize-space()='{space}']");
        private readonly By DirectionDropdown = By.XPath("//span[@aria-owns='Direction_listbox']");
        private static By SelectDirectionDropdown(string direction) => By.XPath($"//ul[@id='Direction_listbox']//li[@role='option'][normalize-space()='{direction}']");
        private readonly By UpdateButton = By.XPath("//a[contains(@class,'k-grid-update')]");
        private readonly By CancelButton = By.XPath("//a[contains(@class,'k-grid-cancel')]");
        private readonly By DimensionDropdown = By.XPath("//span[@aria-owns='dimensions_listbox']");
        private static By SelectDimensionDropdown(string dimension) => By.XPath($"//ul[@id='dimensions_listbox']//li[@role='option'][normalize-space()='{dimension}']");
        private static By AddEditSubDimensionPopupAbbreviation => By.Id("Abbreviation");
        private readonly By SubDimensionDropdown = By.XPath("//input[@id='subdimensions']//preceding-sibling::span");
        private readonly By SubDimensionValidationMessage = By.Id("subdimensions_validationMessage");
        private static By SelectSubDimensionDropdown(string subDimension) => By.XPath($"//ul[@id='subdimensions_listbox']//li[@role='option'][normalize-space()='{subDimension}']");
        private readonly By WorkTypeDropdown = By.XPath("//span[@aria-owns='ExcludeWorkType_listbox']");
        private static By SelectWorkTypeDropdown(string option) => By.XPath($"//ul[@id='ExcludeWorkType_listbox']//li[@role='option'][normalize-space()='{option}']");

        private readonly By WorkTypeOptionsDropdown = By.XPath("//input[@aria-owns='SelectedWorkTypes_taglist SelectedWorkTypes_listbox']");
        private static By SelectWorkTypeOptionsDropdown(string workType) => By.XPath($"//ul[@id='SelectedWorkTypes_listbox']//li[@role='option'][normalize-space()='{workType}']");
        private readonly By MethodologyDropdown = By.XPath("//span[@aria-owns='ExcludeMethodology_listbox']");
        private static By SelectMethodologyDropdown(string option) => By.XPath($"//ul[@id='ExcludeMethodology_listbox']//li[@role='option'][normalize-space()='{option}']");
        private readonly By MethodologiesOptionsDropdown = By.XPath("//input[@aria-owns='SelectedMethodologies_taglist SelectedMethodologies_listbox']");
        private static By SelectMethodologiesOptionsDropdown(string methodology) => By.XPath($"//ul[@id='SelectedMethodologies_listbox']//li[@role='option'][normalize-space()='{methodology}']");
        private readonly By CompanyDropdown = By.XPath("//span[@aria-owns='ExcludeCompany_listbox']");
        private static By SelectCompanyDropdown(string option) => By.XPath($"//ul[@id='ExcludeCompany_listbox']//li[@role='option'][normalize-space()='{option}']");
        private readonly By CompaniesOptionsDropdown = By.XPath("//input[@aria-owns='Companies_taglist Companies_listbox']");
        private static By SelectCompaniesOptionsDropdown(string company) => By.XPath($"//ul[@id='Companies_listbox']//li[@role='option'][normalize-space()='{company}']");

        //Add/Edit  Translated Common Parameter Popup
        private readonly By TranslatedNameTextbox = By.Id("TranslatedName");
        private readonly By TranslatedDimensionDropdown = By.Id("TranslatedDimensionName");
        private static By TranslatedAbbreviation => By.Id("TranslatedAbbreviation");
        private readonly By TranslatedSubDimensionDropdown = By.Id("TranslatedSubDimensionName");
        

        // Radar Grid Section 
        public void GridHeaderClickOnColumnMenuArrow(string arrowIcon)
        {
            Log.Step(nameof(RadarGridBasePage), "Click column menu");
            Wait.UntilElementClickable(ColumnMenu(arrowIcon)).Click();
        }

        public void GridHeaderSelectColumnMenuColumnList(List<string> columns)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select columns {string.Join(",", columns)}");
            foreach (var ele in Wait.UntilAllElementsLocated(GridHeaderColumnsList))
            {
                var elementText = Driver.MoveToElement(ele).GetText();
                var element = Driver.MoveToElement(Wait.UntilElementVisible(GridHeaderColumnListCheckbox(elementText)));
                element.Check(columns.Contains(elementText));
            }
        }

        public void GridHeaderClickOnColumnsMenuColumnsButton(string columnIcon)
        {
            Log.Step(nameof(RadarGridBasePage), "Click column menu item");
            GridHeaderClickOnColumnMenuArrow(columnIcon);
            Wait.UntilElementVisible(GridHeaderColumnMenuColumnsItem).Click();
        }

        public void GridHeaderClickOnColumnMenuSortAscendingButton(string ascIcon)
        {
            Log.Step(nameof(RadarGridBasePage), "Click sort ascending icon");
            GridHeaderClickOnColumnMenuArrow(ascIcon);
            Wait.UntilElementVisible(GridHeaderColumnMenuSortAscending).Click();
        }

        public void GridHeaderClickOnColumnMenuSortDescendingButton(string descIcon)
        {
            Log.Step(nameof(RadarGridBasePage), "Click sort descending icon");
            GridHeaderClickOnColumnMenuArrow(descIcon);
            Wait.UntilElementVisible(GridHeaderColumnMenuSortDescending).Click();
        }

        public List<string> GridHeaderGetColumnList()
        {
            Log.Step(nameof(RadarGridBasePage), "Get List of column header");
            var rawHeaders = Wait.UntilAllElementsLocated(GridHeaderVisibleColumn).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
            return rawHeaders.Select(header => header.Replace("Column Settings\r\n", "")).ToList();
        }

        public int GetNumberOfGridRows()
        {
            Log.Step(nameof(RadarGridBasePage), "Get Number of Grid Rows");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(GridNumberRows).Count;
        }

        public List<string> GetColumnHeaderValues(string columnHeader)
        {
            Log.Step(nameof(RadarGridBasePage), "Get Column Header values");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(ColumnValuesList(columnHeader)).Select(header => Driver.JavaScriptScrollToElement(header).GetText()).ToList();
        }

        public List<string> GetRowHeaderValues(int index, List<string> columns)
        {
            Log.Step(nameof(RadarGridBasePage), "Get Row Header values");
            return columns.Select(column => Driver.JavaScriptScrollToElement(Wait.UntilElementExists(RowValues(index, column))).GetText()).ToList();
        }

        public void ClickOnAddNewButton()
        {
            Log.Step(nameof(RadarGridBasePage), "Click on 'Add New' Button");
            Wait.UntilElementClickable(AddNewButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnDeleteButton(string rowValue)
        {
            Log.Step(nameof(RadarGridBasePage), "Click on 'Delete' Button");
            Wait.UntilElementClickable(DeleteButton(rowValue)).Click();
        }

        public void ClickOnDeleteConfirmationYesButton()
        {
            Log.Step(nameof(RadarGridBasePage), "Click on Delete confirmation 'Yes' Button");
            try
            {
                Driver.AcceptAlert();
            }
            catch (NoAlertPresentException)
            {
                Wait.UntilElementVisible(DeleteConfirmationYesButton);
                Wait.UntilElementClickable(DeleteConfirmationYesButton).Click();
                Wait.UntilElementInvisible(DeleteConfirmationYesButton);
                Wait.UntilJavaScriptReady();
            }
        }

        public void ClickOnEditButton(string rowValue)
        {
            Log.Step(nameof(RadarGridBasePage), "Click on 'Edit' Button");
            Wait.UntilElementClickable(EditButton(rowValue)).Click();
            Wait.UntilJavaScriptReady();
        }

        // Add/Edit Common Parameter Popup
        public void SetName(string name)
        {
            Log.Step(nameof(RadarGridBasePage), $"Enter '{name}' in Name textbox");
            Wait.UntilElementClickable(NameTextbox).SetText(name);
        }

        public void SelectColor(string color)
        {
            Log.Step(nameof(RadarGridBasePage),$"Select '{color}' color from color picker");
            Wait.UntilElementClickable(ColorDropdownArrow).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ColorPickerInput).SetText(color);
            Wait.UntilElementClickable(ColorPickerApplyButton).Click();
        }

        public void SetRadarOrder(string radarOrder)
        {
            Log.Step(nameof(RadarGridBasePage), $"Enter '{radarOrder}' in Radar Order textbox");
            Wait.UntilElementClickable(RadarOrderTextbox).SetText(radarOrder);
        }
        
        public void SelectFont(string font)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{font}' font from Font dropdown");
            SelectItem(FontDropdown, SelectFontDropdown(font));
        }

        public void SelectFontSize(string fontSize)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{fontSize}' font size from Font Size dropdown");
            SelectItem(FontSizeDropdown, SelectFontSizeDropdown(fontSize));
        }

        public void SelectLetterSpacing(string letterSpacing)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{letterSpacing}' spacing from Letter Spacing dropdown");
            SelectItem(LetterSpacingDropdown, SelectLetterSpacingDropdown(letterSpacing));
        }

        public void SelectDirection(string direction)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{direction}' direction from Direction dropdown");
            SelectItem(DirectionDropdown, SelectDirectionDropdown(direction));
        }

        public void ClickOnUpdateButton()
        {
            Log.Step(nameof(RadarGridBasePage), "Click on 'Update' Button");
            Wait.UntilElementClickable(UpdateButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnCancelButton()
        {
            Log.Step(nameof(RadarGridBasePage), "Click on 'Cancel' Button");
            Wait.UntilElementClickable(CancelButton).Click();
        }

        public void SelectDimension(string dimension)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{dimension}' dimension from Dimension dropdown");
            SelectItem(DimensionDropdown, SelectDimensionDropdown(dimension));
        }

        public void SetAbbreviation(string abbreviation)
        {
            Log.Step(nameof(RadarGridBasePage), $"Enter '{abbreviation}' in Abbreviation textbox");
            Wait.UntilElementClickable(AddEditSubDimensionPopupAbbreviation).SetText(abbreviation);
        }

        public void SelectSubDimension(string subDimension)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{subDimension}' Sub Dimension from Sub Dimension dropdown");
            SelectItem(SubDimensionDropdown, SelectSubDimensionDropdown(subDimension));
        }

        public void SelectWorkTypeFilter(string workTypeFilter)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{workTypeFilter}' from Work Type Filter dropdown");
            SelectItem(WorkTypeDropdown, SelectWorkTypeDropdown(workTypeFilter));
        }

        public void SelectWorkType(string workType)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{workType}' Work Type from Work Type dropdown");
            SelectItem(WorkTypeOptionsDropdown, SelectWorkTypeOptionsDropdown(workType));
        }

        public void SelectMethodologyFilter(string methodologyFilter)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{methodologyFilter}' from Methodology Filter dropdown");
            SelectItem(MethodologyDropdown, SelectMethodologyDropdown(methodologyFilter));
        }

        public void SelectMethodology(string methodology)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{methodology}' methodology from Methodology dropdown");
            SelectItem(MethodologiesOptionsDropdown, SelectMethodologiesOptionsDropdown(methodology));
        }

        public void SelectCompanyFilter(string companyFilter)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{companyFilter}' from Company Filter dropdown");
            SelectItem(CompanyDropdown, SelectCompanyDropdown(companyFilter));
        }

        public void SelectCompany(string company)
        {
            Log.Step(nameof(RadarGridBasePage), $"Select '{company}' company from Company dropdown");
            SelectItem(CompaniesOptionsDropdown, SelectCompaniesOptionsDropdown(company));
        }
        public bool IsSubDimensionDropdownEnabled()
        {
            return !Wait.UntilElementClickable(SubDimensionDropdown).GetAttribute("class")
                .Contains("disabled");
        }

        public bool IsSubDimensionDropdownValidationMessageDisplayed()
        {
            return Driver.IsElementDisplayed(SubDimensionValidationMessage);
        }

        // Add/Edit Translated Common Parameter Popup
        public void SetTranslatedName(string name)
        {
            Log.Step(nameof(RadarGridBasePage), $"Enter '{name}' in Translated Name textbox");
            Wait.UntilElementClickable(TranslatedNameTextbox).SetText(name);
        }

        public string GetTranslatedDimensionName(string dimension)
        {
            Log.Step(nameof(RadarGridBasePage), "Get translated dimension name");
            return Wait.UntilElementClickable(TranslatedDimensionDropdown).GetText();
        }

        public bool IsTranslatedDimensionNameEnabled()
        {
            return Driver.IsElementEnabled(TranslatedDimensionDropdown);
        }
        public void SetTranslatedAbbreviation(string abbreviation)
        {
            Log.Step(nameof(RadarGridBasePage), $"Enter '{abbreviation}' in translated Abbreviation textbox");
            Wait.UntilElementClickable(TranslatedAbbreviation).SetText(abbreviation);
        }

        public string GetTranslatedSubDimensionName(string subDimension)
        {
            Log.Step(nameof(RadarGridBasePage), "Get translated Sub Dimension name");
            return Wait.UntilElementClickable(TranslatedSubDimensionDropdown).GetText();
        }

        public bool IsTranslatedSubDimensionNameEnabled()
        {
            return Driver.IsElementEnabled(TranslatedSubDimensionDropdown);
        }
    }
}