using System;
using System.Collections.Generic;
using AtCommon.Dtos.Bulk;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public class UserFactory
    {
        public static AddUser GetBlAdminUserForBulkImport()
        {
            return new AddUser
            {
                FirstName = $"BulkBL{RandomDataUtil.GetFirstName()}",
                LastName = SharedConstants.TeamMemberLastName,
                Email = $"ah_automation+B{RandomDataUtil.GetFirstName():D}@agiletransformation.com",
                Role = "TagAdmin",
                Teams = new List<string>(),
                Tags = new List<TeamTagRequest>
                {
                    new TeamTagRequest { Category = "Business Lines", Tags = new List<string> { "Automation" } }
                }
            };
        }
    }
}