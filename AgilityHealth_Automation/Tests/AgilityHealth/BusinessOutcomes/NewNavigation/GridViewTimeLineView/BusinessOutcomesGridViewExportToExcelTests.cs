using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.GridView;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.GridViewTimeLineView
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesGridViewExportToExcelTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _businessOutcomeResponse, _initiativeCardResponse, _projectCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var initiatives = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;
            var projects = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.ProjectsTimeline.GetDescription()).Tags;
            _businessOutcomeResponse = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
            _initiativeCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiatives);
            _projectCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, projects);
        }
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomeGridViewExportToExcel()
        {
            VerifyExportToExcel(BusinessOutcomesCardType.BusinessOutcomes.GetDescription());
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomeInitiativesGridViewExportToExcel()
        {
            VerifyExportToExcel(BusinessOutcomesCardType.AnnualView.GetDescription());
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomeProjectGridViewExportToExcel()
        {
            VerifyExportToExcel(BusinessOutcomesCardType.ProjectsTimeline.GetDescription());
        }

        private void VerifyExportToExcel(string cardType)
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var businessOutcomesViewPage = new BusinessOutcomesViewPage(Driver, Log);
            var downloadPath = FileUtil.GetDownloadPath();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigating to Business Outcomes Dashboard and Click on Grid View tab");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            addBusinessOutcomePage.ClickOnGridViewTab();

            Log.Info("Verifying Grid View dropdown options");
            var expectedGridViewDropdownValues = new List<string> { "Grid View", "Timeline View" };
            var actualGridViewDropdownValues = businessOutcomesViewPage.GetGridViewDropdownOptions();
            Assert.That.ListsAreEqual(expectedGridViewDropdownValues, actualGridViewDropdownValues);

            Log.Info("Selecting 'Grid View' from the dropdown");
            businessOutcomesViewPage.ClickOnGridViewDropdownOptions("Grid View");

            Log.Info("Clicking on Card Type dropdown and verify the Cardtype Dropdown Options");
            addBusinessOutcomePage.ClickOnCardTypeDropdown();
            var expectedCardTypeDropdownValues = new List<string> { "Business Outcomes", "Initiatives", "Projects" };
            var actualCardTypeDropdownValues = businessOutcomesViewPage.GetCardTypeDropdownOptions();
            Assert.That.ListsAreEqual(expectedCardTypeDropdownValues, actualCardTypeDropdownValues);
            businessOutcomesViewPage.ClickOnCardTypeDropdownOptions(cardType);

            Log.Info("Clicking on Export to Excel button");
            addBusinessOutcomePage.ClickOnExportToExcelButton();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded("Grid-view-*.xlsx");

            Log.Info("Verifying if the Excel file is downloaded");
            var files = Directory.GetFiles(downloadPath, "Grid-view-*.xlsx");
            spreadsheet = files.OrderByDescending(File.GetCreationTime).First();
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            Log.Info("Verifying column headers in the exported file");
            var expectedColumns = cardType == "Business Outcomes"
                ? new List<string> { "ID", "Card Title", "Progress", "Team", "Target", "Actual", "Comment", "Updated By" }
                : new List<string> { "ID", "Card Title", "Progress", "Team", "Status", "End Date", "Owners" };

            var actualColumns = tbl.Columns.Cast<DataColumn>()
                .Select(col => col.ColumnName.Trim())
                .Where(name => !string.IsNullOrEmpty(name) && !name.Contains("Column")) // Exclude unwanted columns
                .ToList();

            Assert.That.ListsAreEqual(expectedColumns, actualColumns, $"Column count matched.");

        }
    }
}
