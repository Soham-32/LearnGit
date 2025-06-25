using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    //[TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2UpdateTeamMembersTests : PulseApiTestBase
    {
        private static Guid _teamUid;
        private static User _user;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                _user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var team = TeamFactory.GetNormalTeam("PulseTeam_");
            var teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, _user).GetAwaiter().GetResult();
            _teamUid = teamResponse.Uid;
        }

        //200
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_WithTags_WithOutCompanyId_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var memberRequest = PulseFactory.GetValidPostPulseTeamMember(Tags);
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();

            //when
            var updateResponse =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid), updatedMemberRequest);


            //then
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, updateResponse.Dto.Uid,"Team member Uid does not match");
            Assert.AreEqual(updatedMemberRequest.FirstName, updateResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMemberRequest.LastName, updateResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMemberRequest.Email, updateResponse.Dto.Email, "Email doesn't match");
            Assert.AreEqual(updatedMemberRequest.ExternalIdentifier, updateResponse.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreNotEqual(memberResponse.Dto.Tags.Select(a => a.Tags.First().Id).ToList(),
                updateResponse.Dto.Tags.Select(b => b.Tags.First().Id).ToList(), "List of Tag Ids does match");
            Assert.AreNotEqual(memberResponse.Dto.Tags.Select(c => c.Tags.First().Name).ToList(),
                updateResponse.Dto.Tags.Select(d => d.Tags.First().Name).ToList(), "List of Tag Names does match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_WithTags_WithCompanyId_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            var memberRequest = PulseFactory.GetValidPostPulseTeamMember(Tags);
            var memberResponse =
                await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();

            //when
            var updateResponse =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest);

            //then
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, updateResponse.Dto.Uid, "Team member Uid does not match");
            Assert.AreEqual(updatedMemberRequest.FirstName, updateResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMemberRequest.LastName, updateResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMemberRequest.Email, updateResponse.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMemberRequest.ExternalIdentifier, updateResponse.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreNotEqual(memberResponse.Dto.Tags.Select(a => a.Tags.First().Id).ToList(),
                updateResponse.Dto.Tags.Select(b => b.Tags.First().Id).ToList(), "List of Tag Ids does match");
            Assert.AreNotEqual(memberResponse.Dto.Tags.Select(c => c.Tags.First().Name).ToList(),
                updateResponse.Dto.Tags.Select(d => d.Tags.First().Name).ToList(), "List of Tag Names does match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_WithOutTags_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var memberRequest = PulseFactory.GetValidPostPulseTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();
            updatedMemberRequest.Tags = null;

            //when
            var updateResponse =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest);

            //then
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, updateResponse.Dto.Uid, "Team member Uid does not match");
            Assert.AreEqual(updatedMemberRequest.FirstName, updateResponse.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMemberRequest.LastName, updateResponse.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMemberRequest.Email, updateResponse.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMemberRequest.ExternalIdentifier, updateResponse.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.AreEqual(memberResponse.Dto.Tags.Count, updateResponse.Dto.Tags.Count, "Tag Count does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_WithEmptyArray_Tags_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var memberRequest = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();
            updatedMemberRequest.Tags.Find(x => x.Key == "Role").Tags = new List<TagRoleRequest>();
            updatedMemberRequest.Tags.Find(x => x.Key == "Participant Group").Tags = new List<TagRoleRequest>();

            //when
            var response =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest);

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, response.Dto.Uid, "Team member Uid does not match");
            Assert.AreEqual(updatedMemberRequest.FirstName, response.Dto.FirstName, "FirstName doesn't match");
            Assert.AreEqual(updatedMemberRequest.LastName, response.Dto.LastName, "LastName doesn't match");
            Assert.AreEqual(updatedMemberRequest.Email, response.Dto.Email, "EmailName doesn't match");
            Assert.AreEqual(updatedMemberRequest.ExternalIdentifier, response.Dto.ExternalIdentifier, "ExternalIdentifier doesn't match");
            Assert.That.ListsAreEqual(updatedMemberRequest.Tags.SelectMany(a=>a.Tags).ToList(),response.Dto.Tags, "Tag Names does not match");
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberRequest = MemberFactory.GetValidPostTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();
            updatedMemberRequest.FirstName = null;
            updatedMemberRequest.LastName = null;
            updatedMemberRequest.Email = null;

            var errorMessagesList = new List<string>()
            {
                "'Email' should not be empty.",
                "'First Name' should not be empty.",
                "'Last Name' should not be empty."
            };

            //when
            var response = await client.PutAsync<IList<string>>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest);

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorMessagesList,response.Dto.ToList(),"Error messages does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Put_TeamMember_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            var memberUid = Guid.NewGuid();
            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();

            //when
            var response =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberUid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest);

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_InValidTeamUid_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = Guid.NewGuid();
            var memberRequest = MemberFactory.GetValidPostTeamMember();
            var memberResponse =
                await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();
            
            var updatedMember = PulseFactory.GetValidPutTeamMemberV2();
            updatedMember.FirstName = null;

            //when
            var response =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMember);

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_WithFakeCompanyId_ForBidden()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var memberRequest = MemberFactory.GetValidPostTeamMember();
            var memberResponse =
                await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();

            //when
            var response = await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid)
                        .AddQueryParameter("companyId", SharedConstants.FakeCompanyId), updatedMemberRequest);

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("Member")]
        public async Task PulseAssessmentV2_Put_TeamMember_ByMember_Forbidden()
        {
            //Given

            var memberClient = await ClientFactory.GetAuthenticatedClient(_user.Username, _user.Password, TestEnvironment.EnvironmentName);
            var memberRequest = MemberFactory.GetValidPostTeamMember();
            var memberResponse =
                await memberClient.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(_teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            var client = await GetAuthenticatedClient();
            var updatedMember = PulseFactory.GetValidPutTeamMemberV2();
            
            //when
            var response =
                await client.PutAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2UpdateTeamMember(_teamUid, memberResponse.Dto.Uid).AddQueryParameter("companyId", Company.Id), updatedMember);

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Put_TeamMember_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var memberUid = Guid.NewGuid();
            var updatedMemberRequest = PulseFactory.GetValidPutTeamMemberV2();
            updatedMemberRequest.Tags = null;
            
            //When
            var response =
                await client.PutAsync(RequestUris.TeamMemberUpdate(_teamUid, memberUid).AddQueryParameter("companyId", Company.Id), updatedMemberRequest.ToStringContent());

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code doesn't match");
        }
    }
}
