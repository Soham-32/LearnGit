using System;
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
    public class AddExternalLinkTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_ExternalLink_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var boLinkBody = BusinessOutcomesFactory.GetBusinessOutcomeLinkRequest(Guid.NewGuid());

            // act
            var response = await client.PostAsync(RequestUris.BusinessOutcomeLinkPost(), boLinkBody.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_ExternalLink_Post_Created()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);
            var boResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse.EnsureSuccess();
            var boId = boResponse.Dto.KeyResults[0].BusinessOutcomeId;

            var boLinkBody = BusinessOutcomesFactory.GetBusinessOutcomeLinkRequest(boId);

            // act
            var response = await client.PostAsync<BusinessOutcomeLinkResponse>(RequestUris.BusinessOutcomeLinkPost(), boLinkBody);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, boResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(boLinkBody.Title, response.Dto.Title, "Title doesn't match");
            Assert.AreEqual(boLinkBody.ExternalUrl, response.Dto.ExternalLink, "ExternalLink doesn't match");
            Assert.AreEqual(boId, response.Dto.BusinessOutcomeUId, "BusinessOutcomeUId doesn't match");
            Assert.AreEqual(boLinkBody.LinkType, response.Dto.LinkType, "LinkType doesn't match");
        }
    }
}