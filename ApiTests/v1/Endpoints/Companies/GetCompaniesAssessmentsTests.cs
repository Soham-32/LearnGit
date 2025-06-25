using AtCommon.Api;
using AtCommon.Dtos.Companies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Utilities;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesAssessmentsTests : BaseV1Test
    {
        private static readonly Dictionary<string, object> Queries = new Dictionary<string, object>()
        {
            {"currentPage", "1"},
            {"pageSize", "2"}
        };
        public static int MaxValueOf32BitInt = int.MaxValue;

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(2, response.Dto.PageSize, "Page Size doesn't match");
            Assert.AreEqual(2, response.Dto.Assessments.Count, "Total assessments doesn't match");
            Assert.IsTrue(response.Dto.Assessments.All(a => a.TeamId >= 0), "Team ID should not < 0");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberPercentageCompleted == 0 || dto.TeamMemberPercentageCompleted > 10), "TeamMemberPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderPercentageCompleted == 0 || dto.StakeholderPercentageCompleted > 10), "StakeholderPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMembersCount != null), "TeamMembers Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholdersCount != null), "Stakeholders Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberRespondents != null), "TeamMember Respondents is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderRespondents != null), "Stakeholder Respondents  is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.AgilityHealthIndex == null || dto.AgilityHealthIndex > 0), "HeathIndex is not null or 0");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessment_Without_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1}
            };

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.IsTrue(response.Dto.PageSize >= 0, "Page size is zero or null");
            Assert.IsTrue(response.Dto.Assessments.Count >= 0, "Assessment count is zero or null");
            Assert.IsTrue(response.Dto.Assessments.All(a => a.TeamId >= 0), "Team ID should not < 0");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberPercentageCompleted == 0 || dto.TeamMemberPercentageCompleted > 10), "TeamMemberPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderPercentageCompleted == 0 || dto.StakeholderPercentageCompleted > 10), "StakeholderPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMembersCount != null), "TeamMembers Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholdersCount != null), "Stakeholders Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberRespondents != null), "TeamMember Respondents is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderRespondents != null), "Stakeholder Respondents  is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.AgilityHealthIndex == null || dto.AgilityHealthIndex > 0), "HeathIndex is not null or 0");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_CurrentPage_Zero_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter("currentPage", 0));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.PageCount, "Page count doesn't match");
            Assert.AreEqual(1, response.Dto.FirstRowOnPage, "First Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.LastRowOnPage > 0, "Last Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Assessments.Count > 0, "Total assessments should not < 0");
            Assert.IsTrue(response.Dto.Assessments.All(a => a.TeamId >= 0), "Team ID should not < 0");
            Assert.IsTrue(response.Dto.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberPercentageCompleted == 0 || dto.TeamMemberPercentageCompleted > 10), "TeamMemberPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderPercentageCompleted == 0 || dto.StakeholderPercentageCompleted > 10), "StakeholderPercentage is below 10");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMembersCount != null), "TeamMembers Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholdersCount != null), "Stakeholders Count is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.TeamMemberRespondents != null), "TeamMember Respondents is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.StakeholderRespondents != null), "Stakeholder Respondents  is null");
            Assert.IsTrue(response.Dto.Assessments.All(dto => dto.AgilityHealthIndex == null || dto.AgilityHealthIndex > 0), "HeathIndex is not null or 0");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_CurrentPage_MaxLimit_And_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", MaxValueOf32BitInt},
                {"pageSize", 1}
            };

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(MaxValueOf32BitInt, response.Dto.CurrentPage, "Current Page do not match");
            Assert.AreEqual(1, response.Dto.PageSize, "Page Size do not match");
            Assert.IsTrue(response.Dto.FirstRowOnPage >= 0, "FirstRowOnPage is less than zero");
            Assert.IsTrue(response.Dto.LastRowOnPage >= 0, "LastRowOnPage is less than zero");
            Assert.IsTrue(response.Dto.PageCount >= 0, "PageCount is less than zero");
            Assert.IsTrue(response.Dto.RowCount >= 0, "RowCount is less than zero");
            Assert.IsTrue(response.Dto.Assessments.Count == 0, "Assessments is not zero");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_CurrentPage_And_PageSize_MaxLimit_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            const int maxPageLimitValue = 1000;
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"pageSize", maxPageLimitValue}
            };

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(maxPageLimitValue, response.Dto.PageSize, "Current Page do not match");
            Assert.AreEqual(1, response.Dto.CurrentPage, "Page Size do not match");
            Assert.IsTrue(response.Dto.FirstRowOnPage >= 0, "FirstRowOnPage is less than zero");
            Assert.IsTrue(response.Dto.LastRowOnPage >= 0, "LastRowOnPage is less than zero");
            Assert.IsTrue(response.Dto.PageCount >= 0, "PageCount is less than zero");
            Assert.IsTrue(response.Dto.RowCount >= 0, "RowCount is less than zero");
            Assert.IsTrue(response.Dto.Assessments.Count >= 0, "Assessments is less than zero");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_CurrentPage_And_PageSize_MoreThan_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            const int maxPageLimitValue = 1001;
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 1},
                {"pageSize", maxPageLimitValue}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessment_Without_CurrentPage_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessments(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Company_Get_Assessment_With_CurrentPage_And_PageSize_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "-123"},
                {"pageSize", "-123"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_CurrentPage_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter("currentPage", MaxValueOf32BitInt));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");

        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();
            var companyId = Company.Id;

            // When
            var response = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(companyId).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_Assessments_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessments(SharedConstants.FakeCompanyId).AddQueryParameter(Queries));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }
    }
}

