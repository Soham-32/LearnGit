using AtCommon.Dtos.Radars.Custom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtCommon.Utilities;
using System;
using AtCommon.Api;
using Newtonsoft.Json.Linq;

namespace AtCommon.ObjectFactories
{
    public static class ManageRadarFactory
    {
        private static readonly LanguageTranslations DefaultLanguagesMessages = GetMessagesByLanguage("English");
        private static readonly LanguageTranslations TranslatedLanguagesMessages = GetMessagesByLanguage(SelectTranslatedLanguage());

        private const string SortOrder = "1";
        private const string RadarOrder = "0";
        private const string Font = "Arial";
        private const string FontSize = "28";
        private const string LetterSpacing = "4";
        private const string Direction = "Inward";
        private const string FilterTag = "Exclude From";
        private const string Company = "Automation_M (DO NOT USE)";

        private static readonly string Dimension1 = "Dimension" + RandomDataUtil.GetDimensionNote();
        private static readonly string Dimension2 = "Dimension" + RandomDataUtil.GetDimensionNote();
        private static readonly string Dimension3 = "Dimension" + RandomDataUtil.GetDimensionNote();
        private static readonly string Dimension4 = "Dimension" + RandomDataUtil.GetDimensionNote();
        private static readonly string Dimension5 = "Dimension" + RandomDataUtil.GetDimensionNote();

        private static readonly string SubDimension1 = "SubDimension" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string SubDimension2 = "SubDimension" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string SubDimension3 = "SubDimension" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string SubDimension4 = "SubDimension" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string SubDimension5 = "SubDimension" + RandomDataUtil.GetSubdimensionNote();

        private static readonly string Competency1 = "Competency" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string Competency2 = "Competency" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string Competency3 = "Competency" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string Competency4 = "Competency" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string Competency5 = "Competency" + RandomDataUtil.GetSubdimensionNote();

        private static readonly string OpenEndedQuestion1 = "OpenEndedQuestion" + RandomDataUtil.GetSubdimensionNote();
        private static readonly string OpenEndedQuestion2 = "OpenEndedQuestion" + RandomDataUtil.GetSubdimensionNote();

        // Dimension and SubDimension Color
        private static readonly Random Random = new Random();
        private static readonly string DimensionColor = CSharpHelpers.GetRandomColor(Random);
        private static readonly string SubDimensionColor = CSharpHelpers.GetRandomColor(Random);

        public static JObject Json { get; set; }
        public static string SelectTranslatedLanguage(List<string> excludedLanguages = null)
        {
            var randomVariable = new Random();
            var allLanguages = Languages();
            if (excludedLanguages != null)
            {
                foreach (var language in excludedLanguages)
                {
                    allLanguages.Remove(language);
                }
            }
                        
            var randomLanguageCount = randomVariable.Next(allLanguages.Count);
            return allLanguages[randomLanguageCount];
        }   

        public static LanguageTranslations GetMessagesByLanguage(string languageName)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/TestData/Radar/RadarTranslations.json";
            var text = File.ReadAllText(filePath);
            Json = JObject.Parse(text);

            var languageData = (Json["Languages"])!
                .FirstOrDefault(language => language["language"].ToString() == languageName)
                .CheckForNull($"No language found in the file with description <{languageName}>");

            return new LanguageTranslations()
            {
                Language = languageData["language"]?.ToString(),
                TranslatedAssessmentWelcomeMessage = languageData["translatedAssessmentWelcomeMessage"]?.ToString(),
                TranslatedEmailWelcomeMessage = languageData["translatedEmailWelcomeMessage"]?.ToString(),
                TranslatedThankYouMessage = languageData["translatedThankYouMessage"]?.ToString(),
                TranslatedEmailMessageSubject = languageData["translatedEmailMessageSubject"]?.ToString()
            };
        }

        public static List<Dimension> GetValidTranslatedRadarDetails(string language)
        {
            var parameterResponses = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\Resources\TestData\Radar\RadarParametersTranslations.json").DeserializeJsonObject<ParameterTranslations>();
              var translatedDimensionResponses = parameterResponses.Languages.FirstOrDefault(x => x.Language == language);

            var firstDimension = translatedDimensionResponses?.Dimension[0];
            var secondDimension = translatedDimensionResponses?.Dimension[1];
            var thirdDimension = translatedDimensionResponses?.Dimension[2];

            return new List<Dimension>(){firstDimension,secondDimension,thirdDimension};
        }

        public static RadarDetails GetValidRadarDetails()
        {
            return new RadarDetails
            {
                CompanyName = SharedConstants.CompanyName,
                Language = TranslatedLanguagesMessages.Language,
                Type = RadarTypes().FirstOrDefault(),
                Name = "AT-Radar"+ RandomDataUtil.GetCompanyCity(),
                Scale = Scale().LastOrDefault(),
                ShowAsAbsolute = true,
                Public = false,
                Show1Response = false,
                IncludeInGlobalBenchmarking = false,
                Active = false,
                TranslatedActive = false,
                Logo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"),
                LimitedTo =new List<string>(){"Automation2 (DO NOT USE)"},
                AvailableTo = new List<string>() { AvailableTo().LastOrDefault() },
                WorkType = new List<string>() { WorkTypes().LastOrDefault() },
                AppendStandardFooter = true,
                CopyrightText = "Copyright © Agile Transformation Inc. | Proprietary and Confidential | www.agilityinsights.ai",
                AssessmentWelcomeMessage = DefaultLanguagesMessages.TranslatedAssessmentWelcomeMessage,
                EmailWelcomeMessage = DefaultLanguagesMessages.TranslatedEmailWelcomeMessage,
                ThankYouMessage = DefaultLanguagesMessages.TranslatedThankYouMessage,
                EmailMessageSenderName = "AgilityHealth",
                EmailMessageSubject = DefaultLanguagesMessages.TranslatedEmailMessageSubject,
                TranslatedAssessmentWelcomeMessage = TranslatedLanguagesMessages.TranslatedAssessmentWelcomeMessage,
                TranslatedEmailWelcomeMessage = TranslatedLanguagesMessages.TranslatedEmailWelcomeMessage,
                TranslatedThankYouMessage = TranslatedLanguagesMessages.TranslatedThankYouMessage,
                TranslatedEmailMessageSubject = TranslatedLanguagesMessages.TranslatedEmailMessageSubject,
                RadarDimensions = new List<RadarDimensionResponse>()
                {
                    new RadarDimensionResponse()
                    {
                        Name = Dimension1,
                        Color = DimensionColor,
                        SortOrder = SortOrder,
                        RadarOrder = RadarOrder,
                        Font = Font,
                        FontSize = FontSize,
                        LetterSpacing = LetterSpacing,
                        Direction = Direction,
                        RadarSubDimensions = new List<RadarSubDimensionResponse>()
                        {
                            new RadarSubDimensionResponse()
                            {
                                Dimension =  Dimension1,
                                Name = SubDimension1,
                                Color = SubDimensionColor,
                                Abbreviation = "Sub Dimension Abbreviation Text",
                                Description = "Description here",
                                Direction = Direction,
                                RadarOrder = RadarOrder,
                                Font = Font,
                                FontSize = FontSize,
                                LetterSpacing = LetterSpacing,
                                RadarCompetencies = new List<RadarCompetencyResponse>()
                                {
                                    new RadarCompetencyResponse()
                                    {
                                        Dimension = Dimension1,
                                        SubDimension = SubDimension1,
                                        Name = Competency1,
                                        Abbreviation = "Competency Abbreviation Text",
                                        AnalyticsAbbreviation = "Competency Analytics Abbreviation Text",
                                        Exclude = "Exclude Text",
                                        Direction = Direction,
                                        RadarOrder = RadarOrder,
                                        Font = Font,
                                        FontSize = FontSize,
                                        LetterSpacing = LetterSpacing,
                                        RadarQuestions = new List<RadarQuestionResponse>()
                                        {
                                            new RadarQuestionResponse()
                                            {
                                                Dimension = Dimension1,
                                                SubDimension = SubDimension1,
                                                Competency = Competency1,
                                                ScaleOverride = "9",
                                                QuantitativeMetric = true,
                                                QuestionText = $"Question text here for {Competency1}",
                                                QuestionHelp = $"Question Help here {Competency1}",
                                                WorkTypeFilter = FilterTag,
                                                WorkType = "Kiosk",
                                                MethodologyFilter = FilterTag,
                                                Methodology = "Other",
                                                CompanyFilter = FilterTag,
                                                Company = Company,
                                                QuestionTags = "Team",
                                                ExcludeRoles = "Product Owner",
                                                ParticipantTagsFilter = FilterTag,
                                                ParticipantTags = "FTE",
                                                RadarOpenEnded = new List<RadarOpenEndedResponse>
                                                {
                                                    new RadarOpenEndedResponse
                                                    {
                                                        OpenQuestions = "Achievements",
                                                        Text = OpenEndedQuestion1,
                                                        Order = "1",
                                                        Exclude = "Exclude Text",
                                                        CompanyFilter = FilterTag,
                                                        Company = Company,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new RadarDimensionResponse()
                    {
                        Name = Dimension2,
                        Color = DimensionColor,
                        SortOrder = SortOrder,
                        RadarOrder = RadarOrder,
                        Font = Font,
                        FontSize = FontSize,
                        LetterSpacing = LetterSpacing,
                        Direction = Direction,
                        RadarSubDimensions = new List<RadarSubDimensionResponse>()
                        {
                            new RadarSubDimensionResponse()
                            {
                                Dimension =  Dimension2,
                                Name = SubDimension2,
                                Color = SubDimensionColor,
                                Abbreviation = "Sub Dimension Abbreviation Text",
                                Description = "Description here",
                                Direction = Direction,
                                RadarOrder = RadarOrder,
                                Font = Font,
                                FontSize = FontSize,
                                LetterSpacing = LetterSpacing,
                                RadarCompetencies = new List<RadarCompetencyResponse>()
                                {
                                    new RadarCompetencyResponse()
                                    {
                                        Dimension = Dimension2,
                                        SubDimension = SubDimension2,
                                        Name = Competency2,
                                        Abbreviation = "Competency Abbreviation Text",
                                        AnalyticsAbbreviation = "Competency Analytics Abbreviation Text",
                                        Exclude = "Exclude Text",
                                        Direction = Direction,
                                        RadarOrder = RadarOrder,
                                        Font = Font,
                                        FontSize = FontSize,
                                        LetterSpacing = LetterSpacing,
                                        RadarQuestions = new List<RadarQuestionResponse>()
                                        {
                                            new RadarQuestionResponse()
                                            {
                                                Dimension = Dimension2,
                                                SubDimension = SubDimension2,
                                                Competency = Competency2,
                                                ScaleOverride = "9",
                                                QuantitativeMetric = true,
                                                QuestionText = $"Question text here for {Competency2}",
                                                QuestionHelp = $"Question Help here {Competency2}",
                                                WorkTypeFilter = FilterTag,
                                                WorkType = "Kiosk",
                                                MethodologyFilter = FilterTag,
                                                Methodology = "Other",
                                                CompanyFilter = FilterTag,
                                                Company = Company,
                                                QuestionTags = "Team",
                                                ExcludeRoles = "Product Owner",
                                                ParticipantTagsFilter = FilterTag,
                                                ParticipantTags = "FTE",
                                                RadarOpenEnded = new List<RadarOpenEndedResponse>
                                                {
                                                    new RadarOpenEndedResponse
                                                    {
                                                        OpenQuestions = "Achievements",
                                                        Text = OpenEndedQuestion2,
                                                        Order = "1",
                                                        Exclude = "Exclude Text",
                                                        CompanyFilter = FilterTag,
                                                        Company = Company,
                                                    }
                                                }
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new RadarDimensionResponse()
                    {
                        Name = Dimension3,
                        Color = DimensionColor,
                        SortOrder = SortOrder,
                        RadarOrder = RadarOrder,
                        Font = Font,
                        FontSize = FontSize,
                        LetterSpacing = LetterSpacing,
                        Direction = Direction,
                        RadarSubDimensions = new List<RadarSubDimensionResponse>()
                        {
                            new RadarSubDimensionResponse()
                            {
                                Dimension =  Dimension3,
                                Name = SubDimension3,
                                Color = SubDimensionColor,
                                Abbreviation = "Sub Dimension Abbreviation Text",
                                Description = "Description here",
                                Direction = Direction,
                                RadarOrder = RadarOrder,
                                Font = Font,
                                FontSize = FontSize,
                                LetterSpacing = LetterSpacing,
                                RadarCompetencies = new List<RadarCompetencyResponse>()
                                {
                                    new RadarCompetencyResponse()
                                    {
                                        Dimension = Dimension3,
                                        SubDimension = SubDimension3,
                                        Name = Competency3,
                                        Abbreviation = "Competency Abbreviation Text",
                                        AnalyticsAbbreviation = "Competency Analytics Abbreviation Text",
                                        Exclude = "Exclude Text",
                                        Direction = Direction,
                                        RadarOrder = RadarOrder,
                                        Font = Font,
                                        FontSize = FontSize,
                                        LetterSpacing = LetterSpacing,
                                        RadarQuestions = new List<RadarQuestionResponse>()
                                        {
                                            new RadarQuestionResponse()
                                            {
                                                Dimension = Dimension3,
                                                SubDimension = SubDimension3,
                                                Competency = Competency3,
                                                ScaleOverride = "9",
                                                QuantitativeMetric = true,
                                                QuestionText = $"Question text here for {Competency3}",
                                                QuestionHelp = $"Question Help here {Competency3}",
                                                WorkTypeFilter = FilterTag,
                                                WorkType = "Kiosk",
                                                MethodologyFilter = FilterTag,
                                                Methodology = "Other",
                                                CompanyFilter = FilterTag,
                                                Company = Company,
                                                QuestionTags = "Team",
                                                ExcludeRoles = "Product Owner",
                                                ParticipantTagsFilter = FilterTag,
                                                ParticipantTags = "FTE",
                                                RadarOpenEnded = new List<RadarOpenEndedResponse>()
                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new RadarDimensionResponse()
                    {
                        Name = Dimension4,
                        Color = DimensionColor,
                        SortOrder = SortOrder,
                        RadarOrder = RadarOrder,
                        Font = Font,
                        FontSize = FontSize,
                        LetterSpacing = LetterSpacing,
                        Direction = Direction,
                        RadarSubDimensions = new List<RadarSubDimensionResponse>()
                        {
                            new RadarSubDimensionResponse()
                            {
                                Dimension =  Dimension4,
                                Name = SubDimension4,
                                Color = SubDimensionColor,
                                Abbreviation = "Sub Dimension Abbreviation Text",
                                Description = "Description here",
                                Direction = Direction,
                                RadarOrder = RadarOrder,
                                Font = Font,
                                FontSize = FontSize,
                                LetterSpacing = LetterSpacing,
                                RadarCompetencies = new List<RadarCompetencyResponse>()
                                {
                                    new RadarCompetencyResponse()
                                    {
                                        Dimension = Dimension4,
                                        SubDimension = SubDimension4,
                                        Name = Competency4,
                                        Abbreviation = "Competency Abbreviation Text",
                                        AnalyticsAbbreviation = "Competency Analytics Abbreviation Text",
                                        Exclude = "Exclude Text",
                                        Direction = Direction,
                                        RadarOrder = RadarOrder,
                                        Font = Font,
                                        FontSize = FontSize,
                                        LetterSpacing = LetterSpacing,
                                        RadarQuestions = new List<RadarQuestionResponse>()
                                        {
                                            new RadarQuestionResponse()
                                            {
                                                Dimension = Dimension4,
                                                SubDimension = SubDimension4,
                                                Competency = Competency4,
                                                ScaleOverride = "9",
                                                QuantitativeMetric = true,
                                                QuestionText = $"Question text here for {Competency4}",
                                                QuestionHelp = $"Question Help here {Competency4}",
                                                WorkTypeFilter = FilterTag,
                                                WorkType = "Kiosk",
                                                MethodologyFilter = FilterTag,
                                                Methodology = "Other",
                                                CompanyFilter = FilterTag,
                                                Company = Company,
                                                QuestionTags = "Team",
                                                ExcludeRoles = "Product Owner",
                                                ParticipantTagsFilter = FilterTag,
                                                ParticipantTags = "FTE",
                                                RadarOpenEnded = new List<RadarOpenEndedResponse>()

                                            }
                                        }
                                    },
                                }
                            },
                        }
                    },
                    new RadarDimensionResponse()
                    {
                        Name = Dimension5,
                        Color = DimensionColor,
                        SortOrder = SortOrder,
                        RadarOrder = RadarOrder,
                        Font = Font,
                        FontSize = FontSize,
                        LetterSpacing = LetterSpacing,
                        Direction = Direction,
                        RadarSubDimensions = new List<RadarSubDimensionResponse>()
                        {
                            new RadarSubDimensionResponse()
                            {
                                Dimension =  Dimension5,
                                Name = SubDimension5,
                                Color = SubDimensionColor,
                                Abbreviation = "Sub Dimension Abbreviation Text",
                                Description = "Description here",
                                Direction = Direction,
                                RadarOrder = RadarOrder,
                                Font = Font,
                                FontSize = FontSize,
                                LetterSpacing = LetterSpacing,
                                RadarCompetencies = new List<RadarCompetencyResponse>()
                                {
                                    new RadarCompetencyResponse()
                                    {
                                        Dimension = Dimension5,
                                        SubDimension = SubDimension5,
                                        Name = Competency5,
                                        Abbreviation = "Competency Abbreviation Text",
                                        AnalyticsAbbreviation = "Competency Analytics Abbreviation Text",
                                        Exclude = "Exclude Text",
                                        Direction = Direction,
                                        RadarOrder = RadarOrder,
                                        Font = Font,
                                        FontSize = FontSize,
                                        LetterSpacing = LetterSpacing,
                                        RadarQuestions = new List<RadarQuestionResponse>()
                                        {
                                            new RadarQuestionResponse()
                                            {
                                                Dimension = Dimension5,
                                                SubDimension = SubDimension5,
                                                Competency = Competency5,
                                                ScaleOverride = "9",
                                                QuantitativeMetric = true,
                                                QuestionText = $"Question text here for {Competency5}",
                                                QuestionHelp = $"Question Help here {Competency5}",
                                                WorkTypeFilter = FilterTag,
                                                WorkType = "Kiosk",
                                                MethodologyFilter = FilterTag,
                                                Methodology = "Other",
                                                CompanyFilter = FilterTag,
                                                Company = Company,
                                                QuestionTags = "Team",
                                                ExcludeRoles = "Product Owner",
                                                ParticipantTagsFilter = FilterTag,
                                                ParticipantTags = "FTE",
                                                RadarOpenEnded = new List<RadarOpenEndedResponse>()

                                            }
                                        }
                                    },
                                }
                            },
                        }
                    }
                },

            };
        }

        public static List<string> RadarTypes()
        {
            return new List<string>()
            {
                "Team",
                "Multi-Team",
                "Individual",
                "Organizational",
                "Facilitator"
            };
        }

        public static List<string> WorkTypes()
        {
            return new List<string>()
            {
                "Software Delivery",
                "Business Operations",
                "Service and Support",
                "Group Of Individuals",
                "SAFe - Release Management",
                "Feature Team",
                "Kiosk",
                "Transformation"
            };
        }

        public static List<string> AvailableTo()
        {
            return new List<string>()
            {
                "Company Admin",
                "Organizational Leader",
                "Business Line Admin",
                "Team Admin"
            };
        }

        public static List<string> Scale()
        {
            return new List<string>()
            {
                "2", "3", "4", "5", "6", "7", "8", "9", "10"
            };
        }

        public static List<string> Languages()
        {
            return new List<string>()
            {
                "Default",
                "Arabic",
                "Chinese",
                "English",
                "French",
                "German",
                "Hungarian",
                "Japanese",
                "Korean",
                "Polish",
                "Portuguese",
                "Spanish",
                "Turkish"
            };
        }
    }
}