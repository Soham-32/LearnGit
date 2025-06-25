using System;
using System.Linq;
using System.Net;
using AtCommon.Api;
using AtCommon.Utilities;
using System.Threading.Tasks;
using AtCommon.Dtos.Companies;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesGrowthPlanItemsTests : BaseV1Test
    {
        private static int _teamId;
        public static int MaxValueOf32BitInt = int.MaxValue;
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;
        }

        private static readonly Dictionary<string, object> Queries = new Dictionary<string, object>
        {
            {"currentPage", 1},
            {"pageSize", 1}
        };

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1}
            };

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes doesn't match");

            var currentPage = int.Parse(queries["currentPage"].ToString());
            var pageSize = int.Parse(response.Dto.Paging.PageSize.ToString());
            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (currentPage * pageSize) - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.TeamId >= 0), "TeamId is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Title != null), "Title is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Category != null), "Category is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.PriorityId >= 0), "PriorityId is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => !string.IsNullOrEmpty(d.Priority)), "Priority is null or empty");

            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Status != null)), "Status is mull or empty");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Date != null)), "Updated status date and time is null ir empty");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_TeamId_PageSize_CurrentPage_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"teamId", _teamId},
                {"pageSize", 1}
            };

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes doesn't match");

            var currentPage = int.Parse(queries["currentPage"].ToString());
            var pageSize = int.Parse(queries["pageSize"].ToString());
            var lastRowOnPage = currentPage * pageSize;
            var firstRowOnPage = lastRowOnPage - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.AreEqual(pageSize, response.Dto.GrowthPlanItems.Select(d => d.TeamId).Count(), "Page-size and Total teams doesn't match");
            Assert.AreEqual(pageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.TeamId == _teamId), "TeamId is not matched");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Title != null), "Title is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Category != null), "Category is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => !string.IsNullOrEmpty(d.Priority)), "Priority is null or empty");

            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Status != null)), "Status is mull or empty");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Date != null)), "Updated status date and time is null ir empty");

        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_CurrentPage_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage","2"},
                {"pageSize", "2"}
            };

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes doesn't match");

            var currentPage = int.Parse(queries["currentPage"].ToString());
            var pageSize = int.Parse(queries["pageSize"].ToString());
            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (currentPage * pageSize) - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            // Verify  Results
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.TeamId >= 0), "TeamId is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Title != null), "Title is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Category != null), "Category is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => !string.IsNullOrEmpty(d.Priority)), "Priority is null or empty");

            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Status != null)), "Status is mull or empty");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Date != null)), "Updated status date and time is null ir empty");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_TeamId_CurrentPage_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"teamId",_teamId}
            };

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes doesn't match");

            var currentPage = int.Parse(queries["currentPage"].ToString());
            var pageSize = int.Parse(response.Dto.Paging.PageSize.ToString());
            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (currentPage * pageSize) - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.TeamId == _teamId), "TeamId is not matched");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Title != null), "Title is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Category != null), "Category is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => !string.IsNullOrEmpty(d.Priority)), "Priority is null or empty");

            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Status != null)), "Status is mull or empty");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Date != null)), "Updated status date and time is null ir empty");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_CurrentPage_Zero_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter("currentPage", 0));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageCount, "Page count doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.FirstRowOnPage, "First Row On Page doesn't match");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");

            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.TeamId >= 0), "TeamId is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Title != null), "Title is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.Category != null), "Category is null");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => d.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.GrowthPlanItems.All(d => !string.IsNullOrEmpty(d.Priority)), "Priority is null or empty");

            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Status != null)), "Status is mull or empty");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Assert.IsTrue(response.Dto.GrowthPlanItems.Select(a => a.Statuses).All(q => q.All(a => a.Date != null)), "Updated status date and time is null ir empty");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesGrowthPlanItems(SharedConstants.FakeCompanyId));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_ValidCompanyId_InvalidCurrentPage_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage", "-123"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_PageSize_MoreThanMaxValue_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage","2"},
                {"pageSize", "1001"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes doesn't match");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_InvalidCompanyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(SharedConstants.FakeCompanyId).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_Get_GrowthPlanItems_With_ValidCompanyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyGrowthPlanItemPagedResponse>(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_CompanyId_MaxLimit_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var companyId = MaxValueOf32BitInt;

            // When
            var response = await client.GetAsync(RequestUris.CompaniesGrowthPlanItems(companyId).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        public async Task Companies_Get_GrowthPlanItems_With_TeamId_MaxLimit_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage","0"},
                {"teamId", MaxValueOf32BitInt}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesGrowthPlanItems(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

    }
}
