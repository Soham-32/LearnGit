using System;
using System.Diagnostics;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Edit
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesEditAndSaveTests : BusinessOutcomesBaseTest
    {
        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_BusinessOutcome_Edit_Save()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            var expected = BusinessOutcomesFactory.GetBusinessOutcomeForUpdate(Company.Id, _response.SwimlaneType);
            addBusinessOutcomePage.FillForm(expected);
            addBusinessOutcomePage.ClickOnSaveAndCloseButton();
            var updatedTime = DateTime.UtcNow;
            
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            addBusinessOutcomePage.CardSearch(expected.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(expected.Title), 
                $"Updated business outcome with title {expected.Title} is not displayed on the dashboard.");
            Assert.AreEqual($"background: {CSharpHelpers.CovertHexToRgb(expected.CardColor)};", 
                businessOutcomesDashboard.GetColorInfoForLeftVerticalStripe(expected.Title), 
                $"Color doesn't match for '{expected.Title} left vertical stripe'");

            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(expected.Title);
            var actual = addBusinessOutcomePage.GetBusinessOutcomeInfo();

            Assert.AreEqual(expected.Title, actual.Title, "Title does not match.");
            Assert.AreEqual(CSharpHelpers.GetNormalizedText(expected.Description), CSharpHelpers.GetNormalizedText(actual.Description), "Description does not match.");
            Assert.AreEqual(expected.CardColor, actual.CardColor, "CardColor does not match.");
            Assert.AreEqual(User.FullName, actual.Owner, "Owner does not match.");
            Debug.Assert(_response.CreatedAt != null, "_response.CreatedAt != null");
            Debug.Assert(actual.CreatedAt != null, "actual.CreatedAt != null");
            Assert.AreEqual(_response.CreatedAt.Value.ToShortDateString(), actual.CreatedAt.Value.ToShortDateString(), "CreatedAt does not match.");
            Assert.AreEqual(User.FullName, actual.UpdatedBy, "UpdatedBy does not match.");
            Assert.AreEqual(expected.Title, actual.Title, "Title does not match.");
            Debug.Assert(actual.UpdatedAt != null, "actual.UpdatedAt != null");
            Assert.AreEqual(updatedTime.ToShortDateString(), actual.UpdatedAt.Value.ToShortDateString(), "UpdatedAt does not match.");
        }
    }
}