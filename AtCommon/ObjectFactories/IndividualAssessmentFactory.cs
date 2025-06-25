using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Utilities;
using System;
using System.Collections.Generic;
using AtCommon.Dtos.Companies;
using Newtonsoft.Json;

namespace AtCommon.ObjectFactories
{
    public static class IndividualAssessmentFactory
    {
        public static CreateIndividualAssessmentRequest GetValidIndividualAssessment(int companyId, string companyName, int surveyId)
        {
            var unique = Guid.NewGuid().ToString();
            var uniqueGuid = Guid.NewGuid();

            return new CreateIndividualAssessmentRequest
            {
                AssessmentName = $"AT_{RandomDataUtil.GetAssessmentName()}",
                PointOfContact = unique,
                PointOfContactEmail = $"{unique}@test.com",
                TeamUid = uniqueGuid,
                CompanyId = companyId,
                CompanyName = companyName,
                SurveyTypeId = surveyId,
                AllowInvite = false,
                AllowResultView = true,
                Published = false,
                ReviewEachOther = false,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10),
                Members = new List<IndividualAssessmentMemberRequest>
                {
                    new IndividualAssessmentMemberRequest
                    {
                        Uid = uniqueGuid,
                        FirstName = unique,
                        LastName = SharedConstants.TeamMemberLastName + unique,
                        Email = $"ah_automation+{unique}@agiletransformation.com"
                    }
                },
                IndividualViewers = null,
                AggregateViewers = null
            };
        }

        public static CreateIndividualAssessmentRequest GetPublishedIndividualAssessment(int companyId, string companyName,
            Guid teamUid, string assessmentName = null)
        {
            var unique = Guid.NewGuid().ToString("D");
            return new CreateIndividualAssessmentRequest
            {
                AssessmentName = assessmentName ?? $"AT_{RandomDataUtil.GetAssessmentName()}",
                PointOfContact = unique,
                PointOfContactEmail = $"{unique}@test.com",
                TeamUid = teamUid,
                CompanyId = companyId,
                CompanyName = companyName,
                AllowInvite = false,
                AllowResultView = true,
                Published = true,
                ReviewEachOther = false,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10)
            };
        }

        public static CreateIndividualAssessmentRequest GetDraftIndividualAssessment(int companyId, string companyName,
            Guid teamUid, string assessmentName = null)
        {
            var unique = Guid.NewGuid().ToString("D");
            return new CreateIndividualAssessmentRequest
            {
                AssessmentName = assessmentName ?? $"AT_{unique}",
                PointOfContact = unique,
                PointOfContactEmail = $"{unique}@test.com",
                TeamUid = teamUid,
                CompanyId = companyId,
                CompanyName = companyName,
                AllowInvite = false,
                AllowResultView = true,
                Published = false,
                ReviewEachOther = false,
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(10)
            };
        }

        public static CreateIndividualAssessmentRequest GetUiIndividualAssessment(string assessmentName,
            bool isInviteAllowed = false, bool isResultViewAllowed = true)
        {
            var unique = Guid.NewGuid().ToString();
            return new CreateIndividualAssessmentRequest
            {
                AssessmentName = assessmentName,
                PointOfContact = "John Smith",
                PointOfContactEmail = $"{unique}@test.us",
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7),
                AllowInvite = isInviteAllowed,
                AllowResultView = isResultViewAllowed
                
            };
        }

        public static CreateTeamMemberTagsRequest CreateTeamMemberTags(Guid assessmentUid, Guid participantUid, List<CompanyTeamMemberTags> individualRoles)
        {
            return new CreateTeamMemberTagsRequest()
            {
                
                AssessmentUid = assessmentUid,
                ParticipantUid = participantUid,
                ParticipantGroups = new List<RoleRequest>
                {
                    new RoleRequest
                    {
                        Key = "Participant Group",
                        Tags = JsonConvert.DeserializeObject<List<TagRoleRequest>>(JsonConvert.SerializeObject(individualRoles))
                    }
                }
            };
        }
    }
}