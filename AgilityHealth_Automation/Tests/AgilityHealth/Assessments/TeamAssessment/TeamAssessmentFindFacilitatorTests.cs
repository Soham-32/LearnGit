using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Facilitator;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{

    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentFindFacilitatorTests : BaseTest
    {
        private static User ActiveCaUser => TestEnvironment.UserConfig.GetUserByDescription("facilitator");
        private static User InActiveCaUser => TestEnvironment.UserConfig.GetUserByDescription("user 5");
        private static User ActiveOlUser => TestEnvironment.UserConfig.GetUserByDescription("org leader");
        private static User InActiveOlUser => TestEnvironment.UserConfig.GetUserByDescription("org leader password reset");
        private static User ActiveBlUser => TestEnvironment.UserConfig.GetUserByDescription("business line admin");
        private static User InActiveBlUser => TestEnvironment.UserConfig.GetUserByDescription("business line admin password reset");
        private static User ActiveTaUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");
        private static User InActiveTaUser => TestEnvironment.UserConfig.GetUserByDescription("team admin password reset");

        private static readonly UserConfig MembersUserConfig = new UserConfig("M");
        private static User ActiveMemberUser => MembersUserConfig.GetUserByDescription("member 4");
        private static User InActiveMemberUser => MembersUserConfig.GetUserByDescription("member 5");

        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                    .GetTeamByName(SharedConstants.Team);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_FindFacilitator_Disabled()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            var teamAssessment = new TeamAssessmentInfo
            {
                AssessmentName = "FaFAssessment" + RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamMembers = new List<string> { Constants.TeamMemberName1 },
                Facilitator = "Automated Testing"
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            assessmentProfile.FillDataForAssessmentProfile(teamAssessment);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            foreach (var member in teamAssessment.TeamMembers)
            {
                selectTeamMembers.SelectTeamMemberByName(member);
            }

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();

            selectStakeHolder.ClickOnReviewAndFinishButton();

            Assert.IsFalse(reviewAndLaunch.IsFindFacilitatorCheckboxEnabled(), "The 'Find A Facilitator' checkbox is not disabled");

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id :32832
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Create_FindFacilitator_Publish()
        {
            VerifySetup(_classInitFailed);

            Log.Info("Test : Verify that email is sent to Facilitators when 'Find A Facilitator' option is checked and assessment is published");

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);
            var facilitatorOptIn = new FacilitatorOptInPage(Driver, Log);

            var teamAssessment = new TeamAssessmentInfo
            {
                AssessmentName = "FaFAssessment" + RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamMembers = new List<string> { Constants.TeamMemberName1 },
                FacilitationDate = DateTime.Today,
                Location = "On Mars"
            };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.AddAnAssessment("Team");

            assessmentProfile.FillDataForAssessmentProfile(teamAssessment);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();

            foreach (var member in teamAssessment.TeamMembers)
            {
                selectTeamMembers.SelectTeamMemberByName(member);
            }

            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();

            Log.Info("Verify that the 'Find a Facilitator' checkbox is enabled.");
            Assert.IsTrue(reviewAndLaunch.IsFindFacilitatorCheckboxEnabled(), "The 'Find A Facilitator' checkbox is disabled");

            reviewAndLaunch.CheckFindFacilitatorCheckbox();
            reviewAndLaunch.ClickOnPublish();

            Log.Info("Verify a confirmation email should be sent to the active facilitator");
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", ActiveCaUser.Username, "inbox", ActiveCaUser.FirstName),
                $"Could not find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{ActiveCaUser.Username}>.");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", ActiveOlUser.Username, "inbox", ActiveOlUser.FirstName),
                $"Could not find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{ActiveOlUser.Username}>.");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", ActiveBlUser.Username, "inbox", ActiveBlUser.FirstName),
                $"Could not find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{ActiveBlUser.Username}>.");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", ActiveTaUser.Username, "inbox", ActiveTaUser.FirstName),
                $"Could not find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{ActiveTaUser.Username}>.");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", ActiveMemberUser.Username, "Auto_Member", ActiveMemberUser.FirstName),
                $"Could not find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{ActiveMemberUser.Username}>.");

            Log.Info("Verify a confirmation email should not sent to the inActive facilitator");
            Assert.IsFalse(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", InActiveCaUser.Username, "Inbox", InActiveCaUser.FirstName, 10),
                $"Could find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{InActiveCaUser.Username}>.");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", InActiveOlUser.Username, "inbox", InActiveOlUser.FirstName, 10),
                $"Could find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{InActiveOlUser.Username}>.");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", InActiveBlUser.Username, "inbox", InActiveBlUser.FirstName, 10),
                $"Could find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{InActiveBlUser.Username}>.");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", InActiveTaUser.Username, "inbox", InActiveTaUser.FirstName, 10),
                $"Could find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{InActiveTaUser.Username}>.");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!", InActiveMemberUser.Username, "Auto_Member", InActiveMemberUser.FirstName, 10),
                $"Could find email with subject <Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}> sent to <{InActiveMemberUser.Username}>.");


            Log.Step(reviewAndLaunch.GetType().Name, "Find the 'Opt In' link from the email");

            var optInLink = GmailUtil.GetFacilitatorOptInLink($"Facilitator needed for {SharedConstants.Team} on {teamAssessment.FacilitationDate:M/d/yyyy}!",
                ActiveCaUser.Username, ActiveCaUser.FirstName + ": Company Admin scheduled a(n) " + teamAssessment.AssessmentName + " assessment");

            facilitatorOptIn.NavigateToUrl(optInLink);

            Log.Info("Verify the info in the Opt In page is accurate");
            var companyTime = teamAssessment.FacilitationDate.ToUniversalTime()
                .ToString("M/d/yyyy h:mm tt");

            Assert.AreEqual($"Date & Time: {companyTime} {Constants.CompanyTimeZone}", facilitatorOptIn.GetDateAndTime(),
                "Date & Time doesn't match");
            Assert.AreEqual($"Team: {SharedConstants.Team}", facilitatorOptIn.GetTeam(), "Team doesn't match");
            Assert.AreEqual($"Assessment: {teamAssessment.AssessmentName}", facilitatorOptIn.GetAssessment(),
                "Assessment doesn't match");
            Assert.AreEqual($"Location: {teamAssessment.Location}", facilitatorOptIn.GetLocation(), "Location doesn't match");

        }

    }
}
