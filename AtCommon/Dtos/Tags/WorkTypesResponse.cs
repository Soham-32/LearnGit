using System.Collections.Generic;

namespace AtCommon.Dtos.Tags
{
    public class WorkTypesResponse
    {
        public List<WorkType> WorkTypes { get; set; }
    }
    public class WorkType
    {
        public bool Disabled { get; set; }
        public object Group { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
