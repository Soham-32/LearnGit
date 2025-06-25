using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Details;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.MultiTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("MultiTeam")]
    public class MultiTeamVerifyFilterTests : BaseTest
    {
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 },
            StakeHolders = new List<string> { Constants.StakeholderName2, Constants.StakeholderName4 },
            EndDate = DateTime.Today.AddHours(23)
        };

        private const int TeamMember1Answer = 4;
        private const int TeamMember2Answer = 6;
        private const int Stakeholder2Answer = 9;
        private const int Stakeholder4Answer = 7;

        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static List<string> _allTeamMtNamesList;
        private static IList<TeamProfileResponse> _teamProfileResponse;
        private static AssessmentResponse _assessmentResponse;
        private static IList<TeamMemberResponse> _teamMembersResponse;
        private static List<string> _categoryList;
        private static List<string> _tagsList;
        private static List<string> _teamMembersRolesList;
        private static List<string> _stakeHoldersRolesList;
        private static List<string> _participantGroupsList;
        private static List<int> _teamMemberSurveyAnswers;
        private static List<int> _stakeholderSurveyAnswers;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForMultiTeamRadar).CheckForNull($"<{Constants.TeamForMultiTeamRadar}> was not found in the response.");
                _multiTeam = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForRadar).CheckForNull($"<{Constants.MultiTeamForRadar}> was not found in the response.");

                _allTeamMtNamesList = new List<string>
                {
                    _team.Name,
                    _multiTeam.Name
                };

                setupUi.DeleteAssessmentsIfPresent(_team.TeamId);
                setupUi.AddTeamAssessment(_team.TeamId, TeamAssessment);
                var teamMemberEmailSearchList = new List<EmailSearch>
                {
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember1.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },
                    new EmailSearch
                    {
                        Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                        To = SharedConstants.TeamMember2.Email,
                        Labels = new List<string> { GmailUtil.MemberEmailLabel }
                    },

                };
                _teamMemberSurveyAnswers = new List<int> { TeamMember1Answer, TeamMember2Answer };

                var stakeholderEmailList = new List<string> { SharedConstants.Stakeholder2.Email, SharedConstants.Stakeholder4.Email };
                _stakeholderSurveyAnswers = new List<int> { Stakeholder2Answer, Stakeholder4Answer };

                setupUi.CompleteTeamMemberSurvey(teamMemberEmailSearchList, _teamMemberSurveyAnswers);
                setupUi.CompleteStakeholderSurvey(_team.Name, stakeholderEmailList, TeamAssessment.AssessmentName, _stakeholderSurveyAnswers);
                setupUi.CloseTeamAssessment(_team.TeamId, TeamAssessment.AssessmentName);

                _teamProfileResponse = setup.GetTeamProfileResponse(_team.Name);
                _assessmentResponse = setup.GetAssessmentResponse(_team.Name, TeamAssessment.AssessmentName).Result;
                _teamMembersResponse = setup.GetTeamMemberResponse(_team.Name);

                _categoryList = _teamProfileResponse.First().TeamTags.Select(a => a.Category).ToList();
                _tagsList = _teamProfileResponse.First().TeamTags.Select(a => a.Tags).ToList().SelectMany(list => list).ToList();
                _teamMembersRolesList = _teamMembersResponse.Where(a => a.Tags != null).ToList().Select(b => b.Tags.Where(a => a.Key.Equals("Role"))).ToList().Where(a => a.FirstOrDefault() != null).ToList().Select(b => b.FirstOrDefault()!.Value).ToList().SelectMany(list => list).ToList();
                _stakeHoldersRolesList = _assessmentResponse.Participants.Select(a => a.StakeHolderTags).ToList().SelectMany(list => list).ToList();
                _participantGroupsList = _teamMembersResponse.Where(a => a.Tags != null).ToList().Select(b => b.Tags.Where(a => a.Key.Equals("Participant Group"))).ToList().Where(a => a.FirstOrDefault() != null).ToList().Select(b => b.FirstOrDefault()!.Value).ToList().SelectMany(list => list).ToList();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeamRadarVerifyTeamFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var multiTeamRadarPage = new MultiTeamRadarPage(Driver, Log);
            var mtEtRadarPage = new MtEtRadarCommonPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info($"Go to Radar page for the MT team - {_multiTeam.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            Thread.Sleep(120000); //Radar takes up few minutes for newly created Teams to plot for Radar Screen
            Driver.RefreshPage();

            Log.Info("Click on the Filter navbar and verify that 'Multi-Teams' tag is present.");
            radarPage.Filter_OpenFilterSidebar();
            var expectedFilterTag = "Teams";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");

            Log.Info("Verify that by default,'Multi Team' filter checkbox is unchecked and 'Team' filter checkboxes is checked ");
            Assert.IsFalse(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(_multiTeam.Name), $"{_multiTeam.Name} filter checkbox is selected");
            Assert.IsTrue(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(_team.Name), $"{_team.Name} filter checkbox isn't selected");

            Log.Info("Verify 'Avg Dots', 'Average value' for the 'Member' and 'Stakeholder' on MT radar.");
            var expectedAvgValueForMembers = (TeamMember1Answer + TeamMember2Answer) / 2;
            var expectedAvgValueForStakeholders = (Stakeholder2Answer + Stakeholder4Answer) / 2;
            var colorForTeam = mtEtRadarPage.FilterGetTeamColor(_team.Name);

            // Verifying individual dots for member and stakeholder on 'Teams' tab
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                // Verify Avg count
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("avg", colorForTeam, comp),
                    $"Competency: <{comp}> avg count doesn't match");

                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorForTeam, comp).First()), $"Competency: <{comp}> avg value doesn't match");
            }

            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                // Verify Avg count
                Assert.AreEqual(1, multiTeamRadarPage.GetRadarDotsCount("avg", colorForTeam, comp), $"Competency: <{comp}> avg count doesn't match");

                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorForTeam, comp).First()), $"Competency: <{comp}> avg value doesn't match");
            }

            Log.Info($"Check {_multiTeam.Name} checkbox and verify 'Individual Dots','Avg Dots' and 'Average value' for 'Member' and 'Stakeholder' on MT radar.");
            mtEtRadarPage.FilterTeamTabSelectFilterItemCheckboxByName(_multiTeam.Name);
            var radarDotsColor = mtEtRadarPage.FilterGetTeamColor(_multiTeam.Name);
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                // Verify Individual count
                Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }
            //Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                // Verify Individual count
                Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }

            Log.Info("Verify 'Individual','Avg Dots' for 'Member' and 'Stakeholder' on MT radar.");
            mtEtRadarPage.FilterTeamTabSelectFilterItemCheckboxByName(_team.Name, false);
            //Member
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                // Verify Individual count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }
            //Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                // Verify Individual count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }

            Log.Info("Verify 'Hide Individual dots' functionality.");
            mtEtRadarPage.SelectHideIndividualDotsCheckbox();
            //Member
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                // Verify Individual count
                Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                    $"Competency: <{comp}> dot count doesn't match");
                // Verify avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                    $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }
            //Stakeholder
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
            {
                // Verify Individual count
                Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                    $"Competency: <{comp}> dot count doesn't match");
                // Verify avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                    $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg value
                Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
            }
            mtEtRadarPage.SelectHideIndividualDotsCheckbox(false);

            Log.Info("Verify 'Hide Teams Names' functionality.");
            mtEtRadarPage.SelectHideMultiTeamNamesCheckbox();
            Assert.AreEqual(mtEtRadarPage.GetMultiTeamName(_team.Name), "Team 1", $"Multi team '{_team.Name}' name doesn't match.");
            mtEtRadarPage.SelectHideMultiTeamNamesCheckbox(false);

            Log.Info("Verify 'Select All' and 'Clear All' functionality.");
            mtEtRadarPage.FilterClickOnSelectAllLink();
            _allTeamMtNamesList.ForEach(team => Assert.IsTrue(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(team), $"{team} filter checkbox isn't selected"));
            mtEtRadarPage.FilterClickOnClearAllLink();
            _allTeamMtNamesList.ForEach(team => Assert.IsFalse(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(team), $"{team} filter checkbox is selected"));
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeamRadarVerifyTagsFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtEtRadarPage = new MtEtRadarCommonPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info($"Go to Radar page for the MT team - {_multiTeam.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            Log.Info("Verify 'Tags' filter is present in the filter list.");
            Thread.Sleep(120000); //Radar takes up few minutes for newly created Teams to plot for Radar Screen
            Driver.RefreshPage();
            radarPage.Filter_OpenFilterSidebar();
            var expectedFilterTag = "Tags";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present. ");

            Log.Info("Verify all tags are available with their respective category.");
            var radarDotsColor = mtEtRadarPage.FilterGetTeamColor(_multiTeam.Name);
            var expectedAvgValueForMembers = (TeamMember1Answer + TeamMember2Answer) / 2;
            var expectedAvgValueForStakeholders = (Stakeholder2Answer + Stakeholder4Answer) / 2;

            mtEtRadarPage.FilterClickOnTagsTab();
            Assert.That.ListsAreEqual(_categoryList, mtEtRadarPage.FilterGetAllCategoryList(), "Category list doesn't match.");
            foreach (var category in _categoryList)
            {
                var tags = _teamProfileResponse.First().TeamTags.Where(a => a.Category.Equals(category)).Select(a => a.Tags).First().ListToString();
                Assert.AreEqual(mtEtRadarPage.FilterGetTagName(category), tags, $"Tags for the respective category {category} doesn't match.");
            }

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for member and stakeholders on MT Radar.");
            foreach (var tagName in _tagsList)
            {
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(tagName);
                radarDotsColor = mtEtRadarPage.FilterGetFilterItemColor(tagName);

                //Member
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {

                    // Verify Individual count
                    Assert.AreEqual(2, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
                //Stakeholder
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                {
                    // Verify Individual count
                    Assert.AreEqual(2, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(tagName, false);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("dots", radarDotsColor, comp), $"Competency: <{comp}> dot is displayed");
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("avg", radarDotsColor, comp), $"Competency: <{comp}> avg dot is displayed");
                }
                //Stakeholder
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                {

                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("dots", radarDotsColor, comp), $"Competency: <{comp}> dot is displayed");
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("avg", radarDotsColor, comp), $"Competency: <{comp}> avg dot is displayed");
                }
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 45561
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeamRadarVerifyRolesFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtEtRadarPage = new MtEtRadarCommonPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info($"Go to Radar page for the MT team - {_multiTeam.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            Log.Info("Verify 'Roles' filter is present in the filter list.");
            Thread.Sleep(120000); //Radar takes up few minutes for newly created Teams to plot for Radar Screen
            Driver.RefreshPage();

            radarPage.Filter_OpenFilterSidebar();
            var expectedFilterTag = "Roles";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for members and stakeholders");
            var expectedAvgValueForMembers = (TeamMember1Answer + TeamMember2Answer) / 2;
            var expectedAvgValueForStakeholders = (Stakeholder2Answer + Stakeholder4Answer) / 2;
            mtEtRadarPage.FilterClickOnRolesTab();
            var memberRolesList = mtEtRadarPage.GetTeamMembersRoleTagsList();
            //Member
            foreach (var role in memberRolesList)
            {
                Assert.That.ListContains(_teamMembersRolesList, role, $"Member Role {role} doesn't present");
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(role);
                var colorHexForDeveloperRole = mtEtRadarPage.FilterGetFilterItemColor(role);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    // Verify Individual count
                    Assert.AreEqual(2, mtEtRadarPage.GetRadarDotsCount("dots", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorHexForDeveloperRole, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(role, false);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("dots", colorHexForDeveloperRole, comp));
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("avg", colorHexForDeveloperRole, comp));
                }
            }

            //Stakeholder
            var stakeHolderRolesList = mtEtRadarPage.GetStakeholdersRoleTagsList();
            foreach (var stakeholderRole in stakeHolderRolesList)
            {
                Assert.That.ListContains(_stakeHoldersRolesList, stakeholderRole, $"Stakeholder role {stakeholderRole} doesn't match.");
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(stakeholderRole);
                var colorHexForDeveloperRole = mtEtRadarPage.FilterGetFilterItemColor(stakeholderRole);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                {
                    // Verify Individual count
                    Assert.AreEqual(2, mtEtRadarPage.GetRadarDotsCount("dots", colorHexForDeveloperRole, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", colorHexForDeveloperRole, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorHexForDeveloperRole, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(stakeholderRole, false);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                {
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("dots", colorHexForDeveloperRole, comp));
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("avg", colorHexForDeveloperRole, comp));
                }
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 45561
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void MultiTeamRadarVerifyParticipantGroupsFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtEtRadarPage = new MtEtRadarCommonPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info($"Go to Radar page for the MT team - {_multiTeam.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            mtEtDashboardPage.NavigateToPage(_multiTeam.TeamId);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

            Log.Info("Verify 'Participant Groups' filter is present in the filter list.");
            Thread.Sleep(120000); //Radar takes up few minutes for newly created Teams to plot for Radar Screen
            Driver.RefreshPage();

            radarPage.Filter_OpenFilterSidebar();
            var expectedFilterTag = "Participant Groups";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for members on MT Radar.");
            var radarDotsColor = mtEtRadarPage.FilterGetTeamColor(_multiTeam.Name);
            var expectedAvgValueForMembers = (TeamMember1Answer + TeamMember2Answer) / 2;
            mtEtRadarPage.FilterClickOnParticipantGroupsTab();
            var participantGroup = mtEtRadarPage.GetParticipantGroupTagsList();
            foreach (var participantGroupTag in participantGroup)
            {
                Assert.That.ListContains(_participantGroupsList, participantGroupTag, $"participant groups {participantGroupTag} doesn't present.");
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(participantGroupTag);
                radarDotsColor = mtEtRadarPage.FilterGetFilterItemColor(participantGroupTag);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    // Verify Individual count
                    Assert.AreEqual(2, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
                mtEtRadarPage.FilterSelectFilterItemCheckboxByName(participantGroupTag, false);
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("dots", radarDotsColor, comp));
                    Assert.IsFalse(mtEtRadarPage.IsRadarDotsDisplayed("avg", radarDotsColor, comp));
                }
            }
        }
    }
}
