using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard;
using AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment;
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
    public class IdeaboardCardIndividualAssessmentToolTipsTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");

        private const string IdeaboardDeleteCardIcon = "Delete Card";
        private const string IdeaboardAddAVoteIcon = "Add a Vote";
        private const string IdeaboardCreateIndividualIcon = "Create Individual Growth Item";
        private const string IdeaboardCreateManagementIcon = "Create Management Growth Item";

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
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public void Ideaboard_IndividualAssessment_ToolTips()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var notesCardText = _notesCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();

            Log.Info("Go to Ideaboard and verify all ToolTips texts for 'Notes' column");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var notesCard = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.HoverOverToDeleteIcon(notesCard, notesCardText);
            var toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardDeleteCardIcon, toolTipText, "Delete tooltip text does not match");

            ideaboardPage.HoverOverToVoteIcon(notesCard, notesCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardAddAVoteIcon, toolTipText, "Vote tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(notesCard, notesCardText, GrowthItemsType.IndividualGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateIndividualIcon, toolTipText, "Create individual GI tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(notesCard, notesCardText, GrowthItemsType.ManagementGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateManagementIcon, toolTipText, "Create management GI tooltip text does not match");


            Log.Info("Verify all ToolTips texts for dimension column");
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.HoverOverToDeleteIcon(dimensionColumn, dimensionCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardDeleteCardIcon, toolTipText, "Delete tooltip text does not match");

            ideaboardPage.HoverOverToVoteIcon(dimensionColumn, dimensionCardText);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardAddAVoteIcon, toolTipText, "Vote tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(dimensionColumn, dimensionCardText, GrowthItemsType.IndividualGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateIndividualIcon, toolTipText, "Create individual GI tooltip text does not match");

            ideaboardPage.HoverOverToGrowthItemIcon(dimensionColumn, dimensionCardText, GrowthItemsType.ManagementGrowthItem);
            toolTipText = ideaboardPage.GetToolTipText();
            Assert.AreEqual(IdeaboardCreateManagementIcon, toolTipText, "Create management GI tooltip text does not match");
        }
    }
}