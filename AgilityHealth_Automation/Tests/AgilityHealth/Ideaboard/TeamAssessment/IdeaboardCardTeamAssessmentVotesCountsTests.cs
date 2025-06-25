using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.TeamAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardTeamAssessmentVotesCountsTests : BaseTest
    {
        private static int _votes = 10;
        private static Guid _assessmentUid;
        private static bool _classInitFailed;
        private const int ClickForNotes = 5;
        private const int ClickForDimension = 5;
        private const string IdeaboardVotesIcon = "No Votes Left";
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        private static User Member1 => MemberUserConfig.GetUserByDescription("ideaboard member1");
        private static User Member2 => MemberUserConfig.GetUserByDescription("ideaboard member2");
        private static List<CreateCardResponse> _dimensionCardResponseOfTeamAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfTeamAssessment;
        private static readonly TeamAssessmentInfo TeamAssessmentInfo = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(), 
            TeamMembers = new List<string>{ Member1.FirstName + " " + Member1.LastName , Member2.FirstName + " " + Member2.LastName }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);
                var client = ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();

                // Create Team Assessment
                var team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
                var teamResponse = setup.GetTeamWithTeamMemberResponse(team.TeamId);

                var radar = setup.GetRadarDetailsBySurveyId(Company.Id, SharedConstants.TeamSurveyId);
                setupUi.AddTeamAssessment(team.TeamId, TeamAssessmentInfo);

                //Getting assessment details
                var teamAssessmentsResponse = client.GetAsync<IList<AssessmentSummaryResponse>>(RequestUris.TeamAssessments(teamResponse.First().Uid));
                _assessmentUid = teamAssessmentsResponse.Result.Dto.First(x => x.AssessmentName.Equals(TeamAssessmentInfo.AssessmentName)).Uid;

                // Creating cards
                _notesCardResponseOfTeamAssessment = setup.CreateIdeaboardCardForTeamAssessment(Company.Id, radar, _assessmentUid, true);
                _dimensionCardResponseOfTeamAssessment = setup.CreateIdeaboardCardForTeamAssessment(Company.Id, radar, _assessmentUid);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_TeamAssessment_VoteCounts_On_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var numberOfMembers = TeamAssessmentInfo.TeamMembers.Count;

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Ideaboard, give votes on cards and verify the total vote counts");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            //Set Votes Allowed from Text area and give votes on cards
            Log.Info("Set votes allowed, give votes on cards and verify");
            ideaboardPage.SetVotesAllowed(_votes.ToString());

            var votedCountsByAdmin = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();
            var numberOfVotesAllowed = ideaboardPage.GetVotesAllowed().ToInt();

            var notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            var dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, votedCountsByAdmin, "Total votes count does not match");
            Assert.AreEqual(_votes, numberOfVotesAllowed, "Allowed votes does not match");
            Assert.AreEqual(_votes * numberOfMembers, votesLeftCountsOnBoard, "number of votes allowed does not match");

            login.NavigateToPage();
            topNav.LogOut();

            //Login With Member1, give votes on cards and verify the 'Your votes', 'Votes left' and 'No votes left' tool tip
            Log.Info("Login with Member1, go to Ideaboard to give votes on cards then verify with total votes and votes left");
            VotesByMember(Member1.Username, Member1.Password);

            //Login With Member2, give votes on cards and verify the 'Your votes', 'Votes left' and 'No votes left' tool tip
            Log.Info("Login with Member2, go to Ideaboard to give votes on cards then verify with total votes and votes left");
            VotesByMember(Member2.Username, Member2.Password);

            //Login with Admin, Increase and Decrease allowed votes, Verify with votes in cards
            Log.Info("Login with Admin, go to Ideaboard to verify with total votes and votes left counts");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var votedCountsOnBoard = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();
            notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, votedCountsOnBoard, "Total voted count on board does not match");

            Log.Info("Set votes allowed using 'Increment' and 'Decrement' button then verify");
            ideaboardPage.SetVotesAllowed(5);

            votedCountsOnBoard = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();
            votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();
            var actualTotalVotesCount = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();

            Assert.AreEqual((_votes += 5) * numberOfMembers, votedCountsOnBoard + votesLeftCountsOnBoard - actualTotalVotesCount, "number of votes allowed does not match");

            ideaboardPage.SetVotesAllowed(10, true);

            actualTotalVotesCount = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            votedCountsOnBoard = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();
            votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();

            Assert.AreEqual((_votes -= 10) * numberOfMembers, votedCountsOnBoard + votesLeftCountsOnBoard - actualTotalVotesCount, "number of votes allowed does not match");

            //Give votes more then allowed
            Log.Info("Verify that user is able to give more then allowed votes on cards");
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, ClickForNotes, notesCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, ClickForDimension, dimensionCardText);

            var notesCardVoteCountByUser = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);
            var dimensionCardVoteCountByUser = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);
            votedCountsByAdmin = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();

            Assert.IsTrue(notesCardVoteCountByUser + dimensionCardVoteCountByUser > _votes, "Given votes are less then allowed votes");
            Assert.AreEqual(notesCardVoteCountByUser + dimensionCardVoteCountByUser, votedCountsByAdmin, "Total votes count does not match");

            // Verify tool tip of Add a vote
            Log.Info("Verify the 'Add a vote' with tool tips");
            ideaboardPage.HoverOverToVoteIcon(notesColumn, notesCardText);
            var toolTipText = ideaboardPage.GetToolTipText();

            Assert.AreEqual("Add a Vote", toolTipText, "'Add a Vote' tooltip text does not match");

            login.NavigateToPage();
            topNav.LogOut();
            //M1
            Log.Info("Login with Member1, go to Ideaboard to verify with 'Your votes' and 'Votes left'");
            login.NavigateToPage();
            login.LoginToApplication(Member1.Username, Member1.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            notesCardVotesCounts = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);
            dimensionCardVotesCounts = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);
            var votedCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, votedCountsByMember, "'Voted Counts' by member does not match");
            Assert.AreEqual(_votes - votedCountsByMember, votesLeftCountForMember, "'Votes left' count for member does not match");

        }

        public void VotesByMember(string userName, string passWord)
        {
            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
            var selectCompanyPage = new SelectCompanyPage(Driver);
            var numberOfMembers = TeamAssessmentInfo.TeamMembers.Count;

            Log.Info("Login with Member1, go to Ideaboard to give votes on cards then verify with total votes and votes left");
            login.NavigateToPage();
            login.LoginToApplication(userName, passWord);

            selectCompanyPage.SelectCompany(User.CompanyName);
            
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfTeamAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            var notesCardText = _notesCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfTeamAssessment.First().Card.ItemText.CheckForNull();

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, ClickForNotes, notesCardText);

            var actualTotalVotesCount = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var notesCardVoteCount = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);

            Assert.AreEqual(ClickForNotes, notesCardVoteCount, $"Vote Counts of {notesCardVoteCount} and {ClickForNotes} does not match");
            Assert.AreEqual(_votes, actualTotalVotesCount + votesLeftCountForMember, "number of votes allowed does not match");

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, ClickForDimension, dimensionCardText);

            actualTotalVotesCount = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var dimensionCardVoteCount = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);

            Assert.AreEqual(ClickForDimension, dimensionCardVoteCount, $"Vote Counts of {dimensionCardVoteCount} and {ClickForDimension} does not match");
            Assert.AreEqual(_votes, actualTotalVotesCount, "Allowed votes does not match");
            Assert.AreEqual(0, votesLeftCountForMember, "Votes left counts does not match");

            // Verify tool tip of no votes left
            Log.Info("Verify the total vote counts with tool tips");
            ideaboardPage.HoverOverToVoteIcon(notesColumn, notesCardText);
            var toolTipText = ideaboardPage.GetToolTipText();

            Assert.AreEqual(IdeaboardVotesIcon, toolTipText, "'No Vote Left' tooltip text does not match");

            //Remove votes from card
            Log.Info("Remove some votes on cards then verify with 'Your votes' and 'Votes left'");

            ideaboardPage.ClickOnRemoveVoteIconByDimensionAndText(dimensionColumn, dimensionCardText, 1);
            ideaboardPage.ClickOnRemoveVoteIconByDimensionAndText(notesColumn, notesCardText, 1);

            notesCardVoteCount = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);
            dimensionCardVoteCount = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);

            actualTotalVotesCount = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var expectedTotalVotesCount = notesCardVoteCount + dimensionCardVoteCount;

            Assert.AreEqual(expectedTotalVotesCount, actualTotalVotesCount, $"Vote Counts of {expectedTotalVotesCount} and {actualTotalVotesCount} does not match");

            votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var remainingVotes = _votes - actualTotalVotesCount;

            Assert.AreEqual(remainingVotes, votesLeftCountForMember, "Votes remaining does not match");

            login.NavigateToPage();
            topNav.LogOut();

            //Login with Admin, verify the vote counts and votes left given by member with board
            Log.Info("Login with Admin, go to Ideaboard to verify with total votes and votes left counts");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            var dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();
            var votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();
            var totalBoardVotes = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, totalBoardVotes, "'Board Votes' count does not match");
            Assert.AreEqual(_votes * numberOfMembers - totalBoardVotes, votesLeftCountsOnBoard, "'Votes Left' count does not match");

            login.NavigateToPage();
            topNav.LogOut();
        }
    }
}