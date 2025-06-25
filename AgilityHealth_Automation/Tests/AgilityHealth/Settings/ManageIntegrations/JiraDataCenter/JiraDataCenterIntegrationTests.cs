using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration.JiraDataCenter;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Integrations.Custom.JiraIntegrations.JiraDataCenterIntegration;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration;
using System.Collections.Generic;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageIntegrations.JiraDataCenter
{
    [TestClass]
    [TestCategory("Integration")]
    public class JiraDataCenterIntegrationTests : BaseTest
    {
        private static bool _classInitFailed;
        private static CompanyResponse _companyResponse;
        private static AddCompanyRequest _companyRequest;
        private static SetUpMethods _setupUi;
        private static readonly string JiraDataCenterInfo = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Integration/JiraDataCenterIntegrationInfo.json");
        private static readonly JiraDataCenterIntegration JiraDataCenterAuthInfo = JsonConvert.DeserializeObject<JiraDataCenterIntegration>(JiraDataCenterInfo);

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            try
            {
                _setupUi = new SetUpMethods(_, TestEnvironment);
                _companyRequest = CompanyFactory.GetValidPostCompany();
                _companyRequest.CompanyAdminEmail = Constants.UserEmailPrefix + "_CA_" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain;
                _companyResponse = _setupUi.CreateNewCompanyAndCompanyUser(_companyRequest, User);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] // Bug: 39549
        public void VerifyCompanyIntegrationWithJiraDataCenter()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(Driver, Log);
            var linkTeamPage = new LinkTeamBasePage(Driver, Log);
            var jiraDataCenterLinkTeamPage = new JiraDataCenterLinkTeamPage(Driver, Log);

            Log.Info("Login to the CA user");
            login.NavigateToPage();
            login.LoginToApplication(_companyRequest.CompanyAdminEmail, SharedConstants.CommonPassword);

            Log.Info("Navigate to the 'Add Connection' page and verify that the 'Link' button is present for the 'Jira Data Center'.");
            manageIntegrationsPage.NavigateToPage(_companyResponse.Id);
            manageIntegrationsPage.WaitUntilAddConnectionPageLoaded();
            Assert.IsTrue(manageIntegrationsPage.IsLinkButtonPresent(Constants.PlatformJiraDataCenter), "The 'Link' button is not present for the 'Jira Data Center'.");

            Log.Info("Verify that the validation messages are displayed correctly for all mandatory fields on the 'Jira Cloud Credentials' popup.");
            manageIntegrationsPage.ClickOnLinkButton(Constants.PlatformJiraDataCenter);
            Assert.IsTrue(linkTeamPage.IsAuthenticationPopupPresent(), "The 'Jira Data Center Credentials' popup is not displayed.");
            Assert.AreEqual("Instance Name is Required", linkTeamPage.GetInstanceNameMandatoryValidationMessage(), "The mandatory field validation message is not correct for the 'Instance Name'.");
            Assert.AreEqual("User Name is Required", jiraDataCenterLinkTeamPage.GetUserNameMandatoryValidationMessage(), "The mandatory field validation message is not correct for the 'User Name'.");
            Assert.AreEqual("Password is Required", jiraDataCenterLinkTeamPage.GetPasswordMandatoryValidationMessage(), "The mandatory field validation message is not correct for the 'Password'.");
            Assert.AreEqual("Jira Data Center URL is Required", linkTeamPage.GetUrlMandatoryValidationMessage(), "The mandatory field validation message is not correct for the 'Url'.");

            Log.Info("Verify that when the correct credentials are passed then the instance is integrated with 'Jira Data Center'");
            jiraDataCenterLinkTeamPage.EnterJiraDataCenterCredentials(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0]);
            linkTeamPage.WaitUntilLinkTeamPageLoaded();
            Assert.IsTrue(linkTeamPage.IsLinkTeamTitleDisplayed(), "The 'Link Team' title is not displayed.");

            Log.Info("Navigate back to 'Add Connection' page and verify if the 'Manage' button is present on the 'Jira Data center' tab.");
            manageIntegrationsPage.NavigateToPage(_companyResponse.Id);
            manageIntegrationsPage.WaitUntilAddConnectionPageLoaded();
            Assert.IsTrue(manageIntegrationsPage.IsManageButtonPresent(Constants.PlatformJiraDataCenter), $"The 'Manage' button is not present for '{Constants.PlatformJiraDataCenter}'.");

            Log.Info("Click on the 'Manage' button and verify that the linked instance name is displayed in the 'Jira Instance' dropdown.");
            manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraDataCenter);
            linkTeamPage.WaitUntilLinkTeamPageLoaded();
            var expectedInstanceNames = new List<string>()
            {
                JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName,
            };
            var actualInstanceName = linkTeamPage.GetJiraInstanceNameLists();
            Assert.That.ListsAreEqual(expectedInstanceNames, actualInstanceName, "Created instance name is not displayed in the 'Jira Instance' dropdown.");

            Log.Info("Delete the instance and verify that the instance is deleted for 'Jira Data Center'.");
            linkTeamPage.ClickOnDeleteInstanceTitle();
            Assert.IsTrue(linkTeamPage.IsWarningPopupPresent(), "The 'Warning' popup is not present.");
            linkTeamPage.DeleteInstance(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName);
            manageIntegrationsPage.WaitUntilAddConnectionPageLoaded();
            Assert.IsTrue(manageIntegrationsPage.IsLinkButtonPresent(Constants.PlatformJiraDataCenter), "The 'Link' button is not present after deleting the instance with 'Jira Data Center'.");
            manageIntegrationsPage.ClickOnLinkButton(Constants.PlatformJiraDataCenter);
            Assert.IsTrue(linkTeamPage.IsAuthenticationPopupPresent(), "The 'Jira Data Center Credentials' popup is not present.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin")]
        [TestCategory("KnownDefect")] // Bug: 39549
        public void VerifyAddOrDeleteMultipleInstancesForTheJiraDataCenter()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(Driver, Log);
            var linkTeamBasePage = new LinkTeamBasePage(Driver, Log);
            var jiraDataCenterLinkTeamPage = new JiraDataCenterLinkTeamPage(Driver, Log);

            Log.Info("Login to the CA user");
            login.NavigateToPage();
            login.LoginToApplication(_companyRequest.CompanyAdminEmail, SharedConstants.CommonPassword);

            Log.Info("Add the first instance for the 'Jira Data Center' and verify that the 'Link Team' page is displayed.");
            manageIntegrationsPage.NavigateToPage(_companyResponse.Id);
            manageIntegrationsPage.WaitUntilAddConnectionPageLoaded();
            manageIntegrationsPage.ClickOnLinkButton(Constants.PlatformJiraDataCenter);
            jiraDataCenterLinkTeamPage.EnterJiraDataCenterCredentials(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0]);
            Assert.IsTrue(linkTeamBasePage.IsLinkTeamTitleDisplayed(), "The 'Link Team' title is not displayed.");

            Log.Info("Verify that the validation messages are displayed correctly after entering duplicate instance name and url in the 'Instance Name' and 'Url' text field.");
            linkTeamBasePage.WaitUntilLinkTeamPageLoaded();
            linkTeamBasePage.ClickOnAddNewInstancePlusIcon();
            linkTeamBasePage.EnterInstanceName(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName);
            Assert.AreEqual("Instance with this name already exists", linkTeamBasePage.GetInstanceNameAlreadyExistValidationMessage(), "The validation message is not  correct for the duplicate instance name.");
            linkTeamBasePage.EnterUrl(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].ServerUrl);
            Assert.AreEqual("Instance with this URL already exists", linkTeamBasePage.GetUrlAlreadyExistValidationMessage(), "The validation message is not correct for the duplicate url.");

            Log.Info("Verify that the user can add multiple instances successfully.");
            jiraDataCenterLinkTeamPage.EnterJiraDataCenterCredentials(JiraDataCenterAuthInfo.JiraDataCenterCredentials[1]);
            linkTeamBasePage.WaitUntilLinkTeamPageLoaded();
            var expectedInstanceNames = new List<string>()
            {
                JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName,
                JiraDataCenterAuthInfo.JiraDataCenterCredentials[1].InstanceName,
            };
            var actualInstanceName = linkTeamBasePage.GetJiraInstanceNameLists();
            Assert.That.ListsAreEqual(expectedInstanceNames, actualInstanceName, "The created instances names are not equal.");

            Log.Info("Verify that the user can delete the multiple instances successfully.");
            //Delete a 1st instance
            linkTeamBasePage.ClickOnDeleteInstanceTitle();
            linkTeamBasePage.DeleteInstance(JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName);
            linkTeamBasePage.WaitUntilLinkTeamPageLoaded();
            actualInstanceName = linkTeamBasePage.GetJiraInstanceNameLists();
            Assert.That.ListNotContains(actualInstanceName, JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName, $"The deleted instance:{JiraDataCenterAuthInfo.JiraDataCenterCredentials[0].InstanceName} is still displayed in the 'Jira Instance' dropdown.");
            Assert.That.ListContains(actualInstanceName, JiraDataCenterAuthInfo.JiraDataCenterCredentials[1].InstanceName, $"The non deleted instance:{JiraDataCenterAuthInfo.JiraDataCenterCredentials[1].InstanceName} is not displayed in the 'Jira Instance' dropdown.");

            //Delete 2nd instance
            linkTeamBasePage.ClickOnDeleteInstanceTitle();
            linkTeamBasePage.DeleteInstance(JiraDataCenterAuthInfo.JiraDataCenterCredentials[1].InstanceName);
            manageIntegrationsPage.WaitUntilAddConnectionPageLoaded();
            Assert.IsTrue(manageIntegrationsPage.IsLinkButtonPresent(Constants.PlatformJiraCloud), "The 'Link' button is not present after deleting the instance with 'Jira Data Center'.");
        }
    }
}