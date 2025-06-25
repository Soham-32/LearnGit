using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api;
using BusinessOutcomeLabelRequest = AtCommon.Dtos.BusinessOutcomes.BusinessOutcomeLabelRequest;
using AgilityHealth_Automation.Enum.BusinessOutcomes;

namespace AtCommon.ObjectFactories
{
    public static class BusinessOutcomesFactory
    {
        //API
        public static CustomBusinessOutcomeRequest GetValidPostBusinessOutcome(Company company, int swimlaneType = 1)
        {
            var metric = new Dictionary<string, string>
            {
                {"Value", "Cost"},
                {"Label", "Cost ($)"},
                {"TypeId", "3"},
                {"Name", "Cost"},
                {"Order", "3"},
                {"Uid", "3"}
            };

            var keyResult = new Dictionary<string, object>()
            {
                {"Start","0"},
                {"Target", "100"},
                {"Progress", 50.0},
                {"IsImpact", false}
            };

            var businessOutcome = new Dictionary<string, string>()
            {
                {"CardColor", "#34B2ED"},
                {"OverallProgress", "50"}
            };

            return GetValidBusinessOutcomeBody(company, metric, keyResult, businessOutcome, null, null, swimlaneType);
        }

        public static CustomBusinessOutcomeRequest GetValidPutBusinessOutcome(Company company, Guid businessOutcomeId,
            Guid keyResultId)
        {
            //var abc = company;

            var metric = new Dictionary<string, string>
            {
                {"Value", "Throughput"},
                {"Label", "Throughput (#)"},
                {"TypeId", "2"},
                {"Name", "Throughput"},
                {"Order", "13"},
                {"Uid", "13"}
            };

            var keyResult = new Dictionary<string, object>()
            {
                {"Start", "50"},
                {"Target", "200"},
                {"Progress", 100.0},
                {"IsImpact", true}
            };

            var businessOutcome = new Dictionary<string, string>()
            {
                {"CardColor", "#B70000"},
                {"OverallProgress", "33"}
            };


            return GetValidBusinessOutcomeBody(company, metric, keyResult, businessOutcome, businessOutcomeId,
                keyResultId);
        }

        public static CustomBusinessOutcomeRequest GetValidPutBusinessOutcomeWithComment(Company company,
            Guid businessOutcomeId, Guid keyResultId, User user)
        {

            var commentData = new CommentRequest()
            {
                ItemId = Guid.Empty,
                Content = "This is an automated API comment",
                Owner = user.Username,
                CommentDate = DateTime.UtcNow,
                Commenter = user.FirstName + " " + user.LastName,
                IsNew = true
            };

            var body = GetValidPutBusinessOutcome(company, businessOutcomeId, keyResultId);
            body.Comments.Add(commentData);
            return body;
        }

        private static CustomBusinessOutcomeRequest GetValidBusinessOutcomeBody(
            Company company,
            Dictionary<string, string> metric,
            Dictionary<string, object> keyResult,
            Dictionary<string, string> businessOutcome,
            Guid? businessOutcomeId = null,
            Guid? keyResultId = null,
            int swimlaneType = 1
        )
        {
            var unique = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;

            var metricData = new BusinessOutcomeMetricRequest()
            {
                TypeId = int.Parse(metric["TypeId"]),
                Name = metric["Name"],
                Order = int.Parse(metric["Order"]),
                Uid = int.Parse(metric["Uid"])
            };

            var keyResultData = new KeyResultRequest()
            {
                Title = "Key Result " + unique.Substring(0, 5),
                Metric = metricData,
                Start = keyResult["Start"].ToString(),
                Target = keyResult["Target"].ToString(),
                Progress = (double)keyResult["Progress"],
                IsImpact = bool.Parse(keyResult["IsImpact"].ToString())
            };

            var outcome = new CustomBusinessOutcomeRequest()
            {
                Title = "API " + unique + now,
                Description = "Description " + unique + now,
                KeyResults = new List<KeyResultRequest>(),
                BusinessValue = 1,
                InvestmentCategory = null,
                OverallProgress = float.Parse(businessOutcome["OverallProgress"]),
                TeamId = int.Parse(company.TeamId1),
                CompanyId = company.Id,
                SwimlaneType = swimlaneType,
                CardColor = businessOutcome["CardColor"],
                SortOrder = 5,
                Tags = new List<BusinessOutcomeTagRequest>(1),
                Comments = new List<CommentRequest>(1),
                CustomFieldValues = new List<CustomFieldValueRequest>(1),
                Uid = Guid.Empty,
                IsDeleted = false,
                CheckListItems = new List<BusinessOutcomeChecklistItemRequest>(),
                Deliverables = new List<DeliverableRequest>(),
                //ChildOutcomes = new List<ChildOutcome>(),
                Documents = new List<BusinessOutcomeAttachmentRequest>(),
                Financials = new List<BusinessOutcomeFinancialRequest>(),
                ChildCards = new List<BusinessOutcomeChildCardRequest>(),
            };

            if (keyResultId != null)
            {
                keyResultData.Uid = keyResultId.Value;
            }

            if (businessOutcomeId != null)
            {
                outcome.Uid = businessOutcomeId.Value;
                keyResultData.BusinessOutcomeId = businessOutcomeId.Value;
            }

            outcome.KeyResults.Add(keyResultData);

            return outcome;
        }

        //External Link
        public static BusinessOutcomesLinkRequest GetBusinessOutcomeLinkRequest(Guid businessOutcomeId)
        {
            return new BusinessOutcomesLinkRequest
            {
                Title = $"AH - {RandomDataUtil.GetBusinessOutcomeTitle()}",
                ExternalUrl = "https://agilityinsights.ai/",
                BusinessOutcomeUId = businessOutcomeId,
                LinkType = Guid.Empty
            };
        }

        public static BusinessOutcomeMetricRequest GetValidBusinessOutcomeAddMetricBody(int companyId = 0)
        {
            return new BusinessOutcomeMetricRequest
            {
                CompanyId = companyId,
                Name = $"Automation{RandomDataUtil.GetBusinessOutcomeTitle():D}",
                Order = 4,
                TypeId = 1
            };

        }

        //Dashboard
        public static List<BusinessOutcomeSortOrderRequestResponse> GetValidBusinessOutcomeSortOrderBody(
            CustomBusinessOutcomeRequest boItem1, CustomBusinessOutcomeRequest boItem2)
        {
            var requestBody = new List<BusinessOutcomeSortOrderRequestResponse>
            {
                new BusinessOutcomeSortOrderRequestResponse
                {
                    BusinessOutcomeUid = boItem1.Uid,
                    SortOrder = 1
                },

                new BusinessOutcomeSortOrderRequestResponse
                {
                    BusinessOutcomeUid = boItem2.Uid,
                    SortOrder = 2
                }
            };

            return requestBody;
        }

        public static CustomBusinessOutcomeRequest GetBusinessOutcomeRequest(Company company)
        {
            return new CustomBusinessOutcomeRequest
            {
                Title = "CategoryFilter" + DateTime.UtcNow,
                Description = "Description",
                KeyResults = new List<KeyResultRequest>(),
                BusinessValue = 1,
                InvestmentCategory = null,
                OverallProgress = 33,
                TeamId = int.Parse(company.TeamId1),
                CompanyId = company.Id,
                SwimlaneType = 2,
                BusinessOutcomeLabel = new List<BusinessOutcomeLabelRequest>(1),
                SortOrder = 5,
                Tags = new List<BusinessOutcomeTagRequest>(1),
                Comments = new List<CommentRequest>(1),
                CustomFieldValues = new List<CustomFieldValueRequest>(1),
                IsDeleted = false,
                SourceCategoryName = "Quarterly Outcomes"
            };
        }

        [Obsolete]
        public static Comment GetComment(string userName)
        {
            return new Comment
            {
                ItemId = Guid.Empty,
                Content = "This is an automated API comment",
                Commenter = userName,
                CommentDate = DateTime.UtcNow
            };
        }

        public static BusinessOutcomeRequest GetBaseBusinessOutcome(int companyId, SwimlaneType swimLaneType, List<string> owners = null)
        {
            return new BusinessOutcomeRequest
            {
                Title = $"AT_{swimLaneType.GetDescription()}_{RandomDataUtil.GetBusinessOutcomeTitle()}",
                KeyResults = new List<KeyResultRequest>(),
                CardColor = "#CCCCCC",
                Description = RandomDataUtil.GetBusinessOutcomeDescription(),
                Tags = new List<BusinessOutcomeTagRequest>(),
                Comments = new List<CommentRequest>(),
                Documents = new List<BusinessOutcomeAttachmentRequest>(),
                ChildCards = new List<BusinessOutcomeChildCardRequest>(),
                SourceCategoryName = swimLaneType.GetDescription(),
                TeamId = 0,
                CompanyId = companyId,
                SwimlaneType = swimLaneType,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                CustomFieldValues = new List<CustomFieldValueRequest>(),
                Financials = new List<BusinessOutcomeFinancialRequest>(),
                Owners = owners != null ? owners.Select(ownerId => new BusinessOutcomeOwnerRequest { UserId = ownerId }).ToList() : null

            };
        }

        public static BusinessOutcomeRequest GetBusinessOutcomeForUpdate(int companyId, SwimlaneType swimLaneType)
        {
            return new BusinessOutcomeRequest
            {
                Title = $"AT{RandomDataUtil.GetBusinessOutcomeTitle()}",
                KeyResults = new List<KeyResultRequest>(),
                CardColor = "#2BB251",
                Description = $"Updated {RandomDataUtil.GetBusinessOutcomeDescription()}",
                Tags = new List<BusinessOutcomeTagRequest>(),
                Comments = new List<CommentRequest>(),
                SourceCategoryName = swimLaneType.ToString("G"),
                TeamId = 0,
                CompanyId = companyId,
                SwimlaneType = swimLaneType
            };
        }

        public static CommentRequest GetCommentRequest(string owner)
        {
            return new CommentRequest
            {
                CommentDate = DateTime.Now,
                Content = $"Test comment {RandomDataUtil.GetBusinessOutcomeSourceCategoryName()}",
                Commenter = owner
            };
        }

        public static KeyResultRequest GetKeyResultRequest(int companyId, bool ui = true,int dividedWeight=100)
        {
            return new KeyResultRequest
            {
                Title = $"AT{RandomDataUtil.GetBusinessOutcomeTitle()}",
                Start = "0",
                Progress = CSharpHelpers.RandomNumber(2),
                Target = "100",
                Metric = ui ? GetUiMetric() : GetMetricRequest(companyId),
                Weight = dividedWeight
            };
        }

        public static BusinessOutcomeMetricRequest GetUiMetric()
        {
            return new BusinessOutcomeMetricRequest
            {
                Name = "NPS"
            };
        }

        public static BusinessOutcomeMetricRequest GetMetricRequest(int companyId)
        {
            return new BusinessOutcomeMetricRequest
            {
                Name = "NPS",
                TypeId = 2,
                Uid = 8,
                //CompanyId = companyId
            };
        }

        public static BusinessOutcomeCategoryLabelRequest GetBusinessOutcomeCategoryLabelRequest(int companyId,
            int numberOfTags = 0)
        {
            var body = new BusinessOutcomeCategoryLabelRequest
            {
                Name = RandomDataUtil.GetBusinessOutcomeTitle(),
                CompanyId = companyId,
                KanbanMode = false,
                BusinessOutcomeCardTypeId = 1,
                CategoryTypeId = 1
            };

            var tags = new List<BusinessOutcomeTagRequest>();
            for (var i = 0; i < numberOfTags; i++)
            {
                var tag = GetBusinessOutcomeTagRequest();
                tag.Order = i;
                tags.Add(tag);
            }

            body.Tags = tags;
            return body;
        }

        public static BusinessOutcomeTagRequest GetBusinessOutcomeTagRequest()
        {
            return new BusinessOutcomeTagRequest
            {
                Name = RandomDataUtil.GetBusinessOutcomeTitle(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CustomFieldRequest GetCustomFieldRequest()
        {
            return new CustomFieldRequest
            {
                Name = $"CF{CSharpHelpers.RandomNumber(8)}"
            };
        }

        public static UpdateBusinessOutcomeChecklistItemRequest GetChecklistItemRequest(List<string> owner = null)
        {
            owner ??= new List<string>();
            return new UpdateBusinessOutcomeChecklistItemRequest
            {
                ItemText = $"AT{RandomDataUtil.GetBusinessOutcomeTitle()}",
                TargetDate = DateTime.Today.AddDays(1),
                Owners = owner
            };
        }
        public static BusinessOutcomeObstaclesRequest GetBusinessOutcomeObstacleRequest(List<string> owners = null)
        {
            owners ??= new List<string>();
            return new BusinessOutcomeObstaclesRequest
            {
                Title = $"AT_obstacle_{RandomDataUtil.GetBusinessOutcomeTitle()}",
                Description = $"AT_obstacle_Description_{RandomDataUtil.GetChecklistItem()}",
                ObstacleType = CSharpHelpers.GetRandomEnumValue<BusinessOutcomesObstacles>(),
                Roam = CSharpHelpers.GetRandomEnumValue<RoamType>(), // Random RoamType Name
                Impact = CSharpHelpers.GetRandomEnumKey<ImpactLevel>().ToString(), // Random ImpactLevel Name
                Status = CSharpHelpers.GetRandomEnumValue<StatusType>(), // Random StatusType Name
                EndDate = DateTime.Today.AddDays(1),
                ObstacleOwners = owners.Select(ownerId => new BusinessOutcomeObstacleOwnerRequest() { UserId = ownerId }).ToList()
            };
        }

        // Generate the Target and Spent values for the Financials
        public static List<BusinessOutcomeFinancialRequest> GenerateTargetCurrentSpends(
            int companyId,
            int approvedBudget,
            DateTime startDate,
            bool useFixedRowCount = false,
            int fixedRowCount = 3
        )
        {
            return useFixedRowCount
                ? GenerateFixedRowsSpends(companyId, approvedBudget, fixedRowCount, startDate)
                : GenerateRandomTargetSpendRows(companyId, approvedBudget, startDate);
        }

        // Generated the Random Target and Spent values for the Financials for Random Rows
        private static List<BusinessOutcomeFinancialRequest> GenerateRandomTargetSpendRows(
            int companyId,
            int approvedBudget,
            DateTime startDate
        )
        {
            var random = new Random();
            var spends = new List<BusinessOutcomeFinancialRequest>();
            var remainingBudget = approvedBudget;
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

            while (remainingBudget >= 100)
            {
                var maxSpend = Math.Min(remainingBudget, approvedBudget / 3);
                var spendTarget = random.Next(100, maxSpend + 1);

                spends.Add(new BusinessOutcomeFinancialRequest
                {
                    BusinessOutcomeId = Guid.NewGuid(),
                    SpendingTarget = spendTarget,
                    CurrentSpent = random.Next(50, spendTarget),
                    FinancialAsOfDate = currentDate
                });

                remainingBudget -= spendTarget;
                currentDate = currentDate.AddMonths(random.Next(1, 3));
                if (random.NextDouble() < 0.1) break;
            }

            return spends;
        }

        // Generated the Fixed Target and Spent values for the Financials for Fixed Rows
        private static List<BusinessOutcomeFinancialRequest> GenerateFixedRowsSpends(
            int companyId,
            int approvedBudget,
            int rowCount,
            DateTime startDate
        )
        {
            var random = new Random();
            var spends = new List<BusinessOutcomeFinancialRequest>();
            var baseAmount = approvedBudget / rowCount;
            var remaining = approvedBudget;
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

            for (var i = 0; i < rowCount; i++)
            {
                var spendTarget = (i == rowCount - 1)
                    ? remaining
                    : random.Next(100, Math.Min(baseAmount + 1, remaining));

                spends.Add(new BusinessOutcomeFinancialRequest
                {
                    BusinessOutcomeId = Guid.NewGuid(),
                    SpendingTarget = spendTarget,
                    CurrentSpent = random.Next(50, spendTarget),
                    FinancialAsOfDate = currentDate
                });

                remaining -= spendTarget;
                currentDate = currentDate.AddMonths(1);
            }

            return spends;
        }

        public static Financial GenerateFinancial()
        {
            var random = new Random();
            var requestedBudget = random.Next(1, 1_000_001); // inclusive lower bound, exclusive upper
            var approvedBudget = random.Next(1, requestedBudget + 1); // same upper bound logic

            return new Financial
            {
                RequestedBudget = requestedBudget,
                ApprovedBudget = approvedBudget,
                BudgetCategory = "TestBudgetCategory(DoNotDelete)",
                TotalSpent = 0,
                CalculationMethod = "Manual"
            };
        }
        public static MeetingNotes GenerateMeetingNotes()
        {
            return new MeetingNotes
            {
                MeetingType = GetMeetingNoteTypeDropdownValues().Last(),
                MeetingTitle = "AT_MeetingNote_" + RandomDataUtil.GetBusinessOutcomeSourceCategoryName(),
                AddAttendees = SharedConstants.TeamMember1.FirstName + " " + SharedConstants.TeamMember1.LastName,
                Decisions = "Reviewed backlog and prioritized tasks"
            };
        }
        public static MeetingNotes GenerateUpdatedMeetingNotes()
        {
            return new MeetingNotes
            {
                MeetingType = GetMeetingNoteTypeDropdownValues().First(),
                MeetingTitle = "AT_MeetingNote_Updated_" + RandomDataUtil.GetBusinessOutcomeSourceCategoryName(),
                AddAttendees = SharedConstants.TeamMember2.FirstName + " " + SharedConstants.TeamMember2.LastName,
                Decisions = "Reviewed backlog and prioritized tasks updated"
            };
        }
        public static List<string> GetMeetingNoteTypeDropdownValues()
        {
            return new List<string>()
            {
                "Other",
                "Daily Standup",
                "Check In",
                "Planning",
                "Review",
                "Retrospective"
            };
        }
        public static List<string> GetAllMeetingNotesDropdownValues()
        {
            return new List<string>()
            {
                "My Meeting Notes",
                "New Notes",
                "Seen Notes",
                "Archived Notes"
            };
        }

        public static BusinessOutcomeDetailsTabs CreateDefaultKeyResultDetails()
        {
            return new BusinessOutcomeDetailsTabs()
            {
                Description = RandomDataUtil.GetBusinessOutcomeTitle(),
                Formula = "(New Coverage - Old Coverage) / Old Coverage * 100",
                Source = RandomDataUtil.GetBusinessOutcomeSourceCategoryName(),
                Frequency = "Monthly",
                Direction = "Increase",
                Comment = RandomDataUtil.GetGrowthPlanComment(),
                FinalTarget = "80%",
                EndDate = DateTime.Today.AddMonths(3).ToString("MM/dd/yyyy"),
            };
        }
    }
}

