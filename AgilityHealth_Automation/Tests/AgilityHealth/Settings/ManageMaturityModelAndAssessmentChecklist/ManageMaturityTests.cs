using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageMaturityModelAndAssessmentChecklist;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageMaturityModelAndAssessmentChecklist
{
    [TestClass]
    [TestCategory("Settings"), TestCategory("ManageMaturity")]
    public class ManageMaturityTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageMaturity_AddEditDeleteStage()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageMaturityPage = new ManageMaturityPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnSettingsLink();

            settingsPage.SelectSettingsOption("Manage Model and Checklist");

            manageMaturityPage.SelectRadarType(SharedConstants.MaturityTestingRadarType);

            manageMaturityPage.ClickAddStage();
            var name = "M" + RandomDataUtil.GetTeamName();
            var aHEquivalent = "Crawl";
            manageMaturityPage.EnterMaturityName(name);
            manageMaturityPage.SelectAhEquivalent(aHEquivalent);
            manageMaturityPage.ClickSaveButton();

            Assert.IsTrue(manageMaturityPage.DoesMaturityExist(name, aHEquivalent), $"Maturity doesn't exist with {name} - {aHEquivalent}");
            
            manageMaturityPage.ClickEditMaturity(name);
            name = "ME" + RandomDataUtil.GetTeamName();
            aHEquivalent = "Walk";
            manageMaturityPage.EnterMaturityName(name);
            manageMaturityPage.SelectAhEquivalent(aHEquivalent);
            manageMaturityPage.ClickSaveButton();

            Assert.IsTrue(manageMaturityPage.DoesMaturityExist(name, aHEquivalent), $"Maturity doesn't exist with {name} - {aHEquivalent}");

            manageMaturityPage.ClickDeleteButton(name);
            manageMaturityPage.AcceptDelete();

            Assert.IsFalse(manageMaturityPage.DoesMaturityExist(name, aHEquivalent), $"Maturity does exist with {name} - {aHEquivalent}");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageMaturity_AddEditDeleteAssessmentChecklistItem()
        {
            var login = new LoginPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var settingsPage = new SettingsPage(Driver, Log);
            var manageMaturityPage = new ManageMaturityPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            topNav.ClickOnSettingsLink();

            settingsPage.SelectSettingsOption("Manage Model and Checklist");

            manageMaturityPage.SelectRadarType(SharedConstants.MaturityTestingRadarType);

            manageMaturityPage.SelectAssessmentCheckListTab();

            manageMaturityPage.ClickAddNewChecklistButton();
            var description = "option" + RandomDataUtil.GetTeamDescription();
            manageMaturityPage.EnterChecklistDescription(description);
            manageMaturityPage.SelectOptionType();
            manageMaturityPage.ClickAddNewOption();
            var checkListOption1 = "Option1";
            manageMaturityPage.EnterOptionTextbox(checkListOption1);
            manageMaturityPage.ClickSaveOptionButton();
            manageMaturityPage.ClickSaveChecklistButton();

            Assert.IsTrue(manageMaturityPage.DoesChecklistExist(description, "Single"),$"Assessment checklist doesn't exists for {description}");
            
            manageMaturityPage.ClickEditChecklistButton(description);
            
            //verifying option is added correctly or not.
            Assert.AreEqual(1,manageMaturityPage.GetAddedChecklistOptions().Count, "Option count doesn't match");
            Assert.AreEqual(checkListOption1, manageMaturityPage.GetAddedChecklistOptions()[0], "Option value doesn't match");

            description = description + "updated";
            manageMaturityPage.EnterChecklistDescription(description);
            manageMaturityPage.ClickSaveChecklistButton();

            Assert.IsTrue(manageMaturityPage.DoesChecklistExist(description, "Single"),$"Assessment checklist doesn't exists for {description}");

            manageMaturityPage.ClickDeleteChecklistButton(description);
            manageMaturityPage.AcceptDeleteChecklist();

            Assert.IsFalse(manageMaturityPage.DoesChecklistExist(description, "Single"), $"Assessment checklist exists for {description}");

            manageMaturityPage.ClickAddNewChecklistButton();
            description = "option" + RandomDataUtil.GetTeamDescription();
            manageMaturityPage.EnterChecklistDescription(description);
            manageMaturityPage.SelectOptionType(false);
            manageMaturityPage.ClickAddNewOption();
            manageMaturityPage.EnterOptionTextbox(checkListOption1);
            manageMaturityPage.ClickSaveOptionButton();
            manageMaturityPage.ClickAddNewOption();
            var checkListOption2 = "Option2";
            manageMaturityPage.EnterOptionTextbox(checkListOption2);
            manageMaturityPage.ClickSaveOptionButton();
            manageMaturityPage.ClickSaveChecklistButton();

            Assert.IsTrue(manageMaturityPage.DoesChecklistExist(description, "Multiple"), $"Assessment checklist doesn't exists for {description}");

            manageMaturityPage.ClickEditChecklistButton(description);

            //verifying option is added correctly or not.
            Assert.AreEqual(2, manageMaturityPage.GetAddedChecklistOptions().Count, "Option count doesn't match");
            Assert.AreEqual(checkListOption1, manageMaturityPage.GetAddedChecklistOptions()[0], "Option value doesn't match");
            Assert.AreEqual(checkListOption2, manageMaturityPage.GetAddedChecklistOptions()[1], "Option value doesn't match");

            description = description + "updated";
            manageMaturityPage.EnterChecklistDescription(description);
            manageMaturityPage.ClickSaveChecklistButton();

            Assert.IsTrue(manageMaturityPage.DoesChecklistExist(description, "Multiple"),$"Assessment checklist doesn't exists for {description}");

            manageMaturityPage.ClickDeleteChecklistButton(description);
            manageMaturityPage.AcceptDeleteChecklist();

            Assert.IsFalse(manageMaturityPage.DoesChecklistExist(description, "Multiple"), $"Assessment checklist exists for {description}");
        }
    }
}
