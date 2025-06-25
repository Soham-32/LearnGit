using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics
{
    public class AnalyticsTableResponse<TDto>
    {
        public AnalyticsTableResponse()
        {
            Table = new List<TDto>();
        }

        public List<TDto> Table { get; set; }
    }
}
