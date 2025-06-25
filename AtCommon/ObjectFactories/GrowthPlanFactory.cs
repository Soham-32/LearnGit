using AtCommon.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api.Enums;
using AtCommon.Dtos.GrowthPlan;
using AtCommon.Dtos.GrowthPlan.Custom;

namespace AtCommon.ObjectFactories
{
    public static class GrowthPlanFactory
    {
        public static GrowthItem GetValidGrowthItem(string assessment = null)
        {
            return new GrowthItem
            {
                Category = GetGrowthPlanCategory().Select(s => s).Last(),
                Type = GetNewGrowthPlanTypes().Select(s => s).Last(),
                Title = RandomDataUtil.GetGrowthPlanTitle(),
                Status = GrowthItemStatusType.NotStarted.AsString(),
                TargetDate = DateTime.Now,
                CompetencyTargets = new List<string> {SharedConstants.SurveyCompetency},
                Priority = GetGrowthPlanPriority().Select(s => s).Last(),
                Size = "2",
                Description = RandomDataUtil.GetGrowthPlanDescription(),
                Owner = SharedConstants.TeamMember1.FirstName +" "+ SharedConstants.TeamMember1.LastName,
                Color = "#dfff82",
                Rank = "3",
                RadarType = SharedConstants.TeamAssessmentType,
                Comments = RandomDataUtil.GetGrowthPlanComment(),
                Assessment = assessment
            };
        }

        public static GrowthItem GetValidUpdatedGrowthItem(string assessment = null)
        {
            return new GrowthItem
            {
                Category = GetGrowthPlanCategory().Select(s => s).First(),
                Type = GetNewGrowthPlanTypes().Select(s => s).First(),
                Title = "updated_" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = GrowthItemStatusType.OnHold.AsString(),
                TargetDate = DateTime.Now.AddMinutes(2),
                CompetencyTargets = new List<string> { SharedConstants.SurveyCompetency },
                Priority = GetGrowthPlanPriority().Select(s => s).First(),
                Size = "3",
                Description = RandomDataUtil.GetGrowthPlanDescription(),
                Owner = SharedConstants.TeamMember2.FirstName + " " + SharedConstants.TeamMember2.LastName,
                Color = "#ffa365",
                Rank = "5",
                RadarType = SharedConstants.TeamHealthRadarName,
                Comments = "This is updated growth plan Item" + RandomDataUtil.GetGrowthPlanComment(),
                Assessment = assessment
            };
        }

        public static GrowthPlanItemRequest GrowthItemCreateRequest(int companyId, int teamId, IEnumerable<int> competencyTargets)
        {
            return new GrowthPlanItemRequest
            {
                CompanyId = companyId,
                Category = GetGrowthPlanCategory().Select(s => s).Last(),
                Description = RandomDataUtil.GetGrowthPlanDescription(),
                Id = 0,
                Owner = SharedConstants.TeamMember1.FirstName + SharedConstants.TeamMember1.LastName,
                Status = GrowthItemStatusType.NotStarted.AsString(),
                StatusId = 1,
                SurveyId = SharedConstants.TeamSurveyId,
                Priority = GetGrowthPlanPriority().Select(s => s).Last(),
                TargetDate = DateTime.Now,
                TeamId = teamId,
                Title = "APIGrowthItemTitle" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = GetNewGrowthPlanTypes().Select(s => s).Last(),
                CompetencyTargets = competencyTargets
            };
        }

        public static GrowthPlanItemRequest GrowthItemUpdateRequest(int companyId, int teamId, IEnumerable<int> competencyTargets, int id)
        {
            return new GrowthPlanItemRequest
            {
                CompanyId = companyId,
                Category = GetGrowthPlanCategory().Select(s => s).First(),
                Description = RandomDataUtil.GetGrowthPlanDescription(),
                Id = id,
                Owner = SharedConstants.TeamMember2.FirstName + SharedConstants.TeamMember2.LastName,
                Status = GrowthItemStatusType.StartedWithAge.AsString(),
                StatusId = 2,
                SurveyId = SharedConstants.TeamSurveyId,
                Priority = GetGrowthPlanPriority().Select(s => s).First(),
                TargetDate = DateTime.Now,
                TeamId = teamId,
                Title = "updated growth item" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = GetNewGrowthPlanTypes().Select(s => s).First(),
                CompetencyTargets = competencyTargets
            };
        }

        public static SaveCustomGrowthPlanTypesRequest CustomTypesCreateRequest(int companyId, int numberOfCustomTypes=1, int listId = 0)
        {
            var growthPlanTypesData = new List<CustomGrowthPlanType>();

            for (var i = 0; i < numberOfCustomTypes; i++)
            {
                var item = new CustomGrowthPlanType
                {
                    CompanyCustomListId = listId,
                    CustomText = "CustomType" + RandomDataUtil.GetCompanyCity() + CSharpHelpers.RandomNumber(),
                };
                growthPlanTypesData.Add(item);
            }

            return new SaveCustomGrowthPlanTypesRequest
            {
                CompanyId = companyId,
                CustomGrowthPlanTypes = growthPlanTypesData
            };
        }


        public static DeleteCustomGrowthPlanTypesRequest CustomTypesDeleteRequest(int companyId, IEnumerable<int> listId)
        {
            return new DeleteCustomGrowthPlanTypesRequest
            {
                CompanyId = companyId,
                CustomGrowthPlanTypeIds = listId
            };
        }

        public static List<string> GetClassicGrowthPlanTypes()
        {
            return new List<string>()
            {
                "Agile Ceremonies",
                "Backlog Management",
                "Coaching",
                "Estimating",
                "Knowledge Transfer",
                "Leadership/Management",
                "Other",
                "Planning",
                "Process Improvement",
                "Self-Study",
                "Technical Excellence",
                "Tools | Technology",
                "Training"
            };

        }

        public static List<string> GetNewGrowthPlanTypes()
        {
            return new List<string>()
            {
                "Agile Enablement",
                "Tech Agility & Tools",
                "Culture & Leadership",
                "Product & Program",
                "Other"
            };
        }

        public static List<string> GetGrowthPlanCategory()
        {
            return new List<string>()
            {
                "Team",
                "Management",
                "Organizational"
            };
        }
        public static List<string> GetTeamGrowthPlanCategories()
        {
            return new List<string>()
            {
                "Team",
                "Organizational"
            };
        }
        public static List<string> GetMultiTeamGrowthPlanCategories()
        {
            return new List<string>()
            {
                "Enterprise",
                "Organizational"
            };
        }
        public static List<string> GetEnterpriseTeamGrowthPlanCategories()
        {
            return new List<string>()
            {
                "Enterprise"
            };
        }
        public static List<string> GetIaParticipantGrowthPlanCategories()
        {
            return new List<string>()
            {
                "Individual",
                "Management"
            };
        }
        public static List<string> GetGrowthPlanPriority()
        {
            return new List<string>()
            {
                "Very Low",
                "Low",
                "Medium",
                "High",
                "Very High"
            };
        }

        public static List<string> GetGrowthPlanColumnNameList()
        {
            return new List<string>{
                "Id",
                "Rank",
                "Title",
                "Description",
                "Priority",
                "Owner",
                "Type",
                "Category",
                "Location",
                "Assessment",
                "Team",
                "Radar Type",
                "Status",
                "Created",
                "Updated By",
                "Completion Date",
                "Size",
                "Affected Teams",
                "Tags",
                "Reporting Manager",
                "Origination"
            };
        }
    }
}