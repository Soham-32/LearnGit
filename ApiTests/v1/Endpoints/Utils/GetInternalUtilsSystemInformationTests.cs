using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Utils
{
    [TestClass]
    [TestCategory("Utils")]
    public class GetInternalUtilsSystemInformationTests : BaseV1Test
    {
        // 200
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Internal_Utils_SystemInformation_Get_OK()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response =
                await client.GetAsync<SystemInformationResponse>(RequestUris.InternalUtilsSystemInformation());
            // then

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match.");
            Assert.AreEqual($"{User.FirstName}_{User.LastName}({User.Username})", response.Dto.PendoInformation.UserId, 
                "Pendo-UserId does not match.");
            Assert.AreEqual(User.Type.ToString(), response.Dto.PendoInformation.UserRole, "Pendo-UserRole does not match.");
            Assert.AreEqual(Company.Id, response.Dto.PendoInformation.CompanyId, "Pendo-CompanyId does not match.");
            Assert.AreEqual(User.CompanyName, response.Dto.PendoInformation.CompanyName, "Pendo-CompanyName does not match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.PendoInformation.CompanyType), "Pendo-CompanyType is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.PendoInformation.Size), "Pendo-Size is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.PendoInformation.LifeCycleStage), 
                "Pendo-LifeCycleStage is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.PendoInformation.Country), "Pendo-Country is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.PendoInformation.Industry), "Pendo-Industry is null or empty.");

            Assert.AreEqual(User.Type.ToString(), response.Dto.SplitUserInformation.UserRole, "Split-UserRole does not match.");
            Assert.AreEqual(Company.Id, response.Dto.SplitUserInformation.CompanyId, "Split-CompanyId does not match.");
            Assert.AreEqual(User.CompanyName, response.Dto.SplitUserInformation.CompanyName, "Split-CompanyName does not match.");
            Assert.IsTrue(Guid.TryParse(response.Dto.SplitUserInformation.UserId, out _), "The Split-UserId is not a valid Guid.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.CompanyType), "Split-CompanyType is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.Size), "Split-Size is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.LifeCycleStage), 
                "Split-LifeCycleStage is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.Country), "Split-Country is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.Industry), "Split-Industry is null or empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Dto.SplitUserInformation.EnvironmentKey), 
                "Pendo-EnvironmentKey is null or empty.");
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Internal_Utils_SystemInformation_Get_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response =
                await client.GetAsync(RequestUris.InternalUtilsSystemInformation());
            // then

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match.");

        }
    }
}
