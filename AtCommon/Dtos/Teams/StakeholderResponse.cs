namespace AtCommon.Dtos.Teams
{
    public class StakeholderResponse : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string ExternalIdentifier { get; set; }
    }

    public static class StakeholderResponseExtensions
    {
        public static string FullName(this StakeholderResponse response)
        {
            return $"{response.FirstName} {response.LastName}";
        }

        public static AddMemberRequest ToAddMemberRequest(this StakeholderResponse response)
        {
            return new AddMemberRequest
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                ExternalIdentifier = response.ExternalIdentifier,
                Uid = response.Uid
            };
        }
    }
}
