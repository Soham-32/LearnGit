using System.Threading;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using AgilityHealth_Automation.Base;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class TeamBasePage : BasePage
    {
        public TeamBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Create Team stepper
        private readonly By CreateTeamStepperTitleText = By.XPath("//div[@class='createTeamContent']//h3");
        private readonly By CreateTeamStepperInfoText = By.XPath("//div[@class='page-title-description hidden-xs']/span");
        #endregion

        #region Multi Team Create Team stepper
        // 'Continue' button
        private readonly By ContinueToAddSubTeamsOrTeamMembersButton = By.XPath("//div//input[@value='Continue to Add Sub-Teams'] | //div//input[@value='Continue to Team Members']");
        #endregion

        #region Tab section
        private readonly By TeamMemberPageTitleText = By.CssSelector("#teamMembersTab .ah-update-team-banner .headingText");
        private readonly By SubTeamPageTitleText = By.CssSelector("#subTeamsTab .ah-update-team-banner .headingText");
        private readonly By TeamProfilePageTitleText = By.XPath("//div[@id='teamProfileTab']//span[@class='headingText']");
        private readonly By TeamMembersTab = By.Id("teamMembersTab_click");
        private readonly By StakeholdersTab = By.Id("stakeHoldersTab_click");
        #endregion

        #region Team section
        // Create Team
        public readonly By TeamName = By.Id("TeamName");
        private readonly By WorkTypeDropDown = By.XPath("//label[@for='WorkType']//following-sibling::div//span[contains(@aria-owns,'TypeId_listbox')] | //label//following-sibling::div//span[contains(@aria-owns,'Type_listbox')]//span[@class='k-select']");
        private readonly By PreferredLanguageDropDown = By.CssSelector("span[aria-owns='IsoLanguageCode_listbox']");
        private readonly By MethodologyDropDown = By.XPath("//label[@for='Methodology']/following-sibling::div/span");
        public readonly By ExternalIdentifierTextBox = By.Id("ExternalIdentifier");
        public readonly By DepartmentOrGroupTextbox = By.Id("Department");
        public readonly By DateEstablishedTextbox = By.Id("TeamFormedDate");
        public readonly By TeamBioOrBackgroundTextArea = By.Id("Biography");
        private readonly By TeamImage = By.Id("preview");
        private readonly By ImagePathField = By.Id("file");
        private readonly By ImageUploadDone = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By EmptyWorkTypeOption = By.XPath("//div[text()='Select One']");
        private static By WorkTypeDropDownItem(string workType) => By.XPath($"//ul[contains(@id,'Type_listbox')]/li[text()='{workType}']");
        private static By PreferredLanguageDropDownItem(string preferredLanguage) => By.XPath($"//ul[@id='IsoLanguageCode_listbox']/li[text()='{preferredLanguage}']");
        private static By MethodologyDropDownItem(string methodology) => By.XPath($"//li[text()='{methodology}']");

        // Edit Team
        private readonly By UpdateTeamProfileButton = By.CssSelector(".inner-content-container ~ div .EditTeamAndAddTeamMembers");
        #endregion

        #region Manage Team Tag section
        private readonly By ManageTeamTagExpandCollapseSection = By.XPath("//div[@id='headingOne']//h4");
        private static By TeamTagListBox(string tagName) => By.XPath($"//label[contains(@for,'Tags') and contains(text(), '{tagName}')]/..//div[contains(@class, 'k-floatwrap')]");
        private static By TeamTagCoachingListItem(string item) => By.XPath($"//ul[contains(@id, 'Tags')]/li[text() = '{item}']");
        private static By SelectedTagRemoveIcon(string key, string value) => By.XPath($"//label[contains(text(),'{key}')]/..//ul/li/span[text()='{value}']/../span/span");
        #endregion

        #region Team Profile Tab
        public readonly By ProfileWorkTypeDropDown = By.XPath("//span[@aria-owns='WorkTypeId_listbox']/span/span[1]");
        #endregion

        #endregion



        #region Methods

        #region Create Team stepper
        // Getting text
        public string GetCreateTeamStepperTitle()
        {
            Log.Step(nameof(TeamBasePage), "On Create Team Stepper, get create team stepper title");
            SwitchToIframeForNewNav();
            return Wait.UntilElementVisible(CreateTeamStepperTitleText).GetText();
        }
        public string GetCreateTeamStepperInfo()
        {
            Log.Step(nameof(TeamBasePage), "On Create Team Stepper, get create team stepper info");
            return Wait.UntilElementVisible(CreateTeamStepperInfoText).GetText();
        }
        public void RemoveWorkType()
        {
            Log.Step(nameof(TeamBasePage), "Remove work type item");
            Wait.UntilElementClickable(WorkTypeDropDown).Click();
            Wait.UntilElementVisible(EmptyWorkTypeOption);
            Wait.UntilElementClickable(EmptyWorkTypeOption).Click();
        }
        #endregion

        #region Tab section
        public void ClickOnTeamMembersTab()
        {
            Log.Step(GetType().Name, "Click on Team Members tab");
            Wait.UntilElementVisible(TeamMembersTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickOnStakeholderTab()
        {
            Log.Step(GetType().Name, "Click on Stakeholder tab");
            Wait.UntilElementVisible(StakeholdersTab).Click();
            Wait.UntilJavaScriptReady();
        }

        public string GetActiveTeamMemberPageTitle()
        {
            return Wait.UntilElementVisible(TeamMemberPageTitleText).GetText();
        }

        public string GetActiveSubTeamPageTitle()
        {
            return Wait.UntilElementVisible(SubTeamPageTitleText).GetText();
        }

        public string GetActiveTeamProfilePageTitle()
        {
            return Wait.UntilElementVisible(TeamProfilePageTitleText).GetText();
        }
        #endregion

        #region Team section

        #region Team Create Team stepper

        public bool IsContinueToAddSubTeamsOrTeamMembersButtonEnabled()
        {
            Wait.UntilElementVisible(ContinueToAddSubTeamsOrTeamMembersButton);
            return Driver.IsElementEnabled(ContinueToAddSubTeamsOrTeamMembersButton);
        }
        // 'Continue' button
        public void ClickOnContinueToSubTeamOrTeamMemberButton()
        {
            Log.Step(nameof(TeamBasePage), "Click on 'Continue To Sub Team' button");
            Wait.UntilElementVisible(ContinueToAddSubTeamsOrTeamMembersButton);
            Driver.JavaScriptScrollToElement(ContinueToAddSubTeamsOrTeamMembersButton);
            Thread.Sleep(500);// Wait till 'Continue To Sub Team' button is enabled
            Wait.UntilElementClickable(ContinueToAddSubTeamsOrTeamMembersButton).Click();
        }
        #endregion

        public void EnterTeamInfo(Team teamInfo)
        {
            Log.Step(GetType().Name, "Enter team info");

            SwitchToIframeForNewNav();
            EnterTeamName(teamInfo.TeamName);

            SelectWorkType(teamInfo.WorkType);


            if (!string.IsNullOrEmpty(teamInfo.Methodology))
            {
                SelectMethodology(teamInfo.Methodology);
            }
            if (!string.IsNullOrEmpty(teamInfo.PreferredLanguage))
            {
                SelectPreferredLanguage(teamInfo.PreferredLanguage);
            }
            if (!string.IsNullOrEmpty(teamInfo.ExternalIdentifier))
            {
                EnterExternalIdentifier(teamInfo.ExternalIdentifier);
            }
            if (!string.IsNullOrEmpty(teamInfo.DepartmentAndGroup))
            {
                EnterDepartmentOrGroup(teamInfo.DepartmentAndGroup);
            }
            if (!string.IsNullOrEmpty(teamInfo.DateEstablished))
            {
                EnterDateEstablished(teamInfo.DateEstablished);
            }
            if (!string.IsNullOrEmpty(teamInfo.TeamBioOrBackground))
            {
                EnterTeamBioOrBackground(teamInfo.TeamBioOrBackground);
            }
            if (!string.IsNullOrEmpty(teamInfo.ImagePath))
            {
                Wait.UntilElementExists(ImagePathField).SendKeys(teamInfo.ImagePath);
                Wait.UntilElementExists(ImageUploadDone);
            }
            if (teamInfo.Tags != null)
            {
                SelectTeamTags(teamInfo);
            }
        }

        public void EnterTeamName(string teamName)
        {
            Log.Step(nameof(TeamBasePage), "Enter team name");
            Wait.UntilElementClickable(TeamName).SetText(teamName);
        }
        public void SelectWorkType(string workType)
        {
            if (!string.IsNullOrEmpty(workType))
            {
                SelectItem(WorkTypeDropDown, WorkTypeDropDownItem(workType));
            }
        }
        public void SelectPreferredLanguage(string preferredLanguage)
        {
            SelectItem(PreferredLanguageDropDown, PreferredLanguageDropDownItem(preferredLanguage));
        }
        public void SelectMethodology(string methodology)
        {
            SelectItem(MethodologyDropDown, MethodologyDropDownItem(methodology));
        }
        public void EnterExternalIdentifier(string externalIdentifier)
        {
            Log.Step(nameof(TeamBasePage), "Enter external identifier");
            if (Driver.IsElementDisplayed(ExternalIdentifierTextBox))
            {
                Wait.UntilElementClickable(ExternalIdentifierTextBox).SetText(externalIdentifier);
            }
        }
        public void EnterDepartmentOrGroup(string departmentOrGroup)
        {
            Log.Step(nameof(TeamBasePage), "Enter department/group");
            Wait.UntilElementClickable(DepartmentOrGroupTextbox).SetText(departmentOrGroup);
        }
        public void EnterDateEstablished(string date)
        {
            Log.Step(nameof(TeamBasePage), "Enter date established");
            Wait.UntilElementClickable(DateEstablishedTextbox).SetText(date);
        }
        public void EnterTeamBioOrBackground(string teamBioOrBackground)
        {
            Log.Step(nameof(TeamBasePage), "Enter 'Team Bio/Background'");
            Wait.UntilElementClickable(TeamBioOrBackgroundTextArea).SetText(teamBioOrBackground);
        }
        #endregion

        #region Team Tag section
        public void SelectTeamTags(Team teamInfo)
        {
            Log.Step(nameof(TeamBasePage), "Select Team Tags");
            foreach (var tag in teamInfo.Tags)
            {
                Driver.JavaScriptScrollToElement(ManageTeamTagExpandCollapseSection);
                if (Wait.UntilElementExists(ManageTeamTagExpandCollapseSection).GetAttribute("class") == "collapsed")
                {
                    Wait.HardWait(3000);
                    Wait.UntilElementClickable(ManageTeamTagExpandCollapseSection).Click();
                }

                Driver.JavaScriptScrollToElement(TeamTagListBox(tag.Key));
                SelectItem(TeamTagListBox(tag.Key), TeamTagCoachingListItem(tag.Value));
            }
        }
        public void ClickOnSelectedTagsRemoveIcon(string key, string value)
        {
            Log.Step(nameof(TeamBasePage), "Delete selected team tags");
            Wait.UntilElementVisible(SelectedTagRemoveIcon(key, value));
            Driver.JavaScriptScrollToElement(SelectedTagRemoveIcon(key, value));
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(SelectedTagRemoveIcon(key, value)).Click();
        }

        public string GetTeamImage()
        {
            return Wait.UntilElementExists(TeamImage).GetAttribute("src");
        }

        public void ClickOnUpdateTeamProfile()
        {
            Log.Step(nameof(TeamBasePage), "Click on Update Team Profile");
            Wait.UntilElementVisible(UpdateTeamProfileButton).Click();
        }
        #endregion

        #region Multi Team Profile Section

        public Team GetMultiTeamInfo()
        {
            var teamInfo = new Team
            {
                TeamName = Wait.UntilElementExists(TeamName).GetElementAttribute("value"),
                WorkType = Wait.UntilElementExists(ProfileWorkTypeDropDown).GetText(),
                DepartmentAndGroup = Wait.UntilElementExists(DepartmentOrGroupTextbox).GetElementAttribute("value"),
                TeamBioOrBackground = Wait.UntilElementExists(TeamBioOrBackgroundTextArea).GetElementAttribute("value"),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextBox).GetElementAttribute("value"),
            };

            return teamInfo;
        }
        #endregion

        #endregion


    }
}
