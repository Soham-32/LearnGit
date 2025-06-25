using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentDisabledIconsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");
        private const string Color = "rgba(0, 0, 0, 0.87)";

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
                var assessmentResponse = individualDataResponse.Item2;

                _assessment = individualDataResponse.Item3;
                _dimensionCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_DisabledIcons_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            //Disabled icons for dimension column
            Log.Info("Go to Ideaboard and remove text from dimensionColumn cards, then verify the disabled 'VoteUp' and 'GI' icons");
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(dimensionColumn, "", dimensionCardText);
            Driver.RefreshPage();

            ideaboardPage.ClickOnVoteUpIconByDimension(dimensionColumn, 1);
            var noOfVotes = ideaboardPage.GetNumberOfVotesByDimension(dimensionColumn).ToInt();
            Assert.IsTrue(noOfVotes == 0, "Vote up icon is enabled and it should not be");

            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, "", GrowthItemsType.IndividualGrowthItem);
            var growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.IndividualGrowthItem);
            Assert.IsTrue(growthItemColor == Color, "Individual Growth item is enabled");

            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, "", GrowthItemsType.ManagementGrowthItem);
            growthItemColor = ideaboardPage.ClickOnGrowthItemIconAndGetColor(GrowthItemsType.ManagementGrowthItem);
            Assert.IsTrue(growthItemColor == Color, "Management Growth item is enabled");
        }
    }
}