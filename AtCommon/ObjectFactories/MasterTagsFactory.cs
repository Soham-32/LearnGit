using System.Collections.Generic;
using static AtCommon.Dtos.Tags.MasterTags;

namespace AtCommon.ObjectFactories
{
    public static class MasterTagsFactory
    {
        public static AllTags GetAllTags()
        {
            return new AllTags()
            {
                Teams = new List<Category>
                {
                    new Category()
                    {
                        CategoryName = "Work Type",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Software Delivery",
                                TagName = "Software Delivery"
                            },
                            new Tag()
                            {
                                ParentTagName = "Service and Support",
                                TagName = "Service and Support"
                            },
                            new Tag()
                            {
                                ParentTagName = "SAFe - Release Management",
                                TagName = "SAFe - Release Management"
                            },
                            new Tag()
                            {
                                ParentTagName = "Kiosk",
                                TagName = "Kiosk"
                            },
                            new Tag()
                            {
                                ParentTagName = "Group Of Individuals",
                                TagName = "Group Of Individuals"
                            },
                            new Tag()
                            {
                                ParentTagName = "Feature Team",
                                TagName = "Feature Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Business Operations",
                                TagName = "Business Operations"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Train",
                        Type = "Team",
                        Tags = new List<Tag>()
                    },

                    new Category()
                    {
                        CategoryName = "Tower",
                        Type = "Team",
                        Tags = new List<Tag>()
                    },

                    new Category()
                    {
                        CategoryName = "Team Formation",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Not Formed",
                                TagName = "Not Formed"
                            },
                            new Tag()
                            {
                                ParentTagName = "Forming",
                                TagName = "Forming"
                            },
                            new Tag()
                            {
                                ParentTagName = "Formed",
                                TagName = "Formed"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Strategic Objectives",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Objective 2",
                                TagName = "Objective 2"
                            },
                            new Tag()
                            {
                                ParentTagName = "Objective 1",
                                TagName = "Objective 1"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Programs",
                        Type = "Team",
                        Tags = new List<Tag>()
                    },

                    new Category()
                    {
                        CategoryName = "Product Lines",
                        Type = "Team",
                        Tags = new List<Tag>()
                    },

                    new Category()
                    {
                        CategoryName = "Portfolio",
                        Type = "Team",
                        Tags = new List<Tag>()
                    },

                    new Category()
                    {
                        CategoryName = "MultiTeam Type",
                        Type = "MultiTeam",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Business Line Team",
                                TagName = "Business Line Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Growth Mgmt.",
                                TagName = "Growth Mgmt."
                            },
                            new Tag()
                            {
                                ParentTagName = "Program Team",
                                TagName = "Program Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Product Line Team",
                                TagName = "Product Line Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Portfolio Team",
                                TagName = "Portfolio Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Group Of Individuals",
                                TagName = "Group Of Individuals"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chapter",
                                TagName = "Chapter"
                            }
                        }
                    },
                    new Category()
                    {
                        CategoryName = "Enterprise Team Type",
                        Type = "Enterprise",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Portfolio Team",
                                TagName = "Portfolio Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Enterprise Team",
                                TagName = "Enterprise Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Group Of Individuals",
                                TagName = "Group Of Individuals"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Coaching",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Embedded Coaching",
                                TagName = "Embedded Coaching"
                            },
                            new Tag()
                            {
                                ParentTagName = "Consult - OnDemand",
                                TagName = "Consult - OnDemand"
                            },
                            new Tag()
                            {
                                ParentTagName = "Active Coaching",
                                TagName = "Active Coaching"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Business Lines",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Automation",
                                TagName = "Automation"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Agile Adoption",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "In-Progress",
                                TagName = "In-Progress"
                            },
                            new Tag()
                            {
                                ParentTagName = "Activated",
                                TagName = "Activated"
                            }
                        }
                    }
                },
                TeamMembers = new List<Category>()
                {
                    new Category()
                    {
                        CategoryName = "Role",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Security",
                                TagName = "Security"
                            },
                            new Tag()
                            {
                                ParentTagName = "Solution Lead",
                                TagName = "Solution Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Subject Matter Expert (SME)/UAT",
                                TagName = "Subject Matter Expert (SME)/UAT"
                            },
                            new Tag()
                            {
                                ParentTagName = "Supervisor",
                                TagName = "Supervisor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Team Member",
                                TagName = "Team Member"
                            },
                            new Tag()
                            {
                                ParentTagName = "Team Member",
                                TagName = "Team Member"
                            },
                            new Tag()
                            {
                                ParentTagName = "Team Facilitator",
                                TagName = "Team Facilitator"
                            },
                            new Tag()
                            {
                                ParentTagName = "Scrum Master",
                                TagName = "Scrum Master"
                            },
                            new Tag()
                            {
                                ParentTagName = "System Admin",
                                TagName = "System Admin"
                            },
                            new Tag()
                            {
                                ParentTagName = "Scrum Master",
                                TagName = "Scrum Master"
                            },
                            new Tag()
                            {
                                ParentTagName = "Network Admin",
                                TagName = "Network Admin"
                            },
                            new Tag()
                            {
                                ParentTagName = "Sales",
                                TagName = "Sales"
                            },
                            new Tag()
                            {
                                ParentTagName = "Release Train Engineer",
                                TagName = "Release Train Engineer"
                            },
                            new Tag()
                            {
                                ParentTagName = "QA Tester",
                                TagName = "QA Tester"
                            },
                            new Tag()
                            {
                                ParentTagName = "Product Owner",
                                TagName = "Product Owner"
                            },
                            new Tag()
                            {
                                ParentTagName = "Product Owner",
                                TagName = "Product Owner"
                            },
                            new Tag()
                            {
                                ParentTagName = "Product Owner",
                                TagName = "Product Owner"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical Analyst",
                                TagName = "Technical Analyst"
                            },
                            new Tag()
                            {
                                ParentTagName = "Marketing",
                                TagName = "Marketing"
                            },
                            new Tag()
                            {
                                ParentTagName = "Manager",
                                TagName = "Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Scrum Master",
                                TagName = "Scrum Master"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical Lead",
                                TagName = "Technical Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Manager",
                                TagName = "Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Lead",
                                TagName = "Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Individual",
                                TagName = "Individual"
                            },
                            new Tag()
                            {
                                ParentTagName = "Lead",
                                TagName = "Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Administrative",
                                TagName = "Administrative"
                            },
                            new Tag()
                            {
                                ParentTagName = "Agile Coach",
                                TagName = "Agile Coach"
                            },
                            new Tag()
                            {
                                ParentTagName = "Auditor",
                                TagName = "Auditor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Business Analyst",
                                TagName = "Business Analyst"
                            },
                            new Tag()
                            {
                                ParentTagName = "Data Specialist",
                                TagName = "Data Specialist"
                            },
                            new Tag()
                            {
                                ParentTagName = "DBA",
                                TagName = "DBA"
                            },
                            new Tag()
                            {
                                ParentTagName = "Architect",
                                TagName = "Architect"
                            },
                            new Tag()
                            {
                                ParentTagName = "Designer",
                                TagName = "Designer"
                            },
                            new Tag()
                            {
                                ParentTagName = "Account Manager",
                                TagName = "Account Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chief Product Owner",
                                TagName = "Chief Product Owner"
                            },
                            new Tag()
                            {
                                ParentTagName = "Developer",
                                TagName = "Developer"
                            }
                        }
                    },

                     new Category()
                     {
                        CategoryName = "Participant Group",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Decade or More",
                                TagName = "Decade or More"
                            },
                            new Tag()
                            {
                                ParentTagName = "Collocated",
                                TagName = "Collocated"
                            },
                            new Tag()
                            {
                                ParentTagName = "Contractor",
                                TagName = "Contractor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Distributed",
                                TagName = "Distributed"
                            },
                            new Tag()
                            {
                                ParentTagName = "FTE",
                                TagName = "FTE"
                            },
                            new Tag()
                            {
                                ParentTagName = "Leadership Team",
                                TagName = "Leadership Team"
                            },
                            new Tag()
                            {
                                ParentTagName = "Multinational",
                                TagName = "Multinational"
                            },
                            new Tag()
                            {
                                ParentTagName = "Support",
                                TagName = "Support"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical",
                                TagName = "Technical"
                            }
                        }
                     },

                     new Category()
                     {
                        CategoryName = "Role",
                        Type = "MultiTeam",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                ParentTagName = "Chief Solution Lead",
                                TagName = "Chief Solution Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chief Product Owner",
                                TagName = "Chief Product Owner"
                            },
                            new Tag()
                            {
                                ParentTagName = "Architect",
                                TagName = "Architect"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical SME",
                                TagName = "Technical SME"
                            },
                            new Tag()
                            {
                                ParentTagName = "Sponsor",
                                TagName = "Sponsor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Sponsor",
                                TagName = "Sponsor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Sponsor",
                                TagName = "Sponsor"
                            },
                            new Tag()
                            {
                                ParentTagName = "Resource Manager",
                                TagName = "Resource Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Resource Manager",
                                TagName = "Resource Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Resource Manager",
                                TagName = "Resource Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Program Manager",
                                TagName = "Program Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Product Manager",
                                TagName = "Product Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Portfolio Manager",
                                TagName = "Portfolio Manager"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chief Solution Lead",
                                TagName = "Chief Solution Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chief Solution Lead",
                                TagName = "Chief Solution Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Chief Solution Lead",
                                TagName = "Chief Solution Lead"
                            },
                            new Tag()
                            {
                                ParentTagName = "Business SME",
                                TagName = "Business SME"
                            },
                            new Tag()
                            {
                                ParentTagName = "Business SME",
                                TagName = "Business SME"
                            },
                            new Tag()
                            {
                                ParentTagName = "Business SME",
                                TagName = "Business SME"
                            },
                            new Tag()
                            {
                                ParentTagName = "Architect",
                                TagName = "Architect"
                            },
                            new Tag()
                            {
                                ParentTagName = "Architect",
                                TagName = "Architect"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical SME",
                                TagName = "Technical SME"
                            },
                            new Tag()
                            {
                                ParentTagName = "Technical SME",
                                TagName = "Technical SME"
                            }
                        }
                     }

                },
                Stakeholders = new List<Category>()
                {
                    new Category()
                    {
                        CategoryName = "Role",
                        Type = "Team",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                TagName = "Manager"
                            },
                            new Tag()
                            {
                                TagName = "Executive"
                            },
                            new Tag()
                            {
                                TagName = "Engineering Director"
                            },
                            new Tag()
                            {
                                TagName = "Customer"
                            },
                            new Tag()
                            {
                                TagName = "Sponsor"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Role",
                        Type = "MultiTeam",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                TagName = "Customer"
                            },
                            new Tag()
                            {
                                TagName = "Sponsor"
                            },
                            new Tag()
                            {
                                TagName = "Manager"
                            },
                            new Tag()
                            {
                                TagName = "Executive"
                            }
                        }
                    },

                    new Category()
                    {
                        CategoryName = "Role",
                        Type = "Individual",
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                TagName = "Team Member"
                            },
                            new Tag()
                            {
                                TagName = "Team Leaders (SM/PO/TL)"
                            },
                            new Tag()
                            {
                                TagName = "Reviewer"
                            },
                            new Tag()
                            {
                                TagName = "Manager/Leader"
                            },
                            new Tag()
                            {
                                TagName = "Direct Report"
                            },
                            new Tag()
                            {
                                TagName = "Customer"
                            },
                            new Tag()
                            {
                                TagName = "Peer"
                            }
                        }
                    }

                }
            };
        }
    }
}