using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamCreateTeamStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_Title_InfoText_ContinueButton()
        {
            Verify_Team_CreateTeamStepper_Title_InfoText_ContinueButton(TeamType.FullWizardTeam);
        }
    }
}
