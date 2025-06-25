using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamReviewStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 50991
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Portfolio_Team_Edit_TeamProfile()
        {
            Verify_Team_ReviewStepper_Edit_TeamProfile(TeamType.PortfolioTeam);
        }
    }
}
