using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Radars.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarCompetenciesBasePage : RadarGridBasePage
    {
        public RadarCompetenciesBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private static By AddEditCompetencyPopupAnalyticsAbbreviationTextbox => By.Id("AnalyticAbbr");
        private static By AddEditCompetencyPopupExcludeTextbox => By.Id("Exclude");
        private static By TranslatedAddEditCompetencyPopupAnalyticsAbbreviationTextbox =>
            By.Id("TranslatedAnalyticAbbr");


        internal void EnterCompetencyInfo(RadarCompetencyResponse radarCompetency)
        {
            Log.Step(nameof(RadarCompetenciesBasePage), "Enter Competency info");

            //Dimension 
            SelectDimension(radarCompetency.Dimension);

            //Sub Dimension
            SelectSubDimension(radarCompetency.SubDimension);

            //Competency Name
            SetName(radarCompetency.Name);

            //Abbreviation
            if (!string.IsNullOrEmpty(radarCompetency.Abbreviation))
                SetAbbreviation(radarCompetency.Abbreviation);

            //Analytics Abbreviation
            if (!string.IsNullOrEmpty(radarCompetency.AnalyticsAbbreviation))
                Wait.UntilElementClickable(AddEditCompetencyPopupAnalyticsAbbreviationTextbox)
                    .SetText(radarCompetency.AnalyticsAbbreviation);

            //Exclude
            if (!string.IsNullOrEmpty(radarCompetency.Exclude))
                Wait.UntilElementClickable(AddEditCompetencyPopupExcludeTextbox).SetText(radarCompetency.Exclude);

            //Direction
            if (!string.IsNullOrEmpty(radarCompetency.Direction))
                SelectDirection(radarCompetency.Direction);

            //Radar Order
            if (!string.IsNullOrEmpty(radarCompetency.RadarOrder))
                SetRadarOrder(radarCompetency.RadarOrder);

            //Font
            if (!string.IsNullOrEmpty(radarCompetency.Font))
                SelectFont(radarCompetency.Font);

            //Font Size
            if (!string.IsNullOrEmpty(radarCompetency.FontSize))
                SelectFontSize(radarCompetency.FontSize);

            //Letter Spacing 
            if (!string.IsNullOrEmpty(radarCompetency.LetterSpacing))
                SelectLetterSpacing(radarCompetency.LetterSpacing);
        }

        internal void EnterTranslatedCompetencyInfo(Competency radarCompetency)
        {
            Log.Step(nameof(RadarCompetenciesBasePage), "Enter Competency info");

            //Dimension 
            SetTranslatedName(radarCompetency.Name);

            SetTranslatedAbbreviation(radarCompetency.Abbreviation);

            //Translated Analytics Abbreviation
            if (!string.IsNullOrEmpty(radarCompetency.AnalyticsAbbreviation))
                Wait.UntilElementClickable(TranslatedAddEditCompetencyPopupAnalyticsAbbreviationTextbox)
                    .SetText(radarCompetency.AnalyticsAbbreviation);
        }
    }
} 