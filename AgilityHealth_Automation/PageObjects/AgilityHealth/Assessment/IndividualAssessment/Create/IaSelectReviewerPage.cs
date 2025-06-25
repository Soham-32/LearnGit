using System.Collections.Generic;
using System.Text.RegularExpressions;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.DataObjects;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create
{
    internal class IaSelectReviewerPage : BasePage

    {
        public IaSelectReviewerPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }

        private readonly By IaSelectReviewerIntro = By.ClassName("intro");
        private readonly By SelectAllCheckbox = By.Id("selectAll");
        private readonly By NextReviewButton = By.XPath("//span[contains(.,'Next, Review')]");
        private readonly By NextReviewButton1 = By.Id("btnAddSubTeams");
        private By RevieweeExpandToggle(int index) => By.CssSelector($"#reviewees-grid tbody tr:nth-of-type({index}) a");
        private By RevieweeExpandToggle(string reviewee) => By.XPath($"//span[@class='fl-lt'][text()='{reviewee}']/parent::td/preceding-sibling::td/a");
        private By ReviewersGrid(string reviewee) => By.XPath($"//span[@class='fl-lt'][text()='{reviewee}']/ancestor::tr/following-sibling::tr[1]//div[@class = 'k-widget k-grid k-editable']");
        private readonly By AddAdditionalReviewerButton = By.CssSelector("a.k-grid-add");
        private readonly By ReviewerFirstNameTextbox = By.Id("FirstName");
        private readonly By ReviewerLastNameTextbox = By.Id("LastName");
        private readonly By ReviewerEmailTextbox = By.Id("Email");
        private readonly By ReviewerRoleListbox = By.CssSelector("span[aria-owns = 'Roles_listbox']");
        private By ReviewerRoleListItem(string item) => By.XPath($"//ul[@id = 'Roles_listbox']/li[text() = '{item}']");
        private readonly By ReviewerUpdateButton = By.Id("update_0");
        private readonly By DownloadReviewersTemplateButton = By.XPath("//img[@src = '/images/Download-icon.png']");
        private readonly By UploadAdditionalReviewerButton = By.ClassName("k-grid-excelUpload");
        private readonly By FilePathField = By.Id("files");
        private readonly By FileUploadDone = By.XPath("//strong[contains(@class,'k-upload-status-total')][contains(.,'Done')]");
        private readonly By UploadButton = By.Id("uploadFile");
        private readonly By ImportCompleteWindowTitle = By.Id("importCompleteWindow_wnd_title");
        private readonly By UploadCompleteCloseButton = By.XPath("//button[@onclick = 'closeImportCompleteWindow()']");
        private readonly By ReviewersRows = By.CssSelector("div[id^='reviewers-grid'] tbody tr");
        private By ReviewerRowValueByColumn(int rowIndex, string columnName) => By.XPath($"//div[starts-with(@id, 'reviewers-grid')]//tbody//tr[{rowIndex}]/td[count(//div[starts-with(@id, 'reviewers-grid')]//th[@data-title='{columnName}']/span/../preceding-sibling::th) + 1]");

        public void SelectAllReviewers()
        {
            Log.Step(nameof(IaSelectReviewerPage), "Click on Select All Reviewers checkbox");
            Wait.UntilElementClickable(SelectAllCheckbox).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickNextReview()
        {
            Log.Step(nameof(IaSelectReviewerPage), "Click on Next Review button");
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(NextReviewButton)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ClickNextReview1()
        {
            Log.Step(nameof(IaSelectReviewerPage), "Click on Next Review 1 button");
            Driver.JavaScriptScrollToElement(Wait.UntilElementVisible(NextReviewButton1)).Click();
            Wait.UntilJavaScriptReady();
        }

        public bool DoesIntroTextDisplayCorrectly(string expected)
        {
            var introText = Wait.UntilElementVisible(IaSelectReviewerIntro).GetText();
            introText = Regex.Replace(introText, @"\r\n?|\n", "");
            return expected.Equals(introText);
        }

        public void ExpandRevieweeRow(int index)
        {
            Wait.UntilElementClickable(RevieweeExpandToggle(1)).Click();
            Wait.UntilJavaScriptReady();
        }

        public void ExpandRevieweeRow(Reviewee reviewee)
        {
            Log.Step(nameof(IaSelectReviewerPage), $"Expand reviewee ${reviewee.FullName}");
            var toggle = Wait.UntilElementClickable(RevieweeExpandToggle(reviewee.FullName));
            if (toggle.GetAttribute("class").Contains("k-plus"))
            {
                toggle.Click();
                Wait.UntilJavaScriptReady();
            }
        }

        public void DownloadReviewersTemplate()
        {
            Log.Step(nameof(IaSelectReviewerPage), "Download the Reviewers template");
            Wait.UntilElementClickable(DownloadReviewersTemplateButton).Click();
        }

        public void UploadAdditionalReviewer(string filePath)
        {
            Log.Step(nameof(IaSelectReviewerPage), "Upload reviewer file");
            Wait.UntilElementClickable(UploadAdditionalReviewerButton).Click();

            Wait.UntilElementExists(FilePathField).SendKeys(filePath);

            Wait.UntilElementExists(FileUploadDone);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UploadButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementVisible(ImportCompleteWindowTitle);
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(UploadCompleteCloseButton).Click();
            Wait.UntilJavaScriptReady();
        }

        public List<Reviewer> GetAllReviewersFromGrid()
        {
            var reviewers = new List<Reviewer>();

            Wait.UntilJavaScriptReady();

            int rowCount = Driver.GetElementCount(ReviewersRows);

            for (int i = 1; i <= rowCount; i++)
            {
                reviewers.Add(GetReviewerFromGrid(i));
            }

            return reviewers;
        }

        public Reviewer GetReviewerFromGrid(int index)
        {
            var reviewer = new Reviewer
            {
                FirstName = Wait.UntilElementVisible(ReviewerRowValueByColumn(index, "Firstname")).GetText(),
                LastName = Wait.UntilElementVisible(ReviewerRowValueByColumn(index, "Lastname")).GetText(),
                Role = Wait.UntilElementVisible(ReviewerRowValueByColumn(index, "Role")).GetText(),
                Email = Wait.UntilElementVisible(ReviewerRowValueByColumn(index, "Email")).GetText()
            };

            return reviewer;
        }

        internal void AddAdditionalReviewer(Reviewee reviewee, Reviewer reviewer)
        {
            ExpandRevieweeRow(reviewee);
            Log.Step(nameof(IaSelectReviewerPage), "Add additional reviewer");
            var reviewersGrid = Wait.UntilElementVisible(ReviewersGrid(reviewee.FullName));
            reviewersGrid.FindElement(AddAdditionalReviewerButton).Click();
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(ReviewerFirstNameTextbox).SetText(reviewer.FirstName);
            Wait.UntilElementClickable(ReviewerLastNameTextbox).SetText(reviewer.LastName);
            SelectItem(ReviewerRoleListbox, ReviewerRoleListItem(reviewer.Role));
            Wait.UntilElementClickable(ReviewerEmailTextbox).SetText(reviewer.Email);
            Wait.UntilElementClickable(ReviewerUpdateButton).Click();
            Wait.UntilJavaScriptReady();

        }

        internal bool DoesReviewerExist(Reviewee reviewee, Reviewer reviewer)
        {
            var grid = Wait.UntilElementVisible(ReviewersGrid(reviewee.FullName));

            var elements = grid.FindElements(By.XPath(
                $"//tr/td[text() = '{reviewer.FirstName}']/following-sibling::td[text() = '{reviewer.LastName}']/following-sibling::td[text() = '{reviewer.Role}']/following-sibling::td[text() = '{reviewer.Email}']"));

            return elements.Count > 0;
        }
    }
}