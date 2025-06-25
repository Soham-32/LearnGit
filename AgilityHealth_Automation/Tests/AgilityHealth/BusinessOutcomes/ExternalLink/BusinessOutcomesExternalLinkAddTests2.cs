using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.ExternalLinkPopup;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.ExternalLink
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesExternalLinkAddTests2 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;
        private const string Title = "title";
        private const string Url = "url";

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }

        
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Edit_ExternalLink_Add_MandatoryFields()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var externalLinkPopUp = new BusinessOutcomesExternalLinkPopUp(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnExternalLinkButton(_response.Title);

            externalLinkPopUp.ClickOnAddButton();
            var actualTitleValidationMessage = externalLinkPopUp.GetValidationMessage(Title);
            Assert.AreEqual("*Please enter valid Title", actualTitleValidationMessage, "Title validation message doesn't match");

            var actualUrlValidationMessage = externalLinkPopUp.GetValidationMessage(Url);
            Assert.AreEqual("*Please enter a valid Url", actualUrlValidationMessage, "Url validation message doesn't match");
        }
    }
}