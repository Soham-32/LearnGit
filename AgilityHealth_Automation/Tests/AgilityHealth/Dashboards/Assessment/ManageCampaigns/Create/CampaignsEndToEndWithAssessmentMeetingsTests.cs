using System.Linq;
using AtCommon.Utilities;
using AtCommon.ObjectFactories;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AtCommon.Dtos.CampaignsV2;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.ManageCampaigns.create
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class CampaignsEndToEndWithAssessmentMeetingsTests : BaseTest
    {
        private static readonly CampaignDetails CreateCampaignDetails = ManageCampaignsV2Factory.GetCampaignDetails();
        private static readonly SelectTeamsDetails SelectTeamsDetails = ManageCampaignsV2Factory.GetSelectTeamsDetails();
        private static readonly SelectFacilitatorsDetails SelectFacilitatorsDetails = ManageCampaignsV2Factory.GetSelectFacilitatorsDetails();

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageCampaignsTab_CreateCampaign_SplitMeeting()
        {
            ManageCampaigns_EndToEnd(FacilitationApproach.SplitMeeting);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageCampaignsTab_CreateCampaign_OneMeeting()
        {
            ManageCampaigns_EndToEnd(FacilitationApproach.OneMeeting);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageCampaignsTab_CreateCampaign_RetroMeeting()
        {
            ManageCampaigns_EndToEnd(FacilitationApproach.RetroOnly);
        }

        public void ManageCampaigns_EndToEnd(FacilitationApproach approach)
        {
            var login = new LoginPage(Driver, Log);
            var teamDashBoardPage = new TeamDashboardPage(Driver, Log);
            var assessmentDashboardListTabPage = new AssessmentDashboardListTabPage(Driver, Log);
            var manageCampaignsTabPage = new ManageCampaignsTabPage(Driver, Log);
            var campaignDetailsPage = new CampaignDetailsPage(Driver, Log);
            var selectTeamsPage = new SelectTeamsPage(Driver, Log);
            var selectFacilitatorPage = new SelectFacilitatorsPage(Driver, Log);
            var autoMatchmakingPage = new AutoMatchMakingPage(Driver, Log);
            var setUpAssessmentsPage = new SetUpAssessmentsPage(Driver, Log);
            var teamNamesList = ManageCampaignsV2Factory.TeamNamesList();
            var facilitatorFirstNamesList = ManageCampaignsV2Factory.FacilitatorFirstNamesList();
            var radarTypeList = ManageCampaignsV2Factory.RadarTypeList();
            var parentTeamList = ManageCampaignsV2Factory.ParentTeamList();
            var targetNoOfTeamsPerFacilitatorList = ManageCampaignsV2Factory.TargetNoPerFacilitatorList();
            var reviewAndSubmitPage = new ReviewAndSubmitPage(Driver, Log);
            var viewCampaignPage = new ViewCampaignPage(Driver, Log);

            Log.Info("Login to the application and Navigate to the 'Assessment dashboard' page, Go to 'Manage Campaigns' Tab and click on 'Create New Campaign' button");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashBoardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            manageCampaignsTabPage.ClickOnCreateNewCampaignsButton();

            Log.Info("Verify that 'Create New Campaign' button is disabled without filling campaign details");
            Assert.IsTrue(!campaignDetailsPage.IsCreateACampaignButtonEnabled(),
                "'Create A Campaign' button is enabled");

            Log.Info("Verify that 'Stepper' is not present on the 'Create Campaign' page");
            Assert.IsTrue(!campaignDetailsPage.IsWizardStepperPresent(),
                "Stepper is present on the 'Create Campaign' page");

            Log.Info("Verify dropdown list items");
            var actualRadarTypeList = campaignDetailsPage.GetRadarTypeList();
            var actualParentTeamList = campaignDetailsPage.GetParentTeamList();
            var actualTargetNoOfTeamsPerFacilitatorList = campaignDetailsPage.GetTargetNumberOfTeamsPerFacilitatorList();

            Assert.IsTrue(radarTypeList.All(item => actualRadarTypeList.Contains(item)),
                "'Radar Type' list does not contain all elements of expected list.");
            Assert.IsTrue(parentTeamList.All(item => actualParentTeamList.Contains(item)),
                "'Parent Team' list does not contain all elements of expected list.");
            Assert.IsTrue(targetNoOfTeamsPerFacilitatorList.All(item => actualTargetNoOfTeamsPerFacilitatorList.Contains(item)),
                "'Target Number of Teams Per Facilitator' list does not contain all elements of expected list.");

            Log.Info("Enter 'Create Campaign' details on 'Campaign Details' page and click on 'Continue To Setup' button");
            campaignDetailsPage.EnterCampaignDetailsInfo(CreateCampaignDetails);
            campaignDetailsPage.ClickOnCreateACampaign();

            Log.Info("Click on 'Add Teams' button and verify 'Close' icon is displayed");
            selectTeamsPage.ClickOnAddTeamsButton();
            Assert.IsTrue(selectTeamsPage.IsSelectTeamAndFacilitatorModelCloseButtonDisplayed(),
                "'Close' icon is not displayed on 'Select Team' popup");

            Log.Info("Click on 'Close' icon and verify that 'Add Teams' button is displayed");
            selectTeamsPage.ClickOnSelectTeamAndFacilitatorModelCloseButton();
            Assert.IsTrue(selectTeamsPage.IsAddTeamsButtonDisplayed(),
                "'Add teams' button is not displayed on 'Select Team' page");

            Log.Info("Click on 'Add teams' button and verify that 'Team Name' list is in alphabetical order");
            selectTeamsPage.ClickOnAddTeamsButton();
            Assert.IsTrue(selectTeamsPage.IsListSorted(selectTeamsPage.GetItemsListByColumnName("Team Name", true, "Teams")), "'Team Name' column list is not sorted in alphabetical order");

            Log.Info("Filter with 'Search by team name' and verify the list");
            selectTeamsPage.SearchWithTeamName(SelectTeamsDetails.TeamName, true);
            Assert.That.ListContains(selectTeamsPage.GetItemsListByColumnName("Team Name", true, "Teams"),
                SelectTeamsDetails.TeamName, "Team Name is not present in Team Name List");
            selectTeamsPage.ClickOnResetFiltersButton(true, "Teams");

            Log.Info("Filter with 'Search by Tags' and verify the list");
            selectTeamsPage.SearchWithTagName(SelectTeamsDetails.Tag, true);
            Assert.That.ListContains(selectTeamsPage.GetTagNameColumnList(SelectTeamsDetails.Tag, true),
                SelectTeamsDetails.Tag, "Tag is not present in Tag Name List");
            selectTeamsPage.ClickOnResetFiltersButton(true, "Teams");

            Log.Info("Filter with 'Work type' and verify the list");
            selectTeamsPage.SelectFilterByWorkType(SelectTeamsDetails.WorkType, true);
            Assert.That.ListContains(selectTeamsPage.GetItemsListByColumnName("Work Type", true, "Teams"),
                SelectTeamsDetails.WorkType, "Work Type is not present in Work Type List");
            selectTeamsPage.ClickOnResetFiltersButton(true, "Teams");

            Log.Info("Verify that 'Reset Filters' button is enabled after selecting 'No' from the 'Team Contact is AHF'");
            selectTeamsPage.SelectTeamContactIsAhf(SelectTeamsDetails.TeamContactIsAhf, true);
            Assert.IsTrue(selectTeamsPage.IsResetFilterButtonEnabledDisabled(),"'Reset Filters' button is not enabled");
            selectTeamsPage.ClickOnResetFiltersButton(false);

            Log.Info("Select team and verify that 'Add To Campaign' button is enabled");
            selectTeamsPage.SelectTeamCheckBox(teamNamesList);
            Assert.IsTrue(selectTeamsPage.IsAddToCampaignButtonEnabled(),
                "'Add To Campaign' button is disabled after selecting the team");

            Log.Info("Deselect team and verify that 'Add To Campaign' button is disabled");
            selectTeamsPage.SelectTeamCheckBox(teamNamesList);
            Assert.IsTrue(!selectTeamsPage.IsAddToCampaignButtonEnabled(),
                "'Add To Campaign' button is enabled after deselecting the team");

            //Add teams and next
            Log.Info("Select Team, Click on 'Add To Campaign' button and verify team is added in campaigns grid");
            selectTeamsPage.SelectTeamCheckBox(teamNamesList);
            selectTeamsPage.ClickOnAddToCampaignButton();
            foreach (var team in teamNamesList)
            {
                Assert.That.ListContains(selectTeamsPage.GetItemsListByColumnName("Team Name", false), team,
                    "Selected team is not added to the campaigns grid");
            }

            Log.Info("Click on 'Continue To Facilitator' button and verify 'Continue To Matchmaking' button is disabled");
            selectTeamsPage.ClickOnContinueToFacilitatorButton();
            Assert.IsTrue(!selectFacilitatorPage.IsContinueToMatchmakingButtonEnabled(),
                "'Continue To Matchmaking' button is enabled");

            Log.Info("Click on 'Add Facilitators' button and verify 'Close' icon is displayed");
            selectFacilitatorPage.ClickOnAddFacilitatorsButton();
            Assert.IsTrue(selectFacilitatorPage.IsSelectTeamAndFacilitatorModelCloseButtonDisplayed(),
                "'Close' icon is not displayed on 'Select Facilitators' popup");

            Log.Info("Click on 'Close' icon of 'Add Facilitators' popup and verify that 'Add Facilitators' button is displayed");
            selectFacilitatorPage.ClickOnSelectTeamAndFacilitatorModelCloseButton();
            Assert.IsTrue(selectFacilitatorPage.IsAddFacilitatorsButtonDisplayed(),
                "'Add Facilitators' button is not displayed");

            Log.Info("Verify that 'Facilitators' list is in alphabetical order");
            selectFacilitatorPage.ClickOnAddFacilitatorsButton();
            Assert.IsTrue(selectFacilitatorPage.IsListSorted(selectFacilitatorPage.GetItemsListByColumnName("Facilitator", true)),
                "'Facilitators' column list is not sorted in alphabetical order");

            Log.Info("Verify that 'Search' filter is working for the 'Select Facilitator' popup");
            selectFacilitatorPage.SearchWithFacilitatorName(SelectFacilitatorsDetails.FirstName, true);
            Assert.That.ListContains(selectFacilitatorPage.GetItemsListByColumnName("First Name", true),
                SelectFacilitatorsDetails.FirstName, $"{SelectFacilitatorsDetails.FirstName} is not present in First Name column");
            selectFacilitatorPage.ClickOnResetFiltersButton(true);
            selectFacilitatorPage.SearchWithFacilitatorName(SelectFacilitatorsDetails.LastName, true);
            Assert.That.ListContains(selectFacilitatorPage.GetItemsListByColumnName("Last Name", true),
                SelectFacilitatorsDetails.LastName, $"{SelectFacilitatorsDetails.LastName} is not present in Last Name column");
            selectFacilitatorPage.ClickOnResetFiltersButton(true);

            Log.Info("Select facilitator and verify that 'Add To Campaign' button is enabled");
            selectFacilitatorPage.SelectFirstNameCheckBox(facilitatorFirstNamesList);
            Assert.IsTrue(selectFacilitatorPage.IsAddToCampaignButtonIsEnabled(),
                "'Add To Campaign' button is disabled after selecting the facilitator");

            Log.Info("Select facilitator, Click on 'Add To Campaign' button and verify the added facilitator");
            selectFacilitatorPage.SelectFirstNameCheckBox(facilitatorFirstNamesList);
            selectFacilitatorPage.ClickOnAddToCampaignButton();
            foreach (var facilitator in facilitatorFirstNamesList)
            {
                Assert.That.ListContains(selectFacilitatorPage.GetItemsListByColumnName("First Name", false),
                    facilitator, "Selected facilitator is not displayed");
            }

            Log.Info("Click on 'Continue To Matchmaking' button and verify 'Continue To SetUp Assessments' button is displayed");
            selectFacilitatorPage.ClickOnContinueToMatchmakingButton();
            Assert.IsTrue(autoMatchmakingPage.IsContinueToSetUpAssessmentsButtonDisplay(),
                "'Continue To SetUp Assessments' button is not displayed");

            Log.Info("Verify that 'Number of Teams', 'Number of Facilitators' and 'Target Number of Teams Per Facilitator' count");
            Assert.AreEqual(teamNamesList.Count(), autoMatchmakingPage.GetCountOfSelectedTeams(),
                "'Number of Teams' count is not matched");
            Assert.AreEqual(facilitatorFirstNamesList.Count(),
                autoMatchmakingPage.GetCountOfSelectedFacilitators(),
                "'Number of Facilitators' count is not matched");
            Assert.IsTrue(
                CreateCampaignDetails.TeamsPerFacilitator.Contains(autoMatchmakingPage
                    .GetTargetNumberOfTeamsPerFacilitatorCount().ToString()),
                "'Target Number of Teams Per Facilitator' count is not matched");

            Log.Info("Click on 'Facilitator View' button and verify that button color is changed");
            const string expectedButtonColor = "rgba(45, 168, 224, 1)";
            autoMatchmakingPage.ClickOnFacilitatorViewButton();
            Assert.AreEqual(expectedButtonColor, autoMatchmakingPage.GetButtonColorByName("Facilitator View"),
                "'Facilitator View' button color is not changed");

            Log.Info("Click on 'Team Contact View' button and verify that button color is changed");
            autoMatchmakingPage.ClickOnTeamContactViewButton();
            Assert.AreEqual(expectedButtonColor, autoMatchmakingPage.GetButtonColorByName("Team Contact View"),
                "'Team contact view' button color is not changed");

            Log.Info("Verify that without 'Export To Excel' button is disabled without clicking on the 'Create Auto Matches'");
            Assert.IsTrue(!autoMatchmakingPage.IsExportToExcelButtonEnabled(), "'Export To Excel' button is enabled");

            const string fileName = "campaign_matchmaking_results.xlsx";
            FileUtil.DeleteFilesInDownloadFolder(fileName);

            var excelColumns = new List<string>
            {
                "Team Name", "Team Contact Name", "Team Contact Email", "Team Contact No. of Teams",
                "Facilitator Name", "Facilitator Email", "Facilitation's Experience", "Teams Assigned"
            };

            var teamContactViewTableColumnsDict = new Dictionary<string, string>
            {
                { "Team Name", "teamName" },
                { "Team Contact Name", "teamContactName" },
                { "No. of Teams", "teamContactTeams" },
                { "Facilitator Name", "facilitatorName" },
                { "Teams Assigned", "teamsToFacilitate" },
                { "Facilitation's Experience", "facilitationsExperience" }
            };

            Log.Info("Click on 'Create Auto Matches' button then verify 'ReCreate Auto Matches' button is displayed and 'Export To Excel' button is enabled");
            autoMatchmakingPage.ClickOnCreateAutoMatchesButton();
            Assert.IsTrue(autoMatchmakingPage.IsReCreateAutoMatchesButtonDisplayed(),
                "'ReCreate Auto Matches' button is not displayed");
            Assert.IsTrue(autoMatchmakingPage.IsExportToExcelButtonEnabled(),
                "'Export To Excel' button is disabled after clicking on the 'Create Auto Matches'");

            autoMatchmakingPage.ClickOnExportToExcelButton();
            var spreadsheet = FileUtil.WaitUntilFileDownloaded(fileName);
            var tbl = ExcelUtil.GetExcelData(spreadsheet);

            var matchingColumns = autoMatchmakingPage.GetMatchingColumns(excelColumns, teamContactViewTableColumnsDict.Values.ToList());

            foreach (var column in matchingColumns)
            {
                var key = teamContactViewTableColumnsDict.FirstOrDefault(x => x.Value == column).Key;

                Assert.That.ListsAreEqual(autoMatchmakingPage.GetTableColumnValuesList(key),                    autoMatchmakingPage.GetExcelColumnData(tbl, column), $"Column data for '{column}' is not matched");
            }

            Log.Info("Click on 'Continue to SetUp Assessments' button and verify 'Continue To Review' button is disabled");
            autoMatchmakingPage.ClickOnContinueToSetUpAssessmentsButton();
            Assert.IsTrue(!setUpAssessmentsPage.IsContinueToReviewButtonEnabled(),
                "'Continue To Review' button is enabled");

            Log.Info($"Enter 'Assessment Name', select {approach} select all dates and click on 'Continue To Review' button");
            var setupAssessmentsDetails = ManageCampaignsV2Factory.GetSetupAssessmentDetails(approach);
            setUpAssessmentsPage.EnterSetupAssessmentsInfo(setupAssessmentsDetails);
            setUpAssessmentsPage.ClickOnContinueToReviewButton();
            Assert.IsTrue(reviewAndSubmitPage.IsLaunchCampaignButtonDisplayed(), "'Launch Campaign' button is not displayed");

            Log.Info("Verify 'Campaign Name', 'Radar Type', 'Start Date', 'End Date' from 'Campaign details' section");
            Assert.AreEqual(CreateCampaignDetails.Name, reviewAndSubmitPage.GetCampaignDetailsCampaignName(), "'Campaign Name' is not matched");
            Assert.AreEqual(CreateCampaignDetails.RadarType, reviewAndSubmitPage.GetCampaignDetailsRadarType(),
                "'Radar type' is not matched");
            Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(CreateCampaignDetails.StartDate),
                reviewAndSubmitPage.GetCampaignDetailsStartDate(), "'Start Date' is not matched");
            Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(CreateCampaignDetails.EndDate),
                reviewAndSubmitPage.GetCampaignDetailsEndDate(), "'End Date' is not matched");


            switch (approach)
            {
                case FacilitationApproach.SplitMeeting:
                    Log.Info("Verify 'Assessment Name', 'Stakeholder Launch Date', 'Team Member Launch Date', 'Assessment Close Date', 'Retrospective Window Start' and 'Retrospective Window End' from the 'Facilitation Approach' section");
                    reviewAndSubmitPage.ClickOnFacilitationApproachExpandMoreButton();
                    Assert.AreEqual(setupAssessmentsDetails.Name,
                        reviewAndSubmitPage.GetFacilitationApproachAssessmentName(), "'Assessment Name' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderLaunchDate),
                        reviewAndSubmitPage.GetFacilitationApproachStakeholderLaunchDate(), "'Stakeholder Launch Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.TeamMemberLaunchDate),
                        reviewAndSubmitPage.GetFacilitationApproachTeamMemberLaunchDate(), "'Team Member Launch Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
                case FacilitationApproach.OneMeeting:
                    Log.Info("Verify 'Assessment Name', 'Stakeholder Window start date', 'Stakeholder Window end date', 'Team Member launch Date',  'Retrospective Window End' from 'Facilitation Approach' section");
                    reviewAndSubmitPage.ClickOnFacilitationApproachExpandMoreButton();
                    Assert.AreEqual(setupAssessmentsDetails.Name,
                        reviewAndSubmitPage.GetFacilitationApproachAssessmentName(), "'Assessment Name' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderWindowStart),
                        reviewAndSubmitPage.GetFacilitationApproachStakeholderWindowStart(), "'Stakeholder window start Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderWindowEnd),
                        reviewAndSubmitPage.GetFacilitationApproachStakeholderWindowEnd(), "'Stakeholder window end Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.TeamMemberLaunchDateForOneMeeting),
                        reviewAndSubmitPage.GetFacilitationApproachTeamMemberLaunchDate(), "'Tea member launch date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
                case FacilitationApproach.RetroOnly:
                    Log.Info("Verify 'Assessment Name', 'Assessment start Date', 'Assessment Close Date', 'Retrospective Window Start' and 'Retrospective Window End' from 'Facilitation Approach' section");
                    reviewAndSubmitPage.ClickOnFacilitationApproachExpandMoreButton();
                    Assert.AreEqual(setupAssessmentsDetails.Name,
                        reviewAndSubmitPage.GetFacilitationApproachAssessmentName(), "'Assessment Name' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.AssessmentStartDate),
                        reviewAndSubmitPage.GetFacilitationApproachAssessmentStartDate(), "'Assessment start Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.AssessmentCloseDate),
                        reviewAndSubmitPage.GetFacilitationApproachAssessmentCloseDate(), "'Assessment close Date' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(reviewAndSubmitPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        reviewAndSubmitPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
            }

            Log.Info("Verify 'Team Name' and 'Name' from 'Team selected for campaign' section");
            reviewAndSubmitPage.ClickOnTeamSelectedForCampaignExpandMoreButton();
            Assert.That.ListsAreEqual(teamNamesList, reviewAndSubmitPage.GetTeamSelectColumnValuesList("Team Name"),
                "Team Names list data are not equal");

            Log.Info("Verify 'First Name' and 'Email' from 'Facilitator selected for campaign' section");
            reviewAndSubmitPage.ClickOnFacilitatorSelectedForCampaignExpandMoreButton();
            Assert.That.ListsAreEqual(facilitatorFirstNamesList, reviewAndSubmitPage.GetFacilitatorSelectColumnValuesList("First Name"),
                "First Names list data are not equal");

            Log.Info("Click on 'Launch Campaign' button and verify that user is navigated to 'Manage Campaigns' page");
            reviewAndSubmitPage.ClickOnLaunchCampaignButton();
            Assert.AreEqual($"{BaseTest.ApplicationUrl}/company/{Company.Id}/ManageCampaignsDashboard",
                Driver.GetCurrentUrl(), "Manage Campaigns page url is not matched");
            Assert.AreEqual("Manage Campaigns", manageCampaignsTabPage.GetManageCampaignHeaderTitleText(),
                "'Manage Campaigns' text is not matched");

            //Manage Tab page and next
            Log.Info("Verify that created campaign name is displayed on 'Manage Campaigns' page");
            Assert.That.ListContains(manageCampaignsTabPage.GetListByColumnName("Campaign Name"), CreateCampaignDetails.Name, $"{CreateCampaignDetails.Name} is not available in the 'Campaign Name' list");

            Log.Info("Click on 'View' button by campaign name and verify that created campaign name is displayed");
            manageCampaignsTabPage.ClickOnViewCampaignsButton(CreateCampaignDetails.Name);
            Assert.AreEqual(CreateCampaignDetails.Name, viewCampaignPage.GetCampaignName(),
                "'Campaign Name' text is not matched");
            Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(CreateCampaignDetails.StartDate),
                viewCampaignPage.GetCampaignDetailsStartDate(), "'Start Date' text is not matched");
            Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(CreateCampaignDetails.EndDate),
                viewCampaignPage.GetCampaignDetailsEndDate(), "'End Date' text is not matched");
            Assert.AreEqual(CreateCampaignDetails.RadarType, viewCampaignPage.GetCampaignDetailsRadarType(),
                "'Radar Type' text is not matched");
            viewCampaignPage.MoveToPageDown();

            switch (approach)
            {
                case FacilitationApproach.SplitMeeting:
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderLaunchDate),
                        viewCampaignPage.GetFacilitationApproachStakeholderLaunchDate(), "'Stakeholder Launch Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.TeamMemberLaunchDate),
                        viewCampaignPage.GetFacilitationApproachTeamMemberLaunchDate(), "'Team Member Launch Date' is not matched");
                    Assert.AreEqual(
                        viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(
                        viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
                case FacilitationApproach.OneMeeting:
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderWindowStart),
                        viewCampaignPage.GetFacilitationApproachStakeholderWindowStart(), "'Stakeholder window start Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.StakeholderWindowEnd),
                        viewCampaignPage.GetFacilitationApproachStakeholderWindowEnd(), "'stakeholder window end Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.TeamMemberLaunchDateForOneMeeting),
                        viewCampaignPage.GetFacilitationApproachTeamMemberLaunchDate(), "'Team Member Launch Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
                case FacilitationApproach.RetroOnly:
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.AssessmentStartDate),
                        viewCampaignPage.GetFacilitationApproachAssessmentStartDate(), "'Assessment start Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.AssessmentCloseDate),
                        viewCampaignPage.GetFacilitationApproachAssessmentCloseDate(), "'Assessment close Date' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowStart),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowStart(), "'Retrospective Window Start' is not matched");
                    Assert.AreEqual(viewCampaignPage.GetDateInMmDdYyyyFormat(setupAssessmentsDetails.RetrospectiveWindowEnd),
                        viewCampaignPage.GetFacilitationApproachRetrospectiveWindowEnd(), "'Retrospective Window End' is not matched");
                    break;
            }
        }
    }
}



