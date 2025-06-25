using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.OAuth
{
    [TestClass]
    [TestCategory("Oauth")]
    public class PostOauthTokenTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("Public")]
        public async Task Oauth_Token_Post_OK()
        {
            // given
            var client = GetUnauthenticatedClient();
            var appRegistration = Company.GetOauthApp("Automation");

            var query = new Dictionary<string, object>
            {
                { "clientId", appRegistration.ClientId },
                { "secret", appRegistration.Secret }
            };
            // when
            var response = await client.PostAsync<OauthTokenResponse>(RequestUris.OauthToken().AddQueryParameter(query));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.Token), "Token is empty or whitespace.");
            Assert.AreEqual(3600, response.Dto.ExpiresIn, "ExpiresIn does not match.");
        }

        // 400
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Oauth_Token_Post_BadRequest()
        {
            // given
            var client = GetUnauthenticatedClient();

            var query = new Dictionary<string, object>
            {
                { "clientId", "Company.OauthClientId" },
                { "secret", "Company.OauthSecret" }
            };
            // when
            var response = await client.PostAsync<OauthErrorResponse>(RequestUris.OauthToken().AddQueryParameter(query));

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
            Assert.AreEqual("Invalid ClientId/Secret", response.Dto.Message, "Message does not match.");
        }

    }

    public class OauthErrorResponse
    {
        public string Message { get; set; }
    }
}