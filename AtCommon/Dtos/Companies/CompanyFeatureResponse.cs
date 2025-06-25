using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyFeatureResponse
    {
        public CompanyFeatureResponse()
        {
            Features = new List<FeatureResponse>();
        }
        public int CompanyId { get; set; }
        public ICollection<FeatureResponse> Features { get; set; }
    }
}