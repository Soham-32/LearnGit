using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes
{
    public class BusinessOutcomesArchiveSettingsPage : BusinessOutcomesSettingsPage
    {
        public BusinessOutcomesArchiveSettingsPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Filter Grid

        private readonly By ColumnMenuButton = By.XPath("//thead[@role='rowgroup']//th[3]//a//*[local-name()='svg']");
        private readonly By ColumnFilterSearchBox = By.XPath("//div[@class='k-filter-menu-container']//input[1]");
        private readonly By GridFilterButton = By.XPath("//span[text()='Filter']");
        private readonly By GridClearButton = By.XPath("//span[text()='Clear']");

        #endregion

        #region Card Details Grid

        private static By RestoreButton(string cardTitle) => By.XPath($"//tbody[@role='rowgroup']//tr//td[contains(normalize-space(),'{cardTitle}')]//preceding-sibling::td/button/span[contains(normalize-space(),'Restore')]");
        private static By CardId(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='2'] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='2']");
        private static By TeamName(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='4'] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='4']");
        private static By DescriptionText(string cardTitle) => By.XPath($"//td[contains(normalize-space(),'{cardTitle}')]//ancestor::tr//td[5] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='5']");
        private static By CreatedBy(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='6'] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='6']");
        private static By ArchivedBy(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='8'] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='8']");
        private static By RestoredBy(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='10'] | //font[text()='{cardTitle}']//ancestor::tr//td[@aria-colindex='10']");
        private static By ArchivedAt(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//following-sibling::td[4] | //font[text()='{cardTitle}']//ancestor::tr//td[7]");
        private static By RestoredAt(string cardTitle) => By.XPath($"//td[text()='{cardTitle}']//following-sibling::td[6] | //font[text()='{cardTitle}']//ancestor::tr//td[9]");

        private readonly By CardTitle = By.XPath("//tbody[@role='rowgroup']//tr//td[@aria-colindex='3']");
        private readonly By ConfirmRestorePopupText = By.XPath("//h2[text()='Confirm Restore']");
        private readonly By RestoreButtonConfirmRestorePopup = By.XPath("//div[@class='k-window k-dialog']/div/button[text()='RESTORE']");
        private readonly By CancelButtonConfirmRestorePopup = By.XPath("//div[@class='k-window k-dialog']/div/button[text()='CANCEL']");
        private readonly By RestoredSuccessfullyToasterMessage = By.XPath("//div[text()='Business Outcome restored successfully']");

        #endregion

        #endregion

        #region Methods

        #region Filter Grid

        public void ClickOnColumnMenuButton()
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Click on  the 'Filter' icon");
            Wait.UntilElementClickable(ColumnMenuButton).Click();
        }

        public void FilterWithCardTitle(string cardTitle)
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Filter with title of the card");
            Wait.UntilElementClickable(ColumnMenuButton).Click();
            Wait.UntilElementVisible(ColumnFilterSearchBox).SetText(cardTitle);
            Wait.UntilElementClickable(GridFilterButton).Click();
        }

        public void ClickOnClearButton()
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Click on the 'Clear' button");
            Wait.UntilElementClickable(GridClearButton).Click();
        }

        #endregion

        #region Card Details Grid

        public void ClickOnRestoreButton(string cardTitle)
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Click on the 'Restore' button");
            Wait.UntilElementClickable(RestoreButton(cardTitle)).Click();
        }

        public bool IsRestoreButtonDisplayed(string cardTitle)
        {
            Wait.UntilElementNotExist(RestoredSuccessfullyToasterMessage);
            return Driver.IsElementDisplayed(RestoreButton(cardTitle));
        }

        public bool IsConfirmRestorePopupTitleDisplayed()
        {
            return Driver.IsElementDisplayed(ConfirmRestorePopupText);
        }

        public void ClickOnRestoreButtonOfConfirmRestorePopup()
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Click on the 'RESTORE' button of 'Confirm Restore' popup");
            Wait.UntilElementClickable(RestoreButtonConfirmRestorePopup).Click();
            Wait.HardWait(2000);// Need to wait till card gets restored
        }

        public void ClickOnCancelButtonOfConfirmRestorePopup()
        {
            Log.Step(nameof(BusinessOutcomesArchiveSettingsPage), "Click on the 'CANCEL' button of 'Confirm Restore' popup");
            Wait.UntilElementClickable(CancelButtonConfirmRestorePopup).Click();
        }

        public bool IsRestoredSuccessfullyToasterMessageDisplayed()
        {
            var toasterMessage = Driver.IsElementDisplayed(RestoredSuccessfullyToasterMessage);
            Wait.UntilElementInvisible(RestoredSuccessfullyToasterMessage);
            return toasterMessage;
        }

        public ArchivedRestoreBoCards GetArchivedBusinessOutcome(string cardTitle)
        {
            return new ArchivedRestoreBoCards
            {
                Id = Wait.UntilElementExists(CardId(cardTitle)).GetText().ToInt(),
                Title = Wait.UntilElementExists(CardTitle).GetText(),
                TeamName = Wait.UntilElementExists(TeamName(cardTitle)).GetText(),
                Description = Wait.UntilElementExists(DescriptionText(cardTitle)).GetText(),
                CreatedBy = Wait.UntilElementExists(CreatedBy(cardTitle)).GetText(),
                ArchivedAt = Wait.UntilElementExists(ArchivedAt(cardTitle)).GetText().ToDateTime(),
                ArchivedBy = Wait.UntilElementExists(ArchivedBy(cardTitle)).GetText(),
                RestoredAt = Wait.UntilElementExists(RestoredAt(cardTitle)).GetText().ToDateTime(),
                RestoredBy = Wait.UntilElementExists(RestoredBy(cardTitle)).GetText()

            };
        }


        #endregion

        #endregion
    }
}