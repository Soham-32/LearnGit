using System;

namespace AtCommon.Api
{
    public class RequestUris
    {
        // Account
        public static string Login() => "v1/account/login";
        public static string AccountGetUserInfo() => "v1/account/GetUserInfo";

        //Ah Trial
        public static string AhTrialAddTrialCompany() => "/v1/agilityhealthtrial/AddTrialCompany";

        //Analytics
        public static string AnalyticsCompanyAssessmentQuarters(int companyId) =>
        $"/analytics/CompanyAssessmentQuarters/{companyId}";
        public static string AnalyticsSyncDateTime(int companyId) => $"analytics/SyncDateTime/{companyId}";
        public static string AnalyticsIndexItems(int companyId) => $"analytics/IndexItems/{companyId}";
        public static string AnalyticsIndexDimensions(int companyId) => $"analytics/IndexDimensions/{companyId}";
        public static string AnalyticsGrowthPlanItems(int companyId) => $"analytics/GrowthPlanItems/{companyId}";
        public static string AnalyticsGrowthPlanItemsDetails(int companyId) =>
        $"analytics/GrowthPlanItemsDetails/{companyId}";
        public static string AnalyticsDimensionsItems(int companyId) => $"analytics/DimensionsItems/{companyId}";
        public static string AnalyticsIndexTeams(int companyId) => $"analytics/IndexTeams/{companyId}";
        public static string AnalyticsOvertimeItems(int companyId) => $"analytics/OvertimeItems/{companyId}";
        public static string AnalyticsRoleAllocationTargets(int companyId, int teamId) =>
        $"analytics/structuralagility/RoleAllocationTargets/{companyId}/{teamId}";
        public static string AnalyticsRoleAllocationTargets(int companyId) =>
        $"analytics/structuralagility/RoleAllocationTargets/{companyId}";
        public static string AnalyticsRoleAllocationAverage(int companyId) =>
        $"analytics/structuralagility/RoleAllocationAverage/{companyId}";
        public static string AnalyticsAgileNonAgileTeams(int companyId) =>
        $"analytics/structuralagility/AgileNonAgileTeams/{companyId}";
        public static string AnalyticsPeopleByRole(int companyId) =>
        $"analytics/structuralagility/PeopleByRole/{companyId}";
        public static string AnalyticsParticipantGroup(int companyId) =>
        $"analytics/structuralagility/ParticipantGroup/{companyId}";
        public static string AnalyticsTeamStability(int companyId) =>
        $"analytics/structuralagility/TeamStability/{companyId}";
        public static string AnalyticsTeamWorkType(int companyId) =>
        $"analytics/structuralagility/TeamWorkTypes/{companyId}";
        public static string AnalyticsTeamRoleAllocationAverage(int companyId) =>
            $"analytics/structuralagility/TeamRoleAllocationAverage/{companyId}";
        public static string AnalyticsRefreshData(int companyId) => $"analytics/RefreshData/{companyId}";

        //Insights
        public static string InsightsPreferences(string preferencesLocation) => $"insights/Preferences/{preferencesLocation}";
        public static string InsightsDashboards() => "insights/Dashboards";
        public static string WidgetPreferences(int companyId) => $"insights/WidgetPreferences/{companyId}";
        public static string InsightsWidgetTheme(int themeId) => $"insights/WidgetTheme/{themeId}";

        // Assessments
        public static string Assessments(Guid assessmentUid) => $"v1/assessments/{assessmentUid}";
        public static string AssessmentsResults(Guid assessmentUid) => $"v1/assessments/{assessmentUid}/results";
        public static string AssessmentsTeamsThatCompletedAssessment = "v1/assessments/teamsthatcompletedassessment";
        public static string AssessmentsIndividualBatch(int batchUid) => $"v1/assessments/individual/batch/{batchUid}";

        public static string AssessmentsRadarByLink(Guid assessmentLink, string employeeHash) =>
        $"v1/assessments/{assessmentLink}/{employeeHash}";
        public static string AssessmentSurveysByType(double companyId) => $"v1/assessments/{companyId}/surveytypes";
        public static string CompleteAssessment(string assessmentKey, string env) =>
        $"https://{env}.agilityinsights.ai/survey/assessment/{assessmentKey}/complete";

        //Pulse
        public static string PulseAssessmentsNames = "v1/assessments/pulse/names";
        public static string PulseAssessmentById(int pulseAssessmentId) => $"v1/assessments/pulse/{pulseAssessmentId}";
        public static string PulseAssessmentSave = "v1/assessments/pulse/save";
        public static string PulseAssessmentResult = "v1/assessments/pulse/results";
        public static string UpsertPulseResults(string pulseKey, string env) =>
            $"https://{env}.agilityinsights.ai/survey/assessment/{pulseKey}/upsertpulseresults";
        public static string CompletePulse(string pulseKey, string env) =>
            $"https://{env}.agilityinsights.ai/survey/assessment/{pulseKey}/completepulse";

        //PulseV2
        public static string PulseAssessmentV2AssessmentTypes() => "/v1/assessments/v2/pulse/assessmentTypes";
        public static string PulseAssessmentV2RadarQuestions() => "/v1/assessments/v2/pulse/radarquestions";
        public static string PulseAssessmentV2TeamMemberExcludeFromAllQuestions() => "/v1/assessments/v2/pulse/teammembersexcludedfromallquestions";
        public static string PulseAssessmentV2Members(Guid teamId) => $"/v1/assessments/v2/pulse/{teamId}/members";
        public static string PulseAssessmentV2UpdateTeamMember(Guid teamId, Guid memberId) => $"/v1/assessments/v2/pulse/{teamId}/member/{memberId}";
        public static string PulseAssessmentV2SavePulseAssessment() => "/v1/assessments/v2/pulse/savepulseassessment";
        public static string PulseAssessmentV2GetPulseAssessment(int pulseAssessmentId) => $"/v1/assessments/v2/pulse/{pulseAssessmentId}";
        public static string PulseAssessmentV2Delete(int pulseAssessmentId) => $"/v1/assessments/v2/pulse/{pulseAssessmentId}";
        public static string PulseAssessmentV2Permissions() => "/v1/assessments/v2/pulse/permissions";
        public static string PulseAssessmentV2Email() => "/v1/assessments/v2/pulse/email";
        public static string PulseAssessmentV2BatchEmail() => "/v1/assessments/v2/pulse/batchemail";
        public static string PulseAssessmentV2TeamMtEtMembers(int teamId) => $"/v1/assessments/v2/pulse/{teamId}/members";
        public static string PulseAssessmentV2GetParticipants() => "/v1/assessments/v2/pulse/participants";

        // Bulk
        public static string Bulk(int companyId) => $"v1/bulk/{companyId}";
        public static string BulkIdentifiers(int companyId) => $"v1/bulk/{companyId}/identifiers";
        public static string BulkTeams(int companyId) => $"v1/bulk/{companyId}/teams";
        public static string BulkMembers(int companyId) => $"v1/bulk/{companyId}/members";
        public static string BulkTeamMemberTags(int companyId) => $"v1/bulk/{companyId}/teammembertags";
        public static string BulkStakeholderTags(int companyId) => $"v1/bulk/{companyId}/stakeholdertags";
        public static string BulkUsers(int companyId) => $"v1/bulk/{companyId}/users";

        //Growth Plan
        public static string GrowthPlanItems(int companyId) => $"/v1/growthplan/items/{companyId}";
        public static string GrowthPlanSave() => "/v1/growthplan/save";
        public static string GrowthPlanStatus() => "/v1/growthplan/growthplanstatus";
        public static string GrowthPlanDelete(int growthPlanItemId) => $"/v1/growthplan/{growthPlanItemId}";
        public static string GrowthPlanCustomTypesGet() => "/v1/growthplan/customtypes";
        public static string GrowthPlanCustomTypesSave() => "/v1/growthplan/customtypes/save";
        public static string GrowthPlanGetCustomDelete() => "/v1/growthplan/customtypes/delete";

        //Ideaboard
        public static string IdeaboardCard() => "/v1/ideaboard/card";
        public static string IdeaboardCards() => "v1/ideaboard/cards";
        public static string IdeaboardGetCardsByAssessmentUid(Guid assessmentUid) => $"/v1/ideaboard/cards/{assessmentUid}";
        public static string IdeaboardGrowthPlanItem() => "/v1/ideaboard/growthplanitem";
        public static string IdeaboardSortCards() => "/v1/ideaboard/sortcards";
        public static string IdeaboardVotesAllowed() => "/v1/ideaboard/votesallowed";
        public static string IdeaboardResetVotes() => "/v1/ideaboard/resetvotes";
        // Companies
        public static string Companies() => "v1/companies";
        public static string CompaniesIndustries() => "v1/companies/industries";
        public static string CompaniesListCompanies() => "v1/companies/ListCompanies";
        public static string CompanyDetails(int companyId) => $"v1/companies/{companyId}";
        public static string CompaniesCountries() => "v1/companies/countries";
        public static string CompaniesSuggestedReviewers(int companyId, Guid memberId) =>
        $"v1/companies/{companyId}/members/{memberId}/suggestedreviewers";
        public static string CompaniesGetTeamHierarchyByCompanyId(int companyId) =>
        $"v1/companies/GetTeamHierarchyByCompanyId/{companyId}";
        public static string CompaniesAssessments(int companyId) => $"v1/companies/{companyId}/assessments";
        public static string CompaniesMembers(int companyId) => $"v1/companies/{companyId}/members";
        public static string CompaniesUsers(int companyId) => $"v1/companies/{companyId}/users";
        public static string CompaniesRadars(int companyId) => $"v1/companies/{companyId}/radars";
        public static string CompaniesRadarsSurvey(int companyId, int surveyId) =>
            $"v1/companies/{companyId}/radars/{surveyId}";
        public static string CompaniesCurrentUsage(int companyId) => $"v1/companies/{companyId}/currentUsage";
        public static string CompaniesPrimaryCompanyContact(int companyId) =>
        $"v1/companies/{companyId}/primaryCompanyContact";
        public static string CompaniesPartners() => "v1/companies/partners";
        public static string CompaniesFeatures(int companyId) => $"v1/companies/{companyId}/features";
        public static string CompaniesUsersRoles() => "/v1/companies/roles";
        public static string CompaniesParticipantGroups() => "/v1/companies/participantgroups";
        public static string CompaniesMembersRolesAndParticipantGroups() => "/v1/companies/memberrolesandparticipantgroups";
        public static string CompanyStakeHolders(int companyId) => $"/v1/companies/{companyId}/stakeholders";
        public static string CompanyMembers(int companyId) => $"/v1/companies/{companyId}/companyMembers";
        public static string CompaniesAssessmentResults(int companyId) => $"/v1/companies/{companyId}/assessmentResults";

        public static string CompaniesGrowthPlanItems(int companyId) => $"/v1/companies/{companyId}/growthplanitems";
        // Radars
        public static string Radars() => "v1/radars";
        public static string RadarsByCompany(int companyId) => $"v1/radars/{companyId}";
        public static string RadarsDetails(int companyId) => $"v1/radars/{companyId}/details";
        public static string RadarsDetailsSurveyId(int companyId, int surveyId) => $"v1/radars/{companyId}/details/{surveyId}";
        public static string RadarsDefault(int companyId) => $"v1/radars/{companyId}/default";
        public static string RadarsByType(int companyId) => $"v1/radars/{companyId}/types";
        public static string RadarQuestions(int companyId) => $"/v1/radars/{companyId}/questionsnotext";
        public static string RadarCompetencies(int companyId) => $"/v1/radars/{companyId}/competencies";



        //IndividualAssessments
        public static string AssessmentGetIndividual(Guid assessmentId) => $"v1/assessments/individual/{assessmentId}";
        public static string IndividualAssessments() => "v1/assessments/individual";
        public static string IndividualAssessmentsEmailsAndClaims() => "v1/assessments/individual/CreateEmailsAndUserClaims";
        public static string AssessmentIndividualReviewer() => "v1/assessments/individual/reviewer";
        public static string AssessmentIndividualReviewerTags() => "v1/assessments/individual/reviewer/tags";
        public static string AssessmentIndividualEmail() => "v1/assessments/individual/Email";
        public static string AssessmentIndividualEdit() => "v1/assessments/individual/edit";
        public static string AssessmentIndividualBulkEmail() => "v1/assessments/individual/BulkEmail";
        public static string AssessmentIndividualImport() => "v1/assessments/individual/Import";
        public static string AssessmentIndividualExcelTemplate() => "v1/assessments/individual/ExportTemplate";
        public static string AssessmentIndividualParticipantGroup() => "/v1/assessments/individual/participant/groups";

        //Reports
        public static string ReportsGrowthPlanItems() => "v1/reports/individualassessments/growthplanitems";
        public static string ReportsIndividualAssessmentResponses() => "v1/reports/individualassessmentresponses";
        public static string ReportsMostSelectedRecommendations() => "v1/reports/mostselectedrecommendations";
        public static string ReportsBusinessOutcomeExcel(int companyId, string categoryLabelUid, string teamId) =>
        $"v1/reports/businessoutcomes/excel/{companyId}/{categoryLabelUid}/{teamId}";

        public static string ReportsList() => "/v1/reports/listreports";
        public static string ReportsById(double reportId) => $"/v1/reports/getreport/{reportId}";
        // Tags
        public static string TagsAddMasterTagsToCompany(int companyId) =>
        $"/v1/tags/{companyId}/AddMasterTagsToCompany";
        public static string TagsTeamWorkTypes() => "/v1/tags/TeamWorkTypes";

        // Teams
        public static string Teams() => "v1/teams";
        public static string TeamUpdate(Guid teamId) => $"v1/teams/{teamId}";
        public static string TeamDetails(Guid teamUid) => $"v1/teams/{teamUid}";
        public static string TeamType() => "/v1/teams/teamType";
        public static string TeamMembers(Guid teamUid) => $"v1/teams/{teamUid}/members";
        public static string TeamAssessments(Guid teamUid) => $"v1/teams/{teamUid}/assessments";
        public static string TeamMemberUpdate(Guid teamUid, Guid memberUid) =>
        $"v1/teams/{teamUid}/members/{memberUid}";
        public static string TeamStakeholder(Guid teamUid) => $"v1/teams/{teamUid}/stakeholders";
        public static string TeamStakeholderUpdate(Guid teamId, Guid stakeholderId) =>
        $"/v1/teams/{teamId}/stakeholders/{stakeholderId}";
        public static string DeleteTeam(Guid teamId) => $"v1/teams/{teamId}";
        public static string DeleteTeamMember(Guid teamUid, Guid memberUid) => $"v1/teams/{teamUid}/members/{memberUid}";
        public static string DeleteTeamStakeholder(Guid teamId, Guid stakeholderId) =>
        $"v1/teams/{teamId}/stakeholders/{stakeholderId}";
        public static string TeamSubteams(Guid teamId) => $"v1/teams/{teamId}/subteams";
        public static string TeamsWithMemberCountAndCompletedAssessmentCount(int companyId) => $"/v1/teams/company/{companyId}/teamswithmembercountandcompletedassessmentcount";
        public static string TeamsWithMembersAndSurveysCount(int companyId) => $"/v1/teams/company/{companyId}/teamswithmembersandsurveys";
        public static string Teams(Guid teamUId) => $"/v1/teams/{teamUId}";
        public static string TeamExport(Guid teamUId) => $"/v1/teams/{teamUId}/export";
        //Team Assessments
        public static string TeamAssessmentIndividualReviewers(Guid teamId, Guid assessmentId) =>
        $"v1/teams/{teamId}/assessments/individual/{assessmentId}/reviewers";


        //Business Outcome
        public static string BusinessOutcomeGetAllCategoryLabelsByCompanyId(int companyId, int teamId, int tagFilterType) =>
            $"/v1/businessoutcomes/GetAllCategoryLabelsByCompanyId/{companyId}/{teamId}/{tagFilterType}";

        //Crud
        public static string BusinessOutcomePost() => "v1/businessoutcomes/AddNewBusinessOutcome";
        public static string BusinessOutcomeUpdate() => "v1/businessoutcomes/UpdateBusinessOutcome";
        public static string BusinessOutcomeDelete(Guid guid, bool isUserAffirmed) => $"v1/businessoutcomes/DeleteBusinessOutcome/{guid}/{isUserAffirmed}";
        public static string BusinessOutcomeGetById(int companyId,Guid uid, Guid swimelaneId) => $"v1/businessoutcomes/GetBusinessOutcomeById/{companyId}/{uid}/{swimelaneId}";

        //External Links
        public static string BusinessOutcomeLinkPost() => "v1/businessoutcomes/AddNewBusinessOutcomeLink";

        //Tags
        public static string BusinessOutcomeGetAllCategoryLabelsByCompanyId(int companyId) =>
            $"v1/businessoutcomes/GetAllCategoryLabelsByCompanyId/{companyId}";
        public static string BusinessOutcomeCreateBusinessOutcomeCategoryLabelsAndTags(int companyId, int teamId) =>
            $"v1/businessoutcomes/CreateBusinessOutcomeCategoryLabelsAndTags/{companyId}/{teamId}";

        //Meeting Notes
        public static string BusinessOutcomeCreateMeetingNotes = "/v1/BusinessOutcomeMeetingNotes/AddBusinessOutcomeMeetingNote";
        public static string BusinessOutcomeGetMeetingNotes = "/v1/BusinessOutcomeMeetingNotes/GetBusinessOutcomeMeetingNotes";
        public static string BusinessOutcomeGetMeetingNote = "/v1/BusinessOutcomeMeetingNotes/GetBusinessOutcomeMeetingNote";
        public static string BusinessOutcomePutMeetingNote = "/v1/BusinessOutcomeMeetingNotes/UpdateBusinessOutcomeMeetingNote";
        public static string BusinessOutcomeGetMeetingNotePendingNotifications = "/v1/BusinessOutcomeMeetingNotes/CheckBusinessOutcomeMeetingNotePendingNotification";
        public static string BusinessOutcomeUpdateMeetingNotePendingActionItemStatusByOwner = "/v1/BusinessOutcomeMeetingNotes/UpdateBusinessOutcomeMeetingNoteActionItemStatusByOwner";
        public static string BusinessOutcomeUploadMeetingNotesAttachments = "/v1/BusinessOutcomeMeetingNotes/UploadBusinessOutcomeMeetingNoteAttachment";
        public static string BusinessOutcomeGetDownloadMeetingNoteAttachments = "/v1/BusinessOutcomeMeetingNotes/DownloadBusinessOutcomeMeetingNoteAttachment";
        public static string BusinessOutcomeDeleteMeetingNoteAttachments = "/v1/BusinessOutcomeMeetingNotes/DeleteBusinessOutcomeMeetingNoteAttachment";

        //Card Type
        public static string BusinessOutcomeGetCardType(int companyId) => $"/v1/businessoutcomes/businessoutcomecardtype/{companyId}";

        //Dashboard
        public static string BusinessOutcomeAllSwimlanes(int companyId) =>
        $"v1/businessoutcomes/GetAllSwimlanesByCompanyOrTeamId/{companyId}";
        public static string BusinessOutcomeUpdateSortOrder = "v1/businessOutcomes/UpdateBusinessOutcomesSortOrder";
        public static string BusinessOutcomeLabelSwimlanes(string categoryLabelUid, int companyId, string teamId) =>
        $"v1/businessoutcomes/categorylabelview/{categoryLabelUid}/{companyId}/{teamId}";

        //Metrics
        public static string BusinessOutcomeAddNewMetric() => "v1/businessoutcomes/AddNewBusinessOutcomeMetric";
        public static string BusinessOutcomeGetMetrics(int companyId) => $"v1/businessoutcomes/GetBusinessOutcomeMetrics/{companyId}";
        public static string BusinessOutcomeGetMetricTypes(int companyId) => $"/v1/businessoutcomes/GetBusinessOutcomeMetricTypes/{companyId}";

        //Custom Fields
        public static string BusinessOutcomesGetAllCustomFieldsByCompanyId(int companyId) =>
        $"/v1/businessoutcomes/GetAllCustomFieldsByCompanyId/{companyId}";

        public static string BusinessOutcomesCreateCustomFields(int companyId) =>
            $"v1/businessoutcomes/CreateCustomFields/{companyId}";

        public static string BusinessOutcomesCreateChecklist(int companyId, Guid businessOutcomeId) =>
            $"/v1/businessoutcomes/{companyId}/checklistitem/{businessOutcomeId}";

        public static string BusinessOutcomeGetAllBusinessOutcomeDashboardSettingsByCompanyId(int companyId) =>
            $"/v1/businessoutcomes/dashboardsettings/{companyId}";

        public static string BusinessOutcomeGetAllCustomFieldsByCompanyId(int companyId) =>
            $"/v1/businessoutcomes/GetAllCustomFieldsByCompanyId/{companyId}";

        //Campaigns
        public static string CampaignDetails(int companyId) => $"v1/companies/{companyId}/campaigns";

        //CampaignsV2
        public static string CampaignsV2(int companyId) => $"/v1/companies/{companyId}/campaigns";
        public static string CampaignsV2Delete(int companyId,int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}";
        public static string CampaignsV2Details(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/details"; 
        public static string CampaignsV2Teams(int companyId) => $"/v1/companies/{companyId}/campaigns/metadata/teams";
        public static string CampaignsV2Facilitators(int companyId) => $"/v1/companies/{companyId}/campaigns/metadata/facilitators";
        public static string CampaignsV2AutoMatchmaking(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/setup/auto-matchmaking";
        public static string CampaignsV2Setup(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/setup";
        public static string CampaignsV2Launch(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/assessments";

        public static string CampaignsV2TeamsIds(int companyId) => $"/v1/companies/{companyId}/campaigns/metadata/teams/ids";
        public static string CampaignsV2CampaignTeamIds(int companyId, int campaignId) =>
            $"/v1/companies/{companyId}/campaigns/{campaignId}/teams/ids";
        public static string CampaignsV2DeleteFacilitators(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/facilitators";
        public static string CampaignsV2PatchFacilitators(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/facilitators";

        public static string CampaignsV2PatchUpdateAssessmentName(int companyId, int campaignId) => $"/v1/companies/{companyId}/campaigns/{campaignId}/teams/assessment/name";

        // TimeZones
        public static string TimeZones() => "v1/timezones";

        public static string InternalUtilsSystemInformation() => "/internal/Utils/SystemInformation";

        // Users
        public static string UsersIsSiteAdmin() => "v1/users/getIsSiteAdmin";

        // OAuth
        public static string OauthToken() => "/v1/oauth/Token";
        public static string OauthRegister() => "v1/oauth/Register";
        public static string OauthAppRegistrations(int companyId) => $"/v1/oauth/{companyId}/AppRegistrations";
        public static string OauthAppRegistration(int companyId) => $"/v1/oauth/{companyId}/AppRegistration";
    }
}