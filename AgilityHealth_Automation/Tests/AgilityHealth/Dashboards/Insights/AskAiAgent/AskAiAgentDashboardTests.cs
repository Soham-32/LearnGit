using AtCommon.Utilities;
using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Insights.AskAiAgent;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Insights.AskAiAgent
{
    [TestClass]
    [TestCategory("Insights"), TestCategory("AskAiAgentDashboard"), TestCategory("Dashboard")]
    public class AskAiAgentDashboardTests : BaseTest
    {

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAiAgent_DashboardLoadedSuccessfully()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and Navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on the AskAi Agent tab and verify that the user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            string username = askAiAgentPage.GetUsernameDisplayedWithWelcomeMessage();
            Assert.AreEqual(InsightsUser.FirstName, username, "Ask AI user is not same as the logged-in user");
            Assert.IsTrue(askAiAgentPage.IsSendButtonDisabled(), "The send button is not disabled.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAI_DownloadPDF()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info($"Login and Navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on the AskAi Agent tab and verify that the user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            var Question = "How many total teams are there?";
            askAiAgentPage.EnterQuestionInTextarea(Question);
            askAiAgentPage.ClickOnSendButton();

            Log.Info("Verify that after clicking on 'Export As PDF' button, the chat is downloaded as PDf successfully");
            const string fileName = "natural_language_query_chat.pdf";
            FileUtil.DeleteFilesInDownloadFolder(fileName);
            askAiAgentPage.ClickOnExportPdfButton();
            Assert.IsTrue(FileUtil.IsFileDownloaded(fileName), $"<{fileName}> file not downloaded successfully");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAi_ExpandCollapse_ChatWindow()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on Ask AI Agent tab and Verify that user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            askAiAgentPage.ClickOnExpandWindowIcon();
            Assert.IsFalse(askAiAgentPage.IsPromptSectionDisplayed(), "The Response Window is not expanded successfully");

            Log.Info("Click on right arrow icon and Verify that the response window minimized sucessfully");
            askAiAgentPage.ClickOnCollapseWindowIcon();
            Assert.IsTrue(askAiAgentPage.IsPromptSectionDisplayed(), "The Response Window is not collapsed successfully");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAi_ClearChat()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on Ask AI Agent tab and Verify that user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            string username = askAiAgentPage.GetUsernameDisplayedWithWelcomeMessage();
            Assert.AreEqual(InsightsUser.FirstName, username, "The Ask Ai Agent dashboard is not loaded successfully");

            Log.Info("Enter question and click on 'Send' button");
            askAiAgentPage.EnterQuestionInTextarea("How many total teams are there?");
            askAiAgentPage.ClickOnSendButton();

            Log.Info("Click on 'Clear Chat' button and verify that the chat is cleared successfully");
            askAiAgentPage.ClickOnClearChatButton();
            Assert.IsTrue(askAiAgentPage.IsDefaultQuestionsTitleDisplayed(), "The Chat is not cleared successfully");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAi_SuggestedQuestions()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on Ask AI Agent tab and Verify that user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            string username = askAiAgentPage.GetUsernameDisplayedWithWelcomeMessage();
            Assert.AreEqual(InsightsUser.FirstName, username, "The Ask Ai Agent dashboard is not loaded successfully");

            Log.Info("Enter question and click on 'Send' button");
            var prompt = "How many total teams are there?";
            askAiAgentPage.EnterQuestionInTextarea(prompt);
            askAiAgentPage.ClickOnSendButton();

            Log.Info("Fetch all suggested questions and verify that the keyword is present in the suggested questions");
            var questions = askAiAgentPage.GetAllSuggestedQuestions();
            Assert.IsTrue(questions.Count >= 1, "Suggested questions count is 0");

            foreach (var question in questions)
            {
                var loweredQuestion = question.ToLower();
                Assert.IsTrue(loweredQuestion.Contains("team") || loweredQuestion.Contains("teams"),
                    $"The question '{question}' does not contain 'team' or 'teams' keyword.");
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAi_QuestionHistory()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on Ask AI Agent tab and Verify that user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            var prompt = "How many growth items have been created so far?";
            askAiAgentPage.EnterQuestionInTextarea(prompt);
            askAiAgentPage.ClickOnSendButton();

            Log.Info("Click on 'History' button and verify that the popup for Questions History is displayed successfully");
            askAiAgentPage.ClickOnHistoryButton();
            Assert.IsTrue(askAiAgentPage.IsHistoryPopupTitleDisplayed(), "The popup for Questions History is not displayed.");

            Log.Info("Click on 'Default Questions' checkbox and verify that the question is added in history successfully");
            askAiAgentPage.ClickOnDefaultQuestionsCheckbox();
            Assert.IsTrue(askAiAgentPage.IsQuestionDisplayedInHistory(prompt),$"The Question '{prompt}' is not added in history");

            Log.Info("Close the History popup and verify that the popup is closed");
            askAiAgentPage.ClickOnHistoryPopupCloseButton();
            Assert.IsFalse(askAiAgentPage.IsHistoryPopupTitleDisplayed(), "The popup for Questions History is not closed.");
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("PartnerAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void AskAi_QuestionResponseReceivedSuccessfully()
        {
            var loginPage = new LoginPage(Driver, Log);
            var askAiAgentPage = new AskAiAgentPage(Driver, Log);

            Log.Info("Login and navigate to Ask AI Agent dashboard");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(InsightsUser.Username, InsightsUser.Password);
            askAiAgentPage.NavigateToPage(Company.InsightsId);

            Log.Info("Click on Ask AI Agent tab and Verify that user is navigated to the dashboard successfully");
            askAiAgentPage.ClickOnAskAiAgentTab();
            string username = askAiAgentPage.GetUsernameDisplayedWithWelcomeMessage();
            Assert.AreEqual(InsightsUser.FirstName, username, "The Ask Ai Agent dashboard is not loaded successfully");

            Log.Info("Enter question and click on 'Send' button");
            var prompt = "How many total teams are there?";
            askAiAgentPage.EnterQuestionInTextarea(prompt);
            askAiAgentPage.ClickOnSendButton();

            Log.Info("Verify that the suggested questions displayed successfully");
            var questions = askAiAgentPage.GetAllSuggestedQuestions();
            Assert.IsTrue(questions.Count > 0,"The suggested questions are not displayed successfully");
        }
    }
}
