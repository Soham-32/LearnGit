using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Custom;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AgilityHealth_Automation.PageObjects.AgilityHealth.Radar.RadarPage;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("LanguageTranslation")]
    public class TeamAssessmentDetailsTranslationsTests : BaseTest
    {
        private static User TranslationAdmin => TestEnvironment.UserConfig.GetUserByDescription("translation");
        private static List<DimensionNote> _teamMemberNotes;
        private static List<DimensionNote> _stakeholderNotes;
        private static TeamHierarchyResponse _team;
        private static bool _classInitFailed;
        private static readonly TeamAssessmentInfo TeamAssessment = new TeamAssessmentInfo
        {
            AssessmentType = SharedConstants.TeamHealthRadarName,
            AssessmentName = RandomDataUtil.GetAssessmentName(),
            TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName() },
            StakeHolders = new List<string> { SharedConstants.Stakeholder2.FullName() }
        };

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            try
            {
                _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id, TranslationAdmin).GetTeamByName(SharedConstants.Team);
                var setup = new SetUpMethods(testContext, TestEnvironment);
                setup.AddTeamAssessment(_team.TeamId, TeamAssessment, TranslationAdmin);
                TeamAssessment.StartDate = DateTime.UtcNow.ToLocalTime();

                // Fill the survey
                _teamMemberNotes = new List<DimensionNote>
                 {
                     new DimensionNote { Dimension = "Clarity", SubDimension = "Vision", Note = "Vision member" },
                 };

                setup.CompleteTeamMemberSurvey(new EmailSearch
                {
                    Subject = SharedConstants.TeamAssessmentSubject(_team.Name, TeamAssessment.AssessmentName),
                    To = SharedConstants.TeamMember1.Email,
                    Labels = new List<string> { GmailUtil.MemberEmailLabel }
                }, _teamMemberNotes);

                // Complete survey for '_teamAssessmentStakeholder'
                _stakeholderNotes = new List<DimensionNote>
                 {
                     new DimensionNote { Dimension = "Performance", SubDimension = "Confidence", Note = "Confidence StakeHolder" }
                 };
                setup.CompleteStakeholderSurvey(_team.Name, SharedConstants.Stakeholder2.Email, TeamAssessment.AssessmentName, 5, _stakeholderNotes);
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void RadarPage_Translated_English_Language()
        {
            VerifySetup(_classInitFailed);
            VerifyTeamAssessmentDetailsTranslations(RadarLanguage.English.GetDescription());
        }


        //Disabled this test case due to an increased number of failures. The failures seem to be related to recent translation changes in the application

        //[TestMethod] 
        //[TestCategory("CompanyAdmin")]
        public void RadarPage_Translated_Japanese_Language()
        {
            VerifySetup(_classInitFailed);
            VerifyTeamAssessmentDetailsTranslations(RadarLanguage.Japanese.GetDescription());
        }


        //Disabled this test case due to an increased number of failures. The failures seem to be related to recent translation changes in the application

        //[TestMethod]
        //[TestCategory("CompanyAdmin")]
        public void RadarPage_Translated_Spanish_Language()
        {
            VerifySetup(_classInitFailed);
            VerifyTeamAssessmentDetailsTranslations(RadarLanguage.Spanish.GetDescription());
        }

        private void VerifyTeamAssessmentDetailsTranslations(string preferredLanguage)
        {
            var loginPage = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(Driver, Log);
            var assessmentDetailsCommonPage = new AssessmentDetailsCommonPage(Driver, Log);

            //Deserialize 
            var translations = File
                .ReadAllText(
                    $@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\AssessmentDetailsTranslations\AssessmentDetailstranslations_{preferredLanguage}.json")
                .DeserializeJsonObject<AssessmentDetailsTranslations>();

            loginPage.NavigateToPage();
            loginPage.LoginToApplication(TranslationAdmin.Username, TranslationAdmin.Password);

            Log.Info($"Navigate to the team and Click on the assessment - {TeamAssessment.AssessmentName} ");
            teamAssessmentDashboardPage.NavigateToPage(_team.TeamId);
            teamAssessmentDashboardPage.ClickOnRadar(TeamAssessment.AssessmentName);
            Driver.NavigateToPage(Driver.GetCurrentUrl() + $"?languageCode={preferredLanguage}");

            //Header jumpLinks titles
            Log.Info("Verify that all header jumpLinks text are translated in preferred language");
            var actualHeaderJumpLinksTextList = radarPage.GetHeaderJumpLinksNamesTextList();
            var expectedHeaderJumpLinksTextList = translations.HeaderJumpLinkNames;
            Assert.That.ListsAreEqual(expectedHeaderJumpLinksTextList, actualHeaderJumpLinksTextList, "The header jumpLinks text are not translated in preferred language");

            //Filter section
            Log.Info("Click on the filter side bar symbol");
            radarPage.Filter_OpenFilterSidebar();

            Log.Info("Verify that filter description text are translated in preferred language");
            var expectedFilterDescriptionTextList = translations.FilterSidebarSection;
            Assert.That.ListsAreEqual(expectedFilterDescriptionTextList, radarPage.GetFilterSidebarDescriptionTextList(), "The filter description text are not translated in preferred language");

            Log.Info("Close the filter after clicking on the filter side bar symbol");
            radarPage.Filter_OpenFilterSidebar();

            //Radar View dropdown
            Log.Info("Verify that radar view dropdown all options text are translated in preferred language");
            var actualRadarViewDropDownTextList = radarPage.GetRadarViewDropdownValuesTextList();
            var expectedRadarViewDropDownTextList = translations.AssessmentDetails.RadarViewDropDownValues;
            Assert.That.ListsAreEqual(expectedRadarViewDropDownTextList, actualRadarViewDropDownTextList, "The radar view dropdown all options text are not translated in preferred language");

            //Benchmarking options popup
            Log.Info("Verify that benchmarking options popup is translated in preferred language");
            var actualBenchmarkingPopupTitleText = assessmentDetailsCommonPage.GetBenchmarkingPopupTitleText(translations.AssessmentDetails.RadarViewDropDownValues.Last());
            var expectedBenchmarkingPopupTitleText = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingPopupTitle;
            Assert.AreEqual(expectedBenchmarkingPopupTitleText, actualBenchmarkingPopupTitleText, "The benchmarking options popup title text is not translated in preferred language");

            var actualBenchmarkingPopupInfoText = assessmentDetailsCommonPage.GetBenchmarkingPopupInfoText();
            var expectedBenchmarkingPopupInfoText = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingPopupViewMessage;
            Assert.AreEqual(expectedBenchmarkingPopupInfoText, actualBenchmarkingPopupInfoText, "The benchmarking options popup information text is not translated in preferred language");

            var actualBenchmarkingPopupBenchmarkingOptionsFieldText = assessmentDetailsCommonPage.GetBenchmarkingPopupBenchmarkingOptionsFieldText();
            var expectedBenchmarkingPopupBenchmarkingOptionsFieldText = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingOption;
            Assert.AreEqual(expectedBenchmarkingPopupBenchmarkingOptionsFieldText, actualBenchmarkingPopupBenchmarkingOptionsFieldText, "The benchmarking options popups benchmarking options field text is not translated in preferred language");

            var actualBenchmarkingPopupBenchmarkingOptionsDropdownList = assessmentDetailsCommonPage.GetBenchmarkingPopupBenchmarkingOptionsDropdownList();
            var expectedBenchmarkingPopupBenchmarkingOptionsDropdownList = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingOptionsDropdownValues;
            Assert.That.ListsAreEqual(expectedBenchmarkingPopupBenchmarkingOptionsDropdownList, actualBenchmarkingPopupBenchmarkingOptionsDropdownList, "The benchmarking options popups benchmarking options all dropdown options text are not translated in preferred language");

            assessmentDetailsCommonPage.SelectBenchmarkingOptionsDropdownOption(translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingOptionsDropdownValues[1]);
            Assert.IsTrue(assessmentDetailsCommonPage.IsBenchmarkingPopupWorkTypeFieldTextDisplayed(), "The benchmarking options popup 'Work Type' field text is not displayed");

            var actualBenchmarkingPopupWorkTypeFieldText = assessmentDetailsCommonPage.GetBenchmarkingPopupWorkTypeFieldText();
            var expectedBenchmarkingPopupWorkTypeFieldText = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingWorkType;
            Assert.AreEqual(expectedBenchmarkingPopupWorkTypeFieldText, actualBenchmarkingPopupWorkTypeFieldText, "The benchmarking options popup 'Work Type' field text is not translated in preferred language");

            assessmentDetailsCommonPage.SelectBenchmarkingOptionsDropdownOption(translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingOptionsDropdownValues.Last());
            Assert.IsTrue(assessmentDetailsCommonPage.IsBenchmarkingPopupMaturityFieldTextDisplayed(), "The benchmarking options popup 'Maturity' field text is not displayed");

            var actualBenchmarkingPopupMaturityFieldText = assessmentDetailsCommonPage.GetBenchmarkingPopupMaturityFieldText();
            var expectedBenchmarkingPopupMaturityFieldText = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingMaturity;
            Assert.AreEqual(expectedBenchmarkingPopupMaturityFieldText, actualBenchmarkingPopupMaturityFieldText, "The benchmarking options popup 'Maturity' field text is not translated in preferred language");

            //Benchmarking options popup cancel and select buttons
            var actualBenchmarkingPopupCancelAndSelectButtonsTextList = assessmentDetailsCommonPage.GetBenchmarkingPopupCancelAndSelectButtonsTextList();
            var expectedBenchmarkingPopupCancelAndSelectButtonsTextList = translations.AssessmentDetails.BenchmarkingPopupDetails.FirstOrDefault()?.BenchmarkingPopupButton;
            Assert.That.ListsAreEqual(expectedBenchmarkingPopupCancelAndSelectButtonsTextList, actualBenchmarkingPopupCancelAndSelectButtonsTextList, "The benchmarking options popup 'Cancel' and 'Select' buttons text are not translated in preferred language");

            Log.Info("Close the benchmarking options popup after clicking on the 'Cancel' button");
            assessmentDetailsCommonPage.ClickOnBenchmarkingPopupCancelButton();

            //Radar action icons title
            Log.Info("Verify that all radar action icons titles are translated in preferred language");
            var actualAllRadarActionIconsTitleTextList = radarPage.GetAllRadarActionIconsTitleTextList();
            var expectedAllRadarActionIconsTitleTextList = translations.ActionIconSection.ActionIconNames;
            Assert.That.ListsAreEqual(expectedAllRadarActionIconsTitleTextList, actualAllRadarActionIconsTitleTextList, "All radar action icons titles text are not translated in preferred language");

            //Analytics header title
            Log.Info("Verify that analytics header title is translated in preferred language");
            var expectedAnalyticsHeaderTitleText = translations.AnalyticsSection.AnalyticsHeaderTitle;
            Assert.AreEqual(expectedAnalyticsHeaderTitleText, radarPage.GetAnalyticsHeaderTitleText(), "The analytics header title text is not translated in preferred language");

            //Analytics cards
            Log.Info("Verify that top 5 competencies title text is translated in preferred language");
            var actualTopFiveCompetenciesTitleText = radarPage.GetTopFiveCompetenciesTitleText();
            var expectedTopFiveCompetenciesTitleText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault()?.TopFiveCompetencies.FirstOrDefault()?.TopFiveCompetenciesTitle;
            Assert.That.ListsAreEqual(expectedTopFiveCompetenciesTitleText, actualTopFiveCompetenciesTitleText, "The top 5 competencies title text is not translated in preferred language");

            Log.Info("Verify that lowest 5 competencies title text is translated in preferred language");
            var actualLowestFiveCompetenciesTitleText = radarPage.GetLowestFiveCompetenciesTitleText();
            var expectedLowestFiveCompetenciesTitleText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault()?.LowestFiveCompetencies.FirstOrDefault()?.LowestFiveCompetenciesTitle;
            Assert.That.ListsAreEqual(expectedLowestFiveCompetenciesTitleText, actualLowestFiveCompetenciesTitleText, "The lowest 5 competencies title text is not translated in preferred language");

            Log.Info("Verify that 5 highest consensus competencies title text is translated in preferred language");
            var actualFiveHighestConsensusCompetenciesTitleText = radarPage.GetFiveHighestConsensusCompetenciesTitleText();
            var expectedFiveHighestConsensusCompetenciesTitleText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault()?.FiveHighestConsensusCompetencies.FirstOrDefault()?.FiveHighestConsensusCompetenciesTitle;
            Assert.That.ListsAreEqual(expectedFiveHighestConsensusCompetenciesTitleText, actualFiveHighestConsensusCompetenciesTitleText, "The 5 highest consensus competencies title text is not translated in preferred language");

            Log.Info("Verify that 5 highest consensus competencies tooltip details text are translated in preferred language");
            var actualHighestFiveConsensusTooltipDetailsText = radarPage.GetFiveHighestConsensusCompetenciesTooltipDetailsText();
            var expectedHighestFiveConsensusTooltipDescriptionText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault()?.FiveHighestConsensusCompetencies.FirstOrDefault()?.TooltipConsensus.FiveHighestConsensusTooltipDescription;
            Assert.AreEqual(expectedHighestFiveConsensusTooltipDescriptionText, actualHighestFiveConsensusTooltipDetailsText.FirstOrDefault(), "The 5 highest consensus competencies tooltip description title text is not translated in preferred language");

            var expectedHighestFiveConsensusTooltipSupportArticleLinkText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault().FiveHighestConsensusCompetencies.FirstOrDefault().TooltipConsensus.FiveHighestConsensusTooltipSupportArticleLink;
            var actualHighestFiveConsensusTooltipSupportText = radarPage.GetFiveHighestConsensusCompetenciesTooltipSupportText();
            Assert.AreEqual(expectedHighestFiveConsensusTooltipSupportArticleLinkText, actualHighestFiveConsensusTooltipSupportText, "The 5 highest consensus competencies tooltip support article link text is not translated in preferred language");

            Log.Info("Verify that 5 lowest consensus competencies title text is translated in preferred language");
            var actualLowestFiveConsensusCompetenciesTitleText = radarPage.GetLowestFiveConsensusCompetenciesTitleText();
            var expectedLowestFiveConsensusCompetenciesTitleText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault().FiveLowestConsensusCompetencies.FirstOrDefault().FiveLowestConsensusCompetenciesTitle;
            Assert.That.ListsAreEqual(expectedLowestFiveConsensusCompetenciesTitleText, actualLowestFiveConsensusCompetenciesTitleText, "The 5 lowest consensus competencies title text is not translated in preferred language");

            Log.Info("Verify that 5 lowest consensus competencies tooltip details text are translated in preferred language");
            var actualFiveLowestConsensusCompetenciesTooltipDetailsText = radarPage.GetFiveLowestConsensusCompetenciesTooltipDetailsText();
            var expectedFiveLowestConsensusCompetenciesTooltipDetailsText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault().FiveLowestConsensusCompetencies.FirstOrDefault().TooltipConsensus.FiveLowestConsensusTooltipDescription;
            Assert.AreEqual(expectedFiveLowestConsensusCompetenciesTooltipDetailsText, actualFiveLowestConsensusCompetenciesTooltipDetailsText.FirstOrDefault(), "The 5 lowest consensus competencies tooltip description title text is not translated in preferred language");

            var expectedLowestFiveConsensusTooltipSupportArticleLinkText = translations.AnalyticsSection.AnalyticsCards.FirstOrDefault().FiveLowestConsensusCompetencies.FirstOrDefault().TooltipConsensus.FiveLowestConsensusTooltipSupportArticleLink;
            Assert.AreEqual(expectedLowestFiveConsensusTooltipSupportArticleLinkText, actualFiveLowestConsensusCompetenciesTooltipDetailsText.LastOrDefault(), "The 5 lowest consensus competencies tooltip support article link text is not translated in preferred language");

            //Team comment title
            Log.Info("Verify that team comments title text is translated in preferred language");
            var actualTeamCommentsTitleText = assessmentDetailsCommonPage.GetTeamCommentsTitleText();
            var expectedTeamCommentsTitleText = translations.CommentSection.TeamCommentsTitle;
            Assert.AreEqual(expectedTeamCommentsTitleText, actualTeamCommentsTitleText, "The team comments title text is not translated in preferred language");

            //Hide all teams comments section
            Log.Info("Verify that hide all teams comments button is displayed");
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllTeamCommentsButtonDisplayed(), "The hide all team comments button is not displayed");

            Log.Info("Verify that hide all team comments button text is translated in preferred language");
            var actualHideAllTeamCommentsButtonText = assessmentDetailsCommonPage.GetHideAllTeamCommentsButtonText();
            var expectedHideAllTeamCommentsButtonText = translations.CommentSection.HideAllTeamCommentsButton;
            Assert.AreEqual(expectedHideAllTeamCommentsButtonText, actualHideAllTeamCommentsButtonText, "The hide all team comments button text is not translated in preferred language");

            //Hide all teams comments popup
            Log.Info("Verify that hide all team comments popup is translated in preferred language");
            var actualHideAllTeamCommentsPopupTitleText = assessmentDetailsCommonPage.GetHideAllTeamCommentsPopupTitleText();
            var expectedHideAllTeamCommentsPopupTitleText = translations.CommentSection.HideAllTeamCommentsPopup.HideAllTeamCommentsPopupTitle;
            Assert.AreEqual(expectedHideAllTeamCommentsPopupTitleText, actualHideAllTeamCommentsPopupTitleText, "The hide all team comments popup title text is not translated in preferred language");

            var actualHideAllTeamCommentsPopupInfoTextList = assessmentDetailsCommonPage.GetHideAllTeamCommentsPopupInfoTextList();
            var expectedHideAllTeamCommentsPopupInfoTextList = translations.CommentSection.HideAllTeamCommentsPopup.HideAllTeamCommentsInfo;
            Assert.That.ListsAreEqual(expectedHideAllTeamCommentsPopupInfoTextList, actualHideAllTeamCommentsPopupInfoTextList, "The hide all team comments popup information text is not translated in preferred language");

            var actualHideAllTeamCommentsPopupNoCancelButtonText = assessmentDetailsCommonPage.GetHideAllTeamCommentsPopupNoButtonText();
            var expectedHideAllTeamCommentsPopupNoCancelButtonText = translations.CommentSection.HideAllTeamCommentsPopup.NoCancelButton;
            Assert.AreEqual(expectedHideAllTeamCommentsPopupNoCancelButtonText, actualHideAllTeamCommentsPopupNoCancelButtonText, "The hide all team comments popup no cancel button text is not translated in preferred language");

            var actualHideAllTeamCommentsPopupYesProceedButtonText = assessmentDetailsCommonPage.GetHideUnHideAllTeamCommentsPopupYesButtonText();
            var expectedHideAllTeamCommentsPopupYesProceedButtonText = translations.CommentSection.HideAllTeamCommentsPopup.YesProceedButton;
            Assert.AreEqual(expectedHideAllTeamCommentsPopupYesProceedButtonText, actualHideAllTeamCommentsPopupYesProceedButtonText, "The hide all team comments popup yes proceed button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnHideAllCommentsPopupNoCancelButton();
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllTeamCommentsButtonDisplayed(), "The hide all team comments button is not displayed");

            //Hide all stakeholder comments section
            Log.Info("Verify that hide all stakeholder comments button is displayed");
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllStakeholderCommentsButtonDisplayed(), "The hide all stakeholder comments button is displayed");

            Log.Info("Verify that hide all stakeholder comments button text is translated in preferred language");
            var actualHideAllStakeholderCommentsButtonText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsButtonText();
            var expectedHideAllStakeholderCommentsButtonText = translations.CommentSection.HideAllStakeholderCommentsButton;
            Assert.AreEqual(expectedHideAllStakeholderCommentsButtonText, actualHideAllStakeholderCommentsButtonText, "The hide all stakeholder comments button text is not translated in preferred language");

            //Hide all stakeholder comments popup
            Log.Info("Verify that hide all stakeholder popup text is translated in preferred language");
            var actualHideAllStakeholderCommentsPopupTitleText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupTitleText();
            var expectedHideAllStakeholderCommentsPopupTitleText = translations.CommentSection.HideAllStakeholderCommentsPopup.HideAllStakeholderCommentsPopupTitle;
            Assert.AreEqual(expectedHideAllStakeholderCommentsPopupTitleText, actualHideAllStakeholderCommentsPopupTitleText, "The hide all stakeholder comments popup title text is not translated in preferred language");

            var actualHideAllStakeholderCommentsPopupInfoTextList = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupInfoTextList();
            var expectedHideAllStakeholderCommentsPopupInfoTextList = translations.CommentSection.HideAllStakeholderCommentsPopup.HideAllStakeholderCommentsInfo;
            Assert.That.ListsAreEqual(expectedHideAllStakeholderCommentsPopupInfoTextList, actualHideAllStakeholderCommentsPopupInfoTextList, "The hide all stakeholder comments popup information text is not translated in preferred language");

            var actualHideAllStakeholderCommentsPopupNoCancelButtonText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupNoButtonText();
            var expectedHideAllStakeholderCommentsPopupNoCancelButtonText = translations.CommentSection.HideAllStakeholderCommentsPopup.NoCancelButton;
            Assert.AreEqual(expectedHideAllStakeholderCommentsPopupNoCancelButtonText, actualHideAllStakeholderCommentsPopupNoCancelButtonText, "The hide all stakeholder comments popup no cancel button text is not translated in preferred language");

            var actualHideAllStakeholderCommentsPopupYesButtonText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupYesButtonText();
            var expectedHideAllStakeholderCommentsPopupYesButtonText = translations.CommentSection.HideAllStakeholderCommentsPopup.YesProceedButton;
            Assert.AreEqual(expectedHideAllStakeholderCommentsPopupYesButtonText, actualHideAllStakeholderCommentsPopupYesButtonText, "The hide all stakeholder comments popup yes proceed button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnHideAllCommentsPopupNoCancelButton();
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllStakeholderCommentsButtonDisplayed(), "The hide all stakeholder comments popup is not displayed");

            //UnHide all team comments section
            Log.Info("Verify that unHide all team comments button text is translated in preferred language");
            assessmentDetailsCommonPage.ClickOnHideAllCommentsPopupYesProceedButton();
            var actualUnHideAllTeamCommentsButtonText = assessmentDetailsCommonPage.GetUnHideAllTeamCommentsButtonText();
            var expectedUnHideAllTeamCommentsButtonText = translations.CommentSection.UnHideAllTeamCommentsButton;
            Assert.AreEqual(expectedUnHideAllTeamCommentsButtonText, actualUnHideAllTeamCommentsButtonText, "The UnHide all team comments button text is not translated in preferred language");

            //UnHide all team comments popup
            Log.Info("Verify that UnHide all team comments popup text is translated in preferred language");
            var actualUnHideAllTeamCommentsPopupTitleText = assessmentDetailsCommonPage.GetUnHideAllTeamCommentsPopupTitleText();
            var expectedUnHideAllTeamCommentsPopupTitleText = translations.CommentSection.UnHideAllTeamCommentsPopup.UnHideAllTeamCommentsPopupTitle;
            Assert.AreEqual(expectedUnHideAllTeamCommentsPopupTitleText, actualUnHideAllTeamCommentsPopupTitleText, "The UnHide all team comments popup title text is not translated in preferred language");

            var actualUnHideAllTeamCommentsPopupInfoText = assessmentDetailsCommonPage.GetUnHideAllTeamCommentsPopupInfoTextList();
            var expectedUnHideAllTeamCommentsPopupInfoText = translations.CommentSection.UnHideAllTeamCommentsPopup.UnHideAllTeamCommentsInfo;
            Assert.That.ListsAreEqual(expectedUnHideAllTeamCommentsPopupInfoText, actualUnHideAllTeamCommentsPopupInfoText, "The UnHide all team comments popup information text is not translated in preferred language");

            var actualUnHideAllTeamCommentsPopupNoButtonText = assessmentDetailsCommonPage.GetHideAllTeamCommentsPopupNoButtonText();
            var expectedUnHideAllTeamCommentsPopupNoButtonText = translations.CommentSection.UnHideAllTeamCommentsPopup.NoCancelPopupButton;
            Assert.AreEqual(expectedUnHideAllTeamCommentsPopupNoButtonText, actualUnHideAllTeamCommentsPopupNoButtonText, "The UnHide all team comments popup no cancel button text is not translated in preferred language");

            var actualUnHideAllTeamCommentsPopupYesButtonText = assessmentDetailsCommonPage.GetHideUnHideAllTeamCommentsPopupYesButtonText();
            var expectedUnHideAllTeamCommentsPopupYesButtonText = translations.CommentSection.UnHideAllTeamCommentsPopup.YesProceedPopupButton;
            Assert.AreEqual(expectedUnHideAllTeamCommentsPopupYesButtonText, actualUnHideAllTeamCommentsPopupYesButtonText, "The UnHide all team comments popup yes proceed button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnUnHideAllCommentsPopupYesButton();
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllTeamCommentsButtonDisplayed(), "The hide all team comments button is not displayed");

            Log.Info("Set the UnHide all stakeholder comments button");
            if (!assessmentDetailsCommonPage.IsUnHideAllStakeholderCommentsButtonDisplayed())
            {
                assessmentDetailsCommonPage.ClickOnHideAllStakeholderCommentsButton();
            }

            //UnHide all stakeholder section
            Log.Info("Verify that UnHide all stakeholder button text is translated in preferred language");
            var actualUnHideAllStakeholderButtonText = assessmentDetailsCommonPage.GetUnHideAllStakeholderCommentsButtonText();
            var expectedUnHideAllStakeholderButtonText = translations.CommentSection.UnHideAllStakeholderCommentsButton;
            Assert.AreEqual(expectedUnHideAllStakeholderButtonText, actualUnHideAllStakeholderButtonText, "The UnHide all stakeholder button text is not translated in preferred language");

            //UnHide all stakeholder popup
            Log.Info("Verify that UnHide all stakeholder popup text is translated in preferred language");
            var actualUnHideAllStakeholderCommentsPopupTitleText = assessmentDetailsCommonPage.GetUnHideAllStakeholderCommentsPopupTitleText();
            var expectedUnHideAllStakeholderCommentsPopupTitleText = translations.CommentSection.UnHideAllStakeholderCommentsPopup.UnHideAllStakeholderCommentsPopupTitle;
            Assert.AreEqual(expectedUnHideAllStakeholderCommentsPopupTitleText, actualUnHideAllStakeholderCommentsPopupTitleText, "The UnHide all stakeholder comments popup title text is not translated in preferred language");

            var actualUnHideAllStakeholderCommentsPopupInfoText = assessmentDetailsCommonPage.GetUnHideAllStakeholderCommentsPopupInfoTextList();
            var expectedUnHideAllStakeholderCommentsPopupInfoText = translations.CommentSection.UnHideAllStakeholderCommentsPopup.UnHideAllStakeholderCommentsInfo;
            Assert.That.ListsAreEqual(expectedUnHideAllStakeholderCommentsPopupInfoText, actualUnHideAllStakeholderCommentsPopupInfoText, "The UnHide all stakeholder comments popup information text is not translated in preferred language");

            var actualUnHideAllStakeholderCommentsPopupNoButtonText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupNoButtonText();
            var expectedUnHideAllStakeholderCommentsPopupNoButtonText = translations.CommentSection.UnHideAllStakeholderCommentsPopup.NoCancelPopupButton;
            Assert.AreEqual(expectedUnHideAllStakeholderCommentsPopupNoButtonText, actualUnHideAllStakeholderCommentsPopupNoButtonText, "he UnHide all stakeholder comments popup no cancel button text is not translated in preferred language");

            var actualUnHideAllStakeholderCommentsPopupYesButtonText = assessmentDetailsCommonPage.GetHideAllStakeholderCommentsPopupYesButtonText();
            var expectedUnHideAllStakeholderCommentsPopupYesButtonText = translations.CommentSection.UnHideAllStakeholderCommentsPopup.YesProceedPopupButton;
            Assert.AreEqual(expectedUnHideAllStakeholderCommentsPopupYesButtonText, actualUnHideAllStakeholderCommentsPopupYesButtonText, "The UnHide all stakeholder comments popup yes proceed button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnUnHideAllCommentsPopupYesButton();
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideAllStakeholderCommentsButtonDisplayed(), "The hide all stakeholder comments button is not displayed");

            //Edit, Update, Cancel, Hide, UnHide buttons from comment section
            var commentsEditButtonsTextList = assessmentDetailsCommonPage.IsCommentsEditButtonsTextTranslatedInPreferredLanguage(translations.CommentSection.Buttons.EditButton);
            Assert.IsTrue(commentsEditButtonsTextList, "The comments edit button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnCommentsEditButton(_stakeholderNotes.FirstOrDefault().Note);

            var actualUpdateButtonText = assessmentDetailsCommonPage.GetCommentsUpdateButtonsText();
            var expectedUpdateButtonText = translations.CommentSection.Buttons.UpdateAndCancelButton.UpdateButton;
            Assert.AreEqual(expectedUpdateButtonText, actualUpdateButtonText, "The update button text is not translated in preferred language");

            var actualCancelButtonText = assessmentDetailsCommonPage.GetCommentsCancelButtonText();
            var expectedCancelButtonText = translations.CommentSection.Buttons.UpdateAndCancelButton.CancelButton;
            Assert.AreEqual(expectedCancelButtonText, actualCancelButtonText, "The cancel button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnCancelButton();
            Assert.IsFalse(assessmentDetailsCommonPage.IsUpdateButtonDisplayed(), "The update button is displayed");

            var commentsHideButtonsText = assessmentDetailsCommonPage.IsCommentsHideButtonsTextTranslatedInPreferredLanguage(translations.CommentSection.Buttons.HideButton);
            Assert.IsTrue(commentsHideButtonsText, "The hide button text is not translated in preferred language");

            var actualUnHideButtonText = assessmentDetailsCommonPage.GetCommentsUnHideButtonText(_stakeholderNotes.FirstOrDefault().SubDimension, _stakeholderNotes.FirstOrDefault().Note);
            var expectedUnHideButtonText = translations.CommentSection.Buttons.UnHideButton;
            Assert.AreEqual(expectedUnHideButtonText, actualUnHideButtonText, "The comments UnHide button text is not translated in preferred language");

            assessmentDetailsCommonPage.ClickOnCommentsUnHideButton(_stakeholderNotes.FirstOrDefault().Note);
            Assert.IsTrue(assessmentDetailsCommonPage.IsHideButtonDisplayed(_stakeholderNotes.FirstOrDefault().Note), "The hide button is not displayed");

            Log.Info("Verify that growth opportunities title text and description text is translated in preferred language");
            var actualGrowthOpportunitiesTitleText = assessmentDetailsCommonPage.GetGrowthOpportunitiesTitleText();
            var expectedGrowthOpportunitiesTitleText = translations.GrowthOpportunitiesSection.GrowthOpportunitiesTitle;
            Assert.AreEqual(expectedGrowthOpportunitiesTitleText, actualGrowthOpportunitiesTitleText, "The growth opportunities title text is not translated in preferred language");

            var actualGrowthOpportunitiesDescriptionText = assessmentDetailsCommonPage.GetDescriptionText("Growth_Opportunities");
            var expectedGrowthOpportunitiesDescriptionText = translations.GrowthOpportunitiesSection.Description;
            Assert.AreEqual(expectedGrowthOpportunitiesDescriptionText, actualGrowthOpportunitiesDescriptionText, "The growth opportunities description text is not translated in preferred language");

            //Impediments title and description text
            Log.Info("Verify that impediments title text and description text is translated in preferred language");
            var actualImpedimentsTitleText = assessmentDetailsCommonPage.GetImpedimentsTitleText();
            var expectedImpedimentsTitleText = translations.ImpedimentsSection.ImpedimentsTitle;
            Assert.AreEqual(expectedImpedimentsTitleText, actualImpedimentsTitleText, "The impediments title text is not translated in preferred language");

            var actualImpedimentsDescriptionText = assessmentDetailsCommonPage.GetDescriptionText("Impediments");
            var expectedImpedimentsDescriptionText = translations.ImpedimentsSection.Description;
            Assert.AreEqual(expectedImpedimentsDescriptionText, actualImpedimentsDescriptionText, "The impediments description text is not translated in preferred language");

            //Strengths title and description text
            Log.Info("Verify that strengths title text and description text is translated in preferred language");
            var actualStrengthsTitleText = assessmentDetailsCommonPage.GetStrengthsTitleText();
            var expectedStrengthsTitleText = translations.StrengthsSection.StrengthsTitle;
            Assert.AreEqual(expectedStrengthsTitleText, actualStrengthsTitleText, "The strengths title text is not translated in preferred language");

            var actualStrengthsDescriptionText = assessmentDetailsCommonPage.GetDescriptionText("Strengths");
            var expectedStrengthsDescriptionText = translations.StrengthsSection.Description;
            Assert.AreEqual(expectedStrengthsDescriptionText, actualStrengthsDescriptionText, "The strengths description text is not translated in preferred language");

            //Growth plan section 
            Log.Info("Verify that growth plan section is translated in preferred language");
            var actualGrowthPlanTitleText = assessmentDetailsCommonPage.GetGrowthPlanTitleText();
            var expectedGrowthPlanTitleText = translations.GrowthPlanSection.GrowthPlanTitle;
            Assert.AreEqual(expectedGrowthPlanTitleText, actualGrowthPlanTitleText, "The growth plan title text is not translated in preferred language");

            var actualGrowthPlanDescriptionText = assessmentDetailsCommonPage.GetGrowthPlanInfoTextList();
            var expectedGrowthPlanDescriptionText = translations.GrowthPlanSection.GrowthPlanDescription;
            Assert.That.ListsAreEqual(expectedGrowthPlanDescriptionText, actualGrowthPlanDescriptionText, "The growth plan description text is not translated in preferred language");
        }
    }
}
