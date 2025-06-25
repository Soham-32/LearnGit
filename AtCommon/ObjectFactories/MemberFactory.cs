using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class MemberFactory
    {
        public static AddMemberRequest GetTeamMember()
        {
            return new AddMemberRequest
            {
                FirstName = $"Member{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+M{Guid.NewGuid():N}@agiletransformation.com"
            };
        }

        public static TeamMemberResponse GetArchiveTeamMember()
        {
            return new TeamMemberResponse
            {
                FirstName = "Archive",
                LastName = "Member",
                Email = "ah_automation+archive_member@agiletransformation.com"

            };
        }
        public static MemberResponse GetArchiveStakeholder()
        {
            return new MemberResponse
            {
                FirstName = "Archive",
                LastName = "Stake",
                Email = "ah_automation+archive_stake@agiletransformation.com"
            };
        }

        public static AddStakeholderRequest GetStakeholder()
        {
            return new AddStakeholderRequest
            {
                FirstName = $"Stakeholder{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+S{Guid.NewGuid():N}@agiletransformation.com"
            };
        }

        public static CreateReviewerRequest GetReviewer()
        {
            return new CreateReviewerRequest
            {
                FirstName = $"Reviewer{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+R{Guid.NewGuid():N}@agiletransformation.com",
                RoleTags = new List<RoleResponse>
                {
                    new RoleResponse
                    {
                        Tags = new List<TagRoleResponse>
                        {
                            new TagRoleResponse
                            {
                                Id = 1,
                                Name = "Reviewer"
                            }
                        }
                    }
                }
            };
        }

        public static AddMemberRequest GetValidPostTeamMember()
        {
            var unique = Guid.NewGuid().ToString();

            return new AddMemberRequest
            {
                FirstName = $"Member{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com",
                ExternalIdentifier = unique
            };
        }

        public static IndividualAssessmentMemberRequest GetValidIndividualPostTeamMember()
        {
            var unique = Guid.NewGuid().ToString();

            return new IndividualAssessmentMemberRequest
            {
                FirstName = $"Member{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com",
                ExternalIdentifier = unique
            };
        }

        public static UpdateTeamMemberRequest GetValidPutTeamMember()
        {
            var unique = Guid.NewGuid().ToString();

            return new UpdateTeamMemberRequest
            {
                FirstName = "Automation",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com",
                ExternalIdentifier = unique,
                Tags = new List<TagRequest>
                {
                    new TagRequest
                    {
                        Category = "Role",
                        Tags = new List<string>
                        {
                            "Developer", "QA Tester"
                        }
                    },
                    new TagRequest
                    {
                        Category = "Participant Group",
                        Tags = new List<string>
                        {
                            "Technical", "FTE"
                        }
                    }
                }
            };
        }

        public static UpdateTeamMemberRequest GetValidPutTeamMember_Update_Tags()
        {
            var unique = Guid.NewGuid().ToString();

            return new UpdateTeamMemberRequest
            {
                FirstName = "Automation",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com",
                ExternalIdentifier = unique,
                Tags = new List<TagRequest>
                {
                    new TagRequest
                    {
                        Category = "Role",
                        Tags = new List<string>
                        {
                            "Business Analyst", "QA Tester"
                        }
                    },
                    new TagRequest
                    {
                        Category = "Participant Group",
                        Tags = new List<string>
                        {
                            "Support"
                        }
                    }
                }
            };
        }

        public static AddStakeholderRequest GetValidPostTeamStakeholder()
        {
            var unique = Guid.NewGuid().ToString();

            return new AddStakeholderRequest()
            {
                FirstName = $"Stakeholder{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com",
                ExternalIdentifier = unique
            };
        }

        public static CreateReviewerRequest GetValidReviewer()
        {
            var unique = Guid.NewGuid().ToString();

            return new CreateReviewerRequest()
            {
                FirstName = $"Reviewer{Guid.NewGuid()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com"
            };
        }

        public static UpdateReviewerRequest UpdateValidReviewer(int reviewerId)
        {
            var unique = Guid.NewGuid().ToString();

            return new UpdateReviewerRequest()
            {
                Id = reviewerId,
                FirstName = $"UpdatedReviewer{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName + unique,
                Email = $"{unique}@test.com"
            };
        }

        public static MemberTagRequest GetMemberTagRequest(string email, string teamExternalId)
        {
            return new MemberTagRequest
            {
                Email = email,
                TeamExternalIdentifier = teamExternalId,
                Category = "Role",
                Tags = new List<string>
                {
                    "Business Analyst", "QA Tester"
                }
            };
        }

        public static MemberTagRequest GetStakeholderTagRequest(string email, string teamExternalId)
        {
            return new MemberTagRequest
            {
                Email = email,
                TeamExternalIdentifier = teamExternalId,
                Category = "Role",
                Tags = new List<string>
                {
                    "Manager"
                }
            };
        }

        public static AddMembers GetMemberForBulkImport(string teamExternalIdentifier)
        {
            return new AddMembers
            {
                FirstName = $"BulkM{RandomDataUtil.GetFirstName():D}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+BM{Guid.NewGuid():D}@agiletransformation.com",
                Hash = Guid.NewGuid().ToString("D"),
                IsStakeholder = false,
                Roles = new List<string> { "Developer", "QA Tester" },
                ParticipantGroups = new List<string> { "Technical", "FTE" },
                TeamExternalIdentifier = teamExternalIdentifier
            };
        }

        public static AddMembers GetStakeholderForBulkImport(string teamExternalIdentifier)
        {
            return new AddMembers
            {
                FirstName = $"BulkS{RandomDataUtil.GetFirstName():D}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+BS{Guid.NewGuid():D}@agiletransformation.com",
                Hash = Guid.NewGuid().ToString("D"),
                IsStakeholder = true,
                Roles = new List<string> { "Manager", "Sponsor" },
                ParticipantGroups = new List<string>(),
                TeamExternalIdentifier = teamExternalIdentifier
            };
        }
    }
}