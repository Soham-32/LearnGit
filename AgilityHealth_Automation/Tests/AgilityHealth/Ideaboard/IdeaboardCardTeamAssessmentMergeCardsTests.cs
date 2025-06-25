    using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.SetUpTearDown;
    using AtCommon.Dtos;
    using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardTeamAssessmentMergeCardsTests : BaseTest
    {
        private static bool _classInitFailed;
        private static Guid _assessmentUid;
        private static TeamHierarchyResponse _team;
        private static readonly TeamAssessmentInfo TeamAssessmentInfo = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = "Test" + CSharpHelpers.RandomNumber(),
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };

        private static SetupTeardownApi _setup;
        private static RadarDetailResponse _radar;
        private static List<CreateCardResponse> _dimensionCardResponseOfTeamAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment1;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");
        private const int ClickForDimension = 2;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var user = User;
                if (user.IsMember())
                {
                    user = CompanyAdminUser;
                }

                _setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                var client = ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

                // Create Team Assessment
                _team = _setup.GetCompanyHierarchy(Company.Id,user).GetTeamByName(SharedConstants.Team);
                _radar = new SetupTeardownApi(TestEnvironment).GetRadarDetails(Company.Id, SharedConstants.TeamAssessmentType);
                setupUi.AddTeamAssessment(_team.TeamId, TeamAssessmentInfo,user);
               
                //Getting assessment details
                var teamResponse = client.GetAsync<IList<TeamProfileResponse>>(
                    RequestUris.Teams().AddQueryParameter("companyId", Company.Id).AddQueryParameter("teamName", SharedConstants.Team)).GetAwaiter().GetResult();
                var teamProfile = teamResponse.Dto.FirstOrDefault(t => t.Name == SharedConstants.Team).CheckForNull();

                var teamAssessmentsResponse = client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(teamProfile.Uid));
                _assessmentUid = teamAssessmentsResponse.Result.Dto.FirstOrDefault(x => x.AssessmentName.Equals(TeamAssessmentInfo.AssessmentName))!.Uid;

                //Creating Cards
                _dimensionCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar.Name, _assessmentUid, false, true, user);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_TeamAssessment_MergeCards_InSame_Column()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            _notesCardResponseOfTeamAssessment = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar.Name, _assessmentUid, true);
            _notesCardResponseOfTeamAssessment1 = _setup.CreateIdeaboardCardForTeamAssessment(Company.Id, _radar.Name, _assessmentUid, true);

            var notesCardText = _notesCardResponseOfTeamAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var notesCardText1 = _notesCardResponseOfTeamAssessment1.FirstOrDefault()?.Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            Log.Info("Give vote on both cards");
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn,ClickForDimension,notesCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn,ClickForDimension,notesCardText1);

            //Merge card in the same notes column
            Log.Info($"Merge {notesCardText} card with {notesCardText1} card in same {SharedConstants.DimensionNotes} column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionNotes, notesCardText, 0, 95);

            ideaboardPage.ClickOnCancelButtonOfMergePopup();
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card does not exist on ideaboard");

            Log.Info($"Merge {notesCardText} card with {notesCardText1} card in same {SharedConstants.DimensionNotes} column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionNotes, notesCardText, 0, 95);
            ideaboardPage.ClickOnMergeButtonOfMergePopup();

            const int expectedVoteCounts = ClickForDimension + ClickForDimension;
            var cardText = ideaboardPage.GetTextFromCardByDimension(notesColumn,notesCardText1);
            var actualVoteCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();

            var combinedText = notesCardText1 + notesCardText;
            Assert.AreEqual(combinedText, cardText, "Card did not merged");
            Assert.AreEqual(expectedVoteCounts,actualVoteCounts,"Vote counts does not match");

        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_TeamAssessment_MergeCards_InDifferent_Column()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            var clarityCardText = _dimensionCardResponseOfTeamAssessment[3].Card.ItemText.CheckForNull();
            var performanceCardText = _dimensionCardResponseOfTeamAssessment[4].Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var clarityColumn = _dimensionCardResponseOfTeamAssessment[3].Card.ColumnName.CheckForNull();
            var performanceColumn = _dimensionCardResponseOfTeamAssessment[4].Card.ColumnName.CheckForNull();
            
            //Merge card with Dimension column card
            Log.Info($"Merge {clarityCardText} card with {performanceCardText} card from 'Clarity' column to 'Performance' column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionClarity, SharedConstants.DimensionPerformance, clarityCardText, 250, 90);

            ideaboardPage.ClickOnCancelButtonOfMergePopup();
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(clarityCardText, clarityColumn), $"{clarityCardText} Card does not exist on ideaboard");

            ideaboardPage.DragAndDropCard(SharedConstants.DimensionClarity, SharedConstants.DimensionPerformance, clarityCardText, 250, 90);

            ideaboardPage.ClickOnMergeButtonOfMergePopup();

            var bothCardsCombinedText = performanceCardText + clarityCardText;
            var mergedCardText = ideaboardPage.GetTextFromCardByDimension(performanceColumn);

            Assert.AreEqual(bothCardsCombinedText, mergedCardText, "Card did not merged");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_TeamAssessment_MergeCards_With_Gi()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            var leadershipCardText = _dimensionCardResponseOfTeamAssessment[0].Card.ItemText.CheckForNull();
            var cultureCardText = _dimensionCardResponseOfTeamAssessment[1].Card.ItemText.CheckForNull();
            const string expectedAlertText =
                "Cards cannot be merged together when there is a Growth Item tied to an Idea board card.";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var leadershipCardColumn = _dimensionCardResponseOfTeamAssessment[0].Card.ColumnName.CheckForNull();
           
            //Select GI and merge card
            ideaboardPage.ClickOnGrowthItemIconByDimension(leadershipCardColumn, leadershipCardText, GrowthItemsType.TeamGrowthItem);

            Log.Info($"Merge {leadershipCardText} card with {cultureCardText} card from '{SharedConstants.DimensionLeadership}' column to '{SharedConstants.DimensionCulture}' column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionLeadership, SharedConstants.DimensionCulture, leadershipCardText, 250, 90);

            var actualAlertText = ideaboardPage.GetTextFromCannotMergePopup();
            Assert.AreEqual(expectedAlertText, actualAlertText, "Alert message does not match");

            ideaboardPage.ClickOnOkayButtonOfCannotMergePopup();
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(leadershipCardText, leadershipCardColumn), $"{leadershipCardText} does not exist");
        }
    }
}