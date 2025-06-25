using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class SortOrderTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_SortOrder_Post_Unauthorized()
        {
            // arrange
            var authenticatedClient =await GetAuthenticatedClient();
            var nonAuthenticatedClient = GetUnauthenticatedClient();

            //Creating two business outcome items
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);
            var boResponse1 = await authenticatedClient.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse1.EnsureSuccess();
            bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);
            var boResponse2 = await authenticatedClient.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse2.EnsureSuccess();
            var sortOrderBody = BusinessOutcomesFactory.GetValidBusinessOutcomeSortOrderBody(boResponse1.Dto, boResponse2.Dto);

            // act
            var boResponse = await nonAuthenticatedClient.PostAsync(RequestUris.BusinessOutcomeUpdateSortOrder, sortOrderBody.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, boResponse.StatusCode,"Status code doesn't match");
        }


        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_SortOrder_Post_Ok()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            //Creating two business outcome items
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);
            var boResponse1 = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse1.EnsureSuccess();
            bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);
            var boResponse2 = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse2.EnsureSuccess();

            var sortOrderBody = BusinessOutcomesFactory.GetValidBusinessOutcomeSortOrderBody(boResponse1.Dto, boResponse2.Dto);

            // act
            var sortOrderResponse = await client.PostAsync<List<BusinessOutcomeSortOrderRequestResponse>>(RequestUris.BusinessOutcomeUpdateSortOrder, sortOrderBody);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, sortOrderResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(sortOrderBody.First(item=>item.BusinessOutcomeUid.Equals(boResponse1.Dto.Uid)).SortOrder, sortOrderResponse.Dto.First(item => item.BusinessOutcomeUid.Equals(boResponse1.Dto.Uid)).SortOrder, "SortOrder doesn't match");
            Assert.AreEqual(sortOrderBody.First(item => item.BusinessOutcomeUid.Equals(boResponse2.Dto.Uid)).SortOrder, sortOrderResponse.Dto.First(item => item.BusinessOutcomeUid.Equals(boResponse2.Dto.Uid)).SortOrder, "SortOrder doesn't match");
        }
    }
}