using System;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Base
{
    internal class RadarReviewAndFinishBasePage : RadarHeaderBasePage
    {
        public RadarReviewAndFinishBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        private static By SectionOptions(string option) => By.XPath($"//a[text()=\"{option}\"]");
        public void SelectSectionOption(string option)
        {
            Log.Step(nameof(RadarReviewAndFinishBasePage), $"Select section option {option}");
            Wait.UntilElementClickable(SectionOptions(option)).Click();
        }
        public enum SelectAction
        {
            PreviewBlankRadar,
            PreviewAssessment,
            EditRadar,
            IamDone
        }
        public void SelectActionType(SelectAction actionType)
        {
            Log.Step(nameof(RadarReviewAndFinishBasePage), $"Select Action type {actionType}");
            switch (actionType)
            {
                case SelectAction.PreviewBlankRadar:
                    SelectSectionOption(" Preview Blank Radar");
                    break;
                case SelectAction.PreviewAssessment:
                    SelectSectionOption(" Preview Assessment");
                    break;
                case SelectAction.EditRadar:
                    SelectSectionOption(" Edit Radar");
                    break;
                case SelectAction.IamDone:
                    SelectSectionOption("I'm Done!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }
        }
    }
}