using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2PulseRadioOptionTests : PulseV2BaseTest
    {
        private static AddTeamWithMemberRequest _team;
        private static int _teamId;
        private static SavePulseAssessmentV2Request _pulseAssessmentV2Request;
        private static bool _classInitFailed;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                _team = TeamFactory.GetNormalTeam("Pulse_Team");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = SharedConstants.TeamMember1.FirstName,
                    LastName = SharedConstants.TeamMember1.LastName,
                    Email = SharedConstants.TeamMember1.Email
                });

                setupApi.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_team.Name).TeamId;

                //Get team profile 
                var teamWithTeamMemberResponse = setupApi.GetTeamWithTeamMemberResponse(_teamId);

                //Get radar details
                var radarResponse = GetQuestions(_teamId);
                _pulseAssessmentV2Request = PulseV2Factory.PulseAssessmentV2AddRequest(radarResponse, teamWithTeamMemberResponse, _teamId);
                setupApi.CreatePulseAssessmentV2(_pulseAssessmentV2Request, Company.Id, User);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseAssessmentV2_CheckRadioButtonAndSwimLane()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            Log.Info($"Navigate to the login page and login as {User.FullName}");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to the 'Team Assessment Dashboard' Tab and verify that 'Pulse' radio button and Swim Lane should be displayed");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.ClickOnAddAnAssessmentButton();
            Assert.IsTrue(teamAssessmentDashboardPage.IsAddAnAssessmentPulseRadioButtonDisplayed(), "'Pulse' radio button is not displayed");

            teamAssessmentDashboardPage.ClickOnCloseIconPopup();
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseCheckSwimLaneDisplayed(), "'Pulse Check' swim lane is not displayed");

            Log.Info("Click on the 'Add Assessment' button and verify the URL");
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");
            Assert.IsTrue(Driver.GetCurrentUrl().Contains("pulse-assessments"));
        }
    }
}