using System;
using System.Collections.Generic;
using System.Globalization;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class CommentsTabPage : BaseTabPage
    {
        public CommentsTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Comments

        //Comments
        
        private readonly By CommentTextBox = By.XPath("//textarea[@placeholder='Add a comment. Use @ to mention a person.']");
        private readonly By AddCommentButton = By.XPath("//button[text()='Add Comment']");
        private static By CommentAuthor(int index) => By.XPath($"//*[@id = 'commentsForm']/div[@id = 'commentWrapper'][{index}]//div[@id = 'commentAuthor']");
        private static By CommentContent(int index) => By.XPath($"//*[@id='commentsForm']/div[@id='commentWrapper'][{index}]//div[2]/p");
        private static By CommentAvatar(int index) => By.XPath($"//*[@id = 'commentsForm']/div[@id = 'commentWrapper'][{index}]//div[contains(@class, 'comment__avatar')]");
        private static By CommentAvatarImage(int index) => By.XPath($"//*[@id = 'commentsForm']/div[@id = 'commentWrapper'][{index}]//div[contains(@class, 'comment__avatar')]//img");
        private readonly By CommentWrapper = By.Id("commentWrapper");

        #endregion


        #region Comments

        //Comments
       

        public void AddComment(string commentText)
        {
            Log.Step(nameof(CommentsTabPage), $"Add comment <{commentText}>");
            Driver.JavaScriptScrollToElement(CommentTextBox).SetText(commentText);
            Wait.UntilJavaScriptReady();
            Driver.JavaScriptScrollToElement(AddCommentButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<CommentsResponse> GetComments()
        {
            var commentCount = Wait.UntilAllElementsLocated(CommentWrapper).Count;

            var list = new List<CommentsResponse>();
            for (var i = 1; i < commentCount + 1; i++)
            {
                var authorInfo = Wait.UntilElementVisible(CommentAuthor(i))
                    .GetText().Replace( " \r\n-","")
                    .Split(new[] { " Commented " }, StringSplitOptions.RemoveEmptyEntries);
                list.Add(new CommentsResponse
                {
                    Commenter = authorInfo[0],
                    CommentDate = DateTime.ParseExact(authorInfo[1].Replace("\r\n", ""), "MM/dd/yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture),
                    Content = Wait.UntilElementVisible(CommentContent(i)).GetText(),
                    Avatar = Wait.InCase(CommentAvatarImage(i))?.GetAttribute("src") ?? Wait.UntilElementVisible(CommentAvatar(i)).GetText()
                });
            }

            return list;
        }

        #endregion


    }
}
