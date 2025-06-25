using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesMembersTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_Members_Get_All_OK()
        {
            var client = await GetAuthenticatedClient();

            var companyId = Company.Id;

            var response = await client.GetAsync<IList<MemberResponse>>(
                RequestUris.CompaniesMembers(companyId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
                "Status code does not match");
            Assert.IsTrue(response.Dto.Count > 0, "List of members is empty");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Hash)), 
                "Hash is null or empty");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Uid.ToString())), 
                "Uid is null or empty");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        public async Task Companies_Members_Get_Specific_Success()
        {
            var client = await GetAuthenticatedClient();

            var team = TeamFactory.GetValidPostTeamWithMember("GetValidPostTeamWithMember_");
            var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamResponse.EnsureSuccess();
            
            var companyId = Company.Id;

            var response = await client.GetAsync<IList<MemberResponse>>(
                RequestUris.CompaniesMembers(companyId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsTrue(response.Dto.Count > 0, "List of members is empty");
            Assert.IsTrue(response.Dto.Any(dto => dto.FirstName == teamResponse.Dto.Members.First().FirstName), "First names do not match");
            Assert.IsTrue(response.Dto.Any(dto => dto.LastName == teamResponse.Dto.Members.First().LastName), "Last names do not match");
            Assert.IsTrue(response.Dto.Any(dto => dto.Email == teamResponse.Dto.Members.First().Email), 
                "Emails do not match");
            Assert.IsTrue(response.Dto.Any(dto => dto.ExternalIdentifier == teamResponse.Dto.Members.First().ExternalIdentifier), "External identifiers do not match");
            Assert.IsTrue(response.Dto.Any(dto => dto.Uid == teamResponse.Dto.Members.First().Uid), 
                "Uids do not match");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Hash)), 
                "Hash is null or empty");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_Members_Get_Unauthorized()
        {
            var companyId = Company.Id;

            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync(RequestUris.CompaniesMembers(companyId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_Members_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            const int companyId = 84;

            var response = await client.GetAsync(
                RequestUris.CompaniesMembers(companyId));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code does not match");
        }
    }
}
