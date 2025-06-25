using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class AddBusinessOutcomeTests : BaseV1Test
    {
        
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);

            // act
            var boResponse = await client.PostAsync(RequestUris.BusinessOutcomePost(), bo.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, boResponse.StatusCode);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Post_Created()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);

            //Setting Custom Field Info
            var siteAdmin = User.IsSiteAdmin() ? User : new UserConfig("SA").GetUserByDescription("user 1");
            var customField = new SetupTeardownApi(TestEnvironment).AddBusinessOutcomesCustomFields(
                Company.Id, new List<CustomFieldRequest> {BusinessOutcomesFactory.GetCustomFieldRequest()}, siteAdmin).First();
            
            var customFieldData = new CustomFieldValueRequest()
            {
                CustomFieldUid = customField.Uid,
                Name = customField.Name,
                Value = "Voltron"
            };
            bo.CustomFieldValues.Add(customFieldData);
            
            
            // act
            var boResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, boResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(bo.Title, boResponse.Dto.Title, "Title doesn't match");
            Assert.AreEqual(bo.Description, boResponse.Dto.Description, "Description doesn't match");
            Assert.AreEqual(bo.OverallProgress, boResponse.Dto.OverallProgress, "OverallProgress doesn't match");
            Assert.AreEqual(bo.CardColor, boResponse.Dto.CardColor, "CardColor doesn't match");
            Assert.AreEqual(User.FullName, boResponse.Dto.Owner, "Owner doesn't match");
            Assert.AreEqual(bo.TeamId, boResponse.Dto.TeamId, "TeamId doesn't match");
            Assert.AreEqual(bo.CompanyId, boResponse.Dto.CompanyId, "CompanyId doesn't match");
            Assert.AreEqual(bo.SwimlaneType, boResponse.Dto.SwimlaneType, "SwimlaneType doesn't match");
            Assert.AreEqual(bo.IsDeleted, boResponse.Dto.IsDeleted, "IsDeleted doesn't match");
            Assert.AreEqual(0, boResponse.Dto.Tags.Count, "Tags count doesn't match");
            Assert.AreEqual(0, boResponse.Dto.Comments.Count, "Comments count doesn't match");

            Assert.AreEqual(bo.KeyResults.Count, boResponse.Dto.KeyResults.Count, "KeyResults count doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Title, boResponse.Dto.KeyResults[0].Title, "Key Result Title doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsImpact, boResponse.Dto.KeyResults[0].IsImpact, "Key Result IsImpact doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Start, boResponse.Dto.KeyResults[0].Start, "Key Result Start doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Target, boResponse.Dto.KeyResults[0].Target, "Key Result Target doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Progress, boResponse.Dto.KeyResults[0].Progress, "Key Result Progress doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsDeleted, boResponse.Dto.KeyResults[0].IsDeleted, "Key Result IsDeleted doesn't match");

            Assert.AreEqual(bo.KeyResults[0].Metric.Name, boResponse.Dto.KeyResults[0].Metric.Name, "Key Result Metric Name doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Metric.TypeId, boResponse.Dto.KeyResults[0].Metric.TypeId, "Key Result Metric TypeId doesn't match");

            Assert.AreEqual(bo.CustomFieldValues.Count, boResponse.Dto.CustomFieldValues.Count, "Custom field count doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].CustomFieldUid, boResponse.Dto.CustomFieldValues[0].CustomFieldUid, "Custom field CustomFieldUid doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].Value, boResponse.Dto.CustomFieldValues[0].Value, "Custom field Value doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].Name, boResponse.Dto.CustomFieldValues[0].Name, "Custom field Name doesn't match");
        }
    }
}