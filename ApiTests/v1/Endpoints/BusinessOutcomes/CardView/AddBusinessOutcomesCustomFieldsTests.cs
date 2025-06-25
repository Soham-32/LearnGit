using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class AddBusinessOutcomesCustomFieldsTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_CustomFields_Post_OK()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = new List<CustomFieldRequest> {BusinessOutcomesFactory.GetCustomFieldRequest()};

            // when
            var response = await client.PostAsync<IList<CustomFieldResponse>>(
                RequestUris.BusinessOutcomesCreateCustomFields(Company.Id), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
                "Status code doesn't match.");
            Assert.AreEqual(request.First().Name, response.Dto.First().Name, 
                "Name does not match.");
            Assert.AreNotEqual(Guid.Empty, response.Dto.First().Uid, "Uid is empty.");
            Assert.AreEqual(Company.Id, response.Dto.First().CompanyId, 
                "CompanyId does not match.");
            Assert.AreEqual(0, response.Dto.First().Order, "Order does not match.");

        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_CustomFields_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var request = new List<CustomFieldRequest> {BusinessOutcomesFactory.GetCustomFieldRequest()};

            // when
            var response = await client.PostAsync(
                RequestUris.BusinessOutcomesCreateCustomFields(Company.Id), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_CustomFields_Post_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var request = new List<CustomFieldRequest> {BusinessOutcomesFactory.GetCustomFieldRequest()};

            // when
            var response = await client.PostAsync(
                RequestUris.BusinessOutcomesCreateCustomFields(SharedConstants.FakeCompanyId), request.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

    }
}