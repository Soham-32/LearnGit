using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.ExternalLinkPopup;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.ExternalLink
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesExternalLinkDeleteTests1 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;
        private static BusinessOutcomeLinkResponse _link;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
            _link = new SetupTeardownApi(TestEnvironment).AddBusinessOutcomesExternalLink(_response.Uid);
        }

        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Edit_ExternalLink_Delete()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var externalLinkPopUp = new BusinessOutcomesExternalLinkPopUp(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.ClickOnExternalLinkButton(_response.Title);
            
            externalLinkPopUp.ClickOnDeleteButton(_link.Title);
            externalLinkPopUp.ClickOnSaveButton();
            businessOutcomesDashboard.WaitForReload();

            Assert.AreEqual("0", businessOutcomesDashboard.GetTotalExternalLinksCount(_response.Title), 
                "External link count doesn't match");

        }
    }
}