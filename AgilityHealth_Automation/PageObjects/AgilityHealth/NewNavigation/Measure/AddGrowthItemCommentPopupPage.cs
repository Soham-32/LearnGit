using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.NewNavigation.Measure
{
    public class AddGrowthItemCommentPopupPage : BasePage
    {
        public AddGrowthItemCommentPopupPage(IWebDriver driver, ILogger log = null) : base(driver, log)
        {
        }

        #region Locators
        private readonly By CommentsTab = By.Id("noteTab");
        private readonly By CommentTextBox = By.Id("content");
        private readonly By CommentSaveButton = By.Id("button");
        private readonly By CommentUpdateButton = By.Id("btnUpdateNotes");
        private static By GrowthItemComment(string comment) => By.XPath($"*//div[@id='parentCommentDiv']//p[text()='{comment}']");
        private static By CommentEditButton(string comment) => By.XPath(
            $"//p[text()='{comment}']//parent::div//parent::div//preceding-sibling::div//div[2]//span[text()='Edit']");
        private static By CommentDeleteButton(string comment) => By.XPath($"//p[text()='{comment}']//parent::div//parent::div//preceding-sibling::div//div[2]//span[text()='Delete']");
        #endregion

        #region Methods
        public void ClickOnCommentTab()
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), "Click On Comments Tab");
            Driver.JavaScriptScrollToElement(CommentsTab);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(CommentsTab).Click();
        }

        public void AddGrowthItemComment(string comment)
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), $"Add comment: {comment} ");
            Wait.UntilElementClickable(CommentTextBox).SetText(comment);
        }

        public void UpdateGrowthItemComment(string comment, string updateComment)
        {
            Wait.UntilJavaScriptReady();
            ClickOnCommentEditButton(comment);

            Log.Step(nameof(AddGrowthItemCommentPopupPage), $"Update {comment} comment to {updateComment} comment");
            Wait.UntilElementClickable(CommentTextBox).Clear();
            Wait.UntilElementClickable(CommentTextBox).SetText(updateComment);
        }
        public void DeleteGrowthItemComment(string updateComment)
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), $"Delete Comment: {updateComment}");
            Wait.UntilElementClickable(CommentDeleteButton(updateComment)).Click();
            Driver.AcceptAlert();
        }
        public void ClickOnCommentEditButton(string comment)
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), $"Click On {comment}'s Edit Button");
            Wait.UntilElementExists(CommentEditButton(comment));
            Wait.UntilElementClickable(CommentEditButton(comment)).Click();
        }
        public void ClickOnCommentSaveButton()
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), "Click On Save Button");
            Wait.UntilElementClickable(CommentSaveButton).Click();
        }
        public void ClickOnCommentUpdateButton()
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), "Click On Update Button");
            Wait.UntilElementClickable(CommentUpdateButton).Click();
        }

        public bool IsCommentPresent(string comment)
        {
            Log.Step(nameof(AddGrowthItemCommentPopupPage), $" Is {comment} Present ?");
            Wait.UntilJavaScriptReady();
            return Driver.IsElementPresent(GrowthItemComment(comment));
        }
        #endregion
    }
}
