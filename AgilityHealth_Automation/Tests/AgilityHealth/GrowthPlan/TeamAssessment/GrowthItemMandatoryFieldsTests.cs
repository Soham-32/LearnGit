using System;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthPlan.TeamAssessment
{
    [TestClass]
    [TestCategory("GrowthPlan")]
    public class GrowthItemMandatoryFieldsTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(), 
            TeamMembers = new List<string> { Constants.TeamMemberName1 },
            StakeHolders = new List<string> { Constants.StakeholderName1 }
        };

        private static List<string> _expectedList;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _expectedList = new List<string>()
                {"Title", "Priority", "Category", "CompetencyTargets", "Type"};
            try
            {
                var teams = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id);
                _team = teams.GetTeamByName(SharedConstants.Team);
                _multiTeam = teams.GetTeamByName(SharedConstants.MultiTeam);
                _enterpriseTeam = teams.GetTeamByName(SharedConstants.EnterpriseTeam);


                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment);
                setup.TurnOnOffGrowthItemTypeFieldRequiredFeature(Company.Id);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GrowthItem_All_Mandatory_Fields_At_AssessmentLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(TeamAssessment.AssessmentName);

            var growthItemInfo = new GrowthItem
            {
                Category = "Team",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = "Other",
                Status = "Not Started",
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "Low"
            };

            Log.Info("Verify that validation message is displayed for every mandatory fields after click on 'Save' button");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.ClickSaveButton();
            foreach (var giField in _expectedList)
            {
                Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(giField), $"Validation message is not displayed for GI '{giField}' field");
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Title' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Title"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field ");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Priority' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Priority"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Category' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Category"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Type' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Type"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'CompetencyTargets' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("CompetencyTargets"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify that validation message is not displayed after click on 'Save' button, when every mandatory fields are filled");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
            }
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GrowthItem_All_Mandatory_Fields_At_MultiLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtDashboardPage.NavigateToRadarDetailsPage(Driver.GetCurrentUrl(), _multiTeam.TeamId);

            var growthItemInfo = new GrowthItem
            {
                Category = "Enterprise",
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = "Other",
                Status = "Not Started",
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "Low"
            };

            Log.Info("Verify that validation message is displayed for every mandatory fields after click on 'Save' button");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.ClickSaveButton();
            foreach (var giField in _expectedList)
            {
                Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(giField), $"Validation message is not displayed for GI '{giField}' field");
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Title' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Title"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field ");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Priority' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Priority"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Category' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Category"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Type' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Type"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'CompetencyTargets' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("CompetencyTargets"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify that validation message is not displayed after click on 'Save' button, when every mandatory fields are filled");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiCategory(growthItemInfo.Category);
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
            }
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void GrowthItem_All_Mandatory_Fields_At_EnterpriseLevel()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var addGrowthItemPopup = new AddGrowthItemPopupPage(Driver, Log);
            var growthItemGridView = new GrowthItemGridViewWidget(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            _expectedList.Remove("Category");
            Driver.NavigateToPage(ApplicationUrl);

            login.LoginToApplication(User.Username, User.Password);

            mtDashboardPage.NavigateToPage(_enterpriseTeam.TeamId, true);
            mtDashboardPage.NavigateToRadarDetailsPage(Driver.GetCurrentUrl(), _enterpriseTeam.TeamId, true);

            var growthItemInfo = new GrowthItem
            {
                Title = "GI" + RandomDataUtil.GetGrowthPlanTitle(),
                Type = "Other",
                Status = "Not Started",
                CompetencyTargets = new List<string> { Constants.TeamHealth2CompentenciesLableForMember[0] },
                Priority = "Low"
            };

            Log.Info("Verify that validation message is displayed for every mandatory fields after click on 'Save' button");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.ClickSaveButton();
            foreach (var giField in _expectedList)
            {
                Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(giField), $"Validation message is not displayed for GI '{giField}' field");
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Title' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Title"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field ");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Priority' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Priority"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'Type' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("Type"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify validation message displayed or not after click on 'Save' button, when 'CompetencyTargets' field blank");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                if (expectedField.Equals("CompetencyTargets"))
                {
                    Assert.IsTrue(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is not displayed for GI '{expectedField}' field");
                }
                else
                {
                    Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
                }
            }
            addGrowthItemPopup.ClickCancelButton();

            Log.Info("Verify that validation message is not displayed after click on 'Save' button, when every mandatory fields are filled");
            growthItemGridView.ClickAddNewGrowthItem();
            addGrowthItemPopup.SetGiTitle(growthItemInfo.Title);
            addGrowthItemPopup.SelectGiType(growthItemInfo.Type);
            addGrowthItemPopup.SelectStatus(growthItemInfo.Status);
            addGrowthItemPopup.SelectCompetencyTarget(growthItemInfo.CompetencyTargets);
            addGrowthItemPopup.SelectPriority(growthItemInfo.Priority);
            addGrowthItemPopup.ClickSaveButton();
            foreach (var expectedField in _expectedList)
            {
                Assert.IsFalse(addGrowthItemPopup.IsFieldValidationMessageDisplayed(expectedField), $"validation message is displayed for GI '{expectedField}' field");
            }
            Assert.IsTrue(growthItemGridView.IsGiPresent(growthItemInfo.Title), "Growth Item is not present");
        }

    }
}