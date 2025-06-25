using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamsWithMembersAndSurveyCountTests : BaseV1Test
    {
        private static int _assignTeamId;
        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static Guid _teamUid;
        private static Guid _multiTeamUid;
        private static Guid _enterpriseTeamUid;
        private static CompanyHierarchyResponse _allTeamsList;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User SiteAdminUser => SiteAdminUserConfig.GetUserByDescription("user 1");

        [ClassInitialize]
        public static void GetTeamDetails(TestContext _)
        {
            //Create a team

            //Get Team Info
            _allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(
                SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, SiteAdminUser);

            //_nonAssignTeamId = _allTeamsList.GetTeamByName(_nonAssignTeamResponse.Name).TeamId;
            _assignTeamId = _allTeamsList.GetTeamByName(SharedConstants.RadarTeam).TeamId;
            _multiTeamId = _allTeamsList.GetTeamByName(SharedConstants.MultiTeamForGrowthJourney).TeamId;
            _enterpriseTeamId = _allTeamsList.GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney).TeamId;

            //get Team Uid
            var client = ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var teamProfileResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("companyId", Company.Id).AddQueryParameter("teamId", _assignTeamId)).GetAwaiter().GetResult();
            teamProfileResponse.EnsureSuccess();
            _teamUid = teamProfileResponse.Dto.First().Uid;

            var multiTeamProfileResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("companyId", Company.Id).AddQueryParameter("teamId", _multiTeamId)).GetAwaiter().GetResult();
            multiTeamProfileResponse.EnsureSuccess();
            _multiTeamUid = multiTeamProfileResponse.Dto.First().Uid;

            var enterpriseTeamProfileResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("companyId", Company.Id).AddQueryParameter("teamId", _enterpriseTeamId)).GetAwaiter().GetResult();
            enterpriseTeamProfileResponse.EnsureSuccess();
            _enterpriseTeamUid = enterpriseTeamProfileResponse.Dto.First().Uid;

        }

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_By_CompanyId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(RequestUris.TeamsWithMembersAndSurveysCount(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status codes do not match");
            foreach (var team in response.Dto)
            {
                Assert.IsTrue(team.TeamId >= 0, "Team Id is not greater than or equal to zero");
                Assert.IsTrue(team.TeamUid.CompareTo(new Guid()) != 0, "Team Uid is greater than zero");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(team.Name), "Team Name is null");
                foreach (var survey in team.Surveys)
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(survey.Name), "Survey Name is invalid");
                    Assert.IsTrue(survey.SurveyId > 0, "Survey id is invalid");
                }
            }
        }

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_By_CompanyId_TeamId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();
            var stakeHolderResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(_teamUid));
            stakeHolderResponse.EnsureSuccess();
            var teamMembers = memberResponse.Dto.Concat(stakeHolderResponse.Dto).ToList();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When
            var response = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(
                RequestUris.TeamsWithMembersAndSurveysCount(Company.Id).AddQueryParameter("teamId", _assignTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                var expectedTeamMembers = teamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                Assert.That.ListsAreEqual(expectedTeamMembers, actualTeamMembers, "Team Members are not matched");
                Assert.AreEqual(_assignTeamId, team.TeamId, "Team Id is not matched");
                Assert.AreEqual(SharedConstants.RadarTeam, team.Name, "Team Name is not matched");
                Assert.AreEqual(_teamUid, team.TeamUid, "Team Uid not matched");
                foreach (var survey in team.Surveys)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.SurveyId, $"{survey.Name + " " + survey.SurveyId} is not available");
                }
            }
        }

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_By_CompanyId_MultiTeamId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();
            var stakeHolderResponse =
                await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(_teamUid));
            stakeHolderResponse.EnsureSuccess();
            var teamMembers = memberResponse.Dto.Concat(stakeHolderResponse.Dto).ToList();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When
            var response = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(
                RequestUris.TeamsWithMembersAndSurveysCount(Company.Id).AddQueryParameter("teamId", _multiTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                if (team.TeamId == _assignTeamId)
                {
                    var expectedTeamMembers = teamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    Assert.That.ListsAreEqual(expectedTeamMembers, actualTeamMembers, "Team Members are not matched");
                    Assert.AreEqual(_assignTeamId, team.TeamId, "Team Id is not matched");
                    Assert.AreEqual(_teamUid, team.TeamUid, "Team Uid not matched");
                    Assert.AreEqual(SharedConstants.RadarTeam, team.Name, "Team Name is not matched");
                }
                else
                {
                    Assert.AreEqual(_multiTeamId, team.TeamId, "Team Id is not matched");
                    Assert.AreEqual(_multiTeamUid, team.TeamUid, "Team Uid not matched");
                    Assert.AreEqual(SharedConstants.MultiTeamForGrowthJourney, team.Name, "Multi Team Name is not matched");
                }
                foreach (var survey in team.Surveys)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.SurveyId, $"{survey.Name + " " + survey.SurveyId} is not available");
                }
            }
        }

        // 200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_By_CompanyId_EnterpriseTeamId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();
            var stakeHolderResponse =
                await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamStakeholder(_teamUid));
            stakeHolderResponse.EnsureSuccess();
            var teamMembers = memberResponse.Dto.Concat(stakeHolderResponse.Dto).ToList();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When
            var response = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(
                RequestUris.TeamsWithMembersAndSurveysCount(Company.Id).AddQueryParameter("teamId", _enterpriseTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                if (team.TeamId == _assignTeamId)
                {
                    var expectedTeamMembers = teamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    Assert.That.ListsAreEqual(expectedTeamMembers, actualTeamMembers, "Team Members are not matched");
                    Assert.AreEqual(_assignTeamId, team.TeamId, "Team Id is not matched");
                    Assert.AreEqual(_teamUid, team.TeamUid, "Team Uid not matched");
                    Assert.AreEqual(SharedConstants.RadarTeam, team.Name, "Team Name is not matched");
                }
                else if (team.TeamId == _multiTeamId)
                {
                    Assert.AreEqual(_multiTeamId, team.TeamId, "Multi Team Id is not matched");
                    Assert.AreEqual(_multiTeamUid, team.TeamUid, "Multi Team Uid not matched");
                    Assert.AreEqual(SharedConstants.MultiTeamForGrowthJourney, team.Name, "Multi Team Name is not matched");
                }
                else
                {
                    Assert.AreEqual(_enterpriseTeamId, team.TeamId, "Enterprise Team Id is not matched");
                    Assert.AreEqual(_enterpriseTeamUid, team.TeamUid, "Enterprise Team Uid not matched");
                    Assert.AreEqual(SharedConstants.EnterpriseTeamForGrowthJourney, team.Name, "Enterprise Team Name is not matched");
                }
                foreach (var survey in team.Surveys)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.SurveyId, $"{survey.Name + " " + survey.SurveyId} is not available");
                }
            }
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.TeamsWithMembersAndSurveysCount(0));

            // Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<TeamsWithMembersAndSurveysResponse>(RequestUris.TeamsWithMembersAndSurveysCount(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        // 403
        // BUG 30557
        [TestMethod]
        //[TestCategory("TeamAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_By_CompanyId_NonAssignTeamId_Forbidden()
        {
            var companyAdminUser = new UserConfig("CA").GetUserByDescription("user 1");
            var team = TeamFactory.GetNormalTeam("Team");
            team.Tags.RemoveAll(a => a.Category.Equals("Business Lines"));
            var nonAssignTeamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, companyAdminUser).GetAwaiter().GetResult();
            var allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(
               SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, companyAdminUser);

            var nonAssignTeamId = allTeamsList.GetTeamByName(nonAssignTeamResponse.Name).TeamId;
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(
                RequestUris.TeamsWithMembersAndSurveysCount(Company.Id).AddQueryParameter("teamId", nonAssignTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }

        // 403
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_Get_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.TeamsWithMembersAndSurveysCount(SharedConstants.FakeCompanyId));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_TeamsWithMemberAndSurveyCount_InvalidTeamId_Get_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync(RequestUris.TeamsWithMembersAndSurveysCount(Company.Id).AddQueryParameter("teamId", 3));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }

    }
}