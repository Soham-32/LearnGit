using System.Collections.Generic;
using System.Linq;
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
    public class GetOauthAppRegistrationsTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("Member"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Oauth_AppRegistrations_Get_OK()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<List<AppRegistrationResponse>>(RequestUris.OauthAppRegistrations(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");

            if (User.IsCompanyAdmin() || User.IsPartnerAdmin() || User.IsSiteAdmin())
            {
                Assert.IsTrue(response.Dto.Any(), "The response is empty"); 
            }
            foreach (var appRegistration in response.Dto)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(appRegistration.AppName), 
                    "AppName is invalid");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(appRegistration.ClientId), 
                    $"ClientId is invalid for <{appRegistration.AppName}>");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(appRegistration.CreatedBy), 
                    $"CreatedBy is invalid for <{appRegistration.AppName}>");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(appRegistration.Access), 
                    $"Access is invalid for <{appRegistration.AppName}>");
                Assert.AreEqual(Company.Id, appRegistration.CompanyId, 
                    $"CompanyId does not match for <{appRegistration.AppName}>");
            }
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Oauth_AppRegistrations_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync<List<AppRegistrationResponse>>(RequestUris.OauthAppRegistrations(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("Member"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("PartnerAdmin")]
        public async Task Oauth_AppRegistrations_Get_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.OauthAppRegistrations(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
            
        }

    }
}