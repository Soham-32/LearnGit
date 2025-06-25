using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamReviewStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 50991
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Enterprise_Team_Edit_TeamProfile()
        {
            Verify_Team_ReviewStepper_Edit_TeamProfile(TeamType.EnterpriseTeam);
        }
    }
}