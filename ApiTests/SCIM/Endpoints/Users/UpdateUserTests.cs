using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.SCIM.Endpoints.Users
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Users")]
    public class UpdateUserTests : BaseV1Test
    {
        //200
        [TestMethod]
        public async Task Scim_Post_UpdateUser_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            var userDto = ScimUserFactory.GetScimUser();
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            response.EnsureSuccess();

            var updateRequest = ScimUserFactory.UpdateScimUser();

            //When
            var updateResponse = await client.PutAsync<ScimUser>(ScimRequestUris.Users(response.Dto.Id), updateRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(updateRequest.DisplayName, updateResponse.Dto.DisplayName, "DisplayName does not match");
            Assert.AreEqual(updateRequest.UserName, updateResponse.Dto.UserName, "UserName does not match");
            Assert.AreEqual(updateRequest.Name.FamilyName, updateResponse.Dto.Name.FamilyName, "FamilyName does not match");
            Assert.AreEqual(updateRequest.Emails.First().Value, updateResponse.Dto.Emails.First().Value, "Email does not match");
        }

        //400
        [TestMethod]
        public async Task Scim_Post_UpdateUser_BadRequest()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            var userDto = ScimUserFactory.GetScimUser();
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            response.EnsureSuccess();

            var updateRequest = new CreateUserRequest();

            //When
            var updateResponse = await client.PutAsync<ScimUser>(ScimRequestUris.Users(response.Dto.Id), updateRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updateResponse.StatusCode, "Response Status Code does not match.");
        }

        //400
        [TestMethod]
        public async Task Scim_Post_UpdateUser_InvalidUsername_BadRequest()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            var userDto = ScimUserFactory.GetScimUser();
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            response.EnsureSuccess();

            var updateRequest = ScimUserFactory.UpdateScimUser();
            updateRequest.UserName = "abcdefg";

            //When
            var updateResponse = await client.PutAsync<ScimUser.Errors>(ScimRequestUris.Users(response.Dto.Id), updateRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, updateResponse.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("'User Name' is not a valid email address.", updateResponse.Dto.Detail, "Error messages does not match");
        }

        //401
        [TestMethod]
        public async Task Scim_Post_UpdateUser_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedScimClient();
            var updateRequest = ScimUserFactory.UpdateScimUser();

            //When
            var updateResponse = await client.PutAsync<ScimUser>(ScimRequestUris.Users(Guid.NewGuid().ToString()), updateRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, updateResponse.StatusCode, "Response Status Code does not match.");
        }

        //404
        [TestMethod]
        public async Task Scim_Post_UpdateUser_NotFound()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            const string userId = "abcdefg";
            var updateRequest = ScimUserFactory.UpdateScimUser();

            //When
            var updateResponse = await client.PutAsync<string>(ScimRequestUris.Users(userId), updateRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, updateResponse.StatusCode, "Response Status Code does not match.");
        }
    }
}

