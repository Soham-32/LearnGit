using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage3Tests4 : BaseTest
    {
        private static AddCompanyRequest _companyRequest;

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page3_MandatoryField_Validation_Message()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompanyPage2 = new AddCompany2RadarSelectionPage(Driver, Log);
            var addCompanyPage3 = new AddCompany3SubscriptionPage(Driver, Log);
            var expectedRequiredFieldList = new List<string> { "Subscription Type" };

            Log.Info("Go to 'Company Dashboard' page and click on 'Add a Company' Button");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            Log.Info("Enter Company Profile Information on 'Company Profile' page and click on 'Next' Button");
            _companyRequest = CompanyFactory.GetCompany("ZZZ_Add8UI", User.CompanyName);
            addCompanyPage1.FillInCompanyProfileInfo(_companyRequest);
            if (User.IsSiteAdmin())
            {
                addCompanyPage1.FillInAdminInfo(_companyRequest);
            }

            addCompanyPage1.ClickNextButton();
            addCompanyPage2.WaitUntilLoaded();

            Log.Info($"Select {SharedConstants.TeamHealthRadarName} on 'Radar Selection' page and click on 'Next' Button");
            const string radarName = SharedConstants.TeamHealthRadarName;
            addCompanyPage2.SelectRadar(radarName);
            addCompanyPage2.ClickNextButton();
            addCompanyPage3.WaitUntilLoaded();

            Log.Info("Click 'Next' Button on 'Subscription' page and verify the 'Validation' message");
            addCompanyPage3.ClickNextButton();

            foreach (var requiredField in expectedRequiredFieldList)
            {
                Assert.AreEqual("Required", addCompanyPage3.GetFieldValidationMessage(requiredField), $"Validation message is not displayed for '{requiredField}' field");
            }
        }
    }
}