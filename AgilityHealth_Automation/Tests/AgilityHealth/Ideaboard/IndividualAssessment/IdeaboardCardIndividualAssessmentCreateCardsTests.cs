using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentCreateCardsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static RadarDetailResponse _radarResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");

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

                var setup = new SetupTeardownApi(TestEnvironment);

                // Create Individual Assessment
                _teamResponse = GetTeamForIndividualAssessment(setup, "GOI", 1, user);
                var individualDataResponse = GetIndividualAssessment(setup, _teamResponse, "IdeaboardCards_", user);

                _assessment = individualDataResponse.Item3;
                _radarResponse = individualDataResponse.Item1;

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("Smoke")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_Create_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var cardText = RandomDataUtil.GetTeamDepartment().ToString();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Ideaboard, create cards on all columns and verify total created cards");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var dimensionColumns = _radarResponse.Dimensions.Where(d => d.Name != "Finish").Select(dimension => dimension.Name).ToList();
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.LastOrDefault().CheckForNull(), cardText);

            var textFromCardByDimension = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            Assert.AreEqual(textFromCardByDimension, cardText, "Card text does not match");

            dimensionColumns.Add("Notes");
            ideaboardPage.ClickOnAddCardButtonByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            ideaboardPage.InputTextInCardByDimension(dimensionColumns.LastOrDefault().CheckForNull(), cardText);

            var textFromCardByNotes = ideaboardPage.GetTextFromCardByDimension(dimensionColumns.LastOrDefault().CheckForNull());
            Assert.AreEqual(textFromCardByNotes, cardText, "Card text does not match");
            Assert.IsTrue(ideaboardPage.GetTotalNumberOfCards() == 2, "Cards counts does not match");
        }
    }
}