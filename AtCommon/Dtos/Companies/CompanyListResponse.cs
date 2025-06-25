namespace AtCommon.Dtos.Companies
{
    public class CompanyListResponse : BaseCompanyResponse
    {
        public int CompanyId { get; set; }
        public string Type { get; set; }
    }
}