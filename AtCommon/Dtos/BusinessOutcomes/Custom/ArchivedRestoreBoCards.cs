using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class ArchivedRestoreBoCards
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TeamName { get; set; }
        public string CreatedBy { get; set; }
        public string ArchivedBy { get; set; }
        public string RestoredBy { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public DateTime? RestoredAt { get; set; }

    }
}
