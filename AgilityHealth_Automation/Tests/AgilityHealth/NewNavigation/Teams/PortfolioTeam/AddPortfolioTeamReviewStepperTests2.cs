using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamReviewStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51202
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Portfolio_Team_Edit_TeamTags()
        {
            Verify_Team_ReviewStepper_Edit_TeamTags(TeamType.PortfolioTeam);
        }
    }
}
