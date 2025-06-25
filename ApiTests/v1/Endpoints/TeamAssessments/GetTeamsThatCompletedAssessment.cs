using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.TeamAssessments
{
    [TestClass]
    [TestCategory("TeamAssessment")]
    public class GetTeamsThatCompletedAssessment : BaseV1Test
    {
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static int _surveyId;
        private static int _teamId;
        private const int FakeSurveyId = 3;
        private static SetupTeardownApi _setupApi;
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _setupApi = new SetupTeardownApi(TestEnvironment);
            var response = _setupApi.GetAssessmentResponse(SharedConstants.RadarTeam, SharedConstants.TeamHealth2Radar, CompanyAdminUser, Company.Id).GetAwaiter().GetResult();
            _surveyId = response.SurveyId;
            _teamId = response.TeamId;
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var companyResponse = await client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(Company.Id));
            var companyUid = companyResponse.Dto.Uid;
            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "companyUid", companyUid},
                { "surveyId", _surveyId}
            };

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter(query));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(1, response.Dto.Count, "Teams count does not match.");
            Assert.AreEqual(_teamId, int.Parse(response.Dto.FirstOrDefault()!), "Team Id does not match.");
        }

        //Bug - Response should not be null
        ////200
        //[TestMethod]
        //[TestCategory("SiteAdmin")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_CompanyUidAndSurveyId_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var companyResponse = await client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(Company.Id));
            var companyUid = companyResponse.Dto.Uid;
            var query = new Dictionary<string, object>
            {
                { "companyUid", companyUid},
                { "surveyId", _surveyId}
            };

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter(query));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(0, response.Dto.Count, "Teams count does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_CompanyIdAndSurveyId_Success()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
            {
                    { "companyId", Company.Id},
                    { "surveyId", _surveyId}
            };

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter(query));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual(1, response.Dto.Count, "Teams count does not match");
            Assert.AreEqual(_teamId, int.Parse(response.Dto.FirstOrDefault()!), "Team Id does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual("SurveyId is required", response.Dto.FirstOrDefault(), "Error message does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_CompanyUid_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var companyResponse = await client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(Company.Id));
            var companyUid = companyResponse.Dto.Uid;

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("companyUid", companyUid));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual("SurveyId is required", response.Dto.FirstOrDefault(), "Error message does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_CompanyId_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("companyId", Company.Id));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual("SurveyId is required", response.Dto.FirstOrDefault(), "Error message does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_FakeCompanyUid_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
                {
                    { "companyId", Company.Id},
                    { "companyUid", Guid.NewGuid() }
                };

            // when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter(query));

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_FakeSurveyId_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<List<string>>(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("surveyId", CSharpHelpers.RandomString()));

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("companyId", Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_FakeCompanyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("companyId", SharedConstants.FakeCompanyId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_SurveyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("surveyId", _surveyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_FakeSurveyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter("surveyId", FakeSurveyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_TeamsThatCompletedAssessment_Get_FakeCompanyUidAndSurveyId_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
            {
                    { "companyUid", Guid.NewGuid()},
                    { "surveyId", FakeSurveyId }
            };

            // when
            var response = await client.GetAsync(RequestUris.AssessmentsTeamsThatCompletedAssessment.AddQueryParameter(query));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }
    }
}

