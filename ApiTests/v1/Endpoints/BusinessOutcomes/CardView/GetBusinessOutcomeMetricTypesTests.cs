using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class GetBusinessOutcomeMetricTypesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_MetricTypes_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var metricTypeResponse = await client.GetAsync<IList<BusinessOutcomeMetricTypeResponse>>(RequestUris.BusinessOutcomeGetMetricTypes(Company.Id));
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, metricTypeResponse.StatusCode,"Status code didn't match'");
            Assert.AreEqual(3, metricTypeResponse.Dto.Count, "Metric Type count didn't match'");
            Assert.IsTrue(metricTypeResponse.Dto.Any(dto=>dto.Name.Equals("Percent")), "'Percent' metric type isn't present");
            Assert.IsTrue(metricTypeResponse.Dto.Any(dto => dto.Name.Equals("Number")), "'Number' metric type isn't present");
            Assert.IsTrue(metricTypeResponse.Dto.Any(dto => dto.Name.Equals("Money")), "Money' metric type isn't present");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_MetricTypes_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var metricTypeResponse = await client.GetAsync<IList<BusinessOutcomeMetricTypeResponse>>(RequestUris.BusinessOutcomeGetMetricTypes(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, metricTypeResponse.StatusCode, "Status code didn't match'");
        }
    }
}
