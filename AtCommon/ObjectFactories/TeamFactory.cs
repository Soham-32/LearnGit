using System;
using System.Collections.Generic;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class TeamFactory
    {
        public static AddTeamWithMemberRequest GetGoiTeam(string teamPrefix, int numberOfNewMembers = 0)
        {
            var request = new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Department = "Test Department",
                Description = RandomDataUtil.GetTeamDescription(),
                Bio = RandomDataUtil.GetTeamBio(),
                AgileAdoptionDate = DateTime.Now,
                FormationDate = DateTime.Now,
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest {Category = "Work Type", Tags = new List<string> {"Group of Individuals"}},
                    new TeamTagRequest {Category = "Methodology", Tags = new List<string> {"Scrum"}},
                    new TeamTagRequest {Category = "Business Lines", Tags = new List<string> {SharedConstants.TeamTag}}
                }
            };

            for (var i = 0; i < numberOfNewMembers; i++)
            {
                request.Members.Add(MemberFactory.GetTeamMember());
            }

            return request;
        }

        public static AddTeamWithMemberRequest GetNormalTeam(string teamPrefix, 
            int members = 0, int stakeholders = 0)
        {
            var request = new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}{CSharpHelpers.RandomNumber()}",
                Department = "Test Department",
                Description = RandomDataUtil.GetTeamDescription(),
                Bio = RandomDataUtil.GetTeamBio(),
                AgileAdoptionDate = DateTime.Now,
                FormationDate = DateTime.Now,
                ExternalIdentifier = Guid.NewGuid().ToString("D"),
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Work Type", Tags = new List<string> { SharedConstants.NewTeamWorkType }
                    },
                    new TeamTagRequest
                    {
                        Category = "Methodology", Tags = new List<string> { "Scrum" }
                    },
                    new TeamTagRequest
                    {
                        Category = "Business Lines", Tags = new List<string> { SharedConstants.TeamTag }
                    }
                }
            };

            for (var i = 0; i < members; i++)
            {
                request.Members.Add(MemberFactory.GetTeamMember());
            }
            for (var i = 0; i < stakeholders; i++)
            {
                request.Stakeholders.Add(MemberFactory.GetStakeholder());
            }

            return request;
        }

        public static AddTeamWithMemberRequest GetMultiTeam(string teamPrefix, string workType = "Program Team")
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            return new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}{CSharpHelpers.RandomNumber()}",
                Type = "MultiTeam",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = unique,
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio(),
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Business Lines",
                        Tags = new List<string> { SharedConstants.TeamTag }
                    },
                    new TeamTagRequest
                    {
                        Category = "MultiTeam Type",
                        Tags = new List<string> { workType }
                    }
                }
            };
        }

        public static AddTeamWithMemberRequest GetPortfolioTeam(string teamPrefix, string workType = "Portfolio Team")
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            return new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Type = "Enterprise",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = unique,
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Business Lines",
                        Tags = new List<string> { SharedConstants.TeamTag }
                    },
                    new TeamTagRequest
                    {
                        Category = "MultiTeam Type",
                        Tags = new List<string> { workType }
                    }
                }
            };
        }

        public static AddTeamWithMemberRequest GetEnterpriseTeam(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            return new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}{CSharpHelpers.RandomNumber()}",
                Type = "Enterprise",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio(),
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Business Lines",
                        Tags = new List<string> { SharedConstants.TeamTag }
                    },
                    new TeamTagRequest
                    {
                        Category = "Enterprise Team Type",
                        Tags = new List<string> { "Portfolio Team" }
                    }
                }
            };

        }
        public static AddTeamWithMemberRequest GetNTierTeam(string teamPrefix, string workType = "N-Tier Team")
        {
            var unique = Guid.NewGuid().ToString();

            return new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}{CSharpHelpers.RandomNumber()}",
                Type = "Outcome",
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Team Type",
                        Tags = new List<string> {workType}
                    }
                }
            };
        }

        public static AddTeamWithMemberRequest GetValidPostTeam(string teamPrefix)
            {
                return new AddTeamWithMemberRequest
                {
                    Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                    Department = RandomDataUtil.GetTeamDepartment(),
                    Description = RandomDataUtil.GetTeamDescription(),
                    Bio = RandomDataUtil.GetTeamBio(),
                    AgileAdoptionDate = DateTime.Now,
                    FormationDate = DateTime.Now,
                    Tags = new List<TeamTagRequest>
                    {
                        new TeamTagRequest {Category = "Work Type", Tags = new List<string> {SharedConstants.NewTeamWorkType}},
                        new TeamTagRequest {Category = "Business Lines", Tags = new List<string> {SharedConstants.TeamTag}}
                    }
                };
            }

        public static UpdateTeamRequest GetValidPutTeam(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            var team = new UpdateTeamRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio(),
                TeamTags = new List<TeamTagRequest>
                {
                    new TeamTagRequest
                    {
                        Category = "Work Type",
                        Tags = new List<string> { SharedConstants.NewTeamWorkType }
                    },
                    new TeamTagRequest
                    {
                        Category = "Business Lines",
                        Tags = new List<string> { SharedConstants.TeamTag }
                    }
                }
            };

            return team;
        }

        public static UpdateTeamRequest GetInvalidPutTeam(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            var team = new UpdateTeamRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Description = "",
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio()
            };

            return team;
        }

        public static AddTeamWithMemberRequest GetValidPostMultiTeam(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            var team = new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Type = "MultiTeam",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio()
            };


            team.Tags.Add(new TeamTagRequest { Category = "Business Lines", Tags = new List<string> { "Automation" } });
            team.Tags.Add(new TeamTagRequest { Category = "MultiTeam Type", Tags = new List<string> { "Program Team" } });

            return team;
        }

        public static AddTeamWithMemberRequest GetValidPostEnterpriseTeam(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            var team = new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Type = "Enterprise",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio()
            };

            team.Tags.Add(new TeamTagRequest { Category = "Business Lines", Tags = new List<string> { "Automation" } });
            team.Tags.Add(new TeamTagRequest { Category = "Enterprise Team Type", Tags = new List<string> { "Portfolio Team" } });

            return team;
        }

        public static AddTeamWithMemberRequest GetValidPostTeamWithMember(string teamPrefix)
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.Now;

            var team = new AddTeamWithMemberRequest
            {
                Name = $"{teamPrefix}{RandomDataUtil.GetTeamName()}",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = unique,
                Department = RandomDataUtil.GetTeamDepartment(),
                FormationDate = now,
                AgileAdoptionDate = now,
                Bio = RandomDataUtil.GetTeamBio(),
                Members = new List<AddMemberRequest>
                {
                    new AddMemberRequest
                    {
                        FirstName = RandomDataUtil.GetFirstName(),
                        LastName = SharedConstants.TeamMemberLastName + RandomDataUtil.GetLastName(),
                        Email =  $"{unique}@test.com",
                        ExternalIdentifier = unique
                    }

                },
                Stakeholders = new List<AddStakeholderRequest>
                {
                    new AddStakeholderRequest
                    {
                        FirstName = RandomDataUtil.GetFirstName(),
                        LastName = SharedConstants.Stakeholder1 + RandomDataUtil.GetLastName(),
                        Email =  $"{unique}@testEmail.com",
                        ExternalIdentifier = unique
                    }
                }
            };

            team.Tags.Add(new TeamTagRequest { Category = "Business Lines", Tags = new List<string> { "Automation" } });
            team.Tags.Add(new TeamTagRequest { Category = "Work Type", Tags = new List<string> { "Software Delivery" } });

            return team;
        }

        public static AddTeam GetTeamForBulkImport(List<string> parentTeams = null)
        {
            return new AddTeam
            {
                Name = $"BulkAdd{RandomDataUtil.GetTeamName():D}",
                Type = "Software Delivery",
                Description = RandomDataUtil.GetTeamDescription(),
                ExternalIdentifier = Guid.NewGuid().ToString("D"),
                Department =RandomDataUtil.GetTeamDepartment() ,
                FormationDate = DateTime.Today,
                AgileAdoptionDate = DateTime.Today,
                Bio = RandomDataUtil.GetTeamBio(),
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest{ Category = "Methodology", Tags = new List<string> { "Scrum" } }
                },
                ParentExternalIdentifiers = parentTeams ?? new List<string>()
            };
        }
    }
}