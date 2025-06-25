using System;
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
    public class GetCompaniesAssessmentsResultsTests : BaseV1Test
    {
        private static readonly Dictionary<string, object> Queries = new Dictionary<string, object>
        {
            {"currentPage", 3},
            {"pageSize", 1}
        };
        public static int MaxValueOf32BitInt = int.MaxValue;

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");

            var currentPage = int.Parse(Queries["currentPage"].ToString());
            var pageSize = int.Parse(Queries["pageSize"].ToString());
            var lastRowOnPage = currentPage * pageSize;
            var firstRowOnPage = lastRowOnPage - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.AreEqual(pageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            // Verify Assessment Results
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.TeamId >= 0), "TeamID is should not be less then 0");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => !d.UId.Equals(null)), " Assessment Uid is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.Name != null), "Assessment Name is null");

            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.All(dimensionsName => dimensionsName.Name != null)), "DimensionsName is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).All(subDimensionsName => subDimensionsName.Name != null)), "SubDimensions Name is null");

            var expectedCompetencyList = response.Dto.AssessmentResults.SelectMany(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).SelectMany(competencies => competencies.Competencies)).ToList();
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Name != null), "Competencies Name is null");
            Assert.IsTrue(expectedCompetencyList.All(competenciesAverageValue => (competenciesAverageValue.AverageValue >= 0 && competenciesAverageValue.AverageValue<=10)), "AverageValue of competencies is below 0 or null");

            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Min >= 0), "Minimum value of Competencies is less then 0");
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Max >= 0), "maximum value of competencies is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => contact.ContactId >= 0), "ContactId is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => (contact.AverageValue >= 0 && contact.AverageValue<=10)), "Contacts averageValue is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.QuestionId >= 0), "Questions Id is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.Value >= 0), "Questions value is less then 0");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_Without_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", 9}
            };

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");

            var currentPage = int.Parse(queries["currentPage"].ToString());
            var pageSize = int.Parse(response.Dto.Paging.PageSize.ToString());
            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (currentPage * pageSize) - (pageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(pageSize));

            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.TeamId >= 0), "TeamID is should not be less then 0");
            Assert.AreEqual(currentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "There are 0 Row counts");

            // Verify Assessment Results
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.TeamId >= 0), "TeamID is should not be less then 0");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.UId.Equals(null)), "Assessment Uid is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.Name != null), "Assessment Name is null");

            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.All(dimensionsName => dimensionsName.Name != null)), "DimensionsName is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).All(subDimensionsName => subDimensionsName.Name != null)), "SubDimensions Name is null");

            var expectedCompetencyList = response.Dto.AssessmentResults.SelectMany(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).SelectMany(competencies => competencies.Competencies)).ToList();
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Name != null), "Competencies Name is null");
            Assert.IsTrue(expectedCompetencyList.All(competenciesAverageValue => (competenciesAverageValue.AverageValue >= 0 && competenciesAverageValue.AverageValue <= 10)), "AverageValue of competencies is below 0 or null");

            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Min >= 0), "Minimum value of Competencies is less then 0");
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Max >= 0), "maximum value of competencies is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => contact.ContactId >= 0), "ContactId is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => (contact.AverageValue >= 0 && contact.AverageValue <= 10)), "Contacts averageValue is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.QuestionId >= 0), "Questions Id is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.Value >= 0), "Questions value is less then 0");

        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_CurrentPage_Zero_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter("currentPage", 0));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageCount, "Page count doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.FirstRowOnPage, "First Row On Page doesn't match");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");

            // Verify Assessment Results
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.TeamId >= 0), "TeamID is should not be less then 0");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => !d.UId.Equals(null)), " Assessment Uid is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(d => d.Name != null), "Assessment Name is null");

            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.All(dimensionsName => dimensionsName.Name != null)), "DimensionsName is null");
            Assert.IsTrue(response.Dto.AssessmentResults.All(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).All(subDimensionsName => subDimensionsName.Name != null)), "SubDimensions Name is null");

            var expectedCompetencyList = response.Dto.AssessmentResults.SelectMany(dimension => dimension.Dimensions.SelectMany(subDimensions => subDimensions.SubDimensions).SelectMany(competencies => competencies.Competencies)).ToList();
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Name != null), "Competencies Name is null");
            Assert.IsTrue(expectedCompetencyList.All(competenciesAverageValue => (competenciesAverageValue.AverageValue >= 0 && competenciesAverageValue.AverageValue <= 10)), "AverageValue of competencies is below 0 or null");

            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Min >= 0), "Minimum value of Competencies is less then 0");
            Assert.IsTrue(expectedCompetencyList.All(competenciesName => competenciesName.Max >= 0), "maximum value of competencies is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => contact.ContactId >= 0), "ContactId is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).All(contact => (contact.AverageValue >= 0 && contact.AverageValue <= 10)), "Contacts averageValue is less then 0");

            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.QuestionId >= 0), "Questions Id is less then 0");
            Assert.IsTrue(expectedCompetencyList.SelectMany(competenciesName => competenciesName.Contacts).SelectMany(contacts => contacts.Questions).All(question => question.Value >= 0), "Questions value is less then 0");

        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_CurrentPage_MaxLimit_And_PageSize_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", MaxValueOf32BitInt},
                {"pageSize", 1}
            };

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(MaxValueOf32BitInt, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Count should not > 0");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0 ");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.AssessmentResults.Count == 0, "Assessment Results is not zero");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_Without_CurrentPage_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessmentResults(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_With_CurrentPage_And_PageSize_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var queries = new Dictionary<string, object>
            {
                {"currentPage", "-123"},
                {"pageSize", "-123"}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(queries));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_CurrentPage_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter("currentPage", MaxValueOf32BitInt));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Company_AssessmentResults_PageSize_MaxLimit_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var queries = new Dictionary<string, object>
            {
                {"currentPage", "1"},
                {"pageSize", MaxValueOf32BitInt}
            };

            // When
            var response = await client.GetAsync(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(queries));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(Company.Id).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("CompanyAdmin"), TestCategory("SiteAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Get_AssessmentResults_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<CompanyAssessmentsResultsResponse>(RequestUris.CompaniesAssessmentResults(SharedConstants.FakeCompanyId).AddQueryParameter(Queries));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match.");
        }
    }
}
