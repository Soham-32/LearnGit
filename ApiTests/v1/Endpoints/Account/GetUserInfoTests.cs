using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Account;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Account
{
    [TestClass]
    [TestCategory("Accounts")]
    public class GetUserInfoTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Account_GetUserInfo_Get_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<UserInfoResponse>(RequestUris.AccountGetUserInfo());

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Id.CompareTo(new Guid()) != 0, "Id is invalid");
            Assert.AreEqual(User.FirstName, response.Dto.FirstName, "FirstName doesn't match.");
            Assert.AreEqual(User.LastName, response.Dto.LastName, "LastName doesn't match.");
            Assert.AreEqual(User.Username, response.Dto.Email, "Email doesn't match.");
            Assert.AreEqual(User.Type.ToString(), response.Dto.Role.Replace(" ", ""),
                "Role doesn't match.");
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) // active company for a SA/PA could be any company
            {
                Assert.AreEqual(Company.Id, response.Dto.ActiveCompanyId, "ActiveCompanyId doesn't match."); 
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Account_GetUserInfo_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.AccountGetUserInfo());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
            
        }
    }
}
