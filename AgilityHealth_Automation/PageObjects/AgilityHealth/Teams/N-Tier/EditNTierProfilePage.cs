using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Edit;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier
{
    internal class EditNTierProfilePage : EditProfileBasePage
    {
        public EditNTierProfilePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        public NTierTeamInfo GetNTierTeamInfo()
        {
            var team = new NTierTeamInfo
            {
                TeamName = Wait.UntilElementExists(TeamNameTextbox).GetElementAttribute("value"),
                ImagePath = Wait.UntilElementExists(TeamImage).GetElementAttribute("src"),
                Department = Wait.UntilElementExists(DepartmentTextbox).GetElementAttribute("value"),
                DateEstablished = Wait.UntilElementExists(DateEstablishedTextbox).GetElementAttribute("value"),
                AgileAdoptionDate = Wait.UntilElementExists(AgileAdoptionDateTextbox).GetElementAttribute("value"),
                Description = Wait.UntilElementExists(DescriptionTextbox).GetElementAttribute("value"),
                TeamBio = Wait.UntilElementExists(BiographyTextbox).GetElementAttribute("value"),
                ExternalIdentifier = Wait.UntilElementExists(ExternalIdentifierTextbox).GetElementAttribute("value")
            };
            return team;
        }

        public void EnterNTierTeamInfo(NTierTeamInfo nTierTeamInfo)
        {
            Log.Step(nameof(EditNTierProfilePage), "On Edit NTier Team page, Team Profile tab, edit Team Profile");
            Wait.UntilElementClickable(TeamNameTextbox).SetText(nTierTeamInfo.TeamName);
            Wait.UntilElementClickable(ExternalIdentifierTextbox).SetText(nTierTeamInfo.ExternalIdentifier);
            Wait.UntilElementClickable(DepartmentTextbox).SetText(nTierTeamInfo.Department);
            Wait.UntilElementClickable(DateEstablishedTextbox).SetText(nTierTeamInfo.DateEstablished);
            Wait.UntilElementClickable(AgileAdoptionDateTextbox).SetText(nTierTeamInfo.AgileAdoptionDate);
            Wait.UntilElementClickable(DescriptionTextbox).SetText(nTierTeamInfo.Description);
            Wait.UntilElementClickable(BiographyTextbox).SetText(nTierTeamInfo.TeamBio);
            Wait.UntilElementExists(ImagePathField).SetText(nTierTeamInfo.ImagePath, false);
            Wait.UntilElementExists(ImageUploadDone);
        }
    }
}