using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using AtCommon.Dtos.GrowthPlan.Custom;

namespace AgilityHealth_Automation.PageObjects.AgilityHealth.Ideaboard
{
    internal class IdeaboardPage : BasePage
    {
        public IdeaboardPage(IWebDriver driver, ILogger log) : base(driver, log)
        {
        }
        // Header
        private readonly By TotalVoteCountOfUser = By.XPath("//h6[text()='Your Votes:']//parent::div//div//h6[text()='Voted']//preceding-sibling::h6 | //h6[text()='Your Votes:']//parent::div//div//h6//font[text()='Voted']//..//..//preceding-sibling::h6");
        private readonly By LeftVotesCountOfUser = By.XPath("//h6[text()='Your Votes:']//parent::div//div//h6[text()='Votes Left']//preceding-sibling::h6");
        private readonly By TotalVoteCountOfBoard = By.XPath("//h6[text()='Board Votes:']//parent::div//div//h6[text()='Voted']//preceding-sibling::h6");
        private readonly By LeftVotesCountOfBoard = By.XPath("//h6[text()='Board Votes:']//parent::div//div//h6[text()='Votes Left']//preceding-sibling::h6");

        private readonly By SortVotesButton = By.XPath("//button//*[local-name() = 'svg'][@data-icon='arrow-up-arrow-down']");
        private readonly By ResetVotesButton = By.XPath("//button//*[local-name() = 'svg'][@data-icon='arrow-rotate-right']");
        private readonly By VotesAllowedTextArea = By.CssSelector(".MuiInputBase-root input");
        private readonly By VotesAllowedUpIcon =
            By.CssSelector("button > svg.fa-angle-up");
        private readonly By VotesAllowedDownIcon =
            By.CssSelector("button > svg.fa-angle-down");
        private readonly By IdeaBoardTitleText = By.XPath("//h6[contains(text(),'Idea Board')]");
        private static By AddCardButtonByDimension(string dimensionName) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div/button[@automation-id='addCardButton']");

        //Grid
        private readonly By NoteDimensionHeader = By.XPath("//h6[text()='Notes']");
        private readonly By LoadingSpinner = By.XPath("//p[contains(text(), 'Loading')]//img[contains(@src, 'load') and contains(@src, '.gif')]");
        public static By CardAreaByDimension(string dimensionName) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div/button[@automation-id='addCardButton']/../following-sibling::div");
        private readonly By CardTextBoxes = By.XPath("//textarea[@rows]");
        private static By CardByDimension(string dimensionName, string cardText = null) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//descendant::p/*[@automation-id='cardTextArea'][contains(text(), '{cardText}')]");
        private static By CardByDimensionDraggableArea(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//descendant::p/*[contains(text(),'{cardText}')]//ancestor::div[@draggable]/div");
        private static By DeleteIconByCardTextAndDimension(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//descendant::p/*[@automation-id='cardTextArea'][contains(text(), '{cardText}')]//parent :: p//parent :: div/../div//following-sibling::div//div");
        private readonly By DeletePopupDeleteButton = AutomationId.Equals("deleteCard_delete");
        private readonly By DeletePopupCancelButton = AutomationId.Equals("deleteCard_cancel");
        private readonly By MergePopupMergeButton = AutomationId.Equals("combineCards_merge");
        private readonly By MergePopupCancelButton = AutomationId.Equals("combineCards_cancel");
        private readonly By CannotMergeDialogBox = By.XPath("//div[@automation-id='combineDialog']//div[contains(@class,'MuiDialogContent-root')]");
        private readonly By CannotMergePopupOkayButton = By.XPath("//div[@automation-id='combineDialog']//button");
        private readonly By ResetPopupResetButton = AutomationId.Equals("resetBtn");
        private readonly By ResetPopupCancelButton = AutomationId.Equals("cancelBtn");
        private static By VoteUpIconByCardTextAndDimension(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//p/*[@automation-id='cardTextArea'][(text()='{cardText}')]//ancestor::div[contains(@class,'MuiPaper-root')]//div//*[local-name() = 'svg'][@automation-id='voteCountUp']");
        private static By VoteCountByDimensionAndCardCount(string dimensionName, int cardCount) => By.XPath($"//h6[contains(text(),'{dimensionName}')]//parent::div//parent::div//following-sibling::div//div[@data-rbd-draggable-context-id='2'][{cardCount}]//div[@pr='2']//p");
        private static By VoteCountForCardByCardTextAndDimension(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']//following-sibling::div//p/*[@automation-id='cardTextArea'][contains(text(),'{cardText}')]//ancestor::div[contains(@class,'MuiPaper-root')]//div[@pr='2']//p[contains(@class, 'MuiTypography-body1')]");

        private static By VotesByUser(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']//following-sibling::div//p/*[@automation-id='cardTextArea'][contains(text(),'{cardText}')]//ancestor::div[contains(@class,'MuiPaper-root')]//div[@pr='2']//*[local-name()='svg'][@automation-id='vote']");
        private static By RemoveVoteIconForCardByCardTextAndDimension(string dimensionName, string cardText) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']//following-sibling::div//p/*[@automation-id='cardTextArea'][(text()='{cardText}')]//ancestor::div[contains(@class,'MuiPaper-root')]//div[@pr='2']//*[local-name() = 'svg'][@automation-id='deleteVote']");
        private static By DeleteCardIconByDimensionAndGrowthItemType(string dimensionName, string cardText, string growthItemType) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//p/textarea[contains(text(),'{cardText}')]//ancestor::div[@display='flex']/following-sibling::div//*[local-name()='svg'][@automation-id='{growthItemType}']//ancestor::div[contains(@class,'MuiPaper-root')]//div/*[@automation-id='cardIcon_button']");
        private static By GrowthItemIconByCardTextAndDimension(string dimensionName, string cardText, string growthItemType) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//p/textarea[contains(text(),'{cardText}')]//ancestor::div[@display='flex']/following-sibling::div//*[local-name()='svg'][@automation-id='{growthItemType}']");
        private readonly By SuccessNotificationToastText = By.XPath("//div[text() = 'Successfully Updated Growth Item']");
        private readonly By SuccessAddNotificationToastText = By.XPath("//div[text() = 'Successfully Added Growth Item']");
        private readonly By SuccessSortNotificationToastText = By.XPath("//div[text() = 'Successfully Sorted Cards']");
        private readonly By SortingCardNotificationToastText = By.XPath("//div[text() = 'Sorting Cards']");
        private readonly By CombineCardNotificationToastText = By.XPath("//div[text() = 'Combining Cards']");
        private readonly By DeletingCardNotificationToastText = By.XPath("//div[text() = 'Deleting Cards']");
        private readonly By MergeSuccessNotificationToastText = By.XPath("//div[text() = 'The card have been successfully combined']");
        private readonly By SettingVotesAllowedToastText = By.XPath("//h6[text() = 'Setting Votes Allowed']");
        private readonly By SuccessfullySetVotesAllowedToastText = By.XPath("//h6[text() = 'Successfully Set Votes Allowed']");
        private readonly By SuccessfullyRetrievedVotesAllowedToastText = By.XPath("//h6[text() = 'Successfully Retrieved Votes Allowed']");
        private readonly By FailedToSetVotesAllowedToastText = By.XPath("//h6[text() = 'Failed to Set Votes Allowed']");
        private readonly By SuccessfullyResettingVotesToastText = By.XPath("//h6[text() = 'Resetting Votes']");
        private readonly By SuccessfullyResetVotesToastText = By.XPath("//h6[text() = 'Successfully Reset Votes']");
        private readonly By SuccessfullyAddedCardText = By.XPath("//h6[text()= 'Successfully Added Card']");
        private readonly By UpdatingToolTip = By.XPath("//div[@role='tooltip']//*[text()='Updating']");
        private readonly By ToolTip = By.XPath("//*[@role = 'tooltip']//div | //*[@role = 'tooltip']//div//font//font");

        // Locators for disabled icons
        private readonly By IndividualGrowthItemIcon = AutomationId.Equals(GrowthItemsType.IndividualGrowthItem.GetDescription());
        private readonly By TeamGrowthItemIcon = AutomationId.Equals(GrowthItemsType.TeamGrowthItem.GetDescription());
        private readonly By OrganizationalGrowthItemIcon = AutomationId.Equals(GrowthItemsType.OrganizationalGrowthItem.GetDescription());
        private readonly By ManageGrowthItemIcon = AutomationId.Equals(GrowthItemsType.ManagementGrowthItem.GetDescription());
        private static By VoteUpIconByDimension(string dimensionName) => By.XPath(
            $"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//p/*[@automation-id='cardTextArea']//ancestor::div[contains(@class,'MuiPaper-root')]/div//*[local-name() = 'svg'][@automation-id='voteCountUp']");
        private static By VoteCountForCardByDimension(string dimensionName) =>
            By.XPath($"//h6[text()='{dimensionName}']//ancestor::div[@display='flex']/following-sibling::div//p/*[@automation-id='cardTextArea']//ancestor::div[contains(@class,'MuiPaper-root')]/div[@pr='2']//p[contains(@class,'MuiTypography-root')]");

        //Growth Item popup
        private readonly By GiTitle = By.XPath("//label[text()='Title*']//following-sibling::div//input");
        private readonly By GiCategoryDropdown = By.XPath("//label[@id='category-label']//following-sibling::div/div");

        private static By GiCategoryDropdownValues(string value) =>
            By.XPath($"//ul[@aria-labelledby='category-label']//li[text()='{value}']");

        private readonly By GiPriorityDropdown = By.XPath("//label[@id='priority-label']//following-sibling::div");

        private static By GiPriorityDropdownValues(string value) =>
            By.XPath($"//ul[@aria-labelledby='priority-label']//li[text()='{value}']");

        private readonly By GiStatusDropdown = By.XPath("//label[@id='status-label']//following-sibling::div");

        private static By GiStatusDropdownValues(string value) =>
            By.XPath($"//ul[@aria-labelledby='status-label']//li[text()='{value}']");

        private readonly By GiCompetencyDropdown =
            By.XPath("//label[@id='competency-target-label']//following-sibling::div");

        private static By GiCompetencyDropdownValues(string value) => By.XPath(
            $"//ul[@aria-labelledby='competency-target-label']//span[text()='{value}']//..//preceding-sibling::span");

        private readonly By GiCreateButton =
            By.XPath("//button[text()='Create']");

        //Methods
        //Header
        public void ClickOnAddCardButtonByDimension(string dimensionName)
        {
            Log.Step(nameof(IdeaboardPage), $"Click on '+' button for creating card of {dimensionName} category");
            Wait.UntilElementClickable(AddCardButtonByDimension(dimensionName)).Click();
            Wait.HardWait(3000); // Wait until header text removed
            Wait.UntilElementNotExist(SuccessfullyAddedCardText);

        }
        public string GetTotalNumberOfVotesOnBoard()
        {
            Log.Step(nameof(IdeaboardPage), "Get the total number of board votes for ideaboard");
            return Wait.UntilElementVisible(TotalVoteCountOfBoard).GetText();
        }
        public string GetNumberOfVotesLeftOfBoard()
        {
            Log.Step(nameof(IdeaboardPage), "Get the total number of board votes left for ideaboard");
            return Wait.UntilElementVisible(LeftVotesCountOfBoard).GetText();
        }
        public string GetTotalNumberOfVotesGivenByUser()
        {
            Log.Step(nameof(IdeaboardPage), "Get the total number of votes given by user for ideaboard");
            return Wait.UntilElementVisible(TotalVoteCountOfUser).GetText();
        }
        public string GetNumberOfVotesLeftForUser()
        {
            Log.Step(nameof(IdeaboardPage), "Get the total number of votes left for user on ideaboard");
            return Wait.UntilElementVisible(LeftVotesCountOfUser).GetText();
        }
        public void ClickOnSortVotesButton()
        {
            Log.Step(nameof(IdeaboardPage), "Click on 'SortVotes' button");
            Wait.UntilElementClickable(SortVotesButton).Click();
            Wait.UntilElementNotExist(SortingCardNotificationToastText);
            Wait.UntilElementNotExist(SuccessSortNotificationToastText);
            Wait.HardWait(2000);// Need to wait till cards gets sorted
        }

        public void ClickOnResetVotesButton()
        {
            Log.Step(nameof(IdeaboardPage), "Click on 'ResetVotes' button");
            var resetVotes = Wait.UntilElementVisible(ResetVotesButton);
            Wait.UntilElementClickable(resetVotes).Click();
        }

        public void SetVotesAllowed(string votes)
        {
            Log.Step(nameof(IdeaboardPage), "Set number of 'Votes Allowed' text area");
            Wait.UntilElementClickable(VotesAllowedTextArea).ClearTextbox();
            Wait.UntilElementVisible(FailedToSetVotesAllowedToastText);
            Wait.UntilElementClickable(VotesAllowedTextArea).SendKeys(votes);
            Wait.UntilElementVisible(SettingVotesAllowedToastText);
        }

        public string GetVotesAllowed()
        {
            Log.Step(nameof(IdeaboardPage), "Get number of 'Votes Allowed'");
            return Wait.UntilElementClickable(VotesAllowedTextArea).GetAttribute("value");
        }

        public void SetVotesAllowed(int clicks, bool decrement = false)
        {
            Log.Step(nameof(IdeaboardPage), "Click on Vote Up/Down icon for Specific card");
            var locator = decrement ? VotesAllowedDownIcon : VotesAllowedUpIcon;

            for (var i = 0; i < clicks; i++)
            {
                Wait.UntilElementClickable(locator).Click();
            }

            Wait.UntilElementVisible(SuccessfullySetVotesAllowedToastText);
        }
        public bool IsIdeaBoardTitleDisplayed()
        {
            return Driver.IsElementDisplayed(IdeaBoardTitleText, 10);
        }


        //Body
        public int GetTotalNumberOfCards()
        {
            Log.Step(nameof(IdeaboardPage), "Get total number of cards");
            return Wait.UntilAllElementsLocated(CardTextBoxes).Count;
        }
        public void InputTextInCardByDimension(string dimensionName, string text)
        {
            Log.Step(nameof(IdeaboardPage), $"Enter text on specific card under dimensionName {dimensionName}");
            Wait.UntilAllElementsLocated(CardByDimension(dimensionName));
            Wait.UntilElementClickable(CardByDimension(dimensionName)).SetText(text);
            Wait.UntilElementNotExist(UpdatingToolTip);
        }
        public void UpdateTextInCardByDimension(string dimensionName, string updatedText, string text = null, bool clear = true, bool react = false)
        {
            Log.Step(nameof(IdeaboardPage), $"Update text on specific card under dimensionName {dimensionName}");
            Wait.UntilAllElementsLocated(CardByDimension(dimensionName));
            Wait.UntilElementClickable(CardByDimension(dimensionName)).ClearTextbox();
            Wait.UntilElementClickable(CardByDimension(dimensionName)).SetText(updatedText, clear);
            Wait.UntilElementNotExist(UpdatingToolTip);
        }
        public string GetTextFromCardByDimension(string dimensionName, string text = null)
        {
            Log.Step(nameof(IdeaboardPage), $"Get text from specific card under dimensionName {dimensionName}");
            return Wait.UntilElementClickable(CardByDimension(dimensionName, text)).GetText().Replace("\r\n", "");
        }
        public List<string> GetVotesFromCardsByDimension(string dimensionName)
        {
            var listCount = Wait.UntilAllElementsLocated(CardByDimension(dimensionName)).Count;
            var votes = new List<string>();
            for (var i = 1; i <= listCount; i++)
            {
                votes.Add(Wait.UntilElementExists(VoteCountByDimensionAndCardCount(dimensionName, i)).GetText());
            }
            return votes;
        }
        public void ClickOnDeleteIconByDimension(string dimensionName, string text = null)
        {
            Log.Step(nameof(IdeaboardPage), "Click delete icon");
            Wait.UntilElementClickable(DeleteIconByCardTextAndDimension(dimensionName, text)).Click();
        }
        public void ClickOnDeleteButtonOfDeletePopup(string dimensionName, string text, GrowthItemsType growthItemType)
        {
            Log.Step(nameof(IdeaboardPage), "Click on delete icon to delete specific card with growth item");
            Wait.UntilElementClickable(DeleteCardIconByDimensionAndGrowthItemType(dimensionName, text, growthItemType.GetDescription())).Click();
            Wait.UntilElementClickable(DeletePopupDeleteButton).Click();
        }
        public void ClickOnCancelButtonOfDeletePopup(string dimensionName, string text, GrowthItemsType growthItemType)
        {
            Log.Step(nameof(IdeaboardPage), "Click on cancel button from delete card popup");
            Wait.UntilElementClickable(DeleteCardIconByDimensionAndGrowthItemType(dimensionName, text, growthItemType.GetDescription())).Click();
            Wait.UntilElementClickable(DeletePopupCancelButton).Click();
        }
        public void ClickOnMergeButtonOfMergePopup()
        {
            Log.Step(nameof(IdeaboardPage), "Click on merge button from merge card popup");
            Wait.UntilElementClickable(MergePopupMergeButton).Click();
            Wait.UntilElementNotExist(CombineCardNotificationToastText);
            Wait.UntilElementNotExist(DeletingCardNotificationToastText);
            Wait.UntilElementNotExist(MergeSuccessNotificationToastText);
        }
        public void ClickOnCancelButtonOfMergePopup()
        {
            Log.Step(nameof(IdeaboardPage), "Click on cancel button from merge card popup");
            Wait.UntilElementClickable(MergePopupCancelButton).Click();
            Wait.HardWait(2000); //Waiting to merge functionality to merge the data
        }
        public string GetTextFromCannotMergePopup()
        {
            Log.Step(nameof(IdeaboardPage), "Get the text from Cannot merge popup");
            return Wait.UntilElementExists(CannotMergeDialogBox).GetText();
        }
        public void ClickOnOkayButtonOfCannotMergePopup()
        {
            Log.Step(nameof(IdeaboardPage), "Click on Okay from Cannot merge popup");
            Wait.UntilElementClickable(CannotMergePopupOkayButton).Click();
        }

        public void ClickOnResetButtonOfResetPopup()
        {
            Log.Step(nameof(IdeaboardPage), "Click on Reset button from reset popup");
            Wait.UntilElementClickable(ResetPopupResetButton).Click();
            Wait.UntilElementExists(SuccessfullyResettingVotesToastText);
            Wait.UntilElementNotExist(SuccessfullyResetVotesToastText);
            Wait.HardWait(5000); // Wait until votes gets reset.
        }

        public void ClickOnCancelButtonOfResetPopup()
        {
            Log.Step(nameof(IdeaboardPage), "Click on Cancel button from reset popup");
            Wait.UntilElementClickable(ResetPopupCancelButton).Click();
        }

        public bool DoesResetVotesButtonExist()
        {
            Log.Step(nameof(IdeaboardPage), $"Verify ResetVotes Button exists");
            return Driver.IsElementDisplayed(ResetVotesButton);
        }

        public bool DoesCardExistByTextAndDimension(string text, string dimensionName)
        {
            Log.Step(nameof(IdeaboardPage), $"Verify {text} in dimension - {dimensionName} exists");
            return Driver.IsElementPresent(CardByDimension(dimensionName, text));
        }
        public void ClickOnVoteUpIconByDimensionAndText(string dimensionName, int clicks, string text)
        {
            Log.Step(nameof(IdeaboardPage), "Click on VoteUp icon for Specific card");
            for (var i = 0; i < clicks; i++)
            {
                Wait.UntilElementVisible(VoteUpIconByCardTextAndDimension(dimensionName, text));
                Wait.UntilElementClickable(VoteUpIconByCardTextAndDimension(dimensionName, text)).Click();
            }
            Wait.UntilElementNotExist(UpdatingToolTip);
        }
        public void ClickOnRemoveVoteIconByDimensionAndText(string dimensionName, string text, int clicks)
        {
            Log.Step(nameof(IdeaboardPage), "Click on Remove Vote icon for Specific card");
            for (var i = 0; i < clicks; i++)
            {
                Wait.UntilElementVisible(RemoveVoteIconForCardByCardTextAndDimension(dimensionName, text));
                Wait.UntilElementClickable(RemoveVoteIconForCardByCardTextAndDimension(dimensionName, text)).Click();
            }
            Wait.UntilElementNotExist(UpdatingToolTip);
        }
        public int GetTotalVotesByUser(string dimensionName, string cardText)
        {
            Log.Step(nameof(IdeaboardPage), $"Get number of votes by user from a {dimensionName}");
            return Wait.UntilAllElementsLocated(VotesByUser(dimensionName, cardText)).Count;
        }
        public string GetNumberOfVotesByCardTextAndDimension(string dimensionName, string text)
        {
            Log.Step(nameof(IdeaboardPage), $"Get number of votes from a {dimensionName}");
            return Wait.UntilElementVisible(VoteCountForCardByCardTextAndDimension(dimensionName, text)).GetText();
        }
        public void ClickOnGrowthItemIconByDimension(string dimensionName, string text, GrowthItemsType growthItemType)
        {
            Log.Step(nameof(IdeaboardPage), "Click on Growth Item icon");
            Wait.UntilElementClickable(GrowthItemIconByCardTextAndDimension(dimensionName, text, growthItemType.GetDescription())).Click();
            Wait.HardWait(5000);// Wait till the time GI selected properly
        }
        public void DragAndDropCard(string dimensionFrom, string dimensionTo, string text, int xOffset, int yOffset)
        {
            Log.Step(nameof(IdeaboardPage), $"Move card from <{dimensionFrom}> column to <{dimensionTo}> column");
            Wait.UntilElementClickable(CardByDimension(dimensionFrom));
            var card1 = Driver.JavaScriptScrollToElement(CardByDimensionDraggableArea(dimensionFrom, text));
            var card2 = Driver.JavaScriptScrollToElement(CardAreaByDimension(dimensionTo));
            Driver.DragElementToElement(card1, card2, xOffset, yOffset);
            Wait.HardWait(2500); //Wait till get text of can not merge popup
        }
        public void WaitUntilIdeaboardLoaded()
        {
            Log.Step(nameof(IdeaboardPage), "Wait until ideaboard page is loaded");
            Wait.UntilElementVisible(SuccessfullyRetrievedVotesAllowedToastText);
        }
        public void WaitForIdeaboardToLoad()
        {
            Wait.UntilElementNotExist(LoadingSpinner);
            Wait.UntilElementVisible(NoteDimensionHeader);
        }
        public void HoverOverToVoteIcon(string dimensionName, string text)
        {
            Log.Step(nameof(IdeaboardPage), "Hover over vote icon ");
            Driver.MoveToElement(Wait.UntilElementVisible(VoteUpIconByCardTextAndDimension(dimensionName, text)));
        }
        public void HoverOverToGrowthItemIcon(string dimensionName, string text, GrowthItemsType growthItemType)
        {
            Log.Step(nameof(IdeaboardPage), "Hover over growth item icon ");
            Driver.MoveToElement(Wait.UntilElementVisible(GrowthItemIconByCardTextAndDimension(dimensionName, text, growthItemType.GetDescription())));
        }
        public void HoverOverToDeleteIcon(string dimensionName, string text)
        {
            Log.Step(nameof(IdeaboardPage), "Hover over delete icon ");
            Driver.MoveToElement(Wait.UntilElementVisible(DeleteIconByCardTextAndDimension(dimensionName, text)));
        }
        public string GetToolTipText()
        {
            Log.Step(nameof(IdeaboardPage), "Get tool tip text");
            return Wait.UntilElementVisible(ToolTip).GetText();
        }
        public void NavigateToIdeaboardPage(int companyId, Guid assessmentGuid)
        {
            NavigateToUrl($"{BaseTest.ApplicationUrl}/v2/ideaboard/company/{companyId}/assessment/{assessmentGuid}");
        }

        // Methods for disabled icons
        public void ClickOnVoteUpIconByDimension(string dimensionName, int clicks)
        {
            Log.Step(nameof(IdeaboardPage), "Click on VoteUp icon");
            for (var i = 0; i < clicks; i++)
            {
                Wait.UntilElementClickable(VoteUpIconByDimension(dimensionName)).Click();
            }
            Wait.UntilElementNotExist(UpdatingToolTip);
        }
        public string GetNumberOfVotesByDimension(string dimensionName)
        {
            Log.Step(nameof(IdeaboardPage), $"Get number of votes from a {dimensionName}");
            return Wait.UntilElementVisible(VoteCountForCardByDimension(dimensionName)).Text;
        }
        public string GetGrowthItemIconColor(GrowthItemsType growthItemType)
        {
            Log.Step(nameof(IdeaboardPage), "Get color of growth item icon after clicking on it");
            return growthItemType switch
            {
                GrowthItemsType.IndividualGrowthItem => Wait.UntilElementVisible(IndividualGrowthItemIcon).GetCssValue("color"),
                GrowthItemsType.TeamGrowthItem => Wait.UntilElementVisible(TeamGrowthItemIcon).GetCssValue("color"),
                GrowthItemsType.OrganizationalGrowthItem => Wait.UntilElementVisible(OrganizationalGrowthItemIcon).GetCssValue("color"),
                GrowthItemsType.ManagementGrowthItem => Wait.UntilElementVisible(ManageGrowthItemIcon).GetCssValue("color"),
                _ => throw new Exception("Unknown value")
            };
        }
        public string ClickOnGrowthItemIconAndGetColor(GrowthItemsType type)
        {
            Log.Step(nameof(IdeaboardPage), "Click on growth item and get the color of it");

            switch (type)
            {
                case GrowthItemsType.IndividualGrowthItem:
                    Wait.UntilElementClickable(IndividualGrowthItemIcon).Click();
                    break;
                case GrowthItemsType.TeamGrowthItem:
                    Wait.UntilElementClickable(TeamGrowthItemIcon).Click();
                    break;
                case GrowthItemsType.OrganizationalGrowthItem:
                    Wait.UntilElementClickable(OrganizationalGrowthItemIcon).Click();
                    break;
                case GrowthItemsType.ManagementGrowthItem:
                    Wait.UntilElementClickable(ManageGrowthItemIcon).Click();
                    break;
                default:
                    throw new Exception("Unknown value");
            }
            return GetGrowthItemIconColor(type);
        }

        public void EnterGrowthItemInfo(GrowthItem growthItem, bool deleteCompetency = true)
        {
            Log.Step(nameof(IdeaboardPage), "Enter growth item info");

            //Competency
            Wait.HardWait(3000);//Takes time to load on automation
            SelectCompetencyTargets(growthItem.CompetencyTargets);

            //Title
            Wait.HardWait(3000);//Takes time to load on automation
            Wait.UntilElementClickable(GiTitle).Click();
            Wait.UntilElementClickable(GiTitle).SetText(growthItem.Title);

            // Category
            Wait.HardWait(4000);//Takes time to load on automation
            SelectItem(GiCategoryDropdown, GiCategoryDropdownValues(growthItem.Category));

            //Priority
            Wait.HardWait(3000);//Takes time to load on automation
            SelectItem(GiPriorityDropdown, GiPriorityDropdownValues(growthItem.Priority));

            //Status
            Wait.HardWait(3000);//Takes time to load on automation
            SelectItem(GiStatusDropdown, GiStatusDropdownValues(growthItem.Status));

            Wait.UntilElementClickable(GiCreateButton).Click();
            Wait.HardWait(4000);//Saving GI takes little time
        }

        public void SelectCompetencyTargets(List<string> competencies)
        {
            // Open the dropdown

            Wait.UntilElementClickable(GiCompetencyDropdown).Click();

            // Loop through each competency to select
            foreach (var competency in competencies)
            {
                try
                {
                    var checkbox = Wait.UntilElementClickable(GiCompetencyDropdownValues(competency));
                    // If not already selected, click it
                    if (!checkbox.Selected)
                    {
                        checkbox.Click();
                    }
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"Competency '{competency}' not found.");
                }
            }
            Wait.UntilJavaScriptReady();
            new Actions(Driver).SendKeys(Keys.Escape).Perform();
        }

        //Navigation
        public void NavigateToTeamAssessmentIdeaboardPageForProd(string env, int companyId, string assessmentId)
        {
            NavigateToUrl($"https://{env}.agilityinsights.ai/v2/ideaboard/company/{companyId}/assessment/{assessmentId}");
            Wait.HardWait(5000);//IdeaBoard Takes Time To Load
        }
    }
    public enum GrowthItemsType
    {
        [Description("teamGrowthItem")]
        TeamGrowthItem,
        [Description("individualGrowthItem")]
        IndividualGrowthItem,
        [Description("orgGrowthItem")]
        OrganizationalGrowthItem,
        [Description("manageGrowthItem")]
        ManagementGrowthItem,
    }
}
