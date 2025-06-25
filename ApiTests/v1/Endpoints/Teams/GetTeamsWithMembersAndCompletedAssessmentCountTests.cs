using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api.Enums;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamsWithMembersAndCompletedAssessmentCountTests : BaseV1Test
    {
        private static int _assignTeamId;
        private static int _multiTeamId;
        private static int _enterpriseTeamId;
        private static Guid _teamUid;
        private static Guid _multiTeamUid;
        private static Guid _enterpriseTeamUid;
        private static readonly UserConfig SiteAdminUserConfig = new UserConfig("SA");
        private static User _siteAdminUser; 

        [ClassInitialize]
        public static void GetTeamDetails(TestContext _)
        {
            _siteAdminUser = SiteAdminUserConfig.GetUserByDescription("user 1");
            //Get Team Info
            var allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(
                SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, _siteAdminUser);
            
            _assignTeamId = allTeamsList.GetTeamByName(SharedConstants.RadarTeam).TeamId;
            _multiTeamId = allTeamsList.GetTeamByName(SharedConstants.MultiTeamForGrowthJourney).TeamId;
            _enterpriseTeamId = allTeamsList.GetTeamByName(SharedConstants.EnterpriseTeamForGrowthJourney).TeamId;
            
            //Get Team Uid
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

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<IList<TeamsWithMemberAndCompletedAssessmentCountResponse>>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id));

            // Then 
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                Assert.IsTrue(team.TeamId >= 0, "Team Id is not greater than or equal to zero");
                Assert.IsTrue(team.TeamUid.CompareTo(new Guid()) != 0, "Team Uid is greater than zero");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(team.Name), "Team Name is null");
                Assert.IsTrue(team.TeamMemberCount >= 0, "Team member count is not greater than or equal to zero");

                foreach (var survey in team.SurveysForCompletedAssessments)
                {
                    Assert.IsTrue(!string.IsNullOrWhiteSpace(survey.Name), "Survey Name is invalid");
                    Assert.IsTrue(survey.Id > 0, "Survey id is invalid");
                }
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_TeamId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When 
            var response = await client.GetAsync<IList<TeamsWithMemberAndCompletedAssessmentCountResponse>>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id).AddQueryParameter("limitToChildrenOfThisTeamId", _assignTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                var expectedTeamMembers = memberResponse.Dto.Select(s => s.FirstName + " " + s.LastName + " " + s.Email).ToList();
                var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                Assert.IsTrue(actualTeamMembers.All(a => expectedTeamMembers.Contains(a)), "Member's FirstName and LastName and Email is not not contains on response");
                Assert.AreEqual(_assignTeamId, team.TeamId, "Team Id is not matched");
                Assert.AreEqual(SharedConstants.RadarTeam, team.Name, "Team Name is not matched");
                Assert.AreEqual(_teamUid, team.TeamUid, "Team Uid not matched");
                foreach (var survey in team.SurveysForCompletedAssessments)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.Id, $"{survey.Name + " " + survey.Id} is not available");
                }
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_SurveyId_Ok()
        {
            // Given
            var radarSurvey = new SetupTeardownApi(TestEnvironment)
                .GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();

            // When
            var response = await client.GetAsync<IList<TeamsWithMemberAndCompletedAssessmentCountResponse>>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id).AddQueryParameter("surveyId", radarSurvey.Id));

            // Then 
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                var expectedTeamMembers = memberResponse.Dto.Select(s => s.FirstName + " " + s.LastName + " " + s.Email).ToList();
                var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                Assert.IsTrue(actualTeamMembers.All(a => expectedTeamMembers.Contains(a)), "Member's FirstName and LastName and Email is not not contains on response");
                Assert.AreEqual(_assignTeamId, team.TeamId, "Team Id is not matched");
                Assert.AreEqual(SharedConstants.RadarTeam, team.Name, "Team Name is not matched");
                Assert.AreEqual(_teamUid, team.TeamUid, "Team Uid not matched");
                foreach (var survey in team.SurveysForCompletedAssessments)
                {
                    Assert.AreEqual(radarSurvey.Id, survey.Id, $"{survey.Id} is not available");
                    Assert.AreEqual(radarSurvey.Name, survey.Name, $"{survey.Id} is not available");
                }
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_MultiTeamId_Ok()
        {
            // Given
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When 
            var response = await client.GetAsync<IList<TeamsWithMemberAndCompletedAssessmentCountResponse>>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id).AddQueryParameter("limitToChildrenOfThisTeamId", _multiTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                if (team.TeamId == _assignTeamId)
                {
                    var expectedTeamMembers = memberResponse.Dto.Select(s => s.FirstName + " " + s.LastName + " " + s.Email).ToList();
                    var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    Assert.IsTrue(actualTeamMembers.All(a => expectedTeamMembers.Contains(a)), "Member's FirstName and LastName and Email is not not contains on response");
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
                foreach (var survey in team.SurveysForCompletedAssessments)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.Id, $"{survey.Name + " " + survey.Id} is not available");
                }
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_EnterpriseTeamId_Ok()
        {
            // Given 
            var client = await GetAuthenticatedClient();

            var memberResponse = await client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(_teamUid));
            memberResponse.EnsureSuccess();

            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
                RequestUris.AssessmentSurveysByType(Company.Id).AddQueryParameter("type", AssessmentType.Team));
            assessmentTypeResponse.EnsureSuccess();
            var surveyNameIdList = assessmentTypeResponse.Dto.Select(a => a.Name + " " + a.Id).ToList();

            // When
            var response = await client.GetAsync<IList<TeamsWithMemberAndCompletedAssessmentCountResponse>>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id).AddQueryParameter("limitToChildrenOfThisTeamId", _enterpriseTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,
                "Status codes do not match");
            foreach (var team in response.Dto)
            {
                if (team.TeamId == _assignTeamId)
                {
                    var expectedTeamMembers = memberResponse.Dto.Select(s => s.FirstName + " " + s.LastName + " " + s.Email).ToList();
                    var actualTeamMembers = team.TeamMembers.Select(a => a.FirstName + " " + a.LastName + " " + a.Email).ToList();
                    Assert.IsTrue(actualTeamMembers.All(a => expectedTeamMembers.Contains(a)), "Member's FirstName and LastName and Email is not not contains on response");
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
                foreach (var survey in team.SurveysForCompletedAssessments)
                {
                    Assert.That.ListContains(surveyNameIdList, survey.Name + " " + survey.Id, $"{survey.Name + " " + survey.Id} is not available");
                }
            }
        }

        // BUG 30557
        [TestMethod]
        //[TestCategory("TeamAdmin")]
        [TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_By_CompanyId_NonAssignTeamId_Forbidden()
        {

            // Given
            //Create a team
            var companyAdminUser = new UserConfig("CA").GetUserByDescription("user 1");
            var team = TeamFactory.GetNormalTeam("Team");
            team.Tags.RemoveAll(a => a.Category.Equals("Business Lines"));
            var noAssignTeamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, companyAdminUser).GetAwaiter()
                .GetResult();
            var allTeamsList = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(
                SiteAdminUserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id, _siteAdminUser);
            var nonAssignTeamId = allTeamsList.GetTeamByName(noAssignTeamResponse.Name).TeamId;

            var client = await GetAuthenticatedClient();

            // When
            var response = await client.GetAsync<TeamsWithMemberAndCompletedAssessmentCountResponse>(
                RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id).AddQueryParameter("limitToChildrenOfThisTeamId", nonAssignTeamId));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode,
                "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_Unauthorized()
        {
            // Given
            var client = GetUnauthenticatedClient();

            // When
            var response = await client.GetAsync<TeamsWithMemberAndCompletedAssessmentCountResponse>(RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(Company.Id));

            // Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_Forbidden()
        {
            // Given
            var client = await GetAuthenticatedClient();

            // When 
            var response = await client.GetAsync(RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(SharedConstants.FakeCompanyId));

            // Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status codes do not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public async Task Teams_GetTeamsWithMembersAndCompletedAssessmentCountTests_Get_BadRequest()
        {
            // Given
            var client = await GetAuthenticatedClient();
            
            // When 
            var response = await client.GetAsync(RequestUris.TeamsWithMemberCountAndCompletedAssessmentCount(0));

            // Then 
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status codes do not match");
        }

    }
}