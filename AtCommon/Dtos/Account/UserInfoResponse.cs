using System;

namespace AtCommon.Dtos.Account
{
    public class UserInfoResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int ActiveCompanyId { get; set; }
        public string SmallAvatarUrl { get; set; }
        public string LargeAvatarUrl { get; set; }
    }
}
