using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Enum.BusinessOutcomes;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.BusinessOutcomes.Dashboard;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.BusinessOutcomes.Dashboard
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class BusinessOutcomesDashboardTagsDropDownValuesTests : BaseTest
    {

        private static List<BusinessOutcomeCategoryLabelResponse> _allTags;

        [ClassInitialize]
        public static void ClassSetUp(TestContext _)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            _allTags = setup.GetBusinessOutcomesAllLabels(Company.Id);
        }


        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void BusinessOutcomes_Dashboard_Tags_DropdownValues()
        {
            var login = new LoginPage(Driver, Log);
            var boDashboard = new BusinessOutcomesDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            boDashboard.NavigateToPage(Company.Id);
            boDashboard.TagsViewExpandCollapseTagDropdown();
            var actualTags = boDashboard.TagsViewGetAllTagsList();

            //Default Tags
            Assert.IsTrue(actualTags.Any(text => text.Equals("Annually")), "'Annually' isn't displayed on Tags view");
            Assert.IsTrue(actualTags.Any(text => text.Equals("Quarterly")), "'Quarterly' isn't displayed on Tags view");

            boDashboard.SelectCardTypeTag(TimeFrameTags.Annually.GetDescription());
            actualTags.RemoveRange(0, 2);

            Log.Info("Select card type 'initiatives', get the list of tags and select 'Initiative timeline' tag");
            boDashboard.SelectCardType(BusinessOutcomesCardType.AnnualView.GetDescription());

            boDashboard.TagsViewExpandCollapseTagDropdown();
            actualTags.AddRange(boDashboard.TagsViewGetAllTagsList());

            //Default Tags
            Assert.IsTrue(actualTags.Any(text => text.Equals("Sub-Teams")), "'Sub-Teams' isn't displayed on Tags view");
            Assert.IsTrue(actualTags.Any(text => text.Equals("Annual View")), "'Annual View' isn't displayed on Tags view");
            actualTags.RemoveAt(2);

            boDashboard.SelectCardTypeTag(BusinessOutcomesCardTypeTags.AnnualView.GetDescription());

            Log.Info("Select 'Projects' option and verify tags of that option");
            boDashboard.SelectCardType(BusinessOutcomesCardType.ProjectsTimeline.GetDescription());
            boDashboard.TagsViewExpandCollapseTagDropdown();

            actualTags.AddRange(boDashboard.TagsViewGetAllTagsList());

            //Default Tags
            Assert.IsTrue(actualTags.Any(text => text.Equals("Sub-Teams")), "'Sub-Teams' isn't displayed on Tags view");
            Assert.IsTrue(actualTags.Any(text => text.Equals("Projects Timeline")), "'Projects Timeline' isn't displayed on Tags view");
            actualTags.RemoveAt(3);
            boDashboard.SelectCardTypeTag(BusinessOutcomesCardTypeTags.ProjectsTimeline.GetDescription());

            Log.Info("Select 'Deliverable' option and verify tags of that option");
            boDashboard.SelectCardType(BusinessOutcomesCardType.DeliverablesTimeline.GetDescription());
            boDashboard.TagsViewExpandCollapseTagDropdown();

            actualTags.AddRange(boDashboard.TagsViewGetAllTagsList());

            //Default Tags
            Assert.IsTrue(actualTags.Any(text => text.Equals("Sub-Teams")), "'Sub-Teams' isn't displayed on Tags view");
            Assert.IsTrue(actualTags.Any(text => text.Equals("Deliverables Timeline")), "'Deliverable Timeline' isn't displayed on Tags view");
            actualTags.RemoveAt(4);
            boDashboard.SelectCardTypeTag(BusinessOutcomesCardTypeTags.DeliverablesTimeline.GetDescription());

            Log.Info("Select 'Stories' option and verify tags of that option");
            boDashboard.SelectCardType(BusinessOutcomesCardType.Monthly.GetDescription());
            boDashboard.TagsViewExpandCollapseTagDropdown();

            actualTags.AddRange(boDashboard.TagsViewGetAllTagsList());

            //Default Tags
            Assert.IsTrue(actualTags.Any(text => text.Equals("Sub-Teams")), "'Sub-Teams' isn't displayed on Tags view");
            Assert.IsTrue(actualTags.Any(text => text.Equals("Monthly")), "'Deliverable Timeline' isn't displayed on Tags view");
            actualTags.RemoveAt(5);
            boDashboard.SelectCardTypeTag(BusinessOutcomesCardTypeTags.Monthly.GetDescription());


            Assert.That.ListsAreEqual(_allTags.Select(a => a.Name).ToList(), actualTags, "CardType tags doesn't match");
        }
    }
}