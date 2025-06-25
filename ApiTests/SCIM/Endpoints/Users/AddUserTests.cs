using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.SCIM.Endpoints.Users
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Users")]
    public class AddUserTests : BaseV1Test
    {
        //201
        [TestMethod]
        public async Task Scim_Post_AddUser_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var userDto = ScimUserFactory.GetScimUser();

            //When
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status Code does not match.");
        }

        //201
        [TestMethod]
        public async Task Scim_Post_AddEnterpriseUser_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var userDto = ScimUserFactory.GetScimEnterpriseUser();

            //When
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Response Status Code does not match.");
        }

        //400
        [TestMethod]
        public async Task Scim_Post_AddUser_InvalidUsername_BadRequest()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var userDto = ScimUserFactory.GetScimUser();
            userDto.UserName = "abcdefg";
            userDto.DisplayName = "abcdefg";

            //When
            var response = await client.PostAsync<ScimUser.Errors>(ScimRequestUris.Users(), userDto);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual("'User Name' is not a valid email address.,User Name, Display Name, and Email should match.", response.Dto.Detail, "Error messages does not match");
        }

        //401
        [TestMethod]
        public async Task Scim_Post_AddUser_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedScimClient();
            var userDto = ScimUserFactory.GetScimUser();

            //When
            var response = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
