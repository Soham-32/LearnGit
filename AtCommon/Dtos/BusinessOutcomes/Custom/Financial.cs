namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class Financial
    {
        public int RequestedBudget { get; set; }
        public int ApprovedBudget { get; set; }
        public string BudgetCategory { get; set; }

        public int TotalSpent { get; set; }

        public string CalculationMethod { get; set; }
    }
}