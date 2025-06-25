using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class GetCompanyFeaturesRequest
    {
        public IEnumerable<int> FeatureIds { get; set; }
    }
}
