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
    public class AddCompaniesRadarsTests : BaseV1Test
    {
        private static AddCompanyRequest _companyRequest;
        // 201
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Post_OK()
        {
            // given
            // auth
            var client = await GetAuthenticatedClient();
            // add a company
            _companyRequest = CompanyFactory.GetValidPostCompany();
            _companyRequest.Name = $"ZZZ_AddRadar{Guid.NewGuid()}";
            var companyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), _companyRequest);
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.Id;
            // get the surveyId
            var radarResponse = await client.GetAsync<IList<RadarResponse>>(RequestUris.Radars());
            radarResponse.EnsureSuccess();
            var surveyId = radarResponse.Dto.First().Id;

            var request = new AddCompanyRadarRequest
            {
                CompanyId = companyId,
                SurveyId = surveyId,
                Editable = false
            };

            // when
            var response = await client.PostAsync<CompanyRadarResponse>(RequestUris.CompaniesRadars(companyId), request);

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status Code doesn't match.");
            Assert.IsTrue(response.Dto.CompanySurveyId > 0, "CompanySurveyId is not valid.");
            Assert.AreEqual(request.CompanyId, response.Dto.CompanyId, "CompanyId does not match.");
            Assert.AreEqual(request.SurveyId, response.Dto.SurveyId, "SurveyId does not match.");
            Assert.AreEqual(request.Editable, response.Dto.Editable, "Editable does not match.");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Post_MissingCompanyId_BadRequest()
        {
            // given
            // auth
            var client = await GetAuthenticatedClient();
            var companyResponse =
                await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First(c => c.Name.StartsWith("Automation2")).Id;
            var radarResponse = await client.GetAsync<IList<RadarResponse>>(RequestUris.Radars());
            radarResponse.EnsureSuccess();
            var surveyId = radarResponse.Dto.First().Id;

            // when
            var request = new AddCompanyRadarRequest
            {
                SurveyId = surveyId,
                Editable = false
            };

            // when
            var response = await client.PostAsync<IList<string>>(RequestUris.CompaniesRadars(companyId), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("'Company Id' must not be empty.", response.Dto.First(), "Response Body doesn't match.");

        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Post_MissingSurveyId_BadRequest()
        {
            // given
            // auth
            var client = await GetAuthenticatedClient();
            var companyResponse =
                await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First(c => c.Name.StartsWith("Automation2")).Id;

            // when
            var request = new AddCompanyRadarRequest
            {
                CompanyId = companyId,
                Editable = false
            };

            // when
            var response = await client.PostAsync<IList<string>>(RequestUris.CompaniesRadars(companyId), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.AreEqual("'Survey Id' must not be empty.", response.Dto.First(), "Response Body doesn't match.");

        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Radars_Post_Unauthorized()
        {
            var client = GetUnauthenticatedClient();
            var request = new AddCompanyRadarRequest
            {
                CompanyId = Company.Id,
                Editable = false
            };

            // when
            var response = await client.PostAsync<IList<string>>(RequestUris.CompaniesRadars(Company.Id), request);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Companies_Radars_Post_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var companyResponse =
                await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id;
            var radarResponse = await client.GetAsync<IList<RadarResponse>>(RequestUris.Radars());
            radarResponse.EnsureSuccess();
            var surveyId = radarResponse.Dto.First().Id;

            // when
            var request = new AddCompanyRadarRequest
            {
                CompanyId = companyId,
                SurveyId = surveyId,
                Editable = false
            };

            // when
            var response = await client.PostAsync(RequestUris.CompaniesRadars(companyId), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
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
