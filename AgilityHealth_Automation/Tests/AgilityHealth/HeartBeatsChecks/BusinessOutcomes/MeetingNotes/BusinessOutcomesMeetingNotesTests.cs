using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.MeetingNotes;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.BusinessOutcomesOverallPerformance;
using AgilityHealth_Automation.Utilities;
using AtCommon.ObjectFactories;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.BusinessOutcomes.MeetingNotes
{
    [TestClass]
    [TestCategory("HeartBeatChecks")]
    public class BusinessOutcomesMeetingNotesTests : BaseTest
    {
        public EnvironmentTestInfo ProductionEnvironmentTestData = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/ProductionEnvironmentData.json").DeserializeJsonObject<EnvironmentTestInfo>();

        [TestMethod]
        [DataRow("srca")]
        [DataRow("hhc")]
        [DynamicData(nameof(Constants.TopVvipCustomersCompanyNames), typeof(Constants), DynamicDataSourceType.Method)]
        public void BusinessOutcomes_MeetingNotes(string env)
        {
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var overallPerformancePage = new BusinessOutcomesOverallPerformancePage(Driver, Log);
            var meetingNotesPage = new MeetingNotesPage(Driver, Log);

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

            Log.Info("Click on Meeting Notes and verify Meeting Note button is not displayed");
            addBusinessOutcomePage.WaitUntilBusinessOutcomesPageLoaded();
            addBusinessOutcomePage.ClickOnMeetingNotes();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNotes.png", 10000);
            var expectedCompanyName = overallPerformancePage.GetLeftNavigationHierarchyTeamName()[0];
            var actualCompanyName = meetingNotesPage.GetTeamNameFormMeetingNotesTitle();
            Assert.AreEqual(expectedCompanyName, actualCompanyName, "Meeting Notes Title is incorrect");
            Assert.IsFalse(meetingNotesPage.IsAddMeetingNoteDisplayed(), "'Add Meeting Note' button is not displayed");

            Log.Info("Click on All Meeting Notes dropdown and verify dropdown Values");
            meetingNotesPage.ClickOnAllMeetingNotesDropdown();
            TakeFullPageScreenShot($"{FileUtil.GetBasePath()}Resources\\Screenshots\\HeartBeatScreenshots\\{env}\\BusinessOutcomesMeetingNotesDropdown.png", 10000);

            var actualDropdownValues = meetingNotesPage.GetMeetingNotesDropdownOptions();
            Assert.That.ListsAreEqual(BusinessOutcomesFactory.GetAllMeetingNotesDropdownValues(), actualDropdownValues, "Meeting Notes Dropdown Values are incorrect");
        }
    }
}
