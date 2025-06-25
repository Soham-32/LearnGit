using System;
using System.Collections.Generic;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class BatchFactory
    {
        public static BatchAssessment GetValidBatchDetails()
        {
            return new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.TeamForBatchAssessment
                    },
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.Team
                    }
                },
                StartDate = DateTime.Today.AddDays(0),
                EndDate = DateTime.Today.AddDays(30)
            };
        }
    }
}