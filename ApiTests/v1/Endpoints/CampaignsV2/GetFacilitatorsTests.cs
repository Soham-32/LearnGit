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
    public class GetFacilitatorsTests : BaseV1Test
    {
        public static int MaxValueOf32BitInt = int.MaxValue;
        public static User CompanyAdmin = new UserConfig("CA").GetUserByDescription("user 1");
        public static CreateFacilitatorRequest FacilitatorRequest= CampaignFactory.GetCompanyFacilitator(CompanyAdmin.FirstName);

        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Facilitators_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), FacilitatorRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = FacilitatorRequest.CurrentPage * FacilitatorRequest.PageSize;
            var firstRowOnPage = lastRowOnPage - (FacilitatorRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(FacilitatorRequest.PageSize));

            //Then
            Assert.AreEqual(FacilitatorRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(FacilitatorRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.AreEqual(lastRowOnPage, response.Dto.Paging.LastRowOnPage, "Last Row on page doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.AreEqual(CompanyAdmin.Username, response.Dto.FacilitatorInfo.FirstOrDefault()?.Email, "Facilitator Email doesn't match");
            Assert.AreEqual(CampaignFactory.FacilitatorId, response.Dto.FacilitatorInfo.FirstOrDefault()?.FacilitatorId, "Facilitator ID doesn't match");
            Assert.AreEqual(FacilitatorRequest.SearchName, response.Dto.FacilitatorInfo.FirstOrDefault()?.FirstName, "Facilitator First Name doesn't match");
            Assert.AreEqual(CompanyAdmin.LastName, response.Dto.FacilitatorInfo.FirstOrDefault()?.LastName, "Facilitator Last Name doesn't match");
            Assert.IsTrue(response.Dto.FacilitatorInfo.FirstOrDefault()!.NumberOfFacilitation == 0, "Number of facilitation count doesn't match");
        }


        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Facilitators_With_Different_CurrentPage_And_PageSize_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "",
                CurrentPage = 2,
                PageSize = 2,
                FacilitatorIds = new List<string>()
            };

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var lastRowOnPage = response.Dto.Paging.LastRowOnPage; 
            var firstRowOnPage = (facilitatorRequest.CurrentPage * facilitatorRequest.PageSize) - (facilitatorRequest.PageSize - 1); 
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(facilitatorRequest.PageSize));

            //Then
            Assert.AreEqual(facilitatorRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.AreEqual(facilitatorRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
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
        public async Task CampaignsV2_Get_Facilitators_Without_FacilitatorId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "test001",
                CurrentPage = 1,
                PageSize = 500,
                FacilitatorIds = new List<string>()
            };

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (facilitatorRequest.CurrentPage * facilitatorRequest.PageSize) - (facilitatorRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(facilitatorRequest.PageSize));

            //Then
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.AreEqual(1, response.Dto.Paging.CurrentPage, "Current Page doesn't match");
            Assert.IsTrue(response.Dto.Paging.PageSize > 0, "Page size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page Size should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.FirstName == facilitatorRequest.SearchName), "Facilitator First Name doesn't Match");
            Assert.IsTrue(response.Dto.FacilitatorInfo.FirstOrDefault()!.NumberOfFacilitation == 0,
                "Number of facilitation count doesn't match");
        }

        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Facilitators_Without_SearchName_And_FacilitatorId_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "",
                CurrentPage = 1,
                PageSize = 1000,
                FacilitatorIds = new List<string>()
            };
             
            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");

            var rowCount = response.Dto.Paging.RowCount;
            var lastRowOnPage = response.Dto.Paging.LastRowOnPage;
            var firstRowOnPage = (facilitatorRequest.CurrentPage * facilitatorRequest.PageSize) - (facilitatorRequest.PageSize - 1);
            var pageCount = Math.Ceiling(Convert.ToDouble(response.Dto.Paging.RowCount) / Convert.ToDouble(facilitatorRequest.PageSize));

            //Then
            Assert.AreEqual(lastRowOnPage, rowCount, "Last Row on page and Total rowCount doesn't match");
            Assert.AreEqual(firstRowOnPage, response.Dto.Paging.FirstRowOnPage, "First Row on Page doesn't match");
            Assert.AreEqual(pageCount, response.Dto.Paging.PageCount, "Page Count doesn't match");
            Assert.AreEqual(facilitatorRequest.CurrentPage,response.Dto.Paging.CurrentPage,"Current Page Value doesn't match");
            Assert.AreEqual(facilitatorRequest.PageSize, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not be < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not be < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.That.ListContains(response.Dto.FacilitatorInfo.Select(x => x.FacilitatorId).ToList(), CampaignFactory.FacilitatorId, $"Facilitator ID List doesn't contains {CampaignFactory.FacilitatorId}");
            Assert.That.ListContains(response.Dto.FacilitatorInfo.Select(x => x.Email).ToList(), CompanyAdmin.Username, $"Email List doesn't contains {CompanyAdmin.Username}");
            Assert.That.ListContains(response.Dto.FacilitatorInfo.Select(x => x.FirstName).ToList(), CompanyAdmin.FirstName, $"First Name List doesn't contains {CompanyAdmin.FirstName}");
            Assert.That.ListContains(response.Dto.FacilitatorInfo.Select(x => x.LastName).ToList(),
                CompanyAdmin.LastName, $"Last Name List doesn't contains {CompanyAdmin.LastName}");
        }

        //200  
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Facilitators_Without_PageSize_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = CompanyAdmin.FirstName,
                CurrentPage = 1,
                FacilitatorIds = new List<string>{ CampaignFactory.FacilitatorId }
            };

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");
            Assert.AreEqual(facilitatorRequest.CurrentPage, response.Dto.Paging.CurrentPage, "Current Page Value doesn't match");
            Assert.AreEqual(1000, response.Dto.Paging.PageSize, "Page Size doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.FacilitatorId == CampaignFactory.FacilitatorId), $"Facilitator ID List doesn't contains {CampaignFactory.FacilitatorId}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.Email == CompanyAdmin.Username), $"Email List doesn't contains {CompanyAdmin.Username}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.FirstName == CompanyAdmin.FirstName), $"First Name List doesn't contains {CompanyAdmin.FirstName}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.LastName == CompanyAdmin.LastName), $"Last Name List doesn't contains {CompanyAdmin.LastName}");
        }

        //200 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("BLAdmin")]
        public async Task CampaignsV2_Get_Facilitators_Without_CurrentPage_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = CompanyAdmin.FirstName,
                PageSize = 1, 
                FacilitatorIds = new List<string>() { CampaignFactory.FacilitatorId }
            };

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response doesn't match");
            Assert.AreEqual(facilitatorRequest.PageSize, response.Dto.Paging.PageSize, "Page Size Value doesn't match");
            Assert.IsTrue(response.Dto.Paging.CurrentPage > 0, "Current Page Value doesn't match");
            Assert.IsTrue(response.Dto.Paging.FirstRowOnPage > 0, "First Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.LastRowOnPage > 0, "Last Row On Page should not < 0");
            Assert.IsTrue(response.Dto.Paging.RowCount > 0, "Row count should not < 0");
            Assert.IsTrue(response.Dto.Paging.PageCount > 0, "Page count doesn't match");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.FacilitatorId == CampaignFactory.FacilitatorId),$"Facilitator ID List doesn't contains {CampaignFactory.FacilitatorId}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.Email == CompanyAdmin.Username),$"Email List doesn't contains {CompanyAdmin.Username}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.FirstName == CompanyAdmin.FirstName),$"First Name List doesn't contains {CompanyAdmin.FirstName}");
            Assert.IsTrue(response.Dto.FacilitatorInfo.All(x => x.LastName == CompanyAdmin.LastName), $"Last Name List doesn't contains {CompanyAdmin.LastName}");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_Facilitators_With_Invalid_CompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Company Id' is not valid",
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(-1), FacilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match.");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response message for Company Id doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task CampaignsV2_Get_Facilitators_With_FakeCompanyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "CompanyId is not found",
            };

            //When
            var response =
                await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(SharedConstants.FakeCompanyId), FacilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitators_With_Invalid_CurrentPage_And_PageSize_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Current Page' must be greater than or equal to '0'.",
                "'Page Size' must be greater than or equal to '0'."
            };

            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "",
                CurrentPage = -123,
                PageSize = -123,
                FacilitatorIds = new List<string>() 
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            //Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error list does not match");
        }

        //400 
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitators_With_CurrentPage_MaxLimit_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var errorResponseList = new List<string>
            {
                "'PageSize' is not valid",
            };

            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "",
                CurrentPage = MaxValueOf32BitInt,
                PageSize = 1,
                FacilitatorIds = new List<string>()
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response error message doesn't match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitators_With_PageSize_MaxLimit_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'PageSize' is not valid",
            };

            var facilitatorRequest = new CreateFacilitatorRequest
            {
                SearchName = "",
                CurrentPage = 1,
                PageSize = MaxValueOf32BitInt,
                FacilitatorIds = new List<string>() 
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(Company.Id), facilitatorRequest);

            //Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
            Assert.That.ListsAreEqual(errorResponseList, response.Dto.ToList(), "Response Error list does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("BLAdmin"),TestCategory("OrgLeader"),TestCategory("TeamAdmin"),TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitators_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.PostAsync<FacilitatorInfoResponse>(RequestUris.CampaignsV2Facilitators(Company.Id), FacilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitators_Invalid_CompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(-1), FacilitatorRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task CampaignsV2_Get_Facilitator_With_DifferentUser_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.CampaignsV2Facilitators(Company.Id), FacilitatorRequest);

            //Then  
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match.");
        }
    }
}
