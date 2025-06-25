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
    public class EnterpriseTeamAddStakeholderFromDirectory : BaseTest
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
                var enterpriseTeam1 = TeamFactory.GetEnterpriseTeam("EnterpriseTeam");
                var enterpriseTeamResponse = setup.CreateTeam(enterpriseTeam1).GetAwaiter().GetResult();
                _enterpriseId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name).TeamId;

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
        public void EnterpriseTeam_stakeholders_FromDirectory_AddMember()
        {
            var login = new LoginPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            var expectedStakeholder = SharedConstants.Stakeholder1.Email;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit stakeholder page and add stakeholder from directory");
            addStakeHolderPage.NavigateToPage(_enterpriseId);
            addStakeHolderPage.CompanyLookupAddMembersFromDirectory(new List<string> { expectedStakeholder });

            Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholder), $"Stakeholders : {expectedStakeholder} is not added");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void EnterpriseTeam_Stakeholder_AddFromDirectory_AddTeam()
        {
            var login = new LoginPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to edit stakeholder page and add team's stakeholders from directory");
            addStakeHolderPage.NavigateToPage(_enterpriseId);

            addStakeHolderPage.CompanyLookupAddTeamsFromDirectory(new List<string> { _expectedTeamResponse.Name });
            var expectedStakeholderList = _expectedTeamResponse.Stakeholders.Select(a => a.Email).ToList();

            foreach (var expectedStakeholder in expectedStakeholderList)
            {
                Assert.IsTrue(addStakeHolderPage.IsTeamMemberDisplayed(expectedStakeholder), $"Team's stakeholder : {expectedStakeholder} is not added");
            }

        }
    }
}