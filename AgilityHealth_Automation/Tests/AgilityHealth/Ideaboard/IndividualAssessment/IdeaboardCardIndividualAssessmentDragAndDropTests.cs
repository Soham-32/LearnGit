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
    public class IdeaboardCardIndividualAssessmentDragAndDropTests : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static IndividualAssessmentResponse _assessment;
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

                var setup = new SetupTeardownApi(TestEnvironment);

                // Create Individual Assessment
                _teamResponse = GetTeamForIndividualAssessment(setup, "GOI", 1, user);
                var individualDataResponse = GetIndividualAssessment(setup, _teamResponse, "IdeaboardCards_", user);
                var assessmentResponse = individualDataResponse.Item2;

                _assessment = individualDataResponse.Item3;

                _notesCardResponseOfIndividualAssessment = setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user, true);
                setup.CreateIdeaboardCardForIndividualAssessment(assessmentResponse, Company.Id, _assessment.AssessmentList.First().AssessmentUid, user);
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
        public void Ideaboard_IndividualAssessment_DragAndDrop_Cards()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var ideaboardPage = new IdeaboardPage(Driver, Log);
            var notesCardText = _notesCardResponseOfIndividualAssessment.LastOrDefault()?.Card.ItemText.CheckForNull();

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to ideaboard page and drag and drop card in the same 'Notes' column and other columns");
            ideaboardPage.NavigateToIdeaboardPage(Company.Id, _assessment.AssessmentList.First().AssessmentUid);
            ideaboardPage.WaitUntilIdeaboardLoaded();

            Log.Info($"drag and drop {notesCardText} card in same {SharedConstants.DimensionNotes} column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionNotes, notesCardText, 0, 140);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension($"{notesCardText}", SharedConstants.DimensionNotes), $"{notesCardText} card didn't move in same column");

            Log.Info($"drag {notesCardText} card from '{SharedConstants.DimensionNotes}' column and drop to '{SharedConstants.DimensionFoundation}' column");
            ideaboardPage.DragAndDropCard(SharedConstants.DimensionNotes, SharedConstants.DimensionFoundation, notesCardText, -330, 170);
            Assert.IsTrue(ideaboardPage.DoesCardExistByTextAndDimension($"{notesCardText}", SharedConstants.DimensionFoundation), $"{notesCardText} Card didn't move in {SharedConstants.DimensionFoundation} column");
        }
    }
}