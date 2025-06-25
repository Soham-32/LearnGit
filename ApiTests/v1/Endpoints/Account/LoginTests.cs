using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Account;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Account
{
    [TestClass]
    [TestCategory("Accounts"), TestCategory("Public")]
    public class LoginTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Login_Post_Success()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var creds = new LoginDto
            {
                Email = User.Username,
                Password = User.Password
            };

            // act
            var response = await client.PostAsync<string>(RequestUris.Login(), creds);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsNotNull(response.Dto);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Login_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var creds = new LoginDto
            {
                Email = "asdf",
                Password = "asdf"
            };

            // act
            var response = await client.PostAsync<string>(RequestUris.Login(), creds);

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Contains("INVALID_LOGIN_ATTEMPT"));
        }
    }
}