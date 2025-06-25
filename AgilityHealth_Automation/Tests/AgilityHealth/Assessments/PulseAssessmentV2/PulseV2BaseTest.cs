using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.PulseAssessmentV2
{
    [TestClass]
    public class PulseV2BaseTest : BaseTest
    {
        protected static int GetTeamId(string teamName)
        {
            return new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(teamName).TeamId;
        }

        protected static CreatePulseAssessmentResponse AddPulseAssessment(SavePulseAssessmentV2Request request)
        {
            return new SetupTeardownApi(TestEnvironment).CreatePulseAssessmentV2(request, Company.Id);
        }


        protected static RadarQuestionDetailsV2Response GetQuestions(int teamId)
        {
            var setup = new SetupTeardownApi(TestEnvironment);
            var surveyId = setup.GetRadar(Company.Id, SharedConstants.TeamAssessmentType).Id;

            return setup.GetRadarQuestionDetailsV2(Company.Id, teamId, surveyId);
        }

        protected static void CompletePulseSurvey(TestContext testContext,
            RadarQuestionDetailsV2Response questions, string teamName, string email)
        {
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.PulseEmailSubject(teamName),
                To = email,
                Labels = new List<string> { "inbox" }
            };
            new SetUpMethods(testContext, TestEnvironment).CompletePulseSurvey(emailSearch, questions, 5);
        }
    }
}