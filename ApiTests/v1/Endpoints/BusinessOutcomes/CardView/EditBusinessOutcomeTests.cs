using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class EditBusinessOutcomeTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Put_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPutBusinessOutcome(Company, Guid.Empty, Guid.Empty);

            // act
            var boResponse = await client.PutAsync(RequestUris.BusinessOutcomeUpdate(), bo.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, boResponse.StatusCode);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Put_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPutBusinessOutcome(Company, Guid.Empty, Guid.Empty);

            // act
            var boResponse = await client.PutAsync(RequestUris.BusinessOutcomeUpdate(), bo.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, boResponse.StatusCode, "Status Code doesn't matched");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Put_Ok()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);

            //Creating a Business Outcome
            var boResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boResponse.EnsureSuccess();
            var businessOutcomeId = boResponse.Dto.KeyResults[0].BusinessOutcomeId;
            var keyResultId = boResponse.Dto.KeyResults[0].Uid;

            // act
            bo = BusinessOutcomesFactory.GetValidPutBusinessOutcome(Company, businessOutcomeId, keyResultId);
            boResponse = await client.PutAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomeUpdate(), bo);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, boResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(bo.Title, boResponse.Dto.Title, "Title doesn't match");
            Assert.AreEqual(bo.Description, boResponse.Dto.Description, "Description doesn't match");
            Assert.AreEqual(bo.OverallProgress, boResponse.Dto.OverallProgress, "OverallProgress doesn't match");
            Assert.AreEqual(bo.CardColor, boResponse.Dto.CardColor, "CardColor doesn't match");
            Assert.AreEqual(User.FullName, boResponse.Dto.Owner, "Owner doesn't match");
            Assert.AreEqual(bo.TeamId, boResponse.Dto.TeamId, "TeamId doesn't match");
            Assert.AreEqual(bo.CompanyId, boResponse.Dto.CompanyId, "CompanyId doesn't match");
            Assert.AreEqual(bo.SwimlaneType, boResponse.Dto.SwimlaneType, "SwimlaneType doesn't match");
            Assert.AreEqual(bo.IsDeleted, boResponse.Dto.IsDeleted, "IsDeleted doesn't match");
            Assert.AreEqual(0, boResponse.Dto.Tags.Count, "Tags count doesn't match");
            Assert.AreEqual(0, boResponse.Dto.Comments.Count, "Comments count doesn't match");
            Assert.AreEqual(bo.Uid, boResponse.Dto.Uid, "Uid doesn't match");

            Assert.AreEqual(bo.KeyResults.Count, boResponse.Dto.KeyResults.Count, "KeyResults count doesn't match");
            Assert.AreEqual(bo.KeyResults[0].BusinessOutcomeId, boResponse.Dto.KeyResults[0].BusinessOutcomeId, "BusinessOutcomeId count doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Title, boResponse.Dto.KeyResults[0].Title, "Key Result Title doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsImpact, boResponse.Dto.KeyResults[0].IsImpact, "Key Result IsImpact doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Start, boResponse.Dto.KeyResults[0].Start, "Key Result Start doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Target, boResponse.Dto.KeyResults[0].Target, "Key Result Target doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Progress, boResponse.Dto.KeyResults[0].Progress, "Key Result Progress doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsDeleted, boResponse.Dto.KeyResults[0].IsDeleted, "Key Result IsDeleted doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Uid, boResponse.Dto.KeyResults[0].Uid, "Key Result Uid doesn't match");

            Assert.AreEqual(bo.KeyResults[0].Metric.Name, boResponse.Dto.KeyResults[0].Metric.Name, "Key Result Metric Name doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Metric.TypeId, boResponse.Dto.KeyResults[0].Metric.TypeId, "Key Result Metric TypeId doesn't match");
        }
    }
}