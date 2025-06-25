using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class Tag
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public object DeletedAt { get; set; }
        public string CategoryLabelUid { get; set; }
        public object CategoryLabelName { get; set; }
    }
}