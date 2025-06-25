using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Dashboard;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Radar;
using AgilityHealth_Automation.SetUpTearDown;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.Team.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Radars.Custom;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.TeamAssessment.Radar
{
    [TestClass]
    [TestCategory("TeamAssessment"), TestCategory("Assessments"), TestCategory("TARadars")]
    public class TeamAssessmentAnalyticsTests : BaseTest
    {
        private static TeamAssessmentInfo _teamAssessment;
        private static TeamHierarchyResponse _team;
        private static TestContext _testContext;
        private static readonly Analytics AnalyticsDataForAfterFillingSurveyByMem1 = new Analytics()
        {
            AnalyticsSections = new List<AnalyticsSection>
                {
                    new AnalyticsSection()
                    {
                        AnalyticsHeader = "Top5Competencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Goals&Outcomes",
                                CompetenciesValue = "100%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Vision&Purpose",
                                CompetenciesValue = "90%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Weekly/Iteration",
                                CompetenciesValue = "80%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName =  "Monthly/Release",
                                CompetenciesValue = "70%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Quarterly/Roadmap",
                                CompetenciesValue = "60%"
                            }

                        }
                    },
                    new AnalyticsSection()
                    {
                        AnalyticsHeader = "Lowest5Competencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Size&Skills",
                                CompetenciesValue =  "10%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Allocation&Stability",
                                CompetenciesValue = "20%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Workspace",
                                CompetenciesValue = "30%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "SustainablePace",
                                CompetenciesValue = "40%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "ImpedimentManagement",
                                CompetenciesValue = "50%"
                            }

                        }
                    }
                    }
        };

        private static readonly Analytics AnalyticsDataForAfterFillingSurveyByMem2 = new Analytics()
        {
            AnalyticsSections = new List<AnalyticsSection>
                {
                    new AnalyticsSection()
                    {
                        AnalyticsHeader = "Top5Competencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Goals&Outcomes",
                                CompetenciesValue = "55%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Vision&Purpose",
                                CompetenciesValue = "50%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Weekly/Iteration",
                                CompetenciesValue = "45%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName =  "Monthly/Release",
                                CompetenciesValue = "40%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Quarterly/Roadmap",
                                CompetenciesValue = "35%"
                            }

                        }
                    },
                    new AnalyticsSection()
                    {
                        AnalyticsHeader = "Lowest5Competencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Size&Skills",
                                CompetenciesValue =  "10%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Allocation&Stability",
                                CompetenciesValue = "15%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Workspace",
                                CompetenciesValue = "20%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "SustainablePace",
                                CompetenciesValue = "25%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "ImpedimentManagement",
                                CompetenciesValue = "30%"
                            }
                        }
                    },
                    new AnalyticsSection()
                     {
                        AnalyticsHeader = "5HighestConsensusCompetencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Size&Skills",
                                CompetenciesValue =  "100%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Allocation&Stability",
                                CompetenciesValue = "90%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Workspace",
                                CompetenciesValue = "80%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "SustainablePace",
                                CompetenciesValue = "70%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "ImpedimentManagement",
                                CompetenciesValue = "60%"
                            }

                        }
                    },
                    new AnalyticsSection()
                    {
                        AnalyticsHeader = "5LowestConsensusCompetencies",

                        AnalyticsValues = new List<AnalyticsValue>()
                        {
                            new AnalyticsValue()
                            {
                                CompetenciesName ="Goals&Outcomes",
                                CompetenciesValue =  "10%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Vision&Purpose",
                                CompetenciesValue = "20%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName = "Weekly/Iteration",
                                CompetenciesValue = "30%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName =  "Monthly/Release",
                                CompetenciesValue = "40%"
                            },
                            new AnalyticsValue()
                            {
                                CompetenciesName =  "Quarterly/Roadmap",
                                CompetenciesValue = "50%"
                            }
                        }
                    }
                }
        };


        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            _team = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.Team);
            _teamAssessment = new TeamAssessmentInfo
            {
                AssessmentType = SharedConstants.TeamAssessmentType,
                AssessmentName = RandomDataUtil.GetAssessmentName() + CSharpHelpers.RandomNumber(),
                TeamMembers = new List<string> { SharedConstants.TeamMember1.FullName(), SharedConstants.TeamMember2.FullName() },
                StakeHolders = new List<string> { SharedConstants.Stakeholder1.FullName(), SharedConstants.Stakeholder2.FullName() }
            };
            var setup = new SetUpMethods(testContext, TestEnvironment);
            setup.AddTeamAssessment(_team.TeamId, _teamAssessment);

            _testContext = testContext;
        }

        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin")]
        public void Verify_TeamAssessment_Analytics()
        {
            var responseWithQuestionsAnswersJson = File.ReadAllText(new FileUtil().GetBasePath() + "Resources/TestData/Survey/TH25ResponseWithQuestionsAnswers.json");
            var dimensionsList = JsonConvert.DeserializeObject<AllDimensions>(responseWithQuestionsAnswersJson)?.Dimensions.ToList();

            var login = new LoginPage(Driver, Log);
            var radarPage = new RadarPage(Driver, Log);
            var teamAssessmentDashboard = new TeamAssessmentDashboardPage(Driver, Log);
            var setup = new SetUpMethods(_testContext, TestEnvironment);

            Log.Info("Survey Fill as Member 1");
            var emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                To = SharedConstants.TeamMember1.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            setup.CompleteTeamMemberSurvey(emailSearch, null, 3, true, memberIndex: 0, dimensionsList);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            var actualTopFiveCompetenciesList = radarPage.GetTopFiveCompetenciesList().ToList();
            var actualLowestFiveCompetenciesList = radarPage.GetLowestFiveCompetenciesList().ToList();

            Log.Info("Verify Top Five / Lowest Five Competencies Values");
            var expectedTopFiveCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem1.AnalyticsSections
                .First(a => a.AnalyticsHeader.Equals("Top5Competencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedTopFiveCompetenciesList, actualTopFiveCompetenciesList, "'Top-5 Competencies List' doesn't match");

            var expectedLowestFiveCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem1.AnalyticsSections
                .First(s => s.AnalyticsHeader.Equals("Lowest5Competencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedLowestFiveCompetenciesList, actualLowestFiveCompetenciesList, "'Lowest-5 Competencies List' doesn't match");

            Assert.IsFalse(radarPage.IsHighestConsensusCompetenciesPresent(), "Highest consensus competencies are present");
            Assert.IsFalse(radarPage.IsLowestConsensusCompetenciesPresent(), "Lowest consensus competencies are present");

            Log.Info("Survey Fill as Member 2");
            emailSearch = new EmailSearch
            {
                Subject = SharedConstants.TeamAssessmentSubject(_team.Name, _teamAssessment.AssessmentName),
                To = SharedConstants.TeamMember2.Email,
                Labels = new List<string> { GmailUtil.MemberEmailLabel }
            };
            setup.CompleteTeamMemberSurvey(emailSearch, null, 3, true, memberIndex: 1, dimensionsList);

            teamAssessmentDashboard.NavigateToPage(_team.TeamId);
            teamAssessmentDashboard.ClickOnRadar(_teamAssessment.AssessmentName);
            var actualFiveLowestConsensusCompetenciesList = radarPage.GetFiveLowestConsensusCompetenciesList().ToList();
            var actualFiveHighestConsensusCompetenciesList = radarPage.GetFiveHighestConsensusCompetenciesList().ToList();
            var actualFiveTopCompetenciesList = radarPage.GetTopFiveCompetenciesList().ToList();
            var actualFiveLowestCompetenciesList = radarPage.GetLowestFiveCompetenciesList().ToList();

            Log.Info("Verify Top Five / Lowest Five Competencies Values");
            var expectedFiveTopCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem2.AnalyticsSections
                .First(a => a.AnalyticsHeader.Equals("Top5Competencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedFiveTopCompetenciesList, actualFiveTopCompetenciesList, "'Top-5 Competencies List' doesn't match");


            var expectedFiveLowestCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem2.AnalyticsSections
                .First(s => s.AnalyticsHeader.Equals("Lowest5Competencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedFiveLowestCompetenciesList, actualFiveLowestCompetenciesList, "'Lowest-5 Competencies List' doesn't match");


            var expectedFiveHighestConsensusCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem2.AnalyticsSections
                .First(s => s.AnalyticsHeader.Equals("5HighestConsensusCompetencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedFiveHighestConsensusCompetenciesList, actualFiveHighestConsensusCompetenciesList, "'Highest-5 Consensus Competencies List' doesn't match");


            var expectedFiveLowestConsensusCompetenciesList = AnalyticsDataForAfterFillingSurveyByMem2.AnalyticsSections
                .First(s => s.AnalyticsHeader.Equals("5LowestConsensusCompetencies")).AnalyticsValues
                .Select(x => x.CompetenciesName + x.CompetenciesValue).ToList();

            Assert.That.ListsAreEqual(expectedFiveLowestConsensusCompetenciesList, actualFiveLowestConsensusCompetenciesList, "'Lowest-5 Consensus Competencies List' doesn't match");

        }
    }
}