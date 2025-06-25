using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.ObjectFactories
{
    public class MemberFactory
    {
        public static TeamMemberInfo GetTeamMemberInfo() => new TeamMemberInfo
        {
            FirstName = RandomDataUtil.GetFirstName(),
            LastName = SharedConstants.TeamMemberLastName,
            Email = Constants.UserEmailPrefix + "m" + RandomDataUtil.GetUserName() + Constants.UserEmailDomain,
        };
    }
}
