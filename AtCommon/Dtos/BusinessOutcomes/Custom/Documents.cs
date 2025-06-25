using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class Documents : BaseBusinessOutcomesDto
    {
        public string Title { get; set; }
        public string Size { get; set; }
        public string AddedBy { get; set; }
        public string Date { get; set; }
        public string Parent { get; set; }
        public string CardType { get; set; }
        public string CardTitle { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
    }
}