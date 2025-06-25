using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetRadarQuestionsTests : PulseApiTestBase
    {
        private static List<string> _errorMessages;
        private static IEnumerable<string> _workType;
        private static IList<TeamProfileResponse> _teams;
        private static TeamHierarchyResponse _multiTeams;
        private static TeamHierarchyResponse _enterpriseTeams;
        private static RadarDetailResponse _radarResponse;
        private static Dictionary<string, object> _queryParameters;
        private static List<int> _excludedByCompanyQuestionIds;
        private static List<string> _excludedWorkTypeQuestionIds;
        private static List<string> _limitedToOtherCompanyQuestionIds;
        private static List<string> _limitedToOtherWorkTypeTagQuestionIds;
        private static List<string> _allExcludedByWorkTypeAndCompanyQuestionsList;
        private static List<string> _allExcludedAndLimitedQuestionsList;
        private static List<string> _expectedQuestionIds;
        private static List<string> _actualQuestionsIds;
        private static IEnumerable<QuestionDetailResponse> _radarQuestions;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("CA");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            var setupApi = new SetupTeardownApi(TestEnvironment);

            var user = User;
            if (User.IsSiteAdmin() || User.IsPartnerAdmin() || User.IsMember())
            {
                user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            //Get team profile 
            _teams = setupApi.GetTeamProfileResponse(SharedConstants.UpdateTeam, user);
            _multiTeams = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            _enterpriseTeams = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.EnterpriseTeam);
            _workType = _teams.First().TeamTags.Where(d => d.Category == "Work Type").Select(d => d.Tags.First()).ToList();

            //Get radar details
            _radarResponse = setupApi.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.AtTeamHealth3SurveyId);
            _radarQuestions = _radarResponse.Dimensions.Where(a => a.Name != "Finish").SelectMany(b => b.Subdimensions)
               .SelectMany(c => c.Competencies).SelectMany(e => e.Questions).ToList();

            // List of individually excluded Question Ids by WorkType and Companies
            _excludedWorkTypeQuestionIds = _radarQuestions
               .Where(f => f.ExcludeWorkType && f.WorkTypes.Any(d => d.Name == _workType.First()))
               .Select(d => d.QuestionId.ToString()).ToList();
            _excludedByCompanyQuestionIds = _radarQuestions
               .Where(a => a.ExcludeCompany && a.Companies.Any(q => q.Id == Company.Id)).Select(s => s.QuestionId)
               .ToList();

            // List of individually limited to QuestionsIDs by other WorkType and Companies 
            _limitedToOtherCompanyQuestionIds = _radarQuestions
               .Where(a => !a.ExcludeCompany && a.Companies.Any(q => q.Id != Company.Id))
               .Select(s => s.QuestionId.ToString()).ToList();
            _limitedToOtherWorkTypeTagQuestionIds = _radarQuestions
               .Where(d => d.WorkTypes.Count != 0 && d.WorkTypes.All(a => a.Name != _workType.First()))
               .Select(s => s.QuestionId.ToString())
               .ToList();

            // List of excluded Question Ids by WorkType and Companies in group
            _allExcludedByWorkTypeAndCompanyQuestionsList = _radarQuestions
               .Where(f => f.ExcludeWorkType && f.WorkTypes.Any(d => d.Name == _workType.First()) ||
                           f.ExcludeCompany && f.Companies.Any(q => q.Id == Company.Id))
               .Select(s => s.QuestionId.ToString()).ToList();

            // List of all excluded and limited to QuestionsIDs by other WorkType, Companies in group
            _allExcludedAndLimitedQuestionsList = _radarQuestions.Where(a =>
                   !a.ExcludeCompany && a.Companies.Any(q => q.Id != Company.Id) ||
                   a.WorkTypes.Count != 0 && a.WorkTypes.All(c => c.Name != _workType.First()))
               .Select(s => s.QuestionId.ToString()).ToList();

            _allExcludedAndLimitedQuestionsList.AddRange(_allExcludedByWorkTypeAndCompanyQuestionsList);

            _expectedQuestionIds = _radarQuestions.Select(d => d.QuestionId.ToString()).Except(_allExcludedAndLimitedQuestionsList).ToList();

            _queryParameters = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", _teams.First().TeamId },
                { "surveyId", _radarResponse.SurveyId }
            };

            _errorMessages = new List<string>()
            {
                "'Company Id' is not valid",
                "'Team Id' is not valid",
                "'Survey Id' is not valid"
            };
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_Team_RadarQuestions_Success()
        {
            //given
            var client = await GetAuthenticatedClient();

            //When
            var assessmentTypesResponse =
                await client.GetAsync<RadarQuestionDetailsV2Response>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(_queryParameters));

            //Then
            _actualQuestionsIds = assessmentTypesResponse.Dto.Dimensions.SelectMany(a => a.SubDimensions)
                .SelectMany(b => b.Competencies).SelectMany(c => c.Questions).Select(d => d.QuestionId.ToString()).ToList();

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            ResponseVerification(_expectedQuestionIds, _actualQuestionsIds);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_MultiTeam_RadarQuestions_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queryParameters = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", _multiTeams.TeamId },
                { "surveyId", _radarResponse.SurveyId }
            };

            //When
            var assessmentTypesResponse =
                await client.GetAsync<RadarQuestionDetailsV2Response>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameters));

            //Then
            _actualQuestionsIds = assessmentTypesResponse.Dto.Dimensions.SelectMany(a => a.SubDimensions)
              .SelectMany(b => b.Competencies).SelectMany(c => c.Questions).Select(d => d.QuestionId.ToString()).ToList();

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            ResponseVerification(_expectedQuestionIds, _actualQuestionsIds);
        }

        //200
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51203
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_EnterpriseTeam_RadarQuestions_Success()
        {
            //given
            var client = await GetAuthenticatedClient();
            var queryParameters = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", _enterpriseTeams.TeamId },
                { "surveyId", _radarResponse.SurveyId }
            };

            //When
            var assessmentTypesResponse =
                await client.GetAsync<RadarQuestionDetailsV2Response>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameters));

            //Then
            _actualQuestionsIds = assessmentTypesResponse.Dto.Dimensions.SelectMany(a => a.SubDimensions)
                .SelectMany(b => b.Competencies).SelectMany(c => c.Questions).Select(d => d.QuestionId.ToString()).ToList();

            Assert.AreEqual(HttpStatusCode.OK, assessmentTypesResponse.StatusCode, "Status code does not match");
            ResponseVerification(_expectedQuestionIds, _actualQuestionsIds);

        }

        //204
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithFakeSurveyID_NoContent()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", _teams.First().TeamId },
                { "SurveyId", 9999}
            };

            //When
            var assessmentTypesResponse = await client.GetAsync(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.NoContent, assessmentTypesResponse.StatusCode, "Status code does not match");
        }

        //400
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInValidTeamAndSurveyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", Company.Id },
                { "teamId", 0000 },
                {"surveyId", 000}
            };
            var errorMessage = new List<string>() { "'Team Id' is not valid", "'Survey Id' is not valid" };
            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorMessage, assessmentTypesResponse.Dto.ToList());
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInValidCompanyIdAndSurveyId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 000 },
                { "teamId", _teams.First().TeamId },
                {"surveyId", 000}
            };
            var errorMessage = new List<string>() { "'Company Id' is not valid", "'Survey Id' is not valid" };
            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorMessage, assessmentTypesResponse.Dto.ToList());
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInValidCompanyIdAndTeamId_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 000 },
                { "teamId", 0000 },
                {"surveyId", _radarResponse.SurveyId}
            };
            var errorMessage = new List<string>() { "'Company Id' is not valid", "'Team Id' is not valid" };
            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorMessage, assessmentTypesResponse.Dto.ToList());
        }

        //400
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInvalidParameters_BadRequest()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 0 },
                { "teamId", 0000 },
                {"surveyId", 000}
            };

            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.BadRequest, assessmentTypesResponse.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(_errorMessages, assessmentTypesResponse.Dto.ToList());
        }

        //401
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_Unauthorized()
        {
            //given
            var client = GetUnauthenticatedClient();

            //When
            var assessmentTypesResponse =
                await client.GetAsync(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(_queryParameters));

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, assessmentTypesResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithFakeCompanyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", SharedConstants.FakeCompanyId },
                { "teamId", _teams.First().TeamId },
                { "SurveyId", _radarResponse.SurveyId }
            };

            //When
            var assessmentTypesResponse = await client.GetAsync(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, assessmentTypesResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInValidCompanyIdAndSurveyId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 000 },
                { "teamId", _teams.First().TeamId },
                {"surveyId", 000}
            };
            
            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, assessmentTypesResponse.StatusCode, "Status code does not match");
        }

        //403
        [TestMethod]
        [TestCategory("OrgLeader"), TestCategory("PartnerAdmin"), TestCategory("Member")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public async Task PulseAssessmentV2_Get_RadarQuestions_WithInValidCompanyIdAndTeamId_Forbidden()
        {
            //Given
            var client = await GetAuthenticatedClient();
            var queryParameter = new Dictionary<string, object>
            {
                { "companyId", 000 },
                { "teamId", 0000 },
                { "surveyId", _radarResponse.SurveyId }
            };
            
            //When
            var assessmentTypesResponse =
                await client.GetAsync<IList<string>>(RequestUris.PulseAssessmentV2RadarQuestions()
                    .AddQueryParameter(queryParameter));

            //Then
            Assert.AreEqual(HttpStatusCode.Forbidden, assessmentTypesResponse.StatusCode,
                "Status code does not match");
            
        }

        //Assert
            private static void ResponseVerification(List<string> actualQuestionsIds, List<string> expectedQuestionIds)
        {
            foreach (var excludedWorkTypeQuestionId in _excludedWorkTypeQuestionIds)
            {
                Assert.That.ListNotContains(actualQuestionsIds, excludedWorkTypeQuestionId, "Excluded by WorkType questionIds does match");
            }
            foreach (var excludeByCompanyQuestionsId in _excludedByCompanyQuestionIds)
            {
                Assert.That.ListNotContains(actualQuestionsIds, excludeByCompanyQuestionsId, "Exclude by company questionIds does match");
            }
            foreach (var limitedToWorkTypeByOtherTagsQuestionId in _limitedToOtherWorkTypeTagQuestionIds)
            {
                Assert.That.ListNotContains(actualQuestionsIds, limitedToWorkTypeByOtherTagsQuestionId, "Limited to other WorkType questionsIds does match");
            }
            foreach (var limitedToOtherCompanyQuestionId in _limitedToOtherCompanyQuestionIds)
            {
                Assert.That.ListNotContains(actualQuestionsIds, limitedToOtherCompanyQuestionId, "Limited to other company questionIds does match");
            }

            CollectionAssert.AreEquivalent(expectedQuestionIds, actualQuestionsIds, "QuestionIds does not match");
        }
    }
}
