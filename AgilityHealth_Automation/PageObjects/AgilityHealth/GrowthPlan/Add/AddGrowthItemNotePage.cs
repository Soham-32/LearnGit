using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using OpenQA.Selenium;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add
{
    internal class AddGrowthItemNotePage : BasePage
    {
        public AddGrowthItemNotePage(IWebDriver driver, ILogger log) : base(driver,log)
        {
        }

        private readonly By NotesTab = By.Id("noteTab");
        private readonly By NotesTextBox = By.Id("content");
        private readonly By NoteSaveButton = By.Id("button");
        private readonly By NoteUpdateButton = By.Id("btnUpdateNotes");
        private static By GrowthItemNotes(string notes) => By.XPath($"*//div[@id='parentCommentDiv']//p[text()='{notes}']");
        private static By NotesEditButton(string notes) => By.XPath(
            $"//p[text()='{notes}']//parent::div//parent::div//preceding-sibling::div//div[2]//span[text()='Edit']");
        private static By NotesDeleteButton(string notes) => By.XPath($"//p[text()='{notes}']//parent::div//parent::div//preceding-sibling::div//div[2]//span[text()='Delete']");


        // Methods
        public void ClickOnNoteTab()
        {
            Log.Step(nameof(AddGrowthItemNotePage), "Click On Notes Tab");
            Wait.UntilJavaScriptReady();
            Wait.UntilElementClickable(NotesTab).Click();
        }

        // Add / Update / Delete Note
        public void AddGrowthItemNote(string note)
        {
            Log.Step(nameof(AddGrowthItemNotePage), $"Add Note: {note} ");
            ClickOnNoteTab();
            Wait.UntilElementClickable(NotesTextBox).SetText(note);
        }

        public void UpdateGrowthItemNote(string note, string updateNote)
        {
            Wait.UntilJavaScriptReady();
            // Click On Edit Button
            ClickOnNoteEditButton(note);

            // Update a Note
            Log.Step(nameof(AddGrowthItemNotePage), $"Update {note} note to {updateNote} note");
            Wait.UntilElementClickable(NotesTextBox).Clear();
            Wait.UntilElementClickable(NotesTextBox).SetText(updateNote);
        }
        public void DeleteGrowthItemNote(string updateNote)
        {
            Log.Step(nameof(AddGrowthItemNotePage), $"Delete Note: {updateNote}");
            ClickOnNoteTab();
            Wait.UntilElementClickable(NotesDeleteButton(updateNote)).Click();
            Driver.AcceptAlert();
        }
        public void ClickOnNoteEditButton(string note)
        {
            Log.Step(nameof(AddGrowthItemNotePage), $"Click On {note}'s Edit Button");
            Wait.UntilElementExists(NotesEditButton(note));
            Wait.UntilElementClickable(NotesEditButton(note)).Click();
        }
        public void ClickOnNoteSaveButton()
        {
            Log.Step(nameof(AddGrowthItemNotePage), "Click On Save Button");
            Wait.UntilElementClickable(NoteSaveButton).Click();
        }
        public void ClickOnNoteUpdateButton()
        {
            Log.Step(nameof(AddGrowthItemNotePage), "Click On Update Button");
            Wait.UntilElementClickable(NoteUpdateButton).Click();
        }

        // Common
        public bool IsNotePresent(string note)
        {
            Log.Step(nameof(AddGrowthItemNotePage), $" Is {note} Present ?");
            Wait.UntilJavaScriptReady();
            return Driver.IsElementPresent(GrowthItemNotes(note));
        }
    }
}