namespace AtCommon.Dtos.Integrations.Custom
{
    public class Release
    {
        public string Name { get; set; }
        public string TargetDate { get; set; }
        public string ActualDate { get; set; }
        public string TargetPoints { get; set; }
        public string ActualPoints { get; set; }
        public string Defects { get; set; }
        public string TotalScope { get; set; }
    }
}
