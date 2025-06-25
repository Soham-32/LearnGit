using AtCommon.Utilities;
using AgilityHealth_Automation.DataObjects;

namespace AgilityHealth_Automation.ObjectFactories
{
    public class UserFactory
    {
        public static AhUser GetUserInfo(string filterBy)
        {
            return new AhUser
            {
                FirstName = RandomDataUtil.GetFirstName(),
                LastName = SharedConstants.TeamMemberLastName,
                Email = "ah_automation+user_" + RandomDataUtil.GetFirstName() + "@agiletransformation.com",
                NotifyUser = true,
                FilterBy = filterBy,
            };
        }
    }
}
