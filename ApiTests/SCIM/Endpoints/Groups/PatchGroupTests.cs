using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.SCIM.Endpoints.Groups
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Groups")]
    public class PatchGroupTests : BaseV1Test
    {
        [TestMethod]
        public async Task Scim_Patch_Group_AddUser_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var groupsResponse = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            groupsResponse.EnsureSuccess();
            var userDto = ScimUserFactory.GetScimUser();
            var addUserResponse = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            addUserResponse.EnsureSuccess();
            var pathGroupRequest = ScimGroupFactory.GetScimPatchGroupAddUser(addUserResponse.Dto.UserName);
            var response = await client.PatchAsync(ScimRequestUris.Groups(groupsResponse.Dto.Resources.Last().DisplayName), pathGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Patch_Group_RemoveUser_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var groupsResponse = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            groupsResponse.EnsureSuccess();
            var userDto = ScimUserFactory.GetScimUser();
            var addUserResponse = await client.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            addUserResponse.EnsureSuccess();
            var pathGroupRequest = ScimGroupFactory.GetScimPatchGroupAddUser(addUserResponse.Dto.UserName);
            var groupId = groupsResponse.Dto.Resources.Last().DisplayName;
            var addUserToGroupResponse = await client.PatchAsync(ScimRequestUris.Groups(groupId), pathGroupRequest);
            addUserToGroupResponse.EnsureSuccessStatusCode();
            var removeUserFromGroupRequest = ScimGroupFactory.GetScimPatchGroupRemoveUser(addUserResponse.Dto.UserName);

            var response = await client.PatchAsync(ScimRequestUris.Groups(groupId), removeUserFromGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Patch_Group_AddUser_Unauthorized()
        {
            // Given
            var authenticatedClient = GetAuthenticatedScimClient();
            var unauthenticatedClient = GetUnauthenticatedScimClient();

            // When
            var groupsResponse = await authenticatedClient.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            var userDto = ScimUserFactory.GetScimUser();
            var addUserResponse = await authenticatedClient.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            var pathGroupRequest = ScimGroupFactory.GetScimPatchGroupAddUser(addUserResponse.Dto.UserName);
            var response = await unauthenticatedClient.PatchAsync(ScimRequestUris.Groups(groupsResponse.Dto.Resources.Last().DisplayName), pathGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Patch_Group_RemoveUser_Unauthorized()
        {
            // Given
            var authenticatedClient = GetAuthenticatedScimClient();
            var unauthenticatedClient = GetUnauthenticatedScimClient();

            // When
            var groupsResponse = await authenticatedClient.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            groupsResponse.EnsureSuccess();
            var userDto = ScimUserFactory.GetScimUser();
            var addUserResponse = await authenticatedClient.PostAsync<ScimUser>(ScimRequestUris.Users(), userDto);
            addUserResponse.EnsureSuccess();
            var pathGroupRequest = ScimGroupFactory.GetScimPatchGroupAddUser(addUserResponse.Dto.UserName);
            var groupId = groupsResponse.Dto.Resources.Last().DisplayName;
            var addUserToGroupResponse = await authenticatedClient.PatchAsync(ScimRequestUris.Groups(groupId), pathGroupRequest);
            addUserToGroupResponse.EnsureSuccessStatusCode();
            var removeUserFromGroupRequest = ScimGroupFactory.GetScimPatchGroupRemoveUser(addUserResponse.Dto.UserName);

            var response = await unauthenticatedClient.PatchAsync(ScimRequestUris.Groups(groupId), removeUserFromGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Patch_Group_InternalServerError()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var groupsResponse = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            var pathGroupRequest = new PatchGroupRequest();
            var response = await client.PatchAsync(ScimRequestUris.Groups(groupsResponse.Dto.Resources.Last().DisplayName), pathGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Patch_Group_Forbidden()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var pathGroupRequest = new PatchGroupRequest();
            var response = await client.PatchAsync(ScimRequestUris.Groups("abcded"), pathGroupRequest);

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
