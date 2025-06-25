using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAggregateViewerTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static User _indAssessUser;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _indAssessUser = TestEnvironment.UserConfig.GetUserByDescription("member");
            
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA");
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void IndividualAssessment_IndividualAggregateViewer()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            var teamId = dashboardPage.GetTeamIdFromLink(_team.Name);
            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid, teamId);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"AddIndAggViewer_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
            
            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment, SharedConstants.IndividualAssessmentType);

            createIndividualAssessment1.ClickNextButton();
            
            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();

            createIndividualAssessment3.InputIndividualAndAggregateEmail(_indAssessUser.Username);
            createIndividualAssessment3.ClickNextButton();

            //get info from page
            createIndividualAssessment4.WaitUntilLoaded();
            var individual = createIndividualAssessment4.GetIndividualViewers();
            //validate
            Assert.That.ListsAreEqual(new List<string> { _indAssessUser.Username }, individual, 
                "Individual Viewers does not match.");
            
            //publish
            createIndividualAssessment4.ClickPublishBottomButton();

            dashboardPage.NavigateToPage(Company.Id);
            topNav.LogOut();

            login.LoginToApplication(_indAssessUser.Username, _indAssessUser.Password);

            var teamAssessmentDashboardUrl = dashboardPage.GetTeamAssessmentDashboardUrl(_team.Name);
            teamAssessmentDashboard.NavigateToUrl(teamAssessmentDashboardUrl);

            teamAssessmentDashboard.SwitchToIndividualAssessmentView();
            individualAssessmentDashboard.WaitUntilLoaded();

            
            Log.Info($"Assert: Verify viewer {_indAssessUser.Username} able to access individual assessment");
            Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent(
                    $"{assessment.AssessmentName} - Roll up"),
                $"Individual Roll up doesn't exists with name : {assessment.AssessmentName}");
            Assert.IsTrue(individualAssessmentDashboard.IsAssessmentPresent(
                    $"{assessment.AssessmentName} - {_team.Members[0].FullName()}"),
                $"Individual Roll up doesn't exists with name : {assessment.AssessmentName}");
        }
    }
}