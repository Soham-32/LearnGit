using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam"), TestCategory("NewNavigation")]
    public class AddMultiTeamReviewStepperTests3 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")] //Bug Id: 53523
        [TestCategory("KnownDefectAsBL")] //Bug Id: 53523
        [TestCategory("KnownDefectAsOL")] //Bug Id: 53523
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Multi_Team_Edit_SubTeams()
        {
            Verify_Team_ReviewStepper_Edit_SubTeams(TeamType.MultiTeam);
        }
    }
}
