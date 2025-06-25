using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class UpdateGrowthPlanItemsTests : BaseV1Test
    {
        private static int _teamId;

        [ClassInitialize]
        public static void GetTeamDetails(TestContext testContext)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team).TeamId;
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        [TestCategory("KnownDefect")]   //Bug: 41997
        public async Task GrowthPlan_Put_Item_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var radarResponse = await client.GetAsync<RadarQuestionDetailsResponse>(
                RequestUris.RadarQuestions(Company.Id).AddQueryParameter("surveyId", SharedConstants.TeamSurveyId));
            radarResponse.EnsureSuccess();

            var competenciesIds = radarResponse.Dto.Dimensions.Select(s => s.Subdimensions).First()
                .Select(c => c.Competencies).First().Select(i => i.CompetencyId).ToList();

            var growthItemRequestDetails = GrowthPlanFactory.GrowthItemCreateRequest(Company.Id, _teamId, competenciesIds);
            var createGrowthItem = await client.PutAsync<GrowthPlanItemIdResponse>(RequestUris.GrowthPlanSave(), growthItemRequestDetails);
            createGrowthItem.EnsureSuccess();
            var updateCompetenciesIds = radarResponse.Dto.Dimensions.Select(s => s.Subdimensions).Last()
                .Select(c => c.Competencies).Last().Select(i => i.CompetencyId).ToList();
            var updatedCompetenciesNames = radarResponse.Dto.Dimensions.Select(s => s.Subdimensions).Last()
                .Select(c => c.Competencies).Last().Select(i => i.Name).OrderBy(n => n).ToList();
            var updateGrowthItemRequestDetails = GrowthPlanFactory.GrowthItemUpdateRequest(Company.Id, _teamId, updateCompetenciesIds, createGrowthItem.Dto.GrowthPlanItemId);

            // when
            var updatedGrowthItemResponse = await client.PutAsync(RequestUris.GrowthPlanSave(), updateGrowthItemRequestDetails.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.NoContent, updatedGrowthItemResponse.StatusCode, "Status code doesn't match.");

            var getUpdatedGrowthItemLists = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamId", _teamId));
            getUpdatedGrowthItemLists.EnsureSuccess();
            Assert.IsTrue(getUpdatedGrowthItemLists.Dto.Any(x => x.Id.Equals(createGrowthItem.Dto.GrowthPlanItemId)), "Growth Item Id is not available");

            var actualUpdatedGrowthItemDetails = getUpdatedGrowthItemLists.Dto.First(x => x.Id.Equals(createGrowthItem.Dto.GrowthPlanItemId));
            Assert.IsTrue(getUpdatedGrowthItemLists.Dto.Any(x => x.Title.Equals(updateGrowthItemRequestDetails.Title)), "Growth Item Title is not available");
            Assert.AreEqual(updateGrowthItemRequestDetails.CompanyId, actualUpdatedGrowthItemDetails.CompanyId, "Company Id is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.TeamId, actualUpdatedGrowthItemDetails.TeamId, "Team Id is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.Category, actualUpdatedGrowthItemDetails.Category, "Category is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.Description, actualUpdatedGrowthItemDetails.Description, "Description is not matched");
            Assert.IsTrue(actualUpdatedGrowthItemDetails.Id != 0, "Id is null");
            Assert.AreEqual(updateGrowthItemRequestDetails.Owner, actualUpdatedGrowthItemDetails.Owner, "Owner is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.Status, actualUpdatedGrowthItemDetails.Status, "Status is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.SurveyId, actualUpdatedGrowthItemDetails.SurveyId, "Survey Id is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.Priority, actualUpdatedGrowthItemDetails.Priority, "Priority is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.TargetDate?.ToString("MM/dd/yyyy"), actualUpdatedGrowthItemDetails.TargetDate?.ToString("MM/dd/yyyy"), "TargetDate is not matched");
            Assert.AreEqual(updateGrowthItemRequestDetails.Type, actualUpdatedGrowthItemDetails.Type, "Type isn't matched");
            Assert.AreEqual(updatedCompetenciesNames.ListToString(), actualUpdatedGrowthItemDetails.CompetencyName, "Competencies aren't matched");
            var updatedBy = User.IsOrganizationalLeader() ? "OrgLeader" :
                User.IsBusinessLineAdmin() ? User.Type.ToString().Replace("Admin", "") : User.Type.ToString();
            Assert.AreEqual(updatedBy, actualUpdatedGrowthItemDetails.UpdatedBy.RemoveWhitespace(), "Updated By field is not matched");
        }
    }
}