namespace AtCommon.Dtos.Teams
{
    public class AddStakeholderRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ExternalIdentifier { get; set; }
    }
}
