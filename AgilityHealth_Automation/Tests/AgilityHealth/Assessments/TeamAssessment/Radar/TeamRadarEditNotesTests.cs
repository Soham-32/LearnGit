using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamRadarEditNotesTests : BaseTest
    {
        private static TeamHierarchyResponse _team;
        private static TeamAssessmentInfo _teamAssessment;
        private static List<DimensionNote> _notes;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id)
                .GetTeamByName(SharedConstants.Team);
            _notes = new List<DimensionNote>
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
                new DimensionNote { Dimension = "Foundation", SubDimension = "Team Structure", Note = "Team Structure member" },
                new DimensionNote { Dimension = "Open", SubDimension = "Strengths", Note = "Strengths member" },
                new DimensionNote { Dimension = "Open", SubDimension = "Improvements", Note = "Improvements member" },
                new DimensionNote { Dimension = "Open", SubDimension = "Impediments", Note = "Impediments member" }
            };

            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = $"TeamComments{Guid.NewGuid()}",
                TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() }
            };
            var setupUi = new SetUpMethods(testContext, TestEnvironment);
            setupUi.AddTeamAssessment(_team.TeamId, _teamAssessment);
            setupUi.CompleteTeamMemberSurvey(new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            }, _notes);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Radar_TA_EditNotes()
        {
            var login = new LoginPage(Driver, Log);
            var radarPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            _notes.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                radarPage.EditDimensionNote(n.SubDimension, n.Note, $"{n.Note} updated"));

            Log.Info("Verify updated dimension notes display correctly");
            _notes.Where(n => n.Dimension != "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetDimensionNotes(n.SubDimension), $"{n.Note} updated"));

            _notes.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                radarPage.EditOpenEndNote(n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension, n.Note, $"{n.Note} updated"));

            Log.Info("Verify open ended notes display correctly");
            _notes.Where(n => n.Dimension == "Open").ToList().ForEach(n =>
                Assert.That.ListContains(radarPage.GetOpenEndedNotes(
                    n.SubDimension == "Improvements" ? "Growth Opportunities" : n.SubDimension), $"{n.Note} updated"));

        }
    }
}