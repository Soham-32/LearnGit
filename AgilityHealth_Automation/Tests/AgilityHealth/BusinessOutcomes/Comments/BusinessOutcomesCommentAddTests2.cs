using System;
using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.V2;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Comments
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCommentAddTests2 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;
        private static User User4 => TestEnvironment.UserConfig.GetUserByDescription("user 4");

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }
        
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 46579
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Comments_Add_Multiple()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var v2Header = new HeaderFooterPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var comment1 = BusinessOutcomesFactory.GetCommentRequest(User.FullName);
            addBusinessOutcomePage.ClickOnTab("Comments");
            addBusinessOutcomePage.CommentsTab.AddComment(comment1.Content);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            v2Header.SignOut();

            login.LoginToApplication(User4.Username, User4.Password);
            
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var comment2 = BusinessOutcomesFactory.GetCommentRequest(User4.FullName);
            comment2.Avatar = User4.FirstName.Substring(0, 1) + User4.LastName.Substring(0, 1);
            addBusinessOutcomePage.ClickOnTab("Comments");
            addBusinessOutcomePage.CommentsTab.AddComment(comment2.Content);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            Driver.RefreshPage();
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);
            addBusinessOutcomePage.ClickOnTab("Comments");
            var expectedComments = new List<CommentRequest> {comment2, comment1};
            var actualComments = addBusinessOutcomePage.CommentsTab.GetComments();

            Assert.AreEqual(expectedComments.Count, actualComments.Count);
            Assert.AreEqual(comment2.Avatar, actualComments[0].Avatar, "Comment2 avatar should be user's initials");
            Assert.IsTrue(actualComments[1].Avatar.EndsWith(".png"), "Comment1 avatar should end in .png");
            
            for (var i = 0; i < expectedComments.Count; i++)
            {
                var commentDate = expectedComments[i].CommentDate;
                // ReSharper disable once PossibleInvalidOperationException
                Assert.That.TimeIsClose(commentDate.Value, actualComments[i].CommentDate);
                Assert.AreEqual(expectedComments[i].Commenter, actualComments[i].Commenter.IndexOf("\r", StringComparison.Ordinal) >= 0 ? actualComments[i].Commenter.Substring(0, actualComments[i].Commenter.IndexOf("\r", StringComparison.Ordinal)).Trim() : actualComments[i].Commenter.Trim());
                Assert.AreEqual(expectedComments[i].Content, actualComments[i].Content);
            }
            
        }
    }
}