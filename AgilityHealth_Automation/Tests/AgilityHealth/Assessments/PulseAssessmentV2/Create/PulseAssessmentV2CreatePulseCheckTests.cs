using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AtCommon.Api;
using AtCommon.Utilities;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Dtos.Companies;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Create
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2CreatePulseCheckTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static RadarQuestionDetailsV2Response _radarDetailResponse;
        private static int _teamId;
        private static IList<TeamV2Response> _teamWithTeamMemberResponses;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team",1);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                //Get team profile 
                _teamId= setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;
                _teamWithTeamMemberResponses = setupApi.GetTeamWithTeamMemberResponse(_teamId);

                //Get radar details
                _radarDetailResponse = GetQuestions(_teamId);

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_PulseCheck_SaveAsDraft_Weekly_EndsNever()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, fill the pulse check details under 'Create PulseCheck' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();
            pulseData.StartDate = DateTime.UtcNow.ToLocalTime();

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);

            // verify defaults
            var actualData = createPulseCheckPage.GetScheduleSectionData();

            Assert.That.TimeIsClose(DateTime.UtcNow.ToLocalTime(), actualData.StartDate);
            Assert.AreEqual("72 Hours", actualData.Period, "Assessment Period doesn't match");
            Assert.AreEqual(RepeatIntervalId.Never.GetDescription(), actualData.RepeatInterval.Type, "Assessment RepeatInterval doesn't match");

            var actualAssessmentPeriodList = createPulseCheckPage.GetAllAssessmentPeriodListItems();
            var expectedAssessmentPeriodList = new List<AssessmentPeriod>((AssessmentPeriod[])System.Enum.GetValues(typeof(AssessmentPeriod))).Select(a => a.GetDescription()).ToList();

            // Remove the last item
            if (expectedAssessmentPeriodList.Count > 0)
            {
                expectedAssessmentPeriodList.RemoveAt(expectedAssessmentPeriodList.Count - 1);
            }

            Assert.That.ListsAreEqual(expectedAssessmentPeriodList, (IList)actualAssessmentPeriodList, "Assessment Period list doesn't match.");

            var actualRepeatIntervalList = createPulseCheckPage.GetAllRepeatIntervalListItems().ToList();
            var expectedRepeatIntervalList = new List<string>
            {
                RepeatIntervalId.Never.GetDescription(),
                $"{RepeatIntervalId.Weekly.GetDescription()}{DateTime.Today:dddd}",
                $"{RepeatIntervalId.BiMonthly.GetDescription()}{DateTime.Today:dddd}",
                $"{RepeatIntervalId.Monthly.GetDescription()}{DateTime.Today:dddd}",
                $"{RepeatIntervalId.Quarterly.GetDescription()}{DateTime.Today:dddd}",
            };

            Assert.That.ListsAreEqual(expectedRepeatIntervalList, actualRepeatIntervalList, "Repeat Interval list doesn't match");

            // verify 'Ends' section show after changing repeat interval
            Assert.IsFalse(createPulseCheckPage.IsEndsSectionDisplayed(), "Never radio button should not be displayed");

            createPulseCheckPage.SelectRepeatInterval($"{RepeatIntervalId.Weekly.GetDescription()}{DateTime.Today:dddd}");
            Assert.IsTrue(createPulseCheckPage.IsEndsSectionDisplayed(), "Never radio button should be displayed");

            var request = new SavePulseAssessmentV2Request
            {
                StartDate = DateTime.Today.AddDays(3),
                RepeatIntervalId = (int)RepeatIntervalId.Weekly,
                RepeatEndStrategyId = (int)End.AfterOccurrences,
                RepeatOccurrenceNumber = 5,
                PeriodId = (int)AssessmentPeriod.TwentyFourHours
            };
            createPulseCheckPage.FillSchedulerInfo(request);
            createPulseCheckPage.Header.ClickOnNextButton();

            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
            selectRecipientsPage.Header.ClickSaveAsDraftButton();

            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseData.Name), "PulseAssessment does not exist");

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            var assessmentName = editPulseCheckPage.GetAssessmentName();
            var assessmentType = editPulseCheckPage.GetAssessmentType();
            var actualPeriod = editPulseCheckPage.GetPeriod();

            Assert.AreEqual(pulseData.Name, assessmentName, "AssessmentName does not match");
            Assert.AreEqual(pulseData.AssessmentType, assessmentType, "AssessmentType does not match");
            Assert.AreEqual(pulseData.Period, actualPeriod, "AssessmentPeriod does not match");
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_PulseCheck_SaveAsDraft_BiMonthly_EndsNever()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.BiMonthly, End.Never, DateTime.Now);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_PulseCheck_SaveAsDraft_Monthly_EndsAfterOccurrences()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Monthly, End.AfterOccurrences, DateTime.Now, 5);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_PulseCheck_SaveAsDraft_Quarterly_EndsOnDate()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Quarterly, End.OnDate, DateTime.Now.AddDays(15));
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Create_PulseCheck_NameAlreadyUsed()
        {
            VerifySetup(_classInitFailed);

            var pulseRequest = PulseV2Factory.PulseAssessmentV2AddRequest(_radarDetailResponse, _teamWithTeamMemberResponses, _teamId);
            AddPulseAssessment(pulseRequest);

            var login = new LoginPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, Verify 'CreatePulseCheck' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            createPulseCheckPage.AddAssessmentName(pulseRequest.Name);
            Assert.AreEqual("This name has already been used for a Pulse Check. Please choose a different name.",
                createPulseCheckPage.GetAssessmentNameErrorMessage(), "Assessment Name error message does not match.");
            Assert.IsFalse(createPulseCheckPage.Header.IsNextButtonEnabled(), "Next button is enabled");
        }

        public void Pulse_RepeatInterval_Validator(RepeatIntervalId repeatInterval, End ends, DateTime endDate, int numberOfOccurrences = 0)
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, fill the pulse check details under 'Create PulseCheck' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();
            pulseData.RepeatInterval.Type = $"{repeatInterval.GetDescription()}{pulseData.StartDate:dddd}";
            pulseData.RepeatInterval.Ends = ends;
            pulseData.EndDate = endDate;
            pulseData.NumberOfOccurrences = numberOfOccurrences;
            pulseData.IsDraft = true;
            pulseData.Period = repeatInterval switch
            {
                RepeatIntervalId.BiMonthly => "3 Weeks",
                RepeatIntervalId.Monthly => "72 Hours",
                RepeatIntervalId.Weekly => "2 Weeks",
                RepeatIntervalId.Quarterly => "Until the next assessment launch date",
                _ => pulseData.Period
            };

            createPulseCheckPage.AddAssessmentName(pulseData.Name);
            createPulseCheckPage.SelectAssessmentType(pulseData.AssessmentType);
            createPulseCheckPage.FillSchedulerInfo(pulseData);
            createPulseCheckPage.Header.ClickOnNextButton();
            
            selectQuestionPage.FillForm(pulseData);
            selectQuestionPage.Header.ClickOnNextButton();

            selectRecipientsPage.SelectDeselectAllTeamsCheckBox();
             selectRecipientsPage.Header.ClickSaveAsDraftButton();
            
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            Assert.IsTrue(teamAssessmentDashboardPage.IsPulseAssessmentDisplayed(pulseData.Name), "PulseAssessment does not exist");

            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            var assessmentName = editPulseCheckPage.GetAssessmentName();
            var assessmentType = editPulseCheckPage.GetAssessmentType();
            var actualPeriod = editPulseCheckPage.GetPeriod();

            Assert.AreEqual(pulseData.Name, assessmentName, "AssessmentName does not match");
            Assert.AreEqual(pulseData.AssessmentType, assessmentType, "AssessmentType does not match");
            Assert.AreEqual(pulseData.Period, actualPeriod, "AssessmentPeriod does not match");
        }
    }
}