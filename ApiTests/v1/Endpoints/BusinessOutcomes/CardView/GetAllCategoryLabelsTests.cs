using System;
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
    public class GetAllCategoryLabelsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_CategoryLabels_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();

            //act
            var response = await client.GetAsync<IList<BusinessOutcomeCategoryLabelResponse>>(RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(Company.Id));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,"Status code didn't match");
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

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_CategoryLabels_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.GetAsync(RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(Company.Id));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }
    }
}
