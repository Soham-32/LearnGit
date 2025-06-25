using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Users;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesPrimaryCompanyContactTests : BaseV1Test
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_PrimaryCompanyContact_Get_Ok()
        {
            var client = await GetAuthenticatedClient();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(Company.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response status code doesn't match.");
            Assert.AreEqual("Automation", response.Dto.FirstName, "FirstName doesn't match.");
            Assert.AreEqual("Contact", response.Dto.LastName, "LastName doesn't match.");
            Assert.AreEqual("ah_automation+primarycontact@agiletransformation.com", response.Dto.UserName, "UserName doesn't match.");
            Assert.AreEqual("ah_automation+primarycontact@agiletransformation.com", response.Dto.Email, "Email doesn't match.");
            Assert.AreEqual(Company.Id, response.Dto.CompanyId, "CompanyId doesn't match.");
            
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_PrimaryCompanyContact_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(Company.Id));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_PrimaryCompanyContact_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(Company.Id));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response status code doesn't match.");

        }

        [TestMethod]
        [TestCategory("PartnerAdmin")]
        public async Task Companies_PrimaryCompanyContact_Get_PA_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(1));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response status code doesn't match.");

        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Companies_PrimaryCompanyContact_Get_Company_NoContent()
        {
            var client = await GetAuthenticatedClient();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(0));

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Response status code doesn't match.");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin")]
        public async Task Companies_PrimaryCompanyContact_Get_Contact_NoContent()
        {
            var client = await GetAuthenticatedClient();

            _companyRequest = CompanyFactory.GetValidPostCompany();
            _companyRequest.CompanyAdminFirstName = null;
            _companyRequest.CompanyAdminLastName = null;
            _companyRequest.CompanyAdminEmail = null;
            _companyRequest.Name = $"ZZZ_PCC{Guid.NewGuid()}";

            var companyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), _companyRequest);
            companyResponse.EnsureSuccess();

            var response =
                await client.GetAsync<UserResponse>(
                    RequestUris.CompaniesPrimaryCompanyContact(companyResponse.Dto.Id));

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Response status code doesn't match.");

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
