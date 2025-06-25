using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardFilterByTagsTests : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response1;
        private static BusinessOutcomeResponse _response2;
        private static List<BusinessOutcomeCategoryLabelResponse> _labels;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            _labels = setup.GetBusinessOutcomesAllLabels(Company.Id);
            var tag1 = _labels.First(n=>n.Name.Contains("Automation Label")).Tags[0];
            var tag2 = _labels.First(n => n.Name.Contains("Automation Label")).Tags[1];
            
            var request1 = GetBusinessOutcomeRequest(SwimlaneType.StrategicIntent);
            request1.Tags.Add(new BusinessOutcomeTagRequest { Name = tag1.Name, Uid = tag1.Uid, CategoryLabelUid = tag1.CategoryLabelUid});
            _response1 = setup.CreateBusinessOutcome(request1);
            
            var request2 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
            request2.Tags.Add(new BusinessOutcomeTagRequest { Name = tag2.Name, Uid = tag2.Uid, CategoryLabelUid = tag2.CategoryLabelUid});
            _response2 = setup.CreateBusinessOutcome(request2);
            
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_Filter_ByTags()
        {
            var login = new LoginPage(Driver, Log);
            var businessOutcomesDashboard = new BusinessOutcomesDashboardPage(Driver, Log);
            
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            businessOutcomesDashboard.NavigateToPage(Company.Id);
            businessOutcomesDashboard.SelectFilterTags(_response1.Tags.Select(t => t.Name).ToList());

            Driver.RefreshPage();
            businessOutcomesDashboard.WaitTillBoPageLoadedCompletely();
            businessOutcomesDashboard.TagsViewSelectTag("Automation Label 1");
            Assert.IsTrue(businessOutcomesDashboard.AreFilterTagsSelected(_response1.Tags.Select(t => t.Name).ToList()), "Tag is not selected");

            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response1.Title),
                $"Business outcome <{_response1.Title}> is not present");
            Assert.IsFalse(businessOutcomesDashboard.IsBusinessOutcomePresent(_response2.Title),
                $"Business outcome <{_response2.Title}> is present");

            // all are shown when filter is cleared
            businessOutcomesDashboard.SelectFilterTags(new List<string>());
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response1.Title),
                $"Business outcome <{_response1.Title}> is not present");
            Assert.IsTrue(businessOutcomesDashboard.IsBusinessOutcomePresent(_response2.Title),
                $"Business outcome <{_response2.Title}> is not present");
        }
    }
}