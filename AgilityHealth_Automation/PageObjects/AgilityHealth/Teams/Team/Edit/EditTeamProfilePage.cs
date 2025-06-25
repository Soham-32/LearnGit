using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Edit
{
    internal class EditTeamProfilePage : EditProfileBasePage
    {
        public EditTeamProfilePage(IWebDriver driver, ILogger log) : base(driver, log)  
        {
        }

        private readonly By MethodologyTextbox = By.CssSelector("span[aria-owns='MethodologyId_listbox'] span[class='k-input']");
        private static By MethodologyListItem(string item) => By.XPath($"//ul[@id='MethodologyId_listbox']/li[text()='{item}']");

        private readonly By WorkType = By.CssSelector("span[aria-owns='WorkTypeId_listbox']");
        private readonly By WorkTypeList = By.XPath("//ul[@id='WorkTypeId_listbox']//li");

        private readonly By TeamTagStrategicObjectivesList = By.XPath("//ul[@id='Tags_1__Values_listbox']//li");
        private readonly By TeamTagStrategicObjectives = By.XPath("//label[@for='Tags_1__Values']/..//div[@class='k-widget k-multiselect k-header']");

        private readonly By TeamTagCoachingList = By.XPath("//ul[@id='Tags_2__Values_listbox']//li");
        private readonly By TeamTagCoaching = By.XPath("//label[@for='Tags_2__Values']/..//div[@class='k-widget k-multiselect k-header']");

        private readonly By TeamTagBusinessLineList = By.XPath("//ul[@id='Tags_0__Values_listbox']//li");
        private readonly By TeamTagBusinessLine = By.XPath("//label[@for='Tags_0__Values']/..//div[@class='k-widget k-multiselect k-header']");
        private readonly By AllLanguagesList = By.XPath("//ul[@id='IsoLanguageCode_listbox']/li[@role='option']");


        public void WaitForTeamDetailsPageToLoad()
        {
            Wait.UntilElementEnabled(UpdateTeamProfileButton);
        }
        public TeamInfo GetTeamInfo()
        {
            Wait.UntilJavaScriptReady();
            var teamInfo = new TeamInfo
            {
                TeamName = Wait.UntilElementExists(TeamNameTextbox).GetElementAttribute("value"),
                WorkType = Wait.UntilElementExists(WorkTypeTextbox).GetText(),
                PreferredLanguage = Wait.UntilElementExists(PreferredLanguage).GetText(),
                Methodology = Wait.UntilElementExists(MethodologyTextbox).GetText(),
                Department = Wait.UntilElementExists(DepartmentTextbox).GetElementAttribute("value"),
                DateEstablished = Wait.UntilElementExists(DateEstablishedTextbox).GetElementAttribute("value"),
                AgileAdoptionDate = Wait.UntilElementExists(AgileAdoptionDateTextbox).GetElementAttribute("value"),
                Description = Wait.UntilElementExists(DescriptionTextbox).GetElementAttribute("value"),
                TeamBio = Wait.UntilElementExists(BiographyTextbox).GetElementAttribute("value"),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src")
            };

            return teamInfo;
        }

        public void EnterTeamInfo(TeamInfo teamInfo)
        {
            if (!string.IsNullOrEmpty(teamInfo.TeamName))
            {
                Wait.UntilElementClickable(TeamNameTextbox).SetText(teamInfo.TeamName);
            }

            if (!string.IsNullOrEmpty(teamInfo.WorkType))
            {
                SelectWorkType(teamInfo.WorkType);
            }

            if (!string.IsNullOrEmpty(teamInfo.PreferredLanguage))
            {
                SelectPreferredLanguage(teamInfo.PreferredLanguage);
            }

            if (!string.IsNullOrEmpty(teamInfo.Methodology))
            {
                SelectMethodology(teamInfo.Methodology);
            }

            if (!string.IsNullOrEmpty(teamInfo.Department))
            {
                Wait.UntilElementClickable(DepartmentTextbox).SetText(teamInfo.Department);
            }

            if (!string.IsNullOrEmpty(teamInfo.DateEstablished))
            {
                Wait.UntilElementClickable(DateEstablishedTextbox).SetText(teamInfo.DateEstablished);
            }

            if (!string.IsNullOrEmpty(teamInfo.AgileAdoptionDate))
            {
                Wait.UntilElementClickable(AgileAdoptionDateTextbox).SetText(teamInfo.AgileAdoptionDate);
            }

            if (!string.IsNullOrEmpty(teamInfo.Description))
            {
                Wait.UntilElementClickable(DescriptionTextbox).SetText(teamInfo.Description);
            }

            if (!string.IsNullOrEmpty(teamInfo.TeamBio))
            {
                Wait.UntilElementClickable(BiographyTextbox).SetText(teamInfo.TeamBio);
            }

            if (string.IsNullOrEmpty(teamInfo.ImagePath)) return;
            if (Driver.IsInternetExplorer())
            {
                Wait.UntilElementExists(TeamImageClass).Click();
                Wait.UntilJavaScriptReady();
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AutoIt\\Teams\\TeamFileUpload.exe");
                var commandLineArguments = teamInfo.ImagePath.Replace(" ", "*");
                CSharpHelpers.RunExternalExe(filePath, commandLineArguments);
            }
            else
            {
                Wait.UntilElementExists(ImagePathField).SetText(teamInfo.ImagePath,false);
            }

            Wait.UntilElementExists(ImageUploadDone);
        }

        public void SelectMethodology(string methodology)
        {
            Log.Step(GetType().Name, "On Edit Team page, Team Profile tab, edit team info");
            SelectItem(MethodologyTextbox, MethodologyListItem(methodology));
        }

        public List<string> GetAllLanguages()
        {
            Log.Step(nameof(EditTeamProfilePage), "Get All Languages from Language dropdown");
            Wait.UntilElementClickable(PreferredLanguage).Click();
            Wait.UntilJavaScriptReady();
            var getHeaderLanguageAllValue = Driver.GetTextFromAllElements(AllLanguagesList).ToList();
            Wait.UntilElementClickable(PreferredLanguage).Click();
            return getHeaderLanguageAllValue;
        }

        public string GetExternalIdentifier()
        {
            return Wait.UntilElementVisible(ExternalIdentifierTextbox).GetText();
        }
        public List<string> GetWorkTypeDropdownList()
        {
            Log.Step(GetType().Name, "Get team's work type list");
            Wait.UntilElementClickable(WorkType).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(WorkTypeList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetStrategicObjectivesDropdownList()
        {
            Log.Step(GetType().Name, "Get team's strategic objectives list");
            Wait.UntilElementClickable(TeamTagStrategicObjectives).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagStrategicObjectivesList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetCoachingDropdownList()
        {
            Log.Step(GetType().Name, "Get team's coaching list");
            Wait.UntilElementClickable(TeamTagCoaching).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagCoachingList).Select(row => row.GetText()).ToList();
        }
        public List<string> GetBusinessLineDropdownList()
        {
            Log.Step(GetType().Name, "Get team's business line list");
            Wait.UntilElementClickable(DescriptionTextbox).Click();
            Wait.UntilElementClickable(TeamTagBusinessLine).Click();
            Wait.UntilJavaScriptReady();
            return Wait.UntilAllElementsLocated(TeamTagBusinessLineList).Select(row => row.GetText()).ToList();
        }

    }
}