using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class AddBusinessOutcomeMetricTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Metric_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            var addMetricBody = BusinessOutcomesFactory.GetValidBusinessOutcomeAddMetricBody(Company.Id);

            // act
            var addMetricResponse = await client.PostAsync(RequestUris.BusinessOutcomeAddNewMetric(), addMetricBody.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, addMetricResponse.StatusCode,"Status code didn't match");
        }

        
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Metric_Post_Created()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var addMetricBody = BusinessOutcomesFactory.GetValidBusinessOutcomeAddMetricBody(Company.Id);

            // act
            var addMetricResponse = await client.PostAsync<BusinessOutcomeMetricResponse>(RequestUris.BusinessOutcomeAddNewMetric(), addMetricBody);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, addMetricResponse.StatusCode, "Status Code didn't match");
            Assert.AreEqual(addMetricBody.Name, addMetricResponse.Dto.Name, "Name didn't match");
            Assert.AreEqual(addMetricBody.Order, addMetricResponse.Dto.Order, "Order didn't match");
            Assert.AreEqual(addMetricBody.TypeId, addMetricResponse.Dto.TypeId, "TypeId didn't match");
            Assert.IsTrue(addMetricResponse.Dto.Type is null, "Type isn't NULL");
        }
    }
}