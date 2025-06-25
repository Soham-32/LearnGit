using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CustomFieldValueResponse
    {
        public Guid Uid { get; set; }
        public string Value { get; set; }
        public Guid CustomFieldUid { get; set; }
        public string Name { get; set; }
    }
}