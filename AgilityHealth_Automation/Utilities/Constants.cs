using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.TeamAssessment.Radar;
using AtCommon.Utilities;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AgilityHealth_Automation.Utilities
{
    public class Constants
    {

        // logging
        public static int LogRetentionDays = 0;

        //Team Info
        public const string GoiTeam = "Automation GOI Team";
        public const string GoiTeam2 = "Automation GOI Team 2";
        public const string MultiTeamForRadar = "Automation MultiTeam For Radar";
        public const string EnterpriseTeamForRadar = "Automation Enterprise Team for Radar";
        public const string TeamForMultiTeamRadar = "Automation Team For Multi Team Radar";
        public const string MultiTeamForRadar2 = "Automation Multi Team for Radar 2";
        public const string TeamForMultiTeamRadarWorkType = "Software Delivery";
        public const string MultiTeamForBenchmarking = "Automation MultiTeam For Benchmarking";
        public const string TeamForGiCopyFromPreviousAssessment = "Automation Team GI Copy From Prev Assess";
        public const string MultiTeamForGi = "Automation MT for GI";
        public const string TeamForBatchAssessment = "Automation Team Batch Assessment";
        public const string TeamForIndividualAssessmentGi = "Automation Team for IA GI";
        public const string MultiTeamForGrowthJourney = "Automation MT for Growth Journey";
        public const string TeamForMtEtGrowthItems = "Automation Radar Team for GI";
        public const string MultiTeamForMtEtGrowthItems = "Automation MT for MTET GI";
        public const string EnterpriseTeamForMtEtGrowthItems = "Automation ET for MTET GI";
        public const string TeamForSurveyPinAccess = "Automation Team for Survey PIN";
        public const string TeamForNotification = "Automation Team for Notification";
        public const string TeamForGiTab = "Automation Team For GI Tab";
        public const string MultiTeamForGiTab = "Automation Multi Team For GI Tab";
        public const string EnterpriseTeamForGrowthJourney = "Automation ET for Growth Journey";
        public const string TeamForGrowthJourney2 = "Automation Team For Growth Journey 2";
        public const string MultiTeamForGrowthJourney2 = "Automation MT Growth Journey 2";
        public const string EnterpriseTeamForGrowthJourney2 = "Automation ET Growth Journey 2";
        public static string EmailNotFoundPopupMessage(string email) => $"We have emailed the AgilityInsights Facilitator your request to participate in this assessment.Your AgilityInsights Facilitator will contact you shortly at {email} with additional information.";
        public static string EmailFoundPopupMessage(string email) => $"We have emailed you your personalized link for this assessment to {email}. Please check your email for your personalized link and try again.";



        public static readonly List<string> AllTestTeams = new List<string>
        {
            SharedConstants.Team,
            GoiTeam,
            SharedConstants.MultiTeam,
            SharedConstants.EnterpriseTeam,
            SharedConstants.RadarTeam,
            MultiTeamForRadar,
            TeamForMultiTeamRadar,
            MultiTeamForBenchmarking,
            TeamForGiCopyFromPreviousAssessment,
            MultiTeamForGi,
            TeamForBatchAssessment,
            TeamForIndividualAssessmentGi,
            MultiTeamForGrowthJourney,
            SharedConstants.EnterpriseTeamForGrowthJourney,
            TeamForMtEtGrowthItems,
            MultiTeamForMtEtGrowthItems,
            EnterpriseTeamForMtEtGrowthItems,
            TeamForSurveyPinAccess,
            TeamForNotification
        };

        public static readonly List<string> TestTeamsNotApplicableToCompanyAdmin = new List<string>(new[]
        {
            TeamForSurveyPinAccess
        });

        //AT Radar type
        public const string AtTeamHealth2Radar = "AT - TH2 - DO NOT USE";

        //Radar Types
        public static readonly List<string> RadarTypesListForGrowthJourney = new List<string>()
        {
            "Program Health",
            "DevOps Assessment",
            "Technical Health",
            "DevOps Health",
            "AT - TH2 - DO NOT USE",
            "AT - TH2.5 - DO NOT USE"
        };

        //Fill Survey Info
        public static readonly DimensionNote MemberSurveyInfo = new DimensionNote { Dimension = "Clarity", SubDimension = "Planning", Note = "Planning member" };

        //Team Members And Stakeholders Emails/Names
        public static string TeamMemberEmail1 = "ah_automation+at_mem1@agiletransformation.com";
        public const string TeamMemberEmail2 = "ah_automation+at_mem2@agiletransformation.com";
        public const string TeamMemberEmail3 = "ah_automation+at_mem3@agiletransformation.com";
        public const string TeamMemberEmail4 = "ah_automation+at_mem4@agiletransformation.com";
        public const string TeamMemberEmail5 = "ah_automation+at_mem5@agiletransformation.com";
        public const string TeamMemberName1 = "AT_Mem 1";
        public const string TeamMemberName2 = "AT_Mem 2";
        public const string TeamMemberName3 = "AT_Mem 3";
        public const string TeamMemberName4 = "AT_Mem 4";
        public const string TeamMemberName5 = "AT_Mem 5";
        public const string TeamMemberRole1 = "Developer";
        public const string TeamMemberParticipantGroup1 = "Leadership Team";
        public const string TeamMemberParticipantGroup2 = "Technical";

        public const string StakeholderEmail1 = "ah_automation+at_stake1@agiletransformation.com";
        public const string StakeholderEmail2 = "ah_automation+at_stake2@agiletransformation.com";
        public const string StakeholderEmail3 = "ah_automation+at_stake3@agiletransformation.com";
        public const string StakeholderName1 = "AT_Stake 1";
        public const string StakeholderName2 = "AT_Stake 2";
        public const string StakeholderName3 = "AT_Stake 3";
        public const string StakeholderName4 = "AT_Stake 4";
        public const string StakeholderRole1 = "Sponsor";
        public const string StakeholderRole2 = "Sponsor";
        public const string StakeholderRole3 = "Manager";
        public const string StakeholderRole4 = "Sponsor";

        public const string UserEmailPrefix = "ah_automation+user";
        public const string UserEmailDomain = "@agiletransformation.com";

        //Assessment info
        public const string IndividualAssessmentName = "Individual";
        //Radar Types
        public static readonly List<string> AssessmentListForAutomationRadarTeam = new List<string>()
        {
            "TH2Radar(Do Not Delete)",
            "TH2ForGrowthJourney(DoNotRemove)"
        };



        //competencies
        public static readonly List<string> TeamHealth2CompentenciesLableForMember = new List<string>()
                                                                                {
                                                                                    "Effective Facilitation",
                                                                                    "Team Facilitator - Leadership",
                                                                                    "Impediment Mgmt.",
                                                                                    "Technical Lead - Leadership",
                                                                                    "Technical  Expertise",
                                                                                    "Engagement",
                                                                                    "Backlog Mgmt.",
                                                                                    "Product Owner - Leadership",
                                                                                    "Management - Leadership",
                                                                                    "Develop People",
                                                                                    "Process Improvement",
                                                                                    "Happiness",
                                                                                    "Collaboration",
                                                                                    "Trust & Respect",
                                                                                    "Creativity & Innovation",
                                                                                    "Accountability",
                                                                                    "Sustainable Pace",
                                                                                    "Self Organization",
                                                                                    "Technical Excellence",
                                                                                    "Planning & Estimating",
                                                                                    "Effective Meetings",
                                                                                    "Size & Skills",
                                                                                    "Allocation & Stability",
                                                                                    "Workspace",
                                                                                    "Vision & Purpose",
                                                                                    "Goals & Outcomes",
                                                                                    "Quarterly/ Roadmap",
                                                                                    "Monthly/ Release",
                                                                                    "Weekly/ Iteration",
                                                                                    "Roles",
                                                                                    "Generalizing Specialists",
                                                                                    "Team Confidence",
                                                                                    "Predictable Delivery",
                                                                                    "Time to Market",
                                                                                    "Value Delivered",
                                                                                    "Quality",
                                                                                    "Response to Change"
                                                                                };

        public static readonly List<string> TeamHealth2CompentenciesLableForStakeholder = new List<string>()
                                                                                {
                                                                                    "Stakeholder Confidence",
                                                                                };

        public static readonly List<string> ProgramHealthCompentenciesLables = new List<string>()
                                                                            {
                                                                                "Cross-Functional Collaboration",
                                                                                "Empowerment",
                                                                                "Trust & Respect",
                                                                                "Sustainable Pace",
                                                                                "Creativity & Innovation",
                                                                                "Time to Market",
                                                                                "Budget",
                                                                                "Quality",
                                                                                "Predictability",
                                                                                "Cross Team Dep.",
                                                                                "Release Mgmt",
                                                                                "Agile Maturity",
                                                                                "Inspects & Adapts",
                                                                                "Build Infra.",
                                                                                "Automated Testing",
                                                                                "Deployment Infra.",
                                                                                "Experiment Infra.",
                                                                                "Documentation",
                                                                                "Technical Debt",
                                                                                "Technical Dep.",
                                                                                "Technical Roadmap",
                                                                                "Vision Clarity",
                                                                                "Roadmap Clarity",
                                                                                "Backlog Mgmt",
                                                                                "Prioritization",
                                                                                "Planning Event",
                                                                                "Stakeholder Engagement",
                                                                                "Process Ownership",
                                                                                "Risk Mgmt",
                                                                                "Vendor Mgmt.",
                                                                                "Stakeholder Confidence",
                                                                                "Leadership Confidence",
                                                                                "Sponsor Confidence",
                                                                                "Sponsor Engagement",
                                                                                "Roles & Ceremonies",
                                                                                "Impediment Management",
                                                                                "Tracking & Reporting"
                                                                            };

        public static readonly List<string> DevOpsHealthCompentenciesLables = new List<string>()
                                                                            {
                                                                                "Production Metrics",
                                                                                "Application Monitoring",
                                                                                "Work Traceability",
                                                                                "Resolving Issues",
                                                                                "Security Testing",
                                                                                "Automated Testing",
                                                                                "Testable Requirements",
                                                                                "Application Resiliency",
                                                                                "Performance Testing",
                                                                                "Global Sharing",
                                                                                "Collective Ownership",
                                                                                "Process Effectiveness",
                                                                                "X-Functional Teams",
                                                                                "Leadership",
                                                                                "Impediment Mgmt.",
                                                                                "Tech Debt Mgmt.",
                                                                                "Risk Taking",
                                                                                "Feedback Loops",
                                                                                "Learn & Exp.",
                                                                                "Customer Engage",
                                                                                "Lead Times",
                                                                                "Recovery Times",
                                                                                "A/B Testing",
                                                                                "System Thinking",
                                                                                "Unified Backlog",
                                                                                "Value Streams",
                                                                                "Prioritize",
                                                                                "Batch Sizes",
                                                                                "Delivery Pipeline",
                                                                                "Continuous Integration",
                                                                                "Feature Flags",
                                                                                "Staying Shippable",
                                                                                "App Driven Infra.",
                                                                                "Infra. as Code",
                                                                                "Emergent Architecture",
                                                                                "Technical Practices",
                                                                                "Flexibility & Scale"
                                                                            };

        public static readonly List<string> TechnicalHealthCompentenciesLables = new List<string>()
                                                                            {
                                                                                "Vision & Roadmap",
                                                                                "Security",
                                                                                "Visual Artifacts",
                                                                                "Architecture Governance",
                                                                                "Standards & Practices",
                                                                                "Technical CoPs",
                                                                                "Engagement & Collaboration",
                                                                                "Frequent Code Commit",
                                                                                "Collective Ownership",
                                                                                "Code Design Rev.",
                                                                                "Tech. Debt Mgmt.",
                                                                                "BDD/TDD",
                                                                                "Pairing",
                                                                                "Story/Feature Testing",
                                                                                "Test Automation",
                                                                                "Non-Functional Testing",
                                                                                "Usability Testing",
                                                                                "Quality",
                                                                                "Build Health",
                                                                                "Code Coverage",
                                                                                "Deployment Speed",
                                                                                "Confidence",
                                                                                "Hardware Infrastructure",
                                                                                "Monitoring/ Reporting",
                                                                                "Software/Tools",
                                                                                "Staging Environments",
                                                                                "Deployment Automation",
                                                                                "Source Code Mgmt.",
                                                                                "Experiment Infrastructure",
                                                                                "DevOps Culture",
                                                                                "Innovation",
                                                                                "Cross-Functional Collaboration"
                                                                            };

        public static readonly List<string> DevOpsAssessmentCompentenciesLables = new List<string>()
                                                                            {
                                                                                "Clarity",
                                                                                "Trust",
                                                                                "Value",
                                                                                "Focus",
                                                                                "Priority",
                                                                                "Improve",
                                                                                "Risk Taking",
                                                                                "Business Value",
                                                                                "Customers First",
                                                                                "Systems Thinking",
                                                                                "Cross Functional",
                                                                                "Delivery Teams",
                                                                                "Issue Resolution",
                                                                                "Guidance",
                                                                                "Quality",
                                                                                "Develop",
                                                                                "Automation",
                                                                                "Management",
                                                                                "Speed",
                                                                                "Improve",
                                                                                "Priority",
                                                                                "Unified Backlog",
                                                                                "Agility",
                                                                                "Customer Satisfaction",
                                                                                "Efficiency",
                                                                                "Code",
                                                                                "Environments",
                                                                                "Security",
                                                                                "Standards",
                                                                                "Testable",
                                                                                "Automation",
                                                                                "Capability",
                                                                                "Visibility",
                                                                                "Quality",
                                                                                "Source Control",
                                                                                "App Driven",
                                                                                "Infrastructure",
                                                                                "Electronic Tracking",
                                                                                "Traceability",
                                                                                "Visibility",
                                                                                "Applications",
                                                                                "Operations"
                                                                            };
        public static readonly List<string> AtIndividualCompentenciesLabels = new List<string>()
                                                                            {
                                                                                "Individual Influence",
                                                                                "Team Influence",
                                                                                "Program/Product Influence",
                                                                                "Org Influence",
                                                                                "Clarity",
                                                                                "Performance",
                                                                                "Leadership",
                                                                                "Culture",
                                                                                "Foundation",
                                                                                "Continuous Growth",
                                                                                "NPS",
                                                                                "Servant Leader",
                                                                                "Influencer",
                                                                                "Teacher",
                                                                                "Mentor",
                                                                                "Coach",
                                                                                "Facilitator",
                                                                                "Self-Growth",
                                                                                "Self-Awareness",
                                                                                "Community",
                                                                                "Values & Principles",
                                                                                "Agile Methods",
                                                                                "Conflict Facilitation",
                                                                                "Planning & Estimating",
                                                                                "Scaling Agile",
                                                                                "Lean Product Dev.",
                                                                                "Technical Excellence",
                                                                                "Business Acumen"
                                                                            };

        public static List<string> CompetencyLabelForAgileCoachHealth = new List<string>
        {
            "Individual Influence",
            "Team Influence",
            "Program/Product Influence",
            "Org Influence",
            "Clarity",
            "Performance",
            "Leadership",
            "Culture",
            "Foundation",
            "Continuous Growth",
            "NPS",
            "Servant Leader",
            "Influencer",
            "Teacher",
            "Mentor",
            "Coach",
            "Facilitator",
            "Self-Growth",
            "Self-Awareness",
            "Community"
        };

        //User info
        public const string CompanyTimeZone = "UTC";

        public const string AssessmentChecklistSingleItem = "Cool Cool Cool";
        public const string AssessmentChecklistMultiItem = "Maybe";

        //Integrations
        public const string PlatformJiraCloud = "Cloud";
        public const string PlatformJiraDataCenter = "Jira Data Center";

        //Campaign
        public const string AtCampaign = "AT Campaign";

        //Already Completed Survey message
        public const string AlreadySurveyCompletedMessage = "You have already completed the survey!Questions?Email: support@agilityinsights.ai";

        //A Global Financial Company Name
        public const string AGlobalFinancialCompany = "A Global Financial Company";

        //Color of assessment dashboard tabs
        public const string BlueColorHexValue = "#65a1df";

        // Validation Messages for Login page
        public const string EmailValidationMessage = "Error: The Email field is required.";
        public const string PasswordValidationMessage = "Error: The Password field is required.";
        public const string InvalidEmailAndPasswordValidationMessage = "Error: Invalid email address or password";

        //Business Outcome 
        public const string KeyResultUnlockedTooltip = "The key results/KPIs are open for edits.";
        public const string KeyResultLockConfirmationPopupTitle = "Key Result/KPI Lock Confirmation";
        public const string KeyResultLockConfirmationPopupDescription = "Are you sure you want to lock future edits to this card? Only the \"Current\" field and sub-target comments will remain editable.";
        public const string KeyResultLockedTooltip = "The key results/KPIs are locked. Only the \"Current\" field and Sub-target comments are editable.";
        public const string KeyResultUnLockConfirmationPopupTitle = "Key Result/KPI Unlock Confirmation";

        public const string KeyResultUnLockConfirmationPopupDescription =
            "Are you sure you want to unlock editing for this card? Unlocking the key results/KPIs will enable all of the fields to be editable.";

        //Alert message before delete user
        public const string DeleteUserAlertMessage = "Are you sure you want to delete this record?";

        //Non-SSO production Environments

        public static List<string> NonSsoEnvironments = new List<string>()
        {
            "eu","appx","alithya","bcbsla","equifax","cvs","fbi","dtcc", "nngroup", "shell", "epic", "merck", "waters"
        };

        public static List<string> AgilityInsightsSaDomain = new List<string>()
        {
            "srca", "hhc", "rcmc"
        };

        public const string PowerBiUri = "https://app.powerbi.com/";

        public static List<string> ListOfAiSummarizationPrompts = new List<string> {
            "Analyze the growth plan items and provide the top themes.",
            "Analyze the growth plan items and provide insights and patterns.",
            "Based on the team's challenges and improvements shared in the growth plan items, what are the top recommendations to start with?",
            "What frequent words or topics are mentioned by the teams in these growth plan items?"
        };
        public static IEnumerable<object[]> GetProdCompanyNames()
        {
            var companyNames = new string[]
            {
                "7-Eleven", "Alithya", "Ameriprise", "CVS Health", "NASA", "NTT (Internal Use)",
                "Prudential", "Raytheon", "BCBSLA", "Equifax", "Bloomberg", "Cardinal Health",
                "Cummins", "Erie Insurance Group", "Insperity", "Northern Trust", "AstraZeneca",
                "IQbusiness (EU)", "EPiC Agile", "FBI", "DTCC", "Petronas", "Schneider Electric",
                "Australian Taxation Office", "BCI", "Blue Cross Blue Shield of MA", "Cognizant",
                "Focus on the Family", "Geico", "Honeywell", "IGM Wealth Management", "IQBusiness",
                "New York Life", "Norfolk Southern", "Prudential Financial, Incorporated - PGIM",
                "Scotiabank", "Society Insurance", "Starbucks", "Union Bank & Trust",
                "CDW (Atkco)", "Eliassen Group", "SendoAgil", "Australian National University",
                "Wellmark BCBS", "Amtrak", "Amway", "Citi", "Edward Jones", "Accenture",
                "Federal Reserve", "Merck", "Shell", "Waters Corp", "Paychex", "USAF",
                "Flexential", "AgilityHealth (not app)", "Citizens Property Insurance Corporation", "Quad"
            };

            return companyNames.Select(name => new object[] { name }).ToList();
        }


        public static IEnumerable<object[]> KeyCustomerCompanyNames()
        {
            var companyNames = new string[]
            {
                "7eleven", "alithya", "amtrak", "amway", "ameriprise", "app", "appx", "astrazeneca", "atos", "bcbsla", "bloomberg", "citi", "citizensfla", "cognizant", "cvs",
                "dtcc", "edwardjones", "eliassen", "epic", "equifax", "erieinsurance", "eu", "fbi", "federalreserve", "insperity", "iqbusiness", "northerntrust", "ntt",
                "petronas", "prudential", "projectandteam", "quad", "sai", "shell", "solutionsiq", "srca", "wellmark", "nasa", "usaf"
            };

            return companyNames.Select(name => new object[] { name }).ToList();
        }

        public static IEnumerable<object[]> TopVvipCustomersCompanyNames()
        {
            var companyNames = new string[]
            {
               "eu", "federalreserve", "appx", "citi", "amtrak", "app", "edwardjones", "prudential", "bloomberg", "quad", "7eleven", "nasa"
            };

            return companyNames.Select(name => new object[] { name }).ToList();
        }
    }
}
