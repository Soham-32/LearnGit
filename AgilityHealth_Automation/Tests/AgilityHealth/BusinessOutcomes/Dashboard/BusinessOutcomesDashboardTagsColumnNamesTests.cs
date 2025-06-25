using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardTagsColumnNamesTests : BaseTest
    {
        private static BusinessOutcomeCategoryLabelResponse _label1;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var allLabels = setup.GetBusinessOutcomesAllLabels(Company.Id);
            _label1 = allLabels.Where(b => b.Name.Contains("Automation Label"))
                .FirstOrDefault(l => l.KanbanMode && l.Tags.Count > 1);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_Tags_ColumnNames()
        {
            
            var login = new LoginPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            var expectedCategories = _label1.Tags.Select(t => t.Name).ToList();

            boDashboard.NavigateToPage(Company.Id);

            boDashboard.TagsViewSelectTag(_label1.Name);
            var actualCategories = boDashboard.GetAllColumnNames();

            Log.Info("Verifying that columns info is correct");
            Assert.AreEqual(expectedCategories.Count, actualCategories.Count, "Columns count doesn't match");

            foreach (var category in expectedCategories)
            {
                Assert.IsTrue(actualCategories.Any(text => text.ToLower().Equals(category.ToLower())),
                    $"{category} column isn't displayed on Tags view for {_label1.Name}");
            }

            Driver.RefreshPage();
            boDashboard.IsPageLoadedCompletely("Tag 6");
            Assert.AreEqual(_label1.Name, boDashboard.GetSelectedLabelFromDropDown(), $"Business outcome- {_label1.Name} isn't selected");
        }
    }
}