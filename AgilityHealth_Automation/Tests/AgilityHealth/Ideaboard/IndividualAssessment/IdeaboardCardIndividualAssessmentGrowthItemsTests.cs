using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentGrowthItemsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static SetupTeardownApi _setup;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");

        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;


        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var user = User;
                if (user.IsMember())
                {
                    user = CompanyAdminUser;
                }

                _setup = new SetupTeardownApi(TestEnvironment);

                // Create Individual Assessment
                _teamResponse = GetTeamForIndividualAssessment(_setup, "GOI", 1, user);
                var individualDataResponse = GetIndividualAssessment(_setup, _teamResponse, "IdeaboardCards_", user);
                _assessmentRequest = individualDataResponse.Item2;

                _assessment = individualDataResponse.Item3;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_GrowthItems_On_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewWidget(Driver, Log);

            _notesCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid, User, true);
            _dimensionCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);

            var notesCardText = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ItemText).FirstOrDefault().CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ItemText).FirstOrDefault().CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_teamResponse.Name).ToInt();
            individualAssessmentDashboard.NavigateToPage(teamId);
            individualAssessmentDashboard.ClickOnRadar($"{_assessment.AssessmentName} - {_assessment.Participants.First().FirstName} {_assessment.Participants.First().LastName}");

            Log.Info("Go to Ideaboard and Create Gi both cards, Select 'Management GI' on card of 'Notes' column & 'Individual GI' on 'Dimension' column and then Verify with Growth Plan in radar page");
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.ManagementGrowthItem);
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.IndividualGrowthItem);

            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();

            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.ManagementGi), $"'Management Growth Item' is not present on radar page with {notesCardText} description");
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.IndividualGi), $"'Individual Growth Item' is not present on radar page with {dimensionCardText} description");

            Driver.SwitchToLastWindow();

            Log.Info("Switch GI on both cards, Select 'Individual GI' on card of 'Notes' column & 'Management GI' on 'Dimension' column and then Verify with Growth Plan in radar page");
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.IndividualGrowthItem);
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.ManagementGrowthItem);

            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();

            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.IndividualGi), $"'Individual Growth Item' is not present on radar page with {notesCardText} description");
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.ManagementGi), $"'Management Growth Item' is not present on radar page with {dimensionCardText} description");

            Driver.SwitchToLastWindow();

            ideaboardPage.ClickOnCancelButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.ManagementGrowthItem);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card with 'Management Growth Item' does not exist on ideaboard");

            Log.Info($"Delete {dimensionCardText} Card with 'Management GI' and Verify with Growth Plan in radar page");
            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.ManagementGrowthItem);
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();
            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.ManagementGi), $"'Management Growth Item' is present on radar page with {notesCardText} description");
        }

        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_Delete_GrowthItems_From_Radar()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var individualAssessmentDashboard = new IndividualAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var dashboardPage = new TeamDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewWidget(Driver, Log);

            _notesCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid, User, true);
            _dimensionCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);

            var notesCardText = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ItemText).FirstOrDefault().CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ItemText).FirstOrDefault().CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            var teamId = dashboardPage.GetTeamIdFromLink(_teamResponse.Name).ToInt();
            individualAssessmentDashboard.NavigateToPage(teamId);
            individualAssessmentDashboard.ClickOnRadar($"{_assessment.AssessmentName} - {_assessment.Participants.First().FirstName} {_assessment.Participants.First().LastName}");

            Log.Info("Go to Ideaboard and Create Gi both cards, Delete growth item from radar page and then Verify growth item icon on cards");
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var growthItemInfo = new GrowthItem
            {
                Category = "Management",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Org Influence", "Clarity" },
                Priority = "Low",
            };

            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.ManagementGrowthItem);
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);

            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.IndividualGrowthItem);
            growthItemInfo = new GrowthItem
            {
                Category = "Individual",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Org Influence", "Clarity" },
                Priority = "Low",
            };
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);
            Driver.SwitchToFirstWindow();
            Driver.RefreshPage();

            growthItemGridViewPage.DeleteAllGIs();

            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.ManagementGi), $"'Management Growth Item' is present on radar page with {notesCardText} description");
            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.IndividualGi), $"'Individual Growth Item' is present on radar page with {dimensionCardText} description");

            Driver.SwitchToLastWindow();
            Driver.RefreshPage();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            const string color = "rgba(0, 0, 0, 0.87)";

            var growthItemColor = ideaboardPage.GetGrowthItemIconColor(GrowthItemsType.IndividualGrowthItem);
            Assert.IsTrue(growthItemColor == color, "Individual Growth item icon is enabled");

            growthItemColor = ideaboardPage.GetGrowthItemIconColor(GrowthItemsType.ManagementGrowthItem);
            Assert.IsTrue(growthItemColor == color, "Management Growth item icon is enabled");

        }
    }
}