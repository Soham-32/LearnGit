using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeChildCardRequest
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public string TeamName { get; set; }
        public int OverallProgress { get; set; }
        public int Status { get; set; }
        public string Priority { get; set; }
        public string CardColor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Tags { get; set; }
        public int Type { get; set; }
        public int PrettyId { get; set; }
        public int TeamId { get; set; }
        public int Health { get; set; }
        public bool IsChanged { get; set; }
    }

}
