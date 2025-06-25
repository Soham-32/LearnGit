using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create
{

    public class CreateMultiTeamPage : BasePage
    {
        public CreateMultiTeamPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }


        private readonly By MultiTeamName = By.Id("TeamName");
        private readonly By MultiTeamType = By.CssSelector("span[aria-owns='MultiTeamType_listbox']");
        private readonly By AssessmentType = By.CssSelector("span[aria-owns='SurveyId_listbox']");
        private readonly By CreateTeamAndAddSubTeamButton = By.XPath("//input[contains(@value,'Create Team')]");
        private readonly By DepartmentTextbox = By.Id("Department");
        private readonly By DateEstablishedTextbox = By.Id("TeamFormedDate");
        private readonly By AgileAdoptionDateTextbox = By.Id("AgileAdoptionDate");
        private readonly By DescriptionTextbox = By.Id("Description");
        private readonly By TeamBioTextbox = By.Id("Biography");
        private readonly By TeamImage = By.Id("preview");
        private readonly By ImageUploadInput = By.Id("file");
        private readonly By ImageUploadDone = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By BusinessLinesListBox = By.XPath("//select[@id = 'Tags_0__Values']/preceding-sibling::div");
        private static By BusinessLinesListItem(string item) => By.XPath($"//ul[@id = 'Tags_0__Values_listbox']/li[text() = '{item}'] | //ul[@id = 'Tags_0__Values_listbox']/li//font[text() = '{item}']");

        private static By MultiteamType(string type) => By.XPath($"//ul[@id='MultiTeamType_listbox']/li[text()='{type}']");
        private static By AssessmentTypeListItem(string assessmentType) => By.XPath($"//ul[@id='SurveyId_listbox']/li[text()='{assessmentType}']");

        public void SelectMultiTeamType(string multiTeamType)
        {
            SelectItem(MultiTeamType, MultiteamType(multiTeamType));
        }

        public void SelectAssessmentType(string assessmentType)
        {
            SelectItem(AssessmentType, AssessmentTypeListItem(assessmentType));
        }

        public void ClickCreateTeamAndAddSubTeam()
        {
            Log.Step(nameof(CreateMultiTeamPage), "On Add A Team page, click Create Team & Add Sub-Team button");
            Wait.UntilElementClickable(CreateTeamAndAddSubTeamButton).Click();
        }

        public void EnterMultiTeamInfo(MultiTeamInfo multiTeamInfo)
        {
            Log.Step(nameof(CreateMultiTeamPage), "On Add A Team page, enter team info");
            Wait.UntilElementClickable(MultiTeamName).SetText(multiTeamInfo.TeamName);

            SelectMultiTeamType(multiTeamInfo.TeamType);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(MultiTeamName);
            Wait.UntilElementClickable(MultiTeamName).Click();

            if (!string.IsNullOrEmpty(multiTeamInfo.Department))
            {
                Wait.UntilElementClickable(DepartmentTextbox).SetText(multiTeamInfo.Department); 
            }
            if (!string.IsNullOrEmpty(multiTeamInfo.DateEstablished))
            {
                Wait.UntilElementClickable(DateEstablishedTextbox).SetText(multiTeamInfo.DateEstablished); 
            }
            if (!string.IsNullOrEmpty(multiTeamInfo.AgileAdoptionDate))
            {
                Wait.UntilElementClickable(AgileAdoptionDateTextbox).SetText(multiTeamInfo.AgileAdoptionDate); 
            }
            if (!string.IsNullOrEmpty(multiTeamInfo.Description))
            {
                Wait.UntilElementClickable(DescriptionTextbox).SetText(multiTeamInfo.Description); 
            }
            if (!string.IsNullOrEmpty(multiTeamInfo.TeamBio))
            {
                Wait.UntilElementClickable(TeamBioTextbox).SetText(multiTeamInfo.TeamBio); 
            }
            if (!string.IsNullOrEmpty(multiTeamInfo.ImagePath))
            {
                Wait.UntilElementExists(ImageUploadInput).SetText(multiTeamInfo.ImagePath, false);

                Wait.UntilElementExists(ImageUploadDone); 
            }

            SelectItem(BusinessLinesListBox, BusinessLinesListItem(SharedConstants.TeamTag));

        }

        public string GetTeamImage()
        {
            return Wait.UntilElementVisible(TeamImage).GetElementAttribute("src");
        }

        public void NavigateToPage(string companyId)
        {
            Log.Step(nameof(CreateMultiTeamPage), $"Navigate to create multiteam page for company id ${companyId}");
            NavigateToUrl($"{BaseTest.ApplicationUrl}/multiteam/{companyId}/create");
        }
    }
}