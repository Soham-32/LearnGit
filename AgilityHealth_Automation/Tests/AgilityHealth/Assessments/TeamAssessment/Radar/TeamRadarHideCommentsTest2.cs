using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamRadarHideCommentsTest2 : BaseTest
    {

        private static User AdminWithAhf => TestEnvironment.UserConfig.GetUserByDescription("user 1");
        private static User AdminWithoutAhf => TestEnvironment.UserConfig.GetUserByDescription("user 4");

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_Ahf_NonAhf_Admin()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            var team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.RadarTeam);

            login.NavigateToPage();
            login.LoginToApplication(AdminWithAhf.Username, AdminWithAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.AssessmentHideUnHideCommentsRadar);

            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is not displayed");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is not displayed");

            Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note),
                "'Hide' button is not displayed ");

            topNav.LogOut();

            login.LoginToApplication(AdminWithoutAhf.Username, AdminWithoutAhf.Password);

            teamAssessmentDashboard.NavigateToPage(team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(SharedConstants.AssessmentHideUnHideCommentsRadar);
            if (User.IsCompanyAdmin())
            {
                Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is not displayed");
                Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is not displayed");
                Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is not displayed");
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note),
                    "'Hide' button is not displayed ");
            }
            else
            {
                Assert.IsFalse(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is displayed");
                Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is displayed");
                Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note),
                    "'Hide' button is displayed ");
            }

        }
    }
}