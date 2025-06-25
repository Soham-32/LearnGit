
namespace AtCommon.Dtos.Companies
{
    public class AddCompanyRadarRequest
    {
        public int CompanyId { get; set; }
        public int SurveyId { get; set; }
        public bool Editable { get; set; }
    }
}
