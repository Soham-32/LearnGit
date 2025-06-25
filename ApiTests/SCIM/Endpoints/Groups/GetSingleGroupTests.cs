using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.SCIM.Endpoints.Groups
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Groups")]
    public class GetSingleGroupTests : BaseV1Test
    {
        [TestMethod]
        public async Task Scim_Get_AGroup_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var groupsResponse = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            var response = await client.GetAsync<ScimGroup.Resource>(ScimRequestUris.Groups(groupsResponse.Dto.Resources.Last().DisplayName));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Members.Count > 0, "Member detail should be returned");
        }

        [TestMethod]
        public async Task Scim_Get_AGroup_NotFound()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var response = await client.GetAsync<ScimGroup.Resource>(ScimRequestUris.Groups("abcdedfs"));

            // Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        public async Task Scim_Get_AGroup_Unauthorized()
        {
            // Given
            var client = GetAuthenticatedScimClient();
            var unAuthorizedClient = GetUnauthenticatedScimClient();

            // When
            var groupsResponse = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());
            var response = await unAuthorizedClient.GetAsync<ScimGroup.Resource>(ScimRequestUris.Groups(groupsResponse.Dto.Resources.FirstOrDefault()?.DisplayName));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
