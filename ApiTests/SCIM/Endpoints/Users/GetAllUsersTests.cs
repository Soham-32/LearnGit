using System.Collections.Generic;
using System.Linq;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Dtos.Scim;
using AtCommon.Utilities;

namespace ApiTests.SCIM.Endpoints.Users
{
    [TestClass]
    [TestCategory("Scim"), TestCategory("Users")]
    public class GetAllUsersTests : BaseV1Test
    {
        public static int StartIndex = 5;
        public static int Count = 6;

        //200
        [TestMethod]
        public async Task Scim_Get_AllUsers_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users());

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(0, response.Dto.StartIndex, "Start Index should be 0 by default");
            Assert.AreEqual(10, response.Dto.ItemsPerPage, "Item Per Page should be 10 by default");
            Assert.IsTrue(response.Dto.Resources.Count > 0, "Group detail should be returned");
        }

        //200
        [TestMethod]
        public async Task Scim_Get_AllUsers_With_StartIndex_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users().AddQueryParameter("StartIndex", StartIndex));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(StartIndex, response.Dto.StartIndex, "Start Index should be 0 by default");
        }

        //200
        [TestMethod]
        public async Task Scim_Get_AllUsers_With_Count_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users().AddQueryParameter("Count", Count));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(Count, response.Dto.ItemsPerPage, $"{Count} does not match");
            Assert.AreEqual(Count, response.Dto.Resources.Count, $"{Count} should match with {response.Dto.Resources.Count}");
        }

        //200
        [TestMethod]
        public async Task Scim_Get_AllUsers_With_CountAndStartIndex_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var request = new Dictionary<string, object>
            {
                {"StartIndex", StartIndex},
                {"Count", Count }
            };

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users().AddQueryParameter(request));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreEqual(StartIndex, response.Dto.StartIndex, $"{StartIndex} does not match");
            Assert.AreEqual(Count, response.Dto.ItemsPerPage, $"{Count} does not match");
            Assert.AreEqual(Count, response.Dto.Resources.Count,$"{Count} should match with {response.Dto.Resources.Count}");
        }

        //200
        [TestMethod]
        public async Task Scim_Get_AllUsers_With_AttributesAndFilter_Success()
        {
            //Given
            var client = GetAuthenticatedScimClient();
            var request = new Dictionary<string, object>
            {
                {"attributes", "photos"},
                {"filter", "type = Avatar" }
            };

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users().AddQueryParameter(request));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.AreNotEqual(response.Dto.Resources.Select(a => a.Photos.Count), 0, "Counts of photos are '0'");
            Assert.That.ListContains(response.Dto.Resources.Select(b => b.Photos.First().Type).ToList(), "Avatar", "Filter value does not match");
        }

        //401
        [TestMethod]
        public async Task Scim_GET_AllUsers_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedScimClient();

            //When
            var response = await client.GetAsync<ScimAllUser>(ScimRequestUris.Users());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }
    }
}
