using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company.Add
{
    [TestClass]
    [TestCategory("Companies")]
    public class CompanyDashboardAddCompanyPage1Tests5 : BaseTest
    {

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public void CompanyDashboard_AddCompany_Page1_MandatoryField_Validation_Message()
        {
            var login = new LoginPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompanyPage1 = new AddCompany1CompanyProfilePage(Driver, Log);
            var expectedRequiredFieldList = new List<string> { "Company Name", "Country of Headquarters", "Company Size", "Industry", "Preferred Timezone", "Life Cycle Stage", "First Name", "Last Name", "Email", "Company Type" };

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            Log.Info("Go to 'Company Dashboard' page and click on 'Add a Company' Button");
            companyDashboardPage.WaitUntilLoaded();

            companyDashboardPage.ClickOnAddCompanyButton();
            addCompanyPage1.WaitUntilLoaded();

            Log.Info("Click 'Next' Button on 'Company Profile' page and verify the 'Validation' messages");
            addCompanyPage1.ClickNextButton();
            if (!User.IsSiteAdmin()) expectedRequiredFieldList.Remove("Company Type");
            foreach (var requiredField in expectedRequiredFieldList)
            {
                Assert.AreEqual("Required", addCompanyPage1.GetFieldValidationMessage(requiredField), $"Validation message is not displayed for '{requiredField}' field");
            }
        }
    }
}