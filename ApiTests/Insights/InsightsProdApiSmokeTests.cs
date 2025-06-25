using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.Account;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Insights;
using AtCommon.Dtos.Radars;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Insights
{
    [TestClass]
    [TestCategory("ProdSmoke")]
    public class InsightsProdApiSmokeTests : BaseV1Test
    {

        private static string Group => TestEnvironment.Parameters["ProdGroup"].ToString().ToUpper();
        private static string AzureDevOpsApiKey => TestEnvironment.Parameters["AzureDevOpsApiKey"].ToString();
        private static User ProdUser => TestEnvironment.UserConfig.GetUserByDescription("insights prod");

        public static IEnumerable<object[]> FilteredEnvironments
        {
            get
            {
                return AzureDevOpsApi
                    .GetProductionEnvironments(AzureDevOpsApiKey).GetAwaiter().GetResult()
                    .Where(e => e.IsActive && e.Group.ToUpper() == Group && e.IsInsightsEnabled)
                    .Select(e => new object[] { e.Name, e });
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(FilteredEnvironments))]
        public async Task Insights_Api_ProductionSmokeTest(string _, ProductionEnvironment environment)
        {
            var client = await ClientFactory.GetAuthenticatedClientWithNewToken(ProdUser.Username, ProdUser.Password, environment.Environment);
            var userResponse = await client.GetAsync<UserInfoResponse>(RequestUris.AccountGetUserInfo());
            var companyId = userResponse.Dto.ActiveCompanyId;

            //radars
            var radarsResponse = await client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsByCompany(companyId));
            radarsResponse.EnsureSuccess();
            //companies/features
            var companyFeaturesResponse = await client.PostAsync<CompanyFeatureResponse>(
                RequestUris.CompaniesFeatures(companyId),CompanyFactory.GetValidGetCompanyFeaturesRequest());
            companyFeaturesResponse.EnsureSuccess();

            //insights
            //insights/WidgetTheme
            var widgetThemeResponse = await client.GetAsync<WidgetThemeResponse>(RequestUris.InsightsWidgetTheme(1));
            widgetThemeResponse.EnsureSuccess();
            //Preferences/insightsdashboardfilters
            var preferenceResponse = await client.GetAsync<UserPreferenceResponse>(RequestUris.InsightsPreferences("insightsdashboardfilters"));
            preferenceResponse.EnsureSuccess();
            
            //structuralagility/teamworktypes
            var teamWorkTypesResponse = await client.PostAsync<IList<TeamWorkTypeResponse>>(RequestUris.AnalyticsTeamWorkType(companyId), InsightsFactory.GetValidTeamWorkTypeRequest());
            teamWorkTypesResponse.EnsureSuccess();

            //insights/dashboards
            var dashboardResponse = await client.GetAsync<IList<DashboardResponse>>(RequestUris.InsightsDashboards());
            dashboardResponse.EnsureSuccess();
            //analytics/SyncDateTime
            var syncDateTimeResponse = await client.GetAsync<string>(RequestUris.AnalyticsSyncDateTime(companyId));
            syncDateTimeResponse.EnsureSuccess();
            //analytics/IndexDimensions - Agility
            var indexDimensionsAgilityResponse = await client.PostAsync<IList<IndexAnalyticsResponse>>(
                RequestUris.AnalyticsIndexDimensions(companyId), InsightsFactory.GetIndexDimensionsRequest(0));
            indexDimensionsAgilityResponse.EnsureSuccess();
            //analytics/IndexItems - Maturity
            var indexItemsMaturityResponse = await client.PostAsync<IList<IndexAnalyticsResponse>>(
                RequestUris.AnalyticsIndexItems(companyId), InsightsFactory.GetIndexRequest(WidgetType.Maturity, 0));
            indexItemsMaturityResponse.EnsureSuccess();
            //analytics/IndexItems - Performance
            var indexItemsPerformanceResponse = await client.PostAsync<IList<IndexAnalyticsResponse>>(
                RequestUris.AnalyticsIndexItems(companyId), InsightsFactory.GetIndexRequest(WidgetType.Maturity, 0));
            indexItemsPerformanceResponse.EnsureSuccess();
            //analytics/OvertimeItems - Agility
            var overTimeItemsAgilityResponse = await client.PostAsync<IList<OvertimeAnalyticsResponse>>(
                RequestUris.AnalyticsOvertimeItems(companyId), InsightsFactory.GetValidOvertimeRequest(WidgetType.Agility, 0)); 
            overTimeItemsAgilityResponse.EnsureSuccess();
            //analytics/OvertimeItems - Maturity
            var overTimeItemsMaturityResponse = await client.PostAsync<IList<OvertimeAnalyticsResponse>>(
                RequestUris.AnalyticsOvertimeItems(companyId), InsightsFactory.GetValidOvertimeRequest(WidgetType.Maturity, 0)); 
            overTimeItemsMaturityResponse.EnsureSuccess();
            //analytics/OvertimeItems - Performance
            var overTimeItemsPerformanceResponse = await client.PostAsync<IList<OvertimeAnalyticsResponse>>(
                RequestUris.AnalyticsOvertimeItems(companyId), InsightsFactory.GetValidOvertimeRequest(WidgetType.Performance, 0)); 
            overTimeItemsPerformanceResponse.EnsureSuccess();
            //analytics/DimensionsItems - Performance
            var dimensionsItemsAgilityRequest = InsightsFactory.GetDimensionsRequest(WidgetType.Performance, DimensionSortOrder.High, 0, 1);
            var dimensionItemsAgilityResponse = await client.PostAsync<IList<DimensionsResponse>>(RequestUris.AnalyticsDimensionsItems(companyId), dimensionsItemsAgilityRequest);
            dimensionItemsAgilityResponse.EnsureSuccess();
            //analytics/DimensionsItems - Maturity
            var dimensionsItemsMaturityRequest = InsightsFactory.GetDimensionsRequest(WidgetType.Maturity, DimensionSortOrder.High, 0, 1);
            var dimensionItemsMaturityResponse = await client.PostAsync<IList<DimensionsResponse>>(RequestUris.AnalyticsDimensionsItems(companyId), dimensionsItemsMaturityRequest);
            dimensionItemsMaturityResponse.EnsureSuccess();
            //analytics/GrowthPlanItems - Team
            var growthPlanItemsTeamRequest = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Team, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            var growthPlanItemsTeamResponse = await client.PostAsync<IList<GrowthPlanAnalyticsResponse>>(
                RequestUris.AnalyticsGrowthPlanItems(companyId), growthPlanItemsTeamRequest);
            growthPlanItemsTeamResponse.EnsureSuccess();
            //analytics/GrowthPlanItems - Org
            var growthPlanItemsOrgRequest = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            var growthPlanItemsOrgResponse = await client.PostAsync<IList<GrowthPlanAnalyticsResponse>>(
                RequestUris.AnalyticsGrowthPlanItems(companyId), growthPlanItemsOrgRequest);
            growthPlanItemsOrgResponse.EnsureSuccess();
            //analytics/GrowthPlanItems - Enterprise
            var growthPlanItemsEntRequest = InsightsFactory.GetGrowthPlanAnalyticsRequest(GrowthItemCategory.Organizational, GrowthItemStatusType.All,
                0, GrowthItemSegmentType.Category);
            var growthPlanItemsEntResponse = await client.PostAsync<IList<GrowthPlanAnalyticsResponse>>(
                RequestUris.AnalyticsGrowthPlanItems(companyId), growthPlanItemsEntRequest);
            growthPlanItemsEntResponse.EnsureSuccess();

            //SA
            //analytics/structuralagility/TeamStability - Average
            var teamStabilityAverageResponse = await client.PostAsync<IList<TeamStabilityResponse>>(
                RequestUris.AnalyticsTeamStability(companyId), InsightsFactory.GetTeamStabilityRequest());
            teamStabilityAverageResponse.EnsureSuccess();
            //analytics/structuralagility/TeamStability - Distribution
            var teamStabilityDistributionResponse = await client.PostAsync<IList<TeamStabilityResponse>>(
                RequestUris.AnalyticsTeamStability(companyId), InsightsFactory.GetTeamStabilityRequest(StructuralAgilityWidgetType.Distribution));
            teamStabilityDistributionResponse.EnsureSuccess();
            //analytics/structuralagility/RoleAllocationAverage
            var roleAverageRequest = InsightsFactory.GetRoleAllocationAverageRequest(0, new List<int> { 0 });
            var roleAverageResponse = await client.PostAsync<AnalyticsTableResponse<RoleAllocationAverageResponse>>(
                RequestUris.AnalyticsRoleAllocationAverage(companyId), roleAverageRequest);
            roleAverageResponse.EnsureSuccess();
            //analytics/structuralagility/PeopleByRole - Average
            var peopleByRoleAverageRequest = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Average, 0);
            var peopleByRoleAverageResponse = await client.PostAsync<PeopleByRoleResponse>(
                RequestUris.AnalyticsPeopleByRole(companyId), peopleByRoleAverageRequest);
            peopleByRoleAverageResponse.EnsureSuccess();
            //analytics/structuralagility/PeopleByRole - Distribution
            var peopleByRoleDistributionRequest = InsightsFactory.GetPeopleByRoleRequest(StructuralAgilityWidgetType.Distribution, 0);
            var peopleByRoleDistributionResponse = await client.PostAsync<PeopleByRoleResponse>(
                RequestUris.AnalyticsPeopleByRole(companyId), peopleByRoleDistributionRequest);
            peopleByRoleDistributionResponse.EnsureSuccess();
            //analytics/structuralagility/ParticipantGroup - Average
            var participantGroupAverageRequest = InsightsFactory.GetValidParticipantGroupRequest();
            var participantGroupAverageResponse = await client.PostAsync<AnalyticsTableResponse<ParticipantGroupResponse>>(
                RequestUris.AnalyticsParticipantGroup(companyId), participantGroupAverageRequest);
            participantGroupAverageResponse.EnsureSuccess();
            //analytics/structuralagility/ParticipantGroup - Distribution
            var participantGroupDistributionRequest = InsightsFactory.GetValidParticipantGroupRequest(StructuralAgilityWidgetType.Distribution);
            var participantGroupDistributionResponse = await client.PostAsync<AnalyticsTableResponse<ParticipantGroupResponse>>(
                RequestUris.AnalyticsParticipantGroup(companyId), participantGroupDistributionRequest);
            participantGroupDistributionResponse.EnsureSuccess();
            //analytics/structuralagility/AgileNonAgileTeams - Average
            var agileNonAgileAverageRequest = InsightsFactory.GetAgileNonAgileTeamsAverageRequest("Agile Adoption");
            var agileNonAgileAverageResponse = await client.PostAsync<IList<AgileNonAgileTeamsResponse>>(
                RequestUris.AnalyticsAgileNonAgileTeams(companyId), agileNonAgileAverageRequest);
            agileNonAgileAverageResponse.EnsureSuccess();
            //analytics/structuralagility/AgileNonAgileTeams - Distribution
            var agileNonAgileDistributionRequest = InsightsFactory.GetAgileNonAgileTeamsAverageRequest("Agile Adoption", StructuralAgilityWidgetType.Distribution);
            var agileNonAgileDistributionResponse = await client.PostAsync<IList<AgileNonAgileTeamsResponse>>(
                RequestUris.AnalyticsAgileNonAgileTeams(companyId), agileNonAgileDistributionRequest);
            agileNonAgileDistributionResponse.EnsureSuccess();
            
        }
    }
}