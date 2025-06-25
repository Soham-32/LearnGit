namespace AtCommon.Dtos.Companies
{
    public class SubscriptionHistoryResponse
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string SubscriptionType { get; set; }
        public string DateContractSigned { get; set; }
        public string ContractEndDate { get; set; }
        public string TeamsLimit { get; set; }
        public string AssessmentsLimit { get; set; }
        public string ManagedSubscription { get; set; }
    }
}