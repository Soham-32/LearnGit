using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiTests.v1.Endpoints;
using AtCommon.Api;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.Analytics.StructuralAgility
{
    [TestClass]
    [TestCategory("Insights")]
    public class PostAnalyticsParticipantGroupTests : BaseV1Test
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var user = User.IsSiteAdmin() || User.IsPartnerAdmin() ? User : InsightsUser;
                var companyHierarchy = setup.GetCompanyHierarchy(Company.InsightsId, user);

                _enterpriseTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsEnterpriseTeam1);
                _multiTeam = companyHierarchy.GetTeamByName(SharedConstants.InsightsMultiTeam1);
                //_multiTeam.Children.RemoveAt(0);
                _team = companyHierarchy.GetTeamByName(SharedConstants.InsightsIndividualTeam1);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Company_Average_Filter0_Success()
        {
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0",
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 5
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 5
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Company_Average_Filter1_Success()
        {
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0",
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 5
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 5
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Company_Distribution_Filter0_Success()
        {
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0",
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 5, ChildTeamName = _enterpriseTeam.Name, ChildTeamId = _enterpriseTeam.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 5, ChildTeamName = _enterpriseTeam.Name, ChildTeamId = _enterpriseTeam.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Company_Distribution_Filter1_Success()
        {
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0",
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 5, ChildTeamName = _enterpriseTeam.Name, ChildTeamId = _enterpriseTeam.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 5, ChildTeamName = _enterpriseTeam.Name, ChildTeamId = _enterpriseTeam.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Enterprise_Average_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamParents = "0",
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 5
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 5
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Enterprise_Average_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamParents = "0",
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 5
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 5
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Enterprise_Distribution_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamParents = "0",
                Filter = 0
            };

            var subTeam1 = _enterpriseTeam.Children[0];
            var subTeam2 = _enterpriseTeam.Children[1];

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 5, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 5, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Enterprise_Distribution_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _enterpriseTeam.TeamId },
                SelectedTeamParents = "0",
                Filter = 1
            };

            var subTeam1 = _enterpriseTeam.Children[0];
            var subTeam2 = _enterpriseTeam.Children[1];

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 5, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 5, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_MultiTeam_Average_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamParents = _multiTeam.ParentId.ToString(),
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_MultiTeam_Average_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamParents = _multiTeam.ParentId.ToString(),
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_MultiTeam_Distribution_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamParents = _multiTeam.ParentId.ToString(),
                Filter = 0
            };

            var subTeam1 = _multiTeam.Children[0];
            var subTeam2 = _multiTeam.Children[1];

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_MultiTeam_Distribution_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _multiTeam.TeamId },
                SelectedTeamParents = _multiTeam.ParentId.ToString(),
                Filter = 1
            };

            var subTeam1 = _multiTeam.Children[0];
            var subTeam2 = _multiTeam.Children[1];

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2, ChildTeamName = subTeam1.Name, ChildTeamId = subTeam1.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2, ChildTeamName = subTeam2.Name, ChildTeamId = subTeam2.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Team_Average_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamParents = _team.ParentId.ToString(),
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Team_Average_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Average,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamParents = _team.ParentId.ToString(),
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Total", MemberCount = 5, ChildTeamName = "Total"
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Team_Distribution_Filter0_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamParents = _team.ParentId.ToString(),
                Filter = 0
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "FTE", MemberCount = 3, ChildTeamName = _team.Name, ChildTeamId = _team.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Contractor", MemberCount = 2, ChildTeamName = _team.Name, ChildTeamId = _team.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Team_Distribution_Filter1_Success()
        {
            VerifySetup(_classInitFailed);
            // when
            var request = new ParticipantGroupRequest
            {
                WidgetType = StructuralAgilityWidgetType.Distribution,
                TeamIds = new List<int> { _team.TeamId },
                SelectedTeamParents = _team.ParentId.ToString(),
                Filter = 1
            };

            var expectedResponse = new AnalyticsTableResponse<ParticipantGroupResponse>
            {
                Table = new List<ParticipantGroupResponse>
                {
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Distributed", MemberCount = 3, ChildTeamName = _team.Name, ChildTeamId = _team.TeamId
                    },
                    new ParticipantGroupResponse
                    {
                        ParticipantGroup = "Collocated", MemberCount = 2, ChildTeamName = _team.Name, ChildTeamId = _team.TeamId
                    }
                }
            };

            await ParticipantGroupValidator(request, expectedResponse);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Unauthorized()
        {
            
            // given
            var client = GetUnauthenticatedClient();

            // when
            var participantGroup = InsightsFactory.GetValidParticipantGroupRequest();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsParticipantGroup(Company.InsightsId), participantGroup);

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_Forbidden()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var participantGroup = InsightsFactory.GetValidParticipantGroupRequest();
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsParticipantGroup(SharedConstants.FakeCompanyId), participantGroup);

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task Analytics_ParticipantGroup_Post_BadRequest()
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            var request = InsightsFactory.GetValidParticipantGroupRequest();
            request.WidgetType = StructuralAgilityWidgetType.BadRequest;

            // when
            var response = await client.PostAsync<IList<string>>(
                RequestUris.AnalyticsParticipantGroup(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status Code does not match.");
            Assert.AreEqual($"'Widget Type' has a range of values which does not include '{StructuralAgilityWidgetType.BadRequest:D}'.",
                response.Dto.FirstOrDefault(), "Error Message does not match.");
        }

        private async Task ParticipantGroupValidator(ParticipantGroupRequest request,
            AnalyticsTableResponse<ParticipantGroupResponse> expectedResponse)
        {
            // given
            var client = await GetInsightsAuthenticatedClient();

            // when
            var response = await client.PostAsync<AnalyticsTableResponse<ParticipantGroupResponse>>(
                RequestUris.AnalyticsParticipantGroup(Company.InsightsId), request);

            // then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status Code does not match.");

            Assert.AreEqual(expectedResponse.Table.Count, response.Dto.Table.Count,
                "The ParticipantGroupResponse List does not have the correct count.");
            foreach (var item in expectedResponse.Table)
            {
                var actualResult = response.Dto.Table.FirstOrDefault(r => r.ParticipantGroup == item.ParticipantGroup && r.ChildTeamName == item.ChildTeamName)
                    .CheckForNull($"{item.ParticipantGroup} ParticipantGroup was not found in the response.");
                Assert.AreEqual(item.MemberCount, actualResult.MemberCount, $"{item.ParticipantGroup} Members does not match.");
                Assert.AreEqual(item.ChildTeamId, actualResult.ChildTeamId,
                    $"{item.ParticipantGroup} Percentage does not match.");
                Assert.AreEqual(item.ChildTeamName, actualResult.ChildTeamName,
                    $"ChildTeamName does not match for <{item.ParticipantGroup}>");
            }
        }
    }
}
