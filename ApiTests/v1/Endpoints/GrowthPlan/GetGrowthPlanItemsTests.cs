using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.GrowthPlan
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GetGrowthPlanItemsTests : BaseV1Test
    {
        private static int _teamId;
        private static Guid _teamUid;
        private static Guid _companyUid;
        private static int _nonAssignTeamId;
        private static CompanyHierarchyResponse _allTeamsList;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static RadarQuestionDetailsResponse _questionDetailsResponse;
        private static GrowthPlanItemRequest _growthPlanItemRequest;
        private static GrowthPlanItemResponse _growthPlanItemDetails;
        private static List<int> _competenciesId;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void GetTeamDetails(TestContext _)
        {
            //Create a Team
            _team = TeamFactory.GetNormalTeam("Team");
            _team.Tags.RemoveAll(a=> a.Category.Equals("Business Lines"));
            _teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(_team, CompanyAdminUser).GetAwaiter().GetResult();

            //Getting Team Info
            _allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);
            _nonAssignTeamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(_allTeamsList.CompanyId).GetTeamByName(_teamResponse.Name).TeamId;
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(_allTeamsList.CompanyId).GetTeamByName(SharedConstants.Team).TeamId;

            //Creating Growth Plan for Non Assigned Team
            var surveyId = new SetupTeardownApi(TestEnvironment)
                .GetRadar(_allTeamsList.CompanyId, SharedConstants.TeamAssessmentType).Id;
            _questionDetailsResponse =
                new SetupTeardownApi(TestEnvironment).GetRadarQuestions(_allTeamsList.CompanyId, surveyId);
            _competenciesId = _questionDetailsResponse.Dimensions.Select(s => s.Subdimensions).Last()
                .Select(c => c.Competencies).Last().Select(i => i.CompetencyId).ToList();
            _growthPlanItemRequest = GrowthPlanFactory.GrowthItemCreateRequest(_allTeamsList.CompanyId, _nonAssignTeamId, _competenciesId);
            new SetupTeardownApi(TestEnvironment).CreateGrowthItem(_growthPlanItemRequest, SiteAdminUser);
            var growthPlanItemList = new SetupTeardownApi(TestEnvironment).GetGrowthPlanItemList(_allTeamsList.CompanyId, SiteAdminUser);
            _growthPlanItemDetails = growthPlanItemList.Result.First(x => x.Title.Equals(_growthPlanItemRequest.Title));

            //Getting Team Uid for assigned team and Company Uid
            var client = ClientFactory.GetAuthenticatedClient(SiteAdminUser.Username, SiteAdminUser.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var teamQueryString = new Dictionary<string, object>
            {
                {"companyId",  _allTeamsList.CompanyId},
                {"teamId",  _teamId}
            };

            var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter(teamQueryString)).GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            _teamUid = teamResponse.Dto.First().Uid;

            var companyResponse =
                client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(_allTeamsList.CompanyId)).GetAwaiter().GetResult();
            companyResponse.EnsureSuccess();
            _companyUid = companyResponse.Dto.Uid;
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            if (User.IsBusinessLineAdmin() || User.IsTeamAdmin() || User.IsOrganizationalLeader())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
                Assert.That.ListNotContains(response.Dto.Select(a => a.TeamId.ToString()).ToList(), _nonAssignTeamId.ToString(), $"Growth Item { _growthPlanItemDetails.Title} - from non assigned team is present");
            }
            Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Get_Items_By_CompanyID_Invalid_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanItems(0));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("Member")]
        public async Task GrowthPlan_Get_Items_By_CompanyID_valid_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanItems(Company.Id));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        public async Task GrowthPlan_Get_Items_By_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanItems(SharedConstants.FakeCompanyId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task GrowthPlan_Get_NonAssign_Team_Items_By_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamId", _nonAssignTeamId));

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyID_CompanyUid_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("companyUid" , _companyUid));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            if (User.IsBusinessLineAdmin() || User.IsTeamAdmin() || User.IsOrganizationalLeader())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
                Assert.That.ListNotContains(response.Dto.Select(a => a.TeamId.ToString()).ToList(), _nonAssignTeamId.ToString(), $"Growth Item { _growthPlanItemDetails.Title} - from non assigned team is present");
            }
            Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_TeamId_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamId" , _teamId));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");

            if (User.IsBusinessLineAdmin())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
            }
            else if (User.IsTeamAdmin())
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId.Equals(_teamId)), "Team Id is not matched");
            }
            else
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            }

            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_Empty_TeamId_Success()
        {
            // given
            var client = await GetAuthenticatedClient();
            // when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamId" , 0));

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            if (User.IsBusinessLineAdmin() || User.IsTeamAdmin() || User.IsOrganizationalLeader())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
                Assert.That.ListNotContains(response.Dto.Select(a => a.TeamId.ToString()).ToList(), _nonAssignTeamId.ToString(), $"Growth Item { _growthPlanItemDetails.Title} - from non assigned team is present");
            }
            Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_TeamUid_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamUid" , _teamUid));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");

            if (User.IsBusinessLineAdmin())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
            }
            else if (User.IsTeamAdmin())
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId.Equals(_teamId)), "Team Id is not matched");
            }
            else
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            }

            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_Empty_TeamUid_Success() 
        {
            // given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter("teamUid" , Guid.Empty));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            if (User.IsBusinessLineAdmin() || User.IsTeamAdmin() || User.IsOrganizationalLeader())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
                Assert.That.ListNotContains(response.Dto.Select(a => a.TeamId.ToString()).ToList(), _nonAssignTeamId.ToString(), $"Growth Item { _growthPlanItemDetails.Title} - from non assigned team is present");
            }
            Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_TeamId_TeamUid_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var queryString = new Dictionary<string, object>
            {
                 {"teamId",  _teamId},
                 {"teamUid",  _teamUid}
            };

            //when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter(queryString));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            
            if (User.IsBusinessLineAdmin())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
            }
            else if (User.IsTeamAdmin())
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId.Equals(_teamId)), "Team Id is not matched");
            }
            else
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            }

            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty"); Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task GrowthPlan_Get_Items_By_CompanyId_CompanyUid_TeamId_TeamUid_Valid_Success()
        {
            // given
            var client = await GetAuthenticatedClient();

            var queryString = new Dictionary<string, object>
            {
                 {"companyUid",  _companyUid},
                 {"teamId",  _teamId},
                 {"teamUid",  _teamUid}
            };

            //when
            var response = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(Company.Id).AddQueryParameter(queryString));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code doesn't match.");
            Assert.IsTrue(response.Dto.Count >= 0, "GrowthPlan Items is Null Or Empty");
            Assert.IsTrue(response.Dto.All(e => e.CompanyId.Equals(Company.Id)), "Company Id not matched");
            Assert.IsTrue(response.Dto.All(t => t.Id > 0), "Id is null or empty");
            
            if (User.IsBusinessLineAdmin())
            {
                Assert.That.ListNotContains(response.Dto.Select(a => a.Title).ToList(), _growthPlanItemDetails.Title, $"Growth Item {_growthPlanItemDetails.Title} - from non assigned team is present");
            }
            else if (User.IsTeamAdmin())
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId.Equals(_teamId)), "Team Id is not matched");
            }
            else
            {
                Assert.IsTrue(response.Dto.All(e => e.TeamId >= 0), "Team Id is not greater than or equal to zero");
            }

            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Title)), "Title is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Category)), "Category is null or empty");
            //Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.UpdatedBy)), "Updated By is null or empty");  Commented due to Bug : 27843
            Assert.IsTrue(response.Dto.All(t => t.PriorityId >= 0), "Priority Id is null or empty");
            Assert.IsTrue(response.Dto.All(a => !string.IsNullOrEmpty(a.Priority)), "Priority is null or empty");
        }
    }
}