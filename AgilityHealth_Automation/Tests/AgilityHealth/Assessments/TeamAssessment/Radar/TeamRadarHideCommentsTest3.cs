using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamRadarHideCommentsTest3 : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static TeamResponse _createTeamResponse;
        private static TeamHierarchyResponse _teamId;
        private static List<DimensionNote> _teamMemberNotes;
        private static List<DimensionNote> _stakeholderNotes;
        private static TeamAssessmentInfo _teamAssessmentTeamMember, _teamAssessmentStakeholder, _teamAssessment;

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
                    FirstName = SharedConstants.TeamMember1.FirstName,
                    LastName = SharedConstants.TeamMember1.LastName,
                    Email = SharedConstants.TeamMember1.Email
                });
                _team.Stakeholders.Add(new AddStakeholderRequest
                {
                    FirstName = SharedConstants.Stakeholder2.FirstName,
                    LastName = SharedConstants.Stakeholder2.LastName,
                    Email = SharedConstants.Stakeholder2.Email
                });
                _createTeamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_createTeamResponse.Name);

                // Create a team assessment with 1 team member
                _teamAssessmentTeamMember = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"TeamComments_TeamMember{Guid.NewGuid()}",
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList()
                };

                // Create a team assessment with 1 stakeholder
                _teamAssessmentStakeholder = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"TeamComments_Stakeholder{Guid.NewGuid()}",
                    StakeHolders = _team.Stakeholders.Select(a => a.FirstName + " " + a.LastName).ToList()
                };

                // Create a team assessment with 1 team member and 1 stakeholder 
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"TeamComments{Guid.NewGuid()}",
                    TeamMembers = _team.Members.Select(a => a.FullName()).ToList(),
                    StakeHolders = _team.Stakeholders.Select(a => a.FirstName + " " + a.LastName).ToList()
                };

                setupUi.AddTeamAssessment(_teamId.TeamId, _teamAssessmentTeamMember);
                setupUi.AddTeamAssessment(_teamId.TeamId, _teamAssessmentStakeholder);
                setupUi.AddTeamAssessment(_teamId.TeamId, _teamAssessment);

                // Fill the survey
                _teamMemberNotes = new List<DimensionNote>
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

                _stakeholderNotes = new List<DimensionNote>
                {
                    new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence StakeHolder" }
                };

                // Complete survey for '_teamAssessmentTeamMember'
                setupUi.CompleteTeamMemberSurvey(new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_teamId.Name, _teamAssessmentTeamMember.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                }, _teamMemberNotes);

                // Complete survey for '_teamAssessmentStakeholder'

                setupUi.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, _teamAssessmentStakeholder.AssessmentName, 5, _stakeholderNotes);

                // Complete survey for '_teamAssessment'
                setupUi.CompleteTeamMemberSurvey(new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_teamId.Name, _teamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                }, _teamMemberNotes);

                setupUi.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, _teamAssessment.AssessmentName, 5, _stakeholderNotes);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_All_ViaPresenterButton_For_TeamMember()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the team and Click on the assessment - {_teamAssessmentTeamMember.AssessmentName} and Verify 'Presenter view' icon ");
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentTeamMember.AssessmentName);
            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Presenter View'' icon should be displayed ");

            Log.Info("Turn off 'Presenter View'");
            if (radarPage.GetHideAllCommentsIconTitleAttribute().Contains("Display Hidden Comments"))
            {
                radarPage.HideUnHideAllComments();
            }

            const int numberOfRecords = 2;
            var limitedNotes = _teamMemberNotes.GetRange(0, numberOfRecords);
            var remainingNotes = _teamMemberNotes.GetRange(numberOfRecords, _teamMemberNotes.Count - numberOfRecords);

            Log.Info("Verify that 'Hide All Team Comments' button should be displayed");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is disabled");

            Log.Info("Verify that 'Hide' button should be displayed for all team comments");
            foreach (var comment in _teamMemberNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note),
                    $"'Hide' button is displayed for '{comment.Note}' comment");
            }

            Log.Info("Hide few team members comments");
            foreach (var comment in limitedNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Turn on 'Presenter View' and Verify that 'Hide All Team Comments' button should not be displayed ");
            radarPage.HideUnHideAllComments();
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide Team Comments' button is displayed");

            Log.Info("Verify that Hidden comments and 'Hide' button should not be displayed for all the team comments ");
            foreach (var comment in limitedNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is displayed for '{comment.Note}' comment");
            }
            foreach (var comment in remainingNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
            }

            Log.Info("Turn off 'Presenter View' and 'UnHide All Team Comments' button should be displayed");
            radarPage.HideUnHideAllComments();
            Assert.IsTrue(radarPage.IsUnHideAllTeamCommentsButtonDisplayed(), "'UnHide All Team Comments' button is not displayed");
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is displayed");

            Log.Info("Verify that hidden comments should be in grayed");
            foreach (var comment in limitedNotes)
            {
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }
            foreach (var comment in remainingNotes)
            {
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is grayed out");
            }

            foreach (var comment in limitedNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Verify that 'Hide All Team Comments' button should be displayed");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All Team Comments' button is not displayed");
            Assert.IsFalse(radarPage.IsUnHideAllTeamCommentsButtonDisplayed(), "'UnHide All Team Comments' button is displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_All_ViaPresenterButton_For_Stakeholder()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the team an Click on the assessment - {_teamAssessmentStakeholder.AssessmentName} and Verify that 'Presenter View' icon should be displayed");
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentStakeholder.AssessmentName);
            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Presenter View'' icon should be displayed ");

            Log.Info("Turn off 'Presenter View'");
            if (radarPage.GetHideAllCommentsIconTitleAttribute().Contains("Display Hidden Comments"))
            {
                radarPage.HideUnHideAllComments();
            }
            Log.Info("Verify that 'Hide all Stakeholder comments' button should be displayed");
            Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is not displayed");

            Log.Info("Verify that 'Hide' button should be displayed for all stakeholder comments");
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is displayed for '{comment.Note}' comment");
            }

            Log.Info("Hide few stakeholder comments");
            foreach (var comment in _stakeholderNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Verify that 'UnHide all Stakeholder comments' button should be displayed");
            Assert.IsTrue(radarPage.IsUnHideAllStakeholderCommentsButtonDisplayed(), "'UnHide all Stakeholder Comments' button is not displayed");

            Log.Info("Verify that hidden comments should be grayed");
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }
            foreach (var comment in _stakeholderNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Verify that 'Hide All Stakeholder Comments' button should be enabled");
            Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All stakeholder Comments' button is not displayed");
            Assert.IsFalse(radarPage.IsUnHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All stakeholder Comments' button is displayed");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_UnHide_All_ViaPresenterButton_For_Stakeholder()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the team an Click on the assessment - {_teamAssessmentStakeholder.AssessmentName} and Verify that 'Presenter View' icon should be displayed");
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentStakeholder.AssessmentName);
            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Presenter View'' icon should be displayed ");

            Log.Info("Turn on 'Presenter View'");
            if (radarPage.GetHideAllCommentsIconTitleAttribute().Contains("Remove Hidden Comments from View"))
            {
                radarPage.HideUnHideAllComments();
            }

            Log.Info("Verify that 'Hide all Stakeholder comments', 'UnHide all Stakeholder comments' buttons should not be displayed");
            Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is displayed");
            Assert.IsFalse(radarPage.IsUnHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All Stakeholder Comments' button is displayed");

            Log.Info("Verify that 'Hide' button should not be displayed for all stakeholder comments");
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is displayed for '{comment.Note}' comment");
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Assessment_Comments_Hide_UnHide_All_ViaPresenterButton()
        {
            VerifySetup(_classInitFailed);
            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info($"Navigate to the team and Click on the assessment - {_teamAssessment.AssessmentName} and 'Presenter View' icon should be displayed");
            teamAssessmentDashboard.NavigateToPage(_teamId.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            Assert.IsTrue(radarPage.IsHideAllCommentsIconDisplayed(), "'Presenter View' icon should be displayed ");

            Log.Info("Turn off 'Presenter View'");
            if (radarPage.GetHideAllCommentsIconTitleAttribute().Contains("Display Hidden Comments"))
            {
                radarPage.HideUnHideAllComments();
            }

            Log.Info("Turn off 'Presenter View'");
            const int numberOfRecords = 2;
            var limitedNotes = _teamMemberNotes.GetRange(0, numberOfRecords);
            var remainingNotes = _teamMemberNotes.GetRange(numberOfRecords, _teamMemberNotes.Count - numberOfRecords);

            Log.Info("Verify that 'Hide all Team comments 'and 'Hide all Stakeholder' buttons should be displayed");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All TeamMember Comments' button is not displayed");
            Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is not displayed");

            Log.Info("Verify that 'Hide' button should be displayed for all team members and stakeholders comments");
            foreach (var comment in _teamMemberNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note),
                    $"'Hide' button is displayed for '{comment.Note}' comment");
            }
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note),
                   $"'Hide' button is displayed for '{comment.Note}' comment");
            }

            Log.Info("Hide few team members and stakeholder comments");
            foreach (var comment in limitedNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }
            foreach (var comment in _stakeholderNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Turn on 'Presenter View' and Verify that 'Hide all Team comments' and 'Hide all Stakeholders' buttons should not be displayed");
            radarPage.HideUnHideAllComments();
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All team Comments' button is displayed");
            Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All Stakeholder Comments' button is displayed");

            Log.Info("Verify that Hidden comments and 'Hide' button should not be displayed for all team members and stakeholders comments ");
            foreach (var comment in limitedNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is displayed for '{comment.Note}' comment");
            }
            foreach (var comment in remainingNotes)
            {
                Assert.IsTrue(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is not displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is not displayed for '{comment.Note}' comment");
            }
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsFalse(radarPage.IsDimensionNoteDisplayed(comment.SubDimension, comment.Note), $"{comment.Note} is displayed");
                Assert.IsFalse(radarPage.IsCommentHideButtonDisplayed(comment.SubDimension, comment.Note), $"'Hide' button is displayed for '{comment.Note}' comment");
            }

            Log.Info("Turn off 'Presenter View' and Verify that 'Hide all Team comments' and 'Hide all stakeholders' buttons should be displayed");
            radarPage.HideUnHideAllComments();
            Assert.IsFalse(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All team member Comments' button is disabled");
            Assert.IsTrue(radarPage.IsUnHideAllTeamCommentsButtonDisplayed(), "'UnHide All team member Comments' button is not displayed");
            Assert.IsFalse(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All stakeholder Comments' button is displayed");
            Assert.IsTrue(radarPage.IsUnHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All stakeholder Comments' button is not displayed");

            Log.Info("Verify that hidden comments should be in grayed for team member and stakeholder");
            foreach (var comment in limitedNotes)
            {
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }
            foreach (var comment in remainingNotes)
            {
                Assert.IsFalse(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is grayed out");
            }
            foreach (var comment in _stakeholderNotes)
            {
                Assert.IsTrue(radarPage.IsCommentGrayedOut(comment.SubDimension, comment.Note), $"{comment.SubDimension} raw is not grayed out");
            }

            foreach (var comment in limitedNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }
            foreach (var comment in _stakeholderNotes)
            {
                radarPage.ClickOnCommentHideUnHideButton(comment.SubDimension, comment.Note);
            }

            Log.Info("Verify that 'Hide All Team Comments' and 'Hide All Stakeholder Comments' buttons should be enabled");
            Assert.IsTrue(radarPage.IsHideAllTeamCommentsButtonDisplayed(), "'Hide All team member Comments' button is not disabled");
            Assert.IsFalse(radarPage.IsUnHideAllTeamCommentsButtonDisplayed(), "'UnHide All team member Comments' button is displayed");
            Assert.IsTrue(radarPage.IsHideAllStakeholderCommentsButtonDisplayed(), "'Hide All stakeholder Comments' button is not displayed");
            Assert.IsFalse(radarPage.IsUnHideAllStakeholderCommentsButtonDisplayed(), "'UnHide All stakeholder Comments' button is displayed");
        }
    }
}