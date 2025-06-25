using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.AhTrial;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using AtCommon.Dtos.CampaignsV2;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AtCommon.Api
{
    public class SetupTeardownApi
    {
        public TestEnvironment TestEnvironment { get; set; }
        public SetupTeardownApi(TestEnvironment testEnvironment)
        {
            TestEnvironment = testEnvironment;
        }

        // Fix for CS1061: Replace 'response.Dto' with proper deserialization or property access.

        public async Task<CompanyResponse> CreateCompany(AddCompanyRequest company, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            var response = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);
            response.EnsureSuccess();

            client.Dispose();

            return response.Dto;
        }

        public AhTrialBaseCompanyResponse CreateAhTrialCompany(AhTrialCompanyRequest ahTrialCompanyRequest)
        {
            var client = ClientFactory.GetUnauthenticatedClient(TestEnvironment.EnvironmentName);

            var response = client.PostAsync<AhTrialBaseCompanyResponse>(RequestUris.AhTrialAddTrialCompany(), ahTrialCompanyRequest).GetAwaiter().GetResult();
            response.EnsureSuccess();

            client.Dispose();

            return response.Dto;
        }

        public async Task DeleteCompany(string companyName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            var companyListResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.Companies());
            var companyToDelete = companyListResponse.Dto.FirstOrDefault(c => c.Name.Equals(companyName));
            if (companyToDelete != null)
                await client.DeleteAsync(RequestUris.CompanyDetails(companyToDelete.Id));

            client.Dispose();
        }

        public async Task<CompanyResponse> GetCompany(string companyName)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            var companyListResponse = await client.GetAsync<IList<CompanyResponse>>(RequestUris.Companies());
            companyListResponse.EnsureSuccess();

            var companyInfo = companyListResponse.Dto.FirstOrDefault(c => c.Name.Equals(companyName));

            client.Dispose();

            return companyInfo;
        }

        public async Task<TeamResponse> CreateTeam(AddTeamWithMemberRequest team, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = await ClientFactory.GetAuthenticatedClient(
            user.Username, user.Password, TestEnvironment.EnvironmentName);

            var response = await client.PostAsync<TeamResponse>(RequestUris.Teams(), team);
            response.EnsureSuccess();

            client.Dispose();

            return response.Dto;
        }

        /// <summary>
        /// Builds a 'TeamResponse' object that includes information from the team profile, team members list, and stakeholders list
        /// </summary>
        /// <param name="teamName">The name of the team you want information about</param>
        /// <param name="user">The user to use in the requests. By default it is 'user 1'. Site/Partner Admin will not work.</param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public TeamResponse GetTeamResponse(string teamName = "", User user = null, int companyId = 0)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (user.IsSiteAdmin() || user.IsPartnerAdmin())
                throw new ArgumentException($"{user.Type:G} is not supported by the /teams endpoints.");

            if (companyId == 0)
            {
                companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            }

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            // team profile
            var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(
            RequestUris.Teams().AddQueryParameter("companyId", companyId)).GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            var teamProfile = teamResponse.Dto.FirstOrDefault(t => t.Name == teamName).CheckForNull();

            // team members
            var memberResponse = client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamProfile.Uid)).GetAwaiter().GetResult();
            memberResponse.EnsureSuccess();
            var memberList = memberResponse.Dto.Select(m => new MemberResponse
            {
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                ExternalIdentifier = m.ExternalIdentifier,
                Uid = m.Uid
            }).ToList();

            // stakeholders
            var stakeholderResponse = client.GetAsync<IList<MemberResponse>>(RequestUris.TeamStakeholder(teamProfile.Uid)).GetAwaiter().GetResult();
            stakeholderResponse.EnsureSuccess();
            var stakeholderList = stakeholderResponse.Dto.Select(s => new StakeholderResponse
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                ExternalIdentifier = s.ExternalIdentifier,
                Uid = s.Uid
            }).ToList();

            return new TeamResponse
            {
                Name = teamProfile.Name,
                Type = teamProfile.Type,
                Members = memberList,
                Stakeholders = stakeholderList,
                Uid = teamProfile.Uid
            };

        }

        public IList<TeamProfileResponse> GetTeamProfileResponse(string teamName = "", User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (user.IsSiteAdmin() || user.IsPartnerAdmin())
                throw new ArgumentException($"{user.Type:G} is not supported by the /teams endpoints.");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                                      .GetAwaiter().GetResult();
            var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("teamName", teamName))
                                     .GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            return teamResponse.Dto;
        }

        public IList<TeamV2Response> GetTeamWithTeamMemberResponse(int teamId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                .GetAwaiter().GetResult();
            var teamResponse = client.GetAsync<IList<TeamV2Response>>(RequestUris.PulseAssessmentV2TeamMtEtMembers(teamId).AddQueryParameter("companyId", companyId))
                .GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            return teamResponse.Dto;
        }

        public SelectedParticipantsResponse GetPulseParticipantsResponse(int pulseAssessmentId, int teamId, GetParticipantsRequest roleFilterRequest = null, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                .GetAwaiter().GetResult();

            roleFilterRequest ??= new GetParticipantsRequest();
            var query = new Dictionary<string, object>
            {
                { "companyId", companyId },
                { "pulseAssessmentId", pulseAssessmentId},
                { "teamId", teamId}
            };

            var response = client.PostAsync<SelectedParticipantsResponse>(
                    RequestUris.PulseAssessmentV2GetParticipants().AddQueryParameter(query), roleFilterRequest).GetAwaiter().GetResult();
            return response.Dto;
        }

        public IList<RoleResponse> GetTeamMemberRoleAndParticipantGroups(int companyId, Guid teamUid, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var query = new Dictionary<string, object>
            {
                { "companyId", companyId },
                {"teamUid", teamUid}
            };

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                    .GetAwaiter().GetResult();
            var teamResponse = client.GetAsync<IList<RoleResponse>>(RequestUris.CompaniesMembersRolesAndParticipantGroups().AddQueryParameter(query))
                .GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            return teamResponse.Dto;
        }

        public TeamMemberResponse AddTeamMembers(Guid teamUid, AddMemberRequest member, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                .GetAwaiter().GetResult();

            var memberResponse = client.PostAsync<TeamMemberResponse>(RequestUris.TeamMembers(teamUid), member).GetAwaiter().GetResult();
            memberResponse.EnsureSuccess();

            return memberResponse.Dto;
        }

        public IList<TeamMemberResponse> GetTeamMemberResponse(string teamName, User user = null, int companyId = 0)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (user.IsSiteAdmin() || user.IsPartnerAdmin())
                throw new ArgumentException($"{user.Type:G} is not supported by the /teams endpoints.");
            if (companyId == 0)
            {
                companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            }

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName)
                                      .GetAwaiter().GetResult();
            var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(
            RequestUris.Teams().AddQueryParameter("companyId", companyId)).GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            var teamProfile = teamResponse.Dto.FirstOrDefault(t => t.Name == teamName).CheckForNull();
            var memberResponse = client.GetAsync<IList<TeamMemberResponse>>(RequestUris.TeamMembers(teamProfile.Uid))
                                       .GetAwaiter().GetResult();
            memberResponse.EnsureSuccess();
            return memberResponse.Dto;
        }

        public async Task<AssessmentResponse> GetAssessmentResponse(string teamName, string assessment, User user = null, int companyId = 0)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (user.IsSiteAdmin() || user.IsPartnerAdmin())
                throw new ArgumentException($"{user.Type:G} is not supported by the /teams endpoints.");
            if (companyId == 0)
            {
                companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            }
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);
            var teamProfileResponse = client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams().AddQueryParameter("companyId", companyId)).GetAwaiter().GetResult();
            teamProfileResponse.EnsureSuccess();
            var teamProfile = teamProfileResponse.Dto.FirstOrDefault(t => t.Name == teamName).CheckForNull();
            var teamResponse = client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(teamProfile.Uid)).GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            var assessmentProfile = teamResponse.Dto.FirstOrDefault(t => t.AssessmentName == assessment).CheckForNull();
            var assessmentResponse = client.GetAsync<AssessmentResponse>(RequestUris.Assessments(assessmentProfile.Uid)).GetAwaiter().GetResult();
            assessmentResponse.EnsureSuccess();
            return assessmentResponse.Dto;
        }
        public async Task<IList<AssessmentSummaryResponse>> GetAllAssessments(Guid assessmentUid, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            if (user.IsSiteAdmin() || user.IsPartnerAdmin())
                throw new ArgumentException($"{user.Type:G} is not supported by the /teams endpoints.");

            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            var teamResponse = client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(assessmentUid)).GetAwaiter().GetResult();
            teamResponse.EnsureSuccess();
            return teamResponse.Dto;
        }

        public async Task<TeamProfileResponse> AddSubteams(Guid multiTeamUid, List<Guid> subteamUids, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = await ClientFactory.GetAuthenticatedClient(
            user.Username, user.Password, TestEnvironment.EnvironmentName);

            var response = await client.PatchAsync<TeamProfileResponse>(RequestUris.TeamSubteams(multiTeamUid), subteamUids);
            response.EnsureSuccess();

            client.Dispose();

            return response.Dto;
        }

        public List<BusinessOutcomeCategoryLabelResponse> GetBusinessOutcomesAllLabels(int companyId)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.GetAsync<List<BusinessOutcomeCategoryLabelResponse>>(
                RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(companyId, 0, 0)).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto;
        }
        public BusinessOutcomesMeetingNotesResponse CreateBusinessOutcomesMeetingNotes(BusinessOutcomesMeetingNotesRequest request,User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.PostAsync<BusinessOutcomesMeetingNotesResponse>(RequestUris.BusinessOutcomeCreateMeetingNotes, request).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto;
        }

        public async Task<ApiResponse<BusinessOutcomeResponse>> CreateBusinessOutcome(CustomBusinessOutcomeRequest businessOutcome, User user, string tagName = "", int teamId = 0)
        {

            // get auth client
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            //get swimlane ID
            var companyId = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName).Id;
            var requestUri = (teamId != 0)
            ? RequestUris.BusinessOutcomeAllSwimlanes(companyId)
            : RequestUris.BusinessOutcomeAllSwimlanes(companyId).AddQueryParameter("teamId", teamId);
            var swimlanesResponse = await client.GetAsync<IList<BusinessOutcomeSwimlaneResponse>>(requestUri);
            swimlanesResponse.EnsureSuccess();
            var swimLane = swimlanesResponse.Dto.FirstOrDefault(s => s.Title == businessOutcome.SourceCategoryName);
            if (swimLane != null) businessOutcome.SwimlaneId = swimLane.SwimlaneId;

            if (tagName.Length > 0)
            {
                //Getting Tag Id from Tag Name
                var labelResponse = await client.GetAsync<List<BusinessOutcomeCategoryLabelRequest>>(RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(businessOutcome.CompanyId));
                labelResponse.EnsureSuccess();

                BusinessOutcomeTagRequest tag = null;
                foreach (var t in labelResponse.Dto)
                {
                    tag = t.Tags.FirstOrDefault(x => x.Name.Equals(tagName));
                    if (tag != null)
                    {
                        break;
                    }
                }
                businessOutcome.Tags.Add(tag);
            }

            var response = await client.PostAsync<BusinessOutcomeResponse>(RequestUris.BusinessOutcomePost(), businessOutcome);
            response.EnsureSuccess();

            client.Dispose();

            return response;
        }

        public BusinessOutcomeResponse CreateBusinessOutcome(BusinessOutcomeRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.PostAsync<BusinessOutcomeResponse>(RequestUris.BusinessOutcomePost(), request)
                .GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }
        public BusinessOutcomeMeetingNoteAttachmentResponse CreateBusinessOutcomeMeetingNoteAttachment(HttpContent formData, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.PostAsync(RequestUris.BusinessOutcomeUploadMeetingNotesAttachments, formData).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var result = JsonConvert.DeserializeObject<BusinessOutcomeMeetingNoteAttachmentResponse>(jsonString);

            return result;
        }

        public List<UpdateBusinessOutcomeChecklistItemRequest> CreateChecklistItemRequest(int companyId, Guid businessOutcomeUid, int numberOfChecklist = 1, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var businessOutcomeResponse = new List<UpdateBusinessOutcomeChecklistItemRequest>();
            for (var i = 0; i < numberOfChecklist; i++)
            {
                var request = BusinessOutcomesFactory.GetChecklistItemRequest();
                var response = client.PostAsync<UpdateBusinessOutcomeChecklistItemRequest>(RequestUris.BusinessOutcomesCreateChecklist(companyId, businessOutcomeUid), request)
                    .GetAwaiter().GetResult();
                response.EnsureSuccess();
                businessOutcomeResponse.Add(response.Dto);
            }

            return businessOutcomeResponse;
        }

        public List<BusinessOutcomeCategoryLabelResponse> GetBusinessOutcomesLabels(int companyId)
        {
            //Authentication
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.GetAsync<List<BusinessOutcomeCategoryLabelResponse>>(
                    RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(companyId)).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto;
        }

        public List<BusinessOutcomeCategoryLabelResponse> AddBusinessOutcomesLabels(
            List<BusinessOutcomeCategoryLabelRequest> labels, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            // get the existing labels
            var existingLabelsResponse = client.GetAsync<List<BusinessOutcomeCategoryLabelResponse>>(
                RequestUris.BusinessOutcomeGetAllCategoryLabelsByCompanyId(labels.First().CompanyId, 0, 0)).GetAwaiter().GetResult();
            existingLabelsResponse.EnsureSuccess();
            // convert to requests and filter out temp labels
            var automationLabels = existingLabelsResponse.Dto.Where(m => m.Name.Contains("Automation"))
                .Select(l => l.ToRequest()).ToList();
            // add the new labels
            var newAndExisting = automationLabels.Concat(labels);
            var newLabelsResponse = client.PostAsync<List<BusinessOutcomeCategoryLabelResponse>>(
                RequestUris.BusinessOutcomeCreateBusinessOutcomeCategoryLabelsAndTags(labels.First().CompanyId, 0), newAndExisting).GetAwaiter().GetResult();
            newLabelsResponse.EnsureSuccess();
            return newLabelsResponse.Dto;
        }

        public List<CustomFieldResponse> AddBusinessOutcomesCustomFields(int companyId, List<CustomFieldRequest> request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.PostAsync<List<CustomFieldResponse>>(
                RequestUris.BusinessOutcomesCreateCustomFields(companyId),
                    request).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto;

        }

        public BusinessOutcomeLinkResponse AddBusinessOutcomesExternalLink(Guid businessOutcomeUid, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var request = BusinessOutcomesFactory.GetBusinessOutcomeLinkRequest(businessOutcomeUid);

            var response = client.PostAsync<BusinessOutcomeLinkResponse>(
                RequestUris.BusinessOutcomeLinkPost(),
                request).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto;
        }

        public async Task<ReviewerResponse> CreateReviewer(CreateReviewerRequest reviewer, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            var response = await client.PostAsync<ReviewerResponse>(RequestUris.AssessmentIndividualReviewer(), reviewer);
            response.EnsureSuccess();

            client.Dispose();

            return response.Dto;
        }

        public async Task<IndividualAssessmentResponse> CreateIndividualAssessment(
        CreateIndividualAssessmentRequest individualAssessment, string surveyName, User user = null)
        {
            // authenticate
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = await ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName);

            // find the Id for the survey
            var assessmentTypeResponse = await client.GetAsync<IList<TypesResponse>>(
            RequestUris.AssessmentSurveysByType(individualAssessment.CompanyId)
            .AddQueryParameter("type", AssessmentType.Individual.ToString("D")));
            assessmentTypeResponse.EnsureSuccess();
            var survey = assessmentTypeResponse.Dto.FirstOrDefault(t => t.Name == surveyName);
            if (survey == null) { throw new Exception($"SurveyId not found for survey <{surveyName}>"); }

            individualAssessment.SurveyTypeId = survey.Id;

            // create the assessment
            var response = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", individualAssessment.CompanyId), individualAssessment);
            response.EnsureSuccess();
            individualAssessment.BatchId = response.Dto.BatchId;
            var response2 = await client.PostAsync<IndividualAssessmentResponse>(RequestUris.IndividualAssessments().AddQueryParameter("companyId", individualAssessment.CompanyId), individualAssessment);
            response2.EnsureSuccess();
            await client.PostAsync<string>(RequestUris.IndividualAssessmentsEmailsAndClaims().AddQueryParameter("companyId", individualAssessment.CompanyId), individualAssessment);

            client.Dispose();

            return response.Dto;
        }

        public List<CreateTeamMemberTagsResponse> AddParticipantGroup(int companyId, Guid teamUid, IndividualAssessmentResponse assessment, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var participantGroupResponse = new List<CreateTeamMemberTagsResponse>();

            var query = new Dictionary<string, object>
            {
                {"companyId", companyId },
                {"teamUid", teamUid}
            };

            var teamMemberTagResponse = client.GetAsync<CompanyTeamMemberTagResponse>(RequestUris.CompaniesParticipantGroups().AddQueryParameter(query)).GetAwaiter().GetResult();
            var teamMemberTagsList = teamMemberTagResponse.Dto.CompanyTeamMemberTags;

            for (var i = 0; i < assessment.AssessmentList.Count; i++)
            {
                var teamMemberTags = new List<CompanyTeamMemberTags>() { teamMemberTagsList[i] };

                var participantGroupRequest = IndividualAssessmentFactory.CreateTeamMemberTags(assessment.AssessmentList[i].AssessmentUid, assessment.AssessmentList[i].MemberId, teamMemberTags);
                var participantGroups = client.PostAsync<CreateTeamMemberTagsResponse>
                    (RequestUris.AssessmentIndividualParticipantGroup().AddQueryParameter("companyId", companyId), participantGroupRequest).GetAwaiter().GetResult();
                participantGroups.EnsureSuccess();

                participantGroupResponse.Add(participantGroups.Dto);
            }

            client.Dispose();
            return participantGroupResponse;
        }


        public CompanyHierarchyResponse GetCompanyHierarchy(int companyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.GetAsync<CompanyHierarchyResponse>(
            RequestUris.CompaniesGetTeamHierarchyByCompanyId(companyId)).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }
        public async Task<IList<TeamsWithMembersAndSurveysResponse>> GetTeamWithMemberCountResponse(int companyId, int teamId = 0, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var queryString = new Dictionary<string, object> { { "teamId", teamId } };

            var teamWithMember = await client.GetAsync<IList<TeamsWithMembersAndSurveysResponse>>(
            RequestUris.TeamsWithMembersAndSurveysCount(companyId).AddQueryParameter(queryString));
            var response = teamWithMember.Dto;
            teamWithMember.EnsureSuccess();
            return response;
        }

        public async Task<IList<RadarCompetenciesResponse>> GetRadarCompetenciesResponse(int companyId, int surveyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var queryString = new Dictionary<string, object>
            {
                {"surveyId", surveyId}
            };
            var targetCompetencies = await client.GetAsync<IList<RadarCompetenciesResponse>>(
                RequestUris.RadarCompetencies(companyId).AddQueryParameter(queryString));
            targetCompetencies.EnsureSuccess();
            return targetCompetencies.Dto;
        }

        public async Task<IList<GrowthPlanStatusResponse>> GetStatusResponse(User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = await client.GetAsync<IList<GrowthPlanStatusResponse>>(
            RequestUris.GrowthPlanStatus());
            response.EnsureSuccess();
            return response.Dto;
        }

        public RadarResponse GetRadar(int companyId, string radarName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.GetAsync<IList<RadarResponse>>(RequestUris.RadarsByCompany(companyId)).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto.FirstOrDefault(r => r.Name == radarName)
            .CheckForNull($"<{radarName}> was not found in the response.");
        }

        public TypesResponse GetSurveyType(int companyId, int assessmentType, string surveyTypeName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.GetAsync<IList<TypesResponse>>(
            RequestUris.AssessmentSurveysByType(companyId).AddQueryParameter("type", assessmentType)).GetAwaiter().GetResult();
            response.EnsureSuccess();

            return response.Dto.FirstOrDefault(r => r.Name == surveyTypeName)
            .CheckForNull($"<{surveyTypeName}> was not found in the response.");
        }
        public async Task<List<AssessmentsResultsResponse>> GetAssessmentsResults(string teamName, User user = null, AssessmentsResultsResponse assessmentName = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var teamsResponse = await client.GetAsync<IList<TeamProfileResponse>>(RequestUris.Teams());
            teamsResponse.EnsureSuccess();

            var firstTeam = teamsResponse.Dto.FirstOrDefault(team => team.Name == teamName);
            if (firstTeam == null) throw new Exception($"<{teamName}> was not found in the response");
            var teamAssessmentsResponse = await client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(firstTeam.Uid));
            teamAssessmentsResponse.EnsureSuccess();

            var assessmentsResultsResponses = new List<AssessmentsResultsResponse>();
            if (assessmentName != null)
            {
                var assessmentUid = teamAssessmentsResponse.Dto.First(a => a.AssessmentName.Equals(assessmentName)).Uid;
                assessmentsResultsResponses.Add(client.GetAsync<AssessmentsResultsResponse>(RequestUris.AssessmentsResults(assessmentUid)).Result.Dto);
            }
            else
            {
                var assessmentUids = teamAssessmentsResponse.Dto.Select(a => a.Uid);
                foreach (var assessmentUid in assessmentUids)
                {
                    var assessmentResultResponse = client.GetAsync<AssessmentsResultsResponse>(RequestUris.AssessmentsResults(assessmentUid));
                    assessmentsResultsResponses.Add(assessmentResultResponse.Result.Dto);
                }
            }
            return assessmentsResultsResponses;
        }

        public MemberResponse GetCompanyMember(int companyId, string email, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.GetAsync<IList<MemberResponse>>(
            RequestUris.CompaniesMembers(companyId)).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto.FirstOrDefault(m => m.Email == email).CheckForNull(($"<{email}> was not found in the response"));
        }

        public CreatePulseAssessmentResponse CreatePulseAssessmentV2(SavePulseAssessmentV2Request request, int companyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client
                .PostAsync<CreatePulseAssessmentResponse>(
                    RequestUris.PulseAssessmentV2SavePulseAssessment().AddQueryParameter("companyId", companyId),
                    request).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }

        public RadarQuestionDetailsV2Response GetRadarQuestionDetailsV2(int companyId, int teamId, int surveyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var radarQuestions = client.GetAsync<RadarQuestionDetailsV2Response>(RequestUris.PulseAssessmentV2RadarQuestions().AddQueryParameter("companyId", companyId
                ).AddQueryParameter("teamId", teamId).AddQueryParameter("surveyId", surveyId)).GetAwaiter().GetResult();
            radarQuestions.EnsureSuccess();

            return radarQuestions.Dto;
        }

        public RadarQuestionDetailsResponse GetRadarQuestions(int companyId, int surveyId)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var radarQuestions = client.GetAsync<RadarQuestionDetailsResponse>(RequestUris.RadarQuestions(companyId)
            .AddQueryParameter("surveyId", surveyId)).GetAwaiter().GetResult();
            radarQuestions.EnsureSuccess();

            return radarQuestions.Dto;
        }

        public RadarDetailResponse GetRadarDetails(int companyId, string assessmentType)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var radarResponse = client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetails(companyId)).GetAwaiter().GetResult();
            return radarResponse.Dto.FirstOrDefault(d => d.Name == assessmentType);
        }

        public RadarDetailResponse GetRadarDetailsBySurveyId(int companyId, int surveyId)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var radarResponse = client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(companyId, surveyId)).GetAwaiter().GetResult();
            return radarResponse.Dto.FirstOrDefault();
        }

        public List<CreateCardResponse> CreateIdeaboardCardForIndividualAssessment(CreateIndividualAssessmentRequest assessmentResponse, int companyId, Guid assessmentUid, User user = null, bool dimension = false, int noOfCards = 1, string text = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory
                .GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter()
                .GetResult();

            var radarResponse = client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetailsSurveyId(companyId, assessmentResponse.SurveyTypeId)).GetAwaiter().GetResult();
            radarResponse.EnsureSuccess();

            var dimensionId = radarResponse.Dto.FirstOrDefault().CheckForNull().Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull()
                .DimensionId;
            var columnName = radarResponse.Dto.FirstOrDefault().CheckForNull().Dimensions.FirstOrDefault(d => d.DimensionId == dimensionId).CheckForNull()
                .Name;
            var radarName = radarResponse.Dto.FirstOrDefault().CheckForNull().Name;

            if (dimension)
            {
                dimensionId = null;
                columnName = "Notes";
            }

            //create card
            var cardRequest = IdeaboardFactory.GetCard(assessmentUid, dimensionId, columnName, radarName, noOfCards, text);
            var cardResponse = new List<CreateCardResponse>();

            //act
            foreach (var response in cardRequest.Select(card => client
                         .PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), card).GetAwaiter().GetResult()))
            {
                response.EnsureSuccess();
                cardResponse.Add(response.Dto);
            }
            return cardResponse;
        }

        public List<CreateCardResponse> CreateIdeaboardCardForTeamAssessment(int companyId, RadarDetailResponse radarSurveyDetails, Guid assessmentUid, bool dimension = false, bool allDimensions = false, User user = null, int noOfCards = 1, string text = null, string teamName = "Automation Normal Team")
        {

            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var dimensionId = radarSurveyDetails.Dimensions.FirstOrDefault(d => d.Name != "Finish").CheckForNull().DimensionId;
            var columnName = radarSurveyDetails.Dimensions.FirstOrDefault(d => d.DimensionId == dimensionId).CheckForNull().Name;
            var radarName = radarSurveyDetails.Name;

            //create card in 'Notes' column
            if (dimension)
            {
                dimensionId = null;
                columnName = "Notes";
            }

            // create card on all dimensions
            if (allDimensions)
            {
                var allDimensionsIds = radarSurveyDetails.Dimensions.Where(a => a.Name != "Finish").Select(d => d.DimensionId)
                    .ToList();

                var cardsResponse = new List<CreateCardResponse>();
                foreach (var dimensionsId in allDimensionsIds)
                {
                    columnName = radarSurveyDetails.Dimensions.FirstOrDefault(d => d.DimensionId == dimensionsId).CheckForNull().Name;
                    var cardsRequest = IdeaboardFactory.GetCard(assessmentUid, dimensionsId, columnName, radarName,
                        noOfCards, text);
                    foreach (var response in cardsRequest.Select(cards => client
                                 .PostAsync<CreateCardResponse>(
                                     RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), cards)
                                 .GetAwaiter().GetResult()))
                    {
                        response.EnsureSuccess();
                        cardsResponse.Add(response.Dto);
                    }
                }

                return cardsResponse;
            }

            //create card
            var cardRequest = IdeaboardFactory.GetCard(assessmentUid, dimensionId, columnName, radarName, noOfCards, text);
            var cardResponse = new List<CreateCardResponse>();

            //act
            foreach (var response in cardRequest.Select(card => client.PostAsync<CreateCardResponse>(RequestUris.IdeaboardCard().AddQueryParameter("companyId", companyId), card).GetAwaiter().GetResult()))
            {
                response.EnsureSuccess();
                cardResponse.Add(response.Dto);
            }
            return cardResponse;
        }

        public Task<ApiResponse<IdeaBoardResponse>> GetIdeaboardCards(int companyId, Guid assessmentUid, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var ideaboardCards = client.GetAsync<IdeaBoardResponse>(RequestUris.IdeaboardGetCardsByAssessmentUid(assessmentUid).AddQueryParameter("companyId", companyId));

            return ideaboardCards;
        }

        public IdeaBoardResponse SetIdeaboardVotesAllowed(int companyId, Guid assessmentUid, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var setVotesAllowedRequests = IdeaboardFactory.SetVotesAllowedRequest(assessmentUid, companyId);
            var ideaboardVotesAllowed = client.PutAsync<IdeaBoardResponse>(RequestUris.IdeaboardVotesAllowed(), setVotesAllowedRequests);

            return ideaboardVotesAllowed.Result.Dto;
        }

        public CreateGrowthPlanItemResponse CreateIdeaboardGrowthPlanItem(CreateCardResponse card, string growthItemType)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            //create growth plan item
            var gIRequest = IdeaboardFactory.CreateGrowthPlanItem(card.AssessmentUid, card.Card.DimensionId ?? 748,
                growthItemType, card.CheckForNull().Card.ItemId, card.Card.ItemText);

            var response = client.PostAsync<CreateGrowthPlanItemResponse>(RequestUris.IdeaboardGrowthPlanItem(), gIRequest).GetAwaiter().GetResult();

            return response.Dto;
        }

        public async Task<IList<GrowthPlanItemResponse>> GetGrowthPlanItemList(int companyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var growthItemsList = await client.GetAsync<IList<GrowthPlanItemResponse>>(RequestUris.GrowthPlanItems(companyId));
            growthItemsList.EnsureSuccess();

            return growthItemsList.Dto;
        }

        public GrowthPlanItemIdResponse CreateGrowthItem(GrowthPlanItemRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var growthItemResponse = client.PutAsync<GrowthPlanItemIdResponse>(RequestUris.GrowthPlanSave(), request).GetAwaiter().GetResult();

            return growthItemResponse.Dto;
        }

        public void CreateGrowthItemCustomType(SaveCustomGrowthPlanTypesRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var growthItemResponse = client.PutAsync(RequestUris.GrowthPlanCustomTypesSave(), request.ToStringContent()).GetAwaiter().GetResult();
            growthItemResponse.EnsureSuccessStatusCode();

            client.Dispose();
        }

        public async Task<IList<CustomGrowthPlanTypesResponse>> GetGrowthItemCustomType(int companyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var growthItemsList = await client.GetAsync<IList<CustomGrowthPlanTypesResponse>>(RequestUris.GrowthPlanCustomTypesGet().AddQueryParameter("companyId", companyId));
            growthItemsList.EnsureSuccess();

            return growthItemsList.Dto;
        }

        public CompanyCampaignsResponse GetCampaignsDetails(int companyId)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var campaignResponse = client.GetAsync<CompanyCampaignsResponse>(RequestUris.CampaignDetails(companyId)).GetAwaiter().GetResult();
            campaignResponse.EnsureSuccess();
            return campaignResponse.Dto;
        }
        public CreateCampaignResponse CreateCampaign(int companyId, CreateCampaignRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(companyId), request).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }

        public async Task DeleteCampaign(int companyId, List<int> campaignIdList)
        {
            var user = TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            foreach (var campaignId in campaignIdList)
            {
                await client.DeleteAsync(RequestUris.CampaignsV2Delete(companyId, campaignId));
            }
            client.Dispose();
        }

        public SaveAsDraftResponse SetupCampaignResponse(int companyId, int campaignId, SaveAsDraftRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.PatchAsync<SaveAsDraftResponse>(RequestUris.CampaignsV2Setup(companyId, campaignId), request).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }

        public MatchmakingResponse AutoMatchmakingResponse(int companyId, int campaignId, MatchmakingRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            var response = client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(companyId, campaignId), request).GetAwaiter().GetResult();

            response.EnsureSuccess();
            return response.Dto;
        }

        public string LaunchCampaign(int companyId, int campaignId, LaunchCampaignRequest request, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            var response = client.PostAsync<string>(RequestUris.CampaignsV2Launch(companyId, campaignId), request).GetAwaiter().GetResult();
            response.EnsureSuccess();
            return response.Dto;
        }

        public (CreateCampaignResponse createCampaignResponse, SaveAsDraftResponse saveAsDraftResponse) CreateAndSetupCampaign(int companyId, CreateCampaignRequest createRequest, int targetRatio, int numberOfTeams, int numberOfFacilitators, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var client = ClientFactory.GetAuthenticatedClient(user.Username, user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

            // Step 1: Create Campaign
            var createCampaignResponse = client.PostAsync<CreateCampaignResponse>(RequestUris.CampaignsV2(companyId), createRequest).GetAwaiter().GetResult();
            createCampaignResponse.EnsureSuccess();

            // campaign ID from the response
            var campaignId = createCampaignResponse.Dto.Id;

            // Step 2: Auto Matchmaking
            var matchmakingRequest = CampaignFactory.AutoMatchmakingCampaignData(campaignId,
                numberOfTeams, numberOfFacilitators, targetRatio);
            var matchmakingResponse = client.PostAsync<MatchmakingResponse>(RequestUris.CampaignsV2AutoMatchmaking(companyId, campaignId), matchmakingRequest).GetAwaiter().GetResult();
            matchmakingResponse.EnsureSuccess();

            // Step 3: Setup Campaign as Draft
            var saveAsDraftRequest = CampaignFactory.SaveAsDraftCampaignData(matchmakingResponse.Dto.CampaignId, matchmakingRequest.TeamIds, matchmakingRequest.FacilitatorIds, matchmakingResponse.Dto.TeamFacilitatorMap);
            var saveAsDraftResponse = client.PatchAsync<SaveAsDraftResponse>(RequestUris.CampaignsV2Setup(companyId, campaignId), saveAsDraftRequest).GetAwaiter().GetResult();
            saveAsDraftResponse.EnsureSuccess();

            // Step 4: Launch Campaign
            var launchRequest = CampaignFactory.LaunchCampaign(saveAsDraftResponse.Dto.CampaignId);
            var launchResponse = client.PostAsync<string>(RequestUris.CampaignsV2Launch(companyId, campaignId), launchRequest).GetAwaiter().GetResult();
            launchResponse.EnsureSuccess();

            // Return the campaign response
            // return createCampaignResponse.Dto;
            return (createCampaignResponse.Dto, saveAsDraftResponse.Dto);
        }
    }
}