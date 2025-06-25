using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class PostCompaniesFeaturesTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Features_Post_OK()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = CompanyFactory.GetValidGetCompanyFeaturesRequest();
            // when
            var response = await client.PostAsync<CompanyFeatureResponse>(
                RequestUris.CompaniesFeatures(Company.Id), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
                "Status code doesn't match.");
            Assert.AreEqual(Company.Id, response.Dto.CompanyId, 
                "CompanyId doesn't match.");
            Assert.AreEqual(request.FeatureIds.Count(), response.Dto.Features.Count, 
                "Feature count does not match.");
            foreach (var featureId in request.FeatureIds)
            {
                var feature = response.Dto.Features.FirstOrDefault(f => f.Feature == featureId)
                    .CheckForNull($"FeatureId <{featureId}> was not found in the response.");
                Assert.IsTrue(feature.CompanyFeatureId > 0, 
                    $"CompanyFeatureId <{feature.CompanyFeatureId}> is invalid for feature <{featureId}>");
                Assert.IsTrue(new[] {"on", "off"}.Contains(feature.Value), 
                    $"Expected: <on> or <off>. Actual: <{feature.Value}>. Value for feature <{featureId}> is not valid.");
            }
        }

        // 401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Features_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var request = CompanyFactory.GetValidGetCompanyFeaturesRequest();
            // when
            var response = await client.PostAsync(
                RequestUris.CompaniesFeatures(Company.Id), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Companies_Features_Post_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = CompanyFactory.GetValidGetCompanyFeaturesRequest();

            // when
            var response = await client.PostAsync(
                RequestUris.CompaniesFeatures(SharedConstants.FakeCompanyId), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }
        
    }
}