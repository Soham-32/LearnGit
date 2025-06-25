using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class CustomFieldValue
    {
        public Guid CustomFieldUid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}