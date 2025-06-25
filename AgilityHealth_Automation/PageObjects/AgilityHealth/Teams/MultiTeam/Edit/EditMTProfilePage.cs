using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Edit
{
    public class EditMtProfilePage : EditProfileBasePage
    {

        public EditMtProfilePage(IWebDriver driver, ILogger log) : base(driver, log) { }
        
        
        public MultiTeamInfo GetMultiTeamInfo()
        {
            MultiTeamInfo team = new MultiTeamInfo
            {
                TeamName = Wait.UntilElementExists(TeamNameTextbox).GetElementAttribute("value"),
                TeamType = Wait.UntilElementExists(WorkTypeTextbox).GetText(),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src"),
                Department = Wait.UntilElementExists(DepartmentTextbox).GetElementAttribute("value"),
                DateEstablished = Wait.UntilElementExists(DateEstablishedTextbox).GetElementAttribute("value"),
                AgileAdoptionDate = Wait.UntilElementExists(AgileAdoptionDateTextbox).GetElementAttribute("value"),
                Description = Wait.UntilElementExists(DescriptionTextbox).GetElementAttribute("value"),
                TeamBio = Wait.UntilElementExists(BiographyTextbox).GetElementAttribute("value")
            };

            return team;
        }


        public void EnterMultiTeamInfo(MultiTeamInfo multiTeamInfo)
        {
            Log.Step(nameof(EditMtProfilePage), "On Edit Team page, Team Profile tab, edit Team Profile");
            Wait.UntilElementClickable(TeamNameTextbox).SetText(multiTeamInfo.TeamName);
            SelectWorkType(multiTeamInfo.TeamType);
            Wait.UntilElementClickable(DepartmentTextbox).SetText(multiTeamInfo.Department);
            Wait.UntilElementClickable(DateEstablishedTextbox).SetText(multiTeamInfo.DateEstablished);
            Wait.UntilElementClickable(AgileAdoptionDateTextbox).SetText(multiTeamInfo.AgileAdoptionDate);
            Wait.UntilElementClickable(DescriptionTextbox).SetText(multiTeamInfo.Description);
            Wait.UntilElementClickable(BiographyTextbox).SetText(multiTeamInfo.TeamBio);
            Wait.UntilElementExists(ImagePathField).SetText(multiTeamInfo.ImagePath, false);
            Wait.UntilElementExists(ImageUploadDone);
        }

    }

}
