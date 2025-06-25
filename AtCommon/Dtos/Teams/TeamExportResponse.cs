using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Teams
{
    public class AssessmentExport
    {
        public int AssessmentId { get; set; }
        public object BatchAssessmentId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? FacilitationDate { get; set; }
        public string Facilitator { get; set; }
        public string FacilitatorEmail { get; set; }
        public bool Imported { get; set; }
        public string Uri { get; set; }
        public bool ParticipantCanSelectRole { get; set; }
        public bool LaunchAhfSurvey { get; set; }
        public bool FoundFacilitator { get; set; }
        public string Location { get; set; }
        public string RandomPinPassword { get; set; }
        public int TeamMemberLogInInvitation { get; set; }
        public DateTime? FacilitationEndDate { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AhfSurveySendDateOption { get; set; }
        public int SurveyId { get; set; }
        public object CampaignId { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<object> AssessmentNotes { get; set; }
        public List<object> AssessmentFacilitators { get; set; }
        public List<object> AssessmentGrowthPlanItems { get; set; }
        public List<object> IndividualAssessments { get; set; }
        public List<object> ScheduledAssessments { get; set; }
        public List<object> AssessmentMaturityChecklists { get; set; }
    }

    public class Competency
    {
        public int CompetencyId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Exclude { get; set; }
        public bool External { get; set; }
        public string BiAbbreviation { get; set; }
        public string FontSize { get; set; }
        public string Font { get; set; }
        public string LetterSpacing { get; set; }
        public int RadarOrder { get; set; }
        public int LinkedAppId { get; set; }
        public object Direction { get; set; }
        public int SourceAppId { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class Contact
    {
        public int MemberId { get; set; }
        public DateTime EmailSentAt { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Role { get; set; }
        public string ParticipantGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<object> CompanyStakeholderTags { get; set; }
        public List<object> CompanyTeamMemberTags { get; set; }
        public List<object> ResultSets { get; set; }
    }

    public class Dimension
    {
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int RadarOrder { get; set; }
        public int SortOrder { get; set; }
        public string FontSize { get; set; }
        public object Direction { get; set; }
        public string LetterSpacing { get; set; }
        public string Font { get; set; }
        public bool Enabled { get; set; }
        public string Position { get; set; }
        public int LinkedAppId { get; set; }
        public int SourceAppId { get; set; }
        public List<SubDimension> SubDimensions { get; set; }
    }

    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<MemberTag> MemberTags { get; set; }
    }

    public class MemberTag
    {
        public string Tag { get; set; }
        public string Category { get; set; }
        public string TeamTag { get; set; }
        public string TeamTagCategory { get; set; }
    }

    public class ParticipantTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MasterTagId { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string AlternativeText { get; set; }
        public int ScaleOverride { get; set; }
        public string HelpText { get; set; }
        public bool ExcludeCompany { get; set; }
        public bool ExcludeMethodology { get; set; }
        public bool ExcludeByTag { get; set; }
        public bool ExcludeWorkType { get; set; }
        public object SurveyGizmoId { get; set; }
        public bool External { get; set; }
        public int LinkedAppId { get; set; }
        public string ExcludeRole { get; set; }
        public int SourceAppId { get; set; }
        public bool? Quantitative { get; set; }
        public List<object> Companies { get; set; }
        public List<object> WorkTypes { get; set; }
        public List<ParticipantTag> ParticipantTags { get; set; }
        public bool IsExcluded { get; set; }

    }

    public class TeamExportResponse
    {
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public DateTime? FormationDate { get; set; }
        public DateTime? AgileAdoptionDate { get; set; }
        public string Bio { get; set; }
        public string Type { get; set; }
        public string MetricsType { get; set; }
        public string ExternalIdentifier { get; set; }
        public List<TagExport> TagExports { get; set; }
        public List<TeamMemberExport> TeamMemberExports { get; set; }
        public List<object> StakeholderExports { get; set; }
        public List<AssessmentExport> AssessmentExports { get; set; }
        public List<SurveyExport> SurveyExports { get; set; }
        public List<object> TeamGrowthPlanItemExports { get; set; }
        public List<object> GrowthPlanItemExports { get; set; }
        public List<object> IndividualBatchExports { get; set; }
        public List<object> MaturityCheckListExports { get; set; }
    }

    public class SubDimension
    {
        public int SubDimensionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string Color { get; set; }
        public bool Enabled { get; set; }
        public string FontSize { get; set; }
        public string Font { get; set; }
        public string LetterSpacing { get; set; }
        public int RadarOrder { get; set; }
        public int LinkedAppId { get; set; }
        public object Direction { get; set; }
        public int SourceAppId { get; set; }
        public List<Competency> Competencies { get; set; }
    }

    public class SurveyExport
    {
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public int OriginalVersion { get; set; }
        public bool Active { get; set; }
        public string SurveyType { get; set; }
        public int SurveyTypeId { get; set; }
        public int Scale { get; set; }
        public string Copyright { get; set; }
        public DateTime? DateActivated { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public bool IsPublic { get; set; }
        public int LinkedAppId { get; set; }
        public string ReviewerThankYouMessage { get; set; }
        public string RevieweeThankYouMessage { get; set; }
        public string ThankYouMessage { get; set; }
        public string ReviewerSurveyMessage { get; set; }
        public string RevieweeSurveyMessage { get; set; }
        public string SurveyMessage { get; set; }
        public string ReviewerEmailMessage { get; set; }
        public string RevieweeEmailMessage { get; set; }
        public string EmailMessage { get; set; }
        public int SourceAppId { get; set; }
        public string Logo { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsBeta { get; set; }
        public int Order { get; set; }
        public bool ShowOneResponse { get; set; }
        public string EmailSender { get; set; }
        public string EmailSubject { get; set; }
        public string ReviewerEmailSender { get; set; }
        public string ReviewerEmailSubject { get; set; }
        public bool EmailAppendStandardFooter { get; set; }
        public bool Absolute { get; set; }
        public string ReminderMessage { get; set; }
        public string RulesDefinitions { get; set; }
        public string RadarDisplayMessage { get; set; }
        public string UserRoles { get; set; }
        public string IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Dimension> Dimensions { get; set; }
    }

    public class TagExport
    {
        public string Tag { get; set; }
        public string Category { get; set; }
        public string TeamTag { get; set; }
        public string TeamTagCategory { get; set; }
    }
    public class TeamMemberExport
    {
        public double AllocationValue { get; set; }
        public object AllocationType { get; set; }
        public Member Member { get; set; }
    }
}