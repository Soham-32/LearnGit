using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthJourney.EnterpriseTeam
{
    [TestClass]
    [TestCategory("GrowthJourney")]
    public class EnterpriseTeamGrowthJourneyTests : BaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setup;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static RadarResponse _radar;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _setup = new SetupTeardownApi(TestEnvironment);
                _enterpriseTeam = _setup.GetCompanyHierarchy(Company.Id)
                    .GetTeamByName(Constants.EnterpriseTeamForGrowthJourney)
                    .CheckForNull($"<{Constants.EnterpriseTeamForGrowthJourney}> was not found in the response.");
                _radar = _setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_GrowthJourney_ExportToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            radarPage.NavigateToGrowthJourney(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);

            var filename = $"{SharedConstants.EnterpriseTeamForGrowthJourney} {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(filename);

            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(filename),
                $"{filename} is not downloaded successfully");

        }

        //32982
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_GrowthJourney_VerifyFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourney = new GrowthJourneyPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            radarPage.NavigateToGrowthJourney(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);

            const string expectedQuarter = "Q2 2019";
            const string expectedMonth = "April 2019";
            const string expectedBiAnnualTime = "1st Half 2019";
            const string expectedAnnualTime = "2019";

            radarPage.Filter_OpenFilterSidebar();
            Assert.AreEqual("Quarterly", growthJourney.GetSelectedCompareTypeFromFilter(),
                "On filter, compare type doesn't match");
            Assert.IsTrue(growthJourney.IsFilterItemCheckboxSelectedFromFilter(expectedQuarter),
                $"On filter, {expectedQuarter} isn't selected");
            growthJourney.SwitchToCompareRadarView();

            var colorHex = growthJourney.GetFilterItemColorFromFilter(expectedQuarter);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot value doesn't match");
            }


            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count, $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                //Assert.AreEqual(expectedavg, Int32.Parse(multiTeamRadarPage.Radar_GetDotValue("avg", colorhex, comp)[0]));
            }

            growthJourney.SelectFilterItemCheckboxByNameFromFilter(expectedQuarter, false);

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            growthJourney.SelectCompareTypeFromFilter("Monthly");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.IsFilterItemCheckboxSelectedFromFilter(expectedMonth),
                $"On filter, {expectedMonth} isn't selected");
            growthJourney.SwitchToCompareRadarView();

            colorHex = growthJourney.GetFilterItemColorFromFilter(expectedMonth);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count, $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                //Assert.AreEqual(expectedavg, Int32.Parse(multiTeamRadarPage.Radar_GetDotValue("avg", colorhex, comp)[0]));
            }

            growthJourney.SelectFilterItemCheckboxByNameFromFilter(expectedMonth, false);

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }


            growthJourney.SelectCompareTypeFromFilter("Bi-Annually");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.IsFilterItemCheckboxSelectedFromFilter(expectedBiAnnualTime));
            growthJourney.SwitchToCompareRadarView();

            colorHex = growthJourney.GetFilterItemColorFromFilter(expectedBiAnnualTime);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }


            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                //Assert.AreEqual(expectedavg, Int32.Parse(multiTeamRadarPage.Radar_GetDotValue("avg", colorhex, comp)[0]));
            }

            growthJourney.SelectFilterItemCheckboxByNameFromFilter(expectedBiAnnualTime, false);

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            growthJourney.SelectCompareTypeFromFilter("Annually");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.IsFilterItemCheckboxSelectedFromFilter(expectedAnnualTime),
                $"On filter, {expectedAnnualTime} isn't selected");
            growthJourney.SwitchToCompareRadarView();

            colorHex = growthJourney.GetFilterItemColorFromFilter(expectedAnnualTime);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }


            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, growthJourney.Radar_GetDotValue(colorHex, comp).Count, $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                //Assert.AreEqual(expectedavg, Int32.Parse(multiTeamRadarPage.Radar_GetDotValue("avg", colorhex, comp)[0]));
            }

            growthJourney.SelectFilterItemCheckboxByNameFromFilter(expectedAnnualTime, false);

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember.Where(comp => !comp.Equals("Response to Change")))
            {
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

            //Verifying Avg value for Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(0, growthJourney.Radar_GetDotValue(colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
            }

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51216
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_GrowthJourney_VerifyCampaignsFilter()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourney = new GrowthJourneyPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var campaignName = _setup.GetCampaignsDetails(Company.Id).Campaigns.First().Name;

            Log.Info($"Navigate to the 'Growth Journey' page of the team - '{_enterpriseTeam.Name}'. and verify that the campaign - {campaignName} is checked by default");
            radarPage.NavigateToGrowthJourney(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);
            radarPage.Filter_OpenFilterSidebar();
            growthJourney.SelectCompareTypeFromFilter("Campaigns");
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.IsFilterItemCheckboxSelectedFromFilter(campaignName), $"{campaignName} checkbox is not checked ");
            Assert.AreEqual(campaignName, growthJourney.GetCampaignName(), $"{campaignName} is not present ");

            Log.Info($"Verify that '{campaignName}' is present in the 'Compare Radar Analysis' table");
            radarPage.Filter_OpenFilterSidebar();
            var campaignListFromCompareCampaigns = growthJourney.GetHeaderList();
            Assert.That.ListContains(campaignListFromCompareCampaigns, campaignName, $"{campaignName} is not present in the 'Compare Campaign' table");

            Log.Info($"Verify that unchecked campaign {campaignName} is not present on the 'Compare Radar Analysis column'");
            radarPage.Filter_OpenFilterSidebar();
            growthJourney.SelectFilterItemCheckboxByNameFromFilter(campaignName, false);
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsFalse(growthJourney.IsItemPresentInColumnList(campaignName), $"{campaignName} is still present in the 'Compare Campaign' column");

            Log.Info($"Verify that checked campaign {campaignName} is present on the 'Compare Radar Analysis column'");
            radarPage.Filter_OpenFilterSidebar();
            growthJourney.SelectFilterItemCheckboxByNameFromFilter(campaignName);
            radarPage.Filter_OpenFilterSidebar();
            Assert.IsTrue(growthJourney.IsItemPresentInColumnList(campaignName), $"{campaignName} is not present in the 'Compare Campaign' column");
        }
    }
}