using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    //[TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetTeamMembersTests : PulseApiTestBase
    {
       private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_GET_TeamMember_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }
            var team = TeamFactory.GetValidPostTeam("PulseTeam_");
            var teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, user);
            var teamUid = teamResponse.Result.Uid;

            var memberRequest = PulseFactory.GetValidPostPulseTeamMember(Tags);
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            //When
            var getMemberResponse = await client.GetAsync<IList<TeamMemberV2Response>>(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getMemberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, getMemberResponse.Dto.First().Uid,  "Team member Uid does not match");
            Assert.AreEqual(memberResponse.Dto.FirstName,getMemberResponse.Dto.First().FirstName,"FirstName does not match");
            Assert.AreEqual(memberResponse.Dto.LastName, getMemberResponse.Dto.First().LastName, "LastName does not match");
            Assert.AreEqual(memberResponse.Dto.Email, getMemberResponse.Dto.First().Email, "Email does not match");
            Assert.That.ListsAreEqual(memberResponse.Dto.Tags.Select(a => a.Tags.First().Name).ToList(),
                getMemberResponse.Dto.First().Tags.Select(b => b.Tags.First().Name).ToList(), "Tags Names does not match");
            Assert.That.ListsAreEqual(memberResponse.Dto.Tags.Select(c => c.Tags.First().Id.ToString()).ToList(),
                getMemberResponse.Dto.First().Tags.Select(d => d.Tags.First().Id.ToString()).ToList(), "Tags IDs does not match");
        }

        //200
        [TestMethod]
        [TestCategory("SiteAdmin"),TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_GET_TeamMember_WithoutTags_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }
            var team = TeamFactory.GetValidPostTeam("PulseTeam_");
            var teamResponse = new SetupTeardownApi(TestEnvironment).CreateTeam(team, user);
            var teamUid = teamResponse.Result.Uid;

            var memberRequest = PulseFactory.GetValidPostPulseTeamMember();
            var memberResponse = await client.PostAsync<TeamMemberV2Response>(RequestUris.PulseAssessmentV2Members(teamUid).AddQueryParameter("companyId", Company.Id), memberRequest);
            memberResponse.EnsureSuccess();

            //When
            var getMemberResponse = await client.GetAsync<IList<TeamMemberV2Response>>(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.OK, getMemberResponse.StatusCode, "Status Code doesn't match");
            Assert.AreEqual(memberResponse.Dto.Uid, getMemberResponse.Dto.First().Uid, "Team member Uid does not match");
            Assert.AreEqual(memberResponse.Dto.FirstName, getMemberResponse.Dto.First().FirstName, "FirstName does not match");
            Assert.AreEqual(memberResponse.Dto.LastName, getMemberResponse.Dto.First().LastName, "LastName does not match");
            Assert.AreEqual(memberResponse.Dto.Email, getMemberResponse.Dto.First().Email, "Email does not match");
            Assert.AreEqual(memberResponse.Dto.Tags.Count,getMemberResponse.Dto.First().Tags.Count,"Tag Counts does not match");
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin") ,TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_GET_TeamMember_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();
            var teamUid = Guid.NewGuid();

            //When
            var getMemberResponse = await client.GetAsync(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, getMemberResponse.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_GET_TeamMember_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = Guid.NewGuid();

            //When
            var getMemberResponse = await client.GetAsync(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, getMemberResponse.StatusCode, "Status Code does not match.");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task PulseAssessmentV2_GET_TeamMember_EmptyGuid_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = new Guid();

            //When
            var getMemberResponse = await client.GetAsync(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, getMemberResponse.StatusCode, "Status Code does not match.");
        }

        //404
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task PulseAssessmentV2_GET_TeamMember_NotFound()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var teamUid = Guid.NewGuid();

            //When
            var getMemberResponse = await client.GetAsync(RequestUris.PulseAssessmentV2Members(teamUid));

            //Then
            Assert.AreEqual(HttpStatusCode.NotFound, getMemberResponse.StatusCode, "Status Code does not match.");
        }
    }
}

