using System;
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
    public class GetRadarByCompanyTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task Radars_ByCompany_Get_Success()
        {
            //given
            var client = await GetInsightsAuthenticatedClient();

            //when
            var response = await client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsByCompany(Company.InsightsId));
            response.EnsureSuccess();

            //then
            var businessAgilityRadar = response.Dto.FirstOrDefault(d => d.Name == "Business Agility") ?? throw new Exception("Business Agility was not found in the response.");
            var teamHealthRadar = response.Dto.FirstOrDefault(d => d.Name == "Team Health") ?? throw new Exception("Team Health was not found in the response.");
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");
            Assert.IsTrue(businessAgilityRadar.Id > 0, $"Id is less than or equal to 0 for {businessAgilityRadar.Name}.");
            Assert.IsTrue(businessAgilityRadar.OriginalVersion > 0, $"OgigionalVersion is less than or equal to 0 for {businessAgilityRadar.Name}.");
            Assert.AreEqual(0, businessAgilityRadar.Order,$"Order does not match for {businessAgilityRadar.Name}");
            Assert.AreEqual("Team", businessAgilityRadar.SurveyType, $"SurveyType does not match for Radar {businessAgilityRadar.Name}");
            Assert.IsTrue(teamHealthRadar.Id > 0, $"Id is less than or equal to 0 for {teamHealthRadar.Name}.");
            Assert.IsTrue(teamHealthRadar.OriginalVersion > 0, $"OrigionalVersion is less than or equal to 0 for {teamHealthRadar.Name}.");
            Assert.AreEqual(1, teamHealthRadar.Order, $"Order does not match for {teamHealthRadar.Name}");
            Assert.AreEqual("Team", teamHealthRadar.SurveyType, $"SurveyType does not match for Radar {teamHealthRadar.Name}");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task Radars_ByCompany_Get_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.GetAsync<RadarResponse>(RequestUris.RadarsByCompany(Company.InsightsId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }
    }
}
