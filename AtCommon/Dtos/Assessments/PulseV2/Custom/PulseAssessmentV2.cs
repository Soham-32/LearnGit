using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments.PulseV2.Custom
{
    public class PulseAssessmentV2
    {
        public string Name { get; set; }
        public string AssessmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
        public RepeatIntervals RepeatInterval { get; set; }
        public int NumberOfOccurrences { get; set; }
        public List<QuestionDetails> Questions { get; set; }
        public int NumberOfSelectedQuestions { get; set; }
        public List<string> MemberList { get; set; }
        public bool IsDraft { get; set; }
    }

    public class QuestionDetails
    {
        public string DimensionName { get; set; }
        public string SubDimensionName { get; set; }
        public string CompetencyName { get; set; }
        public QuestionSelectionPreferences QuestionSelectionPref { get; set; }
    }

    public enum QuestionSelectionPreferences
    {
        All = 0,
        Dimension = 1,
        SubDimension = 2,
        Competency = 3
    }

    public class RepeatIntervals
    {
        public string Type { get; set; }
        public End Ends { get; set; }
    }

    public enum End
    {
        Never = 1,
        OnDate = 2,
        AfterOccurrences = 3,
        None = 4
    }
}