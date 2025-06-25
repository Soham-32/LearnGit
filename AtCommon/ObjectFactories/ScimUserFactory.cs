using System;
using System.Collections.Generic;
using AtCommon.Dtos.Scim;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public class ScimUserFactory
    {
        public static CreateUserRequest GetScimUser()
        {
            var userId = $"ah_automation{RandomDataUtil.GetUserName():D}@test.com";
            const string familyName = "FamilyName";
            var givenName = $"ScimUser{RandomDataUtil.GetUserName()}";
            return new CreateUserRequest
            {
                UserName = userId,
                Active = true,
                DisplayName = userId,
                PreferredLanguage = "fao",
                Schemas = new List<string>
                {
                    "urn:ietf:params:scim:schemas:core:2.0:User"
                },
                ExternalId = Guid.NewGuid().ToString(),
                Name = new CreateUserRequest.NameObject()
                {
                    Formatted = $"{givenName} {familyName}",
                    FamilyName = familyName,
                    GivenName = givenName
                },
                Emails = new List<CreateUserRequest.Email>
                {
                    new CreateUserRequest.Email
                    {
                        Primary = true,
                        Type = "work",
                        Value = userId
                    }
                }
            };
        }

        public static CreateUserRequest GetScimEnterpriseUser()
        {
            var userId = $"ah_automation{RandomDataUtil.GetUserName():D}@test.com";
            const string familyName = "FamilyName";
            var givenName = $"ScimUser{RandomDataUtil.GetUserName()}";
            return new CreateUserRequest
            {
                UserName = userId,
                Active = true,
                DisplayName = userId,
                PreferredLanguage = "fao",
                Schemas = new List<string>
                {
                    "urn:ietf:params:scim:schemas:core:2.0:User",
                    "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User"
                },
                ExternalId = Guid.NewGuid().ToString(),
                Name = new CreateUserRequest.NameObject()
                {
                    Formatted = $"{givenName} {familyName}",
                    FamilyName = familyName,
                    GivenName = givenName
                },
                Emails = new List<CreateUserRequest.Email>
                {
                    new CreateUserRequest.Email
                    {
                        Primary = true,
                        Type = "work",
                        Value = userId
                    }
                }
            };
        }
        public static CreateUserRequest UpdateScimUser()
        {
            var userId = $"updated{RandomDataUtil.GetUserName():D}@test.com";
            const string familyName = "UpdatedFamilyName";
            var givenName = $"ScimUser{RandomDataUtil.GetUserName()}";
            return new CreateUserRequest
            {
                UserName = userId,
                Active = true,
                DisplayName = userId,
                PreferredLanguage = "fao",
                Schemas = new List<string>
                {
                    "urn:ietf:params:scim:schemas:core:2.0:User"
                },
                ExternalId = Guid.NewGuid().ToString(),
                Name = new CreateUserRequest.NameObject()
                {
                    Formatted = $"{givenName} {familyName}",
                    FamilyName = familyName,
                    GivenName = givenName
                },
                Emails = new List<CreateUserRequest.Email>
                {
                    new CreateUserRequest.Email
                    {
                        Primary = true,
                        Type = "work",
                        Value = userId
                    }
                }
            };
        }
    }
}