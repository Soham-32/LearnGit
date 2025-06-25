using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamTeamMembersStepperTests4 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_AddEditDelete_TeamMember_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddEditDelete_Successfully(TeamType.FullWizardTeam);
        }
    }
}
