using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Settings.ManageUsers
{
    [TestClass]
    [TestCategory("ManageUser"), TestCategory("Settings")]
    public class ManageUserSearchTests : BaseTest
    {
        private static readonly UserType UserType = UserType.CompanyAdmin;

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUsers_Search()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType);
            var userSearchPopup = new UserSearchPopup(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            var searchTerm = User.FirstName;
            manageUserPage.Search(searchTerm);

            var firstNameValues = userSearchPopup.GetColumnValues("First Name");
            var lastNameValues = userSearchPopup.GetColumnValues("Last Name");
            var emailValues = userSearchPopup.GetColumnValues("Email");

            Assert.IsTrue(firstNameValues.Any(), $"No results were found for search <{searchTerm}>");

            searchTerm = searchTerm.ToUpper();
            for (var i = 0; i < firstNameValues.Count; i++)
            {
                Assert.IsTrue(firstNameValues[i].ToUpper().Contains(searchTerm) 
                    || lastNameValues[i].ToUpper().Contains(searchTerm) 
                    || emailValues[i].ToUpper().Contains(searchTerm), $"Results aren't matched search term <{searchTerm}>");
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void ManageUsers_Search_Popup()
        {
            var login = new LoginPage(Driver, Log);
            var manageUserPage = new ManageUserPage(Driver, Log, UserType);
            var userSearchPopup = new UserSearchPopup(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageUserPage.NavigateToPage(Company.Id);
            var searchTerm = User.Username;
            manageUserPage.Search(User.FirstName);

            userSearchPopup.Search(searchTerm);

            var results = userSearchPopup.GetColumnValues("Email");

            Assert.IsTrue(results.Any(), $"No results were found for search <{searchTerm}>");
            foreach (var email in results)
            {
                Assert.IsTrue(email.Contains(searchTerm), 
                    $"<{email}> does not match search term <{searchTerm}>.");
            }

            userSearchPopup.ClickCloseButton();
        }
    }
}