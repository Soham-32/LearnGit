using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardExportToExcelTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent, 1);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug : 49256 , 51926
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_ExportToExcel()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            var fileName = $"BusinessOutcomes_{User.CompanyName}_{DateTime.Today:yyyyMMdd}.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);

            businessOutcomesDashboard.ClickOnExportToExcelButton();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);

            ExcelUtil.ExcelColumnAutoAdjustExcel(spreadsheet);

            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var exportColumns = new List<string>
                {"Level","Outcome Type","Column","Card ID","Outcome Title","Description","Key Result Title" ,"Metric" ,"Start" ,"Goal", "Current", "Progress (%)" ,"Created By" , "Create Date & Time" ,"Last Update Date & Time", "Last Updated By"};

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            Log.Info("Verify that data in exported excel file showing properly");
            for (var i = 0; i < exportColumns.Count; i++)
            {
                Log.Info($"Column {i} - Expected='{exportColumns[i]}' Actual='{actualColumns[i]}'");
                Assert.AreEqual(exportColumns[i], actualColumns[i], "Column header text doesn't match");
            }

            //Comparing row data
            var actualRow = tbl.Rows.Cast<DataRow>().First(r => r.ItemArray[4].ToString() == _response.Title)
                .CheckForNull($"Business Outcome with title <{_response.Title}> was not found in the Excel")
                .ItemArray.Select(item => item.ToString()).ToList();

            for (var i = 13; i <= 14; i++)
            {
                var date = double.Parse(actualRow[i]);
                var dateTime = DateTime.FromOADate(date).ToString("g");
                actualRow[i] = dateTime;
            }

            var expectedRow = new List<string>
                {_response.sourceCategoryName,"3 Year", User.CompanyName, _response.PrettyId.ToString(), _response.Title, _response.Description,
                    _response.KeyResults[0].Title, _response.KeyResults[0].Metric.Name, _response.KeyResults[0].Start,_response.KeyResults[0].Target,
                    _response.KeyResults[0].Progress.ToString(CultureInfo.InvariantCulture), _response.OverallProgress.ToString(CultureInfo.InvariantCulture) ,_response.Owner, _response.CreatedAt?.ToString("g"),_response.UpdatedAt?.ToString("g"), _response.UpdatedBy};

            for (var j = 0; j < expectedRow.Count; j++)
            {
                Log.Info($"Column {j} - Expected='{expectedRow[j]}' Actual='{actualRow[j]}'");
                Assert.AreEqual(expectedRow[j], actualRow[j], $"Column {j} value doesn't match");
            }

            tbl.Dispose();
        }
    }
}