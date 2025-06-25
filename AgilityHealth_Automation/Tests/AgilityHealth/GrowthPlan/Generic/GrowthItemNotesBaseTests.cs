using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.Generic
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    [TestCategory("CompanyAdmin")]
    public class GrowthItemNotesBaseTests : BaseTest
    {

        public void GrowthItemAddEditDeleteNotes(string growthItemTitle)
        {
            var growthItemNote = new AddGrowthItemNotePage(Driver, Log);
            var growthItemGrid = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            //Notes
            var note = "Growth Item " + RandomDataUtil.GetGrowthPlanComment() + " Note";
            var updateNote = "Growth Item " + RandomDataUtil.GetGrowthPlanComment() + " Updated Note";

            // Add Note
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.AddGrowthItemNote(note);
            growthItemNote.ClickOnNoteSaveButton();
            Assert.IsTrue(growthItemNote.IsNotePresent(note), $"Note- {note} is not Present");
            addGrowthItemPopup.ClickSaveButton(true);

            // Update Note
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.ClickOnNoteTab();
            growthItemNote.UpdateGrowthItemNote(note, updateNote);
            growthItemNote.ClickOnNoteUpdateButton();
            Assert.IsTrue(growthItemNote.IsNotePresent(updateNote), $"Note - {updateNote} is not present");
            Assert.IsFalse(growthItemNote.IsNotePresent(note), $"Note- {note} is Present");
            addGrowthItemPopup.ClickSaveButton(true);

            // Delete Note
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.DeleteGrowthItemNote(updateNote);
            Assert.IsFalse(growthItemNote.IsNotePresent(updateNote), $"Note - {updateNote} is not deleted");
        }

        public void GrowthItemAddEditNotesViaSaveButtonOfPopup(string growthItemTitle)
        {
            var growthItemNote = new AddGrowthItemNotePage(Driver, Log);
            var growthItemGrid = new GrowthItemGridViewWidget(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);

            //Notes
            var note = "Growth Item " + RandomDataUtil.GetGrowthPlanComment() + " Note";
            var updateNote = "Growth Item " + RandomDataUtil.GetGrowthPlanComment() + " Updated Note";

            // Add Note
            Log.Info("Open 'Edit Growth Plan Item' popup, Add 'Note' via click on 'Save' button of 'Edit Growth Plan Item' popup and verify");
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.AddGrowthItemNote(note);
            addGrowthItemPopup.ClickOnSolutionTab();
            growthItemNote.ClickOnNoteTab();
            addGrowthItemPopup.ClickSaveButton(true);

            // Verification
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.ClickOnNoteTab();
            Assert.IsTrue(growthItemNote.IsNotePresent(note), $"Note- {note} is not Present");

            // Update Note
            Log.Info("Open 'Edit Growth Plan Item' popup, Update 'Note' via click on 'Save' button of 'Edit Growth Plan Item' popup and verify");
            growthItemNote.UpdateGrowthItemNote(note, updateNote);
            addGrowthItemPopup.ClickOnGrowthPlanItemTab();
            growthItemNote.ClickOnNoteTab();
            addGrowthItemPopup.ClickSaveButton(true);

            //Verification
            growthItemGrid.ClickGrowthItemEditButton(growthItemTitle);
            growthItemNote.ClickOnNoteTab();
            Assert.IsTrue(growthItemNote.IsNotePresent(updateNote), $"Note - {updateNote} is not present");
            Assert.IsFalse(growthItemNote.IsNotePresent(note), $"Note- {note} is Present");
        }
    }
}