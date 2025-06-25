using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamAddLeadersStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PortfolioTeam_Verify_AddEditDelete_Leader_Successfully()
        {
            Verify_Team_AddTeamMembersLeadersStepper_AddEditDelete_Successfully(TeamType.PortfolioTeam);
        }
    }
}
