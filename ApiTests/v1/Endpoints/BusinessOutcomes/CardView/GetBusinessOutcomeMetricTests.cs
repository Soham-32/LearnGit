using System.Collections.Generic;
using System.Linq;
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
    public class GetBusinessOutcomeMetricTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Metric_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //creating a business outcome metric
            var addMetricBody = BusinessOutcomesFactory.GetValidBusinessOutcomeAddMetricBody(Company.Id);
            var addMetricResponse = await client.PostAsync<BusinessOutcomeMetricResponse>(RequestUris.BusinessOutcomeAddNewMetric(), addMetricBody);
            addMetricResponse.EnsureSuccess();

            //act
            var getMetricResponse = await client.GetAsync<IList<BusinessOutcomeMetricResponse>>(RequestUris.BusinessOutcomeGetMetrics(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, getMetricResponse.StatusCode, "Status Code didn't match");
            Assert.IsTrue(getMetricResponse.Dto.Count > 0, "Dto count is zero");

            Assert.IsTrue(getMetricResponse.Dto.Any(dto=>dto.Name.Equals(addMetricBody.Name)), $"{addMetricBody.Name} isn't part of metric response'");
            var actualMetric = getMetricResponse.Dto.First(dto => dto.Name.Equals(addMetricBody.Name));
            Assert.AreEqual(addMetricBody.Order, actualMetric.Order, "Order didn't match");
            Assert.AreEqual(addMetricBody.TypeId, actualMetric.TypeId, "TypeId didn't match");
            Assert.AreEqual(addMetricResponse.Dto.CreatedAt?.ToString("dd-MM-yyyy HH:mm"), actualMetric.CreatedAt?.ToString("dd-MM-yyyy HH:mm"), "CreatedAt didn't match");
            Assert.AreEqual(addMetricResponse.Dto.Uid, actualMetric.Uid, "Uid didn't match");
            Assert.AreEqual(addMetricResponse.Dto.TypeId, actualMetric.TypeId, "TypeId didn't match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Metric_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var getMetricResponse = await client.GetAsync(RequestUris.BusinessOutcomeGetMetrics(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, getMetricResponse.StatusCode, "Status code didn't match'");
        }
    }
}
