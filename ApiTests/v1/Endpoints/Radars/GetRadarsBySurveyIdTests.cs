using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Radars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarsBySurveyIdTests : BaseV1Test
    {
        private const int StaticCompanyId = 2;
        private const int StaticSurveyId = 46464654;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Radars_Get_SurveyId_Success()
        {
            var client = await GetAuthenticatedClient();

            var radarResponse = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetails(Company.Id));
            radarResponse.EnsureSuccess();
            var surveyId = radarResponse.Dto.First().SurveyId;

            var response = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(Company.Id, surveyId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.AreEqual(surveyId, response.Dto.First().SurveyId, "Survey Ids do not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Radars_Get_SurveyId_NotFound()
        {
            var client = await GetAuthenticatedClient();


            var response = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(Company.Id, StaticSurveyId));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Radars_Get_SurveyId_Unauthorized()
        {
            var client = GetUnauthenticatedClient();

            var response = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(StaticCompanyId, StaticSurveyId));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Radars_Get_SurveyId_Forbidden()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(StaticCompanyId, StaticSurveyId));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }
    }
}
