using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Radar
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("IARadars"), TestCategory("Assessments")]
    public class IndividualRadarTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static CreateIndividualAssessmentRequest _assessment;
        private static AddTeamWithMemberRequest _team;
        private static User _member;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _member = TestEnvironment.UserConfig.GetUserByDescription("member");

                var adminUser = (User.IsMember()) ?
                    TestEnvironment.UserConfig.GetUserByDescription("user 3") : User;
                // add new GOI team with members
                _team = TeamFactory.GetGoiTeam("IndividualRadar");
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = _member.FirstName,
                    LastName = _member.LastName,
                    Email = _member.Username
                });

                var setup = new SetupTeardownApi(TestEnvironment);
                var teamResponse = setup.CreateTeam(_team, adminUser).GetAwaiter().GetResult();

                // add new Individual assessment
                _assessment = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, teamResponse.Uid, $"RadarIA{Guid.NewGuid()}");
                var individualDataResponse = GetIndividualAssessment(setup, teamResponse, "ReviewRoleEditIASection_", adminUser);
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

                foreach (var member in teamResponse.Members)
                {
                    _assessment.IndividualViewers.Add(new UserRequest()
                    {
                        Uid = member.Uid,
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        Email = member.Email
                    });
                }

                _assessment = individualDataResponse.Item2;
                _assessment.Members = teamResponse.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

                var reviewer = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessment.Members.First().Reviewers.Add(reviewer.ToAddIndividualMemberRequest());

                setup.CreateIndividualAssessment(_assessment,
                    SharedConstants.IndividualAssessmentType, adminUser).GetAwaiter().GetResult();

                // complete survey
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                foreach (var member in _assessment.Members)
                {
                    setupUi.CompleteIndividualSurvey(member.Email, _assessment.PointOfContactEmail);
                }

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Radar_IA_ExportRadarToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var fileName = $"{_team.Name} {_assessment.AssessmentName} - {_assessment.Members.First().FullName()}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();

            var user = (User.IsMember()) ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.ClickOnTeamName(_team.Name);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();

            iAssessmentDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);
            iAssessmentDashboardPage.ClickOnRadar($"{_assessment.AssessmentName} - {_assessment.Members.First().FirstName} {_assessment.Members.First().LastName}");

            radarPage.ClickExportToPdf();

            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} is not downloaded successfully");

            var pdfData = FileUtil.GetPdfData(fileName);
            const string expectedText = "1 of 2 Respondents";
            Assert.IsTrue(pdfData.Contains(expectedText), $"{expectedText} text is not present on pdf");
        }

        [TestMethod]
        [TestCategory("DownloadPDF")]
        [TestCategory("Critical")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Radar_IA_RollUp_ExportToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var iAssessmentDashboardPage = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var fileName = $"{_team.Name} {_assessment.AssessmentName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            login.NavigateToPage();

            var user = User.IsMember() ? _member : User;
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.GridTeamView();
            dashBoardPage.SearchTeam(_team.Name);
            dashBoardPage.ClickOnTeamName(_team.Name);
            teamAssessmentDashboard.SwitchToIndividualAssessmentView();
            iAssessmentDashboardPage.ClickOnAssessmentType(SharedConstants.IndividualAssessmentType);

            var rollUpName = $"{_assessment.AssessmentName} - Roll up";
            iAssessmentDashboardPage.ClickOnRadar(rollUpName);

            radarPage.ClickExportToPdf();

            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} is not downloaded successfully");
        }
    }
}
