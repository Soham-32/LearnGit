using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageBusinessOutcomes;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageBusinessOutcomes
{
    [TestClass]
    [TestCategory("ManageBusinessOutcomes")]
    public class BusinessOutcomesDashboardSettingsEditLabelsTests : BusinessOutcomesBaseTest
    {
        private static User _user;
        private static AddCompanyRequest _companyRequest;
        private static CompanyResponse _companyResponse;
        private static readonly UserConfig CompanyAdminUserConfig = new UserConfig("SA");

        [ClassInitialize]
        public static void ClassSetUp(TestContext testContext)
        {
            _user = User;
            if (User.IsCompanyAdmin())
            {
                _user = CompanyAdminUserConfig.GetUserByDescription("user 1");
            }

            var setupUi = new SetUpMethods(testContext, TestEnvironment);
            _companyRequest = CompanyFactory.GetCompany("ZZZ_BO");
            _companyResponse = setupUi.CreateCompanyAndCompanyAdmin(_companyRequest, _user);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Settings_Dashboard_Edit_Labels()
        {
            var login = new LoginPage(Driver, Log);
            var v2SettingsPage = new SettingsV2Page(Driver, Log);
            var boDashboardSettingsPage = new BusinessOutcomesDashboardSettingsPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            var listToUpdateLabels = new List<string>() { "Strategic", "Annual", "1/4", "Project", "Sprints" };
            var user = User;

            login.NavigateToPage();

            if (User.IsCompanyAdmin())
            {
                user.Username = _companyRequest.CompanyAdminEmail;
                user.Password = SharedConstants.CommonPassword;
            }

            login.LoginToApplication(user.Username, user.Password);

            Log.Info("Navigate to manage business outcome dashboard setting and change card type label's name");
            v2SettingsPage.NavigateToPage(_companyResponse.Id);
            v2SettingsPage.ClickOnBusinessOutcomesManageSettingsButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();

            //Edit Outcome labels
            boDashboardSettingsPage.UpdateOutcomesLabel(SwimlaneType.StrategicIntent.GetDescription(), listToUpdateLabels[0]);
            boDashboardSettingsPage.UpdateOutcomesLabel(SwimlaneType.StrategicTheme.GetDescription().Replace("s",""), listToUpdateLabels[1]);
            boDashboardSettingsPage.UpdateOutcomesLabel(SwimlaneType.QuarterlyObjective.GetDescription(), listToUpdateLabels[2]);
            boDashboardSettingsPage.UpdateInitiativesAndDeliverableLabels(BusinessOutcomesCardType.AnnualView.GetDescription(), listToUpdateLabels[3]);
            boDashboardSettingsPage.UpdateInitiativesAndDeliverableLabels(BusinessOutcomesCardType.DeliverablesTimeline.GetDescription(), listToUpdateLabels[4]);

            boDashboardSettingsPage.ClickOnSaveButton();
            boDashboardSettingsPage.ClickOnManageBusinessOutcomesDashboardSettingsButton();

            var outcomeLabels = boDashboardSettingsPage.GetListOfOutcomesLabels();

            foreach (var label in outcomeLabels)
            {
                Assert.That.ListContains(listToUpdateLabels, label, $"{label} does not exists");
            }

            Assert.IsTrue(boDashboardSettingsPage.IsCardTypePresent(listToUpdateLabels[3]), $"{listToUpdateLabels[3]}Label does not match");
            Assert.IsTrue(boDashboardSettingsPage.IsCardTypePresent(listToUpdateLabels[4]), $"{listToUpdateLabels[4]}Label does not match");

            Log.Info("Navigate to business outcome dashboard and verify the updated label name");
            boDashboard.NavigateToPage(_companyResponse.Id);

            var annuallyColumns = boDashboard.GetAllColumnNames();

            Assert.That.ListContains(annuallyColumns, listToUpdateLabels[0].ToUpper(), $"{listToUpdateLabels[0]} Column does not match");
            Assert.That.ListContains(annuallyColumns, listToUpdateLabels[1].ToUpper(), $"{listToUpdateLabels[1]} Column does not match");

            boDashboard.TagsViewSelectTag(TimeFrameTags.Quarterly.GetDescription());

            var quarterlyColumns = boDashboard.GetAllColumnNames();

            Assert.That.ListContains(quarterlyColumns, listToUpdateLabels[1].ToUpper(), $"{listToUpdateLabels[1]} Column does not match");
            Assert.That.ListContains(quarterlyColumns, listToUpdateLabels[2].ToUpper(), $"{listToUpdateLabels[2]} Column does not match");

            boDashboard.CardTypeExpandCollapseDropdown();
            var actualTags = boDashboard.CardTypeGetAllTypeList();

            //Default Tag 
            Assert.IsTrue(actualTags.Any(text => text.Equals("Business Outcomes")), "'Business Outcomes' isn't displayed on Tags view");

            //Custom Tags
            Assert.That.ListContains(actualTags, listToUpdateLabels[3], $"{listToUpdateLabels[3]} card type does not match");
            Assert.That.ListContains(actualTags, listToUpdateLabels[4], $"{listToUpdateLabels[4]} card type does not match");

        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyResponse.Name).GetAwaiter().GetResult();
        }
    }

}