using System;
using System.Collections.Generic;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.MultiTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("MultiTeam")]
    public class MultiTeamRadarTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2, Constants.TeamMemberName3, Constants.TeamMemberName4 },
            StakeHolders = new List<string> { Constants.StakeholderName2, Constants.StakeholderName3 },
            EndDate = DateTime.Today.AddHours(23)
        };

        private const int TeamMember1Answer = 4;
        private const int TeamMember2Answer = 6;
        private const int TeamMember3Answer = 6;
        private const int TeamMember4Answer = 8;
        private const int Stakeholder2Answer = 9;
        private const int Stakeholder3Answer = 7;

        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static List<int> _teamMemberSurveyAnswers;
        private static List<int> _stakeholderSurveyAnswers;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(Constants.TeamForMultiTeamRadar);
                _multiTeam = teams.GetTeamByName(Constants.MultiTeamForRadar);

                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment);

                var teamMemberEmailSearchList = new List<EmailSearch>
                {
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember1.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember2.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember3.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember4.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },

                };
                _teamMemberSurveyAnswers = new List<int> {TeamMember1Answer, TeamMember2Answer , TeamMember3Answer , TeamMember4Answer };

                var stakeholderEmailList = new List<string> { SharedConstants.Stakeholder2.Email, SharedConstants.Stakeholder3.Email };
                _stakeholderSurveyAnswers = new List<int> { Stakeholder2Answer, Stakeholder3Answer};

                setup.CompleteTeamMemberSurvey(teamMemberEmailSearchList, _teamMemberSurveyAnswers);
                setup.CompleteStakeholderSurvey(_team.Name, stakeholderEmailList, TeamAssessment.AssessmentName, _stakeholderSurveyAnswers);
                setup.CloseTeamAssessment(_team.TeamId, TeamAssessment.AssessmentName);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 45551
        [TestCategory("DownloadPDF")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeam_Radar_ExportRadarToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtDashboardPage.NavigateToPage(_multiTeam.TeamId);

            var fileName = $"{_multiTeam.Name} {User.CompanyName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            radarPage.ClickExportToPdf();

            radarPage.ClickCreatePdf();

            FileUtil.WaitUntilFileDownloaded(fileName);

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"{fileName} isn't downloaded successfully");
        }


        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeam_Radar_Filter_VerifyHideTeamNames()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtDashboardPage.NavigateToPage(_multiTeam.TeamId);

            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            radarPage.Filter_OpenFilterSidebar();
            multiTeamRadarPage.SelectHideTeamNamesCheckbox();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist("Team 1"), "Team 1 doesn't exists.");
            Assert.IsFalse(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist(_team.Name), $"{_team.Name} team does exists.");

            multiTeamRadarPage.SelectHideTeamNamesCheckbox();

            Assert.IsTrue(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist(_team.Name), $"{_team.Name} team doesn't exists.");
            Assert.IsFalse(multiTeamRadarPage.Filter_TeamTab_DoesTeamExist("Team 1"), "Team 1 does exists.");

        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeam_Radar_Filter_VerifyHideIndividualDots()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtDashboardPage.NavigateToPage(_multiTeam.TeamId);

            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            Thread.Sleep(120000); // Radar Takes up 4-5 minutes for newly created Team data to be plotted
            Driver.RefreshPage();
            radarPage.Filter_OpenFilterSidebar();
            multiTeamRadarPage.Filter_TeamTab_SelectFilterItemCheckboxByName(_team.Name, false);
            multiTeamRadarPage.Filter_TeamTab_SelectFilterItemCheckboxByName(_multiTeam.Name);

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }

            multiTeamRadarPage.SelectHideIndividualDotsCheckbox();

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                Assert.AreEqual(0, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                Assert.AreEqual(0, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }

            multiTeamRadarPage.SelectHideIndividualDotsCheckbox();
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("dots", "red", comp),
                    $"Competency: <{comp}> dot count doesn't match");
            }
        }
    }
}
