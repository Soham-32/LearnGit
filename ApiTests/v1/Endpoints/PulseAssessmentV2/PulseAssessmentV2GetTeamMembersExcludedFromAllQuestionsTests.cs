using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetTeamMembersExcludedFromAllQuestionsTests : PulseApiTestBase
    {
        private static string _firstMemberUid;
        private static string _secondMemberUid;
        private static string _thirdMemberUid;
        private static string _fourthMemberUid;
        private static string _fifthMemberUid;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static RadarDetailResponse _radarResponse;
        private static List<int> _allCompetencyIds;
        private static List<int> _excludedByParticipantRoleCompetencyIds;
        private static List<int> _limitedByParticipantRoleCompetencyIds;
        private static List<int> _excludedByParticipantGroupCompetencyIds;
        private static List<int> _limitedByParticipantGroupCompetencyIds;
        private static GetTeamMembersExcludedFromQuestionsRequest _teamMembersAllQuestionsRequest;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupApi = new SetupTeardownApi(TestEnvironment);
            _team = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.UpdateTeam);
            _multiTeam = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            _enterpriseTeam = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam);
            var teamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_team.TeamId,user);

            //Get all memberUids
            _firstMemberUid = teamMemberResponses.First().SelectedParticipants.Where(d => d.FullName().Equals(SharedConstants.PulseMember1)).Select(a => a.Uid).First().ToString();
            _secondMemberUid = teamMemberResponses.First().SelectedParticipants.Where(d => d.FullName().Equals(SharedConstants.PulseMember2)).Select(a => a.Uid).First().ToString();
            _thirdMemberUid = teamMemberResponses.First().SelectedParticipants.Where(d => d.FullName().Equals(SharedConstants.PulseMember3)).Select(a => a.Uid).First().ToString();
            _fourthMemberUid = teamMemberResponses.First().SelectedParticipants.Where(d => d.FullName().Equals(SharedConstants.PulseMember4)).Select(a => a.Uid).First().ToString();
            _fifthMemberUid = teamMemberResponses.First().SelectedParticipants.Where(d => d.FullName().Equals(SharedConstants.PulseMember5)).Select(a => a.Uid).First().ToString();

            //Get radar details
            _radarResponse = setupApi.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.AtTeamHealth3SurveyId);
            var radarQuestions = _radarResponse.Dimensions.Where(a => a.Name != "Finish").SelectMany(b => b.Subdimensions)
               .SelectMany(c => c.Competencies)
               .ToList();

            // Get all CompetencyIds
            _allCompetencyIds = radarQuestions.Select(d => d.CompetencyId).ToList();

            _teamMembersAllQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = _team.TeamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = _allCompetencyIds
            };

            // Get CompetencyIds based on Roles/ Participant Groups
            _excludedByParticipantRoleCompetencyIds = radarQuestions.Where(d => d.Questions.Any(s =>
                s.ExcludeByTag && s.ParticipantTags.All(a =>
                    a.Name == teamMemberResponses.First().SelectedParticipants.First(b => b.FullName().Equals(SharedConstants.PulseMember1)).Tags.First().Tags
                        .First().Name))).Select(d => d.CompetencyId).ToList();

            _limitedByParticipantRoleCompetencyIds = radarQuestions
                .Where(a => a.Questions.Any(b => !b.ExcludeByTag && b.ParticipantTags.Any(c =>
                    c.Name == teamMemberResponses.First().SelectedParticipants.First(e => e.FullName().Equals(SharedConstants.PulseMember1)).Tags
                        .First().Tags
                        .First().Name))).Select(d => d.CompetencyId).ToList();

            _excludedByParticipantGroupCompetencyIds = radarQuestions.Where(d => d.Questions.Any(s =>
                s.ExcludeByTag && s.ParticipantTags.All(a =>
                    a.Name == teamMemberResponses.First().SelectedParticipants.First(b => b.FullName().Equals(SharedConstants.PulseMember4)).Tags
                        .First().Tags
                        .First().Name))).Select(d => d.CompetencyId).ToList();

            _limitedByParticipantGroupCompetencyIds = radarQuestions
                .Where(a => a.Questions.Any(d => !d.ExcludeByTag && d.ParticipantTags.Any(c =>
                    c.Name == teamMemberResponses.First().SelectedParticipants.First(b => b.FullName().Equals(SharedConstants.PulseMember4)).Tags
                        .First().Tags
                        .First().Name))).Select(d => d.CompetencyId).ToList();

        }

        //TeamMemberExcludeFromAllQuestions for Participant Roles
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Team_TeamMemberExcludedFromAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Roles(_team.TeamId, _excludedByParticipantRoleCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_MultiTeam_TeamMemberExcludedFromAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Roles(_multiTeam.TeamId, _excludedByParticipantRoleCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_EnterpriseTeam_TeamMemberExcludedFromAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Roles(_enterpriseTeam.TeamId, _excludedByParticipantRoleCompetencyIds);
        }

        private async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Roles(int teamId, List<int> competencyIds)
        {

            //given
            var client = await GetAuthenticatedClient();
            var teamMembersExcludedFromQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = teamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = competencyIds
            };

            //When
            var teamMemberExcludeFromAllQuestionsResponse = await client.PostAsync<IList<TeamMembersExcludedFromQuestionsResponse>>(RequestUris
                    .PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), teamMembersExcludedFromQuestionsRequest);

            //Then
            if (teamId == _team.TeamId)
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
            else if (teamId == _multiTeam.TeamId)
            {
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First().TeamMembers.All(a => a.IsDisabled), "Excluded Competency is disabled for team member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(b => b.TeamId == _multiTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Excluded Competency is disabled for team member");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
            else
            {
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _enterpriseTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Excluded Competency is disabled for team member");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
        }

        //TeamMemberLimitedToAllQuestions for Participant Roles
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Team_TeamMemberLimitedToAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_Roles(_team.TeamId, _limitedByParticipantRoleCompetencyIds);
        }
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_MultiTeam_TeamMemberLimitedToAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_Roles(_multiTeam.TeamId, _limitedByParticipantRoleCompetencyIds);
        }
        //200
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_EnterpriseTeam_TeamMemberLimitedToAllQuestions_ParticipantRoles_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_Roles(_enterpriseTeam.TeamId, _limitedByParticipantRoleCompetencyIds);
        }

        private async Task PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_Roles(int teamId, List<int> competencyIds)
        {

            //given
            var client = await GetAuthenticatedClient();
            var teamMembersLimitedToQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = teamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = competencyIds
            };

            //When
            var teamMemberLimitedToAllQuestionsResponse = await client.PostAsync<IList<TeamMembersExcludedFromQuestionsResponse>>(RequestUris
                    .PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), teamMembersLimitedToQuestionsRequest);

            //Then
            if (teamId == _team.TeamId)
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMemberLimitedToAllQuestionsResponse.StatusCode, "Status code does not match");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
            else if (teamId == _multiTeam.TeamId)
            {
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(b => b.TeamId == _multiTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Limited Competency is enabled for team member");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
            else
            {
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _enterpriseTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Limited Competency is enabled for team member");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _firstMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _secondMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
        }

        //TeamMemberExcludeFromAllQuestions for Participant Groups
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Team_TeamMemberExcludedFromAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_ParticipantGroup(_team.TeamId, _excludedByParticipantGroupCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_MultiTeam_TeamMemberExcludedFromAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_ParticipantGroup(_multiTeam.TeamId, _excludedByParticipantGroupCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_EnterpriseTeam_TeamMemberExcludedFromAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_ParticipantGroup(_enterpriseTeam.TeamId, _excludedByParticipantGroupCompetencyIds);
        }

        private async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_ParticipantGroup(int teamId, List<int> competencyIds)
        {

            //given
            var client = await GetAuthenticatedClient();
            var teamMembersExcludedFromQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = teamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = competencyIds
            };

            //When
            var teamMemberExcludeFromAllQuestionsResponse = await client.PostAsync<IList<TeamMembersExcludedFromQuestionsResponse>>(RequestUris
                    .PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), teamMembersExcludedFromQuestionsRequest);

            //Then
            if (teamId == _team.TeamId)
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
            else if (teamId == _multiTeam.TeamId)
            {
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First().TeamMembers.All(a => a.IsDisabled));
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(b => b.TeamId == _multiTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Excluded Competency is disabled for team member");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
            else
            {
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _enterpriseTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Excluded Competency is disabled for team member");
                Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Excluded Competency is enabled for first member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Excluded Competency is disabled for second member");
                Assert.IsFalse(teamMemberExcludeFromAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Excluded Competency is disabled for third member");
            }
        }

        //TeamMemberLimitedToAllQuestions for Participant Groups
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Team_TeamMemberLimitedToAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_ParticipantGroup(_team.TeamId, _limitedByParticipantGroupCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_MultiTeam_TeamMemberLimitedToAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_ParticipantGroup(_multiTeam.TeamId, _limitedByParticipantGroupCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_EnterpriseTeam_TeamMemberLimitedToAllQuestions_ParticipantGroups_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_ParticipantGroup(_enterpriseTeam.TeamId, _limitedByParticipantGroupCompetencyIds);
        }

        private async Task PulseAssessmentV2_Post_TeamMemberLimitedToAllQuestions_ParticipantGroup(int teamId, List<int> competencyIds)
        {

            //given
            var client = await GetAuthenticatedClient();
            var teamMembersLimitedToQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = teamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = competencyIds
            };

            //When
            var teamMemberLimitedToAllQuestionsResponse = await client.PostAsync<IList<TeamMembersExcludedFromQuestionsResponse>>(RequestUris
                    .PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), teamMembersLimitedToQuestionsRequest);

            //Then
            if (teamId == _team.TeamId)
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMemberLimitedToAllQuestionsResponse.StatusCode, "Status code does not match");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First().TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
            else if (teamId == _multiTeam.TeamId)
            {
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(b => b.TeamId == _multiTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Limited Competency is enabled for Team member");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
            else
            {
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _enterpriseTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Limited Competency is enabled for Team member");
                Assert.IsFalse(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fourthMemberUid).IsDisabled, "Limited Competency is disabled for first member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _fifthMemberUid).IsDisabled, "Limited Competency is enabled for second member");
                Assert.IsTrue(teamMemberLimitedToAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.First(d => d.MemberUid == _thirdMemberUid).IsDisabled, "Limited Competency is enabled for third member");
            }
        }

        //TeamMemberWithAllQuestions
        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_Team_TeamMemberWithAllQuestions_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberWithAllQuestions(_team.TeamId, _allCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_MultiTeam_TeamMemberWithAllQuestions_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberWithAllQuestions(_multiTeam.TeamId, _allCompetencyIds);
        }

        //200
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_EnterpriseTeam_TeamMemberWithAllQuestions_Success()
        {
            await PulseAssessmentV2_Post_TeamMemberWithAllQuestions(_enterpriseTeam.TeamId, _allCompetencyIds);
        }

        private async Task PulseAssessmentV2_Post_TeamMemberWithAllQuestions(int teamId, List<int> competencyIds)
        {
            //given
            var client = await GetAuthenticatedClient();

            var teamMembersAllQuestionsRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = teamId,
                SurveyId = _radarResponse.SurveyId,
                CompetencyIds = competencyIds
            };

            //When
            var teamMemberWithAllQuestionsResponse =
                await client.PostAsync<IList<TeamMembersExcludedFromQuestionsResponse>>(RequestUris
                    .PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), teamMembersAllQuestionsRequest);

            //Then
            if (teamId == _team.TeamId)
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMemberWithAllQuestionsResponse.StatusCode, "Status code does not match");
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First().TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
            }
            else if (teamId == _multiTeam.TeamId)
            {
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First().TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First(a => a.TeamId == _multiTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
            }
            else
            {
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First().TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First(a => a.TeamId == _team.TeamId).TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
                Assert.IsFalse(teamMemberWithAllQuestionsResponse.Dto.First(a => a.TeamId == _enterpriseTeam.TeamId).TeamMembers.All(a => a.IsDisabled), "Competencies are disabled for Team members");
            }
        }

        //200 - no data request
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_WithOutCompetency_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var noDataRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = _team.TeamId,
                SurveyId = 3,
                CompetencyIds = new List<int> { 0 }
            };

            //When
            var teamMemberExcludeFromAllQuestionsResponse =
                await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), noDataRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
            Assert.IsTrue(teamMemberExcludeFromAllQuestionsResponse.Dto.Count == 0, "Member data does show");
        }

        //400
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var badRequest = new GetTeamMembersExcludedFromQuestionsRequest()
            {
                TeamId = 000,
                SurveyId = 00,
                CompetencyIds = new List<int> { 0 }
            };

            //When
            var teamMemberExcludeFromAllQuestionsResponse =
                 await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                     .AddQueryParameter("companyId", Company.Id), badRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
            Assert.AreEqual("'Team Id' is not valid", teamMemberExcludeFromAllQuestionsResponse.Dto.First(), "Response messages are not the same");
            Assert.AreEqual("'Survey Id' is not valid", teamMemberExcludeFromAllQuestionsResponse.Dto.Last(), "Response messages are not the same");
        }

        //401
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //When
            var teamMemberExcludeFromAllQuestionsResponse =
                await client.PostAsync<string>(RequestUris.PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", Company.Id), _teamMembersAllQuestionsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMemberExcludedFromAllQuestions_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var teamMemberExcludeFromAllQuestionsResponse =
                await client.PostAsync<string>(RequestUris.PulseAssessmentV2TeamMemberExcludeFromAllQuestions()
                    .AddQueryParameter("companyId", SharedConstants.FakeCompanyId), _teamMembersAllQuestionsRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, teamMemberExcludeFromAllQuestionsResponse.StatusCode, "Status code does not match");
        }
    }
}
