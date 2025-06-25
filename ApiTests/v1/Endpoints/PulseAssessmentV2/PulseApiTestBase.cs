using System.Collections.Generic;
using AtCommon.Dtos;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Utilities;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    public class PulseApiTestBase : BaseV1Test
    {

        protected static User GetAdminUser()
        {
            return User.IsSiteAdmin() ? User :
                new UserConfig("SA").GetUserByDescription("user 1");
        }

        public static readonly List<RoleRequest> Tags = new List<RoleRequest>
        {
            new RoleRequest
                { Key = "Role", Tags = new List<TagRoleRequest> { new TagRoleRequest { Name = "Sales" } } },
            new RoleRequest
                { Key = "Participant Group", Tags = new List<TagRoleRequest> { new TagRoleRequest { Name = "Technical" } } }
        };
    }
}