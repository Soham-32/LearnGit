using System.Collections.Generic;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Company.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Edit;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Utilities;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageRadars
{
    [TestClass]
    [TestCategory("Settings")]
    [TestCategory("SiteAdmin")]
    public class ManageRadarDefaultRadarTests : ManageRadarBaseTests 
    {
        private static readonly RadarDetails RadarInfo = ManageRadarFactory.GetValidRadarDetails();
       
        private static AddCompanyRequest _companyRequest, _companyRequest1;
        public string CompanyName = User.CompanyName;

        //Master Radar
        [TestMethod]
        public void ManageRadar_Verify_DefaultRadarFunctionalityForMasterRadar()
        {
            ManageRadar_Verify_DefaultRadar("Master");
        }

        //Company Specific Radar 
        [TestMethod]
        public void ManageRadar_Verify_DefaultRadarFunctionalityForCompanySpecificRadar()
        {
            ManageRadar_Verify_DefaultRadar(CompanyName);
        }

        public void ManageRadar_Verify_DefaultRadar(string companyName)
        {
            var loginPage = new LoginPage(Driver, Log);
            var manageRadarPage = new ManageRadarPage(Driver, Log);
            var addRadarDetailsPage = new AddRadarDetailsPage(Driver, Log);
            var companyDashboardPage = new CompanyDashboardPage(Driver, Log);
            var addCompany1ProfilePage = new AddCompany1CompanyProfilePage(Driver, Log);
            var addCompany2RadarSelectionPage = new AddCompany2RadarSelectionPage(Driver, Log);
            var editRadarSelectionPage = new EditRadarSelectionPage(Driver, Log);
            var editCompanyProfilePage = new EditCompanyProfilePage(Driver, Log);
            var editRadarDetailsPage = new EditRadarDetailsPage(Driver, Log);

            Log.Info("Login to the application and navigate to 'Manage Radar' page.");
            loginPage.NavigateToPage();
            loginPage.LoginToApplication(User.Username, User.Password);
            manageRadarPage.NavigateToPage();

            Log.Info("Click on the 'Create New Radar' button and enter all the radar details with company name");
            manageRadarPage.ClickOnCreateNewRadarButton();
            manageRadarPage.CreateRadarPopupClickOnCreateRadarRadioButton();
            manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();
            addRadarDetailsPage.SelectRadarCompany(companyName);

            Assert.IsTrue(addRadarDetailsPage.IsDefaultRadarFieldDisplayed(), "'Default Radar' checkbox is not displayed even if company name is selected");
            if (companyName == SharedConstants.CompanyName)
            {
                Assert.IsTrue(addRadarDetailsPage.IsLimitToFieldDisplayed(), "'Limit To' dropdown is not displayed");
            }
            else
            {
                Assert.IsFalse(addRadarDetailsPage.IsLimitToFieldDisplayed(), "'Limit Too' dropdown is displayed even if company name is selected");
            }
            addRadarDetailsPage.ClickOnDefaultRadarCheckbox();
            RadarInfo.CompanyName = companyName;
            RadarInfo.LimitedTo = new List<string>() { null };
            addRadarDetailsPage.EnterRadarDetails(RadarInfo);
            addRadarDetailsPage.EnterMessagesTextsDetails(RadarInfo);

            Assert.IsFalse(addRadarDetailsPage.IsLimitToFieldDisplayed(), "'Limit To' dropdown is displayed even if company name is selected");
            addRadarDetailsPage.ClickOnSaveAndContinueButton();

            Log.Info("Edit the created radar and check 'Active' checkbox");
            manageRadarPage.NavigateToPage();
            manageRadarPage.ClickOnRadarEditIcon(RadarInfo.Name);
            addRadarDetailsPage.ClickOnActiveCheckboxButton(true);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Navigate to the company Dashboard and click on the 'Add New' button");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompany1ProfilePage.WaitUntilLoaded();

            Log.Info("Fill the company details and verify that created radar should be displayed");
            _companyRequest = CompanyFactory.GetCompany("ZZZ_DefaultRadarCheck", User.CompanyName);
            addCompany1ProfilePage.FillInCompanyProfileInfo(_companyRequest);
            if (User.IsSiteAdmin())
            {
                addCompany1ProfilePage.FillInAdminInfo(_companyRequest);
            }
            addCompany1ProfilePage.ClickNextButton();
            addCompany2RadarSelectionPage.WaitUntilLoaded();
            Assert.IsTrue(addCompany2RadarSelectionPage.IsRadarPresent(RadarInfo.Name), $"{RadarInfo.Name} radar is present while creating a company");

            Log.Info("Click on the 'Close' icon Edit the 'Automation_CA (DO NOR USE) and select 'Radar Select' tab,Verify that created radar should be displayed");
            editRadarSelectionPage.Header.ClickCloseButton();
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickEditIconByCompanyName(CompanyName);
            editCompanyProfilePage.WaitUntilLoaded();
            editCompanyProfilePage.Header.ClickOnRadarSelectionTab();
            editRadarSelectionPage.WaitUntilLoaded();
            Assert.IsTrue(addCompany2RadarSelectionPage.IsRadarPresent(RadarInfo.Name), $"{RadarInfo.Name} radar is not present while editing a company");

            //UnCheck
            Log.Info("Navigate to the manage radar page and edit the created radar then uncheck the 'Default Radar' checkbox");
            manageRadarPage.NavigateToPage();
            manageRadarPage.ClickOnRadarEditIcon(RadarInfo.Name);
            addRadarDetailsPage.ClickOnDefaultRadarCheckbox(false);
            editRadarDetailsPage.ClickOnUpdateButton();

            Log.Info("Navigate to the Company dashboard page click on the 'Add New' button and fill the company details then verify that created radar should not be displayed");
            companyDashboardPage.NavigateToPage();
            companyDashboardPage.ClickOnAddCompanyButton();
            addCompany1ProfilePage.WaitUntilLoaded();
            _companyRequest1 = CompanyFactory.GetCompany("ZZZ_DefaultRadarUncheck", User.CompanyName);
            addCompany1ProfilePage.FillInCompanyProfileInfo(_companyRequest1);
            if (User.IsSiteAdmin())
            {
                addCompany1ProfilePage.FillInAdminInfo(_companyRequest1);
            }
            addCompany1ProfilePage.ClickNextButton();
            addCompany2RadarSelectionPage.WaitUntilLoaded();
            Assert.IsFalse(addCompany2RadarSelectionPage.IsRadarPresent(RadarInfo.Name), $"{RadarInfo.Name} radar is present while creating a company");

            Log.Info($"Click on the 'Close' icon and edit the {CompanyName} then select the radar selection tab, verify that created radar should not be displayed");
            editRadarSelectionPage.Header.ClickCloseButton();
            companyDashboardPage.WaitUntilLoaded();
            companyDashboardPage.ClickEditIconByCompanyName(CompanyName);
            editCompanyProfilePage.WaitUntilLoaded();
            editCompanyProfilePage.Header.ClickOnRadarSelectionTab();
            editRadarSelectionPage.WaitUntilLoaded();
            Assert.IsFalse(addCompany2RadarSelectionPage.IsRadarPresent(RadarInfo.Name), $"{RadarInfo.Name} radar is present while editing a company");


            //Clean-Up Method
            DeleteRadar(RadarInfo.Name);
        }

    }
}
