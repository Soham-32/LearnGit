using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class CreateFacilitatorRequest
    {
        public string SearchName { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<string> FacilitatorIds { get; set; }
    }
}
