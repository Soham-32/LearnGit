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
    public class IdeaboardCardIndividualAssessmentSortCardsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static User _member;
        private static IndividualAssessmentResponse _assessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment1;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment2;
        private static readonly UserConfig MemberUserConfig = new UserConfig("M");

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

                //Create Cards
                _dimensionCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
                _dimensionCardResponseOfIndividualAssessment1 = setup.CreateIdeaboardCardForIndividualAssessment(assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
                _dimensionCardResponseOfIndividualAssessment2 = setup.CreateIdeaboardCardForIndividualAssessment(assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
                setup.SetIdeaboardVotesAllowed(Company.Id, _assessment.AssessmentList.First().AssessmentUid);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void Ideaboard_IndividualAssessment_SortCards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var selectCompanyPage = new SelectCompanyPage(Driver);
            var topNav = new TopNavigation(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Log.Info("Go to Ideaboard and give votes on dimensionColumn cards by Admin");

            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.First().Card.ColumnName;

            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionCardText1 = _dimensionCardResponseOfIndividualAssessment1.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionCardText2 = _dimensionCardResponseOfIndividualAssessment2.FirstOrDefault()?.Card.ItemText.CheckForNull();

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 0, dimensionCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 1, dimensionCardText1);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 3, dimensionCardText2);

            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(_member.Username, _member.Password);

            selectCompanyPage.SelectCompany(User.CompanyName);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Log.Info("Go to Ideaboard and give votes on dimensionColumn cards by Member then click on 'Sort Votes'");

            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 0, dimensionCardText);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 1, dimensionCardText1);
            ideaboardPage.ClickOnVoteUpIconByDimensionAndText(dimensionColumn, 3, dimensionCardText2);

            var beforeSortDimensionCardVoteList = ideaboardPage.GetVotesFromCardsByDimension(dimensionColumn);
            var expectedVoteList = beforeSortDimensionCardVoteList.OrderByDescending(n => n).Select(n => n.ToString()).ToList();

            ideaboardPage.ClickOnSortVotesButton();

            Log.Info("Verify Card order after clicking on 'Sort Votes' from Member and Admin");
            var actualVoteListByMember = ideaboardPage.GetVotesFromCardsByDimension(dimensionColumn);

            login.NavigateToPage();
            topNav.LogOut();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var actualVoteListByAdmin = ideaboardPage.GetVotesFromCardsByDimension(dimensionColumn);

            CollectionAssert.AreEqual(expectedVoteList, actualVoteListByMember, "Card votes does not sort");
            CollectionAssert.AreEqual(expectedVoteList, actualVoteListByAdmin, "Card votes does not sort");
        }
    }
}