using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("PortfolioTeam"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamToasterMessageTests : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PortfolioTeam_Verify_Success_Message()
        {
            Verify_Team_Success_Message_On_Every_Stepper(TeamType.PortfolioTeam);
        }
    }
}