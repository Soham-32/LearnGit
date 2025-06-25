using AtCommon.Api;
using AtCommon.Dtos.Radars;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarsDefaultTests : BaseV1Test
    {
        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Radars_Default_Get_OK()
        {
            var client = await GetAuthenticatedClient();
            var companyId = Company.Id;
  
            var response = await client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsDefault(companyId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match.");
            Assert.IsTrue(response.Dto.All(radar => radar.CompanyId == Company.Id), "CompanyId does not match");
            Assert.IsTrue(response.Dto.All(radar => !string.IsNullOrWhiteSpace(radar.Name)), "Name is null or empty");
            Assert.IsTrue(response.Dto.All(radar => !string.IsNullOrWhiteSpace(radar.SurveyType)), "SurveyType is null or empty");
            Assert.IsTrue(response.Dto.All(radar => !string.IsNullOrWhiteSpace(radar.ThumbnailUrl)), "ThumbnailUrl is null or empty");

        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Radars_Default_Get_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var companyId = Company.Id;
  
            var response = await client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsDefault(companyId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match.");

        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("Rocket")]
        public async Task Radars_Default_Get_Forbidden()
        {
            var client = await GetAuthenticatedClient();
            var companyId = Company.Id;
  
            var response = await client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsDefault(companyId));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match.");
        }

    }
}
