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
    public class GetCompanyMembersTests : BaseV1Test
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
        public async Task Company_Get_Members_Success()
        {
            // Given 
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyMemberResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(2, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(2, response.Dto.MemberResponse.Count, "Total stakeholders doesn't match");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.MemberResponse.All(dto => dto.TeamId > 0), "Team ID should not < 0");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Members_Without_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1}
            };

            // When
            var response = await client.GetAsync<CompanyMemberResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageCount, "Page count doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.FirstRowOnPage, "First Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.MemberResponse.All(dto => dto.TeamId > 0), "Team ID should not < 0");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_CurrentPage_Zero_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyMemberResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter("currentPage", 0));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageCount, "Page count doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.FirstRowOnPage, "First Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.MemberResponse.All(dto => dto.TeamId > 0), "Team ID should not < 0");
        }

        [TestMethod]
        [TestCategory("Rocket")]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Members_CurrentPage_MaxLimit_And_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", MaxValueOf32BitInt},
                {"pageSize", 1}
            };

            // When
            var response = await client.GetAsync<CompanyMemberResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(MaxValueOf32BitInt, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.PageCount >= 0, "Page Count is less than zero");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage >= 0, "First Row On Page is less than zero ");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage >= 0, "Last Row On Page is less than zero ");
            Assert.IsTrue(response.Dto.Paging.PageSize >= 0, "Page Size is less than zero");
            Assert.IsTrue(response.Dto.Paging.RowCount >= 0, "Row count is less than zero");
            Assert.IsTrue(response.Dto.MemberResponse.Count == 0, "Member Response is not zero");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Archive_Members_Success()
        {
            // Given 
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"includeArchived", true}
            };

            // When
            var response =
                await client.GetAsync<CompanyMemberResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(queries));

            var archiveTeamMember = MemberFactory.GetArchiveTeamMember();

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.That.ListContains(response.Dto.MemberResponse.Select(teamMember => teamMember.FirstName).ToList(), archiveTeamMember.FirstName, $"List does not contain '{archiveTeamMember.FirstName}'");
            Assert.That.ListContains(response.Dto.MemberResponse.Select(teamMember => teamMember.LastName).ToList(), archiveTeamMember.LastName, $"List does not contain '{archiveTeamMember.LastName}'");
            Assert.That.ListContains(response.Dto.MemberResponse.Select(teamMember => teamMember.Email).ToList(), archiveTeamMember.Email, $"List does not contain '{archiveTeamMember.Email}'");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Count is <= zero");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page is <= zero");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page is <= zero");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size is <= zero");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count is <= zero");
            Assert.IsTrue(response.Dto.MemberResponse.Count != 0, "Member Response is zero");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Members_Without_CurrentPage_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompanyMembers(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_With_CurrentPage_And_PageSize_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "-123"},
                {"pageSize", "-123"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_CurrentPage_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompanyMembers(Company.Id).AddQueryParameter("currentPage", MaxValueOf32BitInt));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_PageSize_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "1"},
                {"pageSize", MaxValueOf32BitInt}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyMembers(Company.Id).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Members_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyStakeholderResponse>(RequestUris.CompanyMembers(SharedConstants.FakeCompanyId).AddQueryParameter(_queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }
    }
}