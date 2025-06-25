using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class PulseFactory
    {
        public static RadarQuestionDetailsResponse GetTh25SurveyResponse()
        {
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory}/Resources/TestData/Survey/TH25Response.json";
            return File.ReadAllText(path).DeserializeJsonObject<RadarQuestionDetailsResponse>();
        }

        public static AddMemberV2Request GetValidPostPulseTeamMember(List<RoleRequest> tags = null)
        {
            var unique = Guid.NewGuid();

            tags ??= new List<RoleRequest>();

            return new AddMemberV2Request
            {
                FirstName = $"Member{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = RandomDataUtil.GetEmail(),
                ExternalIdentifier = "",
                Tags = tags,
                Reviewers = null
            };
        }

        public static UpdateTeamMemberV2Request GetValidPutTeamMemberV2()
        {
            var unique = Guid.NewGuid().ToString();

            return new UpdateTeamMemberV2Request
            {
                FirstName = "Automation",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = RandomDataUtil.GetEmail(),
                ExternalIdentifier = unique,
                Tags = new List<RoleRequest>
                {
                    new RoleRequest()
                    {
                        Key = "Role",
                        Tags = new List<TagRoleRequest>
                        {
                            new TagRoleRequest()
                            {
                                Id = 0,
                                Name = "Developer"
                            }
                        }
                    },
                    new RoleRequest()
                    {
                        Key = "Participant Group",
                        Tags = new List<TagRoleRequest>
                        {
                            new TagRoleRequest()
                            {
                                Id = 0,
                                Name = "Support"
                            }
                        }
                    }
                }
            };
        }

        public static List<TeamMemberV2Response> GetTeamMemberV2Responses(IList<TeamMemberResponse> teamMemberResponses, List<TagRoleResponse> totalTagsOfTeam)
        {
            var membersResponses = new List<TeamMemberV2Response>();
            foreach (var member in teamMemberResponses)
            {
                List<RoleResponse> tag;
                var memberTagList = totalTagsOfTeam.Where(a => a.Name == member.Tags?.First().Value.First())
                    .Select(a => a).ToList();

                if (memberTagList.Count == 0)
                {
                    tag = new List<RoleResponse>();
                }
                else
                {
                    tag = new List<RoleResponse>
                    {
                        new RoleResponse()
                        {
                            Key = member.Tags?.First().Key,
                            Tags = new List<TagRoleResponse>
                            {
                                new TagRoleResponse()
                                {
                                    Id = memberTagList.First().Id,
                                    Name = memberTagList.First().Name
                                }
                            }
                        }
                    };
                }
                var members = new TeamMemberV2Response()
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email,
                    Uid = member.Uid,
                    Tags = tag
                };

                membersResponses.Add(members);
            }

            return membersResponses;
        }
    }
}

