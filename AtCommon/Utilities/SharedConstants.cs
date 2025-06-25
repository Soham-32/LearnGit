using AtCommon.Dtos.Teams;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AtCommon.Utilities
{
    public class SharedConstants
    {
        //Common
        public const string CommonPassword = "12345678";

        //Account Setup
        public const string AccountSetupEmailSubject = "Confirm your account";
        public const string MemberAccountCreateEmailSubject = "You now have access to your teams!";

        //Talent Development (IA)
        public const string IaEmailParticipantSubject =
            "Your Talent Development Assessment from AgilityInsights® has launched!";
        public const string IaEmailReviewerSubject =
            "Please Respond – Your Feedback is Requested";
        public const string IndividualAssessmentType = "AT - Individual";
        public const int IndividualAssessmentSurveyId = 145;
        public const string ParticipantAllowInvite = "allowInviteCheckbox";
        public const string ParticipantAllowResultView = "allowResultViewCheckbox";
        public const string EmailTypeParticipant = "Participant";
        public const string EmailTypeReviewer = "Reviewer";
        public const string IndividualAssessmentTypeAccessibleToAll = "Agile Team Coach V2.0";


        //Team Assessment
        public const string TeamAssessmentType = "AT - TH2.5 - DO NOT USE";
        public const int TeamSurveyId = 131;
        public static string TeamAssessmentSubject(string teamName, string assessmentName) =>
            $"{teamName} | {assessmentName} - Please Respond";
        public const string TeamHealthRadarName = "TeamHealth 3.0";
        public const string AtTeamHealth3RadarName = "AT - TH3.0 - DO NOT USE";
        public const int AtTeamHealth3SurveyId = 53;
        public static string ShareAssessmentEmailSubject(string assessmentName) =>
            $"Assessment results for {assessmentName} are available";
        public const string CampaignName = "AT Campaign";

        //Pulse Assessment
        public const string SurveyDimension = "Foundation";
        public const string SurveySubDimension = "Team Structure";
        public const string SurveySubDimensionAgility = "Agility";
        public const string SurveyCompetency = "Size & Skills";
        public const string SurveyCompetencyWorkSpace = "Workspace";
        public const string PulseAssessmentName = "PulseTest(DO NOT DELETE)";
        public static string PulseEmailSubject(string teamName) => $"{teamName} | Pulse Check - Please Respond";

        //Pulse Assessment V2
        public const string PulseMember1 = "Pulse_Mem 1";
        public const string PulseMember2 = "Pulse_Mem 2";
        public const string PulseMember3 = "Pulse_Mem 3";
        public const string PulseMember4 = "Pulse_Mem 4";
        public const string PulseMember5 = "Pulse_Mem 5";

        //Team Info
        public const string NewTeamWorkType = "Software Delivery";
        public const string NewMultiTeamWorkType = "Chapter";
        public const string NewPortfolioTeamWorkType = "Portfolio Team";
        public const string InsightsIndividualTeam1 = "Individual Automation Radar Team 1";
        public const string InsightsIndividualTeam2 = "Individual Automation Radar Team 2";
        public const string InsightsIndividualTeam3 = "Individual Automation Radar Team 3";
        public const string InsightsIndividualTeam4 = "Individual Automation Radar Team 4";
        public const string InsightsEnterpriseTeam1 = "Enterprise Automation Radar Team 1";
        public const string InsightsMultiTeam1 = "Multi Automation Radar Team 1";
        public const string InsightsMultiTeam2 = "Multi Automation Radar Team 2";
        public const string Team = "Automation Normal Team";
        public const string TeamForBatchAssessment = "Automation Team Batch Assessment";
        public const string MultiTeam = "Automation Multi Team";
        public const string MultiTeamForGrowthJourney = "Automation MT for Growth Journey";
        public const string EnterpriseTeam = "Automation Enterprise Team";
        public const string RadarTeam = "Automation Radar Team";
        public const string GoiTeam = "Automation GOI Team";
        public const string UpdateTeam = "Automation Update Team";
        public const string EnterpriseTeamForGrowthJourney = "Automation ET for Growth Journey";
        public const string NotificationTeam = "Automation Team for Notification";
        public const string ArchiveTeam = "Automation Archive Team";
        public const string GiCopyFromPreviousAssessmentTeam = "Automation Team GI Copy From Prev Assess";
        public const string PortfolioTeam = "Automation Portfolio Team";

        // Team Members
        public const string TeamMemberLastName = "Name";
        public const string StakeholderRole = "Sponsor";
        public static AddMemberRequest TeamMember1 = new AddMemberRequest
        {
            FirstName = "AT_Mem",
            LastName = "1",
            Email = "ah_automation+at_mem1@agiletransformation.com"
        };

        public static AddMemberRequest TeamMember2 = new AddMemberRequest
        {
            FirstName = "AT_Mem",
            LastName = "2",
            Email = "ah_automation+at_mem2@agiletransformation.com"
        };

        public static AddMemberRequest TeamMember3 = new AddMemberRequest
        {
            FirstName = "AT_Mem",
            LastName = "3",
            Email = "ah_automation+at_mem3@agiletransformation.com"
        };

        public static AddMemberRequest TeamMember4 = new AddMemberRequest
        {
            FirstName = "AT_Mem",
            LastName = "4",
            Email = "ah_automation+AT_Mem4@agiletransformation.com"
        };

        // Stakeholders
        public static AddMemberRequest Stakeholder1 = new AddMemberRequest
        {
            FirstName = "AT_Stake",
            LastName = "1",
            Email = "ah_automation+at_stake1@agiletransformation.com"
        };

        public static AddMemberRequest Stakeholder2 = new AddMemberRequest
        {
            FirstName = "AT_Stake",
            LastName = "2",
            Email = "ah_automation+at_stake2@agiletransformation.com"
        };

        public static AddMemberRequest Stakeholder3 = new AddMemberRequest
        {
            FirstName = "AT_Stake",
            LastName = "3",
            Email = "ah_automation+at_stake3@agiletransformation.com"
        };

        public static AddMemberRequest Stakeholder4 = new AddMemberRequest
        {
            FirstName = "AT_Stake",
            LastName = "4",
            Email = "ah_automation+at_stake4@agiletransformation.com"
        };

        public static AddMemberRequest Viewer = new AddMemberRequest
        {
            FirstName = "IndividualAggregate",
            LastName = "Viewer",
            Email = "ah_automation+indassess@agiletransformation.com"
        };

        //User info
        public const string TeamTag = "Automation";
        public const string PortfolioTeamTag = "Portfolio Team";
        public const string TeamTagCoaching = "Active Coaching";

        //Company Info
        public static string AccountManagerEmailSubject(string companyName) => $"{companyName} - Number of teams limit reached";
        public const int FakeCompanyId = 3;

        //Ideaboard - Growth Plan Item Category
        public const string IdeaboardIndividualGpi = "Individual";
        public const string IdeaboardTeamGpi = "Team";
        public const string IdeaboardOrganizationalGpi = "Organizational";
        public const string IdeaboardManagementGpi = "Management";

        //User Role Types
        public const string RoleIndividual = "Individual";

        //Radar Dimensions
        public const string DimensionValueDelivered = "Value Delivered";
        public const string DimensionLeadershipAndStyle = "Leadership & Style";
        public const string DimensionClarity = "Clarity";
        public const string DimensionPerformance = "Performance";
        public const string DimensionLeadership = "Leadership";
        public const string DimensionFoundation = "Foundation";
        public const string DimensionFinish = "Finish";
        public const string DimensionNotes = "Notes";
        public const string DimensionCulture = "Culture";

        //Radar CompanyName
        public const string CompanyName = "Master";

        //Radars info
        public const string TeamHealth2Radar = "TH2.5Radar(DoNotDelete)";
        public const string TeamHealth2ForStakeholder = "TH2.5ForStakeholder(DO NOT REMOVE)";
        public const string TeamHealth2ForAgileCoach = "TH2.5RadarForAgileCoach(DO NOT REMOVE)";
        public const string ProgramHealthRadar = "ProgramHealthRadar(DoNotDelete)";
        public const string DevOpsHealthRadar = "DevOpsHealthRadar(DoNotDelete)";
        public const string TechnicalHealthRadar = "TechnicalHealthRadar(DoNotDelete)";
        public const string DevOpsAssessmentRadar = "DevOpsAssessmentRadar(DoNotDelete)";
        public const string MaturityTestingRadarType = "Maturity Testing [DoNotDelete]";
        public const string AssessmentChecklistRadar = "ChecklistAssessment(DoNotDelete)";
        public const string AssessmentHideUnHideCommentsRadar = "AssessmentHideUnHideComments(DoNotDelete)";
        public const string AssessmentResponse25Radar = "AssessmentResponse2.5Radar(DoNotDelete)";
        public const string Th2Radar = "TH2Radar(DoNotRemove)";

        //Other Company Id----Campaign Feature Off for Automation_2FA (DO NOT USE) - 1869
        public const int Automation2FaDoNotUseCompanyId = 1869;
        public const int FakeCampaignId = 1869;

        public static Dictionary<string, string> CountryLanguages = new Dictionary<string, string>
        {
            {"Qatar","Arabic"},
            {"China","Chinese"},
            {"Canada","English"},
            {"France","French"},
            {"Germany","German"},
            {"Hungary","Hungarian"},
            {"Japan","Japanese"},
            {"South Korea","Korean"},
            {"Portugal","Portuguese"},
            {"Mexico","Spanish"},
            {"Turkey","Turkish"}
        };

        public static Dictionary<string, object> GetRandomCityLatAndLong()
        {
            var regionalParametersDict = new Dictionary<string, Dictionary<string, object>>
            {
                {"Doha", new Dictionary<string, object>//Qatar
                {
                    { "latitude", 25.286106},
                    { "longitude", 51.534817},
                    { "accuracy", 100 }
                }},
                {"Beijing", new Dictionary<string, object>//China
                {
                    { "latitude",   39.916668},
                    { "longitude", 116.383331},
                    { "accuracy", 100 }
                }},
                {"Nicolet", new Dictionary<string, object> //Canada
                {
                    { "latitude",   46.216667},
                    { "longitude", -72.616669},
                    { "accuracy", 100 }
                }},
                {"Lyon", new Dictionary<string, object> //France
                {
                    { "latitude",   45.763420},
                    { "longitude", 4.834277},
                    { "accuracy", 100 }
                }},
                {"Berlin", new Dictionary<string, object> //Germany
                {
                    { "latitude", 52.520008 },
                    { "longitude", 13.404954},
                    { "accuracy", 100 }
                }},
                {"Budapest", new Dictionary<string, object> //Hungary
                {
                    { "latitude", 47.497913},
                    { "longitude", 19.040236},
                    { "accuracy", 100 }
                }},
                {"Japan", new Dictionary<string, object> //Tokyo
                {
                    { "latitude", 35.652832},
                    { "longitude",  139.839478},
                    { "accuracy", 100 }
                }},
                {"Busan", new Dictionary<string, object> //South Korea
                {
                    { "latitude",   35.166668},
                    { "longitude", 129.066666},
                    { "accuracy", 100 }
                }},
                {"Lisbon", new Dictionary<string, object> //Portugal
                {
                    { "latitude",  38.736946},
                    { "longitude",-9.142685},
                    { "accuracy", 100 }
                }},
                {"Mexico", new Dictionary<string, object> //Mexico
                {
                    { "latitude", 23.6260333 },
                    { "longitude", -102.5375005},
                    { "accuracy", 100 }
                }},
                {"Istanbul", new Dictionary<string, object> //Turkey
                {
                    { "latitude",   41.015137 },
                    { "longitude", 28.979530},
                    { "accuracy", 100 }
                }}
            };

            var keys = regionalParametersDict.Keys.ToList(); // Convert the keys to a list
            var random = new Random();
            var randomKeyIndex = random.Next(0, keys.Count);
            var randomKey = keys[randomKeyIndex];
            return regionalParametersDict[randomKey];
        }
    }
}
