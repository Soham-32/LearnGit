namespace AtCommon.Dtos.Integrations.Custom
{
    public class Iteration
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CommittedPoints { get; set; }
        public string CompletedPoints { get; set; }
        public string Defects { get; set; }
        public string TotalScope { get; set; }
    }
}