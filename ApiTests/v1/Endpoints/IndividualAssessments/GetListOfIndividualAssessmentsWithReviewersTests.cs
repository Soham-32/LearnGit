using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class GetListOfIndividualAssessmentsWithReviewersTests : BaseV1Test
    {
        private const int InValidBatchId = 28292;

        //200
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Get_Batch_MultipleViewers_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            //create team with member
            var team = TeamFactory.GetValidPostTeamWithMember("GetListOfIA_");
            team.Members.Add(MemberFactory.GetValidPostTeamMember());
            var teamCreateResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            teamCreateResponse.EnsureSuccess();

            //create Viewer
            var companyId = Company.Id;
            var viewerResponse = await client.GetAsync<IList<MemberResponse>>(RequestUris.CompaniesMembers(companyId));
            viewerResponse.EnsureSuccess();
            var listOfViewers = viewerResponse.Dto.Where(v => v.Email.Contains("mem")).ToList();

            //get survey id
            var surveyId = GetIndividualSurveyId();

            //create individual assessment and get members and viewers
            var individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, surveyId);
            individualAssessment.Members = teamCreateResponse.Dto.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            individualAssessment.IndividualViewers = listOfViewers.Take(2).Select(m => m.ToAddUserRequest()).ToList();
            individualAssessment.AggregateViewers = listOfViewers.Skip(individualAssessment.IndividualViewers.Count()).Take(2).Select(m => m.ToAddUserRequest()).ToList();
            individualAssessment.TeamUid = teamCreateResponse.Dto.Uid;

            var iAResponse = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", Company.Id), individualAssessment);
            iAResponse.EnsureSuccess();
            individualAssessment.BatchId = iAResponse.Dto.BatchId;
            var iAResponse2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", Company.Id), individualAssessment);
            iAResponse2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims().AddQueryParameter("companyId", Company.Id), individualAssessment);

            //get list of individual assessments with their reviewers
            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentsIndividualBatch(iAResponse2.Dto.BatchId));

            //then
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Status codes for not match. Expecting {HttpStatusCode.OK}, but receiving {response.StatusCode}");
            Assert.AreEqual(iAResponse2.Dto.BatchId, response.Dto.BatchId, "Batch ids do not match");
            Assert.AreEqual(iAResponse2.Dto.AssessmentName, response.Dto.AssessmentName, "Assessment names do not match");
            foreach (var firstName in response.Dto.Members)
            {
                Assert.IsTrue(individualAssessment.Members.Any(f => f.FirstName == firstName.FirstName), "List of members does not match");
            }
            Assert.AreEqual(response.Dto.AggregateViewers.Count, individualAssessment.AggregateViewers.Count, $"Number of aggregate viewers in list does not match. <response.Dto.AggregateViewers.Count> has {response.Dto.AggregateViewers.Count} and <individualAssessment.AggregateViewers.Count> has {individualAssessment.AggregateViewers.Count}.");
            Assert.AreEqual(response.Dto.IndividualViewers.Count, individualAssessment.IndividualViewers.Count, $"Number of aggregate viewers in list does not match. <response.Dto.IndividualViewers.Count> has {response.Dto.IndividualViewers.Count} and <individualAssessment.IndividualViewers.Count> has {individualAssessment.IndividualViewers.Count}.");

            foreach (var aggregateViewer in response.Dto.AggregateViewers)
            {
                Assert.IsTrue(individualAssessment.AggregateViewers.Any(v => v.Email == aggregateViewer.Email), "List of email for aggregate viewers does not match");
            }
            foreach (var individualViewer in response.Dto.IndividualViewers)
            {
                Assert.IsTrue(individualAssessment.IndividualViewers.Any(v => v.Email == individualViewer.Email), "List of email for individual/aggregate viewers does not match");
            }
        }

        //400
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Assessments_Individual_Get_Batch_BadRequest()
        {
            //given
            var client = await GetAuthenticatedClient();
            const int batchIdNotFound = -1;

            //when
            var response = await client.GetAsync(RequestUris.AssessmentsIndividualBatch(batchIdNotFound));

            //then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, $"Status codes for not match. Expecting {HttpStatusCode.BadRequest}, but receiving {response.StatusCode}");
        }

        //401
        [TestMethod]
        [TestCategory("Member")]
        public async Task Assessments_Individual_Get_Batch_UnauthorizedUser()
        {
            //given
            var client = await GetAuthenticatedClient();
            
            //when
            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentsIndividualBatch(InValidBatchId));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Status codes for not match. Expecting {HttpStatusCode.Unauthorized}, but receiving {response.StatusCode}");
        }

        //401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public async Task Assessments_Individual_Get_Batch_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();
            
            //when
            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentsIndividualBatch(InValidBatchId));
            
            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, $"Status codes for not match. Expecting {HttpStatusCode.Unauthorized}, but receiving {response.StatusCode}");
        }

        //403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Assessments_Individual_Get_Batch_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();
            const int companyId = SharedConstants.FakeCompanyId;

            //when
            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentsIndividualBatch(InValidBatchId).AddQueryParameter("companyId", companyId));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, $"Response status code does not match.Expecting {HttpStatusCode.Forbidden}, but receiving {response.StatusCode}");
        }

        //404
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task Assessments_Individual_Get_Batch_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();
            const int batchNotFound = 10;
             
            //when
            var response = await client.GetAsync<IndividualAssessmentMemberResponse>(RequestUris.AssessmentsIndividualBatch(batchNotFound));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, $"Response status code does not match.Expecting {HttpStatusCode.NotFound}, but receiving {response.StatusCode}");
        }
    }
}