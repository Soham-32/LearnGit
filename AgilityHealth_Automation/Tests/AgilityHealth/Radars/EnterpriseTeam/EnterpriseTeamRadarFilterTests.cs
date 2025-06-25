using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using AtCommon.Dtos.Teams;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Assessments.Team.Custom;
using System.Threading;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Radars.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Radars"), TestCategory("EnterpriseTeam")]
    public class EnterpriseTeamRadarFilterTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam1;
        private static TeamHierarchyResponse _multiTeam2;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static IList<TeamProfileResponse> _teamProfileResponse;
        private static IList<TeamMemberResponse> _teamMembersResponse;
        private static List<string> _allMtEtTeamNamesList;
        private static List<string> _categoryList;
        private static List<string> _tagsList;
        private static List<string> _teamMembersRolesList;
        private static AssessmentResponse _assessmentResponse;
        private static List<string> _participantGroupsList;
        private static List<string> _stakeHoldersRolesList;
        private static List<int> _teamMemberSurveyAnswers;
        private static List<int> _stakeholderSurveyAnswers;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamAssessmentType,
            AssessmentName = $"ETRadar{RandomDataUtil.GetAssessmentName()}",
            TeamMembers = new List<string> { Constants.TeamMemberName1, Constants.TeamMemberName2 },
            StakeHolders = new List<string> { Constants.StakeholderName2, Constants.StakeholderName4 },
        };


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var setupUi = new SetUpMethods(testContext, TestEnvironment);

                _team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForMultiTeamRadar)
                             .CheckForNull($"<{Constants.TeamForMultiTeamRadar}> was not found in the response.");
                _multiTeam1 = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForRadar)
                                   .CheckForNull($"<{Constants.MultiTeamForRadar}> was not found in the response.");
                _multiTeam2 = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForRadar2)
                                   .CheckForNull($"<{Constants.MultiTeamForRadar2}> was not found in the response.");
                _enterpriseTeam = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.EnterpriseTeamForRadar)
                                       .CheckForNull($"<{Constants.EnterpriseTeamForRadar}> was not found in the response.");


                _allMtEtTeamNamesList = new List<string>
                {
                    _enterpriseTeam.Name,
                    _multiTeam2.Name,
                    _multiTeam1.Name
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
                _teamMemberSurveyAnswers = new List<int> { 4, 6 };

                var stakeholderEmailList = new List<string> { SharedConstants.Stakeholder2.Email, SharedConstants.Stakeholder4.Email };
                _stakeholderSurveyAnswers = new List<int> { 9,7 };

                setupUi.CompleteTeamMemberSurvey(teamMemberEmailSearchList, _teamMemberSurveyAnswers);
                setupUi.CompleteStakeholderSurvey(_team.Name, stakeholderEmailList, TeamAssessment.AssessmentName, _stakeholderSurveyAnswers);
                setupUi.CloseTeamAssessment(_team.TeamId,TeamAssessment.AssessmentName);
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
        [TestCategory("KnownDefect")] // Bug Id : 45561
        [TestCategory("Sanity")]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("Member")]
        public void EnterpriseTeamRadarVerifyFilterFunctionality()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var mtEtRadarPage = new MtEtRadarCommonPage(Driver, Log);
            var mtEtDashboardPage = new MtEtDashboardPage(Driver, Log);
            var mtDashboardPage = new MtEtDashboardPage(Driver, Log);

            Log.Info($"Go to Radar page for the ET team - {_enterpriseTeam.Name}");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            mtEtDashboardPage.NavigateToPage(_enterpriseTeam.TeamId, true);
            mtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);
            Thread.Sleep(120000); //Radar takes up few minutes for newly created Teams to plot for Radar Screen
            Driver.RefreshPage();

            Log.Info("Click on the Filter navbar and verify that 'Multi-Teams' tag is present.");
            radarPage.Filter_OpenFilterSidebar();
            var expectedFilterTag = "Multi-Teams";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");

            Log.Info("Verify that by default,'Enterprise Team' filter checkbox is unchecked and both 'Multi-teams' filter checkboxes are checked ");
            Assert.IsFalse(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(_enterpriseTeam.Name), $"{_enterpriseTeam.Name} filter checkbox is selected");
            Assert.IsTrue(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(_multiTeam1.Name), $"{_multiTeam1.Name} filter checkbox isn't selected");
            Assert.IsTrue(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(_multiTeam2.Name), $"{_multiTeam2.Name} filter checkbox isn't selected");

            Log.Info("Verify 'Avg Dots', 'Average value' for the 'Member' and 'Stakeholder' on ET radar.");

            var expectedAvgValueForMembers = _teamMemberSurveyAnswers.Average();
            var expectedAvgValueForStakeholders = _stakeholderSurveyAnswers.Average();

            var multiTeamColorList = new List<string>
            {
                 mtEtRadarPage.FilterGetTeamColor(_multiTeam2.Name),
                 mtEtRadarPage.FilterGetTeamColor(_multiTeam1.Name),
            };
            foreach (var color in multiTeamColorList)
            {
                //Member
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    // Verify Avg count    
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", color, comp), $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", color, comp).First()), $"Competency: <{comp}> avg value doesn't match");
                }
                //Stakeholder
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForStakeholder)
                {
                    // Verify Avg count
                    Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", color, comp), $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", color, comp).First()), $"Competency: <{comp}> avg value doesn't match");
                }
            }

            Log.Info($"Check {_enterpriseTeam.Name} checkbox and verify 'Individual Dots','Avg Dots' and 'Average value' for 'Member' and 'Stakeholder' on ET radar.");
            mtEtRadarPage.FilterTeamTabSelectFilterItemCheckboxByName(_enterpriseTeam.Name);
            var radarDotsColor = mtEtRadarPage.FilterGetTeamColor(_enterpriseTeam.Name);
            //Member
            foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
            {
                // Verify Individual count
                Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                // Verify Avg count
                Assert.AreEqual(1, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
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

            Log.Info("Verify 'Individual','Avg Dots' for 'Member' and 'Stakeholder' on ET radar.");
            mtEtRadarPage.FilterTeamTabSelectFilterItemCheckboxByName(_multiTeam2.Name, false);
            mtEtRadarPage.FilterTeamTabSelectFilterItemCheckboxByName(_multiTeam1.Name, false);
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

            Log.Info("Verify 'Hide Multi-Team Names' functionality.");
            mtEtRadarPage.SelectHideMultiTeamNamesCheckbox();
            Assert.AreEqual(mtEtRadarPage.GetMultiTeamName(_multiTeam2.Name), "Team 1", $"Multi team '{_multiTeam1.Name}' name doesn't match.");
            Assert.AreEqual(mtEtRadarPage.GetMultiTeamName(_multiTeam1.Name), "Team 2", $"Multi team '{_multiTeam2.Name}' name doesn't match.");
            mtEtRadarPage.SelectHideMultiTeamNamesCheckbox(false);

            Log.Info("Verify 'Select All' and 'Clear All' functionality.");
            mtEtRadarPage.FilterClickOnSelectAllLink();
            _allMtEtTeamNamesList.ForEach(team => Assert.IsTrue(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(team), $"{team} filter checkbox isn't selected"));
            mtEtRadarPage.FilterClickOnClearAllLink();
            _allMtEtTeamNamesList.ForEach(team => Assert.IsFalse(mtEtRadarPage.FilterTeamTabIsFilterItemCheckboxSelected(team), $"{team} filter checkbox is selected"));

            Log.Info("Verify 'Tags' filter is present in the filter list.");
            expectedFilterTag = "Tags";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present. ");

            Log.Info("Verify all tags are available with their respective category.");
            mtEtRadarPage.FilterClickOnTagsTab();
            Assert.That.ListsAreEqual(_categoryList, mtEtRadarPage.FilterGetAllCategoryList(), "Category list doesn't match.");
            foreach (var category in _categoryList)
            {
                var tags = _teamProfileResponse.First().TeamTags.Where(a => a.Category.Equals(category)).Select(a => a.Tags).First().ListToString();
                Assert.AreEqual(mtEtRadarPage.FilterGetTagName(category), tags, $"Tags for the respective category {category} doesn't match.");
            }

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for member and stakeholders on ET Radar.");
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
                //Member
                foreach (var comp in Constants.TeamHealth2CompentenciesLableForMember)
                {
                    // Verify Individual count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
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
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
            }

            Log.Info("Verify 'Roles' filter is present in the filter list.");
            expectedFilterTag = "Roles";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for members and stakeholders");
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
                    // Verify Individual count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("avg", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorHexForDeveloperRole, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
            }
            var stakeHolderRolesList = mtEtRadarPage.GetStakeholdersRoleTagsList();
            //Stakeholder
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
                    // Verify Individual count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("avg", colorHexForDeveloperRole, comp),
                            $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForStakeholders, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", colorHexForDeveloperRole, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
            }

            Log.Info("Verify 'Participant Groups' filter is present in the filter list.");
            expectedFilterTag = "Participant Groups";
            Assert.IsTrue(mtEtRadarPage.FilterGetAllTabsList().Contains(expectedFilterTag), $"'{expectedFilterTag}' filter doesn't present.");
            mtEtRadarPage.FilterClickOnParticipantGroupsTab();

            Log.Info("Verify 'Individual Dots','Avg Dots' and 'Average value' for members on ET Radar.");
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
                    // Verify Individual count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("dots", radarDotsColor, comp),
                        $"Competency: <{comp}> dot count doesn't match");
                    // Verify Avg count
                    Assert.AreEqual(0, mtEtRadarPage.GetRadarDotsCount("avg", radarDotsColor, comp),
                        $"Competency: <{comp}> avg count doesn't match");
                    // Verify Avg value
                    Assert.AreEqual(expectedAvgValueForMembers, int.Parse(mtEtRadarPage.GetRadarDotsAvgValue("avg", radarDotsColor, comp).First()),
                        $"Competency: <{comp}> avg value doesn't match");
                }
            }
        }
    }
}
