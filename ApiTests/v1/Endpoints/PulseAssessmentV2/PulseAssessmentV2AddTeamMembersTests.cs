using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    //[TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2AddTeamMembersTests : PulseApiTestBase
    {
        public static Guid TeamUid;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }
            var team = TeamFactory.GetValidPostTeam("PulseTeam_");
            var teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, user).GetAwaiter().GetResult();
            TeamUid = teamResponse.Uid;
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithRoleAndParticipantGroup_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(
                RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id),
                memberDetails);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, memberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberDetails.FirstName, memberResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(memberDetails.LastName, memberResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(memberDetails.Email, memberResponse.Dto.Email, "Email doesn't match");
            Assert.IsTrue(memberResponse.Dto.Tags.First().Tags.First().Id > 0, "Tag Id value is 0");
            Assert.IsTrue(memberResponse.Dto.Tags.Last().Tags.First().Id > 0, "Tag Id value is 0");
            Assert.IsTrue(!string.IsNullOrEmpty(memberResponse.Dto.Uid.ToString()), "Team member Uid is null");
            Assert.That.ListsAreEqual(memberDetails.Tags.Select(a => a.Tags.First().Name).ToList(),
                memberResponse.Dto.Tags.Select(a => a.Tags.First().Name).ToList(), "Tags names does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [ TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithRole_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var tags = new List<RoleRequest>
            {
                new RoleRequest
                {
                    Key = "Role",
                    Tags = new List<TagRoleRequest>
                    {
                        new TagRoleRequest
                        {
                            Id = 0,
                            Name = "Sales"
                        }
                    }
                }
            };

            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(tags);

            //When
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, memberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberDetails.FirstName, memberResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(memberDetails.LastName, memberResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(memberDetails.Email, memberResponse.Dto.Email, "Email doesn't match");
            Assert.AreEqual(memberDetails.Tags.First().Tags.First().Name, memberResponse.Dto.Tags.First().Tags.First().Name, "Tag Roles name doesn't match");
            Assert.IsTrue(memberResponse.Dto.Tags.First().Tags.First().Id > 0, "Tag Id value is 0");
            Assert.IsTrue(!string.IsNullOrEmpty(memberResponse.Dto.Uid.ToString()), "Team member Uid is null");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithParticipantGroup_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var tags = new List<RoleRequest>
            {
                new RoleRequest
                {
                    Key = "Participant Group",
                    Tags = new List<TagRoleRequest>
                    {
                        new TagRoleRequest
                        {
                            Id = 0,
                            Name = "Technical"
                        }
                    }
                }
            };

            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(tags);

            //When
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, memberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberDetails.FirstName, memberResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(memberDetails.LastName, memberResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(memberDetails.Email, memberResponse.Dto.Email, "Email doesn't match");
            Assert.AreEqual(memberDetails.Tags.Last().Tags.First().Name, memberResponse.Dto.Tags.Last().Tags.First().Name, "Participant group name doesn't match");
            Assert.IsTrue(memberResponse.Dto.Tags.Last().Tags.First().Id > 0, "Tag Id value is 0");
            Assert.IsTrue(!string.IsNullOrEmpty(memberResponse.Dto.Uid.ToString()), "Team member Uid is null");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithOutTags_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember();

            //When
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails);

            //Then
            Assert.AreEqual(HttpStatusCode.Created, memberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberDetails.FirstName, memberResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(memberDetails.LastName, memberResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(memberDetails.Email, memberResponse.Dto.Email, "Email doesn't match");
            Assert.AreEqual(memberResponse.Dto.Tags.Count, 0, "Tags Count does not match");
            Assert.IsTrue(!string.IsNullOrEmpty(memberResponse.Dto.Uid.ToString()), "Team member Uid is null");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var member = PulseFactory.GetValidPostPulseTeamMember(Tags);
            member.FirstName = null;
            member.LastName = null;
            member.Email = null;

            var errorMessagesList = new List<string>()
            {
                "'FirstName' should not be empty",
                "'LastName' should not be empty",
                "'Email' should not be empty."
            };

            //When
            var response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), member);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.That.ListsAreEqual(errorMessagesList, response.Dto.ToList(), "Error messages does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_AlreadyExist_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);
            var errorMessageList = new List<string>
            {
                $"The member {memberDetails.Email} already exists."
            };

            //When
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails);
            memberResponse.EnsureSuccess();

            //Again execute the endpoint
            var member1Response = await client.PostAsync<IList<string>>(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails);

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, member1Response.StatusCode, "Status Code does not match.");
            Assert.That.ListsAreEqual(errorMessageList, member1Response.Dto.ToList(), "Status Code doesn't match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_TeamMember_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);
            
            //When
            var response = await client.PostAsync(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_TeamMember_InValidTeamUid_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = Guid.NewGuid();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var response = await client.PostAsync(RequestUris.PulseAssessmentV2Members(teamUid).AddQueryParameter("companyId", Company.Id), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithEmptyGuidTeamUid_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = new Guid();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var response = await client.PostAsync(RequestUris.PulseAssessmentV2Members(teamUid).AddQueryParameter("companyId", Company.Id), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_TeamMember_InValidCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            const int companyId = SharedConstants.FakeCompanyId;
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var response = await client.PostAsync(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", companyId), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("Member")]
        public async Task PulseAssessmentV2_Post_TeamMember_ByMember_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var memberResponse = await client.PostAsync(RequestUris.PulseAssessmentV2Members(TeamUid).AddQueryParameter("companyId", Company.Id), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, memberResponse.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithEmptyGuidTeamUid_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = new Guid();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var memberResponse = await client.PostAsync(RequestUris.PulseAssessmentV2Members(teamUid).AddQueryParameter("companyId", Company.Id), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, memberResponse.StatusCode, "Status Code doesn't match");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Post_TeamMember_WithoutCompanyId_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberDetails = PulseFactory.GetValidPostPulseTeamMember(Tags);

            //When
            var memberResponse = await client.PostAsync(RequestUris.PulseAssessmentV2Members(TeamUid), memberDetails.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, memberResponse.StatusCode, "Status Code doesn't match");
        }
    }
}


