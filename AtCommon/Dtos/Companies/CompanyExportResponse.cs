using Newtonsoft.Json.Linq;

namespace AtCommon.Dtos.Companies
{
    public class CompanyExportResponse
    {
        public JArray Teams { get; set; }
        public JArray Members { get; set; }
        public JArray Users { get; set; }
    }
}