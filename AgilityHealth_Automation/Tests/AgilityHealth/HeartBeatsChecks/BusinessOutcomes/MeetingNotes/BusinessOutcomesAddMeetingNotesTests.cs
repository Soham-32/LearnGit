using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.MeetingNotes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.LeftNav;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes.MeetingNotes
{
    [TestClass]
    [TestCategory("HeartBeatChecks")]
    public class BusinessOutcomesAddMeetingNotesTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DataRow("hhc")]
        [DataRow("srca")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void BusinessOutcomes_AddMeetingNotes(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);
            var leftNav = new LeftNavPage(Driver, Log);

            var companyId = ProductionEnvironmentTestData.Environments.Where(a => a.Name.Equals(env)).Select(a => a.CompanyId).ToList().FirstOrDefault();
            LoginToProductionEnvironment(env);

            if (env == "srca")
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForSaDomain(env, companyId);
            }

            else
            {
                addBusinessOutcomePage.NavigateToBusinessOutcomesPageForProd(env, companyId);
            }

            Log.Info("Select the team level from left nav and verify the Meeting Note button is displayed");
            addBusinessOutcomePage.ClickOnMeetingNotes();

            var expectedCompanyName = overallPerformancePage.GetLeftNavigationHierarchyTeamName()[1].ToUpper();

            leftNav.ClickOnATeam(expectedCompanyName);
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNoteTeamLevel.png", 10000);
            Assert.IsTrue(meetingNotesPage.IsAddMeetingNoteDisplayed(), "Add Meeting Note button is not displayed");

            Log.Info("Click on Add Meeting Note and Verify it is loaded successfully");
            meetingNotesPage.ClickOnAddMeetingNoteButton();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNotePage.png", 10000);
            var expectedHeaderTitles = new List<string>
            {
                "Public Note",
                "Send Email"
            };
            var actualHeaderTitles = meetingNotesPage.GetAddMeetingNoteHeaderTitles();
            Assert.That.ListsAreEqual(expectedHeaderTitles, actualHeaderTitles, "Header titles are not matched");

            meetingNotesPage.ClickOnMeetingTypeDropdown();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNotesDropdownValues.png", 10000);

            var actualMeetingTypeDropdownValues = meetingNotesPage.GetMeetingTypeDropdownOptions();
            Assert.That.ListsAreEqual(BusinessOutcomesFactory.GetMeetingNoteTypeDropdownValues(), actualMeetingTypeDropdownValues, "Meeting Type dropdown values are not matched");
            Assert.IsTrue(meetingNotesPage.IsMeetingTitleDisplayed(), "Meeting Title is not displayed");
            Assert.IsTrue(meetingNotesPage.IsAddAttendeeButtonEnable(), "Add Attendee button is not enabled");


            var expectedMeetingNotesSubTabs = new List<string>
            {
                "Decisions",
                "Action Item(s)",
                "Document(s)"
            };

            var actualMeetingNotesSubTabs = meetingNotesPage.GetMeetingNoteSubTabs();
            Assert.That.ListsAreEqual(expectedMeetingNotesSubTabs, actualMeetingNotesSubTabs, "'Meeting Notes'  SubTabs are not matched");

            Log.Info("Click on Action Item Tab and verify it is loaded successfully");
            meetingNotesPage.ClickMeetingNoteSubTabHeaders("Action Item(s)");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNotesActionItem.png", 10000);
            Assert.IsTrue(meetingNotesPage.IsAddActionItemButtonDisplayed(), "Add Action Item button is not displayed");

            Log.Info("Click on Documents  Tab and verify it is loaded successfully");
            meetingNotesPage.ClickMeetingNoteSubTabHeaders("Document(s)");
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNoteDocument.png", 10000);
            meetingNotesPage.ClickOnAddADocumentDropdown();
            var expectedDocumentDropdownValues = new List<string>
            {
                "Upload a Document",
                "Add a Link"
            };
            var actualDocumentDropdownValues = meetingNotesPage.GetDocumentDropdownValues();
            Assert.That.ListsAreEqual(actualDocumentDropdownValues, expectedDocumentDropdownValues, "Document dropdown values are not matched");
        }
    }
}
