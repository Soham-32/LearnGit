
using System;
using System.Collections.Generic;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.Dtos
{
    public class User
    {
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType Type { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public void VerifyType(List<UserType> validTypes)
        {
            if (!validTypes.Contains(Type))
                throw new Exception($"This test can not be run as <{Type:G}>.");
        }

        private bool IsType(UserType userType)
        {
            return Type == userType;
        }

        public bool IsMember()
        {
            return IsType(UserType.Member);
        }

        public bool IsCompanyAdmin()
        {
            return IsType(UserType.CompanyAdmin);
        }
        public bool IsBusinessLineAdmin()
        {
            return IsType(UserType.BusinessLineAdmin);
        }

        public bool IsTeamAdmin()
        {
            return IsType(UserType.TeamAdmin);
        }

        public bool IsOrganizationalLeader()
        {
            return IsType(UserType.OrganizationalLeader);
        }

        public bool IsSiteAdmin()
        {
            return IsType(UserType.SiteAdmin);
        }

        public bool IsPartnerAdmin()
        {
            return IsType(UserType.PartnerAdmin);
        }

        public AddMemberRequest ToAddMemberRequest()
        {
            return new AddMemberRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Username
            };
        }
    }

}
