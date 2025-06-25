using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Users;
using AtCommon.Utilities;

namespace ApiTests.v1.Endpoints.Users
{
    [TestClass]
    [TestCategory("Users")]
    public class GetIsSiteAdminTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Users_SiteAdmin_Get_Success_Expected_False()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var body = new GetIsSiteAdminRequest
            {
                Email = CSharpHelpers.Random8Number() + "test@mail.com"
            };

            var companyResponse = await client.PostAsync<bool>(RequestUris.UsersIsSiteAdmin(), body) ;

            //assert
            Assert.AreEqual(HttpStatusCode.OK, companyResponse.StatusCode, "Response Status Code does not match.");
            Assert.IsFalse(companyResponse.Dto, "Dto response doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Users_SiteAdmin_Get_Success_Expected_True()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var email = User.Username;
            if (User.IsPartnerAdmin())
            {
                var siteAdminUserConfig = new UserConfig("SA");
                var siteAdminUser = siteAdminUserConfig.GetUserByDescription("user 1");
                email = siteAdminUser.Username;
            }

            var body = new GetIsSiteAdminRequest
            {
                Email = email
            };

            var companyResponse = await client.PostAsync<bool>(RequestUris.UsersIsSiteAdmin(), body);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, companyResponse.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(companyResponse.Dto, "Dto response doesn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Users_SiteAdmin_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            //act
            var body = new GetIsSiteAdminRequest
            {
                Email = User.Username
            };
            var companyResponse = await client.PostAsync(RequestUris.UsersIsSiteAdmin(), body.ToStringContent());

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, companyResponse.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"),TestCategory("TeamAdmin")]
        public async Task Users_SiteAdmin_Get_Forbidden()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            
            //act
            var body = new GetIsSiteAdminRequest
            {
                Email = User.Username
            };
            var companyResponse = await client.PostAsync(RequestUris.UsersIsSiteAdmin(), body.ToStringContent());

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, companyResponse.StatusCode, "Response Status Code does not match.");
        }
    }
}
