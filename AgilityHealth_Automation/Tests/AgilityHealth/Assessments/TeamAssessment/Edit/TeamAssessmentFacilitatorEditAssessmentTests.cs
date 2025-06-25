using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Edit
{

    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentFacilitatorEditAssessmentTests : BaseTest
    {
        private const int EmailTimeOutInSeconds = 20;
        private static bool _classInitFailed;
        private static User FacilitatorUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");
        private static TeamHierarchyResponse _team;
        private static SetUpMethods _setup;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
            Facilitator = FacilitatorUser.FirstName + " " + FacilitatorUser.LastName,
            FacilitationDate = DateTime.Today.AddDays(1),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder2.FullName() },
        };


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment)
                        .GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _setup = new SetUpMethods(testContext, TestEnvironment);
                _setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id : 53365
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void TeamAssessment_Facilitator_EditAssessment_Add_Member_Stakeholder_SendEmail()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(FacilitatorUser.Username, FacilitatorUser.Password);

            Log.Info("Verify that selected team member and stakeholder is present on the assessment edit page");
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.SelectRadarLink(TeamAssessment.AssessmentName, "Edit");
            Assert.That.ListContains(taEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember1.Email, $"{SharedConstants.TeamMember1.Email} is not present in the team member list");
            Assert.That.ListContains(taEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder2.Email, $"{SharedConstants.Stakeholder2.Email} is not present in the stakeholder list");

            Log.Info("Verify that facilitator can add team member and stakeholder for the respective assessment");
            taEditPage.AddTeamMemberByEmail(SharedConstants.TeamMember2.Email);
            taEditPage.AddStakeholderByEmail(SharedConstants.Stakeholder3.Email);
            Assert.That.ListContains(taEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember2.Email, $"{SharedConstants.TeamMember2.Email} is not present in the team member list");
            Assert.That.ListContains(taEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder3.Email, $"{SharedConstants.Stakeholder3.Email} is not present in the stakeholder list");

            Log.Info("Verify that all the members and stakeholder receives survey email");
            taEditPage.ClickSendToAllButtonForTeamMembers();
            taEditPage.ClickSendToAllButtonForStakeholders();

            var expectedSubject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName);
            var expectedMembers = new Dictionary<string, string>
            {
                { SharedConstants.TeamMember1.FirstName+" "+SharedConstants.TeamMember1.LastName, SharedConstants.TeamMember1.Email },
                { SharedConstants.TeamMember2.FirstName+" "+SharedConstants.TeamMember2.LastName, SharedConstants.TeamMember2.Email }
            };
            var expectedStakeholders = new Dictionary<string, string>
            {
                { SharedConstants.Stakeholder2.FirstName+" "+SharedConstants.Stakeholder2.LastName, SharedConstants.Stakeholder2.Email },
                { SharedConstants.Stakeholder3.FirstName+" "+SharedConstants.Stakeholder3.LastName, SharedConstants.Stakeholder3.Email  }
            };
            foreach (var teamMember in expectedMembers)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember.Value, "", "", EmailTimeOutInSeconds),
                    $"Email NOT received <{expectedSubject}> sent to {teamMember.Value}");
            }
            foreach (var stakeholder in expectedStakeholders)
            {
                Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, stakeholder.Value, "", "", EmailTimeOutInSeconds),
                    $"Email NOT received <{expectedSubject}> sent to {stakeholder.Value}");
            }

            Log.Info("Verify that the team member and stakeholder receives survey email when facilitator resend invites individually");
            taEditPage.ClickMemberResendInviteLinkButton(SharedConstants.TeamMember1.Email);
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.TeamMember1.Email, "", "", EmailTimeOutInSeconds),
                   $"Email NOT received <{expectedSubject}> sent to {SharedConstants.TeamMember1.Email}");

            taEditPage.ClickStakeholderResendInviteLinkButton(SharedConstants.Stakeholder2.Email);
            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.Stakeholder2.Email, "", "", EmailTimeOutInSeconds),
                   $"Email NOT received <{expectedSubject}> sent to {SharedConstants.Stakeholder2.Email}");

            Log.Info("Verify that facilitator can remove team member or stakeholder from the assessment");
            taEditPage.DeleteTeamMemberByEmail(SharedConstants.TeamMember2.Email);
            taEditPage.DeleteStakeholderByEmail(SharedConstants.Stakeholder3.Email);

            Assert.That.ListContains(taEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember1.Email, $"{SharedConstants.TeamMember1.Email} is not present in the team member list");
            Assert.That.ListNotContains(taEditPage.GetTeamMemberEmails(), SharedConstants.TeamMember2.Email, $"{SharedConstants.TeamMember2.Email} is present in the team member list");
            Assert.That.ListContains(taEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder2.Email, $"{SharedConstants.Stakeholder2.Email} is not present in the stakeholder list");
            Assert.That.ListNotContains(taEditPage.GetStakeholderEmails(), SharedConstants.Stakeholder3.Email, $"{SharedConstants.Stakeholder3.Email} is present in the stakeholder list");

            Log.Info("Verify that existing team members and stakeholders only receives the email");
            taEditPage.ClickSendToAllButtonForTeamMembers();
            taEditPage.ClickSendToAllButtonForStakeholders();

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.TeamMember1.Email, "", "", EmailTimeOutInSeconds),
                    $"Email NOT received <{expectedSubject}> sent to {SharedConstants.TeamMember1.Email}");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.TeamMember2.Email, "", "UNREAD", EmailTimeOutInSeconds),
                   $"Email received <{expectedSubject}> sent to {SharedConstants.TeamMember2.Email}");

            Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.Stakeholder2.Email, "", "", EmailTimeOutInSeconds),
                    $"Email NOT received <{expectedSubject}> sent to {SharedConstants.Stakeholder2.Email}");

            Assert.IsFalse(GmailUtil.DoesMemberEmailExist(expectedSubject, SharedConstants.Stakeholder3.Email, "", "UNREAD", EmailTimeOutInSeconds),
                   $"Email received <{expectedSubject}> sent to {SharedConstants.Stakeholder3.Email}");

            Log.Info("Fill Assessment Survey as team members");

            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            _setup.CompleteTeamMemberSurvey(emailSearch);

            Log.Info("Launch ahf survey and verify that ahf email is received");
            _setup.LaunchAhfSurvey(_team.TeamId, TeamAssessment.AssessmentName);
            Assert.IsTrue(GmailUtil.DoesAhfSurveyEmailExist(SharedConstants.TeamMember1.Email, TeamAssessment.AssessmentName, 10),
                $"Email NOT received <{expectedSubject}> sent to {SharedConstants.TeamMember1.Email}");
        }
    }
}
