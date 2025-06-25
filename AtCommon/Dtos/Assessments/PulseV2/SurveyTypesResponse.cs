using System;

namespace AtCommon.Dtos.Assessments.PulseV2
{
    public class SurveyTypesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Guid TeamUid { get; set; }
        public Guid Uid { get; set; }
    }
}
