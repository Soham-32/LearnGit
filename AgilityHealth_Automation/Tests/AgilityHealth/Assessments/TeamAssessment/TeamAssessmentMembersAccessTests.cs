using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentMembersAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static AddTeamWithMemberRequest _team;
        private static int _teamId;
        public static TeamAssessmentInfo TeamAssessmentWithTeamMember, TeamAssessmentWithoutTeamMember;
        private const string Password = SharedConstants.CommonPassword;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                //Create a Team with team member
                _team = TeamFactory.GetNormalTeam("Team_");
                _team.Name = "Team" + CSharpHelpers.Random8Number();
                _team.Members.Add(new AddMemberRequest
                {
                    FirstName = "First",
                    LastName ="Mem1",
                    Email = Constants.UserEmailPrefix + "member1" + RandomDataUtil.GetFirstName() + Constants.UserEmailDomain
                });

                var teamResponse = setup.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;


                //Create the Team assessment with One Team member
                TeamAssessmentWithTeamMember = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = "TeamMember" + CSharpHelpers.Random8Number(),
                    TeamMembers = new List<string> { _team.Members.First().FirstName + " " + _team.Members.First().LastName },
                    EndDate = DateTime.Today.AddDays(7).AddHours(12)
                };

                //Create the Team assessment with Zero Team member
                TeamAssessmentWithoutTeamMember = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = $"Test_TeamMember2{RandomDataUtil.GetAssessmentName()}"
                };

                setupUi.AddTeamAssessment(_teamId, TeamAssessmentWithTeamMember);
                setupUi.AddTeamAssessment(_teamId, TeamAssessmentWithoutTeamMember);

                setupUi.StartSharingAssessment(_teamId, TeamAssessmentWithTeamMember.AssessmentName);
                setupUi.StartSharingAssessment(_teamId, TeamAssessmentWithoutTeamMember.AssessmentName);

                //Fill the survey for team member
                var emailSearchTeamMember = new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessmentWithTeamMember.AssessmentName),
                    To = _team.Members.First().Email,
                    Labels = new List<string> { "inbox" }
                };
                setupUi.CompleteTeamMemberSurvey(emailSearchTeamMember);

                //Set the password of newly created member
                setupUi.SetupAccountPassword(_team.Members.First().Email, Password,GmailUtil.UserEmailLabel);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }

        }
        
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51053
        [TestCategory("Critical")]
        [TestCategory("DownloadPDF")]
        [TestCategory("CompanyAdmin")]
        public void TeamAssessment_Member_Access_Verification()
        {
            VerifySetup(_classInitFailed);

            var loginPage = new LoginPage(Driver, Log);
            var assessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var growthItemPopupPage = new AddGrowthItemPopupPage(Driver, Log);
            var teamDashboardPage = new TeamDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            var growthItemInfo = GrowthPlanFactory.GetValidGrowthItem();
            var growthEditedItemInfo = GrowthPlanFactory.GetValidUpdatedGrowthItem();
            var teamMember = _team.Members.First().Email;

            Log.Info($"Login as a team member {_team.Members.First().Email} and verify team {_team.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            loginPage.LoginToApplication(teamMember, Password);
            Assert.IsTrue(teamDashboardPage.DoesTeamDisplay(_team.Name),$"{_team.Name} is not displayed");

            Log.Info($"Navigate to the team assessment dashboard and verify {teamMember} accesses");
            assessmentDashboardPage.NavigateToPage(_teamId);
            Assert.IsFalse(assessmentDashboardPage.IsAddAssessmentButtonDisplayed(),"'Add Assessment' button is displayed");

            Log.Info($"Verify that {TeamAssessmentWithTeamMember} and {TeamAssessmentWithoutTeamMember} assessments should be displayed");
            Assert.IsTrue(assessmentDashboardPage.DoesAssessmentExist(TeamAssessmentWithTeamMember.AssessmentName), $"<{TeamAssessmentWithTeamMember.AssessmentName}> is not displayed on the Assessment Dashboard");
            Assert.IsTrue(assessmentDashboardPage.DoesAssessmentExist(TeamAssessmentWithoutTeamMember.AssessmentName), $"<{TeamAssessmentWithTeamMember.AssessmentName}> is not displayed on the Assessment Dashboard");

            Log.Info($"Verify that {teamMember} should not able to edit the {TeamAssessmentWithTeamMember} and {TeamAssessmentWithoutTeamMember} assessments");
            Assert.IsFalse(assessmentDashboardPage.IsAssessmentEditIconDisplayed(TeamAssessmentWithTeamMember.AssessmentName),$"{teamMember} is able to edit the assessment {TeamAssessmentWithTeamMember.AssessmentName}"); 
            Assert.IsFalse(assessmentDashboardPage.IsAssessmentEditIconDisplayed(TeamAssessmentWithoutTeamMember.AssessmentName), $"{teamMember} is able to edit the assessment {TeamAssessmentWithoutTeamMember.AssessmentName}"); 

            Log.Info($"Click on the {TeamAssessmentWithTeamMember.AssessmentName} and verify that radar title should be matched");
            assessmentDashboardPage.ClickOnRadar(TeamAssessmentWithTeamMember.AssessmentName);
            Assert.AreEqual((TeamAssessmentWithTeamMember.AssessmentName + " - " + SharedConstants.TeamAssessmentType + " Radar"), radarPage.GetRadarTitle(), "RadarTitle doesn't match");

            Log.Info("On Radar page, verify Export PDF, Excel and Questions buttons");
            Assert.IsTrue(radarPage.IsExportPdfButtonDisplayed(),"Export pdf button is not displayed");
            Assert.IsFalse(radarPage.IsExportExcelButtonDisplayed(), "Export excel button is displayed");
            Assert.IsFalse(radarPage.IsExportQuestionsButtonDisplayed(),"Export Questions button is displayed");

            Log.Info("Click on the pdf button and verify that pdf should be downloaded");
            var fileName = $"{_team.Name} {TeamAssessmentWithTeamMember.AssessmentName} {TeamAssessmentWithTeamMember.EndDate:MM-dd-yyyy}.pdf";
            radarPage.ClickExportToPdf();
            radarPage.ClickCreatePdf();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");

            Log.Info("Click on the 'Add New Item' and verify that growth item should be added");
            growthItemGridView.ClickAddNewGrowthItem();
            Assert.IsFalse(growthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar Page");
            growthItemInfo.Owner = null;
            growthItemPopupPage.EnterGrowthItemInfo(growthItemInfo);
            growthItemPopupPage.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), $"Growth item {growthItemInfo.Title} is not added");

            Log.Info($"Edit {growthItemInfo.Title} GI and verify that growth item should be edited");
            growthItemGridView.ClickGrowthItemEditButton(growthItemInfo.Title);
            Assert.IsFalse(growthItemPopupPage.IsRadarTypeEnabled(), "'Radar Type' dropdown is Enabled on Radar page");
            growthEditedItemInfo.Owner = null;
            growthEditedItemInfo.Category = "Team";
            growthItemPopupPage.EnterGrowthItemInfo(growthEditedItemInfo);
            growthItemPopupPage.ClickSaveButton();
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth Item {growthEditedItemInfo.Title} is not edited");

            /*Bug id: 35461
            Log.Info($"Copy {growthEditedItemInfo.Title} GI and verify that growth item should be copied");
            growthItemGridView.ClickCopyGrowthItemButton(growthEditedItemInfo.Title);
            Assert.AreEqual(2, growthItemGridView.GetCopiedGiCount(growthEditedItemInfo.Title), $"Growth item {growthItemInfo.Title} isn't copied");*/

            Log.Info("Click on the export button and verify that excel should be downloaded"); 
            fileName = $"GrowthPlanFor{_team.Name}Assessment{TeamAssessmentWithTeamMember.AssessmentName}.xlsx";
            growthItemGridView.ClickExportToExcel();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"{fileName} not downloaded successfully");

            Log.Info($"'Delete' {growthEditedItemInfo.Title} and verify that growth item should be deleted");
            growthItemGridView.DeleteAllGIs();
            Assert.IsFalse(growthItemGridView.IsGiPresent(growthEditedItemInfo.Title), $"Growth item {growthEditedItemInfo.Title} is not deleted");
        }
    }


}