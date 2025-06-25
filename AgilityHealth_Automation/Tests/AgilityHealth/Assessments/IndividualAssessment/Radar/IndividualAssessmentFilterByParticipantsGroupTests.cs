using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Radar
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("IARadars"), TestCategory("Assessments")]
    public class IndividualAssessmentFilterByParticipantsGroupTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static CreateIndividualAssessmentRequest _assessment;
        private static List<CreateTeamMemberTagsResponse> _memberParticipantGroupResponses;
        private const int TeamMember1Answer = 7;
        private const int TeamMember2Answer = 6;

        [ClassInitialize]
        public static void CreateGoiTeam(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                // Get GOI Team with members having Participant's Group
                _team = TeamFactory.GetGoiTeam("IndividualGI");
                _team.Members.Add(SharedConstants.TeamMember1);
                _team.Members.Add(SharedConstants.TeamMember2);

                var teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();

                // Create new Individual Assessment
                _assessment = IndividualAssessmentFactory.GetPublishedIndividualAssessment(Company.Id, User.CompanyName, teamResponse.Uid);

                foreach (var member in teamResponse.Members)
                {
                    _assessment.Members.Add(new IndividualAssessmentMemberRequest
                    {
                        Uid = member.Uid,
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        Email = member.Email
                    });
                }

                var assessment = setup.CreateIndividualAssessment(_assessment, SharedConstants.IndividualAssessmentType)
                     .GetAwaiter().GetResult();

                _memberParticipantGroupResponses = setup.AddParticipantGroup(Company.Id, teamResponse.Uid, assessment);

                // Complete survey
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                setupUi.CompleteIndividualSurvey(_assessment.Members[0].Email, _assessment.PointOfContactEmail, TeamMember1Answer);
                setupUi.CompleteIndividualSurvey(_assessment.Members[1].Email, _assessment.PointOfContactEmail, TeamMember2Answer);
            }

            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 50992
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void IA_FilterBy_Participant_Groups()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var participantGroupAndId = _memberParticipantGroupResponses.Select(t => t.ParticipantGroups.First().Tags.First().Name).ToList();

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            
            Log.Info($"Navigate to radar page for the assessment {_assessment.AssessmentName} and click on rollup radar");
            dashBoardPage.GridTeamView();
            var teamId = dashBoardPage.GetTeamIdFromLink(_team.Name);
            teamAssessmentDashboard.NavigateToPage(int.Parse(teamId));
            var rollUpRadarName = $"{_assessment.AssessmentName} - Roll up";
            iAssessmentDashboardPage.ClickOnRadar(rollUpRadarName);

            Log.Info("Open filter bar and verify that participant group tab is present in filter bar");
            radarPage.Filter_OpenFilterSidebar();
            var topFilterTabList = radarPage.Filter_GetTabsList();
            Assert.That.ListContains(topFilterTabList, "Participant Groups", "'Participant Groups' tab is not present in top filter tab list");

            Log.Info("Verify that participant group tags are present in participant groups tab");
            radarPage.Filter_ClickOnParticipantGroupsTab();
            var participantGroupTagList = radarPage.Filter_GetParticipantGroupsTagsList();

            Assert.That.ListContains(participantGroupTagList, participantGroupAndId.First(), $"Participant Group does not contains '{participantGroupAndId.First()}' tag");
            Assert.That.ListContains(participantGroupTagList, participantGroupAndId.Last(), $"Participant Group does not contains '{participantGroupAndId.Last()}' tag");

            Log.Info("Select 'Support' as participant groups and verify that there is only one average line and also verify average point value");
            radarPage.Filter_SelectTagCheckboxByTagName(participantGroupAndId.First());
            var colorHexForSupport = radarPage.Filter_GetTagColor(participantGroupAndId.First());

            var expectedParticipantGroup = _memberParticipantGroupResponses.Count(a =>a.ParticipantGroups.Any(x => x.Tags.Any(z => z.Name.Equals(participantGroupAndId.First()))));

            foreach (var comp in Constants.CompetencyLabelForAgileCoachHealth)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(expectedParticipantGroup, assessmentDetailPage.Radar_GetDotValue("dots", colorHexForSupport, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(TeamMember1Answer,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHexForSupport, comp).First()),
                    $"Competency: <{comp}> dot value doesn't match");
            }
            radarPage.Filter_SelectTagCheckboxByTagName(participantGroupAndId.First(), false);

            Log.Info("Select 'Technical' as participant groups and verify that there is only one average line and also verify average point value");
            radarPage.Filter_SelectTagCheckboxByTagName(participantGroupAndId.Last());
            var colorHexForTechnical = radarPage.Filter_GetTagColor(participantGroupAndId.Last());

            const int expectedAvg = (TeamMember1Answer + TeamMember2Answer) / 2;

            expectedParticipantGroup = _memberParticipantGroupResponses.Count(a => a.ParticipantGroups.Any(x => x.Tags.Any(z => z.Name.Equals(participantGroupAndId.Last()))));

            foreach (var comp in Constants.CompetencyLabelForAgileCoachHealth)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(expectedParticipantGroup, assessmentDetailPage.Radar_GetDotValue("dots", colorHexForTechnical, comp).Count,
                   $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedAvg,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHexForTechnical, comp).First()),
                    $"Competency: <{comp}> dot value doesn't match");
            }
        }
    }
}
