using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class GetCompaniesSuggestedReviewersTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_SuggestedReviewers_Get_OK()
        {
            var client = await GetAuthenticatedClient();

            ////get company id
            var companyId = Company.Id;

            var memberResponse = await client.GetAsync<IList<MemberResponse>>(
                RequestUris.CompaniesMembers(companyId));
            memberResponse.EnsureSuccess();
            var member = memberResponse.Dto.FirstOrDefault();
            if (member == null) { throw new Exception("No members were returned in the response");}

            //act 
            var response = await client.GetAsync<IList<SuggestedReviewerResponse>>(
                RequestUris.CompaniesSuggestedReviewers(companyId, member.Uid));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
                "Status code does not match");
            Assert.IsTrue(response.Dto.All(dto => dto.CompanyId == companyId), 
                "CompanyId does not match");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Id)), 
                "Id is null or empty");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.FirstName)), 
                "FirstName is null or empty");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.LastName)), 
                "LastName is null or empty");
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.Email)), 
                "Email is null or empty");
            Assert.IsTrue(response.Dto.All(dto => dto.ReviewerType.Count > 0), 
                "Reviewer Type is empty");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_SuggestedReviewers_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();
            var memberId = new Guid();

            //act 
            var response = await client.GetAsync<IList<SuggestedReviewerResponse>>(
                RequestUris.CompaniesSuggestedReviewers(Company.Id, memberId));

            //assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_Get_SuggestedReviewers_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            const int companyId = 2;
            var memberId = Guid.NewGuid();
            
            //act 
            var response = await client.GetAsync<IList<SuggestedReviewerResponse>>(
                RequestUris.CompaniesSuggestedReviewers(companyId, memberId));

            //assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task Companies_SuggestedReviewers_Get_NotFound()
        {
            var client = await GetAuthenticatedClient();

            //get company id
            var companyId = Company.Id;

            var memberId = new Guid();

            //act 
            var response = await client.GetAsync(
                RequestUris.CompaniesSuggestedReviewers(companyId, memberId));

            //assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
                "Status code does not match");
        }
    }
}
