using Newtonsoft.Json;
using System.Collections.Generic;

namespace AtCommon.Dtos.Radars.Custom
{

    public class SurveyTranslations
    {
        public IList<Languages> Languages { get; set; }
    }
    public class Languages
    {
        public string Language { get; set; }
        public string Welcome { get; set; }
        public string Finish { get; set; }
        public string StartButton { get; set; }
        public string DimensionName { get; set; }
        public string SubDimensionName { get; set; }
        public string SubDimensionDescription { get; set; }
        public string CompetencyName { get; set; }
        public string CompetencyTooltipMessage { get; set; }
        public string Question { get; set; }

        [JsonProperty("N/A")]
        public string NA { get; set; }
        public string ProgressAnswer { get; set; }
        public string FinishButton { get; set; }
    }

}