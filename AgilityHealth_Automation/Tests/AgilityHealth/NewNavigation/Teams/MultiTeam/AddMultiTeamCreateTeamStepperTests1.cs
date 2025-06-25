using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddMultiTeamCreateTeamStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void MultiTeam_Title_InfoText_ContinueButton()
        {
            Verify_Team_CreateTeamStepper_Title_InfoText_ContinueButton(TeamType.MultiTeam);
        }
    }
}
