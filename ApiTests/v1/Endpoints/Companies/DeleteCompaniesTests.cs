using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class DeleteCompaniesTests : BaseV1Test
    {
        private static AddCompanyRequest _companyRequest;
        // 200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Delete_Success()
        {
            // given
            var client = await GetAuthenticatedClient();
            _companyRequest = CompanyFactory.GetValidPostCompany();
            _companyRequest.Name = $"ZZZ_Delete{Guid.NewGuid()}";

            var postCompanyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), _companyRequest);
            postCompanyResponse.EnsureSuccess();
            var companyId = postCompanyResponse.Dto.Id;

            // when
            var response = await client.DeleteAsync(RequestUris.CompanyDetails(companyId));
            

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            var companyListResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            Assert.IsFalse(companyListResponse.Dto.Any(c => c.Id == companyId), 
                $"The company with id: <{companyId}> was not deleted.");

        }

        // 401
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Delete_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            const int companyId = 10000;

            // when
            var response = await client.DeleteAsync(RequestUris.CompanyDetails(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Companies_Delete_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var companyListResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            var companyId = companyListResponse.Dto.Last().Id;
            
            // when
            var response = await client.DeleteAsync(RequestUris.CompanyDetails(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin")]
        public async Task Companies_Delete_NoCompanyAccess_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            const int companyId = 10000;

            // when
            var response = await client.DeleteAsync(RequestUris.CompanyDetails(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");

        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_Delete_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            const int companyId = 1000000;

            // when
            var response = await client.DeleteAsync(RequestUris.CompanyDetails(companyId));

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code does not match.");

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
