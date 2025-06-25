using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.HeartBeatsChecks.TopScreensLoadTimeTests.Assessments
{
    [TestClass]
    [TestCategory("CompanyAdmin")]
    [TestCategory("ScreenLoadTime")]
    public class TakeSurveyTests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static TeamAssessmentInfo _teamAssessment;
        private static List<EmailSearch> _teamMemberEmailSearchList;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
            _team = teams.GetTeamByName(SharedConstants.Team);
            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
                StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName() }
            };

            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, _teamAssessment);

            _teamMemberEmailSearchList = new List<EmailSearch>
            {
                new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                }
            };
        }

        [TestMethod]
        public void TakeSurvey_LoadTime()
        {
            var surveyPage = new SurveyPage(Driver, Log);
            var surveyLink = GmailUtil.GetSurveyLink(_teamMemberEmailSearchList[0].Subject, _teamMemberEmailSearchList[0].To, _teamMemberEmailSearchList[0].Labels.First());

            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            surveyPage.NavigateToUrl(surveyLink + "/start");
            surveyPage.WaitForConfirmIdentityPopupToLoad();
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var timeToLoad = (endTime - startTime) / 1000f;
            PageLoadTime.Add("Survey Page", timeToLoad);
        }
    }
}
