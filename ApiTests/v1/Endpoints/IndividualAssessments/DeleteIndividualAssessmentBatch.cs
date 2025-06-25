using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.IndividualAssessments
{
    [TestClass]
    [TestCategory("TalentDevelopment")]
    public class DeleteIndividualAssessmentBatch : BaseV1Test
    {
        private const int InValidBatchNo = 2987;
        private static bool _classInitFailed;
        public static Guid AssessmentUid;
        private static CreateIndividualAssessmentRequest _individualAssessment;
        private static SetupTeardownApi _setup;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                if (User.IsMember()) return;
                _setup = new SetupTeardownApi(TestEnvironment);

                var teams = _setup.GetTeamResponse(SharedConstants.GoiTeam);
                var radarDetails = _setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.IndividualAssessmentSurveyId);

                //get up individual assessment members
                _individualAssessment = IndividualAssessmentFactory.GetValidIndividualAssessment(Company.Id, User.CompanyName, radarDetails.SurveyId);
                _individualAssessment.Members = new List<IndividualAssessmentMemberRequest>
                {
                    teams.Members.FirstOrDefault().CheckForNull("No team members found in the response")
                        .ToAddIndividualMemberRequest()
                };
                _individualAssessment.TeamUid = teams.Uid;
                _individualAssessment.Published = true;

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        //200
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Batch_Delete_Success()
        {
            VerifySetup(_classInitFailed);

            //given
            var client = await GetAuthenticatedClient();

            //when
            //Create individual assessment and get radar details
            var createIndividualAssessment =
                _setup.CreateIndividualAssessment(_individualAssessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            
            //Delete individual assessment
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(createIndividualAssessment.BatchId));

            //then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response status code does not match.");

            //Get assessment list from company
            var companyAssessmentResponse = await client.GetAsync<CompanyAssessmentsResponse>(RequestUris.CompaniesAssessments(Company.Id).AddQueryParameter("currentPage",0));
            
            Assert.That.ListNotContains(companyAssessmentResponse.Dto.Assessments.Select(a => a.AssessmentName).ToList(), createIndividualAssessment.AssessmentName,$"Individual assessment {createIndividualAssessment.AssessmentName} - is present");
            
        }

        //401
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task Assessments_Individual_Batch_Delete_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //when
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(InValidBatchNo));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code does not match");
        }

        //401
        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Batch_Delete_UnauthorizedUser()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(InValidBatchNo));

            //then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Response status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("Member"), TestCategory("OrgLeader")]
        public async Task Assessments_Individual_Batch_Delete_Forbidden()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(InValidBatchNo));

            //then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Response status code does not match");
        }

        //404
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public async Task Assessments_Individual_Batch_Delete_NotFound()
        {
            //given
            var client = await GetAuthenticatedClient();

            //when
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(InValidBatchNo));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response status code does not match");
        }

        //404
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task Assessments_Individual_Batch_Delete_Success_ReDelete_NotFound()
        {
            VerifySetup(_classInitFailed);

            //given
            var client = await GetAuthenticatedClient();

            //when
            //Create individual assessment and get radar details
            var createIndividualAssessment =
                _setup.CreateIndividualAssessment(_individualAssessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

            //Delete individual assessment
            var deleteResponse = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(createIndividualAssessment.BatchId));
            deleteResponse.EnsureSuccessStatusCode();

            //ReDelete individual assessment
            var response = await client.DeleteAsync(RequestUris.AssessmentsIndividualBatch(createIndividualAssessment.BatchId));

            //then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Response status code does not match");
        }
    }
}
