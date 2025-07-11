﻿using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Ideaboard;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Ideaboard.IndividualAssessment
{
    [TestClass]
    [TestCategory("Ideaboard")]
    public class IdeaboardCardIndividualAssessmentMergeGiCardsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static SetupTeardownApi _setup;
        private static IndividualAssessmentResponse _assessment;
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
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

                _setup = new SetupTeardownApi(TestEnvironment);

                // Create Individual Assessment
                _teamResponse = GetTeamForIndividualAssessment(_setup, "GOI", 1, user);
                var individualDataResponse = GetIndividualAssessment(_setup, _teamResponse, "IdeaboardCards_", user);
                _assessmentRequest = individualDataResponse.Item2;

                _assessment = individualDataResponse.Item3;
                _dimensionCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid);
                _notesCardResponseOfIndividualAssessment = _setup.CreateIdeaboardCardForIndividualAssessment(_assessmentRequest, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user, true);

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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_MergeCards_With_Gi()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var notesCardText = _notesCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            const string expectedAlertText = "Cards cannot be merged together when there is a Growth Item tied to an Idea board card.";

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.FirstOrDefault()!.AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            //Select GI and merge card
            ideaboardPage.ClickOnGrowthItemIconByDimension(dimensionColumn, dimensionCardText, GrowthItemsType.IndividualGrowthItem);

            Log.Info($"Merge {dimensionCardText} card with {notesCardText} card from '{SharedConstants.DimensionValueDelivered}' column to '{SharedConstants.DimensionNotes}' column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionValueDelivered, SharedConstants.DimensionNotes, dimensionCardText, 800, 85);

            var actualAlertText = ideaboardPage.GetTextFromCannotMergePopup();
            Assert.AreEqual(expectedAlertText, actualAlertText, "Alert message does not match");

            ideaboardPage.ClickOnOkayButtonOfCannotMergePopup();
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension(dimensionCardText, dimensionColumn), $"{dimensionCardText} does not exist on ideaboard");
        }
    }
}