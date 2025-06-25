using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("PortfolioTeam"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamCreateTeamStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PortfolioTeam_Title_InfoText_ContinueButton()
        {
            Verify_Team_CreateTeamStepper_Title_InfoText_ContinueButton(TeamType.PortfolioTeam);
        }
    }
}
