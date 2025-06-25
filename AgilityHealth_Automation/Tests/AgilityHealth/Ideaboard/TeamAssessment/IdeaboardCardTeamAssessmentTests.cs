using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.TeamAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardTeamAssessmentTests : BaseTest
    {
        private static bool _classInitFailed;
        private static Guid _assessmentUid;
        private static User _user;
        private static TeamHierarchyResponse _team;
        private static readonly TeamAssessmentInfo TeamAssessmentInfo = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(), 
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };

        private static SetupTeardownApi _setup;
        private static RadarDetailResponse _radar;
        private static List<CreateCardResponse> _dimensionCardResponseOfTeamAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment1;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment2;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");

        private readonly string UpdatedCardText = "Update_" + RandomDataUtil.GetTeamDepartment();
        private const string DisabledGiIconsColor = "rgba(0, 0, 0, 0.87)";
        private const string IdeaboardDeleteCardIcon = "Delete Card";
        private const string IdeaboardAddAVoteIcon = "Add a Vote";
        private const string IdeaboardCreateTeamIcon = "Create Team Growth Item";
        private const string IdeaboardCreateOrganizationalIcon = "Create Organizational Growth Item";


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _user = User;
                if (_user.IsMember())
                {
                    _user = CompanyAdminUser;
                }

                _setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                var client = ClientFactory.GetAuthenticatedClient(_user.Username, _user.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

                // Create Team Assessment
                _team = _setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                _radar = new SetupTeardownApi(TestEnvironment).GetRadarDetailsBySurveyId(Company.Id, SharedConstants.TeamSurveyId);
                setupUi.AddTeamAssessment(_team.TeamId, TeamAssessmentInfo);

                var teamProfile = _setup.GetTeamWithTeamMemberResponse(_team.TeamId, _user);
                var teamAssessmentsResponse = client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(teamProfile.First().Uid));
                _assessmentUid = teamAssessmentsResponse.Result.Dto.First(x => x.AssessmentName.Equals(TeamAssessmentInfo.AssessmentName)).Uid;
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Smoke")]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_Create_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var cardText = RandomDataUtil.GetTeamDepartment().ToString();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard, create cards for all dimensions and verify cards count");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var dimensionColumns = _radar.Dimensions.Where(d => d.Name != "Finish").Select(dimension => dimension.Name).ToList().CheckForNull();
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.LastOrDefault().CheckForNull(), cardText);

            var textFromCardByDimension = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            Assert.AreEqual(textFromCardByDimension, cardText, "Card text does not match");

            dimensionColumns.Add("Notes");
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.LastOrDefault().CheckForNull(), cardText);

            var textFromCardByNotes = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.Last().CheckForNull());
            Assert.AreEqual(textFromCardByNotes, cardText, "Card text does not match");
            Assert.IsTrue(ideaboardPage.GetTotalNumberOfCards() >= 2, "Cards counts does not match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_Update_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            //Creating a card
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard, update created cards with new text and verify the text after refreshing the page");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(dimensionColumn, UpdatedCardText, dimensionCardText);

            Driver.RefreshPage();

            var updatedDimensionText = ideaboardPage.GetTextFromCardByDimension(dimensionColumn);
            Assert.AreEqual(UpdatedCardText, updatedDimensionText, $"{UpdatedCardText} on card is not updated for 'Dimension' column");
            Assert.AreNotEqual(dimensionCardText, updatedDimensionText, $"{dimensionCardText} is still present on 'Dimension' column");

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_GrowthItems_On_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var growthItemGridViewPage = new GrowthItemGridViewWidget(Driver, Log);

            // Creating cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true);
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard and Create GI both cards, Select 'TeamGI' on card of 'Notes' column & 'Organizational GI' on 'Dimension' column and then Verify with Growth Plan in radar page");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.TeamGrowthItem);
            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Effective Facilitation", "Technical Lead - Leadership" },
                Priority = "Low",
            };
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);

            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.OrganizationalGrowthItem);

            growthItemInfo = new GrowthItem
            {
                Category = "Organizational",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Effective Facilitation", "Technical Lead - Leadership" },
                Priority = "Low",
            };
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);
            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentInfo.AssessmentName);

            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.TeamGi), $"'Team Growth Item' is not present on radar page with {notesCardText} description");
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.OrganizationalGi), $"'Organizational Growth Item' is not present on radar page with {dimensionCardText} description");

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Switch GI on both cards, Select 'Organizational GI' on card of 'Notes' column & 'Team GI' on 'Dimension' column and then Verify with Growth Plan in radar page");
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.OrganizationalGrowthItem);
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentInfo.AssessmentName);

            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.TeamGi), $"'Team Growth Item' is not present on radar page with {dimensionCardText} description");
            Assert.IsTrue(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.OrganizationalGi), $"'Organizational Growth Item' is not present on radar page with {notesCardText} description");

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            ideaboardPage.ClickOnCancelButtonOfDeletePopup(notesColumn, notesCardText, GrowthItemsType.OrganizationalGrowthItem);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card with 'Organizational Growth Item' does not exist on ideaboard");

            Log.Info($"Delete {notesCardText} Card with 'Organizational GI' and Verify with Growth Plan in radar page");
            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(notesColumn, notesCardText, GrowthItemsType.OrganizationalGrowthItem);

            ideaboardPage.ClickOnCancelButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card with 'Team Growth Item' does not exist on ideaboard");

            Log.Info($"Delete {dimensionCardText} Card with 'Team GI' and Verify with Growth Plan in radar page");
            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessmentInfo.AssessmentName);
            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(notesCardText, GrowthItemGridViewWidget.GrowthItemType.OrganizationalGi), $"'Organizational Growth Item' is present on radar page with {notesCardText} description");
            Assert.IsFalse(growthItemGridViewPage.IsGiPresentWithDescriptionAndCategory(dimensionCardText, GrowthItemGridViewWidget.GrowthItemType.TeamGi), $"'Team Growth Item' is present on radar page with {dimensionCardText} description");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_DisabledIcons_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            // Creating Cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true);
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard and remove text from card of 'Notes' column, then verify the disabled 'VoteUp' and 'GI' icons");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            //Notes disabled card
            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(notesColumn, "", notesCardText);
            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(dimensionColumn, "", dimensionCardText);

            Driver.RefreshPage();

            ideaboardPage.ClickOnVoteUpIconByDimension(notesColumn, 1);
            var noOfVotes = ideaboardPage.GetNumberOfVotesByDimension(notesColumn).ToInt();
            Assert.IsTrue(noOfVotes == 0, "Vote up icon is enabled and it should not be");

            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, "", GrowthItemsType.TeamGrowthItem);
            var growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.TeamGrowthItem);
            Assert.IsTrue(growthItemColor == DisabledGiIconsColor, "Team Growth item is selected");

            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, "", GrowthItemsType.OrganizationalGrowthItem);
            growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.OrganizationalGrowthItem);
            Assert.IsTrue(growthItemColor == DisabledGiIconsColor, "Organizational Growth item is selected");

            //Dimension disabled card
            Log.Info("Remove text from card of dimension column, then verify the disabled 'VoteUp' and 'GI' icons");

            ideaboardPage.ClickOnVoteUpIconByDimension(dimensionColumn, 1);
            noOfVotes = ideaboardPage.GetNumberOfVotesByDimension(dimensionColumn).ToInt();
            Assert.IsTrue(noOfVotes == 0, "Vote up icon is enabled and it should not be");

            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, "", GrowthItemsType.TeamGrowthItem);
            growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.TeamGrowthItem);
            Assert.IsTrue(growthItemColor == DisabledGiIconsColor, "Team Growth item is selected");

            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, "", GrowthItemsType.OrganizationalGrowthItem);
            growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.OrganizationalGrowthItem);
            Assert.IsTrue(growthItemColor == DisabledGiIconsColor, "Organizational Growth item is selected");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_ToolTips()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            // Creating Cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true);
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);

            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard and verify all ToolTips texts for Dimension column");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            ideaboardPage.HoverOverToDeleteIcon(dimensionColumn, dimensionCardText);
            var toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardDeleteCardIcon, toolTipText, "'Delete' tooltip text does not match");

            ideaboardPage.HoverOverToVoteIcon(dimensionColumn, dimensionCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardAddAVoteIcon, toolTipText, "'Vote' tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateTeamIcon, toolTipText, "'Create team GI' tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(dimensionColumn, dimensionCardText, GrowthItemsType.OrganizationalGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateOrganizationalIcon, toolTipText, "'Create organizational GI' tooltip text does not match");

            Log.Info("Go to Ideaboard and verify all ToolTips texts for 'Notes' column");
            ideaboardPage.HoverOverToDeleteIcon(notesColumn, notesCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardDeleteCardIcon, toolTipText, "'Delete' tooltip text does not match");

            ideaboardPage.HoverOverToVoteIcon(notesColumn, notesCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardAddAVoteIcon, toolTipText, "'Vote' tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(notesColumn, notesCardText, GrowthItemsType.TeamGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateTeamIcon, toolTipText, "'Create team GI' tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(notesColumn, notesCardText, GrowthItemsType.OrganizationalGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateOrganizationalIcon, toolTipText, "'Create organizational GI' tooltip text does not match");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_DragAndDrop_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            //Creating Cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true, false, _user);
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to ideaboard page and drag and drop card in same 'Notes' column and other columns ");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Log.Info($"drag and drop {notesCardText} card in same {SharedConstants.DimensionNotes} column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionNotes, notesCardText, 0, 140);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension($"{notesCardText}", SharedConstants.DimensionNotes), $"{notesCardText} card didn't move in same column");

            Log.Info($"drag {notesCardText} card from '{SharedConstants.DimensionNotes}' column and drop to '{SharedConstants.DimensionCulture}' column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionCulture, notesCardText, -670, 20);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension($"{notesCardText}", SharedConstants.DimensionCulture), $"{notesCardText} Card didn't move in {SharedConstants.DimensionCulture} column");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_Delete_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            //Creating Cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true);
            _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid);
            _setup.CreateIdeaboardGrowthPlanItem(_dimensionCardResponseOfTeamAssessment[0], SharedConstants.IdeaboardTeamGpi);

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);

            Log.Info("Go to Ideaboard and delete a card, One normal card from 'Notes' column & one with GI from 'Dimension' column");
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            Log.Info($"Delete {notesCardText} Card with GI from '{notesColumn}' column");
            ideaboardPage.ClickOnDeleteIconByDimension(notesColumn, notesCardText);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card does still exist on 'Notes' column");

            Log.Info($"Delete {dimensionCardText} Card with GI from '{dimensionColumn}' column");
            ideaboardPage.ClickOnCancelButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card with 'Team Growth Item' does not exist on ideaboard");

            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(dimensionColumn, dimensionCardText, GrowthItemsType.TeamGrowthItem);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card does still exist with Gi on 'Dimension' column");

            Driver.RefreshPage();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card does still exist on 'Notes' column");
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card does still exist with Gi on 'Dimension' column");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_TeamAssessment_SortCards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            //Creating Cards
            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true, false, _user);
            _notesCardResponseOfTeamAssessment1 = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true, false, _user);
            _notesCardResponseOfTeamAssessment2 = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar, _assessmentUid, true, false, _user);
            _setup.SetIdeaboardVotesAllowed(Company.Id, _assessmentUid, _user);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            //card sorting
            Log.Info("Go to Ideaboard and give votes on 'Notes' column cards, then click on refresh button and verify if cards are sorted");

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var notesCardText1 = _notesCardResponseOfTeamAssessment1.First().Card.ItemText.CheckForNull();
            var notesCardText2 = _notesCardResponseOfTeamAssessment2.First().Card.ItemText.CheckForNull();

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, 0, notesCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, 1, notesCardText1);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, 3, notesCardText2);

            var beforeSortNotesCardVoteList = ideaboardPage.GetVotesFromCardsByDimension(notesColumn);
            var expectedVoteList = beforeSortNotesCardVoteList.OrderByDescending(n => n).Select(n => n.ToString()).ToList();

            ideaboardPage.ClickOnSortVotesButton();

            var actualVoteList = ideaboardPage.GetVotesFromCardsByDimension(notesColumn);

            CollectionAssert.AreEqual(expectedVoteList, actualVoteList, "Card votes does not sort");
        }
    }
}