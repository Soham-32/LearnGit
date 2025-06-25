using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentMembersCanTakeSurvey : BaseTest
    {
        private static bool _classInitFailed;
        public static TeamAssessmentInfo TeamAssessmentForTeamMember, TeamAssessmentForStakeholder, TeamAssessmentForMultipleTeamMember;
        private static TeamHierarchyResponse _team;
        public static readonly List<DimensionNote> NotesForTeamMember1 = new List<DimensionNote>
        {
            new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = $"Vision member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = $"Planning member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = $"Roles member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = $"Confidence member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = $"Measurements member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = $"Team Facilitator member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = $"Technical Lead(s) member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = $"Product Owner member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = $"Management member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = $"Team Dynamics member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = $"Agility member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = $"Team Structure member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments member {RandomDataUtil.GetUserName()}" }
        };
        public static readonly List<DimensionNote> NotesForTeamMember2 = new List<DimensionNote>
        {
            new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = $"Vision member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = $"Planning member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = $"Roles member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = $"Confidence member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = $"Measurements member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = $"Team Facilitator member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = $"Technical Lead(s) member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = $"Product Owner member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = $"Management member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = $"Team Dynamics member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = $"Agility member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = $"Team Structure member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements member {RandomDataUtil.GetUserName()}" },
            new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments member {RandomDataUtil.GetUserName()}" }
        };

        public static readonly List<DimensionNote> NotesForStakeHolder = new List<DimensionNote>
            {
                new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = $"Confidence stakeholder {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = $"Strengths member {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = $"Improvements member {RandomDataUtil.GetUserName()}" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = $"Impediments member {RandomDataUtil.GetUserName()}" }
            };

        public static readonly List<string> Dimensions = new List<string> { "Clarity", "Performance", "Leadership", "Culture", "Foundation" };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                    .GetTeamByName(SharedConstants.Team);
                //Create the Team assessment with One Team member
                TeamAssessmentForTeamMember = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Test_TeamMember{RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber()}",
                    TeamMembers = new List<string> { Constants.TeamMemberName1 }
                };

                //Create the Team assessment with One Stakeholder
                TeamAssessmentForStakeholder = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Test_Stakeholder{RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber()}",
                    TeamMembers = new List<string> { Constants.TeamMemberName1 },
                    StakeHolders = new List<string> { Constants.StakeholderName2 }
                };

                //Create the Team assessment with Two Team member
                TeamAssessmentForMultipleTeamMember = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Test_TeamMembers{RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber()}",
                    TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 },
                    StakeHolders = new List<string> { Constants.StakeholderName2 }
                };

                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessmentForTeamMember);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessmentForStakeholder);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessmentForMultipleTeamMember);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }


        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Single_TeamMember_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Team Member' {Constants.TeamMemberName1}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForTeamMember.AssessmentName), Constants.TeamMemberEmail1));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimensions, NotesForTeamMember1);

            surveyPage.EnterOpenEndedNotes(NotesForTeamMember1);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            Log.Info("Verify the number of Team Members should display correctly ");
            var participant = teamAssessmentDashboard.GetAssessmentParticipantTeamMembers(TeamAssessmentForTeamMember.AssessmentName);
            Assert.AreEqual("Completed by 1 out of 1  Team Members", participant, "Participant info doesn't match");

            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentForTeamMember.AssessmentName);

            Log.Info("Verify dimension notes for Team Member display correctly");
            NotesForTeamMember1.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            Log.Info("Verify open ended notes for Team Member display correctly");
            NotesForTeamMember1.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));

            Log.Info("Navigate back to Assessment dashboard and verifying Pulse radio button is displayed or not after Completing the survey but End date is not reached");
            Driver.Back();
            teamAssessmentDashboard.ClickOnAddAnAssessmentButton();
            Assert.IsTrue(teamAssessmentDashboard.IsAddAnAssessmentPulseRadioButtonDisplayed(), "Pulse radio button is not displayed");
            teamAssessmentDashboard.ClickOnCloseIconPopup();
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_StakeHolderAndTeamMember_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Team Member' {Constants.TeamMemberName1}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForStakeholder.AssessmentName), Constants.TeamMemberEmail1));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimensions, NotesForTeamMember1);

            surveyPage.EnterOpenEndedNotes(NotesForTeamMember1);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Stake Holder' {Constants.StakeholderName2}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForStakeholder.AssessmentName), Constants.StakeholderEmail2));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();

            surveyPage.EnterNotesForDimension("Performance", NotesForStakeHolder);
            surveyPage.ClickNextButton();

            surveyPage.EnterOpenEndedNotes(NotesForStakeHolder);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            Log.Info("Verify the number of Stakeholders should display correctly ");
            var participant = teamAssessmentDashboard.GetAssessmentParticipantStakeholders(TeamAssessmentForStakeholder.AssessmentName);
            Assert.AreEqual("Completed by 1 out of 1  Stakeholders", participant, "Participant info doesn't match");

            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentForStakeholder.AssessmentName);

            Log.Info("Verify dimension notes for Stakeholder display correctly");
            NotesForStakeHolder.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            Log.Info("Verify open ended notes for Stakeholder display correctly");
            NotesForStakeHolder.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));

            Log.Info("Verify dimension notes for Team Member display correctly");
            NotesForTeamMember1.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            Log.Info("Verify open ended notes for Team Member display correctly");
            NotesForTeamMember1.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void TeamAssessment_Multiple_TeamMembersAndStakeholder_TakeAssessment()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var surveyPage = new SurveyPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Stake holder' {Constants.StakeholderName2}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForMultipleTeamMember.AssessmentName), Constants.StakeholderEmail2));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();

            surveyPage.EnterNotesForDimension("Performance", NotesForStakeHolder);
            surveyPage.ClickNextButton();

            surveyPage.EnterOpenEndedNotes(NotesForStakeHolder);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Team Member1' {Constants.TeamMemberName1}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForMultipleTeamMember.AssessmentName), Constants.TeamMemberEmail1));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimensions, NotesForTeamMember1);

            surveyPage.EnterOpenEndedNotes(NotesForTeamMember1);
            surveyPage.ClickFinishButton();

            Log.Info($"Go to Team assessment and fill the survey with notes for 'Team Member2' {Constants.TeamMemberName2}");
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentForMultipleTeamMember.AssessmentName), Constants.TeamMemberEmail2));

            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();

            surveyPage.SubmitRandomSurvey();
            surveyPage.SubmitSurveyNotes(Dimensions, NotesForTeamMember2);

            surveyPage.EnterOpenEndedNotes(NotesForTeamMember2);
            surveyPage.ClickFinishButton();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentForMultipleTeamMember.AssessmentName);

            Log.Info("Verify dimension notes and open ended notes for Stake holder display correctly");
            NotesForStakeHolder.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            NotesForStakeHolder.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));


            Log.Info("Verify dimension notes and open ended notes for Team Member1 display correctly");
            NotesForTeamMember1.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            NotesForTeamMember1.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));


            Log.Info("Verify dimension notes and open ended notes for Team Member2 display correctly");
            NotesForTeamMember2.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), n.Note));

            NotesForTeamMember2.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), n.Note));
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void TeamAssessment_Edit_AssessmentChecklist()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var taEditPage = new TeamAssessmentEditPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.SelectRadarLink(TeamAssessmentForTeamMember.AssessmentName, "Edit");

            taEditPage.ClickOnAssessmentChecklistTab();

            taEditPage.SelectChecklistItem1(Constants.AssessmentChecklistSingleItem);
            taEditPage.SelectChecklistItem2(Constants.AssessmentChecklistMultiItem);
            taEditPage.ClickSaveMaturityChecklistButton();

            Driver.Back();
            teamAssessmentDashboard.SelectRadarLink(TeamAssessmentForTeamMember.AssessmentName, "Edit");

            taEditPage.ClickOnAssessmentChecklistTab();

            Log.Info("Verify updates are saved");
            Assert.AreEqual(Constants.AssessmentChecklistSingleItem, taEditPage.GetCheckList1Value(),
                "Automation Item 1 value does not match.");
            Assert.IsTrue(taEditPage.GetCheckList2Value().Contains(Constants.AssessmentChecklistMultiItem),
                "Automation Item 2 value does not match.");
        }
    }
}