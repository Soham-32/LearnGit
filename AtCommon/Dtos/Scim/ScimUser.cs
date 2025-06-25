using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Scim
{
    public class ScimUser
    {
        public string UserName { get; set; }
        public Name Name { get; set; }
        public string DisplayName { get; set; }
        public string NickName { get; set; }
        public string PreferredLanguage { get; set; }
        public object Timezone { get; set; }
        public bool Active { get; set; }
        public List<Email> Emails { get; set; }
        public List<Photo> Photos { get; set; }
        public List<UserGroup> Groups { get; set; }
        public List<Role> Roles { get; set; }
        public string Id { get; set; }
        public string Uid { get; set; }
        public List<string> Schemas { get; set; }
        public ResourceMetadata Meta { get; set; }

        public class Email
        {
            public string Type { get; set; }
            public bool Primary { get; set; }
            public string Display { get; set; }
            public string Value { get; set; }

            [JsonProperty("$ref")] public string Ref { get; set; }
        }

        public class UserGroup
        {
            public string Type { get; set; }
            public bool Primary { get; set; }
            public string Display { get; set; }
            public string Value { get; set; }

            [JsonProperty("$ref")] public string Ref { get; set; }
        }

        public class ResourceMetadata
        {
            public string ResourceType { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }
            public string Location { get; set; }
            public string Version { get; set; }
        }

        public class Photo
        {
            public string Type { get; set; }
            public bool Primary { get; set; }
            public string Display { get; set; }
            public string Value { get; set; }

            [JsonProperty("$ref")] public string Ref { get; set; }
        }

        public class Role
        {
            public string Type { get; set; }
            public bool Primary { get; set; }
            public string Display { get; set; }
            public string Value { get; set; }

            [JsonProperty("$ref")] public string Ref { get; set; }
        }

        public class Errors
        {
            public string Status { get; set; }
            public string Detail { get; set; }
        }

    }
    public class Name
    {
        public string Formatted { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string HonorificPrefix { get; set; }
        public string HonorificSuffix { get; set; }
    }
}
