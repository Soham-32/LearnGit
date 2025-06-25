using System;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam
{
    public class EditEtProfilePage : EditProfileBasePage
    {
        public EditEtProfilePage(IWebDriver driver, ILogger log) : base(driver, log) { }

        public EnterpriseTeamInfo GetEnterpriseTeamInfo()
        {
            EnterpriseTeamInfo team = new EnterpriseTeamInfo
            {
                TeamName = Wait.UntilElementExists(TeamNameTextbox).GetElementAttribute("value"),
                TeamType = Wait.UntilElementExists(WorkTypeTextbox).GetText(),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextbox).GetElementAttribute("value"),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src"),
                Department = Wait.UntilElementExists(DepartmentTextbox).GetElementAttribute("value"),
                DateEstablished = DateTime.Parse(Wait.UntilElementExists(DateEstablishedTextbox).GetElementAttribute("value")),
                AgileAdoptionDate = DateTime.Parse(Wait.UntilElementExists(AgileAdoptionDateTextbox).GetElementAttribute("value")),
                Description = Wait.UntilElementExists(DescriptionTextbox).GetElementAttribute("value"),
                TeamBio = Wait.UntilElementExists(BiographyTextbox).GetElementAttribute("value")
            };

            return team;
        }


        public void EnterEnterpriseTeamInfo(EnterpriseTeamInfo enterpriseTeamInfo)
        {
            Log.Step(nameof(EditEtProfilePage), "On Edit Team page, Team Profile tab, edit Team Profile");
            Wait.UntilElementClickable(TeamNameTextbox).SetText(enterpriseTeamInfo.TeamName);
            SelectWorkType(enterpriseTeamInfo.TeamType);

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
                Wait.UntilElementClickable(BiographyTextbox).SetText(enterpriseTeamInfo.TeamBio);
            }
            if (!string.IsNullOrEmpty(enterpriseTeamInfo.ImagePath))
            {
                Wait.UntilElementExists(ImagePathField).SetText(enterpriseTeamInfo.ImagePath, false);
                Wait.UntilJavaScriptReady();
            }
        }
    }
}
