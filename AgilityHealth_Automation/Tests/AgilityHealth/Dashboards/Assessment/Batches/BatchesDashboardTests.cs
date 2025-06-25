using System;
using System.Collections.Generic;
using System.Globalization;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.Batches
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class BatchesDashboardTests : BaseTest
    {
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BatchesDashboard_ScheduleTeamBatchAssessment()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            batchesTabPage.ClickPlusButton();

            var batchAssessment = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment
                    }
                },
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(30)
            };
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);

            createBatchAssessmentPopupPage.ClickScheduleButton();

            Assert.IsTrue(createBatchAssessmentPopupPage.IsScheduleConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessment.AssessmentType, createBatchAssessmentPopupPage.GetSchedulePopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(1, createBatchAssessmentPopupPage.GetSchedulePopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");
            Assert.AreEqual(batchAssessment.StartDate.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                createBatchAssessmentPopupPage.GetPopupStartDate(), "Start Date should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickScheduleYesProceedButton();

            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.SearchBatchName(batchAssessment.BatchName);

            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BatchesDashboard_TeamBatchAssessment_SendToTeamMembers()
        {
            Log.Info("Test : Verify that the team batch assessment can be added successfully with emails sent only to the Team Members");

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

            batchesTabPage.ClickPlusButton();

            var batchAssessment = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                StartDate = DateTime.Today,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.Team,
                        TeamMembers = new List<string>
                        {
                            Constants.TeamMemberEmail1,
                            Constants.TeamMemberEmail2,
                            Constants.TeamMemberEmail3,
                            Constants.TeamMemberEmail4,
                            Constants.TeamMemberEmail5
                        }
                    },
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment,
                        TeamMembers = new List<string>
                        {
                            Constants.TeamMemberEmail1,
                            Constants.TeamMemberEmail2,
                            Constants.TeamMemberEmail3,
                            Constants.TeamMemberEmail4,
                            Constants.TeamMemberEmail5
                        }
                    }
                }
            };
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);
            batchAssessment.EndDate = DateTime.Today.AddDays(7);

            createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();

            Log.Info("Verify that A prompt will appear to confirm");
            Assert.IsTrue(createBatchAssessmentPopupPage.IsSendConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessment.AssessmentType, createBatchAssessmentPopupPage.GetSendPopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(batchAssessment.TeamAssessments.Count, createBatchAssessmentPopupPage.GetSendPopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickSendYesProceedButton();

            Log.Info("Verify that A prompt will appear to validate that the batch was sent successfully");
            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.SearchBatchName(batchAssessment.BatchName);

            Log.Info("Verify that created batch is showing correctly in grid");
            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");

            Log.Info("Verify Emails were sent out correctly");
            foreach (var teamAssessment in batchAssessment.TeamAssessments)
            {
                var expectedSubject = SharedConstants.TeamAssessmentSubject(teamAssessment.TeamName, batchAssessment.AssessmentName);
                foreach (var teamMember in teamAssessment.TeamMembers)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember),
                        $"Unable to find email with subject <{expectedSubject}> sent to {teamMember}");
                }
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug id : 48906 
        [TestCategory("CompanyAdmin")]
        public void BatchesDashboard_TeamBatchAssessment_SendToStakeholders()
        {
            Log.Info("Test : Verify that the team batch assessment can be added successfully with emails sent only to the Stakeholders");

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);


            batchesTabPage.ClickPlusButton();

            var batchAssessment = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                StartDate = DateTime.Today,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.Team,
                        TeamMembers = new List<string>
                        {
                            Constants.StakeholderEmail2,
                            Constants.StakeholderEmail3
                        }
                    },
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment,
                        TeamMembers = new List<string>
                        {
                            Constants.StakeholderEmail2,
                            Constants.StakeholderEmail3
                        }
                    }
                }
            };
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);
            createBatchAssessmentPopupPage.SendTo("Stakeholders");
            batchAssessment.StartDate = DateTime.Today;
            batchAssessment.EndDate = DateTime.Today.AddDays(7);

            createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();

            Log.Info("Verify that A prompt will appear to confirm");
            Assert.IsTrue(createBatchAssessmentPopupPage.IsSendConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessment.AssessmentType, createBatchAssessmentPopupPage.GetSendPopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(batchAssessment.TeamAssessments.Count, createBatchAssessmentPopupPage.GetSendPopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickSendYesProceedButton();

            Log.Info("Verify that A prompt will appear to validate that the batch was sent successfully");
            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.SearchBatchName(batchAssessment.BatchName);

            Log.Info("Verify that created batch is showing correctly in grid");
            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");

            Log.Info("Verify Emails were sent out correctly");
            foreach (var teamAssessment in batchAssessment.TeamAssessments)
            {
                var expectedSubject = SharedConstants.TeamAssessmentSubject(teamAssessment.TeamName, batchAssessment.AssessmentName);
                foreach (var teamMember in teamAssessment.TeamMembers)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember),
                        $"Unable to find email with subject <{expectedSubject}> sent to {teamMember}");
                }
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin")]
        public void BatchesDashboard_TeamBatchAssessment_SendToTeamMembersAndStakeholders()
        {
            Log.Info("Test : Verify that the team batch assessment can be added successfully with emails sent to the Team Members and Stakeholders");

            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var batchesTabPage = new BatchesTabPage(Driver, Log);
            var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            dashBoardPage.ClickAssessmentDashBoard();

            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);


            batchesTabPage.ClickPlusButton();

            var batchAssessment = new BatchAssessment
            {
                BatchName = "batch_" + RandomDataUtil.GetAssessmentName(),
                AssessmentName = RandomDataUtil.GetAssessmentName(),
                AssessmentType = SharedConstants.TeamAssessmentType,
                StartDate = DateTime.Today,
                TeamAssessments = new List<TeamAssessmentInfo>
                {
                    new TeamAssessmentInfo
                    {
                        TeamName = SharedConstants.Team,
                        TeamMembers = new List<string>
                        {
                            Constants.TeamMemberEmail1,
                            Constants.TeamMemberEmail2,
                            Constants.TeamMemberEmail3,
                            Constants.TeamMemberEmail4,
                            Constants.TeamMemberEmail5
                        },
                        StakeHolders = new List<string>
                        {
                            Constants.StakeholderEmail2,
                            Constants.StakeholderEmail3
                        }
                    },
                    new TeamAssessmentInfo
                    {
                        TeamName = Constants.TeamForBatchAssessment,
                        TeamMembers = new List<string>
                        {
                            Constants.TeamMemberEmail1,
                            Constants.TeamMemberEmail2,
                            Constants.TeamMemberEmail3,
                            Constants.TeamMemberEmail4,
                            Constants.TeamMemberEmail5
                        },
                        StakeHolders = new List<string>
                        {
                            Constants.StakeholderEmail2,
                            Constants.StakeholderEmail3
                        }
                    }
                }
            };
            createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);
            createBatchAssessmentPopupPage.SendTo("Team Members and Stakeholders");
            batchAssessment.EndDate = DateTime.Today.AddDays(7);

            createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();

            Log.Info("Verify that A prompt will appear to confirm");
            Assert.IsTrue(createBatchAssessmentPopupPage.IsSendConfirmationPopupDisplayed(),
                "Confirmation popup should display on popup");
            Assert.AreEqual(batchAssessment.AssessmentType, createBatchAssessmentPopupPage.GetSendPopupSurveyType(),
                "Assessment Type should be shown on popup");
            Assert.AreEqual(batchAssessment.TeamAssessments.Count, createBatchAssessmentPopupPage.GetSendPopupSelectedTeam(),
                "Total selected team should be shown correctly on popup");

            createBatchAssessmentPopupPage.ClickSendYesProceedButton();

            Log.Info("Verify that A prompt will appear to validate that the batch was sent successfully");
            Assert.AreEqual("Batch process has successfully been completed.",
                createBatchAssessmentPopupPage.GetSuccessPopupText(), "Success popup should be shown correctly");

            createBatchAssessmentPopupPage.ClickOkButton();

            batchesTabPage.SearchBatchName(batchAssessment.BatchName);

            Log.Info("Verify that created batch is showing correctly in grid");
            var actualBatchAssessment = batchesTabPage.GetBatchItemFromGrid(1);
            Assert.AreEqual(batchAssessment.BatchName, actualBatchAssessment.BatchName, "Batch name should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentName, actualBatchAssessment.AssessmentName, "Assessment name should be shown correctly");
            Assert.AreEqual(batchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), actualBatchAssessment.StartDate.ToString("M/d/yyyy", CultureInfo.InvariantCulture), "Start Date should be shown correctly");
            Assert.AreEqual(batchAssessment.AssessmentType, actualBatchAssessment.AssessmentType, "Assessment type should be shown correctly");

            Log.Info("Verify Emails were sent out correctly");
            foreach (var teamAssessment in batchAssessment.TeamAssessments)
            {
                var expectedSubject = SharedConstants.TeamAssessmentSubject(teamAssessment.TeamName, batchAssessment.AssessmentName);
                foreach (var teamMember in teamAssessment.TeamMembers)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, teamMember),
                        $"Unable to find email with subject <{expectedSubject}> sent to {teamMember}");
                }
                foreach (var stakeholder in teamAssessment.StakeHolders)
                {
                    Assert.IsTrue(GmailUtil.DoesMemberEmailExist(expectedSubject, stakeholder),
                        $"Unable to find email with subject <{expectedSubject}> sent to {stakeholder}");
                }
            }
        }
    }
}
