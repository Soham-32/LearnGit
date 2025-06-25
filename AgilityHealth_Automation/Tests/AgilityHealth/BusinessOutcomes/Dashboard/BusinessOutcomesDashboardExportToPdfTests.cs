using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardExportToPdfTests : BaseTest
    {

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 46753
        public void BusinessOutcomes_Dashboard_ExportToPDF()
        {

            var login = new LoginPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            boDashboard.NavigateToPage(Company.Id);

            var fileName = DateTime.Today.ToString("yyyy-MM-dd") + "_BusinessOutcomes.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            boDashboard.ClickOnExportToPdfButton();

            Log.Info("Verify PDF is downloaded successfully.");
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"{fileName} PDF not downloaded successfully");
        }
    }
}