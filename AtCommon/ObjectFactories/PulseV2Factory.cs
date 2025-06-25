using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Tags;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;


namespace AtCommon.ObjectFactories
{
    public static class PulseV2Factory
    {
        public static SavePulseAssessmentV2Request PulseAssessmentV2AddRequest(RadarQuestionDetailsV2Response surveyDetails, IList<TeamV2Response> teamResponses, int teamId, bool isPublished = true, List<RoleRequest> roles = null, AssessmentPeriod period = AssessmentPeriod.TwentyFourHours, RepeatIntervalId interval = RepeatIntervalId.Never, DateTime? startDate = null, string surveyName = null)
        {
            var team = new List<PulseAssessmentTeamRequest>();
            var radarDimensions = surveyDetails.Dimensions.Where(a => a.Name != "Finish").ToList();

            DateTime? publishedDate = null;
            if (isPublished)
            {
                publishedDate = DateTime.UtcNow;
            }

            foreach (var teams in teamResponses)
            {

                var participants = new List<PulseAssessmentMemberRequest>();
                foreach (var teamMember in teams.SelectedParticipants)
                {
                    List<RoleRequest> tag;

                    if (teamMember.Tags.Count == 0)
                    {
                        tag = new List<RoleRequest>();
                    }
                    else
                    {
                        tag = new List<RoleRequest>
                        {
                            new RoleRequest
                            {
                                Key = teamMember.Tags?.First().Key,
                                Tags = new List<TagRoleRequest>
                                {
                                    new TagRoleRequest
                                    {
                                        Id = teamMember.Tags?.First().Tags.First().Id,
                                        Name = teamMember.Tags?.First().Tags.First().Name
                                    }
                                }
                            }
                        };
                    }

                    participants.Add(new PulseAssessmentMemberRequest()
                    {
                        Uid = teamMember.Uid,
                        FirstName = teamMember.FirstName,
                        LastName = teamMember.LastName,
                        Email = teamMember.Email,
                        Tags = tag
                    });

                }

                team.Add(new PulseAssessmentTeamRequest()
                {
                    TeamId = teams.TeamId,
                    TeamUid = teams.Uid,
                    Name = teams.Name,
                    SelectedParticipants = participants
                });
            }

            roles ??= new List<RoleRequest>()
            {
                new RoleRequest()
                {
                    Key = "Role",
                    Tags = new List<TagRoleRequest>()
                }
            };

            var roleFilter = new PulseAssessmentRoleFilterRequest()
            {
                Tags = roles
            };

            return new SavePulseAssessmentV2Request
            {
                TeamId = teamId,
                SurveyId = surveyDetails.SurveyId,
                SurveyName = string.IsNullOrEmpty(surveyName) ? surveyDetails.Name : surveyName,
                PulseAssessmentId = 0,
                TemplatePulseAssessmentId = 0,
                Name = $"PulseATV2{RandomDataUtil.GetPulseAssessmentName()} + {Guid.NewGuid()}",
                IsTemplate = false,
                SelectedTeams = team,
                RoleFilter = roleFilter,
                IsPublished = isPublished,
                PublishedDate = publishedDate,
                PeriodId = (int)period,
                StartDate = startDate ?? DateTime.UtcNow.AddMinutes(-1),
                EndDate = (startDate ?? DateTime.UtcNow.AddMinutes(-1)).AddDays(7),
                RepeatIntervalId = (int)interval,
                RepeatEndStrategyId = 1,
                RepeatOccurrenceNumber = 0,
                DimensionIds = radarDimensions.Select(d => d?.DimensionId).ToList(),
                SubDimensionIds = radarDimensions.SelectMany(d => d.SubDimensions).Select(s => s.SubDimensionId).ToList(),
                Competencies = radarDimensions.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies)
                    .Select(c => new PulseCompetencyV2
                    { CompetencyId = c.CompetencyId, QuestionIds = c.Questions.Select(q => q.QuestionId).ToList() }).ToList()
            };
        }

        public static SavePulseAssessmentV2Request GetPulseUiAddRequest(RadarQuestionDetailsV2Response surveyDetails, List<PulseAssessmentTeamRequest> teams, bool isDraftAssessment = false)
        {
            var radarDimensions = surveyDetails.Dimensions.ToList();
            DateTime? publishedDate = null;
            if (!isDraftAssessment)
            {
                publishedDate = DateTime.UtcNow;
            }

            return new SavePulseAssessmentV2Request
            {
                Name = $"PulseAT{RandomDataUtil.GetPulseAssessmentName()}",
                SurveyId = surveyDetails.SurveyId,
                SelectedTeams = teams,
                PeriodId = (int)AssessmentPeriod.TwentyFourHours,
                RepeatIntervalId = (int)RepeatIntervalId.Never,
                RepeatEndStrategyId = 1,
                RepeatOccurrenceNumber = 1,
                StartDate = DateTime.UtcNow.AddMinutes(2),
                EndDate = DateTime.UtcNow.AddDays(7),
                PublishedDate = publishedDate,
                DimensionIds = radarDimensions.Select(d => d?.DimensionId).ToList(),
                SubDimensionIds = radarDimensions.SelectMany(d => d.SubDimensions).Select(s => s.SubDimensionId).ToList(),
                Competencies = radarDimensions.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).Select(c => new PulseCompetencyV2() { CompetencyId = c.CompetencyId, QuestionIds = c.Questions.Select(q => q.QuestionId).ToList() }).ToList()
            };
        }

        public static SavePulseAssessmentV2Request GetPulseUiDraftRequest(RadarQuestionDetailsV2Response surveyDetails, List<PulseAssessmentTeamRequest> teams)
        {
            var radarDimensions = surveyDetails.Dimensions.ToList();

            return new SavePulseAssessmentV2Request()
            {
                Name = $"PulseAT{RandomDataUtil.GetPulseAssessmentName()}",
                SurveyId = surveyDetails.SurveyId,
                SelectedTeams = teams,
                PeriodId = (int)AssessmentPeriod.TwentyFourHours,
                RepeatIntervalId = (int)RepeatIntervalId.Never,
                RepeatEndStrategyId = 1,
                RepeatOccurrenceNumber = 1,
                StartDate = DateTime.UtcNow.AddDays(7),
                DimensionIds = radarDimensions.Select(d => d?.DimensionId).ToList(),
                SubDimensionIds = radarDimensions.SelectMany(d => d.SubDimensions).Select(s => s.SubDimensionId).ToList(),
                Competencies = radarDimensions.SelectMany(d => d.SubDimensions).SelectMany(s => s.Competencies).Select(c => new PulseCompetencyV2() { CompetencyId = c.CompetencyId, QuestionIds = c.Questions.Select(q => q.QuestionId).ToList() }).ToList()
            };
        }

        public static GetPulseAssessmentV2Request GetPulseAssessmentRequest(string dataRequest, PulseAssessmentRoleFilterRequest roleFilterRequest = null, List<int> competencyIds = null)
        {
            competencyIds ??= new List<int>(0);

            roleFilterRequest ??= new PulseAssessmentRoleFilterRequest()
            {
                Tags = new List<RoleRequest>()
                {
                    new RoleRequest()
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>()
                    }
                }
            };

            return new GetPulseAssessmentV2Request()
            {
                DataRequest = dataRequest,
                IsFilterTeamsBasedOnSelectedRoleFilter = true,
                SelectedRoleFilter = roleFilterRequest,
                IsSelectedCompetencies = true,
                SelectedCompetencyIds = competencyIds
            };
        }

        public static PulseAssessmentV2 GetPulseAddData()
        {

            return new PulseAssessmentV2
            {
                Name = $"PulseAT{RandomDataUtil.GetPulseAssessmentName()}{CSharpHelpers.RandomNumber()}",
                AssessmentType = SharedConstants.TeamAssessmentType,
                StartDate = DateTime.Today,
                Period = "24 Hours",
                RepeatInterval = new RepeatIntervals
                {
                    Type = "Does not repeat",
                    Ends = End.None
                },
                Questions = new List<QuestionDetails>
                {
                    new QuestionDetails
                    {
                        DimensionName =  SharedConstants.SurveyDimension,
                        SubDimensionName = SharedConstants.SurveySubDimension,
                        CompetencyName =  SharedConstants.SurveyCompetency,
                        QuestionSelectionPref = QuestionSelectionPreferences.Dimension
                    }
                }
            };
        }

        public static RadarQuestionDetailsV2Response GetTh25SurveyResponse()
        {
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory}/Resources/TestData/Survey/TH25Response.json";
            return File.ReadAllText(path).DeserializeJsonObject<RadarQuestionDetailsV2Response>();
        }

        public static AddMemberRequest GetValidTeamMemberWithRole(string role)
        {
            var unique = Guid.NewGuid().ToString();

            return new AddMemberRequest
            {
                FirstName = "Automation",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"ah_automation+M{unique}@agiletransformation.com",
                ExternalIdentifier = unique,
                Tags = new List<TeamMemberTagRequest>
                {
                    new TeamMemberTagRequest()
                    {
                        Category = "Role",
                        Tags = new List<string>
                        {
                            role
                        }
                    }
                }
            };
        }

        public static List<TeamV2Response> GetTeams(SavePulseAssessmentV2Request pulseAssessmentV2Request)
        {
            var teamMember = new List<TeamMemberV2Response>();
            foreach (var member in pulseAssessmentV2Request.SelectedTeams.SelectMany(a => a.SelectedParticipants))
            {
                List<RoleResponse> tag;

                if (member.Tags.Count == 0)
                {
                    tag = new List<RoleResponse>();
                }
                else
                {
                    tag = new List<RoleResponse>
                    {
                        new RoleResponse
                        {
                            Key = member.Tags.First().Key,
                            Tags = new List<TagRoleResponse>
                            {
                                new TagRoleResponse()
                                {
                                    Id = member.Tags.First().Tags.First().Id,
                                    Name = member.Tags.First().Tags.First().Name
                                }
                            }
                        }
                    };
                }

                teamMember.Add(new TeamMemberV2Response()
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email,
                    Uid = member.Uid,
                    Tags = tag
                });
            }

            return pulseAssessmentV2Request.SelectedTeams.Select(team => new TeamV2Response()
            {
                IsSelected = true,
                TeamId = team.TeamId,
                Name = team.Name,
                IsAssessmentCompleted = false,
                Uid = team.TeamUid,
                TotalTeamMembers = team.SelectedParticipants.Count,
                SelectedParticipants = teamMember
            })
                  .ToList();
        }

        public static GetParticipantsRequest GetParticipantsRequest(PulseAssessmentRoleFilterRequest roleFilterRequest = null, List<int> competencyIds = null)
        {
            competencyIds ??= new List<int>(0);

            roleFilterRequest ??= new PulseAssessmentRoleFilterRequest()
            {
                Tags = new List<RoleRequest>()
                {
                    new RoleRequest()
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>()
                    }
                }
            };

            return new GetParticipantsRequest()
            {
                SelectedRoleFilter = roleFilterRequest,
                IsSelectedCompetencies = true,
                SelectedCompetencyIds = competencyIds
            };
        }
    }
}

