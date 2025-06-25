using System.Collections.Generic;

namespace AtCommon.Dtos.Scim
{
    public class PatchGroupRequest
    {
        public List<string> Schemas { get; set; }
        public List<Operation> Operations { get; set; }

        public class Operation
        {
            public string Name { get; set; }
            public string Op { get; set; }
            public string Path { get; set; }
            public List<ValueObject> Value { get; set; }
        }

        public class ValueObject
        {
            public string DisplayName { get; set; }
            public string Value { get; set; }
        }
    }
}
