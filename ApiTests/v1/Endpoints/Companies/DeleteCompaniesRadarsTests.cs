using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class DeleteCompaniesRadarsTests : BaseV1Test
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Survey_Delete_OK()
        {
            // given
            var client = await GetAuthenticatedClient();

            // add a company
            _companyRequest = CompanyFactory.GetValidPostCompany();
            _companyRequest.Name = $"ZZZ_DeleteRadar{Guid.NewGuid()}";
            var companyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), _companyRequest);
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.Id;
            // get surveyId
            var radarResponse = await client.GetAsync<IList<RadarResponse>>(RequestUris.Radars());
            radarResponse.EnsureSuccess();
            var surveyId = radarResponse.Dto.First().Id;
            // add radar
            var request = new AddCompanyRadarRequest
            {
                CompanyId = companyId,
                SurveyId = surveyId,
                Editable = false
            };

            var surveyResponse = await client.PostAsync<CompanyRadarResponse>(RequestUris.CompaniesRadars(companyId), request);
            surveyResponse.EnsureSuccess();
            
            //when
            var response =
                await client.DeleteAsync(RequestUris.CompaniesRadarsSurvey(companyId, surveyId));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match.");

        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Survey_Delete_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var companyId = Company.Id;
            const int surveyId = 1;

            // when
            var response =
                await client.DeleteAsync(RequestUris.CompaniesRadarsSurvey(companyId, surveyId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");

        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_Radars_Survey_Delete_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var companyId = Company.Id;
            const int surveyId = 1;

            // when
            var response =
                await client.DeleteAsync(RequestUris.CompaniesRadarsSurvey(companyId, surveyId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_Radars_Survey_Delete_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var companyId = Company.Id;
            const int surveyId = 1;

            // when
            var response =
                await client.DeleteAsync(RequestUris.CompaniesRadarsSurvey(companyId, surveyId));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match.");

        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }
    }
}
