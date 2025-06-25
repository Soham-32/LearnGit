using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam"), TestCategory("NewNavigation")]
    public class AddEnterpriseTeamReviewStepperTests2 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")] //Bug Id: 53523
        [TestCategory("KnownDefectAsBL")] //Bug Id: 53523
        [TestCategory("KnownDefectAsOL")] //Bug Id: 53523
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Enterprise_Team_Edit_SubTeams()
        {
            Verify_Team_ReviewStepper_Edit_SubTeams(TeamType.EnterpriseTeam);
        }
    }
}