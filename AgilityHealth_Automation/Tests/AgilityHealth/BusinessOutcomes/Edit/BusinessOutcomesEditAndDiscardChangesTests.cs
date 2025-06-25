using System.Diagnostics;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
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
    public class BusinessOutcomesEditAndDiscardChangesTests : BusinessOutcomesBaseTest
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
        public void BusinessOutcomes_BusinessOutcome_Edit_DiscardChanges()
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
            addBusinessOutcomePage.ClickOnCloseIcon();
            addBusinessOutcomePage.ConfirmPopUpClickOnDiscardChangesButton();
            
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response.Title), 
                $"Business outcome with title {_response.Title} is not displayed on the dashboard.");
            Assert.AreEqual($"background: {CSharpHelpers.CovertHexToRgb(_response.CardColor)};", 
                businessOutcomesDashboard.GetColorInfoForLeftVerticalStripe(_response.Title), 
                $"Color doesn't match for '{_response.Title} left vertical stripe'");

            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);
            var actual = addBusinessOutcomePage.GetBusinessOutcomeInfo();

            Assert.AreEqual(_response.Title, actual.Title, "Title does not match.");
            Assert.AreEqual(CSharpHelpers.GetNormalizedText(_response.Description), CSharpHelpers.GetNormalizedText(actual.Description), "Description does not match.");
            Assert.AreEqual(_response.CardColor, actual.CardColor, "CardColor does not match.");
            Assert.AreEqual(User.FullName, actual.Owner, "Owner does not match.");
            Debug.Assert(_response.CreatedAt != null, "_response.CreatedAt != null");
            Debug.Assert(actual.CreatedAt != null, "actual.CreatedAt != null");
            Assert.AreEqual(_response.CreatedAt.Value.ToShortDateString(), actual.CreatedAt.Value.ToShortDateString(), "CreatedAt does not match.");
            Assert.AreEqual(User.FullName, actual.UpdatedBy, "UpdatedBy does not match.");
            Assert.AreEqual(_response.Title, actual.Title, "Title does not match.");
            Debug.Assert(actual.UpdatedAt != null, "actual.UpdatedAt != null");
            Debug.Assert(_response.UpdatedAt != null, "_response.UpdatedAt != null");
            Assert.AreEqual(_response.UpdatedAt.Value.ToShortDateString(), actual.UpdatedAt.Value.ToShortDateString(), "UpdatedAt does not match.");
        }
    }
}