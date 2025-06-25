using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.MultiTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("MultiTeam")]
    public class MultiTeamRadarNoTeamCommentsTests : BaseTest
    {
        private static TeamHierarchyResponse _multiTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _multiTeam = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForGrowthJourney);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void MultiTeam_Radar_TeamComments_NoComments()
        {
            _multiTeam.CheckForNull($"<{nameof(_multiTeam)}> is null. Aborting test.");

            var login = new LoginPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtetDashboardPage.NavigateToPage(_multiTeam.TeamId);

            mtetDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            radarPage.ClickTeamCommentsExcelButton();

            Assert.IsTrue(radarPage.IsTeamCommentsNoDataPopupDisplayed(), "The 'No Team Comments' popup is not displayed.");

        }
    }
}
