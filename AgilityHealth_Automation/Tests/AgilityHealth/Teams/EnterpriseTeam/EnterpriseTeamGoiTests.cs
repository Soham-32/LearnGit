using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.EnterpriseTeam;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam")]
    public class EnterpriseTeamGoiTests : BaseTest
    {
        public static bool ClassInitFailed;
        private static TeamResponse _team;
        private static AddTeamWithMemberRequest _multiTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                var teamRequest = TeamFactory.GetGoiTeam("EditIA", 1);
                _team = setup.CreateTeam(teamRequest).GetAwaiter().GetResult();

                var assessment = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, _team.Uid, $"TakeIA_{Guid.NewGuid()}");
                assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                setup.CreateIndividualAssessment(assessment, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();

                _multiTeam = TeamFactory.GetMultiTeam("EditMT", "Group Of Individuals");
                var multiTeamResponse = setup.CreateTeam(_multiTeam).GetAwaiter().GetResult();
                setup.AddSubteams(multiTeamResponse.Uid, new List<Guid> { _team.Uid }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Verify_Goi_EnterpriseTeam_Create()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createEnterpriseTeamPage = new CreateEnterpriseTeamPage(Driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);
            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.EnterpriseTeam);
            dashBoardPage.ClickAddTeamButton();

            var enterpriseTeamInfo = new EnterpriseTeamInfo
            {
                TeamName = "ET_" + RandomDataUtil.GetTeamName(),
                TeamType = "Group Of Individuals",
                ExternalIdentifier = "external identifier",
                Department = "IT",
                DateEstablished = DateTime.Now,
                AgileAdoptionDate = DateTime.Now,
                Description = "test description for enterprise team",
                TeamBio = $"{RandomDataUtil.GetTeamBio()} enterprise team",
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg")
            };

            Assert.IsTrue(createEnterpriseTeamPage.IsTeamTypePresent(enterpriseTeamInfo.TeamType), $"{enterpriseTeamInfo.TeamType} is not Present");
            createEnterpriseTeamPage.EnterEnterpriseTeamInfo(enterpriseTeamInfo);
            var teamImageSource = createEnterpriseTeamPage.GetTeamImageSource();
            enterpriseTeamInfo.ImagePath = teamImageSource;

            createEnterpriseTeamPage.GoToAddSubteams();
            Assert.IsTrue(addMtSubTeamPage.IsSubTeamPresent(_multiTeam.Name), $"{_multiTeam.Name} is not Present");

            addMtSubTeamPage.SelectSubTeam(_multiTeam.Name);
            addMtSubTeamPage.ClickAddSubTeamButton();
            addTeamMemberPage.ClickContinueToAddStakeHolder();
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToTeamDashboard();
            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(enterpriseTeamInfo.TeamName);
            Assert.AreEqual(enterpriseTeamInfo.TeamName, dashBoardPage.GetCellValue(1, "Team Name"), "Team Name doesn't match");
            Assert.AreEqual(enterpriseTeamInfo.TeamType, dashBoardPage.GetCellValue(1, "Work Type"), "Team Type doesn't match");
            Assert.AreEqual($"{SharedConstants.TeamTag}, {enterpriseTeamInfo.TeamType}", dashBoardPage.GetCellValue(1, "Team Tags"), "Team Type doesn't match");

            dashBoardPage.GoToTeamAssessmentDashboard(1);
            Assert.AreEqual(enterpriseTeamInfo.TeamName + " - " + SharedConstants.IndividualAssessmentType, radarPage.GetRadarTitle() + " - " + SharedConstants.IndividualAssessmentType, $"RadarTitle {(enterpriseTeamInfo.TeamName + " - " + SharedConstants.IndividualAssessmentType)} doesn't match");
        }
    }
}
