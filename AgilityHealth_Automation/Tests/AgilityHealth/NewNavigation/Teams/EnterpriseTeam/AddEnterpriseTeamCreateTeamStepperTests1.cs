using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamCreateTeamStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void EnterpriseTeam_Title_InfoText_ContinueButton()
        {
            Verify_Team_CreateTeamStepper_Title_InfoText_ContinueButton(TeamType.EnterpriseTeam);
        }
    }
}