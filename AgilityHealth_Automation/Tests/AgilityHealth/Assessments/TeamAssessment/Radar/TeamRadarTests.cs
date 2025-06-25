using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamRadarTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamAssessmentInfo _teamAssessment;
        private const int TeamMember1Answer = 4;
        private const int TeamMember2Answer = 6;
        private const int Stakeholder2Answer = 9;
        private const int Stakeholder4Answer = 7;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _radarTeam;
        private static List<int> _stakeholderSurveyAnswerList;
        private static List<int> _teamMemberSurveyAnswerList;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(SharedConstants.Team);
                _radarTeam = teams.GetTeamByName(SharedConstants.RadarTeam);

                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
                    TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() },
                    StakeHolders = new List<string> { SharedConstants.Stakeholder2.FullName(), SharedConstants.Stakeholder4.FullName() }
                };

                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, _teamAssessment);


                var teamMemberEmailSearchList = new List<EmailSearch>
                {
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember1.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember2.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },

                };
                _teamMemberSurveyAnswerList = new List<int> { TeamMember1Answer, TeamMember2Answer };

                var stakeholderEmailList = new List<string> { SharedConstants.Stakeholder2.Email, SharedConstants.Stakeholder4.Email };
                _stakeholderSurveyAnswerList = new List<int> { Stakeholder2Answer, Stakeholder4Answer };

                setup.CompleteTeamMemberSurvey(teamMemberEmailSearchList, _teamMemberSurveyAnswerList);
                setup.CompleteStakeholderSurvey(_team.Name, stakeholderEmailList, _teamAssessment.AssessmentName, _stakeholderSurveyAnswerList);
            }
            catch (System.Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 51247
        [TestCategory("CompanyAdmin"), TestCategory("Member")]
        public void Assessment_Radar_VerifyFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            assessmentDetailPage.RadarSwitchView(ViewType.Detail);
            radarPage.Filter_OpenFilterSidebar();

            Assert.IsTrue(assessmentDetailPage.Filter_IsFilterItemCheckboxSelected("All"));

            //Verifying Avg value
            var expectedavg = _teamMemberSurveyAnswerList.Average();
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Predictable Delivery' comp,will raise with Dave/Elkhair soon.Handling for now
                if (!comp.Equals("Predictable Delivery"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedavg, int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", "red", comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Select(int.Parse).ToList().OrderBy(a => a).ToList();
                Assert.AreEqual(TeamMember1Answer, dotsValue[0],
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(TeamMember2Answer, dotsValue[1],
                    $"Competency: <{comp}> dot value doesn't match");
            }


            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName("All");
            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName(Constants.TeamMemberParticipantGroup1);

            //Verifying Avg value
            var colorHex = assessmentDetailPage.Filter_GetFilterItemColor(Constants.TeamMemberParticipantGroup1);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Predictable Delivery' comp,will raise with Dave/Elkhair soon.Handling within test now.
                if (!comp.Equals("Predictable Delivery"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedavg,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp).Select(int.Parse).ToList().OrderBy(a => a).ToList();
                Assert.AreEqual(TeamMember1Answer, dotsValue[0],
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(TeamMember2Answer, dotsValue[1],
                    $"Competency: <{comp}> dot value doesn't match");
            }

            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName(Constants.TeamMemberParticipantGroup1);
            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName(Constants.StakeholderRole2);

            //Verifying Avg value
            colorHex = assessmentDetailPage.Filter_GetFilterItemColor(Constants.StakeholderRole2);
            expectedavg = _stakeholderSurveyAnswerList.Average();
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedavg,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp);
                var value = dotsValue.Select(int.Parse).ToList().OrderByDescending(a => a).ToList();
                Assert.AreEqual(Stakeholder2Answer, value[0],
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(Stakeholder4Answer, value[1],
                    $"Competency: <{comp}> dot value doesn't match");
            }

            assessmentDetailPage.Filter_SelectFilterItemCheckboxByName(Constants.TeamMemberParticipantGroup1);

            //Verifying Avg value for Constants.TMParticipantGroup1
            colorHex = assessmentDetailPage.Filter_GetFilterItemColor(Constants.TeamMemberParticipantGroup1);
            expectedavg = _teamMemberSurveyAnswerList.Average();
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Predictable Delivery' comp,will raise with Dave/Elkhair soon.Handling for now
                if (!comp.Equals("Predictable Delivery"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedavg,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values Constants.TMParticipantGroup1 
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                //Verifying whether there are two dots
                Assert.AreEqual(0, assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

            //Verifying Avg value for Constants.StakeholderRole1
            colorHex = assessmentDetailPage.Filter_GetFilterItemColor(Constants.StakeholderRole2);

            // ReSharper disable once PossibleLossOfFraction
            expectedavg = (Stakeholder2Answer + Stakeholder4Answer) / 2;

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                //Verifying whether there is only one avg line or not
                Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedavg,
                    int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", colorHex, comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values for Constants.StakeholderRole1
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                //Verifying whether there are two dots
                Assert.AreEqual(0, assessmentDetailPage.Radar_GetDotValue("dots", colorHex, comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Assessment_Radar_VerifyExportToPDF()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            var fileName = $"{_team.Name} {_teamAssessment.AssessmentName}.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"{fileName} PDF not downloaded successfully");

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Assessment_Radar_VerifyExportToExcel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);

            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);

            const string fileName = "Assessment Results.xlsx";

            FileUtil.DeleteFilesInDownloadFolder(fileName);

            radarPage.ClickExportToExcel();

            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName),
                $"{fileName} Excel not downloaded successfully");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void Assessment_Radar_VerifyThatAllRadarsAreLoadedCorrectly()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var assessmentDetailPage = new AssessmentDetailsCommonPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_radarTeam.TeamId);

            teamAssessmentDashboard.ClickOnRadar(SharedConstants.ProgramHealthRadar);

            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            //Verifying Avg value
            var expectedAvg = _teamMemberSurveyAnswerList.Average();
            foreach (var comp in Constants.ProgramHealthCompentenciesLables)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Tracking & Reporting' comp,already raised with Dave/Elkhair.Handling for now
                if (!comp.Equals("Tracking & Reporting"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedAvg, int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", "red", comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.ProgramHealthCompentenciesLables)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", "black", comp);
                Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[0]),
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[1]),
                    $"Competency: <{comp}> dot value doesn't match");
            }


            Driver.Back();

            teamAssessmentDashboard.ClickOnRadar(SharedConstants.DevOpsHealthRadar);

            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            //Verifying Avg value
            foreach (var comp in Constants.DevOpsHealthCompentenciesLables)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Flexibility & Scale' comp,already raised with Dave/Elkhair.Handling for now
                if (!comp.Equals("Flexibility & Scale"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedAvg, int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", "red", comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.DevOpsHealthCompentenciesLables)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", "black", comp);
                Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[0]),
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[1]),
                    $"Competency: <{comp}> dot value doesn't match");
            }


            Driver.Back();

            teamAssessmentDashboard.ClickOnRadar(SharedConstants.TechnicalHealthRadar);

            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            //Verifying Avg value
            foreach (var comp in Constants.TechnicalHealthCompentenciesLables)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Cross-Functional Collaboration' comp,already raised with Dave/Elkhair.Handling for now
                if (!comp.Equals("Cross-Functional Collaboration"))
                    Assert.AreEqual(1, assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify avg value
                Assert.AreEqual(expectedAvg, int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", "red", comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.TechnicalHealthCompentenciesLables)
            {
                //Verifying whether there are two dots
                Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                    $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", "black", comp);
                Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[0]),
                    $"Competency: <{comp}> dot value doesn't match");
                Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[1]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            Driver.Back();

            teamAssessmentDashboard.ClickOnRadar(SharedConstants.DevOpsAssessmentRadar);

            assessmentDetailPage.RadarSwitchView(ViewType.Detail);

            //Verifying Avg value
            foreach (var comp in Constants.DevOpsAssessmentCompentenciesLables)
            {
                //Verifying whether there is only one avg line or not

                //There is bug in system - two dots displayed for 'Operations' comp,already raised with Dave/Elkhair.Handling for now
                if (!comp.Equals("Operations"))
                {
                    //Below comps are repeated twice
                    if (comp.Equals("Automation") || comp.Equals("Quality") || comp.Equals("Priority") ||
                        comp.Equals("Improve"))
                    {
                        Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count,
                            $"Competency: <{comp}> dot count doesn't match");
                    }
                    else
                    {
                        var dotCount = assessmentDetailPage.Radar_GetDotValue("avg", "red", comp).Count;
                        Assert.AreEqual(1, dotCount,
                            $"The number of red dots for '{comp}' is incorrect. Expected: <1>. Actual: <{dotCount}>.");
                    }
                }

                Assert.AreEqual(expectedAvg, int.Parse(assessmentDetailPage.Radar_GetDotValue("avg", "red", comp)[0]),
                    $"Competency: <{comp}> dot value doesn't match");
            }

            //Verifying individual values
            foreach (var comp in Constants.DevOpsAssessmentCompentenciesLables)
            {
                //Verifying whether there are two dots
                //Below comps are repeated twice
                if (comp.Equals("Automation") || comp.Equals("Quality") || comp.Equals("Priority") ||
                    comp.Equals("Improve"))
                    Assert.AreEqual(4, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");
                else
                    Assert.AreEqual(2, assessmentDetailPage.Radar_GetDotValue("dots", "black", comp).Count,
                        $"Competency: <{comp}> dot count doesn't match");

                //Verify value for each dot
                var dotsValue = assessmentDetailPage.Radar_GetDotValue("dots", "black", comp);
                if (comp.Equals("Automation") || comp.Equals("Quality") || comp.Equals("Priority") ||
                    comp.Equals("Improve"))
                {
                    Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[0]),
                        $"Competency: <{comp}> dot value doesn't match");
                    Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[1]),
                        $"Competency: <{comp}> dot value doesn't match");
                    Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[2]),
                        $"Competency: <{comp}> dot value doesn't match");
                    Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[3]),
                        $"Competency: <{comp}> dot value doesn't match");
                }
                else
                {
                    Assert.AreEqual(TeamMember1Answer, int.Parse(dotsValue[0]),
                        $"Competency: <{comp}> dot value doesn't match");
                    Assert.AreEqual(TeamMember2Answer, int.Parse(dotsValue[1]),
                        $"Competency: <{comp}> dot value doesn't match");
                }
            }
        }
    }
}