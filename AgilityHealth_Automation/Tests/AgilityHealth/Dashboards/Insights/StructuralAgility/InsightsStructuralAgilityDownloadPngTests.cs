using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.StructuralAgility;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.TeamAgility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("StructuralAgilityDashboard"), TestCategory("Dashboard"), TestCategory("Critical")]
    public class InsightsStructuralAgilityDownloadPngTests : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_TeamStability_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.TeamStability);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_AllocationByRole_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.AllocationByRole);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_ParticipantGroup_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.ParticipantGroup);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_ParticipantGroupDistribution_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.ParticipantGroupDistribution);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_PeopleByRole_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.PeopleByRole);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_PeopleDistribution_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.PeopleDistribution);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_AgileAdoption_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.AgileAdoption);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_AgileAdoptionDistribution_DownloadPng()
        {
            InsightsVerifyPngDownload(StructuralAgilityWidgets.AgileAdoptionDistribution);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_TeamFormation_DownloadPng()
        {
            InsightsVerifyPngDownload(new InsightsWidget("Team Formation", "structuralagilityagilevsnonagileteams"));
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_TeamFormationDistribution_DownloadPng()
        {
            InsightsVerifyPngDownload(new InsightsWidget("Team Formation Distribution", "structuralagilityagilevsnonagiledistribution"));
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_WorkType_DownloadPng()
        {
            InsightsVerifyPngDownload(new InsightsWidget("Work Type", "structuralagilityagilevsnonagileteams"));
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public void Insights_WorkTypeDistribution_DownloadPng()
        {
            InsightsVerifyPngDownload(new InsightsWidget("Work Type Distribution", "structuralagilityagilevsnonagiledistribution"));
        }

        private void InsightsVerifyPngDownload(InsightsWidget widget)
        {
            var login = new LoginPage(Driver, Log);
            var structuralAgilityTab = new StructuralAgilityPage(Driver, Log);
            var teamAgilityTab = new TeamAgilityPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(InsightsUser.Username, InsightsUser.Password);

            teamAgilityTab.NavigateToPage(Company.InsightsId);

            teamAgilityTab.ClickOnStructuralAgilityTab();

            var fileName = $"{widget.Title}.png";
            if (widget.Title == "Participant Group Distribution") fileName = "Participant Group  Distribution.png";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            if (widget.Title == StructuralAgilityWidgets.AllocationByRole.Title)
            {
                structuralAgilityTab.SelectAllocationByRoleWorkType("Software Delivery");
            }
            else if (widget.Title == StructuralAgilityWidgets.PeopleByRole.Title ||
                     widget.Title == StructuralAgilityWidgets.PeopleDistribution.Title)
            {
                structuralAgilityTab.SelectPeopleFilter("Software Delivery");
            }
            else if (widget.Title == StructuralAgilityWidgets.AgileAdoption.Title ||
                     widget.Title == StructuralAgilityWidgets.AgileAdoptionDistribution.Title)
            {
                structuralAgilityTab.SelectTeamCategories("Agile Adoption");
            }
            else switch (widget.Title)
            {
                case "Team Formation":
                case "Team Formation Distribution":
                    structuralAgilityTab.SelectTeamCategories("Team Formation");
                    break;
                case "Work Type":
                case "Work Type Distribution":
                    structuralAgilityTab.SelectTeamCategories("Work Type");
                    break;
            }

            structuralAgilityTab.SelectDownloadOption(widget, InsightsDownloadOption.Png);

            Log.Info("Verify PNG is downloaded successfully.");
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"<{fileName}> not downloaded successfully");
        }
    }
}