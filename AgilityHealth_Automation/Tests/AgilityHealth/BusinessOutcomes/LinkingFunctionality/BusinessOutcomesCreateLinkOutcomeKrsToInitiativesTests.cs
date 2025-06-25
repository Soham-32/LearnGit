using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.LinkingFunctionality
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesCreateLinkOutcomeKrsToInitiativesTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _parentCardResponse;
        private static BusinessOutcomeResponse _initiativesCardResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _parentCardResponse = CreateBusinessOutcome(SwimlaneType.StrategicIntent, 2);

            var setupApi = new SetupTeardownApi(TestEnvironment);
            var labels = setupApi.GetBusinessOutcomesAllLabels(Company.Id);
            var initiative = labels
                .First(a => a.Name == BusinessOutcomesCardTypeTags.AnnualView.GetDescription()).Tags;

            _initiativesCardResponse = CreateBusinessOutcome(SwimlaneType.Initiatives, 0, initiative);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] // Bug Id: 47821
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_Add_LinkOutcomeKrsToInitiatives() 
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);
            var initiativeCardPage = new InitiativeCardPage(Driver, Log);
            var businessOutcomeBasePage = new BusinessOutcomeBasePage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Login to Business outcome dashboard, add and verify key results");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomeBasePage.CardSearch(_parentCardResponse.Title);
            businessOutcomesDashboard.ClickOnCardExpandCollapseButton(_parentCardResponse.Title);
            _parentCardResponse.KeyResults.ForEach(kr => Assert.AreEqual($"{kr.Progress}%",
                businessOutcomesDashboard.GetKeyResultPercentageValue(kr.Title),
                $"Key Result - {kr.Title}  percentage doesn't match"));

            Log.Info("Navigate to Initiatives CardType and verify that last tag is not removing");
            businessOutcomesDashboard.SelectCardType("Initiatives");
            businessOutcomesDashboard.SelectCardTypeFromDropdown("Annual View");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_initiativesCardResponse.Title);

            initiativeCardPage.ClickOnAdditionalDetail();
            initiativeCardPage.AdditionalDetails.RemoveSelectedTag(initiativeCardPage.AdditionalDetails.GetSelectedTag());

            Assert.AreEqual("The last tag cannot be deleted. Please ensure that at least one tag is retained for this card.",
                initiativeCardPage.AdditionalDetails.GetLastTagCannotBeDeletedToasterMessage(), "Toaster message doesn't exist");

            initiativeCardPage.ClickOnAdditionalDetail(false);
            initiativeCardPage.ClickOnTab("Key Results");

            Log.Info("Click on Link Key Results to link Parent key Results with Child card and verify the same");
            initiativeCardPage.KeyResultsTab.ClickOnLinkKeyResultsButton();
            initiativeCardPage.KeyResultsTab.ClickOnLinkKeyResultPopupCancelButton();
            Assert.IsFalse(initiativeCardPage.KeyResultsTab.IsKeyResultDisplayed(), "Key Result does display");

            initiativeCardPage.KeyResultsTab.ClickOnLinkKeyResultsButton();
            initiativeCardPage.KeyResultsTab.SelectParentOutcome(_parentCardResponse.Title);
            initiativeCardPage.KeyResultsTab.SelectParentOutcomeKeyResults(_parentCardResponse.KeyResults.Select(a => a.Title).ToList());
            Assert.AreEqual("2 Key Results Linked Successfully", initiativeCardPage.KeyResultsTab.GetKeyResultsLinkedSuccessfullyToasterMessage(), "Toaster message doe not match");

            initiativeCardPage.ClickOnSaveButton();
            //Bug - 47344 - Added With Refresh as Card is not linked to the parent 
            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely("2024");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_initiativesCardResponse.Title);
            initiativeCardPage.ClickOnTab("Key Results");
            var actualInitiativeCard = initiativeCardPage.GetBusinessOutcomeInfo();
            Assert.AreEqual(_parentCardResponse.Title, initiativeCardPage.GetParentOutcomeLinkName(), "Parent Outcome does not match");
            Assert.AreEqual(_parentCardResponse.KeyResults.Count, actualInitiativeCard.KeyResults.Count,
                "Key Results count does not match");
            Assert.That.ListsAreEqual(_parentCardResponse.KeyResults.Select(a => a.Title).ToList(), actualInitiativeCard.KeyResults.Select(a => a.Title).ToList(), "KRs does not match");
            Assert.That.ListsAreEqual(_parentCardResponse.KeyResults.Select(a => a.Start).ToList(), actualInitiativeCard.KeyResults.Select(a => a.Start).ToList(), "Start value does not match");
            Assert.That.ListsAreEqual(_parentCardResponse.KeyResults.Select(a => a.Target).ToList(), actualInitiativeCard.KeyResults.Select(a => a.Target).ToList(), "Target value does not match");

            Log.Info("Click on Parent key results link icon and parent outcome link to verify navigation to parent outcome");
            initiativeCardPage.KeyResultsTab.ClickOnLinkedKrLinkedIcon(_parentCardResponse.KeyResults.First().Title);
            Driver.SwitchToLastWindow();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.AreEqual(_parentCardResponse.Title, addBusinessOutcomePage.GetTitleText(), "BO card title does not match");

            Driver.Close();
            Driver.SwitchToFirstWindow();
            
            initiativeCardPage.ClickOnParentOutcomeLink();
            Driver.SwitchToLastWindow();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.AreEqual(_parentCardResponse.Title, addBusinessOutcomePage.GetTitleText(), "BO card title does not match");

            Log.Info("Click on KeyResult expand icon and Initiative tab to verify that parent outcome is showing, Click on Project outcome to navigate to child card");
            addBusinessOutcomePage.KeyResultsTab.ClickOnKeyResultExpandIcon(_parentCardResponse.KeyResults.First().Title);
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsInitiativeCardNamePresent(_initiativesCardResponse.Title), "Child card name does not display under LinkedKrs");

            addBusinessOutcomePage.ClickOnTab("Initiatives");
            Assert.IsTrue(addBusinessOutcomePage.InitiativesTab.IsProjectOutcomeDisplayed(_initiativesCardResponse.Title),"Initiative card does not display");

            addBusinessOutcomePage.InitiativesTab.ClickOnProjectOutcomeNameLink(_initiativesCardResponse.Title);
            Driver.SelectWindowByIndex(2);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely("2024");
            Assert.AreEqual(_initiativesCardResponse.Title, initiativeCardPage.GetTitleText(), "BO card title does not match");
            
            Driver.Close();
            Driver.SwitchToFirstWindow();

            Log.Info("Unlink the key result and verify that Child KR is not showing under Parent KR");
            initiativeCardPage.KeyResultsTab.ClickOnUnlinkKeyResultButton(actualInitiativeCard.KeyResults.First().Title);
            Assert.IsTrue(initiativeCardPage.KeyResultsTab.IsKeyResultTitleDisplayed(actualInitiativeCard.KeyResults.First().Title), "Key Result title does not display on popup");
            initiativeCardPage.KeyResultsTab.ClickOnConfirmPopupUnlinkButton();
            Assert.IsFalse(initiativeCardPage.KeyResultsTab.IsKeyResultLinkedIconDisplayed(actualInitiativeCard.KeyResults.First().Title), "KeyResult Linked icon does display");

            addBusinessOutcomePage.ClickOnSaveButton();
            Driver.SwitchToLastWindow();
            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsKeyResultsCaretIconDisplayed(actualInitiativeCard.KeyResults.First().Title), "The key result caret icon does display");

            Log.Info("Link the KR again and verify that Child KR is showing under Parent KR");
            Driver.SwitchToFirstWindow();
            initiativeCardPage.KeyResultsTab.ClickOnLinkToOutcomeKeyResultButton(actualInitiativeCard.KeyResults.First().Title);
            initiativeCardPage.KeyResultsTab.SelectParentOutcome(_parentCardResponse.Title);
            initiativeCardPage.KeyResultsTab.SelectLinkParentOutcomeKeyResult(actualInitiativeCard.KeyResults.First().Title);
            initiativeCardPage.ClickOnConfirmButton();
            Assert.IsTrue(initiativeCardPage.KeyResultsTab.IsKeyResultLinkedIconDisplayed(actualInitiativeCard.KeyResults.First().Title), "KeyResult Linked icon does not display");

            initiativeCardPage.ClickOnSaveButton();
            Driver.SwitchToLastWindow();
            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            addBusinessOutcomePage.KeyResultsTab.ClickOnKeyResultExpandIcon(actualInitiativeCard.KeyResults.First().Title);
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsInitiativeCardNamePresent(actualInitiativeCard.Title), "Child card name does not display under LinkedKrs");
        }
    }
}