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

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{
    [TestClass]
    [TestCategory("Teams")]
    public class TeamAddStakeholderFromDirectory : BaseTest
    {
        public static bool ClassInitFailed;
        private static TeamResponse _expectedTeamResponse;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team");
                var teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;

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
        public void Team_Stakeholder_AddFromDirectory_AddMembers()
        {
            var login = new LoginPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            var expectedStakeholderEmail = SharedConstants.Stakeholder1.Email;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit stakeholder page and add stakeholder from directory");

            addStakeHolderPage.NavigateToPage(_teamId);

            addStakeHolderPage.CompanyLookupAddMembersFromDirectory(new List<string> { expectedStakeholderEmail });

            Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholderEmail), $"Stakeholder: {expectedStakeholderEmail} is not added");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Team_Stakeholder_AddFromDirectory_AddTeam()
        {
            var login = new LoginPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit stakeholder page and add team's stakeholders from directory");

            addStakeHolderPage.NavigateToPage(_teamId);

            addStakeHolderPage.CompanyLookupAddTeamsFromDirectory(new List<string> { _expectedTeamResponse.Name });

            var expectedStakeHolderList = _expectedTeamResponse.Stakeholders.Select(a => a.Email).ToList();

            foreach (var expectedStakeHolder in expectedStakeHolderList)
            {
                Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeHolder), $"Team's stakeholder : {expectedStakeHolder} is not added");
            }
        }
    }
}