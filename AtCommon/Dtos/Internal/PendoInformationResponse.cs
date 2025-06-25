namespace AtCommon.Dtos.Internal
{
    public class PendoInformationResponse
    {
        public string UserId { get; set; }
        public string UserRole { get; set; } 
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyType { get; set; }
        public string Size { get; set; }
        public string LifeCycleStage { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
    }
}
