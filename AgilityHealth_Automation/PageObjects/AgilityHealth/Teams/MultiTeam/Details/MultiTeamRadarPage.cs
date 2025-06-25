using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details
{
    internal class MultiTeamRadarPage : BasePage
    {
        public MultiTeamRadarPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        //Radar
        private readonly By MtRadarViewDropdownArrow = By.CssSelector("span[aria-owns='radarMultiSelect_listbox']");
        private static By MtRadarViewItem(string item) => By.XPath(
            $"//ul[@id = 'radarMultiSelect_listbox']/li[text() = '{item}']");
        private readonly By EtRadarViewDropdownArrow = By.CssSelector("span[aria-owns='radarEnterpriseSelect_listbox']");
        private static By EtRadarViewItem(string item) => By.XPath(
            $"//ul[@id = 'radarEnterpriseSelect_listbox']/li[text() = '{item}']");

        private static By RadarDotsCountLocator(string dotType, string color, string competency) =>
            By.CssSelector($"circle[class*='{dotType}'][fill='{color}'][competency='{competency}']");
        private static By RadarDotsAvgValueLocator(string dotType, string color, string competency) =>
        By.CssSelector($"circle[class*='{dotType}'][fill='{color}'][competency='{competency}'][val]");

        //Filter
        private readonly By FilterTagsTab = By.CssSelector("a[href='#FilterAway-2']");
        private readonly By FilterRolesTab = By.CssSelector("a[href='#FilterAway-3']");
        private readonly By FilterParticipantGroupsTab = By.CssSelector("a[href='#FilterAway-4']");
        private static By TeamFilterCheckBox(string filterBy) => By.CssSelector($"input[id^='{filterBy}']");
        private static By TagFilterCheckBox(string filterBy) => By.CssSelector($"input[tag_name='{filterBy}']");
        private readonly By TeamNameLabels = By.XPath("//ul[@id='ulTeams']/li[@class='teamList']//label");
        private static By TeamNameLabel(string name) => By.CssSelector($"label[for^='{name}']");

        //Teams Tab
        private readonly By HideTeamNamesCheckbox = By.Id("checkHide");
        private readonly By HideIndividualDotsCheckbox = By.Id("hideIndividualDots");

        private readonly By BenchmarkLastUpdate = By.XPath("//div[@id = 'legend']/span[2]");


        //Radar
        /// <summary>
        /// Get the values of plotted dots on radar
        /// </summary>
        /// <param name="dotType">dots,avg</param>
        /// <param name="color">hex code of color</param>
        /// <param name="competency">competency name</param>
        /// <returns>dot value</returns>
        public int GetRadarDotsCount(string dotType, string color, string competency)
        {
            return Wait.UntilAllElementsLocated(RadarDotsCountLocator(dotType, color, competency)).Where(e => e.Displayed)
                    .Select(e => e.GetAttribute("val")).ToList().Count;
        }
        public List<string> GetRadarDotsAvgValue(string dotType, string color, string competency)
        {
            return Wait.UntilAllElementsLocated(RadarDotsAvgValueLocator(dotType, color, competency)).Select(e => e.GetAttribute("val")).ToList();
        }
        public void RadarSwitchView(string view)
        {
            Log.Step(nameof(MultiTeamRadarPage), "Select Benchmarking view");
            SelectItem(MtRadarViewDropdownArrow, MtRadarViewItem(view));
        }
        public void EtRadarSwitchView(string view)
        {
            SelectItem(EtRadarViewDropdownArrow, EtRadarViewItem(view));
        }
        public void Radar_SwitchToBenchmarkingView()
        {
            RadarSwitchView("Benchmarking");
        }

        //Filter
        public void Filter_ClickOnTagsTab()
        {
            Log.Step(nameof(MultiTeamRadarPage), "Click on Tags tab");
            Wait.UntilElementClickable(FilterTagsTab).Click();
        }

        public void Filter_ClickOnRolesTab()
        {
            Log.Step(nameof(MultiTeamRadarPage), "Click on Roles tab");
            Wait.UntilElementClickable(FilterRolesTab).Click();
        }

        public void Filter_ClickOnParticipantGroupsTab()
        {
            Log.Step(nameof(MultiTeamRadarPage), "Click on Participant Groups tab");
            Wait.UntilElementClickable(FilterParticipantGroupsTab).Click();
        }


        //Team Tab
        public void SelectHideTeamNamesCheckbox()
        {
            Log.Step(nameof(MultiTeamRadarPage), "Select 'Hide Team Names' checkbox");
            Wait.UntilElementClickable(HideTeamNamesCheckbox).Click();
            Wait.UntilJavaScriptReady();
        }

        public void SelectHideIndividualDotsCheckbox()
        {
            Log.Step(nameof(MultiTeamRadarPage), "Select 'Hide Individual Dots' checkbox");
            Wait.UntilElementClickable(HideIndividualDotsCheckbox).Click();
            Wait.UntilJavaScriptReady();
        }

        public void Filter_TeamTab_SelectFilterItemCheckboxByName(string filterBy, bool check = true)
        {
            Log.Step(nameof(MultiTeamRadarPage), $"On Filter, Team tab, {(check ? "select":"deselect")} {filterBy}");
            Wait.UntilElementClickable(TeamFilterCheckBox(filterBy.RemoveWhitespace())).Check(check);
            Wait.UntilJavaScriptReady();
        }

        public bool Filter_TeamTab_IsFilterItemCheckboxSelected(string filterBy) =>
            Wait.UntilElementClickable(TeamFilterCheckBox(filterBy.RemoveWhitespace())).Selected;

        public string Filter_TeamTab_GetFilterItemColor(string filterBy)
        {
            var tooltip = Wait.UntilElementVisible(TeamNameLabel(filterBy.RemoveWhitespace())).GetElementAttribute("tooltipfor");
            return tooltip.Contains("red") ? "red" : "#" + tooltip.Split('#')[1];
        }

        public bool Filter_TeamTab_DoesTeamExist(string teamName)
        {
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamNameLabels).Any(element => element.GetText().Equals(teamName));
        }


        //Tags/Roles/Participant Groups tab
        public void Filter_NonTeamTab_SelectFilterItemCheckboxByName(string filterBy, bool check = true)
        {
            Log.Step(nameof(MultiTeamRadarPage), $"On Filter, Non Team tab, {(check ? "select" : "deselect")} {filterBy}");
            Wait.UntilElementClickable(TagFilterCheckBox(filterBy)).Check(check);
            Wait.UntilJavaScriptReady();
        }

        public string Filter_NonTeamTab_GetFilterItemColor(string filterBy) =>
            Wait.UntilElementExists(TagFilterCheckBox(filterBy)).GetElementAttribute("tag_color");

        public DateTime GetBenchmarkLastUpdatedDate()
        {
            var date = Wait.UntilElementVisible(BenchmarkLastUpdate).GetText();

            date = date.Remove(0, 15).Replace(")", "");

            return DateTime.Parse(date);

        }

        public void NavigateToPage(int teamId, int radarId, bool isEt = false)
        {
            var teamType = isEt ? "enterprise" : "multiteam";
            NavigateToUrl($"{BaseTest.ApplicationUrl}/{teamType}/{teamId}/radar/{radarId}");
        }

    }
}