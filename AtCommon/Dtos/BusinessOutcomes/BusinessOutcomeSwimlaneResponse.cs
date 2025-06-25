using System;
using System.Collections.Generic;
using AtCommon.Api.Enums;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeSwimlaneResponse : BaseBusinessOutcomesDto
    {
        public Guid SwimlaneId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public SwimlaneType Type { get; set; }
        public int CompanyId { get; set; }
        public int? TeamId { get; set; }
        public string BackgroundColor { get; set; }
        public int ChildId { get; set; }

        public List<BusinessOutcomeResponse> BusinessOutcomes { get; set; }
    }
}