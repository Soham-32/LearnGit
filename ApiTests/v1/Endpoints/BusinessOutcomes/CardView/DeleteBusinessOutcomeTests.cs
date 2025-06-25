using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class DeleteBusinessOutcomeTests : BaseV1Test
    {
        //200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id:53151 
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Delete_Ok()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);

            //Creating a Business Outcome
            var boResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse.EnsureSuccess();
            var businessOutcomeId = boResponse.Dto.KeyResults[0].BusinessOutcomeId;

            // act
            var response = await client.PostAsync(RequestUris.BusinessOutcomeDelete(businessOutcomeId, true), null);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id:53151 
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Delete_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.PostAsync(RequestUris.BusinessOutcomeDelete(Guid.Empty, true), null);

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code doesn't match");
        }
    }
}