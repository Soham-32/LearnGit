using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam
{
    public class CreateEnterpriseTeamPage : BasePage
    {
        public CreateEnterpriseTeamPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By TeamNameTextbox = By.Id("TeamName");
        private readonly By TeamTypeListbox = By.CssSelector("span[aria-owns='EnterpriseTeamType_listbox']");
        private static By TeamTypeListItem(string item) => By.XPath($"//ul[@id='EnterpriseTeamType_listbox']/li[text()='{item}' or .//*[text()='{item}']]");
        private readonly By ExternalIdentifierTextbox = By.Id("ExternalIdentifier");
        private readonly By DepartmentTextbox = By.Id("Department");
        private const string DateEstablishedId = "TeamFormedDate_dateview";
        private const string AgileAdoptionDateId = "AgileAdoptionDate_dateview";
        private readonly By DescriptionTextbox = By.Id("Description");
        private readonly By TeamBioTextbox = By.Id("Biography");
        private readonly By ImageUploadInput = By.Id("file");
        private readonly By ResetPhotoLink = By.Id("reset");
        private readonly By TeamImage = By.Id("preview");
        private readonly By BusinessLinesListBox = By.XPath("//div[@class='form-group']/label[contains(text(),'Business Lines')]/following-sibling::div/div");
        private static By BusinessLinesListItem(string item) => By.XPath($"//li[text()='{item}'] | //li//font[text()='{item}']");
        private readonly By CreateTeamAndAddSubTeamButton = By.CssSelector("input.green-btn.done");

        public void EnterEnterpriseTeamInfo(EnterpriseTeamInfo enterpriseTeamInfo)
        {
            Log.Step(nameof(CreateEnterpriseTeamPage), "On Add A Team page, enter team info");
            Wait.UntilElementClickable(TeamNameTextbox).SetText(enterpriseTeamInfo.TeamName);
            SelectItem(TeamTypeListbox, TeamTypeListItem(enterpriseTeamInfo.TeamType));

            if (!string.IsNullOrEmpty(enterpriseTeamInfo.ExternalIdentifier))
            {
                Wait.UntilElementClickable(ExternalIdentifierTextbox).SetText(enterpriseTeamInfo.ExternalIdentifier);
            }
            if (!string.IsNullOrEmpty(enterpriseTeamInfo.Department))
            {
                Wait.UntilElementClickable(DepartmentTextbox).SetText(enterpriseTeamInfo.Department);
            }
            if (enterpriseTeamInfo.DateEstablished.CompareTo(new DateTime()) != 0)
            {
                var establishedCal = new CalendarWidget(Driver, DateEstablishedId);
                establishedCal.SetMonth(enterpriseTeamInfo.DateEstablished);
            }
            if (enterpriseTeamInfo.AgileAdoptionDate.CompareTo(new DateTime()) != 0)
            {
                var adoptionCal = new CalendarWidget(Driver, AgileAdoptionDateId);
                adoptionCal.SetMonth(enterpriseTeamInfo.AgileAdoptionDate);
            }
            if (!string.IsNullOrEmpty(enterpriseTeamInfo.Description))
            {
                Wait.UntilElementClickable(DescriptionTextbox).SetText(enterpriseTeamInfo.Description);
            }
            if (!string.IsNullOrEmpty(enterpriseTeamInfo.TeamBio))
            {
                Wait.UntilElementClickable(TeamBioTextbox).SetText(enterpriseTeamInfo.TeamBio);
            }
            if (!string.IsNullOrEmpty(enterpriseTeamInfo.ImagePath))
            {
                Wait.UntilElementExists(ImageUploadInput).SetText(enterpriseTeamInfo.ImagePath, false);
                Wait.HardWait(2000); // Takes time to load image.
                Wait.UntilElementExists(ResetPhotoLink);
            }

            SelectItem(BusinessLinesListBox, BusinessLinesListItem(SharedConstants.TeamTag));

        }

        public void GoToAddSubteams()
        {
            Log.Step(nameof(CreateEnterpriseTeamPage), "On Add A Team page, click 'Create Team & Add Sub-Teams' button");
            Wait.UntilElementClickable(CreateTeamAndAddSubTeamButton).Click();
        }

        public string GetTeamImageSource()
        {
            return Wait.UntilElementVisible(TeamImage).GetElementAttribute("src");
        }

        public bool IsTeamTypePresent(string teamType)
        {
            Log.Step(nameof(CreateEnterpriseTeamPage), $"Is {teamType} Present? ");
            return Driver.IsElementPresent(TeamTypeListItem(teamType));
        }

        public void NavigateToPage(string companyId)
        {
            Log.Step(nameof(CreateEnterpriseTeamPage), $"Navigate to create enterprise team page for company id ${companyId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/enterprise/{companyId}/create");
        }
    }
}
