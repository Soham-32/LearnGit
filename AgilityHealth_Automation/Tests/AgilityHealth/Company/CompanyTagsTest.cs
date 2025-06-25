using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Tags;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageTeamTags;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company
{

    [TestClass]
    [TestCategory("Companies")]
    public class CompanyTagsTest : CompanyEditBaseTest
    {

        private static AddCompanyRequest _companyRequest;
        private static readonly MasterTags.AllTags AllTags = MasterTagsFactory.GetAllTags();

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            _companyRequest = CreateCompany();
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Critical")]
        [TestCategory("SiteAdmin")]
        public void Company_Verify_NewCompanyDefaultTags()
        {
            var manageTags = new ManageTagsPage(Driver, Log);
            var login = new LoginPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            manageTags.NavigateToPage(Company.Id);
            manageTags.SelectCompany(_companyRequest.Name);

            var teamCategoriesList = AllTags.Teams.Select(e => (e.CategoryName)).ToList();
            var teamTypeList = AllTags.Teams.Select(e => (e.Type)).ToList();
            for (var categoriesCount = 0; categoriesCount < teamCategoriesList.Count; categoriesCount++)
            {
                Assert.IsTrue(manageTags.IsTeamsCategoryPresent(teamCategoriesList[categoriesCount], teamTypeList[categoriesCount]), $"Team category '{teamCategoriesList[categoriesCount]} - {teamTypeList[categoriesCount]}' is not present");
                manageTags.ClickByTeamsCategoryNameAndType(teamCategoriesList[categoriesCount], teamTypeList[categoriesCount]);
                var expectedTagNameList = AllTags.Teams.Where(e => e.CategoryName == teamCategoriesList[categoriesCount]).SelectMany(b => b.Tags).Select(e => (e.TagName)).ToList();
                var expectedParentTagNameList = AllTags.Teams.Where(e => e.CategoryName == teamCategoriesList[categoriesCount] && e.Type == teamTypeList[categoriesCount]).SelectMany(b => b.Tags).Select(e => (e.ParentTagName)).ToList();

                for (var tagNameCount = 0; tagNameCount < expectedTagNameList.Count; tagNameCount++)
                {
                    Assert.IsTrue(manageTags.IsTeamTagPresent(expectedTagNameList[tagNameCount], expectedParentTagNameList[tagNameCount]), $"Team Tag '{expectedTagNameList[tagNameCount]} - {expectedParentTagNameList[tagNameCount]}' is not present");
                }
            }

            manageTags.ClickOnTeamMembersTab();
            var teamMemberCategoriesList = AllTags.TeamMembers.Select(e => (e.CategoryName)).ToList();
            var teamMemberTypeList = AllTags.TeamMembers.Select(e => (e.Type)).ToList();
            for (var categoriesCount = 0; categoriesCount < teamMemberCategoriesList.Count; categoriesCount++)
            {
                Assert.IsTrue(manageTags.IsTeamMemberCategoryDisplayed(teamMemberCategoriesList[categoriesCount], teamMemberTypeList[categoriesCount]), $"Team member category '{teamMemberCategoriesList[categoriesCount]} - {teamMemberTypeList[categoriesCount]}' is not present");
                manageTags.ClickByTeamMemberCategoryNameAndType(teamMemberCategoriesList[categoriesCount], teamMemberTypeList[categoriesCount]);

                var expectedTeamMemberTagNameList = AllTags.TeamMembers.Where(e => e.CategoryName == teamMemberCategoriesList[categoriesCount] && e.Type == teamMemberTypeList[categoriesCount]).SelectMany(b => b.Tags).Select(e => (e.TagName)).ToList();
                var expectedTeamMemberParentTagNameList = AllTags.TeamMembers.Where(e => e.CategoryName == teamMemberCategoriesList[categoriesCount] && e.Type == teamMemberTypeList[categoriesCount]).SelectMany(b => b.Tags).Select(e => (e.ParentTagName)).ToList();

                for (var tagNameCount = 0; tagNameCount < expectedTeamMemberTagNameList.Count; tagNameCount++)
                {
                    Assert.IsTrue(manageTags.IsTeamMemberTagsPresents(expectedTeamMemberTagNameList[tagNameCount], expectedTeamMemberParentTagNameList[tagNameCount]), $"Team member tag '{expectedTeamMemberTagNameList[tagNameCount]} - {expectedTeamMemberParentTagNameList[tagNameCount]}' is not present");
                }
            }

            manageTags.ClickOnStakeHoldersTab();
            var stakeholderCategoriesList = AllTags.Stakeholders.Select(e => (e.CategoryName)).ToList();
            var stakeholderTypeList = AllTags.Stakeholders.Select(e => (e.Type)).ToList();
            for (var categoriesCount = 0; categoriesCount < stakeholderCategoriesList.Count; categoriesCount++)
            {
                Assert.IsTrue(manageTags.IsStakeholderCategoryDisplayed(stakeholderCategoriesList[categoriesCount], stakeholderTypeList[categoriesCount]), $"Stakeholder category '{stakeholderCategoriesList[categoriesCount]} - {stakeholderTypeList[categoriesCount]}' is not present");
                manageTags.ClickByStakeholderCategoryNameAndType(stakeholderCategoriesList[categoriesCount], stakeholderTypeList[categoriesCount]);

                var expectedStakeholderTagNameList = AllTags.Stakeholders.Where(e => e.CategoryName == stakeholderCategoriesList[categoriesCount] && e.Type == stakeholderTypeList[categoriesCount]).SelectMany(b => b.Tags).Select(e => (e.TagName)).ToList();
                foreach (var t in expectedStakeholderTagNameList)
                {
                    Assert.IsTrue(manageTags.IsStakeholderTagsPresents(t), $"Stakeholder tag '{t}' is not present");
                }
            }
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }
    }
}
