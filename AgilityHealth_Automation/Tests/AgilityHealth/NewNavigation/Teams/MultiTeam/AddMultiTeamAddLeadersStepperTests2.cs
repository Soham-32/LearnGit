using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddMultiTeamAddLeadersStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_Verify_Create_Leaders_ValidationMessage()
        {
            Verify_Team_AddTeamMembersLeadersStepper_ValidationMessage(TeamType.MultiTeam);
        }
    }
}
