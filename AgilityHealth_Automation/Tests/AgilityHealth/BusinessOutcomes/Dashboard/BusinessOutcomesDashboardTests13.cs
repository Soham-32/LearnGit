using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardTests13 : BusinessOutcomesBaseTest
    {

        private static BusinessOutcomeResponse _response1;
        private static BusinessOutcomeResponse _response2;
        private static BusinessOutcomeCategoryLabelResponse _label2;
        private static BusinessOutcomeTagResponse _tag1;
        private static BusinessOutcomeTagResponse _tag2;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);

            var labels = setup.GetBusinessOutcomesAllLabels(Company.Id);
            _label2 = labels.Where(b => b.Name.Contains("Automation Label"))
                .FirstOrDefault(l => !l.KanbanMode && l.Tags.Any());
            _tag1 = _label2.Tags[0];
            _tag2 = _label2.Tags[1];

            var request1 = GetBusinessOutcomeRequest(SwimlaneType.StrategicIntent);
            request1.Tags.Add(new BusinessOutcomeTagRequest { Name = _tag1.Name, Uid = _tag1.Uid, CategoryLabelUid = _tag1.CategoryLabelUid});
            _response1 = setup.CreateBusinessOutcome(request1);

            var request2 = GetBusinessOutcomeRequest(SwimlaneType.StrategicTheme);
            request2.Tags.Add(new BusinessOutcomeTagRequest { Name = _tag2.Name, Uid = _tag2.Uid, CategoryLabelUid = _tag2.CategoryLabelUid});
            _response2 = setup.CreateBusinessOutcome(request2);

        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void BusinessOutcomes_Dashboard_Tags_DragAndDrop_Disabled()
        {

            var login = new LoginPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            boDashboard.NavigateToPage(Company.Id);
            boDashboard.TagsViewSelectTag(_label2.Name);
            boDashboard.DragCardToCard(_response1.SwimlaneType.GetDescription(), _response1.Title, _response2.Title, 300, 0);

            Log.Info("Verify that BO items are dragged or not");
            Assert.IsFalse(boDashboard.GetAllBusinessOutcomeNamesByColumn(_tag2.Name).Any(text => text.Contains(_response1.Title)),
                $"Horizontal drag worked. {_response1} present under {_tag2.Name}");
            Assert.IsTrue(boDashboard.GetAllBusinessOutcomeNamesByColumn(_tag1.Name).Any(text => text.Contains(_response1.Title)),
                $"Horizontal drag worked. {_response1} isn't present under {_tag1.Name}");

            Log.Info("verify that BO items still under respective columns");
            Assert.IsFalse(boDashboard.GetAllBusinessOutcomeNamesByColumn(_tag2.Name).Any(text => text.Contains(_response1.Title)),
                $"Horizontal drag worked. {_response1.Title} present under {_tag2.Name}");
            Assert.IsTrue(boDashboard.GetAllBusinessOutcomeNamesByColumn(_tag1.Name).Any(text => text.Contains(_response1.Title)),
                $"Horizontal drag worked. {_response1.Title} isn't present under {_tag1.Name}");
        }
    }
}