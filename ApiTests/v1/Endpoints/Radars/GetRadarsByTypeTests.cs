using AtCommon.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Companies;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarsByTypeTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_TeamType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.RadarsByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Team"), "Type 'Team' no longer exists");
            //Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Uid.CompareTo(Guid.Empty) != 0), "Uid is empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_MultiTeamType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.RadarsByType(Company.Id).AddQueryParameter("type", AssessmentType.MultiTeam));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.AreEqual(0, assessmentTypeResponse.Dto.Count, "Response is not null");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_IndividualType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.RadarsByType(Company.Id).AddQueryParameter("type", AssessmentType.Individual));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Individual"), "Type 'Individual' no longer exists");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_OrganizationalType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.RadarsByType(Company.Id).AddQueryParameter("type", AssessmentType.Organizational));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.AreEqual(0, assessmentTypeResponse.Dto.Count, "Response is not null");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_FacilitatorType_Success()
        {
            var client = await GetAuthenticatedClient();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.RadarsByType(Company.Id).AddQueryParameter("type", AssessmentType.Facilitator));

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypeResponse.StatusCode, "Status codes do not match");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Id > 0), "Id value is null");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => !string.IsNullOrWhiteSpace(dto.Name)), "Name is empty");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Type == "Facilitator"), "Type 'Facilitator' no longer exists");
            Assert.IsTrue(assessmentTypeResponse.Dto.All(dto => dto.Uid.CompareTo(Guid.Empty) != 0), "Uid is empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task RadarsByType_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            const int companyId = 5;

            var assessmentTypeResponse = await client.GetAsync(RequestUris.RadarsByType(companyId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, assessmentTypeResponse.StatusCode, "Status codes do not match");
        }

    }
}