using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Public")]
    public class GetTeamAssessmentsTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member")]
        public async Task TeamAssessments_Get_MissingToken_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamAssessments(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task TeamAssessments_Get_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            const string teamName = "Automation Radar Team";
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();
            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not returned in the response");

            // act
            var response = await client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(firstTeam.Uid));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response Status Code does not match.");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.AssessmentName)), "AssessmentName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Status)), "Status is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.SurveyType)), "SurveyType is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.SurveyName)), "SurveyName is null or empty");
            Assert.IsTrue(response.Dto.Any(dto => !string.IsNullOrWhiteSpace(dto.Uid.ToString())), "Uid is null or empty");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task TeamAssessments_Get_InvalidUid_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            var unique = Guid.NewGuid();

            // act
            var response = await client.GetAsync(RequestUris.TeamAssessments(unique));

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response Status Code does not match.");
        }
    }
}