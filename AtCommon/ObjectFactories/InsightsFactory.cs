using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Analytics;
using AtCommon.Dtos.Analytics.Custom;
using AtCommon.Dtos.Analytics.StructuralAgility;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Insights;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class InsightsFactory
    {
        public static UpdateUserWidgetPreferenceRequest GetValidWidgetPreferenceRequest(Guid widgetUid)
        {
            var widgetBody = new UpdateUserWidgetPreferenceRequest()
            {
                DashboardWidgetUid = widgetUid,
                Preference = "{\"selectedView\":\"5\",\"selectedSubDimension\":\"1\",\"selectedTimespan\":\"5\",\"selectedSort\":\"0\"}"
            };

            return widgetBody;
        }

        public static IndexTeamRequest GetIndexTeamRequest(WidgetType widgetType, Stage stage, int teamId)
        {
            return new IndexTeamRequest
            {
                WidgetType = widgetType,
                Stage = stage,
                RadarType = 1,
                TeamIds = new List<int> { teamId },
                Page = 1,
                PageSize = 4,
                FilterBy = "",
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };
        }

        public static IndexTeamResponse GetIndexTeamByMaturity(Stage stage)
        {
            var teamName = stage switch
            {
                Stage.PreCrawl => SharedConstants.InsightsIndividualTeam1,
                Stage.Walk => SharedConstants.InsightsIndividualTeam2,
                Stage.Run => SharedConstants.InsightsIndividualTeam3,
                Stage.Fly => SharedConstants.InsightsIndividualTeam4,
                _ => throw new Exception($"IndexTeam not found for <{stage:G}>")
            };
            return InsightsTeams.FirstOrDefault(t => t.Name == teamName);
        }

        public static PagedResponse<IndexTeamResponse> GetPagedIndexTeamResponse(Stage stage)
        {
            return new PagedResponse<IndexTeamResponse>
            {
                Results = new List<IndexTeamResponse> { GetIndexTeamByMaturity(stage) },
                CurrentPage = 1,
                PageCount = 1,
                PageSize = 4,
                RowCount = 1
            };

        }

        public static PagedResponse<IndexTeamResponse> GetEmptyPagedIndexTeamResponse()
        {
            return new PagedResponse<IndexTeamResponse>
            {
                CurrentPage = 1,
                PageCount = 0,
                PageSize = 4,
                RowCount = 0
            };
        }

        public static List<IndexTeamResponse> InsightsTeams = new List<IndexTeamResponse>
        {
            new IndexTeamResponse
            {
                TeamId = (int)AnalyticsMaturityTeamId.Precrawl,
                Name = SharedConstants.InsightsIndividualTeam1,
                WorkType = "Software Delivery",
                Email = SharedConstants.TeamMember2.Email.ToLower(),
                TeamMemberContact = SharedConstants.TeamMember2.FullName()
            },
            new IndexTeamResponse
            {
                TeamId = (int)AnalyticsMaturityTeamId.Walk,
                Name = SharedConstants.InsightsIndividualTeam2,
                WorkType = "Business Operations",
                Email = SharedConstants.TeamMember2.Email.ToLower(),
                TeamMemberContact = SharedConstants.TeamMember2.FullName()
            },
            new IndexTeamResponse
            {
                TeamId = (int)AnalyticsMaturityTeamId.Run,
                Name = SharedConstants.InsightsIndividualTeam3,
                WorkType = "Service and Support",
                Email = SharedConstants.TeamMember2.Email.ToLower(),
                TeamMemberContact = SharedConstants.TeamMember2.FullName()
            },
            new IndexTeamResponse
            {
                TeamId = (int)AnalyticsMaturityTeamId.Fly,
                Name = SharedConstants.InsightsIndividualTeam4,
                WorkType = "Software Delivery",
                Email = SharedConstants.TeamMember2.Email.ToLower(),
                TeamMemberContact = SharedConstants.TeamMember2.FullName()
            }
        };

        public static OvertimeAnalyticsRequest GetValidOvertimeRequest(WidgetType widgetType, int teamId,
            int? benchmarkType = null, int? numberOfQuarters = 1, DateTime? endQuarter = null)
        {
            endQuarter ??= AnalyticsQuarter.GetValidQuarters().Last().LastDayOfQuarter;
            var currentQuarter = new AnalyticsQuarter(endQuarter.Value);

            return new OvertimeAnalyticsRequest
            {
                WidgetType = widgetType,
                EndQuarter = currentQuarter.QuarterName,
                NumberOfQuarters = numberOfQuarters,
                BenchmarkType = benchmarkType,
                Tags = new List<int>(),
                TeamIds = new List<int> { teamId }
            };
        }

        public static IList<OvertimeAnalyticsResponse> GetValidOvertimeResponse(
            int assessmentCount, decimal resultPercentage, int? benchmarkAssessmentCount = null, decimal? benchmarkResultPercentage = null, int maxNumberOfQuarters = 1, DateTime? endQuarter = null)
        {
            endQuarter ??= AnalyticsQuarter.GetValidQuarters().Last().LastDayOfQuarter;
            var responses = AnalyticsQuarter.GetValidQuarters(maxNumberOfQuarters, endQuarter)
                .Select(quarter => new OvertimeAnalyticsResponse
                {
                    DateKey = quarter.LastDayOfQuarter,
                    QuarterName = quarter.QuarterName,
                    AssessmentCount = assessmentCount,
                    ResultPercentage = resultPercentage,
                    BenchmarkAssessmentCount = benchmarkAssessmentCount,
                    BenchmarkResultPercentage = benchmarkResultPercentage
                })
                .ToList();

            return responses;
        }

        public static IndexAnalyticsRequest GetIndexRequest(WidgetType widgetType, int teamId)
        {
            return new IndexAnalyticsRequest
            {
                WidgetType = widgetType,
                TeamIds = new List<int> { teamId },
                RadarType = null,
                BenchmarkType = 1
            };
        }

        public static PeopleByRoleRequest GetPeopleByRoleRequest(StructuralAgilityWidgetType widgetType, int teamId, int workTypeId = 8)
        {
            return new PeopleByRoleRequest
            {
                WidgetType = widgetType,
                WorkTypeId = workTypeId,
                TeamIds = new List<int> { teamId }
            };
        }

        public static PeopleByRoleResponse GetPeopleByRoleAverageResponse(int teamId)
        {
            return new PeopleByRoleResponse
            {
                HasResults = true,
                TotalPeople = 5,
                Parameters = new RequestParameters
                {
                    WidgetType = StructuralAgilityWidgetType.Average.ToString("D"),
                    WorkTypeId = 8,
                    TeamId = teamId
                },
                Data = new List<PeopleByRoleData>
                {
                    new PeopleByRoleData
                    {
                        WidgetType = "Summary",
                        ChildTeamName = "",
                        MemberRole = "Product Owner",
                        MemberCount = 2,
                        MemberRoleRank = 1
                    },
                    new PeopleByRoleData
                    {
                        WidgetType = "Summary",
                        ChildTeamName = "",
                        MemberRole = "Designer",
                        MemberCount = 1,
                        MemberRoleRank = 2
                    },
                    new PeopleByRoleData
                    {
                        WidgetType = "Summary",
                        ChildTeamName = "",
                        MemberRole = "QA Tester",
                        MemberCount = 1,
                        MemberRoleRank = 3
                    },
                    new PeopleByRoleData
                    {
                        WidgetType = "Summary",
                        ChildTeamName = "",
                        MemberRole = "Technical Lead",
                        MemberCount = 1,
                        MemberRoleRank = 4
                    },
                    new PeopleByRoleData
                    {
                        MemberRole = "Total",
                        MemberCount = 5,
                        MemberRoleRank = 999
                    }
                }
            };
        }

        public static PeopleByRoleResponse GetPeopleByRoleDistributionResponse(int teamId, List<TeamHierarchyResponse> children)
        {
            var response = new PeopleByRoleResponse
            {
                HasResults = true,
                TotalPeople = 0,
                Parameters = new RequestParameters
                {
                    WidgetType = StructuralAgilityWidgetType.Distribution.ToString("D"),
                    WorkTypeId = 8,
                    TeamId = teamId
                },
                Data = new List<PeopleByRoleData>()
            };

            foreach (var child in children)
            {
                var newList = new List<PeopleByRoleData>
                {
                    new PeopleByRoleData
                    {
                        MemberRole = "Product Owner",
                        MemberCount = 2,
                        MemberRoleRank = 1
                    },
                    new PeopleByRoleData
                    {
                        MemberRole = "Designer",
                        MemberCount = 1,
                        MemberRoleRank = 2
                    },
                    new PeopleByRoleData
                    {
                        MemberRole = "QA Tester",
                        MemberCount = 1,
                        MemberRoleRank = 3
                    },
                    new PeopleByRoleData
                    {
                        MemberRole = "Technical Lead",
                        MemberCount = 1,
                        MemberRoleRank = 4
                    }
                };

                newList.ForEach(p => { p.ChildTeamId = child.TeamId; p.ChildTeamName = child.Name; });

                response.Data = response.Data.Concat(newList).ToList();
            }

            return response;
        }

        public static UpdateRoleAllocationTargetsRequest GetRoleAllocationTargetsRequest(IList<RoleAllocationTargetsResponse> getResponse = null)
        {

            var request = new UpdateRoleAllocationTargetsRequest
            {
                RoleAllocationTargets = new List<RoleAllocationTargetsRequestDto>()
            };

            if (getResponse == null) return request;
            foreach (var role in getResponse)
            {
                request.RoleAllocationTargets.Add(new RoleAllocationTargetsRequestDto
                {
                    RoleAllocationTargetId = role.RoleAllocationTargetId,
                    CompanyId = role.CompanyId,
                    TeamId = role.TeamId,
                    WorkType = role.WorkType,
                    Role = role.Role,
                    Allocation = role.Allocation
                });
            }

            return request;
        }

        public static RoleAllocationRequest GetRoleAllocationAverageRequest(int teamId, IEnumerable<int> parentIds)
        {
            return new RoleAllocationRequest()
            {
                TeamIds = new List<int> { teamId },
                SelectedWorkType = 8,
                SelectedTeamParents = string.Join("&", parentIds)
            };
        }

        public static TeamRoleAllocationAverageRequest GetTeamRoleAllocationAverageRequest(int teamId, IEnumerable<int> parentIds)
        {
            return new TeamRoleAllocationAverageRequest()
            {
                TeamIds = new List<int> { teamId },
                SelectedWorkType = 8,
                SelectedTeamParents = string.Join("&", parentIds),
                WidgetType = 1
            };
        }

        public static AnalyticsTableResponse<RoleAllocationAverageResponse> GetRoleAllocationAverageResponse()
        {
            return new AnalyticsTableResponse<RoleAllocationAverageResponse>()
            {
                Table = new List<RoleAllocationAverageResponse>
                {
                    new RoleAllocationAverageResponse
                    {
                       WorkType = "Software Delivery",
                       MemberRole = "Product Owner",
                       MemberName = "AT_Mem 1",
                       TeamsSupportedCount = 1,
                       TeamsSupportedTarget = 1
                    },
                    new RoleAllocationAverageResponse
                    {
                        WorkType = "Software Delivery",
                        MemberRole = "Designer",
                        MemberName = "AT_Mem 5",
                        TeamsSupportedCount = 1,
                        TeamsSupportedTarget = 1
                    },
                    new RoleAllocationAverageResponse
                    {
                        WorkType = "Software Delivery",
                        MemberRole = "QA Tester",
                        MemberName = "AT_Mem 4",
                        TeamsSupportedCount = 1,
                        TeamsSupportedTarget = 1
                    },
                    new RoleAllocationAverageResponse
                    {
                        WorkType = "Software Delivery",
                        MemberRole = "Technical Lead",
                        MemberName = "AT_Mem 3",
                        TeamsSupportedCount = 1,
                        TeamsSupportedTarget = 1
                    },
                    new RoleAllocationAverageResponse
                    {
                        WorkType = "Software Delivery",
                        MemberRole = "Product Owner",
                        MemberName = "AT_Mem 2",
                        TeamsSupportedCount = 1,
                        TeamsSupportedTarget = 1
                    }
                }
            };
        }

        public static AnalyticsTableResponse<TeamRoleAllocationAverageResponse> GetTeamRoleAllocationAverageResponse()
        {
            return new AnalyticsTableResponse<TeamRoleAllocationAverageResponse>()
            {
                Table = new List<TeamRoleAllocationAverageResponse>
                {
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Technical Lead",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Product Owner",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 0,
                        ActualAllocation = 2
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Product Owner",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 4",
                        TargetAllocation = 0,
                        ActualAllocation = 2
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "QA Tester",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Designer",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "QA Tester",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 4",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Technical Lead",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 4",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Designer",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 4",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    }

                }
            };
        }

        public static AnalyticsTableResponse<TeamRoleAllocationAverageResponse> GetTeamRoleAllocationAverageTeamResponse()
        {
            return new AnalyticsTableResponse<TeamRoleAllocationAverageResponse>()
            {
                Table = new List<TeamRoleAllocationAverageResponse>
                {
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Technical Lead",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Product Owner",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 0,
                        ActualAllocation = 2
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "QA Tester",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    },
                    new TeamRoleAllocationAverageResponse
                    {
                        MemberRole = "Designer",
                        WorkType = "Software Delivery",
                        TeamName = "Individual Automation Radar Team 1",
                        TargetAllocation = 1,
                        ActualAllocation = 1
                    }
                }
            };
        }

        public static AgileNonAgileTeamsRequest GetAgileNonAgileTeamsAverageRequest(string selectedTeamCategoryName, StructuralAgilityWidgetType widgeType = StructuralAgilityWidgetType.Average)
        {
            return new AgileNonAgileTeamsRequest
            {
                WidgetType = widgeType,
                TeamIds = new List<int> { 0 },
                SelectedTeamCategoryName = selectedTeamCategoryName,
                SelectedTeamParents = "0"
            };
        }

        public static TeamStabilityRequest GetTeamStabilityRequest(StructuralAgilityWidgetType widgetType = StructuralAgilityWidgetType.Average)
        {
            return new TeamStabilityRequest
            {
                WidgetType = widgetType,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0"
            };
        }

        public static GrowthPlanAnalyticsRequest GetGrowthPlanAnalyticsRequest(GrowthItemCategory growthItemCategory,
            GrowthItemStatusType growthItemStatusType, int teamId, GrowthItemSegmentType segmentType)
        {
            return new GrowthPlanAnalyticsRequest
            {
                GrowthItemCategory = growthItemCategory,
                TeamIds = new List<int> { teamId },
                SubDimensionId = segmentType,
                GrowthItemStatusType = growthItemStatusType,
            };
        }

        public static GrowthPlanDetailsAnalyticsRequest GetGrowthPlanDetailsAnalyticsRequest(GrowthItemCategory category,
            GrowthItemStatusType status, GrowthItemDetailStatusType detailStatus, string selectedName, GrowthItemSegmentType segmentType, int teamId)
        {
            return new GrowthPlanDetailsAnalyticsRequest
            {
                GrowthItemCategory = category,
                GrowthItemStatusType = status,
                Page = 1,
                PageSize = 6,
                SubDimensionId = (int)segmentType,
                TeamIds = new List<int> { teamId },
                SelectedName = selectedName,
                SelectedStatusId = detailStatus,
                FilterBy = ""
            };
        }

        public static List<GrowthItem> GetGrowthItemsFromExcel()
        {
            var filePath = $@"{(new FileUtil()).GetBasePath()}Resources\TestData\InsightsWidgets\Growth Items.xlsx";

            var table = ExcelUtil.GetExcelData(filePath);

            return (from DataRow tableRow in table.Rows
                    select new GrowthItem
                    {
                        Title = tableRow.Field<string>("Title"),
                        Priority = tableRow.Field<string>("Priority"),
                        Type = tableRow.Field<string>("Type"),
                        Category = (GrowthItemCategory)Enum.Parse(typeof(GrowthItemCategory), tableRow.Field<string>("Category"), true),
                        Status = tableRow.Field<string>("Status"),
                        CompetencyTarget = tableRow.Field<string>("Competency Target").NormalizeSpace(),
                        Created = DateTime.Parse(tableRow.Field<string>("Created")),
                        Description = tableRow.Field<string>("Description"),
                        RadarType = tableRow.Field<string>("Radar Type"),
                        Team = tableRow.Field<string>("Team")
                    }).ToList();
        }

        public static Dictionary<string, int> GetGiTypesAndCounts(List<GrowthItem> gis, GrowthItemCategory growthItemCategory,
            GrowthItemStatusType growthItemStatusType, GrowthItemSegmentType segmentType, IList<string> teamNames = null)
        {
            var growthItems = GetFilteredGrowthItems(gis, growthItemCategory, growthItemStatusType, teamNames);

            return segmentType switch
            {
                GrowthItemSegmentType.Category => growthItems.Select(i => i.Type)
                    .Distinct()
                    .ToDictionary(type => type, type => growthItems.Count(i => i.Type == type)),
                GrowthItemSegmentType.Competency => growthItems.Select(i => i.CompetencyTarget)
                    .Distinct()
                    .ToDictionary(c => c, c => growthItems.Count(i => i.CompetencyTarget == c)),
                GrowthItemSegmentType.All => growthItems.Select(g => g.Status)
                    .Distinct()
                    .ToDictionary(c => c, c => growthItems.Count(i => i.Status == c)),
                _ => throw new Exception($"<{segmentType:G}> cannot be used due to lack of data.")
            };
        }

        public static List<GrowthItem> GetFilteredGrowthItems(List<GrowthItem> gis, GrowthItemCategory growthItemCategory,
            GrowthItemStatusType growthItemStatusType, IList<string> teamNames = null)
        {
            // first filter by category
            var items = gis.Where(gi => gi.Category == growthItemCategory);

            // second filter by status unless it's 'All'
            items = (growthItemStatusType == GrowthItemStatusType.All)
                ? items
                : items.Where(gi => gi.Status == (growthItemStatusType == GrowthItemStatusType.StartedWithAge ? GrowthItemStatusType.Started.GetDescription() : growthItemStatusType.GetDescription()));
            // third remove any that have Category 'Individual' since they don't show on Insights
            items = items.Where(i => i.Category != GrowthItemCategory.Individual && i.Status != string.Empty);
            // fourth filter by team names
            if (teamNames != null)
                items = items.Where(i => teamNames.Contains(i.Team));

            var growthItems = items.ToList();

            return growthItems;
        }

        public static TeamWorkTypeRequest GetValidTeamWorkTypeRequest()
        {
            return new TeamWorkTypeRequest
            {
                TeamIds = new List<int> { 0 },
            };
        }

        public static ParticipantGroupRequest GetValidParticipantGroupRequest(StructuralAgilityWidgetType widgetType = StructuralAgilityWidgetType.Average)
        {
            return new ParticipantGroupRequest
            {
                WidgetType = widgetType,
                TeamIds = new List<int> { 0 },
                SelectedTeamParents = "0",
                Filter = 0
            };
        }


        public static DimensionsAnalyticsRequest GetDimensionsRequest(WidgetType widgetType, DimensionSortOrder sortOrder, int teamId, int? benchmarkType = null)
        {
            return new DimensionsAnalyticsRequest
            {
                WidgetType = widgetType,
                DimensionSortOrder = sortOrder,
                BenchmarkType = benchmarkType,
                TeamIds = new List<int> { teamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };
        }

        public static IList<DimensionsResponse> GetDimensionItemsResponse(WidgetType widgetType,
            decimal avgValue, decimal? benchmarkValue = null)
        {
            var segments = widgetType switch
            {
                WidgetType.Maturity => new List<string>
                {
                    "Performance",
                    "Foundation",
                    "Culture",
                    "Clarity",
                    "Leadership",
                    "Customer",
                    "Connections",
                    "Work",
                    "Mindset"
                },
                WidgetType.Performance => new List<string>
                {
                    "Predictability",
                    "Deployment Frequency",
                    "Lead Time to Deploy/Change",
                    "Feature Throughput",
                    "MTTR (Mean Time To Recover)",
                    "Change Failure Rate",
                    "Test Automation",
                    "Defect Ratio",
                    "Story Cycle Time",
                    "Feature Cycle Time",
                    "Impediments"
                },
                _ => throw new ArgumentOutOfRangeException(nameof(widgetType), widgetType, null)
            };

            return segments.Select(segment => new DimensionsResponse
            {
                Dimension = segment,
                ResultPercentage = avgValue,
                BenchmarkResultPercentage = benchmarkValue
            }).ToList();


        }

        public static IndexDimensionsRequest GetIndexDimensionsRequest(int benchmarkType, int teamId = 0)
        {
            return new IndexDimensionsRequest
            {
                BenchmarkType = benchmarkType,
                TeamIds = new List<int> { teamId },
                EndQuarter = AnalyticsQuarter.GetValidQuarters().Last().QuarterName
            };
        }

        public static IList<DimensionsResponse> GetIndexDimensionsResponse(decimal result,
            decimal? benchmark = null)
        {
            return new List<DimensionsResponse>
            {
                new DimensionsResponse
                {
                    Dimension = "Maturity",
                    ResultPercentage = result,
                    BenchmarkResultPercentage = benchmark
                },
                new DimensionsResponse
                {
                    Dimension = "Performance",
                    ResultPercentage = result,
                    BenchmarkResultPercentage = benchmark
                }
            };
        }
    }
}