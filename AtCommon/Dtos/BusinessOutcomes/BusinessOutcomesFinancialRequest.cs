using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeFinancialRequest
    {
        public int BusinessOutcomeFinancialId { get; set; }
        public Guid BusinessOutcomeId { get; set; }
        public int? CurrentSpent { get; set; }
        public int? SpendingTarget { get; set; }
        public DateTime? FinancialAsOfDate { get; set; }
    }
}
