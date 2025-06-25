using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.ExternalLinkPopup;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.ExternalLink
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesExternalLinkAddTests1 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            _response = CreateBusinessOutcome(SwimlaneType.StrategicIntent);
        }

        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_BusinessOutcome_Edit_ExternalLink_Add()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var externalLinkPopUp = new BusinessOutcomesExternalLinkPopUp(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            var expected = BusinessOutcomesFactory.GetBusinessOutcomeLinkRequest(Guid.Empty);
            businessOutcomesDashboard.ClickOnExternalLinkButton(_response.Title);

            externalLinkPopUp.AddExternalLink(expected);
            businessOutcomesDashboard.WaitForReload();

            Assert.AreEqual("1", businessOutcomesDashboard.GetTotalExternalLinksCount(_response.Title), 
                "External link count doesn't match");
            //External link fly-out links verification
            businessOutcomesDashboard.MouseHoverOnExternalLinkButton(_response.Title);
            Assert.IsTrue(businessOutcomesDashboard.IsExternalLinkPresent(expected.Title), 
                "External link not present on fly-out");

            businessOutcomesDashboard.ClickOnExternalLink(expected.Title);
            Driver.SwitchToLastWindow();

            Assert.AreEqual(expected.ExternalUrl, Driver.GetCurrentUrl(), 
                "User is navigated to incorrect url");

        }
    }
}