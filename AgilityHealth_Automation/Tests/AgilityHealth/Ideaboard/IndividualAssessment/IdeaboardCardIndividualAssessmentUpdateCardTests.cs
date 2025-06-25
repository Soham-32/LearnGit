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
    public class IdeaboardCardIndividualAssessmentUpdateCardTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("M");
        private static User CompanyAdminUser => CompanyAdminUserConfig.GetUserByDescription("user 3");

        private static List<CreateCardResponse> _notesCardResponseOfIndividualAssessment;
        private static List<CreateCardResponse> _dimensionCardResponseOfIndividualAssessment;
        private readonly string UpdatedText = "Update_" + RandomDataUtil.GetTeamDepartment();

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

                _notesCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(
                    assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user, true);
                _dimensionCardResponseOfIndividualAssessment =
                    setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id,
                        _assessment.AssessmentList.First().AssessmentUid, user);
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
        public void Ideaboard_IndividualAssessment_Update_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to Ideaboard, update created cards with new text and verify the text after refreshing the page");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var dimensionCardText = _dimensionCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var dimensionColumn = _dimensionCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(dimensionColumn, UpdatedText, dimensionCardText);

            var notesCardText = _notesCardResponseOfIndividualAssessment.FirstOrDefault()?.Card.ItemText.CheckForNull();
            var notesColumn = _notesCardResponseOfIndividualAssessment.Select(d => d.Card.ColumnName).FirstOrDefault().CheckForNull();
            ideaboardPage.UpdateTextInCardByDimension(notesColumn, UpdatedText, notesCardText);

            Driver.RefreshPage();
            ideaboardPage.WaitUntilIdeaboardLoaded();

            var updatedDimensionText = ideaboardPage.GetTextFromCardByDimension(dimensionColumn);
            var updatedNotesText = ideaboardPage.GetTextFromCardByDimension(notesColumn);

            Assert.AreEqual(UpdatedText, updatedDimensionText, $"{UpdatedText} on card does not updated for 'Dimension' column");
            Assert.AreEqual(UpdatedText, updatedNotesText, $"{UpdatedText} on card does not updated for 'Notes' column");
            Assert.AreNotEqual(dimensionCardText, updatedDimensionText, $"{dimensionCardText} is still present on 'Dimension' column");
            Assert.AreNotEqual(notesCardText, updatedNotesText, $"{notesCardText} is still present on 'Notes' column");
        }
    }
}