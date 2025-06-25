using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
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

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamRadarHideCommentsTest1 : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _createTeamResponse;
        private static TeamHierarchyResponse _teamId;
        private static List<DimensionNote> _teamNotes;
        private static List<DimensionNote> _stackHolderNotes;
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
                    FirstName = SharedConstants.Stakeholder2.FirstName,
                    LastName = SharedConstants.Stakeholder2.LastName,
                    Email = SharedConstants.Stakeholder2.Email
                });
                _createTeamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponse.Name);

                setupUi.TeamMemberAccessAtTeamLevel(_teamId.TeamId, (_createTeamResponse.Members.First().Email));

                // Create a team assessment 
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"TeamComments{Guid.NewGuid()}",
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList(),
                    StakeHolders = _team.Stakeholders.Select(a => a.FirstName+" "+a.LastName).ToList()
                };

                setupUi.AddTeamAssessment(_teamId.TeamId, _teamAssessment);

                // Fill the survey
                _teamNotes = new List<DimensionNote>
                {
                    new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = "Vision member" },
                    new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = "Planning member" },
                    new DimensionNote { Dimension = "Clarity", SubDimension = "Roles", Note = "Roles member" },
                    new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence member" },
                    new DimensionNote { Dimension = "Performance", SubDimension = "Measurements", Note = "Measurements member" },
                    new DimensionNote { Dimension = "Leadership", SubDimension = "Team Facilitator", Note = "Team Facilitator member" },
                    new DimensionNote { Dimension = "Leadership", SubDimension = "Technical Lead(s)", Note = "Technical Lead(s) member" },
                    new DimensionNote { Dimension = "Leadership", SubDimension = "Product Owner", Note = "Product Owner member" },
                    new DimensionNote { Dimension = "Leadership", SubDimension = "Management", Note = "Management member" },
                    new DimensionNote { Dimension = "Culture", SubDimension = "Team Dynamics", Note = "Team Dynamics member" },
                    new DimensionNote { Dimension = "Foundation", SubDimension = "Agility", Note = "Agility member" },
                    new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = "Team Structure member" }
                };

                _stackHolderNotes = new List<DimensionNote>
                {
                    new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence StakeHolder" }
                };


                setupUi.CompleteTeamMemberSurvey(new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_teamId.Name, _teamAssessment.AssessmentName),
                    To = MemberUser.Username,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                }, _teamNotes);
                setupUi.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, _teamAssessment.AssessmentName, 5, _stackHolderNotes);

                setupUi.StartSharingAssessment(_teamId.TeamId, _teamAssessment.AssessmentName);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 51053
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_OneByOne()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Hide all team members comments one by one and verifying");
            foreach (var comment in _teamNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }

            Log.Info("Hide all stakeholders comments one by one and verifying");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }

            Log.Info("Logout as company admin and login as a member");
            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Verify that assessment team members comment is not visible to member");
            foreach (var comment in _teamNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
            }

            Log.Info("Verify that assessment stakeholders comment is not visible to member");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
            }

            Log.Info("Logout as a member and login as a company admin");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("UnHide all team members comments one by one and verifying ");
            foreach (var comments in _teamNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comments.SubDimension, comments.Note);
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comments.SubDimension, comments.Note), $"{comments.SubDimension} raw is grayed out");
            }

            Log.Info("UnHide all stakeholders comments one by one and verifying ");
            foreach (var comments in _stackHolderNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comments.SubDimension, comments.Note);
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comments.SubDimension, comments.Note), $"{comments.SubDimension} raw is grayed out");
            }

            Log.Info("Log out as company admin and login as a member");
            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Verify that assessment team members comments is visible to member");
            foreach (var comment in _teamNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
            }

            Log.Info("Verify that assessment stakeholders comments is visible to member");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 51053
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_All()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            radarPage.ClickOnHideAllTeamCommentsButton();

            Log.Info("Hide all team comments and verifying");
            foreach (var comment in _teamNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }

            radarPage.ClickOnHideAllStakeholderCommentsButton();

            Log.Info("Hide all stakeholder comments and verifying");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }

            Log.Info("Logout as company admin and login as a member");
            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
             teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Verify that assessment team comment is not visible to member");
            foreach (var comment in _teamNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
            }

            Log.Info("Verify that assessment stakeholder comment is not visible to member");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
            }

            Log.Info("Logout as a member and login as a company admin");
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            radarPage.ClickOnUnHideAllTeamCommentsButton();

            Log.Info("UnHide all team comment and verifying ");
            foreach (var comments in _teamNotes)
            {
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comments.SubDimension, comments.Note), $"{comments.SubDimension} raw is grayed out");
            }

            radarPage.ClickOnUnHideAllStakeholderCommentsButton();

            Log.Info("UnHide all stakeholder comment and verifying ");
            foreach (var comments in _stackHolderNotes)
            {
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comments.SubDimension, comments.Note), $"{comments.SubDimension} raw is grayed out");
            }

            Log.Info("Log out as company admin and login as a member");
            topNav.LogOut();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Verify that assessment team comments is visible to member");
            foreach (var comment in _teamNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
            }

            Log.Info("Verify that assessment stakeholder comments is visible to member");
            foreach (var comment in _stackHolderNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 51053
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_InvisibleForMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(MemberUser.Username, MemberUser.Password);

            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            Log.Info("Verify that 'Hide' button displayed or not ?");
            Assert.IsFalse(radarPage.IsHideAllCommentsIconDisplayed(), "'Hide All Comments' icon is displayed");
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is displayed");
            Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is displayed");
            Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(Constants.MemberSurveyInfo.SubDimension, Constants.MemberSurveyInfo.Note),
                "'Hide' button is displayed ");
        }
    }
}