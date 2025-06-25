using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class KeyResult
    {
        public string Title { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public Metric Metric { get; set; }
        public string Start { get; set; }
        public string Target { get; set; }

        public string Stretch { get; set; }
        public double Progress { get; set; }
        public bool IsImpact { get; set; }
        public Guid Uid { get; set; }
        public bool IsDeleted { get; set; }
        public int SortOrder { get; set; }
        public double Weight { get; set; }
        public string ProgressBar { get; set; }
        public string ProgressBarPercentage { get; set; }
        public string ProgressBarLabel { get; set; }

        public int? SubTargetsOrder { get; set; }
        public ICollection<BusinessOutcomeKeyResultSubTargetRequest> SubTargets { get; set; }

    }
}