using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoleRequest = AtCommon.Dtos.IndividualAssessments.RoleRequest;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]

    public class CreateParticipantGroupForSpecificParticipantTests : BaseV1Test
    {
        public static Guid AssessmentUid;
        private static AddTeamWithMemberRequest _team;
        private static IndividualAssessmentResponse _assessment;
        private static TeamResponse _teamResponse;
        private readonly CreateTeamMemberTagsRequest ParticipantGroupRequest = new CreateTeamMemberTagsRequest
        {
            AssessmentUid = new Guid(),
            ParticipantUid = new Guid(),
            ParticipantGroups = new List<RoleRequest>
            {
                new RoleRequest
                {
                    Key = null,
                    Tags = new List<TagRoleRequest>
                    {
                        new TagRoleRequest
                        {
                            Id =  0,
                            Name =  null
                        }
                    }
                }
            }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            if (!User.IsBusinessLineAdmin() && !User.IsTeamAdmin() && !User.IsCompanyAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            _team = TeamFactory.GetValidPostTeamWithMember("ParticipantGroup_");
            _teamResponse = setup.CreateTeam(_team, User).GetAwaiter().GetResult();

            //get radar details
            var radarDetails = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

            //get up individual assessment members
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, radarDetails.SurveyId);
            individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
            {
                _teamResponse.Members.FirstOrDefault().CheckForNull("No team members found in the response")
                    .ToAddIndividualMemberRequest()
            };
            individualAssessment.TeamUid = _teamResponse.Uid;
            individualAssessment.Published = true;

            //create individual assessment and get radar details
            _assessment = setup
                .CreateIndividualAssessment(individualAssessment, SharedConstants.IndividualAssessmentType).GetAwaiter()
                .GetResult();
            AssessmentUid = _assessment.AssessmentList[0].AssessmentUid;
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task IndividualAssessment_Participant_Group_Post_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                {"teamUid",_teamResponse.Uid}
            };

            var teamMemberTagResponse =
                await client.GetAsync<CompanyTeamMemberTagResponse>(RequestUris.CompaniesParticipantGroups()
                    .AddQueryParameter(query));
            var teamMemberTags = teamMemberTagResponse.Dto.CompanyTeamMemberTags;

            //When
            var participantGroupRequest =
                IndividualAssessmentFactory.CreateTeamMemberTags(AssessmentUid, _assessment.Participants[0].Uid,
                    teamMemberTags);
            var participantGroupResponse = await client.PostAsync<CreateTeamMemberTagsResponse>(
                RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", Company.Id),
                participantGroupRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.OK, participantGroupResponse.StatusCode, "Status code does not match");
            Assert.AreEqual(participantGroupRequest.AssessmentUid, participantGroupResponse.Dto.AssessmentUid,
                "AssessmentUid does not match");
            Assert.AreEqual(participantGroupRequest.ParticipantUid, participantGroupResponse.Dto.ParticipantUid,
                "ParticipantUid does not match");
            Assert.IsTrue(participantGroupResponse.Dto.ParticipantGroups.Count == 1,
                "Participant Group tags does not available");
            Assert.That.ListsAreEqual(participantGroupRequest.ParticipantGroups.Select(d => d.Key).ToList(),
                participantGroupResponse.Dto.ParticipantGroups.Select(d => d.Key).ToList(), "Key does not match");
            Assert.That.ListsAreEqual(teamMemberTags.Select(a => a.Id.ToString()).ToList(),
                participantGroupResponse.Dto.ParticipantGroups.First().Tags.Select(b => b.Id.ToString()).ToList(),
                "Tags Id does not match");
            Assert.That.ListsAreEqual(teamMemberTags.Select(a => a.Name).ToList(),
                participantGroupResponse.Dto.ParticipantGroups.First().Tags.Select(b => b.Name).ToList(),
                "Tags Name does not match");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task IndividualAssessment_Participant_Group_Post_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var query = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                {"teamUid",_teamResponse.Uid}
            };

            var teamMemberTagResponse =
                await client.GetAsync<CompanyTeamMemberTagResponse>(RequestUris.CompaniesParticipantGroups()
                    .AddQueryParameter(query));
            var teamMemberTags = teamMemberTagResponse.Dto.CompanyTeamMemberTags;

            //When
            var participantGroupRequest =
                IndividualAssessmentFactory.CreateTeamMemberTags(AssessmentUid, Guid.NewGuid(), teamMemberTags);
            var participantGroupResponse = await client.PostAsync(
                RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", Company.Id),
                participantGroupRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, participantGroupResponse.StatusCode, "Status code does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task IndividualAssessment_Participant_Group_Post_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var errorResponseList = new List<string>
            {
                "'Assessment Uid' is not valid",
                "'Participant Uid' is not valid",
            };

            //When
            var participantGroupResponse = await client.PostAsync<IList<string>>(
                RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", Company.Id),
                ParticipantGroupRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, participantGroupResponse.StatusCode,
                "Status code does not match");
            Assert.That.ListsAreEqual(errorResponseList, participantGroupResponse.Dto.ToList(),
                "Error list does not match");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task IndividualAssessment_Participant_Group_Post_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            const int companyId = SharedConstants.FakeCompanyId;

            //When
            var participantGroupResponse = await client.PostAsync(
                RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", companyId),
                ParticipantGroupRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, participantGroupResponse.StatusCode,
                "Status code does not match");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task IndividualAssessment_Participant_Group_Post_UnAuthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var participantGroupResponse = await client.PostAsync(
                RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", Company.Id),
                ParticipantGroupRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, participantGroupResponse.StatusCode,
                "Status code does not match");
        }
    }
}
