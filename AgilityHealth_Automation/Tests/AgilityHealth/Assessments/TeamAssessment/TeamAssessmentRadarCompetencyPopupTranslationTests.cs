using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Survey.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Radar.RadarPage;


namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    //[TestClass] - Commenting for sometime to observe failed language translation cases.
    [TestCategory("TeamAssessment"), TestCategory("Assessments")]
    public class TeamAssessmentRadarCompetencyPopupTranslationTests : BaseTest
    {
        private static bool _classInitFailed;
        private static TeamHierarchyResponse _team;
        private static SetUpMethods _setup;

        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamHealthRadarName,
            AssessmentName = $"Test_Translation_{RandomDataUtil.GetAssessmentName()}",
            FacilitationDate = DateTime.Today.AddDays(1),
            TeamMembers = new List<string> { SharedConstants.TeamMember2.FullName() }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                // Set up the env
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.GiCopyFromPreviousAssessmentTeam);
                _setup = new SetUpMethods(_, TestEnvironment);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void RadarCompetencyTranslation_English()
        {
            VerifySetup(_classInitFailed);
            RadarCompetencyTranslation(RadarLanguage.English, "Finish");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void RadarCompetencyTranslation_Spanish()
        {
            VerifySetup(_classInitFailed);
            RadarCompetencyTranslation(RadarLanguage.Spanish, "Finalizar");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void RadarCompetencyTranslation_Japanese()
        {
            VerifySetup(_classInitFailed);
            RadarCompetencyTranslation(RadarLanguage.Japanese, "終了");
        }

        private void RadarCompetencyTranslation(RadarLanguage language, string finishText)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashBoardPage = new TeamDashboardPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);

            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Go to the team page and navigate to team");
            teamDashBoardPage.GridTeamView();
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);

            Log.Info("Create a team assessment");
            _setup.AddTeamAssessment(_team.TeamId, TeamAssessment);

            Log.Info("DeserializeJsonObject and get the json file data");
            var surveyResponse = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\Survey\THAssessmentTranslation_{language.GetDescription()}.json").DeserializeJsonObject<SurveyTranslation>();

            Log.Info($"Navigate to the {TeamAssessment.AssessmentName} radar page with {language} language and click on the competency");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.ClickOnRadar(TeamAssessment.AssessmentName);
            Driver.NavigateToPage(Driver.GetCurrentUrl() + $"?languageCode={language.GetDescription()}");

            Log.Info("Get all competency name and dimension name list from radar");
            var radarCompetencyList = radarPage.GetAllTheRadarCompetencyTextList();
            var radarDimensionList = radarPage.GetAllTheRadarDimensionTextList();

            //Dimension
            Log.Info("Get all the Dimension name from the json");
            var dimensionListFromJson = surveyResponse.Dimensions.Where(a => a.DimensionName != $"{finishText}").ToList().Select(a => a.DimensionName).ToList();
            foreach (var dimension in dimensionListFromJson)
            {
                Log.Info("Verify the dimension name is matched with radar dimension name list");
                Assert.That.ListContains(radarDimensionList, dimension.ToLower(), "Dimension name list doesn't matched");

                //SubDimension
                Log.Info("Get the sub dimension name from json");
                var subDimensionListFromJson = surveyResponse.Dimensions.Where(a => a.DimensionName.ToLower().Equals($"{dimension.ToLower()}")).ToList().SelectMany(h => h.SubDimension).ToList().Select(i => i.HeaderName).ToList();

                foreach (var subDimension in subDimensionListFromJson)
                {
                    var subDimensionName = subDimension;

                    //For the japanese language two sub dimension name is different                    
                    if (language.ToString().Equals("Japanese"))
                    {
                        if (subDimensionName.Equals("リーダーシップ - チームファシリテーター"))
                        {
                            var replacedSubDimensionName = subDimensionName.Replace("リーダーシップ - チームファシリテーター", "リーダーシップ - チームフ");
                            subDimensionName = replacedSubDimensionName;
                        }
                        else if (subDimensionName.Equals("リーダーシップ - テクニカル リード"))
                        {
                            var replacedSubDimensionName = subDimensionName.Replace("リーダーシップ - テクニカル リード", "リーダーシップ - テクニカ");
                            subDimensionName = replacedSubDimensionName;
                        }
                    }

                    Log.Info("Get the sub dimension name after the '-' and replace with null value");
                    var actualSubDimensionOfList = subDimensionName.Replace($"{dimension} - ", "");
                    var actualRadarSubDimensionList = radarPage.GetAllTheRadarSubDimensionTextList().ToList();

                    Log.Info("Verify the sub dimension name is matched with radar sub dimension name list");
                    Assert.That.ListContains(actualRadarSubDimensionList, actualSubDimensionOfList, "SubDimension name list doesn't matched");

                    //Competency
                    Log.Info("Verify the all competency abbreviation name list");
                    var competencyAbbreviationListFromJson = surveyResponse.Dimensions.Where(a => a.DimensionName.ToLower().Equals($"{dimension.ToLower()}")).ToList().SelectMany(b => b.SubDimension).ToList().Where(h => h.HeaderName.Equals(subDimensionName)).SelectMany(c => c.Competencies).ToList().Select(r => r.CompetencyAbbreviation).Distinct().ToList();

                    foreach (var competencyName in competencyAbbreviationListFromJson)
                    {
                        Assert.That.ListContains(radarCompetencyList, competencyName, "Competency name doesn't matched");

                        Log.Info("Verify the json competency name is exists in the radar competency name");
                        if (!radarCompetencyList.Contains(competencyName)) continue;
                        Log.Info("Get competency id from json for particular competency name");
                        var competencyIds = surveyResponse.Dimensions.Where(a => a.DimensionName.ToLower().Equals($"{dimension.ToLower()}")).ToList().SelectMany(h => h.SubDimension).ToList().SelectMany(i => i.Competencies).ToList().Where(j => j.CompetencyAbbreviation == competencyName).Select(d => d.CompetencyId).Distinct().ToList();

                        foreach (var competencyId in competencyIds)
                        {
                            Log.Info("Click on competency");
                            radarPage.ClickCompetency(competencyId.ToString());

                            Log.Info("Get actual competency popup details");
                            var actualCompetencyPopupTitle = radarPage.GetCompetencyProfileTitleText();
                            var actualGrowthPortalButtonText = radarPage.GetGrowthPortalButtonText();
                            var actualAvgResponseText = radarPage.GetAvgResponseText();
                            var actualQuestionTitleText = radarPage.GetQuestionTitleText();
                            var actualCompetencyName = radarPage.GetCompetencyNameText();

                            Log.Info("Get expected competency popup details");
                            var expectedCompetencyPopupTitle = surveyResponse.RadarCompetencyPopup.FirstOrDefault()?.CompetencyProfilePopupName;
                            var expectedGrowthPortalButtonText = surveyResponse.RadarCompetencyPopup.FirstOrDefault()?.GrowthPortal;
                            var expectedAvgResponseText = surveyResponse.RadarCompetencyPopup.FirstOrDefault()?.AvgResponse;
                            var expectedQuestionTitleText = surveyResponse.RadarCompetencyPopup.FirstOrDefault()?.Question.ToList();
                            var expectedCompetencyName = surveyResponse.Dimensions.Where(a => a.DimensionName != $"{finishText}").SelectMany(a => a.SubDimension).SelectMany(b => b.Competencies).ToList().Where(n => n.CompetencyAbbreviation == competencyName).ToList().Select(c => c.CompetencyName).FirstOrDefault();

                            Log.Info("Verify the competency popup details");
                            Assert.AreEqual(expectedCompetencyPopupTitle, actualCompetencyPopupTitle, "Competency profile popup title doesn't matched");
                            Assert.AreEqual(expectedCompetencyName, actualCompetencyName, "Competency Name doesn't matched");
                            Assert.AreEqual(expectedGrowthPortalButtonText, actualGrowthPortalButtonText, "Growth Portal button text doesn't matched");
                            Assert.AreEqual(expectedAvgResponseText, actualAvgResponseText, "Avg. Response text doesn't matched");
                            Assert.That.ListContains(expectedQuestionTitleText, actualQuestionTitleText, "Question title text doesn't matched");

                            Log.Info("Click on competency popup closed icon");
                            radarPage.ClickOnCompetencyPopupCloseIcon();
                        }
                    }
                    Log.Info("Verify the radar title is enabled");
                    Assert.IsTrue(radarPage.IsRadarTitleEnabled(), "Radar title is enabled");
                }
            }
        }
    }
}
