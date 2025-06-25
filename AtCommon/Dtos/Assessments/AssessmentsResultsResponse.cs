using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments
{
    public class AssessmentsResultsResponse
    {
        public class Dimension
        {
            public string Name { get; set; }
            public List<string> Note { get; set; }
            public List<SubDimension> SubDimensions { get; set; }
        }
        public class SubDimension
        {
            public string Name { get; set; }
            public List<string> Note { get; set; }
            public List<Competency> Competencies { get; set; }
        }
        public class Competency
        {
            public string Name { get; set; }
            public double AverageValue { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public List<Contact> Contacts { get; set; }

        }
        public class Contact
        {
            public int ContactId { get; set; }
            public double AverageValue { get; set; }
            public List<Question> Questions { get; set; }

        }
        public class Question
        {
            public int QuestionId { get; set; }
            public double Value { get; set; }
        }

        public int TeamId { get; set; }
        public Guid UId { get; set; }
        public string Name { get; set; }
        public List<Dimension> Dimensions { get; set; }
    }
}
