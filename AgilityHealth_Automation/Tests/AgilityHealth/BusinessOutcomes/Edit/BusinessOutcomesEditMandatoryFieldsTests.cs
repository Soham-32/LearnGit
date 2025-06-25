using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.CardType;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Edit
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesEditMandatoryFieldsTests : BusinessOutcomesBaseTest
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
        public void BusinessOutcomes_BusinessOutcome_Edit_MandatoryFields()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var addBusinessOutcomePage = new BusinessOutcomeCardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnBusinessOutcomeLink(_response.Title);

            addBusinessOutcomePage.EnterTitle("");
            addBusinessOutcomePage.EnterDescription("New Description");

            Assert.IsFalse(addBusinessOutcomePage.IsSaveButtonEnabled(),
                "'Save And Close' button is enabled and can be clicked without adding Title");
            Assert.IsFalse(addBusinessOutcomePage.IsSaveDropdownButtonEnabled(),
                "'Save Dropdown' button is enabled and can be clicked without adding Title");
            Assert.AreEqual("Title is required", addBusinessOutcomePage.GetTitleValidationText(),
                "Title validation text isn't matched'");

            addBusinessOutcomePage.EnterTitle("expected.Title");

            Assert.IsTrue(addBusinessOutcomePage.IsSaveButtonEnabled(),
                "'Save And Close' button is not enabled.");
            Assert.IsTrue(addBusinessOutcomePage.IsSaveDropdownButtonEnabled(),
                "'Save Dropdown' button is not enabled.");

        }
    }
}