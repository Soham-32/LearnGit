using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentVotesCountsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static User _member;
        private static SetupTeardownApi _setup;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static int _votes = 10;
        private const int ClickForNotes = 5;
        private const int ClickForDimension = 5;
        private const string IdeaboardVotesIcon = "No Votes Left";
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        private static User Reviewer => MemberUserConfig.GetUserByDescription("ideaboard member2");

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                _member = MemberUserConfig.GetUserByDescription("ideaboard member1");

                var team = TeamFactory.GetGoiTeam("IndividualRadar");
                team.Members.Add(new AddMemberRequest
                {
                    FirstName = _member.FirstName,
                    LastName = _member.LastName,
                    Email = _member.Username
                });

                _setup = new SetupTeardownApi(TestEnvironment);
                _teamResponse = _setup.CreateTeam(team).GetAwaiter().GetResult();

                var member = _setup.GetCompanyMember(Company.Id, Reviewer.Username);
                _assessmentRequest = IndividualAssessmentFactory.GetPublishedIndividualAssessment(
                    Company.Id, User.CompanyName, _teamResponse.Uid, $"IA_{Guid.NewGuid()}");
                _assessmentRequest.Members = _teamResponse.Members.Select(a => a.ToAddIndividualMemberRequest()).ToList();
                _assessmentRequest.Members.First().Reviewers.Add(member.ToAddIndividualMemberRequest());
                _assessmentRequest.IndividualViewers.Add(member.ToAddUserRequest());

                _assessment = _setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType)
                    .GetAwaiter().GetResult();

                // Creating cards
                _notesCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid, User, true);
                _dimensionCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 50810
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Ideaboard_IndividualAssessment_VoteCounts_On_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var topNav = new TopNavigation(Driver, Log);
           
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Ideaboard as Admin, give votes on cards and verify the total vote counts");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            var notesCardText = _notesCardResponseOfIndividualAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.First().Card.ItemText.CheckForNull();

            //Set Votes Allowed from Text area and give votes on cards
            Log.Info("Set votes allowed, give votes on cards and verify");
            ideaboardPage.SetVotesAllowed(_votes.ToString());

            var votedCountsByAdmin = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();
            var numberOfVotesAllowed = ideaboardPage.GetVotesAllowed().ToInt();

            var notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            var dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, votedCountsByAdmin, "'Your votes' count does not match");
            Assert.AreEqual(_votes, numberOfVotesAllowed, "Allowed votes does not match");
            Assert.AreEqual(_votes , votesLeftCountsOnBoard, "Allowed votes as per member count does not match");

            login.NavigateToPage();
            topNav.LogOut();

            //Login With Member, give votes on cards and verify the 'Your votes', 'Votes left' and 'No votes left' tool tip
            Log.Info("Login with Member, go to Ideaboard to give votes on cards then verify with 'Your Votes' and 'Votes Left'");
            VotesByMember(_member.Username, _member.Password);

            //Login With Reviewer, give votes on cards and verify the 'Your votes', 'Votes left' and 'No votes left' tool tip 
            Log.Info("Login with Reviewer, go to Ideaboard to give votes on cards then verify with 'Your Votes' and 'Votes Left'");
            VotesByMember(Reviewer.Username, Reviewer.Password);

            //Login with Admin, Increase and Decrease allowed votes, Verify with votes in cards
            Log.Info("Login with Admin, go to Ideaboard to verify with 'Your votes' and 'Votes left' counts");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var votedCountsOnBoard = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();
            notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, votedCountsOnBoard, "Total voted count on board does not match");

            Log.Info("Set votes allowed using 'Increment' and 'Decrement' button then verify");
            ideaboardPage.SetVotesAllowed(5);

            votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();

            Assert.AreEqual(_votes += 5, notesCardVotesCounts + votesLeftCountsOnBoard, "number of votes allowed does not match");

            ideaboardPage.SetVotesAllowed(10, true);

            votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();

            Assert.AreEqual(_votes -= 10, notesCardVotesCounts + votesLeftCountsOnBoard, "number of votes allowed does not match");

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

            //Login with Member and Verify the allowed votes after decreasing it.
            Log.Info("Login with Member, go to Ideaboard to verify with 'Your votes' and 'Votes left'");
            login.NavigateToPage();
            login.LoginToApplication(_member.Username, _member.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
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

            Log.Info("Login with Member, go to Ideaboard to give votes on cards then verify with 'Your votes' and 'Votes left'");
            login.NavigateToPage();
            login.LoginToApplication(userName, passWord);
     
            selectCompanyPage.SelectCompany(User.CompanyName);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            var notesCardText = _notesCardResponseOfIndividualAssessment.First().Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.First().Card.ItemText.CheckForNull();

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesColumn, ClickForNotes, notesCardText);

            var votedCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var notesCardVoteCount = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);

            Assert.AreEqual(ClickForNotes, notesCardVoteCount, $"Vote Counts of {notesCardVoteCount} and {ClickForNotes} does not match");
            Assert.AreEqual(_votes, votedCountsByMember + votesLeftCountForMember, "number of votes allowed does not match");

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, ClickForDimension, dimensionCardText);

            votedCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var dimensionCardVoteCount = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);

            Assert.AreEqual(ClickForDimension, dimensionCardVoteCount, $"Vote Counts of {dimensionCardVoteCount} and {ClickForDimension} does not match");
            Assert.AreEqual(_votes, votedCountsByMember, "Allowed votes does not match");
            Assert.AreEqual(0, votesLeftCountForMember, "Votes left counts does not match");

            // Verify tool tip of no votes left
            Log.Info("Verify the 'No votes left' with tool tips");
            ideaboardPage.HoverOverToVoteIcon(notesColumn, notesCardText);
            var toolTipText = ideaboardPage.GetToolTipText();

            Assert.AreEqual(IdeaboardVotesIcon, toolTipText, "'No Vote Left' tooltip text does not match");

            //Remove votes from card
            Log.Info("Remove some votes on cards then verify with 'Your votes' and 'Votes left'");
            ideaboardPage.ClickOnRemoveVoteIconByDimensionAndText(dimensionColumn, dimensionCardText, 1);
            ideaboardPage.ClickOnRemoveVoteIconByDimensionAndText(notesColumn, notesCardText, 1);

            notesCardVoteCount = ideaboardPage.GetTotalVotesByUser(notesColumn, notesCardText);
            dimensionCardVoteCount = ideaboardPage.GetTotalVotesByUser(dimensionColumn, dimensionCardText);

            votedCountsByMember = ideaboardPage.GetTotalNumberOfVotesGivenByUser().ToInt();
            var expectedTotalVotesCount = notesCardVoteCount + dimensionCardVoteCount;

            Assert.AreEqual(expectedTotalVotesCount, votedCountsByMember, $"Vote Counts of {expectedTotalVotesCount} and {votedCountsByMember} does not match");

            votesLeftCountForMember = ideaboardPage.GetNumberOfVotesLeftForUser().ToInt();
            var remainingVotes = _votes - votedCountsByMember;

            Assert.AreEqual(remainingVotes, votesLeftCountForMember, "Votes remaining does not match");

            login.NavigateToPage();
            topNav.LogOut();

            //Login with Admin, verify the vote counts and votes left given by member with board
            Log.Info("Login with Admin, go to Ideaboard to verify with 'Board votes' and 'Votes left' counts");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesColumn, notesCardText).ToInt();
            var dimensionCardVotesCounts = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionColumn, dimensionCardText).ToInt();
            var votesLeftCountsOnBoard = ideaboardPage.GetNumberOfVotesLeftOfBoard().ToInt();
            var totalBoardVotes = ideaboardPage.GetTotalNumberOfVotesOnBoard().ToInt();

            Assert.AreEqual(notesCardVotesCounts + dimensionCardVotesCounts, totalBoardVotes, "'Board Votes' count does not match");
            if (userName==_member.Username)
            {
                Assert.AreEqual(_votes - totalBoardVotes, votesLeftCountsOnBoard, "'Votes Left' count does not match");
            }
            
            login.NavigateToPage();
            topNav.LogOut();
        }
    }
}