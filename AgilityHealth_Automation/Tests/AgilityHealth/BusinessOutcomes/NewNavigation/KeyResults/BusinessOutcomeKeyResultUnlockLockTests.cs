using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.NewNavigation.KeyResults
{
    [TestClass]
    [TestCategory("BusinessOutcomes"), TestCategory("NewNavigation")]
    public class BusinessOutcomeKeyResultUnlockLockTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicTheme);
        }

        
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_KeyResults_Lock_Unlock_Test()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            businessOutcomesDashboard.NavigateToPage(Company.Id);

            Log.Info("Verify the Card's Created Key Results");
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var expectedKeyResults = new List<KeyResultRequest>
                {BusinessOutcomesFactory.GetKeyResultRequest(Company.Id)};

            expectedKeyResults.ForEach(kr => addBusinessOutcomePage.KeyResultsTab.AddKeyResult(kr));

            var actualKeyResults = addBusinessOutcomePage.KeyResultsTab.GetKeyResult();

            Assert.AreEqual(actualKeyResults.Count, expectedKeyResults.Count, "Added Key Results are not present");

            Log.Info("Verify the Card's KeyResult Lock Status");
            var iconState = addBusinessOutcomePage.KeyResultsTab.GetKeyResultIconStatus();
            Assert.AreEqual("UnLocked", iconState, "Key Result Lock status doesn't match");
            Assert.AreEqual(Constants.KeyResultUnlockedTooltip,
                addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockStatusTooltipTitle(),
                "KeyResult Lock is in Locked Status");

            Log.Info("Verify the Card's KeyResult Lock/Unlock popup Title and Description");
            addBusinessOutcomePage.KeyResultsTab.ClickKeyResultLockIcon();

            Assert.AreEqual(addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockUnlockPopupConfirmationTitle(),
                Constants.KeyResultLockConfirmationPopupTitle, "Key Result Lock Confirmation Popup title doesn't match");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsUnlockToLockImageDisplayed(),
                "UnLock to Lock Image is not displayed");
            Assert.AreEqual(addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockUnlockPopupConfirmationDescription(),
                Constants.KeyResultLockConfirmationPopupDescription, "Key Result Lock Confirmation Popup Description doesn't match");

            Log.Info("Verify the Card's KeyResult Lock/Unlock popup Cancel and Confirm Button");
            
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsConfirmButtonDisplayed(), "Confirm button is not displayed");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsCancelButtonDisplayed(), "Cancel button is not displayed");

            Log.Info("Verify the Card's KeyResult Lock/Unlock changes not Saved on Cancel button");
            addBusinessOutcomePage.KeyResultsTab.ClickOnCancelButton();
            Assert.AreEqual("UnLocked", iconState, "Key Result Lock status doesn't match");

            Log.Info("Verify the Card's KeyResult Card is Locked");
            addBusinessOutcomePage.KeyResultsTab.ClickKeyResultLockIcon();

            addBusinessOutcomePage.KeyResultsTab.ClickOnConfirmButton();
            Assert.AreEqual("Locked", addBusinessOutcomePage.KeyResultsTab.GetKeyResultIconStatus(false), "Key Result Lock status doesn't match");
            Assert.AreEqual(Constants.KeyResultLockedTooltip,
                addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockStatusTooltipTitle(),
                "Key Result is not in Locked Status");

            Log.Info("Verify the Card's Key Result fields are Disabled except Key Result Current Field");
            //Bug 51798: Key Result plus icon button should be greyed out when locked
            //Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsKeyResultAddButtonDisabled(),
            //    "Key Result button is enabled");
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsWeightButtonEnabled(), "Weight Icon  is enabled");
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsLinkToOutcomeEnabled(), "Link to Outcome is enabled");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.AreKeyResultFieldsDisabled(),
                "Key result fields are not Disabled ");

            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            Log.Info("Verify the Card's KeyResult is Locked after Save and Close");
            Assert.AreEqual("Locked", addBusinessOutcomePage.KeyResultsTab.GetKeyResultIconStatus(false), "Key Result Lock status doesn't match");
            Assert.AreEqual(Constants.KeyResultLockedTooltip,
                addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockStatusTooltipTitle(),
                "Key Result Icon is Unlocked");
            //Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsKeyResultButtonEnabled(),
            //    "Key Result button is enabled");
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsWeightButtonEnabled(), "Weight Icon  is enabled");
            Assert.IsFalse(addBusinessOutcomePage.KeyResultsTab.IsLinkToOutcomeEnabled(), "Link to Outcome is enabled");

            Log.Info("Verify the Card's KeyResult is Unlocked");
            addBusinessOutcomePage.KeyResultsTab.ClickKeyResultLockIcon(false);
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsLockToUnlockImageDisplayed(),
                "Lock to UnLock Image is not displayed");
            Assert.AreEqual(addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockUnlockPopupConfirmationTitle(),
                Constants.KeyResultUnLockConfirmationPopupTitle, "Key Result Lock Confirmation Popup title doesn't match");
            Assert.AreEqual(addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockUnlockPopupConfirmationDescription(),
                Constants.KeyResultUnLockConfirmationPopupDescription);
            addBusinessOutcomePage.KeyResultsTab.ClickOnConfirmButton();
            Assert.AreEqual("UnLocked", addBusinessOutcomePage.KeyResultsTab.GetKeyResultIconStatus(), "Key Result Lock status doesn't match");

            Assert.AreEqual(Constants.KeyResultUnlockedTooltip,
                addBusinessOutcomePage.KeyResultsTab.GetKeyResultLockStatusTooltipTitle(),
                "KeyResult Lock is in Locked Status");
            //Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsKeyResultButtonEnabled(),
            //    "Key Result button is enabled");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsWeightButtonEnabled(), "Weight Icon  is enabled");
            Assert.IsTrue(addBusinessOutcomePage.KeyResultsTab.IsLinkToOutcomeEnabled(), "Link to Outcome is enabled");

            addBusinessOutcomePage.ClickOnSaveAndCloseButton();

            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();

            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            Assert.AreEqual("UnLocked", addBusinessOutcomePage.KeyResultsTab.GetKeyResultIconStatus(), "Key Result Lock status doesn't match");
        }

    }
}
