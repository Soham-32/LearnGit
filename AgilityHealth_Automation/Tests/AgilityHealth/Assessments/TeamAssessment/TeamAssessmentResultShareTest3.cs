using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentResultShareTest3 : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _teamResponse;
        private static TeamAssessmentInfo _teamAssessment;
        private static User MemberUser => TestEnvironment.UserConfig.GetUserByDescription("member");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                // Create a team
                _team = TeamFactory.GetNormalTeam("Team");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = MemberUser.FirstName,
                    LastName = MemberUser.LastName,
                    Email = MemberUser.Username
                });
                _team.Stakeholders.Add(new AddStakeholderRequest
                {
                    FirstName = SharedConstants.Stakeholder1.FirstName,
                    LastName = SharedConstants.Stakeholder1.LastName,
                    Email = SharedConstants.Stakeholder1.Email
                });
                _teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                var teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                setupUi.TeamMemberAccessAtTeamLevel(teamId, (_teamResponse.Members.First().Email));

                // Create a team assessment
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"TeamComments{Guid.NewGuid()}",
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList(),
                    StakeHolders = _team.Stakeholders.Select(a => a.FirstName + " " + a.LastName).ToList()
                };

                setupUi.AddTeamAssessment(teamId, _teamAssessment);
                setupUi.StartSharingAssessment(teamId, _teamAssessment.AssessmentName);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id: 51428
        [TestCategory("CompanyAdmin")]
        public void TA_Result_Share_ReceiveEmail()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var subjectForResultsShareToMemberStakeholder = SharedConstants.ShareAssessmentEmailSubject(_teamAssessment.AssessmentName);

            Log.Info("Searching email for member and verifying");
            foreach (var emailSearchForTeamMember in _team.Members.Select(members => new EmailSearch
            {
                Subject = subjectForResultsShareToMemberStakeholder,
                To = members.Email,
                Labels = new List<string> { "UNREAD" },
                Timeout = new TimeSpan(0, 0, 30)
            }))
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(emailSearchForTeamMember),
                    $"Could not find email with subject <{subjectForResultsShareToMemberStakeholder}> sent to <{emailSearchForTeamMember}>");

                var startSharingAssessmentResultEmailBody = GmailUtil.GetAccountManagerEmailBody(subjectForResultsShareToMemberStakeholder, _teamResponse.Members.First().Email);
                Assert.IsTrue(startSharingAssessmentResultEmailBody.Contains(_teamAssessment.AssessmentName), "Member haven't received any email");

                Log.Info($"Navigate to email link and login as {_teamResponse.Members.First().Email} then verify assessment radar title");
                var getAssessmentResultLink = GmailUtil.GetSharedAssessmentLink(subjectForResultsShareToMemberStakeholder, _teamResponse.Members.First().Email);

                Driver.NavigateToPage(getAssessmentResultLink);
                login.LoginToApplication(_teamResponse.Members.First().Email, SharedConstants.CommonPassword);
                Assert.IsTrue(Driver.GetCurrentUrl().Contains("radar"), "Member is not navigated to the assessment radar page");

                var expectedRadarTitle = _teamAssessment.AssessmentName + " - " + _teamAssessment.AssessmentType + " Radar";
                Assert.AreEqual(expectedRadarTitle.ToLower(), radarPage.GetRadarTitle().ToLower(), "Assessment radar title doesn't match");
            }

            Log.Info("Searching email for stakeHolder and verifying");
            foreach (var emailSearchForStakeholder in _team.Stakeholders.Select(stakeHolder => new EmailSearch
            {
                Subject = subjectForResultsShareToMemberStakeholder,
                To = stakeHolder.Email,
                Labels = new List<string> { "UNREAD" },
                Timeout = new TimeSpan(0, 0, 30)
            }))
            {
                Assert.IsFalse(GmailUtil.DoesMemberEmailExist(emailSearchForStakeholder),
                    $"Could not find email with subject <{subjectForResultsShareToMemberStakeholder}> sent to <{emailSearchForStakeholder}>");
            }
        }
    }
}