using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Radars
{
    [TestClass]
    [TestCategory("Radars")]
    public class GetRadarsDetails : BaseV1Test
    {
        private const int InvalidCompanyId = 544436464;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Radars_Get_Details_Success()
        {
            var client = await GetAuthenticatedClient();

            var companyResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            companyResponse.EnsureSuccess();
            var companyId = companyResponse.Dto.First().Id;

            var response = await client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetails(companyId));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            Assert.IsTrue(response.Dto.All(dto => dto.SurveyId > 0), "SurveyId is invalid");
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            Assert.IsTrue(response.Dto.All(dto => !string.IsNullOrEmpty(dto.CreatedAt.ToString())), "CreatedAt is empty or null");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Radars_Get_Details_NotFound()
        {
            var client = await GetAuthenticatedClient();

            var response = await client.GetAsync(RequestUris.RadarsDetails(InvalidCompanyId));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status codes do not match");
        }
    }
}
