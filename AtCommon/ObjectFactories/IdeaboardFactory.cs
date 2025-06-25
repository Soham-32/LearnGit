using System;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public class IdeaboardFactory
    {
        public static List<CreateCardRequest> GetCard(Guid assessmentUid, int? dimensionId, string columnName, string radarName, int noOfCards = 1, string text = null)
        {
            var listOfCards = new List<CreateCardRequest>();
            text = text == null ? RandomDataUtil.GetCompanyZipCode().ToString() : "Ideaboard Card";
            for (var i = 0; i < noOfCards; i++)
            {
                listOfCards.Add(
                    new CreateCardRequest
                    {
                        AssessmentUid = assessmentUid,
                        SignalRGroupName = assessmentUid.ToString(),
                        SignalRUserId = Guid.NewGuid().ToString(),
                        Name = radarName,
                        Card = new CardRequest
                        {
                            ItemId = 1,
                            IsCardMovedToDifferentColumn = false,
                            DimensionId = dimensionId,
                            ColumnName = columnName,
                            SortOrder = 1,
                            ItemText = text,
                            VoteCount = 0,
                            Votes = new Dictionary<string, int>()
                        }
                    }
                );
            }
            return listOfCards;
        }


        public static UpdateCardRequest UpdateCard(List<CreateCardResponse> cardResponse)
        {
            return new UpdateCardRequest
            {
                AssessmentUid = cardResponse.First().AssessmentUid,
                SignalRGroupName = cardResponse.First().AssessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString(),
                Card = new CardRequest
                {
                    ItemId = cardResponse.First().Card.ItemId,
                    IsCardMovedToDifferentColumn = false,
                    DimensionId = cardResponse.First().Card.DimensionId,
                    ColumnName = cardResponse.First().Card.ColumnName,
                    SortOrder = cardResponse.First().Card.SortOrder ?? default,
                    ItemText = Guid.NewGuid().ToString(),
                    VoteCount = cardResponse.First().Card.VoteCount,
                    Votes = new Dictionary<string, int>()
                }
            };
        }

        public static UpdateCardsRequest UpdateBulkCards(ApiResponse<IdeaBoardResponse> cardResponse, List<DimensionDetailResponse> dimensions)
        {
            var listOfCards = new List<CardRequest>();

            var i = 0;
            foreach (var card in cardResponse.Dto.Cards)
            {
                listOfCards.Add(new CardRequest
                {
                    ItemId = card.ItemId,
                    IsCardMovedToDifferentColumn = true,
                    DimensionId = dimensions[i].DimensionId,
                    ColumnName = dimensions[i].Name,
                    SortOrder =card.SortOrder ?? default,
                    ItemText = $"Updated bulk card from {card.ColumnName} column",
                    VoteCount = card.VoteCount,
                    Votes = new Dictionary<string, int>()
                });
                i++;
            }
            return new UpdateCardsRequest
            {
                AssessmentUid = cardResponse.Dto.AssessmentUid,
                SignalRGroupName = "Update Ideaboard",
                SignalRUserId = cardResponse.Dto.AssessmentUid.ToString(),
                Cards = listOfCards
            };
        }
        public static UpdateCardsRequest UpdateBulkCards(ApiResponse<IdeaBoardResponse> cardResponse)
        {
            var listOfCards = cardResponse.Dto.Cards.Select(card => new CardRequest
                {
                    ItemId = card.ItemId,
                    IsCardMovedToDifferentColumn = false,
                    DimensionId = card.DimensionId,
                    ColumnName = card.ColumnName,
                    SortOrder = card.SortOrder ?? default,
                    ItemText = RandomDataUtil.GetCompanyZipCode().ToString(),
                    VoteCount = card.VoteCount,
                    Votes = new Dictionary<string, int>()
                })
                .ToList();

            return new UpdateCardsRequest
            {
                AssessmentUid = cardResponse.Dto.AssessmentUid,
                SignalRGroupName = "Update Ideaboard",
                SignalRUserId = cardResponse.Dto.AssessmentUid.ToString(),
                Cards = listOfCards
            };
        }
        public static DeleteCardRequest DeleteCard(Guid assessmentUid, int itemId)
        {
            return new DeleteCardRequest
            {
                AssessmentUid = assessmentUid,
                SignalRGroupName = assessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString(),
                ItemId = itemId
            };
        }

        public static CreateGrowthPlanItemRequest CreateGrowthPlanItem(Guid assessmentUid, int? dimensionId, string category, int itemId, string cardText = "Created Growth Plan Thru API")
        {
            return new CreateGrowthPlanItemRequest
            {
                AssessmentUid = assessmentUid,
                SignalRGroupName = assessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString(),
                Card = new GrowthPlanItemCardRequest
                {
                    ItemId = itemId,
                    DimensionId = dimensionId,
                    GrowthPlanItemCategory = category,
                    ItemText = cardText,
                }

            };
        }

        public static UpdateGrowthPlanItemRequest UpdateGrowthPlanItem(Guid assessmentUid, int? dimensionId, string category, int itemId)
        {
            return new UpdateGrowthPlanItemRequest
            {
                AssessmentUid = assessmentUid,
                SignalRGroupName = assessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString(),
                Card = new UpdateGrowthPlanItemCardRequest
                {
                    ItemId = itemId,
                    DimensionId = dimensionId,
                    GrowthPlanItemId = 1,
                    GrowthPlanItemCategory = category,
                    ItemText = $"Updated - {Guid.NewGuid().ToString()}",
                }
            };
        }

        public static SortCardsRequests SortCardRequest(Guid assessmentUid)
        {
           return new SortCardsRequests
            {
                AssessmentUid = assessmentUid,
                SignalRGroupName = assessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString()
            };
        }
        public static SetVotesAllowedRequests SetVotesAllowedRequest(Guid assessmentUid, int companyId, int votesAllowed = 10)
        {
            return new SetVotesAllowedRequests
            {
                AssessmentUid = assessmentUid,
                VotesAllowed = votesAllowed,
                CompanyId = companyId
            };
        }

        public static ResetCardsRequests ResetCardsRequest(Guid assessmentUid)
        {
            return new ResetCardsRequests
            {
                AssessmentUid = assessmentUid,
                SignalRGroupName = assessmentUid.ToString(),
                SignalRUserId = Guid.NewGuid().ToString(),
            };
        }

    }
}