using System;
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
    public class CreateBusinessOutcomeCategoryLabelTests : BaseV1Test
    {
        // 200-201
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_CategoryLabels_Post_OK()
        {
            // given
            var client = await GetAuthenticatedClient();
            var existingLabelsResponse = client.GetAsync<List<BusinessOutcomeCategoryLabelResponse>>(
                RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(Company.Id)).GetAwaiter().GetResult();
            existingLabelsResponse.EnsureSuccess();
            // convert to requests and filter out temp labels
            var automationLabels = existingLabelsResponse.Dto.Where(m => m.Name.Contains("Automation"))
                .Select(l => l.ToRequest()).ToList();
            // add the new labels
            var newLabelRequest = BusinessOutcomesFactory.GetBusinessOutcomeCategoryLabelRequest(Company.Id, 1);
            automationLabels.Add(newLabelRequest);

            // when
            var response = await client.PostAsync<IList<BusinessOutcomeCategoryLabelResponse>>(
                RequestUris.BusinessOutcomeCreateBusinessOutcomeCategoryLabelsAndTags(Company.Id,0),
                automationLabels);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Any(), "The response body is empty");
            foreach (var labelResponse in response.Dto)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(labelResponse.Name), "Name is empty");
                Assert.AreEqual(Company.Id, labelResponse.CompanyId, $"Company Id doesn't match for <{labelResponse.Name}>.");
                Assert.IsTrue(labelResponse.Uid != Guid.Empty, "Uid is all zeroes");
                Assert.IsTrue(DateTime.Compare(labelResponse.CreatedAt, new DateTime()) != 0, 
                    $"CreatedAt is not valid for <{labelResponse.Name}>.");
                foreach (var tag in labelResponse.Tags)
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(tag.Name), $"Tag Name is empty for <{labelResponse.Name}>");
                    Assert.IsTrue(tag.Uid != Guid.Empty, $"Uid is all zeroes for tag <{tag.Name}> under label <{labelResponse.Name}>");
                    Assert.IsTrue(DateTime.Compare(tag.CreatedAt, new DateTime()) != 0, 
                        $"CreatedAt is not valid for tag <{tag.Name}> under <{labelResponse.Name}>.");
                    Assert.AreEqual(labelResponse.Uid, tag.CategoryLabelUid, $"CategoryLabelUid doesn't match for tag <{tag.Name}> under label <{labelResponse.Name}>");
                }
            }
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task BusinessOutcomes_CategoryLabels_Post_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.PostAsync(RequestUris.BusinessOutcomeCreateBusinessOutcomeCategoryLabelsAndTags(Company.Id,0),
                new List<BusinessOutcomeCategoryLabelRequest>().ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code doesn't match.");
        }

    }
}