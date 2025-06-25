using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Assessments
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public class SurveySubmit
    {
        public List<SurveyResponse> Results { get; set; }
        public List<string> Answers { get; set; }
        public List<string> Notes { get; set; }
        public String KioskId { get; set; }
    }
}
