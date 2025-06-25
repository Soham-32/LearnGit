using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarSubDimensionsBasePage : RadarGridBasePage
    {
        public RadarSubDimensionsBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {

        }

        private readonly By AddEditSubDimensionPopupDimensionDropdown = By.XPath("//span[@aria-owns='DimensionID_listbox']");
        private static By AddEditSubDimensionPopupSelectDimensionDropdown(string dimension) => By.XPath($"//ul[@id='DimensionID_listbox']//li[@role='option'][normalize-space()='{dimension}']");
        private static By AddEditSubDimensionPopupDescriptionIframe => By.XPath("//div[normalize-space()='Description']//following-sibling::div//iframe");
        private static By AddEditSubDimensionPopupDescriptionBodyIframe => By.XPath("//body");


        private static By TranslatedAddEditSubDimensionPopupDescriptionIframe => By.XPath("//div[normalize-space()='Description' and contains(@class,'translated')]//following-sibling::div//iframe");


        internal void EnterSubDimensionInfo(RadarSubDimensionResponse radarSubDimension)
        {
            Log.Step(nameof(RadarSubDimensionsBasePage), "Enter Sub Dimension info");

            //Dimension 
            SelectItem(AddEditSubDimensionPopupDimensionDropdown, AddEditSubDimensionPopupSelectDimensionDropdown(radarSubDimension.Dimension));

            //Sub Dimension Name
            SetName(radarSubDimension.Name);

            //Color
            if (!string.IsNullOrEmpty(radarSubDimension.Color))
                SelectColor(radarSubDimension.Color);

            //Abbreviation
            if (!string.IsNullOrEmpty(radarSubDimension.Abbreviation))
                SetAbbreviation(radarSubDimension.Abbreviation);

            //Description
            EnterSubDimensionDescription(radarSubDimension.Description);

            //Direction
            if (!string.IsNullOrEmpty(radarSubDimension.Direction))
                SelectDirection(radarSubDimension.Direction);

            //Radar Order
            if (!string.IsNullOrEmpty(radarSubDimension.RadarOrder))
                SetRadarOrder(radarSubDimension.RadarOrder);

            //Font
            if (!string.IsNullOrEmpty(radarSubDimension.Font))
                SelectFont(radarSubDimension.Font);

            //Font Size
            if (!string.IsNullOrEmpty(radarSubDimension.FontSize))
                SelectFontSize(radarSubDimension.FontSize);

            //Letter Spacing 
            if (!string.IsNullOrEmpty(radarSubDimension.LetterSpacing))
                SelectLetterSpacing(radarSubDimension.LetterSpacing);
        }

        internal void EnterTranslatedSubDimensionInfo(SubDimension radarSubDimension)
        {
            Log.Step(nameof(RadarSubDimensionsBasePage), "Enter Translated Sub Dimension info");
            SetTranslatedName(radarSubDimension.Name);
            SetTranslatedAbbreviation(radarSubDimension.Abbreviation);
            EnterTranslatedSubDimensionDescription(radarSubDimension.Description);
        }

        private void EnterSubDimensionDescription(string text = "Description here")
        {
            Log.Step(nameof(RadarSubDimensionsBasePage), "Enter value in 'Sub Dimension' description");
            Driver.SwitchToFrame(AddEditSubDimensionPopupDescriptionIframe);
            Wait.UntilElementClickable(AddEditSubDimensionPopupDescriptionBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }

        private void EnterTranslatedSubDimensionDescription(string text = "Description here")
        {
            Log.Step(nameof(RadarSubDimensionsBasePage), "Enter value in 'Sub Dimension' description");
            Driver.SwitchToFrame(TranslatedAddEditSubDimensionPopupDescriptionIframe);
            Wait.UntilElementClickable(AddEditSubDimensionPopupDescriptionBodyIframe).SetText(text);
            Driver.SwitchTo().DefaultContent();
        }
    }
}