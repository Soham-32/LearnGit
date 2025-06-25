using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.OAuth
{
    [TestClass]
    [TestCategory("Oauth")]
    public class PostOauthRegisterTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Oauth_Register_Post_OK()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = new AddAppRegistrationRequest {AppName = $"temp{Guid.NewGuid():D}"};
            // when
            var response = await client.PostAsync<AddAppRegistrationResponse>(RequestUris.OauthRegister(), request);

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual(request.AppName, response.Dto.AppName, "AppName does not match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.ClientId), "ClientId is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.Secret), "Secret is null or empty.");
        }

        // 401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Oauth_Register_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var request = new AddAppRegistrationRequest {AppName = $"temp{Guid.NewGuid():D}"};
            // when
            var response = await client.PostAsync(RequestUris.OauthRegister(), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        public async Task Oauth_Register_Post_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = new AddAppRegistrationRequest {AppName = $"temp{Guid.NewGuid():D}"};
            // when
            var response = await client.PostAsync(RequestUris.OauthRegister(), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

    }
}