using System;
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
    public class GetBusinessOutcomeByIdTests : BaseV1Test
    {
        
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id : 53393
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Get_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var boGetResponse = await client.GetAsync<List<CustomBusinessOutcomeRequest>>(RequestUris.BusinessOutcomeGetById(Company.Id,Guid.NewGuid(), Guid.NewGuid()));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, boGetResponse.StatusCode, "Status Code doesn't match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id : 53393
        [TestCategory("CompanyAdmin"), TestCategory("SiteAdmin"), TestCategory("OrgLeader"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcome_Get_Ok()
        {

            // arrange
            var client = await GetAuthenticatedClient();
            var bo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company);

            //Setting Custom Field Info
            var siteAdmin = User.IsSiteAdmin() ? User : new UserConfig("SA").GetUserByDescription("user 1");
            var customField = new SetupTeardownApi(TestEnvironment).AddBusinessOutcomesCustomFields(Company.Id, new List<CustomFieldRequest> {BusinessOutcomesFactory.GetCustomFieldRequest()}, siteAdmin).First();
            
            var customFieldData = new CustomFieldValueRequest()
            {
                CustomFieldUid = customField.Uid,
                Name = customField.Name,
                Value = "Voltron"
            };
            bo.CustomFieldValues.Add(customFieldData);
            
            //Creating business outcome card
            
            var boPostResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(RequestUris.BusinessOutcomePost(), bo);
            boPostResponse.EnsureSuccess();

            var boId = boPostResponse.Dto.Uid;
            var swimelaneId = boPostResponse.Dto.SwimlaneId;

            // acts
            var boGetResponse = await client.GetAsync<List<CustomBusinessOutcomeRequest>>(RequestUris.BusinessOutcomeGetById(Company.Id,boId,swimelaneId));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, boGetResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(1, boGetResponse.Dto.Count, "Dto size doesn't match");
            Assert.AreEqual(boId, boGetResponse.Dto[0].Uid, "Title doesn't match");
            Assert.AreEqual(bo.Title, boGetResponse.Dto[0].Title, "Title doesn't match");
            Assert.AreEqual(bo.Description, boGetResponse.Dto[0].Description, "Description doesn't match");
            Assert.AreEqual(bo.OverallProgress, boGetResponse.Dto[0].OverallProgress, "OverallProgress doesn't match");
            Assert.AreEqual(bo.CardColor, boGetResponse.Dto[0].CardColor, "CardColor doesn't match");
            Assert.AreEqual(User.FullName, boGetResponse.Dto[0].Owner, "Owner doesn't match");
            Assert.AreEqual(bo.TeamId, boGetResponse.Dto[0].TeamId, "TeamId doesn't match");
            Assert.AreEqual(bo.CompanyId, boGetResponse.Dto[0].CompanyId, "CompanyId doesn't match");
            Assert.AreEqual(bo.SwimlaneType, boGetResponse.Dto[0].SwimlaneType, "SwimlaneType doesn't match");
            Assert.AreEqual(swimelaneId, boGetResponse.Dto[0].SwimlaneId, "SwimlaneType doesn't match");
            Assert.AreEqual(bo.IsDeleted, boGetResponse.Dto[0].IsDeleted, "IsDeleted doesn't match");
            Assert.AreEqual(0, boGetResponse.Dto[0].Tags.Count, "Tags count doesn't match");
            Assert.AreEqual(0, boGetResponse.Dto[0].Comments.Count, "Comments count doesn't match");

            Assert.AreEqual(bo.KeyResults.Count, boGetResponse.Dto[0].KeyResults.Count, "KeyResults count doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Title, boGetResponse.Dto[0].KeyResults[0].Title, "Key Result Title doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsImpact, boGetResponse.Dto[0].KeyResults[0].IsImpact, "Key Result IsImpact doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Start, boGetResponse.Dto[0].KeyResults[0].Start, "Key Result Start doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Target, boGetResponse.Dto[0].KeyResults[0].Target, "Key Result Target doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Progress, boGetResponse.Dto[0].KeyResults[0].Progress, "Key Result Progress doesn't match");
            Assert.AreEqual(bo.KeyResults[0].IsDeleted, boGetResponse.Dto[0].KeyResults[0].IsDeleted, "Key Result IsDeleted doesn't match");

            Assert.IsTrue(boGetResponse.Dto[0].KeyResults[0].Metric!=null, "Key Result Metric is null");
            Assert.AreEqual(bo.KeyResults[0].Metric.Name, boGetResponse.Dto[0].KeyResults[0].Metric.Name, "Key Result Metric Name doesn't match");
            Assert.AreEqual(bo.KeyResults[0].Metric.TypeId, boGetResponse.Dto[0].KeyResults[0].Metric.TypeId, "Key Result Metric TypeId doesn't match");

            Assert.AreEqual(bo.CustomFieldValues.Count, boGetResponse.Dto[0].CustomFieldValues.Count, "Custom field count doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].CustomFieldUid, boGetResponse.Dto[0].CustomFieldValues[0].CustomFieldUid, "Custom field CustomFieldUid doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].Value, boGetResponse.Dto[0].CustomFieldValues[0].Value, "Custom field Value doesn't match");
            Assert.AreEqual(bo.CustomFieldValues[0].Name, boGetResponse.Dto[0].CustomFieldValues[0].Name, "Custom field Name doesn't match");
        }
    }
}