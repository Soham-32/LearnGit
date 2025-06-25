using AtCommon.Api;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class GetTeamsIdsTests : BaseV1Test
    {
        public static GetAllCampaignTeamIdsRequest TeamIdsRequest = CampaignFactory.GetCompanyTeamsId();
        public static GetAllCampaignTeamIdsRequest TeamIdsBlankRequest = new GetAllCampaignTeamIdsRequest
        {
            SearchTeam = "",
            SearchWorkType = "",
            TeamIds = new List<int>(),
            ParentTeamId = 0,
            ExcludeTeamIds = new List<int>(),
            IsAhf = false,
            SearchTag = ""
        };

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_AllRequestData_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), TeamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.AreEqual(TeamIdsRequest.TeamIds.FirstOrDefault(), response.Dto.TeamIds.FirstOrDefault(), "TeamIds doesn't match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_AHF_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), TeamIdsBlankRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_AHF_ParentTeamId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamIdsRequest = TeamIdsBlankRequest;
            teamIdsRequest.ParentTeamId = 7132;

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), teamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_SearchTeam_WorkType_And_Tag_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
 
            var random = new Random();
            var teamIdsRequest = TeamIdsBlankRequest;
            teamIdsRequest.SearchTeam = SharedConstants.RadarTeam;
            teamIdsRequest.SearchWorkType = "Software Delivery";
            teamIdsRequest.SearchTag = CampaignFactory.SearchTagList.OrderBy(x => random.Next()).FirstOrDefault();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), teamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_SearchTeam_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamIdsRequest = TeamIdsBlankRequest;
            teamIdsRequest.SearchTeam = SharedConstants.RadarTeam;

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), teamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_SearchTag_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var random = new Random();
            var teamIdsRequest = TeamIdsBlankRequest;
            teamIdsRequest.SearchTag = CampaignFactory.SearchTagList.OrderBy(x => random.Next()).FirstOrDefault();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), teamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_With_SearchWorkType_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamIdsRequest = TeamIdsBlankRequest;
            teamIdsRequest.SearchWorkType = "Software Delivery";

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), teamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.That.ListContains(response.Dto.TeamIds.Select(c => c.ToString()).ToList(), TeamIdsRequest.TeamIds.First().ToString(), $"Team Ids List doesn't contain { TeamIdsRequest.TeamIds.First()}");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_TeamsIds_WithFakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2TeamsIds(SharedConstants.FakeCompanyId), TeamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code doesn't match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_TeamsIds_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), TeamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_TeamsIds_With_FakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(SharedConstants.FakeCompanyId), TeamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_TeamsIds_WithDifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<GetAllCampaignTeamIdsResponse>(RequestUris.CampaignsV2TeamsIds(Company.Id), TeamIdsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code doesn't match.");
        }
    }
}