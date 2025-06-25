using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Teams
{
    [TestClass]
    [TestCategory("TeamsDashboard"), TestCategory("Dashboard")]
    public class TeamDashboardColumnDataVerificationTest : IndividualAssessmentBaseTest
    {
        public static bool ClassInitFailed;
        private static AddTeamWithMemberRequest _team,_goiTeam,_multiTeam,_enterpriseTeam;
        private static TeamResponse _createGoiTeamResponse, _teamResponse, _multiTeamResponse, _enterpriseTeamResponse;
        private static TeamAssessmentInfo _teamAssessment;
        private static CreateIndividualAssessmentRequest _individualAssessmentRequest1, _individualAssessmentRequest2;
    
        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                var setup = new SetUpMethods(_, TestEnvironment);
                var setupUi = new SetupTeardownApi(TestEnvironment);

                //Create a Team
                _team = TeamFactory.GetNormalTeam("Team");
                _team.Members.Add(SharedConstants.TeamMember1);
                 _teamResponse = setupUi.CreateTeam(_team).GetAwaiter().GetResult();
                var teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

                //Create a GOI Team
                _goiTeam = TeamFactory.GetGoiTeam("GoiTeam");
                _goiTeam.ExternalIdentifier = Guid.NewGuid().ToString("D");
                _goiTeam.Members.Add(SharedConstants.TeamMember1);
                _goiTeam.Members.Add(SharedConstants.TeamMember2);
                _createGoiTeamResponse = setupUi.CreateTeam(_goiTeam).GetAwaiter().GetResult();
               
                //Create a MultiTeam
                _multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                _multiTeam.Members.Add(SharedConstants.TeamMember1);
                _multiTeam.Members.Add(SharedConstants.TeamMember2);
                _multiTeam.Members.Add(SharedConstants.TeamMember3);
                _multiTeamResponse = setupUi.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setupUi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _teamResponse.Uid }).GetAwaiter().GetResult();
                setupUi.AddSubteams(_multiTeamResponse.Uid, new List<Guid> { _createGoiTeamResponse.Uid }).GetAwaiter().GetResult();
               

                //Create a Enterprise Team
                _enterpriseTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeam");
                _enterpriseTeam.Members.Add(SharedConstants.TeamMember1);
                 _enterpriseTeamResponse = setupUi.CreateTeam(_enterpriseTeam).GetAwaiter().GetResult();
                setupUi.AddSubteams(_enterpriseTeamResponse.Uid, new List<Guid> { _multiTeamResponse.Uid }).GetAwaiter().GetResult();

                //Create a Team Assessment
                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = new List<string> { _team.Members.First().FullName() },
                    StartDate = DateTime.Today
                }; 
                setup.AddTeamAssessment(teamId, _teamAssessment);

                //Create a Individual Assessment
                var individualDataResponse1 = GetIndividualAssessment(setupUi, _createGoiTeamResponse, "IA1_");
                var individualDataResponse2 = GetIndividualAssessment(setupUi, _createGoiTeamResponse, "IA2_");
                _individualAssessmentRequest1 = individualDataResponse1.Item2;
                setupUi.CreateIndividualAssessment(_individualAssessmentRequest1, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
                _individualAssessmentRequest2 = individualDataResponse2.Item2;
                setupUi.CreateIndividualAssessment(_individualAssessmentRequest2, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void TeamDashboard_Column_Data_Verification()
        {
            VerifySetup(ClassInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);

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

            Log.Info($"Login as {User.FullName} and navigate to the 'Team Dashboard'");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            dashboardPage.GridTeamView();

            Log.Info("Search created normal team and Verify that all columns' value");
            dashboardPage.SearchTeam(_team.Name);
            dashboardPage.SelectAllColumns();
            Assert.AreEqual(_team.Name, dashboardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");

            var teamWorkType = _team.Tags.Where(e => e.Category.Equals("Work Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(teamWorkType, dashboardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("1", dashboardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual(_teamResponse.Subteams.Count.ToString(), dashboardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_team.Members.Count.ToString(), dashboardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual(_teamAssessment.StartDate.ToString("MM/dd/yyyy"), dashboardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            var teamTagList = _team.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tags in teamTagList)
            {
                Assert.IsTrue(dashboardPage.GetCellValue(1, teamTags).Contains(tags), $"{tags} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_team.ExternalIdentifier, dashboardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_team.Type, dashboardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, subTeams), $"{subTeams} column value doesn't match");
            Assert.AreEqual(_multiTeam.Name, dashboardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Name, dashboardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");

            Log.Info("Search created goi team and Verify that all column's value");
            dashboardPage.SearchTeam(_goiTeam.Name);
            dashboardPage.SelectAllColumns();
            Assert.AreEqual(_goiTeam.Name, dashboardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var goiTeamWorkType = _goiTeam.Tags.Where(e => e.Category.Equals("Work Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(goiTeamWorkType.ToLower(), dashboardPage.GetCellValue(1, workType).ToLower(), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashboardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("2", dashboardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual(_createGoiTeamResponse.Subteams.Count.ToString(), dashboardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_goiTeam.Members.Count.ToString(), dashboardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual(_individualAssessmentRequest2.Start.ToString("MM/dd/yyyy"), dashboardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _goiTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tags in teamTagList)
            {
                Assert.IsTrue(dashboardPage.GetCellValue(1, teamTags).ToLower().Contains(tags.ToLower()), $"{tags} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_goiTeam.ExternalIdentifier, dashboardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_goiTeam.Type, dashboardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, subTeams), $"{subTeams} column value doesn't match");
            Assert.AreEqual(_multiTeam.Name, dashboardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Name, dashboardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");

            Log.Info("Search created multi team team and Verify that all columns' value");
            dashboardPage.SearchTeam(_multiTeam.Name);
            dashboardPage.SelectAllColumns();
            Assert.AreEqual(_multiTeam.Name, dashboardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var multiTeamTag = _multiTeam.Tags.Where(e => e.Category.Equals("MultiTeam Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(multiTeamTag, dashboardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashboardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual("2", dashboardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_multiTeam.Members.Count.ToString(), dashboardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, lastDateOfAssessment),$"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _multiTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tags in teamTagList)
            {
                Assert.IsTrue(dashboardPage.GetCellValue(1, teamTags).Contains(tags), $"{tags} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_multiTeam.ExternalIdentifier, dashboardPage.GetCellValue(1, externalId),$"{externalId} column value doesn't match");
            Assert.AreEqual(_multiTeam.Type, dashboardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.IsTrue(dashboardPage.GetCellValue(1,subTeams).Contains(_team.Name), $"{subTeams} column value doesn't contains team name");
            Assert.IsTrue(dashboardPage.GetCellValue(1, subTeams).Contains(_goiTeam.Name), $"{subTeams} column value doesn't contains goi team name");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Name, dashboardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");

            Log.Info("Search created enterprise team team and Verify that all columns' value");
            dashboardPage.SearchTeam(_enterpriseTeam.Name);
            dashboardPage.SelectAllColumns();
            Assert.AreEqual(_enterpriseTeam.Name, dashboardPage.GetCellValue(1, teamName), $"{teamName} column value doesn't match");
            var enterpriseTeamTag = _enterpriseTeam.Tags.Where(e => e.Category.Equals("Enterprise Team Type")).Select(a => a.Tags).First().ToList().ListToString();
            Assert.AreEqual(enterpriseTeamTag, dashboardPage.GetCellValue(1, workType), $"{workType} column value doesn't match");
            Assert.AreEqual("0", dashboardPage.GetCellValue(1, numberOfTeamAssessment), $"{numberOfTeamAssessment} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, numberOfIndividualAssessment), $"{numberOfIndividualAssessment} column value doesn't match");
            Assert.AreEqual("1", dashboardPage.GetCellValue(1, numberOfSubTeams), $"{numberOfSubTeams} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Members.Count.ToString(), dashboardPage.GetCellValue(1, numberOfTeamMembers), $"{numberOfTeamMembers} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, lastDateOfAssessment), $"{lastDateOfAssessment} column value doesn't match");
            teamTagList = _enterpriseTeam.Tags.Select(e => e.Tags.First()).ToList();
            foreach (var tags in teamTagList)
            {
                Assert.IsTrue(dashboardPage.GetCellValue(1, teamTags).Contains(tags), $"{teamTags} column value doesn't contains correct tags");
            }
            Assert.AreEqual(_enterpriseTeam.ExternalIdentifier, dashboardPage.GetCellValue(1, externalId), $"{externalId} column value doesn't match");
            Assert.AreEqual(_enterpriseTeam.Type, dashboardPage.GetCellValue(1, type), $"{type} column value doesn't match");
            Assert.IsTrue(dashboardPage.GetCellValue(1, subTeams).Contains(_team.Name), $"{subTeams} column value doesn't contains team name");
            Assert.IsTrue(dashboardPage.GetCellValue(1, subTeams).Contains(_goiTeam.Name), $"{subTeams} column value doesn't contains goi team name");
            Assert.AreEqual(_multiTeam.Name, dashboardPage.GetCellValue(1, multiTeamNames), $"{multiTeamNames} column value doesn't match");
            Assert.AreEqual("", dashboardPage.GetCellValue(1, enterpriseTeamNames), $"{enterpriseTeamNames} column value doesn't match");
        }
    }
}