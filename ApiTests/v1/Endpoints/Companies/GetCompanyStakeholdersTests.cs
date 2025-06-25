using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Utilities;
using System.Linq;
using AtCommon.ObjectFactories;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompanyStakeholdersTests : BaseV1Test
    {
        private static Dictionary<string, object> _queries;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _queries = new Dictionary<string, object>()
            {
                {"currentPage", "1"},
                {"pageSize", "2"}
            };
        }
        public static int MaxValueOf32BitInt = int.MaxValue;

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(2, response.Dto.PageSize, "Page Size doesn't match");
            Assert.AreEqual(2, response.Dto.Stakeholders.Count, "Total stakeholders doesn't match");
            Assert.IsTrue(response.Dto.Stakeholders.All(dto => dto.TeamId > 0), "Team ID should not < 0");

        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Stakeholders_Without_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1}
            };

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.PageSize >= 0, "Page size is zero or null");
            Assert.IsTrue(response.Dto.Stakeholders.Count >= 0, "Stakeholders count is zero or null");
            Assert.IsTrue(response.Dto.Stakeholders.All(dto => dto.TeamId > 0), "Team ID should not < 0");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_CurrentPage_Zero_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter("currentPage", 0));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.PageCount, "Page count doesn't match");
            Assert.IsTrue(response.Dto.LastRowOnPage > 0, "Last Row On Page  should not < 0");
            Assert.IsTrue(response.Dto.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Stakeholders.All(dto => dto.TeamId > 0), "Team ID should not < 0");
        }

        [TestMethod]
        [TestCategory("Rocket")]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Stakeholders_CurrentPage_MaxLimit_And_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", MaxValueOf32BitInt},
                {"pageSize", 1}
            };

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(MaxValueOf32BitInt, response.Dto.CurrentPage, "Current Page do not match");
            Assert.AreEqual(1, response.Dto.PageSize, "Page Size do not match");
            Assert.IsTrue(response.Dto.LastRowOnPage >= 0, "LastRowOnPage is less than zero");
            Assert.IsTrue(response.Dto.PageCount >= 0, "PageCount is less than zero");
            Assert.IsTrue(response.Dto.RowCount >= 0, "RowCount is less than zero");
            Assert.IsTrue(response.Dto.Stakeholders.Count == 0, "Stakeholder is not zero");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Archive_Stakeholder_Success()
        {
            // Given 
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"includeArchived", true}
            };

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(queries));
            var archiveStakeholder = MemberFactory.GetArchiveStakeholder();

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.That.ListContains(response.Dto.Stakeholders.Select(stakeholder => stakeholder.FirstName).ToList(), archiveStakeholder.FirstName, $"List does not contain '{archiveStakeholder.FirstName}'");
            Assert.That.ListContains(response.Dto.Stakeholders.Select(stakeholder => stakeholder.LastName).ToList(), archiveStakeholder.LastName, $"List does not contain '{archiveStakeholder.LastName}'");
            Assert.That.ListContains(response.Dto.Stakeholders.Select(stakeholder => stakeholder.Email).ToList(), archiveStakeholder.Email, $"List does not contain '{archiveStakeholder.Email}'");
            Assert.IsTrue(response.Dto.PageCount > 0, "Page Count is <= zero");
            Assert.IsTrue(response.Dto.LastRowOnPage > 0, "Last Row On Page is <= zero ");
            Assert.IsTrue(response.Dto.PageSize > 0, "Page Size is <= zero");
            Assert.IsTrue(response.Dto.RowCount > 0, "Row count is <= zero");

        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Stakeholders_Without_CurrentPage_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompanyStakeHolders(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_With_CurrentPage_And_PageSize_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "-123"},
                {"pageSize", "-123"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_CurrentPage_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter("currentPage", MaxValueOf32BitInt));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_PageSize_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "1"},
                {"pageSize", MaxValueOf32BitInt}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(Company.Id).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Stakeholders_Forbidden()
        {
            // Given 
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyStakeHolders(SharedConstants.FakeCompanyId).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }
    }
}