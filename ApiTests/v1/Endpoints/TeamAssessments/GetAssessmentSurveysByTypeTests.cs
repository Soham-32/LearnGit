using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.TeamAssessments
{
    [TestClass]
    [TestCategory("TeamAssessment")]
    public class GetAssessmentAssessmentSurveysByTypeTests : BaseV1Test
    {
        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_TeamType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Name != null), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Team"), "Type 'Team' no longer exists");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_MultiTeamType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.MultiTeam));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.AreEqual(0, assessmentTypeResponse.Dto.Count, "Response is not null");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_IndividualType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Individual));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Name != null), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Individual"), "Type 'Individual' no longer exists");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_OrganizationalType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Organizational));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.AreEqual(0, assessmentTypeResponse.Dto.Count, "Response is not null");
        }


        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_FacilitatorType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Facilitator));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Name != null), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Facilitator"), "Type 'Facilitator' no longer exists");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Uid.CompareTo(Guid.Empty) != 0), "Uid is empty");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            const int companyId = SharedConstants.FakeCompanyId;
            var assessmentTypeResponse = await client.GetAsync(RequestUris.AssessmentSurveysByType(companyId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, assessmentTypeResponse.StatusCode, "Status codes do not match");
        }

        //403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            const int companyId = SharedConstants.FakeCompanyId;
            var assessmentTypeResponse = await client.GetAsync(RequestUris.AssessmentSurveysByType(companyId));
            
            Assert.AreEqual(HttpStatusCode.Forbidden, assessmentTypeResponse.StatusCode, "Status codes do not match");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Assessments_SurveysByType_Get_NotFound()
        {
            var client = await GetAuthenticatedClient();

            const double companyId = 3333333333;
            var assessmentTypeResponse = await client.GetAsync(RequestUris.AssessmentSurveysByType(companyId));

            Assert.AreEqual(HttpStatusCode.NotFound, assessmentTypeResponse.StatusCode, "Status codes do not match");
        }

    }
}