using System;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Comments
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCommentAddTests1 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }
        
        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Comments_Add_Single()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var comment1 = BusinessOutcomesFactory.GetCommentRequest(User.FullName);
            addBusinessOutcomePage.ClickOnTab("Comments");
            addBusinessOutcomePage.CommentsTab.AddComment(comment1.Content);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);
            addBusinessOutcomePage.ClickOnTab("Comments");
            var actualComments = addBusinessOutcomePage.CommentsTab.GetComments();

            Assert.AreEqual(1, actualComments.Count);
            // ReSharper disable once PossibleInvalidOperationException
            Assert.That.TimeIsClose(comment1.CommentDate.Value, actualComments.First().CommentDate);
            Assert.AreEqual(comment1.Commenter, actualComments.First().Commenter.IndexOf("\r", StringComparison.Ordinal) >= 0 ? actualComments.First().Commenter.Substring(0, actualComments.First().Commenter.IndexOf("\r", StringComparison.Ordinal)).Trim() : actualComments.First().Commenter.Trim());
            Assert.AreEqual(comment1.Content, actualComments.First().Content);
        }
    }
}