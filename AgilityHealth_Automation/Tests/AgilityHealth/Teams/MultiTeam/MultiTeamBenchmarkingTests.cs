using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Benchmarking;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam")]
    public class MultiTeamBenchmarkingTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _multiTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _multiTeam = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForBenchmarking);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
            
        }

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void MultiTeam_Benchmarking_BasicFlowAndPDFExport()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var benchmarkPopup = new BenchmarkingPopUp(Driver, Log);
            var benchmarkDetail = new BenchmarkingDetailsPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtetDashboardPage.NavigateToPage(_multiTeam.TeamId);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamAssessmentType);

            multiTeamRadarPage.Radar_SwitchToBenchmarkingView();

            benchmarkPopup.ClickSelectButton();

            Assert.AreEqual("Multi-Team Benchmarking", benchmarkDetail.GetTitle(), "Title doesn't match");
            Assert.AreEqual(_multiTeam.Name, benchmarkDetail.GetBaseTeamLegendText(), 
                "Base Team Legend text doesn't match");
            Assert.AreEqual($"All Teams in {User.CompanyName}", benchmarkDetail.GetBenchmarkAgainstLegendText(), 
                "Benchmarking Legend text doesn't match");

            Driver.Back();

            multiTeamRadarPage.Radar_SwitchToBenchmarkingView();

            const string option = "Work Type";
            const string workTypeValue = SharedConstants.NewTeamWorkType;
            benchmarkPopup.SelectBenchMarkingOption(option);
            benchmarkPopup.SelectBenchMarkingWorkType(workTypeValue);
            benchmarkPopup.ClickSelectButton();

            Assert.AreEqual("Multi-Team Benchmarking", benchmarkDetail.GetTitle(), "Title doesn't match");
            Assert.AreEqual(_multiTeam.Name, benchmarkDetail.GetBaseTeamLegendText(), 
                "Base Team Legend text doesn't match");
            Assert.AreEqual($"{option} ({workTypeValue}) in {User.CompanyName}", 
                benchmarkDetail.GetBenchmarkAgainstLegendText(), "Benchmarking Legend text doesn't match");

            var filename = $"Benchmark {_multiTeam.Name} Against {option} ({workTypeValue}) in  {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(filename);

            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(filename), 
                $"{filename} PDF is not downloaded successfully");
        }

        [TestMethod]
        public void Benchmarking_VerifyLastUpdatedDate()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var assessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var benchmarkPopup = new BenchmarkingPopUp(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var mtetDashboardPage = new MtEtDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            mtetDashboardPage.NavigateToPage(_multiTeam.TeamId);
            assessmentDashboard.ClickMtEtRadar(SharedConstants.TeamHealthRadarName);

            multiTeamRadarPage.Radar_SwitchToBenchmarkingView();

            benchmarkPopup.SelectAgilityHealthRadioButton();
            benchmarkPopup.SelectBenchMarkingOption("All Teams");
            benchmarkPopup.ClickSelectButton();

            var actualDate = multiTeamRadarPage.GetBenchmarkLastUpdatedDate();
            Assert.IsTrue(DateTime.Now.AddDays(-7) <= actualDate, 
                $"The Benchmark was last updated on '{actualDate.ToShortDateString()}'");

        }

    }
}
