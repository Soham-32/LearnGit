using System;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.Tests.AgilityHealth.Company;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.LanguageTranslations
{
    [TestClass]
    public class LanguageTranslationsBaseTest : CompanyEditBaseTest
    {
        public void CreateAssessmentAndVerifyEmail(string language, string companyName, string teamName, AddMemberRequest teamMember, AddStakeholderRequest stakeholder = null)
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            Log.Info("Create a Team Assessment and publish it");
            teamAssessmentDashboard.AddAnAssessment("Team");
            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamHealthRadarName;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(teamMember.FirstName + " " + teamMember.LastName);
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            if (stakeholder != null)
            {   
                selectStakeHolder.SelectStakeHolderByName(stakeholder.FirstName + " " + stakeholder.LastName);
            }

            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            var emailInformation = EmailFactory.GetSurveyEmailBodyByLanguage(language);

            Log.Info($"Verify all the details of the Assessment survey which is received by 'Team Member' with {language} language");
            //Get Survey Email Subject from Email Information 
            var emailSubject = emailInformation.SurveyEmailMessageSubject.Replace("{TeamName}", teamName).Replace("{AssessmentName}", assessmentName);

            //Get Actual Team teamMember Survey Email Data from Gmail
            var teamMemberEmailBody = GmailUtil.GetAccountManagerEmailBody(emailSubject, teamMember.Email);
            var actualTeamMemberEmailBody = teamMemberEmailBody.HtmlDecode();

            //Get 'Time' from Actual Team Member Email body
            var actualTeamMemberEmailTiming = actualTeamMemberEmailBody.GetTimeFromString();

            //Get Expected Team teamMember Survey Email Data from Json
            var translatedSurveyEmailBody = emailInformation.SurveyEmailBody.Replace("{AssessmentName}", assessmentName)
                    .Replace("{TeamName}", teamName).Replace("{CompanyName}", companyName);
            var expectedTeamMemberEmailBody = TranslationsFactory.GetExpectedTeamMemberEmailBody(translatedSurveyEmailBody, language);

            //Get 'Time' from Expected Team Member Email
            var expectedTeamMemberEmailTiming = expectedTeamMemberEmailBody.GetTimeFromString();

            //Verify actual and expected time from Email Body
            Assert.That.TimeIsClose(expectedTeamMemberEmailTiming, actualTeamMemberEmailTiming, 3);

            //Remove 'Time' from Expected Team Member Email
            expectedTeamMemberEmailBody = expectedTeamMemberEmailBody.RemoveTimeFromString().RemoveWhitespace();
            //Remove 'Time' from Actual Team Member Email
            actualTeamMemberEmailBody = actualTeamMemberEmailBody.RemoveTimeFromString().RemoveWhitespace();

            //Verify actual and expected Team teamMember Survey Email Body
            Assert.AreEqual(expectedTeamMemberEmailBody, actualTeamMemberEmailBody, "Team Member Email body is not matched");

            if (stakeholder == null) return;
            Log.Info($"Verify all the details of assessment survey which is received by 'StakeHolder' with {language} language");

            //Get Actual Stakeholder Survey Email Data from Gmail
            var stakeHolderEmailBody = GmailUtil.GetAccountManagerEmailBody(emailSubject, stakeholder.Email, GmailUtil.MemberEmailLabel);
            var actualStakeHolderEmailBody = stakeHolderEmailBody.HtmlDecode();

            //Get 'Time' from Actual Stakeholder Email body
            var actualStakeHolderEmailTiming = actualStakeHolderEmailBody.GetTimeFromString();

            //Get Expected Stakeholder Survey Email Data from Json
            var translatedStakeHolderSurveyEmailBody = emailInformation.SurveyEmailBody.Replace("{AssessmentName}", assessmentName)
                .Replace("{TeamName}", teamName).Replace("{CompanyName}", companyName);
            var expectedStakeHolderEmailBody = TranslationsFactory.GetExpectedStakeholderEmailBody(translatedStakeHolderSurveyEmailBody, language, teamName, $"{teamMember.FirstName} {teamMember.LastName}");

            //Get 'Time' from Expected Stakeholder Email
            var expectedStakeHolderEmailTiming = expectedStakeHolderEmailBody.GetTimeFromString();

            //Verify actual and expected time from Email Body
            Assert.That.TimeIsClose(expectedStakeHolderEmailTiming, actualStakeHolderEmailTiming, 3);

            //Remove 'Time' from Expected Stakeholder Email
            expectedStakeHolderEmailBody = expectedStakeHolderEmailBody.RemoveTimeFromString().RemoveWhitespace();
            //Remove 'Time' from Actual Stakeholder Email
            actualStakeHolderEmailBody = actualStakeHolderEmailBody.RemoveTimeFromString().RemoveWhitespace();

            //Verify actual and expected Stakeholder Survey Email Body
            Assert.AreEqual(expectedStakeHolderEmailBody, actualStakeHolderEmailBody, "Stakeholder Email body is not matched");
        }

        public void CreateAssessmentAndVerifySetupEmail(string language, string companyName, string teamName, AddMemberRequest teamMember )
        {
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var selectTeamMembers = new SelectTeamMembersPage(Driver, Log);
            var assessmentProfile = new AssessmentProfilePage(Driver, Log);
            var selectStakeHolder = new SelectStakeHolderPage(Driver, Log);
            var reviewAndLaunch = new ReviewAndLaunchPage(Driver, Log);

            Log.Info("Create a Team Assessment and publish it");
            teamAssessmentDashboard.AddAnAssessment("Team");
            var assessmentName = RandomDataUtil.GetAssessmentName();
            const string surveyType = SharedConstants.TeamHealthRadarName;
            assessmentProfile.FillDataForAssessmentProfile(surveyType, assessmentName);
            assessmentProfile.ClickOnNextSelectTeamMemberButton();
            selectTeamMembers.SelectTeamMemberByName(teamMember.FirstName + " " + teamMember.LastName);
            selectTeamMembers.ClickOnNextSelectStakeholdersButton();
            selectStakeHolder.ClickOnReviewAndFinishButton();
            reviewAndLaunch.ClickOnPublish();

            var surveyEmailInformation = EmailFactory.GetSurveyEmailBodyByLanguage(language);
            var surveyEmailSubject = surveyEmailInformation.SurveyEmailMessageSubject.Replace("{TeamName}", teamName).Replace("{AssessmentName}", assessmentName);

            Log.Info($"Get the survey link of {teamMember.FirstName } from gmail and fill the survey");
            var surveyPage = new SurveyPage(Driver, Log);
            surveyPage.NavigateToUrl(GmailUtil.GetSurveyLink(
                surveyEmailSubject, teamMember.Email, "inbox"));
            surveyPage.ConfirmIdentity();
            surveyPage.ClickStartSurveyButton();
            surveyPage.SubmitSurvey(7);
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickNextButton();
            surveyPage.ClickFinishButton(false);
            Driver.Quit();

            var setupEmailInformation = EmailFactory.GetSetupEmailBodyByLanguage(language);
            Log.Info($"Verify all the details of the Assessment survey which is received by 'Team Member' with {language} language");
            //Get Setup Email Subject from Email Information 
            var setupEmailSubject = setupEmailInformation.SetupEmailMessageSubject;

            //Get Actual Team teamMember Setup Email Data from Gmail
            var teamMemberEmailBody = GmailUtil.GetAccountManagerEmailBody(setupEmailSubject, teamMember.Email);
            var actualTeamMemberEmailBody = teamMemberEmailBody.HtmlDecode().RemoveWhitespace();

            //Get Expected Team teamMember Setup Email Data from Json
            var expectedTeamMemberEmailBody = setupEmailInformation.SetupEmailBody.Replace("{AssessmentName}", assessmentName)
                    .Replace("{TeamName}", teamName).Replace("{CompanyName}", companyName).RemoveWhitespace();

            if (User.IsSiteAdmin())
            {
                List<string> expectedEmailBodyList;
                switch (language)
                {
                    case "Japanese":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('。').ToList();
                        expectedEmailBodyList[4] = "サポートセンターをチェックして、AgilityHealthの旅をすぐに始めましょう!AgilityHealthチーム";
                        expectedTeamMemberEmailBody = string.Join("。", expectedEmailBodyList);
                        break;

                    case "Chinese":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('。').ToList();
                        expectedEmailBodyList[4] = "查看我们的支持中心，快速开始您的AgilityHealth之旅！AgilityHealth团队";
                        expectedTeamMemberEmailBody = string.Join("。", expectedEmailBodyList);
                        break;

                    case "French":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "Jetezuncoupd´œilànotreCentredeSupportpourcommencervotreparcoursavecAgilityHealth!ÉquipeAgilityHealth";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "Korean":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "AgilityHealth여정을빠르게시작하려면지원센터를확인하세요!AgilityHealth팀";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "Portuguese":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "ConfiranossoCentrodesuporteparacomeçarsuajornadadeAgilityHealth!EquipeAgilityHealth";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "Turkish":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "AgilityHealthYolculuğunuzdahızlıbirbaşlangıçyapmakiçinDestekMerkezimizegözatın!ÇeviklikSağlıkEkibi";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "Spanish":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "ConsultenuestroCentrodeasistenciaparacomenzartucaminohaciaAgilityHealth.EquipoAgilityHealth";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "English":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "CheckoutourSupportCentertogetajumpstartonyourAgilityHealthJourney!AgilityHealthTeam";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;

                    case "Hungarian":
                        expectedEmailBodyList = expectedTeamMemberEmailBody.Split('.').ToList();
                        expectedEmailBodyList[4] = "TekintsemegTámogatásiKözpontunkat,hogylendületetkapjonazAgilityHealthútján!AgilityHealthTeam";
                        expectedTeamMemberEmailBody = string.Join(".", expectedEmailBodyList);
                        break;
                }
            }

            //Verify actual and expected Team teamMember Setup Email Body
            Assert.AreEqual(expectedTeamMemberEmailBody, actualTeamMemberEmailBody, "Team Member Email body is not matched");
        }
    }
}
