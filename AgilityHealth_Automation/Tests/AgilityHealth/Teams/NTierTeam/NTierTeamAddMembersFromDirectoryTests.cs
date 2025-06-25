using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierTeamAddMembersFromDirectoryTests : NTierBaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public void NTierTeam_TeamMembers_AddFromDirectory_AddMember()
        {
            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);
            var expectedTeamMember = SharedConstants.TeamMember1.Email;
            var nTierTeamName = "N-TierTeam_" + RandomDataUtil.GetTeamName();

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();

            //Add N-Tier Team
            Log.Info("Create an N-Tier team & Add team member from directory");
            createNTierPage.CreateNTierTeamWithSubTeam(nTierTeamName);
            addTeamMemberPage.CompanyLookupAddMembersFromDirectory(new List<string> { expectedTeamMember });

            Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(expectedTeamMember), $"Team member : {expectedTeamMember} is not added");

        }
    }
}
