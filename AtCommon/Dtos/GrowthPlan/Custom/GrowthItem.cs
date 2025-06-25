using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.GrowthPlan.Custom
{

    public class GrowthItem
    {
        public string Id { get; set; }
        public string Rank { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Priority { get; set; }
        public string Size { get; set; }
        public int ExternalId { get; set; }
        public string AffectedTeams { get; set; }
        public string Color { get; set; }
        public string RadarType { get; set; }
        public string Dimension { get; set; }
        public string SubDimension { get; set; }
        public List<string> CompetencyTargets { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Comments { get; set; }
        public string Assessment { get; set; }
    }
}