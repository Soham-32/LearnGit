using System;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Assessments
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class CreateAssessmentTests : BaseTest
    {
        [TestMethod]
        public void CreateAssessment_LoadTime()
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            teamAssessmentDashboard.NavigateToPage(team.TeamId);

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            teamAssessmentDashboard.AddAnAssessment("team");
            assessmentProfile.WaitForAssessmentProfilePageToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Assessment Profile", timeToLoad);

            var assessmentInfo = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = "LoadTesting" + RandomDataUtil.GetAssessmentName(),
            };

            assessmentProfile.FillDataForAssessmentProfile(assessmentInfo);

            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.WaitForTeamMembersPageToLoad();
            endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Team Member Page", timeToLoad);

            selectTeamMembers.SelectTeamMemberByName(Constants.TeamMemberName1);

            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.WaitForStakeholdersPageToLoad();
            endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Stakeholder Page", timeToLoad);

            startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.WaitForReviewAndLaunchPageToLoad();
            endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Review & launch", timeToLoad);
        }
    }
}
