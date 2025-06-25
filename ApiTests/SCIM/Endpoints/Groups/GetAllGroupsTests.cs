using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Scim;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.SCIM.Endpoints.Groups
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Groups")]
    public class GetAllGroupsTests : BaseV1Test
    {
        [TestMethod]
        public async Task Scim_Get_AllGroups_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var response = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(0, response.Dto.StartIndex, "Start Index should be 0 by default");
            Assert.AreEqual(10, response.Dto.ItemsPerPage, "Item Per Page should be 10 by default");
            Assert.IsTrue(response.Dto.Resources.Count > 0, "Group detail should be returned");
        }

        [TestMethod]
        public async Task Scim_Get_AllGroups_With_StartIndex_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var startIndex = 5;
            var response = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups().AddQueryParameter("StartIndex", startIndex));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(startIndex, response.Dto.StartIndex, "Start Index should be 0 by default");
        }

        [TestMethod]
        public async Task Scim_Get_AllGroups_With_Count_Success()
        {
            // Given
            var client = GetAuthenticatedScimClient();

            // When
            var count = 6;
            var response = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups().AddQueryParameter("Count", count));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(count, response.Dto.ItemsPerPage, $"Start Index should be {count}");
        }

        [TestMethod]
        public async Task Scim_Get_AllGroups_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedScimClient();

            // When
            var response = await client.GetAsync<ScimGroup>(ScimRequestUris.Groups());

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
