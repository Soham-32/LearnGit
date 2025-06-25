using System;
using System.IO;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.ObjectFactories;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.ManageCampaigns;
using AtCommon.Dtos.CampaignsV2;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Dashboards.Assessment.ManageCampaigns.create
{
    [TestClass]
    [TestCategory("AssessmentDashboard"), TestCategory("Dashboard")]
    public class CampaignVerbiageVerificationTests : BaseTest
    {
        private static readonly CampaignDetails CreateCampaignDetails = ManageCampaignsV2Factory.GetCampaignDetails();

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 48906
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        public void ManageCampaignsTab_CreateCampaign_VerbiageVerification()
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
            var manageCampaignCommonPage = new ManageCampaignsCommonPage(Driver, Log);
            var teamNamesList = ManageCampaignsV2Factory.TeamNamesList();
            var facilitatorFirstNamesList = ManageCampaignsV2Factory.FacilitatorFirstNamesList();

            var manageCampaignsVerbiage = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\ManageCampaignsV2\ManageCampaignsVerbiage.json").DeserializeJsonObject<CampaignVerbiage>();

            Log.Info("Login to the application and Navigate to the Assessment dashboard, Go to'Manage Campaigns' Tab and verify the 'Manage Campaigns' text ");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            teamDashBoardPage.ClickAssessmentDashBoard();
            assessmentDashboardListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.ManageCampaignsTab);
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.ManageCampaignsDashboard
                .ManageCampaignHeaderText, manageCampaignsTabPage.GetManageCampaignHeaderTitleText(), "'Manage Campaigns' text is not matched");

            Log.Info("Click on 'Create New Campaign' button and verify title and description");
            manageCampaignsTabPage.ClickOnCreateNewCampaignsButton();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignHeaderText, campaignDetailsPage.GetCreateCampaignHeaderTitle(), "'Create Campaign' text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.LetsCreateYourCampaignText, campaignDetailsPage.GetDescriptionText(), "Description text is not matched");

            Log.Info("Verify all the tooltip messages from each field");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[0], campaignDetailsPage.GetTooltipMessage("Campaign Name"), "'Campaign Name' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[1], campaignDetailsPage.GetTooltipMessage("Radar Type"), "'Radar type' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[2], campaignDetailsPage.GetTooltipMessage("Start Date"), "'Start Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[3], campaignDetailsPage.GetTooltipMessage("End Date"), "'End Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[4], campaignDetailsPage.GetTooltipMessage("Parent Team"), "'Parent Team' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignTooltipMessages[5], campaignDetailsPage.GetTooltipMessage("Target Number of Teams Per Facilitator"), "'Target Number of Teams Per Facilitator' tooltip is not matched");

            Log.Info("Verify all the validation message for each field displayed on 'Create Campaign' page");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignValidationMessages[0], campaignDetailsPage.GetCampaignNameValidation(), "'Campaign Name' validation message is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignValidationMessages[1], campaignDetailsPage.GetRadarTypeValidationMessage(), "'Radar Type' validation message is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignValidationMessages[2], campaignDetailsPage.GetStartDateValidationMessage(), "'Start Date' validation message is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignValidationMessages[3], campaignDetailsPage.GetEndDateValidationMessage(), "'End Date' validation message is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.CampaignDetail.CreateCampaignValidationMessages[4], campaignDetailsPage.GetTargetNoOfTeamsPerFacilitatorValidation(), "'Target Number of Teams Per Facilitator' validation message is not matched");

            Log.Info("Enter valid details, click on 'Continue to setup' button and verify Campaign's name on 'Select teams' page");
            campaignDetailsPage.EnterCampaignDetailsInfo(CreateCampaignDetails);
            campaignDetailsPage.ClickOnCreateACampaign();
            Assert.AreEqual(CreateCampaignDetails.Name, selectTeamsPage.GetSetUpCampaignHeaderTitle(), "Campaign name on header is not matched");

            Log.Info("Verify all verbiage and tooltip messages from 'Select Teams' page");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectTeams.SelectTeamsHeaderText, selectTeamsPage.GetSelectTeamsHeaderText(),"Select teams header text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectTeams.SelectTeamsDescriptionText,campaignDetailsPage.GetDescriptionText(),"Select team description is not matched");

            selectTeamsPage.ClickOnAddTeamsButton();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectTeams.SelectTeamPopupText, selectTeamsPage.GetSelectTeamDescription(), "Description of 'Select Team' popup is not matched");

            selectTeamsPage.HoverOverOnTeamContactIsAHFInfoIcon();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectTeams.TeamContactIsAhfTooltip,selectTeamsPage.GetTooltipMessageOfTeamContactIsAhf(),"Tooltip message is not displayed");

            Log.Info("Select checkbox by 'Team Name', click on 'Add To Campaign' button and verify team is added in campaign");
            selectTeamsPage.SelectTeamCheckBox(teamNamesList);
            selectTeamsPage.ClickOnAddToCampaignButton();
            foreach (var team in teamNamesList)
            {
                Assert.That.ListContains(selectTeamsPage.GetItemsListByColumnName("Team Name", false), team, "Team is not added to the campaign from 'Add Teams' model");
            }

            Log.Info("Click on 'Continue To Facilitator' button and verify all the verbiage and tooltips");
            selectTeamsPage.ClickOnContinueToFacilitatorButton();

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectFacilitators
                    .SelectFacilitatorsHeaderText, selectFacilitatorPage.GetSelectTeamsHeaderText(), "Select facilitators header text is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SelectFacilitators
                .SelectFacilitatorsDescriptionText, campaignDetailsPage.GetDescriptionText(), "Select facilitators header text is not matched");

            Log.Info("Click on 'Add Facilitators' button, Select facilitator and Click on 'Add  to campaign' button");
            selectFacilitatorPage.ClickOnAddFacilitatorsButton();
            selectFacilitatorPage.SelectFirstNameCheckBox(facilitatorFirstNamesList);
            selectFacilitatorPage.ClickOnAddToCampaignButton();

            Log.Info("Click on 'Continue To Matchmaking' button and verify all the verbiage and tooltip messages");
            selectFacilitatorPage.ClickOnContinueToMatchmakingButton();

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.AutoMatchmaking.CreateAutoMatchesHeaderTitleMessageText, autoMatchmakingPage.GetHeaderTitleMessageText(), "Title message text is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.AutoMatchmaking.TeamContactViewTooltip, autoMatchmakingPage.GetTeamContactViewTooltipMessage(), "'Team Contact View' tooltip message text is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.AutoMatchmaking.FacilitatorViewTooltip, autoMatchmakingPage.GetFacilitatorViewTooltipMessage(), "'Facilitator view' tooltip message text is not matched");

            autoMatchmakingPage.ClickOnCreateAutoMatchesButton();
            manageCampaignCommonPage.ClickOnBackButton();
            selectFacilitatorPage.ClickOnContinueToMatchmakingButton();

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.AutoMatchmaking.MatchmakingDescriptionAfterCreateAutoMatchesBackAndForth, autoMatchmakingPage.GetHeaderTitleMessageText(), "Description message after create auto matches and back and forth is not matched");

            Log.Info("Click on 'Continue to SetUp Assessments' button and verify 'Continue To Review' button is disabled");
            autoMatchmakingPage.ClickOnContinueToSetUpAssessmentsButton();
            Assert.IsTrue(!setUpAssessmentsPage.IsContinueToReviewButtonEnabled(), "'Continue To Review' button is enabled");

            Log.Info("Verify validation message for 'Assessment Name' textbox");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.AssessmentNameValidationMessage, setUpAssessmentsPage.GetAssessmentNameValidationMessage(), "'Assessment Name' validation message is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.AssessmentNameTooltip, setUpAssessmentsPage.GetAssessmentNameTooltipMessage(), "'Assessment Name' tooltip message is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingTooltip, setUpAssessmentsPage.GetSplitMeetingTooltipMessage(), "'Split meeting' tooltip message is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingTooltip, setUpAssessmentsPage.GetOneMeetingTooltipMessage(), "'One meeting' tooltip message is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.RetroMeetingTooltip, setUpAssessmentsPage.GetRetroOnlyTooltipMessage(), "'Retro only' tooltip message is not matched");

            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.AssessmentTimelineTooltip, setUpAssessmentsPage.GetAssessmentTimelineTooltipMessage(), "'Assessment Timeline' tooltip message is not matched");

            Log.Info("Click on 'Split Meeting' Verify all the tooltip messages from each field");
            setUpAssessmentsPage.ClickOnSplitMeetingRadioButton();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingDatesTooltip[0], campaignDetailsPage.GetSplitAssessmentTooltipMessage("Stakeholder Launch Date"), "'Stakeholder Launch Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingDatesTooltip[1], campaignDetailsPage.GetSplitAssessmentTooltipMessage("Team Member Launch Date"), "'Team Member Launch Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingDatesTooltip[2], campaignDetailsPage.GetSplitAssessmentTooltipMessage("Assessment Close Date"), "'Assessment Close Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingDatesTooltip[3], campaignDetailsPage.GetSplitAssessmentTooltipMessage("Retrospective Window Start"), "'Retrospective Window Start' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.SplitMeetingDatesTooltip[4], campaignDetailsPage.GetSplitAssessmentTooltipMessage("Retrospective Window End"), "'Retrospective Window End' tooltip text is not matched");

            Log.Info("Click on 'One Meeting' Verify all the tooltip messages from each field");
            setUpAssessmentsPage.ClickOnOneMeetingRadioButton();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingDatesTooltip[0], campaignDetailsPage.GetOneAssessmentTooltipMessage("Stakeholder Window Start"), "'Stakeholder Window Start' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingDatesTooltip[1], campaignDetailsPage.GetOneAssessmentTooltipMessage("Stakeholder Window End"), "'Stakeholder Window End' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingDatesTooltip[2], campaignDetailsPage.GetOneAssessmentTooltipMessage("Retrospective Window Start"), "'Retrospective Window Start' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingDatesTooltip[3], campaignDetailsPage.GetOneAssessmentTooltipMessage("Team Member Launch Date"), "'Team Member Launch Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.OneMeetingDatesTooltip[4], campaignDetailsPage.GetOneAssessmentTooltipMessage("Retrospective Window End"), "'Retrospective Window End' tooltip text is not matched");

            Log.Info("Click on 'Retro Meeting' Verify all the tooltip messages from each field");
            setUpAssessmentsPage.ClickOnRetroOnlyRadioButton();
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.RetroMeetingDatesTooltip[0], campaignDetailsPage.GetRetroAssessmentTooltipMessage("Assessment Start Date"), "'Assessment Start Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.RetroMeetingDatesTooltip[1], campaignDetailsPage.GetRetroAssessmentTooltipMessage("Assessment Close Date"), "'Assessment Close Date' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.RetroMeetingDatesTooltip[2], campaignDetailsPage.GetRetroAssessmentTooltipMessage("Retrospective Window Start"), "'Retrospective Window Start' tooltip text is not matched");
            Assert.AreEqual(manageCampaignsVerbiage.ManageCampaignsVerbiage.ManageCampaigns.SetUpAssessment.RetroMeetingDatesTooltip[3], campaignDetailsPage.GetRetroAssessmentTooltipMessage("Retrospective Window End"), "'Retrospective Window End' tooltip text is not matched");
        }
    }
}

