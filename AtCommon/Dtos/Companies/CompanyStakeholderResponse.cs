using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class CompanyStakeholderResponse
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int LastRowOnPage { get; set; }
        public int PageCount { get; set; }
        public int RowCount { get; set; }
        public List<StakeholderResponseDto> Stakeholders { get; set; }
    }

    public class StakeholderResponseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string ExternalIdentifier { get; set; }
        public int TeamId { get; set; }
        public int StakeholderId { get; set; }
        public List<CompanyStakeholderTagModel> Tags { get; set; }
    }

    public class CompanyStakeholderTagModel
    {
        public string Category { get; set; }
        public List<StakeholderTagModel> Tags { get; set; }
    }

    public class StakeholderTagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
