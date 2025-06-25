namespace AtCommon.Dtos.AhTrial
{
    public class AhTrialBaseCompanyResponse
    {
        public int? ErrorStatusCode { get; set; }
        public string Status { get; set; }
        public string Industry { get; set; }
        public string Country { get; set; }
        public string CompanyFirstName { get; set; }
        public string CompanyLastName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyAdmin { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Uid { get; set; }
    }
}