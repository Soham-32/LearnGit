using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarDimensionsBasePage : RadarGridBasePage
    {
        public RadarDimensionsBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By AddEditDimensionPopupSortOrderTextbox = By.Id("SortOrder");
        private readonly By TranslatedNameValidationMessage = By.Id("TranslatedName_validationMessage");

        internal void EnterDimensionInfo(RadarDimensionResponse radarDimension)
        {
            Log.Step(nameof(RadarDimensionsBasePage), "Enter Dimension info");

            //Dimension Name
            SetName(radarDimension.Name);

            //Sort Order
            Wait.UntilElementClickable(AddEditDimensionPopupSortOrderTextbox).SetText(radarDimension.SortOrder);

            //Color 
            if (!string.IsNullOrEmpty(radarDimension.Color))
                SelectColor(radarDimension.Color);

            //Radar Order
            if (!string.IsNullOrEmpty(radarDimension.RadarOrder))
                SetRadarOrder(radarDimension.RadarOrder);
            
            //Font  
            if (!string.IsNullOrEmpty(radarDimension.Font))
                SelectFont(radarDimension.Font);
            
            //Font Size
            if (!string.IsNullOrEmpty(radarDimension.FontSize))
                SelectFontSize(radarDimension.FontSize);

            //Letter Spacing 
            if (!string.IsNullOrEmpty(radarDimension.LetterSpacing))
                SelectLetterSpacing(radarDimension.LetterSpacing);

            //Direction
            if (!string.IsNullOrEmpty(radarDimension.Direction))
                SelectDirection(radarDimension.Direction);
        }

        internal void EnterTranslatedDimensionInfo(Dimension radarDimension)
        {
            Log.Step(nameof(RadarDimensionsBasePage), "Enter Translated Dimension info");
            SetTranslatedName(radarDimension.Name);
        }

        public bool IsTranslatedDimensionNameValidationMessageDisplayed()
        {
            return Driver.IsElementDisplayed(TranslatedNameValidationMessage);
        }
    }
}