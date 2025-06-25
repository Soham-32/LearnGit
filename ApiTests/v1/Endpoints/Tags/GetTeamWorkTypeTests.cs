using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Tags;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Tags
{
    [TestClass]
    [TestCategory("Tags")]
    public class GetTeamWorkTypeTests : BaseV1Test
    {
        private static List<TeamProfileResponse> _teams;
        public static bool ClassInitFailed;
        public static User CompanyAdminUser => new UserConfig("CA").GetUserByDescription("user 1");
        private static string _workType;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {

            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                _teams = setupApi.GetTeamProfileResponse(SharedConstants.RadarTeam, CompanyAdminUser).ToList();
                _workType = _teams.First().TeamTags.First(x => x.Category == "Work Type").Tags.First();
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        public async Task Tags_Get_TeamWorkTypes_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response =
                await client.GetAsync<WorkTypesResponse>(RequestUris.TagsTeamWorkTypes().AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");
            Assert.That.ListContains(response.Dto.WorkTypes.Select(x => x.Text).ToList(), _workType, $"List doesn't contain {_workType}");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Tags_Get_TeamWorkTypes_Invalid_CompanyID_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Company Id' is not valid"
            };

            //When
            var response =
                await client.GetAsync<IList<string>>(RequestUris.TagsTeamWorkTypes().AddQueryParameter("companyId", 0));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response doesn't match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Tags_Get_TeamWorkTypes_With_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response =
                await client.GetAsync<IList<string>>(RequestUris.TagsTeamWorkTypes().AddQueryParameter("companyId", SharedConstants.FakeCompanyId));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Tags_Get_TeamWorkTypes_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response =
                await client.GetAsync<WorkTypesResponse>(RequestUris.TagsTeamWorkTypes().AddQueryParameter("companyId", Company.Id));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response doesn't match");
        }

        //403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Tags_Get_TeamWorkTypes_With_Invalid_CompanyID_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response =
                await client.GetAsync<string>(RequestUris.TagsTeamWorkTypes().AddQueryParameter("companyId", SharedConstants.FakeCompanyId));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response doesn't match");
        }

    }
}