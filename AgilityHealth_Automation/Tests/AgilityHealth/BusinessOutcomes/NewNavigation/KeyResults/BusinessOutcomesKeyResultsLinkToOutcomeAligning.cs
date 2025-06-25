using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomesKeyResultsLinkToOutcomeAligning : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _strategicCardResponse;
        private static BusinessOutcomeResponse _annualBusinessOutcomeResponse;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _strategicCardResponse = CreateBusinessOutcome(SwimlaneType.StrategicIntent, 1);
            _annualBusinessOutcomeResponse = CreateBusinessOutcome(SwimlaneType.StrategicTheme, 1);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        [TestCategory("KnownDefect")] //Bug Id: 51927
        public void BusinessOutcomes_Add_LinkAnnualKrToStrategic()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var annualBusinessOutcomeCardPage = new BusinessOutcomeCardPage(Driver, Log);
            var strategicCardPage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Login to Business outcome dashboard, add and verify key results");
            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            Log.Info("Navigate to BusinessOutcome CardType and verify the Card is linked to Parent Strategic Cardtype");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_annualBusinessOutcomeResponse.Title);

            Log.Info("Click on Link Key Results to link Parent key Results with Child card and verify the same");
            annualBusinessOutcomeCardPage.KeyResultsTab.ClickOnKeyResultsMoreButton(_annualBusinessOutcomeResponse.KeyResults.First().Title);
            annualBusinessOutcomeCardPage.KeyResultsTab.ClickOnActionLinkToOutcomeButton();
            Assert.IsTrue(annualBusinessOutcomeCardPage.KeyResultsTab.IsCancelButtonDisplayed(), "Cancel Button is not displayed");

            annualBusinessOutcomeCardPage.KeyResultsTab.SelectParentOutcome(_strategicCardResponse.Title);
            annualBusinessOutcomeCardPage.KeyResultsTab.SelectParentOutcomeKeyResults(_strategicCardResponse.KeyResults.Select(a => a.Title).ToList());
            Assert.AreEqual(annualBusinessOutcomeCardPage.KeyResultsTab.GetKeyResultLinkToOutcomeAligningContributingConfirmationPopupTitle(), "Aligning Confirmation", "Key Result Aligning/Contributing Title doesn't match");
            Assert.AreEqual(annualBusinessOutcomeCardPage.KeyResultsTab.GetKeyResultLinkToOutcomeAligningContributingConfirmationPopupDescription(), "Are you sure you want to align the key results with the parent outcome? \n\nThis action will replace the existing targets, objectives, and related values with those from the parent key result. \n\nAll the contributing children of the updated KR will get updated to aligning too. \n\nLearn More", "Key Result Popup Description Is Incorrect");
            
            
            annualBusinessOutcomeCardPage.KeyResultsTab.ClickOnConfirmButton();

            Assert.IsTrue(annualBusinessOutcomeCardPage.KeyResultsTab.AreKeyResultFieldsDisabled(true),
                "Key result fields are not Disabled ");

            annualBusinessOutcomeCardPage.ClickOnSaveAndCloseButton();
           Log.Info("Click on Parent Card and verify the Linked Child Card");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_strategicCardResponse.Title);

            var strategicKeyResult = strategicCardPage.KeyResultsTab.GetKeyResult();
            Assert.AreEqual(_strategicCardResponse.KeyResults[0].Title, strategicCardPage.KeyResultsTab.GetKeyResultTitleText(1), "Parent Outcome does not match");
            Assert.AreEqual(_strategicCardResponse.KeyResults.Count, strategicKeyResult.Count,
                "Key Results count does not match");
            Assert.That.ListsAreEqual(_strategicCardResponse.KeyResults.Select(a => a.Title).ToList(), strategicKeyResult.Select(a => a.Title).ToList(), "KRs does not match");
            Assert.That.ListsAreEqual(_strategicCardResponse.KeyResults.Select(a => a.Start).ToList(), strategicKeyResult.Select(a => a.Start).ToList(), "Start value does not match");
            Assert.That.ListsAreEqual(_strategicCardResponse.KeyResults.Select(a => a.Target).ToList(), strategicKeyResult.Select(a => a.Target).ToList(), "Target value does not match");


            var expectedLinkedAnnualKeyResultColumn = new List<string> { "Linked Key Results", "Relationship", "Metric", "Card Type", "Card Title", "Team", "Start", "Target","Current", "Progress" };
            annualBusinessOutcomeCardPage.KeyResultsTab.ClickOnKeyResultExpandArrowIcon(1);
            Assert.That.ListsAreEqual(annualBusinessOutcomeCardPage.KeyResultsTab.GetLinkedKeyResultsColumnName(), expectedLinkedAnnualKeyResultColumn, "Target value does not match");
            var actualLinkedKeyResultValues = annualBusinessOutcomeCardPage.KeyResultsTab.GetLinkedKeyResults();

            Assert.AreEqual(actualLinkedKeyResultValues.Count, _annualBusinessOutcomeResponse.KeyResults.Count, "Added Key Results are not present");
            var expectedOverallPercentage =strategicCardPage.KeyResultsTab.GetOverallPercentageInfo();
            for (var i = 0; i < _annualBusinessOutcomeResponse.KeyResults.Count; i++)
            {
                Assert.AreEqual(actualLinkedKeyResultValues[i].LinkedKeyResultsTitle, _annualBusinessOutcomeResponse.KeyResults[i].Title, $"Linked Key result title doesn't match for {i + 1}th row");
                Assert.AreEqual(actualLinkedKeyResultValues[i].Relationship, "keyResultsDetails.aligning", "Relationship Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].Metric,$"{_annualBusinessOutcomeResponse.KeyResults[i].Metric.Name} (#)" , "Metric Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].CardType,"Business Outcome", "CardType Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].CardTitle, _annualBusinessOutcomeResponse.Title, "Card Title Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].Start, _annualBusinessOutcomeResponse.KeyResults[i].Start, $"Key Result Start Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].Target, _annualBusinessOutcomeResponse.KeyResults[i].Target, "Key Result Target Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].Progress, $"{_strategicCardResponse.KeyResults[i].Progress}", "Key Result Target Doesn't match");
                Assert.AreEqual(actualLinkedKeyResultValues[i].ProgressBar, $"{_strategicCardResponse.KeyResults[i].Progress}%");
            }

            Assert.IsTrue(strategicCardPage.KeyResultsTab.GetOverallPercentageInfo().Contains(expectedOverallPercentage), "Overall progress percentage doesn't match");

            Log.Info("Click on Parent key results link icon and parent outcome link to verify navigation to Child outcome");
            strategicCardPage.KeyResultsTab.ClickOnLinkedKeyResultName(_annualBusinessOutcomeResponse.KeyResults
                .FirstOrDefault()?.Title);

            Driver.SwitchToLastWindow();

            Log.Info("CVerify Linked Parent Key Result navigating to Parent Card");
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.AreEqual(_annualBusinessOutcomeResponse.Title, annualBusinessOutcomeCardPage.GetTitleText(), "BO card title does not match");
            Assert.AreEqual($"Aligning - {_strategicCardResponse.PrettyId} - {_strategicCardResponse.Title}", annualBusinessOutcomeCardPage.KeyResultsTab.GetLinkedKeyResultTooltipText(1), "BO card title does not match");

            annualBusinessOutcomeCardPage.KeyResultsTab.ClickLinkedKeyResult(1);
            Driver.SwitchToLastWindow();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            Assert.AreEqual(_annualBusinessOutcomeResponse.Title, annualBusinessOutcomeCardPage.GetTitleText(), "BO card title does not match");
        }
    }
}