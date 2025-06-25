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
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentResetVotesTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static User _member;
        private static IndividualAssessmentResponse _assessment;
        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");
        public static int NotesCardVotes = 2;
        public static int DimensionCardVotes = 3;

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

                var setup = new SetupTeardownApi(TestEnvironment);
                var teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();

                var individualDataResponse = GetIndividualAssessment(setup, teamResponse, "IdeaboardCards_");
                var assessmentRequest = individualDataResponse.Item2;

                _assessment = individualDataResponse.Item3;

                _notesCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid, User, true);
                _dimensionCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
                setup.SetIdeaboardVotesAllowed(Company.Id, _assessment.AssessmentList[0].AssessmentUid);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 46511
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Ideaboard_IndividualAssessment_ResetVotes_From_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var selectCompanyPage = new SelectCompanyPage(Driver);
            var topNav = new TopNavigation(Driver, Log);

            Log.Info("Go to Ideaboard and verify all given votes should be 0 after clicking on 'Reset Votes' button");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesCardText = _notesCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();

            var notesCardColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            var dimensionCardColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            Log.Info("Give votes on cards by Admin and verify the same on member's login");
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesCardColumn, NotesCardVotes, notesCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionCardColumn, DimensionCardVotes, dimensionCardText);

            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(_member.Username, _member.Password);

            selectCompanyPage.SelectCompany(User.CompanyName);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Assert.IsFalse(ideaboardPage.DoesResetVotesButtonExist(), "Reset button does exist");

            var adminVotesOnNotesCard = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesCardColumn, notesCardText).ToInt();
            var adminVotesOnDimensionCard = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionCardColumn, dimensionCardText).ToInt();

            Assert.AreEqual(NotesCardVotes, adminVotesOnNotesCard, "Votes are not showing on note column");
            Assert.AreEqual(DimensionCardVotes, adminVotesOnDimensionCard, "Votes are not showing on dimension column");

            Log.Info("Give votes on cards by member");
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(notesCardColumn, NotesCardVotes, notesCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionCardColumn, DimensionCardVotes, dimensionCardText);

            var memberVotesOnNotesCard = ideaboardPage.GetTotalVotesByUser(notesCardColumn, notesCardText);
            var memberVotesOnDimensionCard = ideaboardPage.GetTotalVotesByUser(dimensionCardColumn, dimensionCardText);

            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Log.Info("Verify total votes of admin and member");
            var totalGivenVotesOnNotesCard = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesCardColumn, notesCardText).ToInt();
            var totalGivenVotesOnDimensionCard = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionCardColumn, dimensionCardText).ToInt();

            Assert.AreEqual(adminVotesOnNotesCard + memberVotesOnNotesCard, totalGivenVotesOnNotesCard, "Votes on Notes column does not match");
            Assert.AreEqual(adminVotesOnDimensionCard + memberVotesOnDimensionCard, totalGivenVotesOnDimensionCard, "Votes on Notes column does not match");

            Log.Info("Click on reset votes and verify the vote counts");
            ideaboardPage.ClickOnResetVotesButton();
            ideaboardPage.ClickOnCancelButtonOfResetPopup();

            Assert.IsTrue(totalGivenVotesOnNotesCard != 0, "Votes are reset");
            Assert.IsTrue(totalGivenVotesOnDimensionCard != 0, "Votes are reset");

            ideaboardPage.ClickOnResetVotesButton();
            ideaboardPage.ClickOnResetButtonOfResetPopup();

            var votesOnNotesCardAfterReset = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(notesCardColumn, notesCardText).ToInt();
            var votesOnDimensionCardAfterReset = ideaboardPage.GetNumberOfVotesByCardTextAndDimension(dimensionCardColumn, dimensionCardText).ToInt();

            Assert.AreNotEqual(totalGivenVotesOnNotesCard, votesOnNotesCardAfterReset, "Votes are equal");
            Assert.AreNotEqual(totalGivenVotesOnDimensionCard, votesOnDimensionCardAfterReset, "Votes are equal");
            Assert.IsTrue(votesOnNotesCardAfterReset == 0, "Votes did not reset");
            Assert.IsTrue(votesOnDimensionCardAfterReset == 0, "Votes did not reset");
        }
    }
}