using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.CampaignsV2
{
    [TestClass]
    [TestCategory("CampaignsV2")]
    public class GetTeamsTests : BaseV1Test
    {
        public static int MaxValueOf32BitInt = int.MaxValue;
        public static bool ClassInitFailed;
        public static User CompanyAdminUser => new UserConfig("CA").GetUserByDescription("user 1");
        public static CreateTeamRequest TeamRequest = CampaignFactory.GetCompanyTeam();
        public static string ExpectedTags = string.Join(", ", CampaignFactory.SearchTagList);
        public static List<int> TeamIds = CampaignFactory.TeamIdsList;

        public static CreateTeamRequest TeamBlankRequest = new CreateTeamRequest
        {
            SearchTeam = "",
            SearchWorkType = "",
            CurrentPage = 1,
            PageSize = 50,
            TeamIds = new List<int>(),
            ParentTeamId = 0,
            ExcludeTeamIds = new List<int>(),
            OrderByColumn = "",
            OrderByDirection = "",
            IsAhf = false,
            SearchTag = ""
        };

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_AllRequestData_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), TeamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");

            var lastRowOnPage = TeamRequest.CurrentPage * TeamRequest.PageSize;
            var firstRowOnPage = lastRowOnPage - (TeamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(TeamRequest.PageSize));

            //Then
            Assert.AreEqual(TeamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(TeamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.AreEqual(TeamRequest.SearchTeam, response.Dto.TeamInfo.FirstOrDefault()?.Name, "Team Name Doesn't match");
            Assert.AreEqual(TeamRequest.TeamIds.FirstOrDefault(), response.Dto.TeamInfo.FirstOrDefault()?.Id, "TeamIds doesn't match");
            Assert.AreEqual(TeamRequest.SearchWorkType, response.Dto.TeamInfo.FirstOrDefault()?.WorkType, "Work type doesn't match");
            Assert.AreEqual(ExpectedTags, response.Dto.TeamInfo.FirstOrDefault()?.TeamTags,"Team tags doesn't match");
            Assert.AreEqual(TeamRequest.IsAhf,response.Dto.TeamInfo.FirstOrDefault()?.IsAhf,"Ahf value doesn't match");
        }

        //200 
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47138
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_Different_CurrentPage_And_PageSize_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = 2;
            teamRequest.PageSize = 2;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));
            
            //Then
            Assert.AreEqual(teamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(teamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not  be< 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.AreEqual(ExpectedTags, response.Dto.TeamInfo.FirstOrDefault()?.TeamTags, "Team tags doesn't match");
        }

        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_Descending_Order_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamDescendingRequest = new CreateTeamRequest
            {
                SearchTeam = "",
                SearchWorkType = "",
                CurrentPage = 1,
                PageSize = 50,
                TeamIds = new List<int>(),
                ParentTeamId = 0,
                ExcludeTeamIds = new List<int>(),
                OrderByColumn = "Name",
                OrderByDirection = "desc",
                IsAhf = false,
                SearchTag = ""
            };

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), TeamBlankRequest);
            var expectedDescendingOrderByName = response.Dto.TeamInfo.OrderByDescending(x => x.Name).Select(x => x.Name).ToList();
            
            var response1 = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamDescendingRequest);
            var actualDescendingOrderByName = response1.Dto.TeamInfo.Select(x => x.Name).ToList();

            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode, "Response doesn't match");
            
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (TeamBlankRequest.CurrentPage * TeamBlankRequest.PageSize) - (TeamBlankRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(TeamBlankRequest.PageSize));

            //Then
            Assert.IsTrue(expectedDescendingOrderByName.SequenceEqual(actualDescendingOrderByName), "Team list doesn't match in descending order");
            Assert.AreEqual(TeamBlankRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(TeamBlankRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
        }

        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_Ascending_Order_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamAscendingRequest = new CreateTeamRequest
            {
                SearchTeam = "",
                SearchWorkType = "",
                CurrentPage = 1,
                PageSize = 50,
                TeamIds = new List<int>(),
                ParentTeamId = 0,
                ExcludeTeamIds = new List<int>(),
                OrderByColumn = "Id",
                OrderByDirection = "asc",
                IsAhf = false,
                SearchTag = ""
            };

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), TeamBlankRequest);
            var expectedAscendingOrderById = response.Dto.TeamInfo.OrderBy(x => x.Id).Select(x => x.Id).ToList();

            var response1 = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamAscendingRequest);
            var actualAscendingOrderById = response1.Dto.TeamInfo.Select(x => x.Id).ToList();

            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (TeamBlankRequest.CurrentPage * TeamBlankRequest.PageSize) - (TeamBlankRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(TeamBlankRequest.PageSize));

            //Then
            Assert.IsTrue(expectedAscendingOrderById.SequenceEqual(actualAscendingOrderById), "Order list doesn't match in Ascending order");
            Assert.AreEqual(TeamBlankRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(TeamBlankRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47138
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_SearchWorkType_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamRequest = TeamBlankRequest;
            teamRequest.SearchWorkType = SharedConstants.NewTeamWorkType;
            teamRequest.SearchTeam = "";
            teamRequest.PageSize = 1000;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));

            //Then
            Assert.AreEqual(teamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(teamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.TeamInfo.Select(x => x.WorkType).ToList().Contains(teamRequest.SearchWorkType), $"Team WorkType list doesn't contain {teamRequest.SearchWorkType}");
            Assert.IsTrue(response.Dto.TeamInfo.Select(x=>x.IsAhf).ToList().Contains(teamRequest.IsAhf),($"Ahf value doesn't match"));
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.TeamTags).ToList(), ExpectedTags, $"Team tags list doesn't contains {TeamRequest.SearchTag}");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team Names list doesn't contains{TeamRequest.SearchTeam}");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Id.ToString()).ToList(), TeamRequest.TeamIds.FirstOrDefault().ToString(), $"Team Id list doesn't contain {TeamRequest.TeamIds}");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47138
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_SearchTag_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var random = new Random();

            var teamRequest = TeamBlankRequest;
            teamRequest.SearchTag = CampaignFactory.SearchTagList.OrderBy(x => random.Next()).FirstOrDefault();
            teamRequest.PageSize = 1000;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));

            //Then
            Assert.AreEqual(teamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(teamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.TeamInfo.Select(x => x.IsAhf).ToList().Contains(teamRequest.IsAhf), ($"Ahf value doesn't match"));
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.TeamTags).ToList(), ExpectedTags, $"Team tags list doesn't contains {TeamRequest.SearchTag}");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team Names list doesn't contains{TeamRequest.SearchTeam}");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Id.ToString()).ToList(), TeamRequest.TeamIds.FirstOrDefault().ToString(), $"Team Id list doesn't contain {TeamRequest.TeamIds}");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47138
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_SearchTeam_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamRequest = TeamBlankRequest;
            teamRequest.SearchWorkType = "";
            teamRequest.CurrentPage = 1;
            teamRequest.SearchTeam = SharedConstants.RadarTeam;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));

            //Then
            Assert.AreEqual(teamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(teamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.AreEqual(TeamRequest.SearchWorkType, response.Dto.TeamInfo.FirstOrDefault()?.WorkType, "Work type doesn't match");
            Assert.AreEqual(teamRequest.SearchTeam, response.Dto.TeamInfo.FirstOrDefault()?.Name, "Team Name Doesn't match");
            Assert.AreEqual(TeamRequest.TeamIds.FirstOrDefault(), response.Dto.TeamInfo.FirstOrDefault()?.Id, "TeamIds doesn't match");
            Assert.AreEqual(ExpectedTags, response.Dto.TeamInfo.FirstOrDefault()?.TeamTags, "Team tags doesn't match");
            Assert.AreEqual(TeamRequest.IsAhf, response.Dto.TeamInfo.FirstOrDefault()?.IsAhf, "Ahf value doesn't match");
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 47138
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_Without_SearchName_WorkType_And_TeamId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = 1;
            teamRequest.PageSize = 500;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");

            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));

            //Then
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.CurrentPage > 0, "Current Page doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team name List doesn't contains { TeamRequest.SearchTeam}");
            Assert.AreEqual(TeamRequest.TeamIds.FirstOrDefault(), response.Dto.TeamInfo.FirstOrDefault(x => x.Name == TeamRequest.SearchTeam)?.Id, "Team Id doesn't match");
            Assert.AreEqual(TeamRequest.SearchWorkType, response.Dto.TeamInfo.FirstOrDefault(x => x.Name == TeamRequest.SearchTeam)?.WorkType, "Work-type doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.TeamTags).ToList(), ExpectedTags, $"Team tags list doesn't contains {TeamRequest.SearchTag}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_TeamId_ExcludeTeamId_and_DefaultValues_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = 1;
            teamRequest.PageSize = 1000;
            teamRequest.TeamIds = TeamIds.GetRange(0, 4);
            teamRequest.ExcludeTeamIds = TeamIds.GetRange(3, 1);

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");

            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (teamRequest.CurrentPage * teamRequest.PageSize) - (teamRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(teamRequest.PageSize));

            //Then
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.CurrentPage > 0, "Current Page doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page Size should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team name List doesn't contains { TeamRequest.SearchTeam}");
            Assert.That.ListNotContains(response.Dto.TeamInfo.Select(x => x.Id.ToString()).ToList(), teamRequest.ExcludeTeamIds.ToString(),$"Team Id list contains{teamRequest.ExcludeTeamIds}" );
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_With_Default_PageSize_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = 1;
            teamRequest.PageSize = 0;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");
            Assert.AreEqual(teamRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page Value doesn't match");
            Assert.AreEqual(1000, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team name List doesn't contains { TeamRequest.SearchTeam}");
            Assert.AreEqual(TeamRequest.TeamIds.FirstOrDefault(), response.Dto.TeamInfo.FirstOrDefault(x => x.Name == TeamRequest.SearchTeam)?.Id, "Team Id doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.TeamTags).ToList(), ExpectedTags, $"Team tags list doesn't contains {TeamRequest.SearchTag}");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Teams_Without_CurrentPage_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = 0;
            teamRequest.PageSize = 1000;

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page Value doesn't match");
            Assert.AreEqual(teamRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x => x.Name).ToList(), TeamRequest.SearchTeam, $"Team name List doesn't contains {TeamRequest.SearchTeam}");
            Assert.AreEqual(TeamRequest.TeamIds.FirstOrDefault(), response.Dto.TeamInfo.FirstOrDefault(x => x.Name == TeamRequest.SearchTeam)?.Id, "Team Id doesn't match");
            Assert.AreEqual(TeamRequest.SearchWorkType, response.Dto.TeamInfo.FirstOrDefault(x => x.Name == TeamRequest.SearchTeam)?.WorkType, "Work-type doesn't match");
            Assert.That.ListContains(response.Dto.TeamInfo.Select(x=>x.TeamTags).ToList(), ExpectedTags,$"Team tags list doesn't contains {TeamRequest.SearchTag}");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_Teams_With_Invalid_CompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Company Id' is not valid"
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(-1), TeamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_Teams_With_FakeCompanyID_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found"
            };

            //When
            var response = await client.PostAsync<List<string>>(RequestUris.CampaignsV2Teams(SharedConstants.FakeCompanyId), TeamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Teams_With_Invalid_CurrentPage_And_PageSize_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Current Page' must be greater than or equal to '0'.",
                "'Page Size' must be greater than or equal to '0'."
            };

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = -123;
            teamRequest.PageSize = -123;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //400 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Teams_With_CurrentPage_MaxLimit_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'PageSize' is not valid"
            };

            var teamRequest = TeamBlankRequest;
            teamRequest.CurrentPage = MaxValueOf32BitInt;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response message for Company ID doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Teams_With_PageSize_MaxLimit_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'PageSize' is not valid"
            };

            var teamRequest = TeamBlankRequest;
            teamRequest.PageSize = MaxValueOf32BitInt;
            teamRequest.CurrentPage = 1;

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(Company.Id), teamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Error message does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Teams_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PostAsync<TeamInfoResponse>(RequestUris.CampaignsV2Teams(Company.Id), TeamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin"), TestCategory("Member")]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task CampaignsV2_Get_Team_With_Invalid_CompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(-1), TeamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Teams_With_DifferentUsers_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Teams(Company.Id), TeamRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match.");
        }
    }
}
