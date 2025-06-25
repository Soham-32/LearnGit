using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;
using AgilityHealth_Automation.DataObjects.NewNavigation.Teams;
using System.Linq;
using System.Collections.Generic;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Teams.Base
{
    public class CommonGridBasePage : TeamBasePage
    {

        public CommonGridBasePage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        #region Locators

        #region Grid

        // Grid by Email
        private static By FirstNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[2]");
        private static By LastNameFromGrid(string email) => By.XPath($"//td[text()='{email}']/preceding-sibling::td[1]");
        private static By EmailFromGrid(string email) => By.XPath($"//td[text()='{email}']");

        //Role tag
        private static By FirstRoleTag(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td[2]/div[1]/div/div");
        private static By MoreRoleTags(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td[2]//span");
        private static By ExtraRoleTags(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td[2]/div[2]/div/div");

        //Tags tag
        private static By FirstTags(string email, int tagIndex) => By.XPath($"//td[text()='{email}']/following-sibling::td[3]/div[1]/div/div");
        private static By MoreTags(string email, int tagIndex) => By.XPath($"//td[text()='{email}']/following-sibling::td[3]//span");
        private static By ExtraTags(string email, int tagIndex) => By.XPath($"//td[text()='{email}']/following-sibling::td[3]/div[2]/div/div");

        // Action on grid
        private readonly By DeleteButton = By.XPath("//li/span[text()='Delete']");
        private static By ActionDropDown(string email) => By.XPath($"//td[text()='{email}']/following-sibling::td/ul/li/span[text()='Edit']/parent::li/following-sibling::li//span/i");

        #endregion

        #region Toaster Message

        private readonly By ToasterMessage = By.XPath("//div[@id='createsuccess']/div[1]");

        #endregion

        #endregion




        #region Methods

        #region Grid


        public Member GetTeamMemberOrStakeholderOrLeadersInfoFromGrid(string email, int tagColumnIndex = 2)
        {
            Wait.HardWait(1000);//Wait till 'TeamMember or Stakeholder info' is displayed
            var leadersInfo = new Member
            {
                FirstName = Wait.UntilElementExists(FirstNameFromGrid(email)).Text,
                LastName = Wait.UntilElementExists(LastNameFromGrid(email)).Text,
                Email = Wait.UntilElementExists(EmailFromGrid(email)).Text,
                Role = GetRoleTagsValue(email),
                ParticipantGroup = GetTagsValue(email, tagColumnIndex)
            };
            return leadersInfo;
        }

        public List<string> GetRoleTagsValue(string email)
        {
            List<string> roleTagList;
            if (Driver.IsElementPresent(FirstRoleTag(email)))
            {
                var firstRoleTagsValue = Wait.UntilElementVisible(FirstRoleTag(email)).GetText();
                Wait.UntilJavaScriptReady();
                Driver.MoveToElement(MoreRoleTags(email));
                roleTagList = Driver.GetTextFromAllElements(ExtraRoleTags(email)).ToList();
                roleTagList.Add(firstRoleTagsValue);
            }
            else { roleTagList = null; }
            return roleTagList;
        }

        public List<string> GetTagsValue(string email, int tagColumnIndex)
        {
            List<string> tagValueList;

            if (Driver.IsElementPresent(FirstTags(email, tagColumnIndex)))
            {
                var firstTagsValue = Wait.UntilElementVisible(FirstTags(email, tagColumnIndex)).GetText();
                Driver.MoveToElement(MoreTags(email, tagColumnIndex));
                tagValueList = Driver.GetTextFromAllElements(ExtraTags(email, tagColumnIndex)).ToList();
                tagValueList.Add(firstTagsValue);
            }
            else { tagValueList =  null; }
            return tagValueList;
        }

        public void ClickOnDeleteButton(string email)
        {
            Log.Step(nameof(CommonGridBasePage), $"Delete team member by {email} email");
            Driver.JavaScriptScrollToElement(ActionDropDown(email));
            Wait.UntilElementClickable(ActionDropDown(email)).Click();
            Wait.UntilElementClickable(DeleteButton).Click();
            Driver.AcceptAlert();
            Wait.UntilJavaScriptReady();
        }

        public bool IsTeamMemberDisplayed(string email)
        {
            var emailElements = Driver.FindElements(EmailFromGrid(email.ToLower()));

            // If the element exists and is displayed, return true, otherwise false
            return emailElements.Count > 0 && emailElements[0].Displayed;
        }


        #endregion

        #region Toaster Message
        public string GetSuccessMessage()
        {

            return Wait.UntilElementVisible(ToasterMessage).GetText().Trim();
        }
        #endregion

        #endregion

    }
}