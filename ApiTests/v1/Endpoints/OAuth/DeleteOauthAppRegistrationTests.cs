using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.OAuth;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.OAuth
{
    [TestClass]
    [TestCategory("Oauth")]
    public class DeleteOauthAppRegistrationTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Oauth_AppRegistration_Delete_OK()
        {
            // given
            var request = new AddAppRegistrationRequest {AppName = $"temp{Guid.NewGuid():D}"};
            var client = await GetAuthenticatedClient();

            // SA and PA cannot add new app registrations so get a CA client
            var addClient = User.IsSiteAdmin() || User.IsPartnerAdmin() ? GetCaClient() : client;
            var registerResponse = await addClient.PostAsync<AddAppRegistrationResponse>(RequestUris.OauthRegister(), request);
            registerResponse.EnsureSuccess();
            
            // when
            var response = await client.DeleteAsync(RequestUris.OauthAppRegistration(Company.Id)
                .AddQueryParameter("clientId", registerResponse.Dto.ClientId));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
        }
        
        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Oauth_AppRegistration_Delete_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.DeleteAsync(RequestUris.OauthAppRegistration(Company.Id)
                .AddQueryParameter("clientId", Guid.NewGuid().ToString("N")));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task Oauth_AppRegistration_Delete_Forbidden()
        {
            var client = await GetAuthenticatedClient();
            
            // when
            var response = await client.DeleteAsync(RequestUris.OauthAppRegistration(SharedConstants.FakeCompanyId)
                .AddQueryParameter("clientId", "somefakeclientId"));
            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

    }
}