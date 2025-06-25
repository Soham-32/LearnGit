using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamAddLeadersStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Verify_Create_Leaders_ValidationMessage()
        {
            Verify_Team_AddTeamMembersLeadersStepper_ValidationMessage(TeamType.EnterpriseTeam);
        }
    }
}