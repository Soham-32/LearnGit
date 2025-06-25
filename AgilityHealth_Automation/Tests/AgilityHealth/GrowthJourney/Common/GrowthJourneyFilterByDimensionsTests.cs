using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthJourney;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AtCommon.Dtos.Assessments.Team.Custom;

namespace AgilityHealth_Automation.Tests.AgilityHealth.GrowthJourney.Common
{

    [TestClass]
    [TestCategory("GrowthJourney"), TestCategory("Critical")]
    [TestCategory("Smoke")]
    public class GrowthJourneyFilterByDimensionsTests : BaseTest
    {
        private static bool _classInitFailed;
        private static SetUpMethods _setupUi;
        private static SetupTeardownApi _setup;
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;
        private static TeamHierarchyResponse _multiTeam;
        private static TeamHierarchyResponse _enterpriseTeam;
        private static RadarResponse _radar;
        private static List<AssessmentsResultsResponse> _surveyResult;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _setupUi = new SetUpMethods(testContext, TestEnvironment);
                _setup = new SetupTeardownApi(TestEnvironment);

                _team = _setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.TeamForGrowthJourney2).CheckForNull($"<{Constants.TeamForGrowthJourney2}> was not found in the response.");

                _teamAssessment = new TeamAssessmentInfo
                {
                    AssessmentType = SharedConstants.TeamAssessmentType,
                    AssessmentName = RandomDataUtil.GetAssessmentName(),
                    TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() }
                };
                _setupUi.AddTeamAssessment(_team.TeamId, _teamAssessment);
                var emailSearch = new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                };
                _setupUi.CompleteTeamMemberSurvey(emailSearch);
                _setupUi.CloseTeamAssessment(_team.TeamId, _teamAssessment.AssessmentName);

                _multiTeam = _setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.MultiTeamForGrowthJourney2).CheckForNull($"<{Constants.MultiTeamForGrowthJourney2}> was not found in the response.");
                _enterpriseTeam = _setup.GetCompanyHierarchy(Company.Id).GetTeamByName(Constants.EnterpriseTeamForGrowthJourney2).CheckForNull($"<{Constants.EnterpriseTeamForGrowthJourney2}> was not found in the response.");

                _radar = _setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType);
                _surveyResult = _setup.GetAssessmentsResults(Constants.TeamForGrowthJourney2).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("KnownDefect")] //Bug Id: 45551
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void TeamGrowthJourneyFilterByDimensions()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            Log.Info($"Navigate to Growth Journey page for the team {Constants.TeamForGrowthJourney2}");
            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            growthJourneyPage.NavigateToGrowthJourneyPage(_team.TeamId);

            Log.Info(
                "Verify that by default all the dimension are present on the 'Growth Journey radar' and 'Compare Radar Analysis' table");
            growthJourneyPage.ClickOnDimensionFilterDropdown();
            var dimensionNamesFromDropdown = growthJourneyPage.GetDimensionsListFromFilterDropdown();
            var dimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
            Assert.That.ListsAreEqual(dimensionNamesFromChart, dimensionNamesFromDropdown,
                "All the dimensions are not displayed by default in the 'Growth Journey Radar'");
            var dimensionNamesFromGrid = growthJourneyPage.GetDimensionListFromAnalysisGrid();
            Assert.That.ListsAreEqual(dimensionNamesFromGrid, dimensionNamesFromDropdown,
                "All the dimensions are not displayed by default in 'Compare radar Analysis' grid");

            foreach (var dimension in dimensionNamesFromDropdown)
            {
                Log.Info(
                    $"Verify data is filtered by Dimension - {dimension} and all the sub-dimensions are displayed on 'Growth Journey radar' and 'Compare Radar Analysis' table ");
                growthJourneyPage.SelectDimensionFromFilterDropdown(dimension);
                growthJourneyPage.ClickOnSubDimensionFilterDropdown();
                var subDimensionNamesFromDropdown = growthJourneyPage.GetSubDimensionsFromFilterDropdown();
                var subDimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
                Assert.That.ListsAreEqual(subDimensionNamesFromChart, subDimensionNamesFromDropdown,
                    "All the sub-dimensions are not displayed in 'Growth Journey Radar'");
                var subDimensionNamesFromGrid = growthJourneyPage.GetSubDimensionListFromAnalysisGrid();
                Assert.That.ListsAreEqual(subDimensionNamesFromGrid, subDimensionNamesFromDropdown,
                    "All the sub-dimensions are not displayed in 'Compare Radar Analysis' grid");

                foreach (var subDimension in subDimensionNamesFromDropdown)
                {
                    Log.Info($"Verify data is filtered by Dimension {dimension} and sub-dimension {subDimension} from the filter dropdown");
                    var assessmentColumn = 0;
                    growthJourneyPage.SelectSubDimensionFromFilterDropdown(subDimension);
                    var competencyFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart().Select(a => a.RemoveWhitespace()).ToList();
                    foreach (var assessmentsResultsResponse in _surveyResult)
                    {

                        var competencyForSurveyValue = assessmentsResultsResponse.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))?.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.Select(a => a.Name).ToList().CheckForNull();
                        competencyForSurveyValue.Remove("Stakeholder");
                        competencyForSurveyValue.Remove("Product Owner");


                        var actualCompetencyList = competencyForSurveyValue.Select(a => a.RemoveWhitespace()).ToList();
                        Assert.That.ListsAreEqual(competencyFromChart, actualCompetencyList, "All the competency are not displayed in 'Growth Journey Radar' chart");
                        var competencyFromGrid = growthJourneyPage.GetCompetencyListFromAnalysisGrid().Select(a => a.RemoveWhitespace()).ToList();
                        Assert.That.ListsAreEqual(competencyFromGrid, actualCompetencyList, "All the competency are not displayed in 'Compare Radar Analysis' table");

                        foreach (var competency in competencyForSurveyValue)
                        {
                            var actualSurveyResult = assessmentsResultsResponse.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))!.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.FirstOrDefault(a => a.Name.Equals(competency))!.AverageValue;

                            Log.Info(
                                $"Verify that Competency - {competency} is having survey value as - {actualSurveyResult} ");
                            var surveyResultFromGrid =
                                growthJourneyPage.GetSurveyValueFromAnalysisGridForAssessment(competency.TrimEnd());
                            if (actualSurveyResult != null)
                                Assert.AreEqual(Math.Round((double)actualSurveyResult * 10),
                                    Math.Round((double)(int.Parse(surveyResultFromGrid[assessmentColumn]))),
                                    "Survey Result doesn't match");
                        }

                        assessmentColumn++;
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("KnownDefect")] //Bug Id: 45551
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void MultiTeamGrowthJourneyFilterByDimensions()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            Log.Info("Navigate to 'Growth Journey' page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Thread.Sleep(300000);  //Radar takes up few minutes for newly created Teams to plot for Growth Journey Screen
            Driver.RefreshPage();

            radarPage.NavigateToGrowthJourney(_multiTeam.TeamId, _radar.Id, TeamType.MultiTeam);

            Log.Info("Verify that by default all the dimension are present on the 'Growth Journey radar' and 'Compare Radar Analysis' table");
            growthJourneyPage.ClickOnDimensionFilterDropdown();
            var dimensionNamesFromDropdown = growthJourneyPage.GetDimensionsListFromFilterDropdown();
            var dimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
            Assert.That.ListsAreEqual(dimensionNamesFromChart, dimensionNamesFromDropdown, "All the dimensions are not displayed by default in the 'Growth Journey Radar'");
            var dimensionNamesFromGrid = growthJourneyPage.GetDimensionListFromAnalysisGrid();
            Assert.That.ListsAreEqual(dimensionNamesFromGrid, dimensionNamesFromDropdown, "All the dimensions are not displayed by default in 'Compare radar Analysis' grid");

            foreach (var dimension in dimensionNamesFromDropdown)
            {
                Log.Info($"Verify data is filtered by Dimension - {dimension} and all the sub-dimensions are displayed on 'Growth Journey radar' and 'Compare Radar Analysis' table ");
                growthJourneyPage.SelectDimensionFromFilterDropdown(dimension);
                growthJourneyPage.ClickOnSubDimensionFilterDropdown();
                var subDimensionNamesFromDropdown = growthJourneyPage.GetSubDimensionsFromFilterDropdown();
                var subDimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
                Assert.That.ListsAreEqual(subDimensionNamesFromChart, subDimensionNamesFromDropdown, "All the sub-dimensions are not displayed in 'Growth Journey Radar'");
                var subDimensionNamesFromGrid = growthJourneyPage.GetSubDimensionListFromAnalysisGrid();
                Assert.That.ListsAreEqual(subDimensionNamesFromGrid, subDimensionNamesFromDropdown, "All the sub-dimensions are not displayed in 'Compare Radar Analysis' grid");

                foreach (var subDimension in subDimensionNamesFromDropdown)
                {
                    Log.Info($"Verify data is filtered by Dimension {dimension} and sub-dimension {subDimension} from the filter dropdown");
                    growthJourneyPage.SelectSubDimensionFromFilterDropdown(subDimension);
                    var competencyFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart().Select(a => a.RemoveWhitespace()).ToList();
                    var competencyForSurveyValue = _surveyResult.FirstOrDefault()?.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))?.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.Select(a => a.Name).ToList();
                    competencyForSurveyValue.Remove("Stakeholder");
                    competencyForSurveyValue.Remove("Product Owner");

                    var actualCompetencyList = competencyForSurveyValue.Select(a => a.RemoveWhitespace()).ToList();
                    Assert.That.ListsAreEqual(competencyFromChart, actualCompetencyList, "All the competency are not displayed in 'Growth Journey Radar' chart");
                    var competencyFromGrid = growthJourneyPage.GetCompetencyListFromAnalysisGrid().Select(a => a.RemoveWhitespace()).ToList();
                    Assert.That.ListsAreEqual(competencyFromGrid, actualCompetencyList, "All the competency are not displayed in 'Compare Radar Analysis' table");

                    foreach (var competency in competencyForSurveyValue)
                    {
                        List<double> results = new List<double>();
                        foreach (var assessmentsResultsResponse in _surveyResult)
                        {
                            var actualSurveyResult = assessmentsResultsResponse.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))!.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.FirstOrDefault(a => a.Name.Equals(competency))!.AverageValue;
                            if (actualSurveyResult != null) results.Add((double)actualSurveyResult);
                        }
                        Log.Info($"Verify that Competency - {competency} is having survey value as - {results.Average()} ");
                        var surveyResultFromGrid = growthJourneyPage.GetSurveyValueFromAnalysisGrid(competency.TrimEnd());
                        var averageOfCompetenciesScores = results.Average() * 10;
                        var avg = Math.Round(averageOfCompetenciesScores, MidpointRounding.AwayFromZero);
                        Assert.AreEqual(surveyResultFromGrid, avg, "Survey Result doesn't match");
                    }
                }
            }

        }

        [TestMethod]
        [TestCategory("Sanity")]
        [TestCategory("KnownDefect")] //Bug Id: 45551
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin")]
        public void EnterpriseTeamGrowthJourneyFilterByDimensions()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var growthJourneyPage = new GrowthJourneyPage(Driver, Log);

            Log.Info("Navigate to 'Growth Journey' page");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);

            Thread.Sleep(300000);//Radar takes up few minutes for newly created Teams to plot for Growth Journey Screen
            Driver.RefreshPage();
            radarPage.NavigateToGrowthJourney(_enterpriseTeam.TeamId, _radar.Id, TeamType.EnterpriseTeam);

            Log.Info("Verify that by default all the dimension are present on the 'Growth Journey radar' and 'Compare Radar Analysis' table");
            growthJourneyPage.ClickOnDimensionFilterDropdown();
            var dimensionNamesFromDropdown = growthJourneyPage.GetDimensionsListFromFilterDropdown();
            var dimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
            Assert.That.ListsAreEqual(dimensionNamesFromChart, dimensionNamesFromDropdown, "All the dimensions are not displayed by default in the 'Growth Journey Radar'");
            var dimensionNamesFromGrid = growthJourneyPage.GetDimensionListFromAnalysisGrid();
            Assert.That.ListsAreEqual(dimensionNamesFromGrid, dimensionNamesFromDropdown, "All the dimensions are not displayed by default in 'Compare radar Analysis' grid");

            foreach (var dimension in dimensionNamesFromDropdown)
            {
                Log.Info($"Verify data is filtered by Dimension - {dimension} and all the sub-dimensions are displayed on 'Growth Journey radar' and 'Compare Radar Analysis' table ");
                growthJourneyPage.SelectDimensionFromFilterDropdown(dimension);
                growthJourneyPage.ClickOnSubDimensionFilterDropdown();
                var subDimensionNamesFromDropdown = growthJourneyPage.GetSubDimensionsFromFilterDropdown();
                var subDimensionNamesFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart();
                Assert.That.ListsAreEqual(subDimensionNamesFromChart, subDimensionNamesFromDropdown, "All the sub-dimensions are not displayed in 'Growth Journey Radar'");
                var subDimensionNamesFromGrid = growthJourneyPage.GetSubDimensionListFromAnalysisGrid();
                Assert.That.ListsAreEqual(subDimensionNamesFromGrid, subDimensionNamesFromDropdown, "All the sub-dimensions are not displayed in 'Compare Radar Analysis' grid");

                foreach (var subDimension in subDimensionNamesFromDropdown)
                {
                    Log.Info($"Verify data is filtered by Dimension {dimension} and sub-dimension {subDimension} from the filter dropdown");
                    growthJourneyPage.SelectSubDimensionFromFilterDropdown(subDimension);
                    var competencyFromChart = growthJourneyPage.GetAllTitlesFromTimeLineChart().Select(a => a.RemoveWhitespace()).ToList();
                    var competencyForSurveyValue = _surveyResult.FirstOrDefault()?.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))?.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.Select(a => a.Name).ToList();
                    competencyForSurveyValue.Remove("Stakeholder");
                    competencyForSurveyValue.Remove("Product Owner");

                    var actualCompetencyList = competencyForSurveyValue.Select(a => a.RemoveWhitespace()).ToList();
                    Assert.That.ListsAreEqual(competencyFromChart, actualCompetencyList, "All the competency are not displayed in 'Growth Journey Radar' chart");
                    var competencyFromGrid = growthJourneyPage.GetCompetencyListFromAnalysisGrid().Select(a => a.RemoveWhitespace()).ToList();
                    Assert.That.ListsAreEqual(competencyFromGrid, actualCompetencyList, "All the competency are not displayed in 'Compare Radar Analysis' table");

                    foreach (var competency in competencyForSurveyValue)
                    {
                        List<double> results = new List<double>();
                        foreach (var assessmentsResultsResponse in _surveyResult)
                        {
                            var actualSurveyResult = assessmentsResultsResponse.Dimensions.FirstOrDefault(a => a.Name.Equals(dimension))!.SubDimensions.FirstOrDefault(a => a.Name.Equals(subDimension))?.Competencies.FirstOrDefault(a => a.Name.Equals(competency))!.AverageValue;
                            if (actualSurveyResult != null) results.Add((double)actualSurveyResult);
                        }
                        Log.Info($"Verify that Competency - {competency} is having survey value as - {results.Average()} ");
                        var surveyResultFromGrid = growthJourneyPage.GetSurveyValueFromAnalysisGrid(competency.TrimEnd());
                        var averageOfCompetenciesScores = results.Average() * 10;
                        Assert.AreEqual(surveyResultFromGrid, (Math.Round(averageOfCompetenciesScores, MidpointRounding.AwayFromZero)), "Survey Result doesn't match");
                    }
                }
            }
        }

    }
}