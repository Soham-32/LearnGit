using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams")]
    public class EnterpriseTeamAddMembersFromDirectory : BaseTest
    {
        public static bool ClassInitFailed;
        private static TeamResponse _expectedTeamResponse;
        private static int _enterpriseId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("MultiTeam");
                var enterpriseTeamResponse = setup.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                _enterpriseId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name).TeamId;

                // Getting team details from existing team
                _expectedTeamResponse = setup.GetTeamResponse(SharedConstants.NotificationTeam);
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void EnterpriseTeam_TeamMembers_AddFromDirectory_AddMember()
        {
            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            var expectedTeamMember = SharedConstants.TeamMember1.Email;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit team members page and add team member from directory");
            addTeamMemberPage.NavigateToPage(_enterpriseId);
            addTeamMemberPage.CompanyLookupAddMembersFromDirectory(new List<string> { expectedTeamMember });

            Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(expectedTeamMember), $"Team member : {expectedTeamMember} is not added");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void EnterpriseTeam_TeamMembers_AddFromDirectory_AddTeam()
        {
            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit team members page and add team's members from directory");
            addTeamMemberPage.NavigateToPage(_enterpriseId);

            addTeamMemberPage.CompanyLookupAddTeamsFromDirectory(new List<string> { _expectedTeamResponse.Name });
            var expectedMemberList = _expectedTeamResponse.Members.Select(a => a.Email).ToList();

            foreach (var expectedMember in expectedMemberList)
            {
                Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(expectedMember), $"Team's member : {expectedMember} is not added");
            }
        }
    }
}