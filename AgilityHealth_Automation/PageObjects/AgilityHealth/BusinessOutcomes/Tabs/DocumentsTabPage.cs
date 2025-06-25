using System.Collections.Generic;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Tabs
{
    public class DocumentsTabPage : BaseTabPage
    {
        public DocumentsTabPage(IWebDriver driver, ILogger log) : base(driver, log) { }

        #region Documents

        //Documents

        private readonly By AddDocument = By.XPath("//button[text()='Add a Document']");
        private readonly By UploadADocument = By.XPath("//li[text()='Upload a Document']");
        private readonly By AddLinkButton = By.XPath("//li[text()='Add a Link']");
        private static By DocumentTitle(string title) => By.XPath($"//p[@title='{title}']");

        private readonly By FilePathField = By.XPath("//input[@type='file']");
        private readonly By AddLinkTitle = By.Id("title");
        private readonly By AddLinkValue = By.Id("link");
        private static By ExternalLinkIcon(string title) => By.XPath($"//p[text()='{title}']//ancestor::td//*[local-name()='svg' and @data-icon='external-link-alt']");
        private readonly By AddLinkPopup = By.XPath("//h6[@id='add-link-title']//ancestor::div[2]");
        private readonly By DocumentUploadedToasterMessage = By.XPath("//div[text()='Document uploaded successfully.']");
        private readonly By AddLinkAddButton = By.XPath("//button[text()='Add']");

        private readonly By DocumentRows = By.XPath("//button[text()='Add a Document']/parent::div/following-sibling::table[contains(@class, 'MuiTable-root')]//tbody//tr[not(td[contains(text(), 'No records available')])]");
        private static By DocumentStatusColumn(int rowIndex, int columnIndex) => By.XPath($"(//button[text()='Add a Document']/parent::div/following-sibling::table/tbody/tr[{rowIndex}])//td[{columnIndex}]//div[@aria-haspopup='listbox']");
        private static By DocumentColumnValues(int rowIndex,int columnIndex) => By.XPath($"//button[text()='Add a Document']/parent::div/following-sibling::table/tbody/tr[{rowIndex}]//td[{columnIndex}]//p");

        private static By DocumentApprovedDateValues(int rowIndex, int columnIndex) => By.XPath($"//button[text()='Add a Document']/parent::div/following-sibling::table/tbody/tr[{rowIndex}]//td[{columnIndex}]");


        #endregion


        #region Documents

        //Documents


        public void UploadDocument(string filepath)
        {
            Log.Step(nameof(DocumentsTabPage), $"Upload the Document");
            Wait.UntilElementClickable(AddDocument).Click();
            Wait.UntilElementClickable(UploadADocument).Click();
            Wait.UntilElementExists(FilePathField).SendKeys(filepath);
            Wait.UntilElementVisible(DocumentUploadedToasterMessage);
        }

        public void ClickOnUploadALink()
        {
            Log.Step(nameof(DocumentsTabPage), $"Click on `Add A Link' dropdown");
            Wait.UntilElementClickable(AddDocument).Click();
            Wait.UntilElementClickable(AddLinkButton).Click();
        }

        public void AddLinkPopUp(string title,string link)
        {
            Log.Step(nameof(DocumentsTabPage), $"Add title and url for the Add Link Popup");
            Wait.UntilElementClickable(AddLinkTitle).SendKeys(title);
            Wait.UntilElementClickable(AddLinkValue).SendKeys(link);
            Wait.UntilElementClickable(AddLinkAddButton).Click();
        }

        public void ClickAddedLink(string title)
        {
            Log.Step(nameof(DocumentsTabPage), $"Click on Added Link");
            Wait.UntilElementClickable(ExternalLinkIcon(title)).Click();
        }


        public bool IsAddLinkPopupDisplayed()
        {
            return Driver.IsElementDisplayed(AddLinkPopup);
        }

        public List<Documents> GetDocumentsResponse()
        {

            Log.Step(nameof(DocumentsTabPage), $"Get the added Document response");

            Driver.JavaScriptScrollToElement(DocumentRows);
            // Get row count from the table
            var rowCount = Driver.GetElementCount(DocumentRows);

            var list = new List<Documents>();

            // Loop through each row to get the data
            for (var i = 1; i <= rowCount; i++)
            {
                // Extract data from each column using a helper method
                var documentTitle = GetTextFromRow(i, 1);
                var size = GetTextFromRow(i, 2);
                var addedBy = GetTextFromRow(i, 3);
                var date = GetTextFromRow(i, 4);
                var parent = GetTextFromRow(i, 5);
                var cardType = GetTextFromRow(i, 6);
                var cardTitle = GetTextFromRow(i, 7);
                var status = Wait.UntilElementVisible(DocumentStatusColumn(i, 8)).GetText();
                var approvedBy = GetTextFromRow(i, 9);
                var approvedDate = Wait.UntilElementVisible(DocumentApprovedDateValues(i, 10)).GetText();

                // Create an object and add it to the list
                list.Add(new Documents()
                {
                    Title = documentTitle,
                    Size = size,
                    AddedBy = addedBy,
                    Date = date,
                    Parent = parent,
                    CardType = cardType,
                    CardTitle = cardTitle,
                    Status = status,
                    ApprovedBy = approvedBy,
                    ApprovedDate = approvedDate
                });
            }

            return list;
        }

        public void ClickOnLink(string title)
        {
            Log.Step(nameof(DocumentsTabPage), $"Click on Added Link<{title}>");
            ClickOnActionsKebabIcon(title);
            ClickOnActionsDeleteOption();
            ClickOnRemoveButton();
        }
        public void DeleteDocument(string title)
        {
            Log.Step(nameof(DocumentsTabPage), $"Click on 'Delete' button for Document <{title}>");
            ClickOnActionsKebabIcon(title);
            ClickOnActionsDeleteOption();
            ClickOnRemoveButton();
        }

        public bool IsDocumentDisplayed(string title)
        {
            return Driver.IsElementDisplayed(DocumentTitle(title));
        }

        // Helper function to retrieve the value from each cell in the row
        private string GetTextFromRow(int rowIndex, int columnIndex)
        {
            Log.Step(nameof(DocumentsTabPage), $"Get the Document text");
            return Wait.UntilElementVisible(DocumentColumnValues(rowIndex,columnIndex)).GetText();
        }

        #endregion


    }
}
