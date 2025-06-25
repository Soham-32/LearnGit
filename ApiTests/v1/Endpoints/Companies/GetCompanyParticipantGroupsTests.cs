using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompanyParticipantGroupsTests : BaseV1Test
    {
        private static TeamResponse _teamResponse;
        private Dictionary<string, object> Query = new Dictionary<string, object>
        {
            { "companyId", Company.Id },
            {"teamUid",_teamResponse.Uid}
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _teamResponse = setup.GetTeamResponse(SharedConstants.Team);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_ParticipantGroups_GET_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //when
            var response = await client.GetAsync<CompanyTeamMemberTagResponse>(RequestUris.CompaniesParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes are not the same");
            Assert.AreEqual("Participant Group", response.Dto.Name, "Name should be 'Participant Group'");
            Assert.IsTrue(response.Dto.CompanyTeamMemberTags.All(i => i.Id > 0), "Tag id must be greater than 0");
            Assert.IsTrue(response.Dto.CompanyTeamMemberTags.All(i => !string.IsNullOrEmpty(i.Name)), "Name of tag cannot be null or empty");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_ParticipantGroups_GET_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync(RequestUris.CompaniesParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes are not the same");
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Companies_ParticipantGroups_GET_InvalidCompany_Forbidden()
        
        {
            //given
            var client = await GetAuthenticatedClient();

            Query = new Dictionary<string, object>
            {
                { "companyId", -1 }
            };
            //when
            var response = await client.GetAsync(RequestUris.CompaniesParticipantGroups().AddQueryParameter(Query));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes are not the same");
        }
    }
}