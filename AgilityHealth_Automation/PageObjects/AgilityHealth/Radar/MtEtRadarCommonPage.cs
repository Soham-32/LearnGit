using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using System.Linq;
using System.Collections.Generic;
using AtCommon.Utilities;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Radar
{
    internal class MtEtRadarCommonPage : BasePage
    {
        public MtEtRadarCommonPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }
        //Filter left nav
        private readonly By AllFilterTabsList = By.XPath("//ul[@class='k-reset k-tabstrip-items']//li");
        private static By FilterCheckBox(string value) => By.CssSelector($"input[tag_name='{value}']");

        //multi-team
        private readonly By HideMultiTeamNamesCheckbox = By.Id("checkHide");
        private readonly By HideIndividualDotsCheckbox = By.Id("hideIndividualDots");
        public readonly By FilterSelectAllLink = By.Id("selectAllDots");
        public readonly By FilterClearAllLink = By.Id("clearAllDots");
        private static By FilterTeamCheckBox(string filterBy) => By.CssSelector($"input[id^='{filterBy}']");
        private static By FilterTeamName(string name) => By.CssSelector($"label[for^='{name}']");

        //Tags
        private readonly By FilterTagsTab = By.CssSelector("a[href='#FilterAway-2']");
        private readonly By FilterCategoriesNames = By.XPath("//div[@id='teamTags']//h4");
        private static By FilterTagsNames(string category) => By.XPath($"//h4[text()='{category}']//following-sibling::ul[1]//label");


        //Roles
        private readonly By FilterRolesTab = By.CssSelector("a[href='#FilterAway-3']");
        private readonly By TeamMembersRoleTags = By.XPath("//div[@id='roleTags']//label");
        private readonly By StakeholderRoleTags = By.XPath("//div[@id='stakeholderTags']//label");

        //Participant Groups
        private readonly By FilterParticipantGroupsTab = By.CssSelector("a[href='#FilterAway-4']");
        private readonly By ParticipantGroupTags = By.Id("pgTags");

        //Radar 
        private static By RadarDotsAvgValue(string dotType, string color, string competency) =>
                By.CssSelector($"circle[class*='{dotType}'][fill='{color}'][competency='{competency}'][val]");
        private static By RadarDotsCount(string dotType, string color, string competency) =>
                By.CssSelector($"circle[class*='{dotType}'][fill='{color}'][competency='{competency}']");


        //Methods

        //Filter left nav
        public List<string> FilterGetAllTabsList()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get all tabs list");
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(AllFilterTabsList).Select(e => e.GetText()).ToList();
        }

        //Multi-Teams 
        public void FilterClickOnSelectAllLink()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Click on 'Select All' link");
            Wait.UntilElementVisible(FilterSelectAllLink).Click();
        }
        public void FilterClickOnClearAllLink()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Click on 'Clear All' link");
            Wait.UntilElementVisible(FilterClearAllLink).Click();
        }
        public void SelectHideMultiTeamNamesCheckbox(bool check = true)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Select 'Hide Team Names' checkbox");
            Wait.UntilElementClickable(HideMultiTeamNamesCheckbox).Check();
            Wait.UntilJavaScriptReady();
        }
        public void SelectHideIndividualDotsCheckbox(bool check = true)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Select 'Hide Individual Dots' checkbox");
            Wait.UntilElementClickable(HideIndividualDotsCheckbox).Check();
            Wait.UntilJavaScriptReady();
        }

        public bool FilterTeamTabIsFilterItemCheckboxSelected(string filterBy) =>
            Wait.UntilElementClickable(FilterTeamCheckBox(filterBy.RemoveWhitespace())).Selected;

        public void FilterTeamTabSelectFilterItemCheckboxByName(string filterBy, bool check = true)
        {
            Log.Step(nameof(MtEtRadarCommonPage), $"On Filter, Team tab, {(check ? "select" : "deselect")} {filterBy}");
            Wait.UntilElementClickable(FilterTeamCheckBox(filterBy.RemoveWhitespace())).Check(check);
            Wait.UntilJavaScriptReady();
        }
        public string FilterGetTeamColor(string filterBy)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get Filter Team color.");
            var tooltip = Wait.UntilElementVisible(FilterTeamName(filterBy.RemoveWhitespace())).GetElementAttribute("tooltipfor");
            return tooltip.Contains("red") ? "red" : "#" + tooltip.Split('#')[1];
        }
        public string GetMultiTeamName(string filterBy)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get Multi team name.");
            return Wait.UntilElementVisible(FilterTeamName(filterBy.RemoveWhitespace())).GetText();
        }

        //Tags 
        public void FilterClickOnTagsTab()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Click on Tags tab");
            Wait.UntilElementClickable(FilterTagsTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public void FilterSelectFilterItemCheckboxByName(string filterBy, bool check = true)
        {
            Log.Step(nameof(MultiTeamRadarPage), $"On Filter, Non Team tab, {(check ? "select" : "deselect")} {filterBy}");
            Wait.UntilElementClickable(FilterCheckBox(filterBy)).Check(check);
            Wait.UntilJavaScriptReady();
        }
        public string FilterGetFilterItemColor(string filterBy) =>
            Wait.UntilElementExists(FilterCheckBox(filterBy)).GetElementAttribute("tag_color");
        public List<string> FilterGetAllCategoryList()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get Filter All category list.");
            return Wait.UntilAllElementsLocated(FilterCategoriesNames).Select(e => e.GetText()).ToList();
        }
        public string FilterGetTagName(string category)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get Filter tag name.");
            return Wait.UntilElementVisible(FilterTagsNames(category)).GetText();
        }

        //Roles 
        public void FilterClickOnRolesTab()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Click on 'Roles' tab");
            Wait.UntilElementClickable(FilterRolesTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public List<string> GetTeamMembersRoleTagsList()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get 'Team members' tags list.");
            return Wait.UntilAllElementsLocated(TeamMembersRoleTags).Select(e => e.GetText()).ToList();
        }
        public List<string> GetStakeholdersRoleTagsList()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get 'Stake holders' tags list.");
            return Wait.UntilAllElementsLocated(StakeholderRoleTags).Select(e => e.GetText()).ToList();
        }

        //Participant Group 
        public void FilterClickOnParticipantGroupsTab()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Click on 'Participant Groups' tab");
            Wait.UntilElementClickable(FilterParticipantGroupsTab).Click();
            Wait.UntilJavaScriptReady();
        }
        public List<string> GetParticipantGroupTagsList()
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get 'Participant Groups' tags list.");
            return Wait.UntilAllElementsLocated(ParticipantGroupTags).Select(e => e.GetText()).ToList();
        }

        public bool IsRadarDotsDisplayed(string dotType, string color, string competency)
        {
            return Driver.IsElementDisplayed(RadarDotsCount(dotType, color, competency), 3);
        }

        //Radar 
        public int GetRadarDotsCount(string dotType, string color, string competency)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get Radar dots count.");
            return Wait.UntilAllElementsLocated(RadarDotsCount(dotType, color, competency)).Where(e => e.Displayed)
                .Select(e => e.GetAttribute("val")).ToList().Count;
        }

        public List<string> GetRadarDotsAvgValue(string dotType, string color, string competency)
        {
            Log.Step(nameof(MtEtRadarCommonPage), "Get radar dots average value.");
            return Wait.UntilAllElementsLocated(RadarDotsAvgValue(dotType, color, competency)).Select(e => e.GetAttribute("val")).ToList();
        }

        public void NavigateToMultiTeamRadarPageForProd(string env, int teamId, int assessmentId, string teamHierarchy = "multiteam")
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/{teamHierarchy}/{teamId}/radar/{assessmentId}");
        }
    }
}


