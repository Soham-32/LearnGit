using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgilityHealth_Automation.Base;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes
{
    [TestClass]
    public class BusinessOutcomesBaseTest : BaseTest
    {

        protected static BusinessOutcomeResponse CreateBusinessOutcome(SwimlaneType swimLaneType, int numberOfKeyResults = 0, List<BusinessOutcomeTagResponse> tag = null, List<string> owners = null,BusinessOutcomeRequest businessOutcomeRequest= null)
        {
            var request = GetBusinessOutcomeRequest(swimLaneType, numberOfKeyResults, tag, owners,businessOutcomeRequest);
            return new SetupTeardownApi(TestEnvironment).CreateBusinessOutcome(request);
        }

     
        protected static BusinessOutcomeRequest GetBusinessOutcomeRequest(SwimlaneType swimLaneType, int numberOfKeyResults = 0, List<BusinessOutcomeTagResponse> tag = null, List<string> owners = null,BusinessOutcomeRequest businessOutcomeRequest= null)
        {
            var request = BusinessOutcomesFactory.GetBaseBusinessOutcome(Company.Id, swimLaneType);
            if (tag != null)
            {
               var tags = new List<BusinessOutcomeTagRequest>()
                {
                    new BusinessOutcomeTagRequest()
                    {
                        Uid = tag.First().Uid
                    }
                };

                request.Tags = tags;
            }

             if (businessOutcomeRequest != null && businessOutcomeRequest.Obstacles != null)
             {
                 request.Obstacles = businessOutcomeRequest.Obstacles;
             }


            for (var i = 0; i < numberOfKeyResults; i++)
            {
                request.KeyResults.Add(BusinessOutcomesFactory.GetKeyResultRequest(Company.Id, false, (numberOfKeyResults == 0) ? 100 : (100 / numberOfKeyResults)));
            }

            request.OverallProgress = long.Parse(GetKeyResultsProgressPercentage(request.KeyResults));

            return request;
        }
        protected static string GetKeyResultsProgressPercentage(List<KeyResultRequest> keyResults)
        {
            return keyResults.Any() ? Math.Round((keyResults.Select(k => k.Progress).Sum() * 100 / keyResults.Select(k => k.Target.ToInt()).Sum()), MidpointRounding.AwayFromZero)
                .ToString(CultureInfo.InvariantCulture) : "0";
        }

        protected static string CustomGetKeyResultsProgressPercentage(KeyResultRequest keyResultRequest)
        {
            return keyResultRequest != null && keyResultRequest.Target.ToInt() > 0
                ? Math.Round((keyResultRequest.Progress * 100 / keyResultRequest.Target.ToInt()), MidpointRounding.AwayFromZero)
                    .ToString(CultureInfo.InvariantCulture)
                : "0";
        }
        protected static List<int> DistributedWeights(int keyResultCount)
        {
            var weights = new List<int>();
            if (keyResultCount <= 0) return weights;

            var baseWeight = 100 / keyResultCount;  // Base weight for each KR
            var remainder = 100 % keyResultCount;   // Remainder to be distributed

            for (var i = 0; i < keyResultCount; i++)
            {
                if (i < remainder)
                    weights.Add(baseWeight + 1); // Distribute remainder to the first few
                else
                    weights.Add(baseWeight);
            }

            return weights;
        }


        protected static string GetChecklistProgressPercentage(List<UpdateBusinessOutcomeChecklistItemRequest> checklist)
        {
            if (!checklist.Any()) return "0";
            var totalItems = checklist.Count;

            var completedItems = checklist.Count(n => n.IsComplete.Equals(true));
            return completedItems > 0 ? Math.Round((completedItems / (double)totalItems) * 100, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture) : "0";
        }

        protected static string GetDeliverablesProgressPercentage(List<DeliverableTabChildCard> deliverableChildCards)
        {
            return deliverableChildCards.Any() ? Math.Round(deliverableChildCards.Select(b => b.Progress.ToInt()).ToList().Average(), MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture) : "0";
        }
    }
}