using System;
using AtCommon.Utilities;

namespace AtCommon.Dtos.Analytics.Custom
{
    public class ProductionEnvironment
    {
        public string Name { get; set; }
        public Uri LoginUrl { get; set; }
        public string Group { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public bool IsInsightsEnabled { get; set; }

        public string Environment => LoginUrl.Host.Split('.')[0];

        public static ProductionEnvironment Parse(string csvLine)
        {
            var values = csvLine.Split('|');
            var env = new ProductionEnvironment
            {
                Name = values[1].Trim(),
                LoginUrl = new Uri(values[2].Trim()),
                IsInsightsEnabled = CSharpHelpers.YesNoToBool(values[3].Trim().Replace("**", "")),
                Group = values[4].Trim().Replace("**", ""),
                IsActive = CSharpHelpers.YesNoToBool(values[5].Trim().Replace("**", ""))
            };
            return env;
        }
    }
}
