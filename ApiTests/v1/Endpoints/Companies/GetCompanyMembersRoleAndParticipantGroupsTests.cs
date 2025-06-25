using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompanyMembersRoleAndParticipantGroupsTests : BaseV1Test
    {
        private static TeamResponse _teamResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private Dictionary<string, object> Query = new Dictionary<string, object>
        {
            { "companyId", Company.Id },
            {"teamId",_teamResponse.TeamId}
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupApi = new SetupTeardownApi(TestEnvironment);
            _teamResponse = setupApi.GetTeamResponse(SharedConstants.UpdateTeam, user);
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_GET_MembersRolesAndParticipantGroups_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            var listOfRoleTags = new List<string>()
            {
                  "Agile Coach",
                  "Architect",
                  "Business Analyst",
                  "Chief Product Owner",
                  "Designer",
                  "Developer",
                  "Product Owner",
                  "QA Tester",
                  "Scrum Master",
                  "Subject Matter Expert (SME)/UAT",
                  "Technical Analyst",
                  "Technical Lead"
            };
            var listOfParticipantGroupTags = new List<string>()
            {
                  "Collocated",
                  "Contractor",
                  "Decade or More",
                  "Distributed",
                  "FTE",
                  "FTE",
                  "Leadership Team",
                  "Multinational",
                  "Support",
                  "Technical",
                  "Technical"
            };
            Query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                {"teamId",9765}// Passing this as hard coded value because of a bug. API is not fetching TeamID.
            };

            //when
            var response = await client.GetAsync<IList<RoleResponse>>(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(listOfParticipantGroupTags, response.Dto.First(a => a.Key == "Participant Group").Tags.Select(a => a.Name).ToList(), "Participant Group tags does not match");
            Assert.That.ListsAreEqual(listOfRoleTags, response.Dto.First(a => a.Key == "Role").Tags.Select(a => a.Name).ToList(), "Role tags does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_GET_MembersRolesAndParticipantGroups_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();
            Query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamUid", new Guid() }
            };

            //when
            var response = await client.GetAsync(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_GET_MembersRolesAndParticipantGroups_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_GET_MembersRolesAndParticipantGroups_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            Query = new Dictionary<string, object>
            {
                { "companyId", SharedConstants.FakeCompanyId },
                { "teamUid", _teamResponse.Uid }
            };

            //when
            var response = await client.GetAsync(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_GET_MembersRolesAndParticipantGroups_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();
            Query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamUid", Guid.NewGuid() }
            };

            //when
            var response = await client.GetAsync(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status code does not match");
        }
    }
}