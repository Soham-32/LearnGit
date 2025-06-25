using System;
using AtCommon.Api;
using AtCommon.Api.Enums;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Dtos.Assessments.PulseV2.Custom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.PulseAssessmentV2.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AtCommon.Dtos.Companies;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2.Edit
{
    [TestClass]
    [TestCategory("Critical")]
    [TestCategory("PulseAssessmentsV2"), TestCategory("Assessments")]
    public class PulseAssessmentV2EditPulseCheckTests : PulseV2BaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _teamResponse;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setupApi = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("PulseV2Team", 2);
                _teamResponse = setupApi.CreateTeam(team).GetAwaiter().GetResult();

                //Get team profile 
                _teamId = setupApi.GetCompanyHierarchy(Company.Id).GetTeamByName(_teamResponse.Name).TeamId;

            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_PulseCheck_Never_EndsNever_Publish()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Never, End.Never, DateTime.Now);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_PulseCheck_Weekly_EndsNever_Publish()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Weekly, End.Never, DateTime.Now);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 51186
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_PulseCheck_Monthly_EndsAfterOccurrences_Publish()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Monthly, End.AfterOccurrences, DateTime.Now, 5);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefect")] //Bug Id: 51186
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_PulseCheck_BiMonthly_EndsNever_Publish()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.BiMonthly, End.Never, DateTime.Now);
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PulseV2_Edit_PulseCheck_Quarterly_EndsOnDate_Publish()
        {
            VerifySetup(_classInitFailed);
            Pulse_RepeatInterval_Validator(RepeatIntervalId.Quarterly, End.OnDate, DateTime.Now.AddDays(15));
        }

        public void Pulse_RepeatInterval_Validator(RepeatIntervalId repeatInterval, End ends, DateTime endDate, int numberOfOccurrences = 0)
        {
            var login = new LoginPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var createPulseCheckPage = new CreatePulseCheckPage(Driver, Log);
            var editPulseCheckPage = new EditPulseCheckPage(Driver, Log);
            var selectQuestionPage = new SelectQuestionsPage(Driver, Log);
            var selectRecipientsPage = new SelectRecipientsPage(Driver, Log);
            var reviewAndPublishPage = new ReviewAndPublishPage(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            Log.Info("Navigate to Team assessment dashboard and create Pulse, fill the pulse check details under 'Create PulseCheck' page");
            teamAssessmentDashboardPage.NavigateToPage(_teamId);
            teamAssessmentDashboardPage.AddAnAssessment("Pulse");

            var pulseData = PulseV2Factory.GetPulseAddData();

            pulseData.RepeatInterval.Type = repeatInterval == RepeatIntervalId.Never ? $"{repeatInterval.GetDescription()}" : $"{repeatInterval.GetDescription()}{pulseData.StartDate:dddd}";

            pulseData.RepeatInterval.Ends = ends;
            pulseData.EndDate = endDate;
            pulseData.NumberOfOccurrences = numberOfOccurrences;
            pulseData.IsDraft = true;
            pulseData.Period = repeatInterval switch
            {
                RepeatIntervalId.Never => "24 Hours",
                RepeatIntervalId.BiMonthly => "3 Weeks",
                RepeatIntervalId.Monthly => "72 Hours",
                RepeatIntervalId.Weekly => "1 Week",
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
            selectRecipientsPage.Header.ClickOnNextButton();

            reviewAndPublishPage.ClickOnPublishButton();
            reviewAndPublishPage.Header.ClickOnPublishPopupPublishButton();

            Log.Info("Edit Pulse assessment and verify the details");
            teamAssessmentDashboardPage.SwitchToPulseAssessment();
            teamAssessmentDashboardPage.ClickOnPulseEditLink(pulseData.Name);

            var assessmentName = editPulseCheckPage.GetAssessmentName();
            var assessmentType = editPulseCheckPage.GetAssessmentType();
            var actualEditedData = editPulseCheckPage.GetScheduleSectionData();

            Assert.AreEqual(pulseData.Name, assessmentName, "AssessmentName doesn't match");
            Assert.AreEqual(pulseData.AssessmentType, assessmentType, "AssessmentType doesn't match");
            Assert.AreEqual(pulseData.StartDate.ToString("d"), actualEditedData.StartDate.ToString("d"), "Assessment StartDateTime doesn't match");
            Assert.AreEqual(pulseData.Period, actualEditedData.Period, "Assessment Period doesn't match");
            Assert.AreEqual(pulseData.RepeatInterval.Type, actualEditedData.RepeatInterval.Type, "Assessment RepeatInterval doesn't match");
            Assert.AreEqual(pulseData.RepeatInterval.Ends, actualEditedData.RepeatInterval.Ends, "Assessment RepeatInterval End doesn't match");

            switch (pulseData.RepeatInterval.Ends)
            {
                case End.OnDate:
                    Assert.AreEqual(pulseData.EndDate.ToString("d"), actualEditedData.EndDate.ToString("d"), "Assessment RepeatInterval End Date doesn't match");
                    break;
                case End.AfterOccurrences:
                    Assert.AreEqual(pulseData.NumberOfOccurrences, actualEditedData.NumberOfOccurrences, "Assessment RepeatInterval Number Of Occurrences doesn't match");
                    break;
                case End.Never:
                case End.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}