using System.Collections.Generic;

namespace AtCommon.Dtos.Scim
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public bool Active { get; set; }
        public string DisplayName { get; set; }
        public string PreferredLanguage { get; set; }
        public List<string> Schemas { get; set; }
        public string ExternalId { get; set; }
        public NameObject Name { get; set; }
        public List<Email> Emails { get; set; }

        public class Email
        {
            public bool Primary { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class NameObject
        {
            public string Formatted { get; set; }
            public string FamilyName { get; set; }
            public string GivenName { get; set; }
        }
    }
}
