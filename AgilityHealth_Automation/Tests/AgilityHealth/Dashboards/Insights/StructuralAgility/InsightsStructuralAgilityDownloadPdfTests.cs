using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard")]
    public class InsightsStructuralAgilityDownloadPdfTests : BaseTest
    {

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_TeamStability_DownloadPdf()
        {
            var login = new LoginPage(Driver, Log);
            var teamAgilityPage = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgilityPage.NavigateToPage(Company.InsightsId);

            teamAgilityPage.ClickOnStructuralAgilityTab();

            var fileName = $"{StructuralAgilityWidgets.TeamStability.Title}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            teamAgilityPage.SelectDownloadOption(StructuralAgilityWidgets.TeamStability, InsightsDownloadOption.Pdf);

            Log.Info("Verify PDF is downloaded successfully.");
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");
        }
    }
}