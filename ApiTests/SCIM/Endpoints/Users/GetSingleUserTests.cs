using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using AtCommon.ObjectFactories;

namespace ApiTests.SCIM.Endpoints.Users
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Groups")]
    public class GetSingleUserTests : BaseV1Test
    {
        //200
        [TestMethod]
        public async Task Groups_Get_SingleUser_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var addUserRequest= ScimUserFactory.GetScimUser();
            var addUserResponse = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), addUserRequest);
            
            //When
            var getUserResponse = await client.GetAsync<ScimUser>(ScimRequestUris.Users(addUserResponse.Dto.DisplayName));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getUserResponse.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(addUserResponse.Dto.Id, getUserResponse.Dto.Id, "User's Id does not match");
            Assert.AreEqual(addUserResponse.Dto.DisplayName,getUserResponse.Dto.DisplayName,"DisplayName does not match");
            Assert.AreEqual(addUserResponse.Dto.UserName, getUserResponse.Dto.UserName, "UserName does not match");
            Assert.AreEqual(addUserResponse.Dto.Name.FamilyName,getUserResponse.Dto.Name.FamilyName,"FamilyName does not match");
            Assert.AreEqual(addUserResponse.Dto.Name.GivenName,getUserResponse.Dto.Name.GivenName,"GivenName does not match");
            Assert.IsTrue(getUserResponse.Dto.Emails.Count > 0, "Member email count is '0'");
        }

        //401
        [TestMethod]
        public async Task Groups_Get_SingleUser_Unauthorized()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var addUserRequest = ScimUserFactory.GetScimUser();
            var addUserResponse = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), addUserRequest);

            var unAuthorizedClient = GetUnauthenticatedScimClient();

            //When
            var response = await unAuthorizedClient.GetAsync<ScimUser>(ScimRequestUris.Users(addUserResponse.Dto.DisplayName));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        public async Task Groups_Get_SingleUser_NotFound()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            //When
            var response = await client.GetAsync<ScimUser>(ScimRequestUris.Users("abcdedfs"));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
