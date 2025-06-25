using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamAddLeadersStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Verify_AddEditDelete_Leader_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddEditDelete_Successfully(TeamType.EnterpriseTeam);
        }
    }
}