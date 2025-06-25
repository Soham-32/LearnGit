using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.ObjectFactories;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Edit;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.AssessmentList;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment.Batches;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Add;
using AgilityHealth_Automation.PageObjects.AgilityHealth.GrowthPlan.Radar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageFeatures;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Survey;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.GrowthPlan.Custom;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadarDimension = AtCommon.Dtos.Radars.Custom.RadarDimension;
using AtCommon.Dtos.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Assessment;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.Integration;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageUsers;
using AtCommon.Dtos.Assessments.PulseV2;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Settings.ManageRadar.Add;
using AtCommon.Dtos.Radars.Custom;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.MultiTeam.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;

namespace AgilityHealth_Automation.SetUpTearDown
{
    public class SetUpMethods
    {
        public Logger Log { get; }
        public TestEnvironment TestEnvironment { get; set; }

        public SetUpMethods(TestContext testContext, TestEnvironment testEnvironment)
        {
            TestEnvironment = testEnvironment;
            Log = new Logger(testContext);
            var fileUtil = new FileUtil();
            var fullPath = testContext.FullyQualifiedTestClassName.Split('.');
            var classname = fullPath[fullPath.Length - 1];
            Log = new Logger(
                $@"{fileUtil.GetBasePath()}Resources\Logs\[ClassSetup]_{classname}_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log");
        }

        public CompanyResponse CreateCompanyAndCompanyAdmin(AddCompanyRequest companyRequest, User user)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var setUpApi = new SetupTeardownApi(TestEnvironment);
                var login = new LoginPage(driver, Log);
                var manageUserPage = new ManageUserPage(driver, Log, UserType.CompanyAdmin);
                var topNav = new TopNavigation(driver, Log);

                companyRequest.RequireSecurityQuestions = false;
                var companyResponse = setUpApi.CreateCompany(companyRequest, user).GetAwaiter().GetResult();

                Log.Info($"Setting up CA user for the company - {companyResponse.Name}");
                login.NavigateToPage();
                if (user != null) login.LoginToApplication(user.Username, user.Password);
                manageUserPage.NavigateToPage(companyResponse.Id);
                manageUserPage.SelectTab();
                manageUserPage.ClickOnSendEmailIcon(companyRequest.CompanyAdminEmail);
                driver.NavigateToPage(GmailUtil.GetUserActivationLink("Confirm your account", companyRequest.CompanyAdminEmail));
                login.SetUserPassword(SharedConstants.CommonPassword);
                topNav.LogOut();

                return companyResponse;
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }


        public void AddTeamAssessment(int teamId, TeamAssessmentInfo assessmentInfo, User user = null)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {

                var login = new LoginPage(driver, Log);
                var assessmentProfile = new AssessmentProfilePage(driver, Log);
                var selectTeamMembers = new SelectTeamMembersPage(driver, Log);
                var selectStakeHolder = new SelectStakeHolderPage(driver, Log);
                var reviewAndLaunch = new ReviewAndLaunchPage(driver, Log);

                login.NavigateToPage();

                user ??= BaseTest.User.IsMember() ?
                    TestEnvironment.UserConfig.GetUserByDescription("user 3") : BaseTest.User;

                login.LoginToApplication(user.Username, user.Password);

                assessmentProfile.NavigateToPage(teamId);
                assessmentProfile.FillDataForAssessmentProfile(assessmentInfo);
                assessmentProfile.ClickOnNextSelectTeamMemberButton();

                foreach (var member in assessmentInfo.TeamMembers)
                {
                    selectTeamMembers.SelectTeamMemberByName(member);
                }

                selectTeamMembers.ClickOnNextSelectStakeholdersButton();

                foreach (var stake in assessmentInfo.StakeHolders)
                {
                    selectStakeHolder.SelectStakeHolderByName(stake);
                }

                selectStakeHolder.ClickOnReviewAndFinishButton();

                reviewAndLaunch.SelectSendToEveryone();
                if (assessmentInfo.SendRetroSurvey)
                {
                    reviewAndLaunch.ClickOnSendRetrospectiveSurveyCheckbox();
                }
                if (assessmentInfo.StartDate.CompareTo(new DateTime()) != 0)
                {
                    reviewAndLaunch.SelectStartAssessmentDate(assessmentInfo.StartDate);
                }
                if (assessmentInfo.EndDate.CompareTo(new DateTime()) != 0)
                {
                    reviewAndLaunch.SelectEndAssessmentDate(assessmentInfo.EndDate);
                }
                reviewAndLaunch.ClickOnPublish();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void TeamMemberAccessAtTeamLevel(int teamId, string email, User user = null)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            try
            {
                var login = new LoginPage(driver, Log);
                var addTeamMemberPage = new AddTeamMemberPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                addTeamMemberPage.NavigateToTeamPage(teamId);
                addTeamMemberPage.ClickOnTeamMemberTeamAccessButton(email);

            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver.Quit();
            }
        }



        public void CompleteTeamMemberSurvey(List<EmailSearch> emailSearchList, List<int> teamMemberAnswers)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2
            };

            for (int i = 0; i < emailSearchList.Count; i++)
            {
                CompleteTeamMemberSurvey(emailSearchList[i], ansValue: teamMemberAnswers[i]);
            }
        }
        public void CompleteStakeholderSurvey(string teamName, List<string> emailAddresses, string assessmentName, List<int> stakeHolderAnswers)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2
            };
            for (int i = 0; i < emailAddresses.Count; i++)
            {
                CompleteStakeholderSurvey(teamName, emailAddresses[i], assessmentName, stakeHolderAnswers[i]);
            }
        }

        public void CompleteTeamMemberSurvey(EmailSearch search, List<DimensionNote> notes = null, int ansValue = 0, bool isQuestionSpecific = false, int memberIndex = 0, List<RadarDimension> surveyData = null)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var surveyPage = new SurveyPage(driver, Log);

                notes ??= new List<DimensionNote>();
                var surveyLink = GmailUtil.GetSurveyLink(search.Subject, search.To, search.Labels.First());
                surveyPage.NavigateToUrl(surveyLink);

                surveyPage.ConfirmIdentity();
                surveyPage.ClickStartSurveyButton();
                if (isQuestionSpecific)
                {
                    surveyPage.SubmitSurveyPerQuestion(memberIndex, surveyData);
                }
                else
                {
                    if (ansValue == 0) surveyPage.SubmitRandomSurvey();
                    else surveyPage.SubmitSurvey(ansValue);
                }

                
                surveyPage.EnterNotesForDimension("Clarity", notes);
                surveyPage.ClickNextButton();

                surveyPage.EnterNotesForDimension("Performance", notes);
                surveyPage.ClickNextButton();

                surveyPage.EnterNotesForDimension("Leadership", notes);
                surveyPage.ClickNextButton();

                surveyPage.EnterNotesForDimension("Culture", notes);
                surveyPage.ClickNextButton();

                surveyPage.EnterNotesForDimension("Foundation", notes);
                surveyPage.ClickNextButton();

                surveyPage.EnterOpenEndedNotes(notes);
                surveyPage.ClickFinishButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void CompletePulseSurvey(EmailSearch search, RadarQuestionDetailsV2Response questions, int ansValue = 0)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var surveyPage = new SurveyPage(driver, Log);

                var surveyLink = GmailUtil.GetPulseSurveyLink(search.Subject, search.To, search.Labels.First());
                surveyPage.NavigateToUrl(surveyLink);

                surveyPage.ConfirmIdentity();

                if (ansValue == 0) surveyPage.SubmitRandomSurvey();
                else surveyPage.SubmitSurvey(ansValue);

                surveyPage.ClickStartSurveyButton();
                for (var i = 0; i < questions.Dimensions.Count() - 1; i++)
                {
                    surveyPage.ClickNextButton();
                }
                surveyPage.ClickFinishButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void CompleteStakeholderSurvey(string teamName, string emailAddress, string assessmentName, int ansValue, List<DimensionNote> notes = null)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var surveyPage = new SurveyPage(driver, Log);

                var surveyLink = GmailUtil.GetSurveyLink(
                    SharedConstants.TeamAssessmentSubject(teamName, assessmentName), emailAddress);

                driver.NavigateToPage(surveyLink);

                surveyPage.ConfirmIdentity();
                surveyPage.SubmitSurvey(ansValue);

                surveyPage.ClickStartSurveyButton();

                if (notes != null)
                {
                    surveyPage.EnterNotesForDimension("Performance", notes);
                    surveyPage.ClickNextButton();
                    surveyPage.EnterOpenEndedNotes(notes);
                }
                else
                {
                    surveyPage.ClickNextButton();
                }

                surveyPage.ClickFinishButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void CompleteIndividualSurvey(string email, string pointOfContact, int answer = 0)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);

            try
            {
                var surveyPage = new SurveyPage(driver, Log);

                var surveyLink = GmailUtil.GetSurveyLink(SharedConstants.IaEmailParticipantSubject, email, "unread", pointOfContact);
                if (string.IsNullOrEmpty(surveyLink)) { throw new Exception("Failed to get survey link from gmail"); }

                driver.NavigateToPage(surveyLink);

                surveyPage.ConfirmIdentity();
                surveyPage.ClickStartSurveyButton();
                if (answer != 0)
                    surveyPage.SubmitSurvey(answer);
                else
                    surveyPage.SubmitRandomSurvey();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickNextButton();
                surveyPage.ClickFinishButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                driver?.Quit();
            }
        }

        public void AddTeamAssessmentAndGi(int teamId, TeamAssessmentInfo assessmentInfo, List<GrowthItem> giList)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {

                var login = new LoginPage(driver, Log);
                var teamAssessmentDashboard = new TeamAssessmentDashboardPage(driver, Log);
                var assessmentProfile = new AssessmentProfilePage(driver, Log);
                var selectTeamMembers = new SelectTeamMembersPage(driver, Log);
                var selectStakeHolder = new SelectStakeHolderPage(driver, Log);
                var reviewAndLaunch = new ReviewAndLaunchPage(driver, Log);
                var addGrowthItemPopup = new AddGrowthItemPopupPage(driver, Log);
                var growthItemGridView = new GrowthItemGridViewWidget(driver, Log);

                login.NavigateToPage();

                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                assessmentProfile.NavigateToPage(teamId);

                assessmentProfile.FillDataForAssessmentProfile(assessmentInfo.AssessmentType, assessmentInfo.AssessmentName);
                assessmentProfile.ClickOnNextSelectTeamMemberButton();

                foreach (var member in assessmentInfo.TeamMembers)
                {
                    selectTeamMembers.SelectTeamMemberByName(member);
                }

                selectTeamMembers.ClickOnNextSelectStakeholdersButton();

                foreach (var stake in assessmentInfo.StakeHolders)
                {
                    selectStakeHolder.SelectStakeHolderByName(stake);
                }

                selectStakeHolder.ClickOnReviewAndFinishButton();

                reviewAndLaunch.SelectSendToEveryone();
                reviewAndLaunch.ClickOnPublish();

                teamAssessmentDashboard.ClickOnRadar(assessmentInfo.AssessmentName);
                growthItemGridView.SwitchToGridView();

                foreach (var gi in giList)
                {
                    growthItemGridView.ClickAddNewGrowthItem();
                    addGrowthItemPopup.EnterGrowthItemInfo(gi);
                    addGrowthItemPopup.ClickSaveButton();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";

                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }
        public void DeleteAssessmentsIfPresent(int teamId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var login = new LoginPage(driver, Log);
            try
            {
                var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(driver, Log);
                var taEditPage = new TeamAssessmentEditPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                teamAssessmentDashboardPage.NavigateToPage(teamId);
                if (teamAssessmentDashboardPage.IsAnyAssessmentsDisplayed())
                {
                    var links = teamAssessmentDashboardPage.GetAssessmentEditLinks();
                    foreach (var link in links)
                    {
                        driver.NavigateToPage(link);
                        taEditPage.ClickOnDeleteAssessmentButtonAndChooseRemoveOption();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void LaunchAhfSurvey(int teamId, string assessmentName)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);

            try
            {
                var login = new LoginPage(driver, Log);
                var teamAssessmentDashboard = new TeamAssessmentDashboardPage(driver, Log);
                var taEditPage = new TeamAssessmentEditPage(driver, Log);

                login.NavigateToPage();

                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                teamAssessmentDashboard.NavigateToPage(teamId);

                teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");

                taEditPage.ClickEditDetailButton();

                var editedAssessment = new TeamAssessmentInfo
                {
                    SendRetroSurvey = true,
                    SendRetroSurveyOption = "Launch Now",
                    StartDate = DateTime.Today
                };

                taEditPage.FillDataForAssessmentProfile(editedAssessment);

                taEditPage.EditPopup_ClickUpdateButton();
                new SeleniumWait(driver).HardWait(30000);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void ScheduleTeamBatchAssessment(BatchAssessment batchAssessment)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);

            try
            {
                var login = new LoginPage(driver, Log);
                var dashBoardPage = new TeamDashboardPage(driver, Log);
                var assessmentListTabPage = new AssessmentDashboardListTabPage(driver, Log);
                var batchesDashboardPage = new BatchesTabPage(driver, Log);
                var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(driver, Log);

                login.NavigateToPage();

                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                dashBoardPage.ClickAssessmentDashBoard();

                assessmentListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

                batchesDashboardPage.ClickPlusButton();

                createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);

                createBatchAssessmentPopupPage.ClickScheduleButton();

                createBatchAssessmentPopupPage.ClickScheduleYesProceedButton();

                createBatchAssessmentPopupPage.ClickOkButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void CreateTeamBatchAssessment(BatchAssessment batchAssessment)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);

            try
            {
                var login = new LoginPage(driver, Log);
                var dashBoardPage = new TeamDashboardPage(driver, Log);
                var assessmentListTabPage = new AssessmentDashboardListTabPage(driver, Log);
                var batchesDashboardPage = new BatchesTabPage(driver, Log);
                var createBatchAssessmentPopupPage = new CreateBatchAssessmentPopupPage(driver, Log);

                login.NavigateToPage();

                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                dashBoardPage.ClickAssessmentDashBoard();

                assessmentListTabPage.ClickOnTab(AssessmentDashboardBasePage.TabSelection.BatchesTab);

                batchesDashboardPage.ClickPlusButton();

                createBatchAssessmentPopupPage.EnterBatchAssessment(batchAssessment);

                createBatchAssessmentPopupPage.ClickSendAndLaunchNowButton();

                createBatchAssessmentPopupPage.ClickOnCreateEditBatchAssessmentPopupYesProceedButton();

                createBatchAssessmentPopupPage.ClickOkButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void AddGiForMTeam(int multiTeamId, GrowthItem growthItemInfo)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var growthItemGridView = new GrowthItemGridViewWidget(driver, Log);
                var addGrowthItemPopup = new AddGrowthItemPopupPage(driver, Log);
                var mtEtDashboardPage = new MtEtDashboardPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                mtEtDashboardPage.NavigateToPage(multiTeamId);
                mtEtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
                addGrowthItemPopup.ClickSaveButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }
        public void AddGiForETeam(int enterpriseTeamId, GrowthItem growthItemInfo)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var growthItemGridView = new GrowthItemGridViewWidget(driver, Log);
                var addGrowthItemPopup = new AddGrowthItemPopupPage(driver, Log);
                var mtEtDashboardPage = new MtEtDashboardPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                mtEtDashboardPage.NavigateToPage(enterpriseTeamId, true);
                mtEtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

                growthItemGridView.ClickAddNewGrowthItem();
                addGrowthItemPopup.EnterGrowthItemInfo(growthItemInfo);
                addGrowthItemPopup.ClickSaveButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void PullGi(int teamId, string title, TeamType teamType)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var growthItemGridView = new GrowthItemGridViewWidget(driver, Log);
                var mtEtDashboardPage = new MtEtDashboardPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                var isEt = teamType == TeamType.EnterpriseTeam;
                mtEtDashboardPage.NavigateToPage(teamId, isEt);
                mtEtDashboardPage.ClickOnRadar(SharedConstants.TeamAssessmentType);

                growthItemGridView.ClickPullItemFromSubTeam();
                growthItemGridView.PullItemFromSubTeam(title);
                growthItemGridView.ClickClosePullDialog();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void TurnOnBusinessOutcomesFeature(User user, int companyId)
        {

            var driver = new Browser().SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var manageFeaturesPage = new ManageFeaturesPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                manageFeaturesPage.NavigateToPage(companyId);
                manageFeaturesPage.TurnOnBusinessOutcomesDashboard();
                manageFeaturesPage.ClickUpdateButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void TurnOnOffGrowthItemTypeFieldRequiredFeature(int companyId, bool toggleButton = true)
        {

            var driver = new Browser().SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var manageFeaturesPage = new ManageFeaturesPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                manageFeaturesPage.NavigateToPage(companyId);

                if (toggleButton)
                {
                    manageFeaturesPage.TurnOnGrowthItemTypeFieldRequired();
                }
                else
                {
                    manageFeaturesPage.TurnOffGrowthItemTypeFieldRequired();
                }

                manageFeaturesPage.ClickUpdateButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void StartSharingAssessment(int teamId, string assessmentName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var driver = new Browser().SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var teamAssessmentDashboard = new TeamAssessmentDashboardPage(driver, Log);
                var taEditPage = new TeamAssessmentEditPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                teamAssessmentDashboard.NavigateToPage(teamId);
                teamAssessmentDashboard.SelectRadarLink(assessmentName, "Edit");
                taEditPage.StartSharingAssessmentResult();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void SetUserPassword(string email, string password, string inbox)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var login = new LoginPage(driver, Log);
            try
            {
                driver.NavigateToPage(GmailUtil.GetUserActivationLink(SharedConstants.MemberAccountCreateEmailSubject, email, inbox));
                login.SetUserPassword(password);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                driver?.Quit();
            }
        }

        public void SetupAccountPassword(string email, string password, string label)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var login = new LoginPage(driver, Log);
            try
            {
                driver.NavigateToPage(GmailUtil.GetUserCreateAccountLink(SharedConstants.AccountSetupEmailSubject, email, label));
                login.SetUserPassword(password);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                driver?.Quit();
            }
        }
        public void TurnOnOffDisableFileUploadRequiredFeature(int companyId, bool toggleButton = true)
        {

            var driver = new Browser().SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var manageFeaturesPage = new ManageFeaturesPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                manageFeaturesPage.NavigateToPage(companyId);

                if (toggleButton)
                {
                    manageFeaturesPage.TurnOnDisableFileUpload();
                }
                else
                {
                    manageFeaturesPage.TurnOffDisableFileUpload();
                }

                manageFeaturesPage.ClickUpdateButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void JiraIntegrationLinkTeamToBoard(string jiraPlatform, string jiraBoard, string jiraProjectName, TeamResponse teamResponse, int companyId, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);
                var linkTeamBasePage = new LinkTeamBasePage(driver, Log);
                var manageIntegrationsPage = new ManageIntegrationsPage(driver, Log);

                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                Log.Info($"Navigate to the 'Manage Integrations' page and link the team - {teamResponse.Name} with the Jira board - {jiraBoard} .");
                manageIntegrationsPage.NavigateToPage(companyId);
                manageIntegrationsPage.ClickOnManageButton(jiraPlatform);

                //Unlink the previous team with Jira Automation Board if it is already linked.
                linkTeamBasePage.UnlinkAlreadyLinkedTeam(jiraBoard);
                linkTeamBasePage.SelectJiraProject(jiraProjectName);
                linkTeamBasePage.SelectJiraBoard(jiraBoard);
                linkTeamBasePage.SelectAgilityHealthTeam(teamResponse.Name);
                linkTeamBasePage.ClickOnLinkButton();
            }
            catch (Exception)
            {
                //ignored
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }
        public void CreateRadar(RadarDetails radarInfo)
        {
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {

                var login = new LoginPage(driver, Log);
                var manageRadarPage = new ManageRadarPage(driver, Log);
                var addRadarDetailsPage = new AddRadarDetailsPage(driver, Log);

                Log.Info("Login to the application and navigate to 'Manage Radar' page.");
                login.NavigateToPage();
                login.LoginToApplication(BaseTest.User.Username, BaseTest.User.Password);

                manageRadarPage.NavigateToPage();

                Log.Info("Click on the 'Create New Radar' button and Verify whether the 'Create Radar' button is enabled or disabled on the 'Create Radar' pop-up when selecting the 'Create Radar' radio button.");
                manageRadarPage.ClickOnCreateNewRadarButton();
                manageRadarPage.CreateRadarPopupClickOnCreateRadarRadioButton();
                manageRadarPage.CreateRadarPopupClickOnCreateRadarButton();

                Log.Info("Enter All the Radar Details and Click 'Save and Continue' button on 'Create Radar' page.");
                addRadarDetailsPage.EnterRadarDetails(radarInfo);
                addRadarDetailsPage.EnterMessagesTextsDetails(radarInfo);
                addRadarDetailsPage.ClickOnSaveAndContinueButton();
            }
            catch (Exception)
            {
                //ignored
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public CompanyResponse CreateNewCompanyAndCompanyUser(AddCompanyRequest companyRequest, User user = null)
        {
            var browser = new Browser();
            var driver = browser.SetUp(TestEnvironment);
            try
            {
                var setUpApi = new SetupTeardownApi(TestEnvironment);
                var login = new LoginPage(driver, Log);
                var manageUserPage = new ManageUserPage(driver, Log, UserType.CompanyAdmin);
                var manageFeaturesPage = new ManageFeaturesPage(driver, Log);
                var topNav = new TopNavigation(driver, Log);

                companyRequest.RequireSecurityQuestions = false;
                CompanyResponse companyResponse = setUpApi.CreateCompany(companyRequest).GetAwaiter().GetResult();

                Log.Info($"Setting up CA user for the company - {companyResponse.Name}");
                login.NavigateToPage();
                if (user != null) login.LoginToApplication(user.Username, user.Password);
                manageUserPage.NavigateToPage(companyResponse.Id);
                manageUserPage.SelectTab();
                manageUserPage.ClickOnSendEmailIcon(companyRequest.CompanyAdminEmail);
                manageFeaturesPage.NavigateToPage(companyResponse.Id);
                manageFeaturesPage.TurnOnIntegrations();
                manageFeaturesPage.ClickUpdateButton();
                driver.NavigateToPage(GmailUtil.GetUserActivationLink("Confirm your account", companyRequest.CompanyAdminEmail));
                login.SetUserPassword(SharedConstants.CommonPassword);
                topNav.LogOut();

                return companyResponse;
            }
            catch (Exception)
            {
                //ignored
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
            return null;

        }

        public void JiraIntegrationUnlinkJiraBoard(string jiraIntegration, int companyId, string jiraBoard, string teamName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var linkTeamBasePage = new LinkTeamBasePage(driver, Log);
            var manageIntegrationsPage = new ManageIntegrationsPage(driver, Log);
            var login = new LoginPage(driver, Log);

            try
            {
                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);

                Log.Info($"Navigate to the 'Manage Integrations' page and link the team - {teamName} with the Jira board - {jiraBoard} .");
                manageIntegrationsPage.NavigateToPage(companyId);
                manageIntegrationsPage.ClickOnManageButton(Constants.PlatformJiraCloud);

                //Unlink the previous team with Jira Automation Board if it is already linked.
                if (linkTeamBasePage.IsLinkedBoardNamePresent(jiraBoard))
                {
                    linkTeamBasePage.UnlinkAlreadyLinkedTeam(jiraBoard);
                }
            }
            catch (Exception)
            {
                //ignored
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }
        public void CloseTeamAssessment(int teamId, string assessmentName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            try
            {
                var login = new LoginPage(driver, Log);

                var teamAssessmentDashboardPage = new TeamAssessmentDashboardPage(driver, Log);
                var taEditPage = new TeamAssessmentEditPage(driver, Log);
                var reviewAndLaunch = new ReviewAndLaunchPage(driver, Log);
                login.NavigateToPage();
                login.LoginToApplication(user.Username, user.Password);
                teamAssessmentDashboardPage.NavigateToPage(teamId);
                if (teamAssessmentDashboardPage.IsAnyAssessmentsDisplayed())
                {
                    teamAssessmentDashboardPage.ClickOnTheEditIcon(assessmentName);

                }
                taEditPage.ClickEditDetailButton();
                var editedAssessment = new TeamAssessmentInfo
                {
                    StartDate = DateTime.Today.AddDays(-15),
                    EndDate = DateTime.Today.AddDays(-2),
                };
                reviewAndLaunch.SelectStartAssessmentDate(editedAssessment.StartDate);
                reviewAndLaunch.SelectEndAssessmentDate(editedAssessment.EndDate);
                taEditPage.EditPopup_ClickUpdateButton();
            }
            catch (Exception e)
            {
                Log.Error(e);
                var originalError = $"{e.Message}\nOriginal Stack Trace: \n{e.StackTrace}";
                throw new Exception(originalError);
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }
        public void DeleteRadar(string radarName, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");

            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var login = new LoginPage(driver, Log);
            var manageRadarPage = new ManageRadarPage(driver, Log);
            login.NavigateToPage();
            login.LoginToApplication(user.Username, user.Password);
            try
            {

                var attempt = 0;
                manageRadarPage.NavigateToPage();
                manageRadarPage.ClickOnDeleteRadarIcon(radarName);
                manageRadarPage.DeleteAssessmentPopUpClickOnDeleteButton();
                while (manageRadarPage.IsRadarPresent(radarName) && attempt < 5)
                {
                    manageRadarPage.ClickOnDeleteRadarIcon(radarName);
                    manageRadarPage.DeleteAssessmentPopUpClickOnDeleteButton();
                    attempt++;
                }
            }
            catch
            {
                Log.Info($"'{radarName}' radar was not deleted");
            }
            finally
            {
                //Close browser
                driver?.Quit();
            }
        }

        public void NTier_CreateNTierTeam(string nTierTeamName, List<string> subTeam, User user = null)
        {
            user ??= TestEnvironment.UserConfig.GetUserByDescription("user 1");
            var b = new Browser();
            var driver = b.SetUp(TestEnvironment);
            var dashBoardPage = new TeamDashboardPage(driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(driver, Log);
            var addMtSubTeamPage = new AddMtSubTeamPage(driver, Log);
            var createNTierPage = new CreateNTierPage(driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(driver, Log);//
            var addStakeHolderPage = new AddStakeHolderPage(driver, Log);

            var login = new LoginPage(driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(user.Username, user.Password);

            dashBoardPage.ClickAddATeamButton();
            dashBoardPage.SelectTeamType(TeamType.NTier);
            dashBoardPage.ClickAddTeamButton();
            createNTierPage.InputNTierTeamName(nTierTeamName);
            createNTierPage.ClickCreateButton();
            foreach (var team in subTeam)
            {
                addMtSubTeamPage.SelectSubTeam(team);
            }
            addMtSubTeamPage.ClickAddSubTeamButton();
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToTeamDashboard();
        }
    }
}