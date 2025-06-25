using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Reports
{
    [TestClass]
    [TestCategory("Reports")]
    public class GetBusinessOutcomeExcelTests : BaseV1Test
    {
        private const string FileName = "CRUD.xlsx";
        private static readonly Random RandomVariable = new Random();
        private static readonly string BoName1 = FileName + RandomVariable.Next(10000000, 99999999);
        private readonly int CompanyId = Company.Id;
        private readonly string TeamId = Company.TeamId1;
        private readonly string TeamName = User.IsMember() ? SharedConstants.EnterpriseTeamForGrowthJourney : SharedConstants.MultiTeam;

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 46753
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Excel_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var setup = new SetupTeardownApi(TestEnvironment);
            var fileUtil = new FileUtil();
            var filePath = fileUtil.GetDownloadPath();
            var fileFullName = filePath + "\\"+ FileName;
            fileUtil.DeleteFilesInDownloadFolder(FileName);
            const string outcomeType = "";

            var metricData = new BusinessOutcomeMetricRequest
            {
                TypeId = 2,
                Name = "Throughput",
                Order = 13,
                Uid = 13
            };

            var keyResultData = new KeyResultRequest
            {
                Title = "Key Result " + RandomDataUtil.GetKeyResultsName(),
                Metric = metricData,
                Start = "50",
                Target = "200",
                Progress = 100,
                IsImpact = true,
                SubTargets = new List<BusinessOutcomeKeyResultSubTargetRequest>(),
                Weight = 100
            };

            var businessOutcome = new CustomBusinessOutcomeRequest
            {
                Title = BoName1,
                Description = "Description",
                KeyResults = new List<KeyResultRequest>(),
                CardColor = "#CCCCCC",
                BusinessValue = 1,
                InvestmentCategory = null,
                OverallProgress = 33.33,
                TeamId = int.Parse(TeamId),
                CompanyId = CompanyId,
                SwimlaneType = 4,
                BusinessOutcomeLabel = new List<AtCommon.Dtos.BusinessOutcomes.BusinessOutcomeLabelRequest>(),
                SortOrder = 5,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                Tags = new List<BusinessOutcomeTagRequest>(),
                Comments = new List<CommentRequest>(1),
                CustomFieldValues = new List<CustomFieldValueRequest>(),
                Uid = Guid.Empty,
                IsDeleted = false,
                SourceCategoryName = "1 Year Outcomes",
                CheckListItems = new List<BusinessOutcomeChecklistItemRequest>(),
                Deliverables = new List<DeliverableRequest>(),
                Documents = new List<BusinessOutcomeAttachmentRequest>(),
                ChildCards = new List<BusinessOutcomeChildCardRequest>(),
                Financials = new List<BusinessOutcomeFinancialRequest>()
            }; 

            businessOutcome.KeyResults.Add(keyResultData);
            var labels = setup.GetBusinessOutcomesAllLabels(Company.Id);
            var label1 = labels.FirstOrDefault(l => l.Tags.Any())
                .CheckForNull("No label was found in the response with any tags.");
            var response = await setup.CreateBusinessOutcome(businessOutcome, User, label1.Tags[0].Name);
            var boCreatedBy = response.Dto.Owner;
            var boUpdatedBy = response.Dto.UpdatedBy;
            var boCreatedDateTime = response.Dto.CreatedAt;
            var boUpdatedDateTime = response.Dto.UpdatedAt;

            var categoryLabelUid = label1.Uid;

            //act
            using (var request = new HttpRequestMessage(HttpMethod.Get, RequestUris.ReportsBusinessOutcomeExcel(CompanyId, categoryLabelUid.ToString("D"), TeamId)))
            {
                using Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                    stream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write, FileShare.None,500000, false);
                await contentStream.CopyToAsync(stream);
            }

            //assert
            Assert.IsTrue(fileUtil.IsFileDownloaded(FileName) ,$"{fileFullName} isn't present");

            //Verify excel data
            var spreadsheet = fileUtil.WaitUntilFileDownloaded(FileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var exportColumns = new List<string>
                {"Level","Outcome Type","Column","Outcome Title","Hypothesis","Key Result Title" ,"Metric" ,"Start" ,"Goal", "Current", "Progress (%)" ,"Created By" , "Create Date & Time" ,"Last Update Date & Time", "Last Updated By"};

            var actualColumns = (from DataColumn item in tbl.Columns select item.ColumnName).ToList();

            for (var i = 0; i < exportColumns.Count; i++)
            {
                Assert.AreEqual(exportColumns[i], actualColumns[i], "Column header text doesn't match");
            }

            //Comparing row data
            var actualRow = tbl.Rows.Cast<DataRow>().FirstOrDefault(r => r.ItemArray[3].ToString() == businessOutcome.Title)
                .CheckForNull($"Business Outcome with title <{businessOutcome.Title}> was not found in the Excel")
                .ItemArray.Select(item => item.ToString()).ToList();

            for (var i = 12; i <= 13; i++)
            {
                var date = double.Parse(actualRow[i]);
                var dateTime = DateTime.FromOADate(date).ToString("g");
                actualRow[i] = dateTime;
            }

            var expectedRow = new List<string>
            {
                TeamName, outcomeType, businessOutcome.Tags.First().Name, businessOutcome.Title,
                businessOutcome.Description, businessOutcome.KeyResults[0].Title,
                businessOutcome.KeyResults[0].Metric.Name, businessOutcome.KeyResults[0].Start,
                businessOutcome.KeyResults[0].Target, businessOutcome.KeyResults[0].Progress.ToString(),
                businessOutcome.OverallProgress.ToString(CultureInfo.InvariantCulture), boCreatedBy,
            };

            for (var j = 0; j < expectedRow.Count; j++)
            {
                Assert.AreEqual(expectedRow[j], actualRow[j], $"Column {j} value doesn't match");
            }

            tbl.Dispose();
        }


        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Excel_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            using var request = new HttpRequestMessage(HttpMethod.Get, RequestUris.ReportsBusinessOutcomeExcel(CompanyId, Guid.Empty.ToString(), TeamId));
            var response = await client.SendAsync(request);

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }


        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Excel_Get_NotFound()
        {
            //arrange
            var client = GetAuthenticatedClient();

            // act
            using var request = new HttpRequestMessage(HttpMethod.Get, RequestUris.ReportsBusinessOutcomeExcel(CompanyId, "", TeamId));
            var response = await client.Result.SendAsync(request);

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code didn't match");
        }

    }
}