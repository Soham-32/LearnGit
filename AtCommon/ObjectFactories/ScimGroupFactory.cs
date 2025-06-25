using AtCommon.Dtos.Scim;
using System.Collections.Generic;

namespace AtCommon.ObjectFactories
{
    public class ScimGroupFactory
    {
        public static PatchGroupRequest GetScimPatchGroupAddUser(string userId)
        {
            return new PatchGroupRequest
            {
                Schemas = new List<string>
                {
                    "urn:ietf:params:scim:api:messages:2.0:PatchOp"
                },
                Operations = new List<PatchGroupRequest.Operation>
                {
                    new PatchGroupRequest.Operation
                    {
                        Name = "addMember",
                        Op = "add",
                        Path = "members",
                        Value = new List<PatchGroupRequest.ValueObject>
                        {
                            new PatchGroupRequest.ValueObject()
                            {
                                DisplayName = "New user",
                                Value = userId
                            }
                        }
                    }
                }
            };
        }

        public static PatchGroupRequest GetScimPatchGroupRemoveUser(string userId)
        {
            return new PatchGroupRequest
            {
                Schemas = new List<string>
                {
                    "urn:ietf:params:scim:api:messages:2.0:PatchOp"
                },
                Operations = new List<PatchGroupRequest.Operation>
                {
                    new PatchGroupRequest.Operation
                    {
                        Op = "remove",
                        Path = $"members[value eq \"{userId}\"]"
                    }
                }
            };
        }
    }
}
