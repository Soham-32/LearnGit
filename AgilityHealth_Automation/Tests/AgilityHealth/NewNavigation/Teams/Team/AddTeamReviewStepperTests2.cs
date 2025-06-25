using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamReviewStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_Edit_TeamTags()
        {
            Verify_Team_ReviewStepper_Edit_TeamTags(TeamType.FullWizardTeam);
        }
    }
}
