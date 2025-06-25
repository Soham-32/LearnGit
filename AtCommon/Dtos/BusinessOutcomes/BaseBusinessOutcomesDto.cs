namespace AtCommon.Dtos.BusinessOutcomes
{
    public abstract class BaseBusinessOutcomesDto : BaseDto
    {
        public bool IsDeleted { get; set; }
        public string Owner { get; set; }
        public void SetOwner(string firstname, string lastname)
        {
            this.Owner = $"{firstname} {lastname}";
        }
    }
}