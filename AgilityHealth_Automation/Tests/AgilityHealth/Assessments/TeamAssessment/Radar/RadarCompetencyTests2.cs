using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]

    public class RadarCompetencyTests2 : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static TeamAssessmentInfo _teamAssessment;
        private static List<int> _teamMemberSurveyAnswers;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() },
                    StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName(), SharedConstants.Stakeholder2.FullName() }
                };
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, _teamAssessment);

                var teamMemberEmailSearchList = new List<EmailSearch>
                {
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember1.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember2.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                };
                _teamMemberSurveyAnswers = new List<int> { 4, 6 };
                setup.CompleteTeamMemberSurvey(teamMemberEmailSearchList, _teamMemberSurveyAnswers);

            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
            
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void RadarReview_SummaryView_CompetencyValue_Edit()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            radarPage.ClickCompetency("4422");

            var newCompetency = radarPage.GetCompetencyValue() == "5" ? "6" : "5";
            radarPage.UpdateCompetency(newCompetency);

            radarPage.ClickCompetency("4422");

            Assert.AreEqual(radarPage.GetCompetencyValue(), newCompetency,
                "Competency should be updated properly in Detail view");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void RadarReview_DetailView_CompetencyValue_Edit()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            radarPage.ClickCompetency("4422");

            var newCompetency = radarPage.GetCompetencyValue() == "5" ? "6" : "5";
            radarPage.UpdateCompetency(newCompetency);

            radarPage.ClickCompetency("4422");

            Assert.AreEqual(radarPage.GetCompetencyValue(), newCompetency,
                "Competency should be updated properly in Detail view");
        }
    }
}
