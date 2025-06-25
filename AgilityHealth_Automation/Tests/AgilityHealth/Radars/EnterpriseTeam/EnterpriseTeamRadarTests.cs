using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("EnterpriseTeam")]
    public class EnterpriseTeamRadarTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                        .GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Radar_ET_ExportRadarToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var etDashboard = new MtEtDashboardPage(Driver, Log);

            var fileName = $"{_team.Name} {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            etDashboard.NavigateToPage(_team.TeamId, true);
            etDashboard.ClickOnRadar(SharedConstants.TeamAssessmentType);

            radarPage.ClickExportToPdf();

            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"<{fileName}> not downloaded successfully");

        }


    }
}
