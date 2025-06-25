using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentDeleteCardTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
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
                var assessmentResponse = individualDataResponse.Item2;
                _assessment = individualDataResponse.Item3;

                _notesCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user, true);
                _dimensionCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_Delete_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var notesCardText = _notesCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Ideaboard and delete both cards, One normal card from 'Dimension' column & one with GI from 'Notes' column");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.ClickOnGrowthItemIconByDimension(notesColumn, notesCardText, GrowthItemsType.IndividualGrowthItem);
            var growthItemInfo = new GrowthItem
            {
                Category = "Individual",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Status = "Not Started",
                CompetencyTargets = new List<string> { "Org Influence", "Clarity" },
                Priority = "Low",
            };
            ideaboardPage.EnterGrowthItemInfo(growthItemInfo);
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();

            Log.Info($"Delete {dimensionCardText} Card with GI from '{dimensionColumn}' column");
            ideaboardPage.ClickOnDeleteIconByDimension(dimensionColumn, dimensionCardText);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card does still exist on 'Dimension' column");

            Log.Info($"Delete {notesCardText} Card with GI from '{notesColumn}' column");
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card with 'Individual Growth Item' does not exist on ideaboard");

            ideaboardPage.ClickOnDeleteButtonOfDeletePopup(notesColumn, notesCardText, GrowthItemsType.IndividualGrowthItem);
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card does still exist with Gi on 'Notes' column");

            Driver.RefreshPage();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} Card is still exist on 'Dimension' column");
            Assert.IsFalse(ideaboardPage.DoesCardExistByTextAndDimension(notesCardText, notesColumn), $"{notesCardText} Card does still exist with Gi on 'Notes' column");
        }
    }
}