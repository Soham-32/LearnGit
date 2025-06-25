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
    public class GetBusinessOutcomeCardTypeTest : BaseV1Test
    {
        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_CardType_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var getCardTypeResponse = await client.GetAsync<IList<BusinessOutcomeCardTypeResponse>>(RequestUris.BusinessOutcomeGetCardType(Company.Id));

            // assert

            Assert.AreEqual(HttpStatusCode.OK, getCardTypeResponse.StatusCode, "Status code didn't match");
            Assert.IsTrue(getCardTypeResponse.Dto.Any(), "The response body is empty");
            Assert.AreEqual(5, getCardTypeResponse.Dto.Count, "Unexpected number of card types returned");

            var expectedCardTypes = new[]
            {
                new { BusinessOutcomeCardTypeId = 1, Name = "Business Outcomes", MasterName = "Business Outcomes" },
                new { BusinessOutcomeCardTypeId = 4, Name = "Initiatives", MasterName = "Initiatives" },
                new { BusinessOutcomeCardTypeId = 2, Name = "Projects", MasterName = "Projects" },
                new { BusinessOutcomeCardTypeId = 3, Name = "Deliverables", MasterName = "Deliverables" },
                new { BusinessOutcomeCardTypeId = 5, Name = "Stories", MasterName = "Stories" }
            };

            for (var i = 0; i < getCardTypeResponse.Dto.Count; i++)
            {
                Assert.AreEqual(expectedCardTypes[i].BusinessOutcomeCardTypeId, getCardTypeResponse.Dto[i].BusinessOutcomeCardTypeId, $"BusinessOutcomeCardTypeId does not match at index {i}");
                Assert.AreEqual(expectedCardTypes[i].Name, getCardTypeResponse.Dto[i].Name, $"Name does not match at index {i}");
                Assert.AreEqual(expectedCardTypes[i].MasterName, getCardTypeResponse.Dto[i].MasterName, $"MasterName does not match at index {i}");
                Assert.IsTrue(getCardTypeResponse.Dto[i].IsActive, $"IsActive is false for <{getCardTypeResponse.Dto[i].Name}>.");
            }
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_CardType_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var getMetricResponse = await client.GetAsync(RequestUris.BusinessOutcomeGetCardType(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, getMetricResponse.StatusCode, "Status code didn't match'");
        }
    }
}
