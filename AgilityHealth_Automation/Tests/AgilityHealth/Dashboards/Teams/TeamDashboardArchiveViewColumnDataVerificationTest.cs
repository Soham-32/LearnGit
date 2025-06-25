using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardArchiveViewColumnDataVerificationTest : IndividualAssessmentBaseTest
    {
        public static bool ClassInitFailed;
        private static AddTeamWithMemberRequest _team, _goiTeam, _multiTeam, _enterpriseTeam;
        private static TeamResponse _createGoiTeamResponse, _teamResponse, _multiTeamResponse, _enterpriseTeamResponse;
        private static TeamAssessmentInfo _teamAssessment;
        private static CreateIndividualAssessmentRequest _individualAssessmentRequest1, _individualAssessmentRequest2;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setupUi = new SetUpMethods(_, TestEnvironment);
                var setupApi = new SetupTeardownApi(TestEnvironment);

                //Create a Team
                _team = TeamFactory.GetNormalTeam("ArchiveTeam");
                _team.Members.Add(SharedConstants.TeamMember1);
                _teamResponse = setupApi.CreateTeam(_team).GetAwaiter().GetResult();
                var teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                //Create a GOI Team
                _goiTeam = TeamFactory.GetGoiTeam("ArchiveGoiTeam");
                _goiTeam.ExternalIdentifier = Guid.NewGuid().ToString("D");
                _goiTeam.Members.Add(SharedConstants.TeamMember1);
                _goiTeam.Members.Add(SharedConstants.TeamMember2);
                _createGoiTeamResponse = setupApi.CreateTeam(_goiTeam).GetAwaiter().GetResult();

                //Create a MultiTeam
                _multiTeam = TeamFactory.GetMultiTeam("ArchiveMultiTeam");
                _multiTeam.Members.Add(SharedConstants.TeamMember1);
                _multiTeam.Members.Add(SharedConstants.TeamMember2);
                _multiTeam.Members.Add(SharedConstants.TeamMember3);
                _multiTeamResponse = setupApi.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _teamResponse.Uid }).GetAwaiter().GetResult();
                setupApi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _createGoiTeamResponse.Uid }).GetAwaiter().GetResult();

                //Create a Enterprise Team
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("ArchiveEnterpriseTeam");
                _enterpriseTeam.Members.Add(SharedConstants.TeamMember1);
                _enterpriseTeamResponse = setupApi.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
                setupApi.AddSubteams(_enterpriseTeamResponse.Uid, new List<Guid> { _multiTeamResponse.Uid }).GetAwaiter().GetResult();

                //Create a Team Assessment
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = new List<string> { _team.Members.First().FullName() },
                    StartDate = DateTime.Today
                };
                setupUi.AddTeamAssessment(teamId, _teamAssessment);

                //Create a Individual Assessment
                var individualDataResponse1 = GetIndividualAssessment(setupApi, _createGoiTeamResponse, "IA1_");
                var individualDataResponse2 = GetIndividualAssessment(setupApi, _createGoiTeamResponse, "IA2_");
                _individualAssessmentRequest1 = individualDataResponse1.Item2;
                setupApi.CreateIndividualAssessment(_individualAssessmentRequest1, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
                _individualAssessmentRequest2 = individualDataResponse2.Item2;
                setupApi.CreateIndividualAssessment(_individualAssessmentRequest2, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_Archive_View_Column_Data_Verification()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);

            const string numberOfTeamAssessment = "Number of Team Assessments";
            const string numberOfIndividualAssessment = "Number of Individual Assessments";
            const string numberOfSubTeams = "Number of Sub Teams";
            const string numberOfTeamMembers = "Number of Team Members";
            const string teamName = "Team Name";
            const string workType = "Work Type";
            const string lastDateOfAssessment = "Last Date of Assessment";
            const string teamTags = "Team Tags";
            const string externalId = "External ID";
            const string type = "Type";
            const string subTeams = "Sub Teams";
            const string multiTeamNames = "Multi Teams";
            const string enterpriseTeamNames = "Enterprise Teams";

            Log.Info($"Login as {User.FullName} and navigate to the 'Team Dashboard' page");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.GridTeamView();

            Log.Info("Archive all created teams");
            var teamList= new List<string> { _team.Name,_goiTeam.Name,_multiTeam.Name,_enterpriseTeam.Name };
            foreach (var team in teamList)
            {
                dashBoardPage.SearchTeam(team);
                dashBoardPage.DeleteTeam(team, RemoveTeamReason.ArchiveProjectCompleted);
            }

            Log.Info("Go to archive state");
            dashBoardPage.FilterTeamStatus("Archived");

            Log.Info("Search created normal team and Verify that all columns' value");
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.SelectAllColumns();
            Assert.AreEqual(_team.Name, dashBoardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");

            var teamWorkType = _team.Tags.Where(e => e.Category.Equals("Work Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(teamWorkType, dashBoardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("1", dashBoardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual(_teamResponse.Subteams.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_team.Members.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual(_teamAssessment.StartDate.ToString("MM/dd/yyyy"), dashBoardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            var teamTagList = _team.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tag in teamTagList)
            {
                Assert.IsTrue(dashBoardPage.GetCellValue(1, teamTags).Contains(tag), $"{tag} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_team.ExternalIdentifier, dashBoardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_team.Type, dashBoardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, subTeams), $"{subTeams} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");

            Log.Info("Search created goi team and Verify that all column's value");
            dashBoardPage.SearchTeam(_goiTeam.Name);
            dashBoardPage.SelectAllColumns();
            Assert.AreEqual(_goiTeam.Name, dashBoardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var goiTeamWorkType = _goiTeam.Tags.Where(e => e.Category.Equals("Work Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(goiTeamWorkType.ToLower(), dashBoardPage.GetCellValue(1, workType).ToLower(), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("2", dashBoardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual(_createGoiTeamResponse.Subteams.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_goiTeam.Members.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual(_individualAssessmentRequest2.Start.ToString("MM/dd/yyyy"), dashBoardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _goiTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tag in teamTagList)
            {
                Assert.IsTrue(dashBoardPage.GetCellValue(1, teamTags).ToLower().Contains(tag.ToLower()), $"{tag} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_goiTeam.ExternalIdentifier, dashBoardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_goiTeam.Type, dashBoardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, subTeams), $"{subTeams} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match"); 

            Log.Info("Search created multi team team and Verify that all columns' value");
            dashBoardPage.SearchTeam(_multiTeam.Name);
            dashBoardPage.SelectAllColumns();
            Assert.AreEqual(_multiTeam.Name, dashBoardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var multiTeamTag = _multiTeam.Tags.Where(e => e.Category.Equals("MultiTeam Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(multiTeamTag, dashBoardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_multiTeam.Members.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _multiTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tag in teamTagList)
            {
                Assert.IsTrue(dashBoardPage.GetCellValue(1, teamTags).Contains(tag), $"{tag} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_multiTeam.ExternalIdentifier, dashBoardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_multiTeam.Type, dashBoardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, subTeams).Contains(""), $"{subTeams} column value doesn't contains team name");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, subTeams).Contains(""), $"{subTeams} column value doesn't contains goi team name");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match"); 

            Log.Info("Search created enterprise team team and Verify that all columns' value");
            dashBoardPage.SearchTeam(_enterpriseTeam.Name);
            dashBoardPage.SelectAllColumns();
            Assert.AreEqual(_enterpriseTeam.Name, dashBoardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var enterpriseTeamTag = _enterpriseTeam.Tags.Where(e => e.Category.Equals("Enterprise Team Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(enterpriseTeamTag, dashBoardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual("0", dashBoardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Members.Count.ToString(), dashBoardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _enterpriseTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tag in teamTagList)
            {
                Assert.IsTrue(dashBoardPage.GetCellValue(1, teamTags).Contains(tag), $"{teamTags} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_enterpriseTeam.ExternalIdentifier, dashBoardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Type, dashBoardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, subTeams).Contains(""), $"{subTeams} column value doesn't contains team name");
            Assert.IsTrue(dashBoardPage.GetCellValue(1, subTeams).Contains(""), $"{subTeams} column value doesn't contains goi team name");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual("", dashBoardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");
        }
    }
}
