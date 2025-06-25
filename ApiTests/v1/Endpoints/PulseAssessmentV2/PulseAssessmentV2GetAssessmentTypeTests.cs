using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetAssessmentTypeTests : PulseApiTestBase
    {
        private static TypesResponse _radarTypeResponse;
        private static TeamHierarchyResponse _teamResponse;
        private static TeamHierarchyResponse _multiTeamResponse;
        private static TeamHierarchyResponse _enterpriseTeamResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupApi = new SetupTeardownApi(TestEnvironment);

            _teamResponse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            _multiTeamResponse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            _enterpriseTeamResponse = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam);
            _radarTypeResponse = setupApi.GetSurveyType(Company.Id, Convert.ToInt32(AssessmentType.Team), SharedConstants.AtTeamHealth3RadarName, user);
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Team_AssessmentsTypes_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<SurveyTypesResponse>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter("companyId", Company.Id)
                    .AddQueryParameter("teamId", _teamResponse.TeamId));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Type).ToList(), _teamResponse.Type, "Team Type does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Id.ToString()).ToList(), _radarTypeResponse.Id.ToString(), "Assessment type ID does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Name).ToList(), _radarTypeResponse.Name, "Assessment type name does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Uid.ToString()).ToList(), _radarTypeResponse.Uid.ToString(), "Assessment type Uid does not match");
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_MultiTeam_AssessmentsTypes_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<SurveyTypesResponse>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter("companyId", Company.Id)
                    .AddQueryParameter("teamId", _multiTeamResponse.TeamId));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Type).ToList(), _teamResponse.Type, "Team Type does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Id.ToString()).ToList(), _radarTypeResponse.Id.ToString(), "Assessment type ID does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Name).ToList(), _radarTypeResponse.Name, "Assessment type name does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Uid.ToString()).ToList(), _radarTypeResponse.Uid.ToString(), "Assessment type Uid does not match");
        }

        //200
        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_EnterpriseTeam_AssessmentsTypes_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<SurveyTypesResponse>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter("companyId", Company.Id)
                    .AddQueryParameter("teamId", _enterpriseTeamResponse.TeamId));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Type).ToList(), _teamResponse.Type, "Team Type does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Id.ToString()).ToList(), _radarTypeResponse.Id.ToString(), "Assessment type ID does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Name).ToList(), _radarTypeResponse.Name, "Assessment type name does not match");
            Assert.That.ListContains(assessmentTypesResponse.Dto.Select(d => d.Uid.ToString()).ToList(), _radarTypeResponse.Uid.ToString(), "Assessment type Uid does not match");
        }

        //400
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_AssessmentTypes_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", 0 }
            };

            //When
            var assessmentTypesResponse = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Team Id' is not valid", assessmentTypesResponse.Dto[0], "Response messages are not the same");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_AssessmentTypes_InvalidCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 0},
                { "teamId", _teamResponse.TeamId }
            };

            //When
            var assessmentTypesResponse = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Company Id' is not valid", assessmentTypesResponse.Dto[0], "Response messages are not the same");
        }

        //401
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_AssessmentsTypes_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //When
            var assessmentTypesResponse = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter("teamId", _teamResponse.TeamId));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, assessmentTypesResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_AssessmentsTypes_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", SharedConstants.FakeCompanyId },
                { "teamId", _teamResponse.TeamId }
            };

            //When
            var assessmentTypesResponse = await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2AssessmentTypes().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, assessmentTypesResponse.StatusCode, "Status code does not match");
        }
    }
}
