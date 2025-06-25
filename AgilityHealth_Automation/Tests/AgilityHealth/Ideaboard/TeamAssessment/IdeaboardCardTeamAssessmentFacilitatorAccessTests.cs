using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.TeamAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardTeamAssessmentFacilitatorAccessTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static int _teamId;
        private static AddTeamWithMemberRequest _team;
        private static RadarDetailResponse _radar;
        private static TeamAssessmentInfo _teamAssessmentWithFacilitator;
        private const string GiIconsColor = "rgba(0, 128, 0, 1)";

        private static User FacilitatorUser => TestEnvironment.UserConfig.GetUserByDescription("team admin");

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetUpMethods(testContext, TestEnvironment);
                var setupApiMethods = new SetupTeardownApi(TestEnvironment);

                _team = TeamFactory.GetNormalTeam("Team",1);
                _teamResponse = setupApiMethods.CreateTeam(_team).GetAwaiter().GetResult();
                _teamId = setupApiMethods.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _radar = setupApiMethods.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.TeamSurveyId);

                _teamAssessmentWithFacilitator = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
                    Facilitator = FacilitatorUser.FirstName + " " + FacilitatorUser.LastName,
                    FacilitationDate = DateTime.Today.AddDays(1),
                    StartDate = DateTime.Today,
                    TeamMembers = _teamResponse.Members.Select(a=>a.FullName()).ToList()
                };
                
                setup.AddTeamAssessment(_teamId, _teamAssessmentWithFacilitator);
            }
            
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Ideaboard_TeamAssessment_Facilitator_Access()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewWidget(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            const int votes = 2;
            const int updatedVotes = 3;
            var card1Text = RandomDataUtil.GetTeamDepartment().ToString();
            var card2Text = RandomDataUtil.GetTeamDepartment().ToString();
            var updatedCardText = "Update_" + RandomDataUtil.GetTeamDepartment();
           
            login.NavigateToPage();
            login.LoginToApplication(FacilitatorUser.Username, FacilitatorUser.Password);

            Log.Info("Go to Ideaboard");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentWithFacilitator.AssessmentName);
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitUntilIdeaboardLoaded();
            var dimensionColumns = _radar.Dimensions.Where(d => d.Name != "Finish").Select(dimension => dimension.Name).ToList().CheckForNull();

            //Create a Card
            Log.Info($"Create Card to '{dimensionColumns.First()}' column with votes");
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.First());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.First(), card1Text);
            ideaboardPage.ClickOnVoteUpIconByDimension(dimensionColumns.First(), votes);
            var textFromCardByDimension = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.First());
            var voteCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            Assert.AreEqual(card1Text, textFromCardByDimension, "Card1 text does not match");
            Assert.AreEqual(votes, voteCountsByMember,"Vote count does not match");

            Log.Info($"Create Card to '{dimensionColumns.LastOrDefault().CheckForNull()}' column with growth item");
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.LastOrDefault().CheckForNull(), card2Text);
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumns.LastOrDefault().CheckForNull(), card2Text, GrowthItemsType.TeamGrowthItem);
            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Effective Facilitation", "Technical  Expertise" },
                Priority = "Low",
            };
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);
            var growthItemIconColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.TeamGrowthItem);
            Assert.AreEqual(GiIconsColor, growthItemIconColor, "Growth item icon color does not match");

            Log.Info("Verify that GI successfully added on radar page and navigate back to Idea board page");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentWithFacilitator.AssessmentName);
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(card2Text, GrowthItemGridViewWidget.GrowthItemType.TeamGi), $"'Team Growth Item' does not present on radar page with '{card2Text}' description");
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            //Edit a Card
            Log.Info($"Edit '{textFromCardByDimension}' Card from '{dimensionColumns.First()}' column & add votes");
            ideaboardPage.UpdateTextInCardByDimension(dimensionColumns.First(), updatedCardText);
            ideaboardPage.ClickOnVoteUpIconByDimension(dimensionColumns.First(), updatedVotes);
            var updatedTextFromCardByDimension = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.First());
            var updatedVoteCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            Assert.AreEqual(updatedCardText, updatedTextFromCardByDimension, "Updated Card text does not match");
            Assert.AreEqual(votes+updatedVotes, updatedVoteCountsByMember, "Vote count does not match");

            Log.Info($"Edit Growth item for '{card2Text}' Card from '{dimensionColumns.LastOrDefault().CheckForNull()}' column");
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumns.LastOrDefault().CheckForNull(), card2Text, GrowthItemsType.OrganizationalGrowthItem);
            var updatedGrowthItemIconColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.OrganizationalGrowthItem);
            Assert.AreEqual(GiIconsColor, updatedGrowthItemIconColor, "Growth item icon color does not match");

            Log.Info("Verify that GI successfully updated on radar page and navigate back to Idea board page");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentWithFacilitator.AssessmentName);
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(card2Text, GrowthItemGridViewWidget.GrowthItemType.OrganizationalGi), $"'Organizational Growth Item' does not present on radar page with {card2Text} description");
            radarPage.ClickOnIdeaboardLink();
            Driver.SwitchToLastWindow();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            //Delete a Card
            Log.Info($"Delete '{updatedTextFromCardByDimension}' Card from '{dimensionColumns.First()}' column and delete votes");
            ideaboardPage.ClickOnRemoveVoteIconByDimensionAndText(dimensionColumns.First(), updatedCardText, votes+updatedVotes);
            var deletedVoteCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            Assert.AreEqual(0, deletedVoteCountsByMember, "Vote count does not match");

            ideaboardPage.ClickOnDeleteIconByDimension(dimensionColumns.First(), updatedTextFromCardByDimension);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(updatedTextFromCardByDimension, dimensionColumns.First()), $"'{updatedTextFromCardByDimension}' Card does still exist on 'Dimension' column");

            Log.Info($"Delete '{card2Text}' Card from '{dimensionColumns.LastOrDefault().CheckForNull()}' column");
            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(dimensionColumns.LastOrDefault().CheckForNull(), card2Text, GrowthItemsType.OrganizationalGrowthItem);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(card2Text, dimensionColumns.LastOrDefault().CheckForNull()), $"'{card2Text}' Card does still exist on 'Dimension' column");

            Log.Info("Verify that GI successfully deleted on radar page");
            teamAssessmentDashboard.NavigateToPage(_teamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessmentWithFacilitator.AssessmentName);
            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(card2Text, GrowthItemGridViewWidget.GrowthItemType.OrganizationalGi), $"'Organizational Growth Item' does present on radar page with {card2Text} description");
        } 
    }
}
