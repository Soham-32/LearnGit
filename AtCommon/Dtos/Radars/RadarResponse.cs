namespace AtCommon.Dtos.Radars
{
    public class RadarResponse
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OriginalVersion { get; set; }
        public int Order { get; set; }
        public string SurveyType { get; set; }
        public bool? IsDefault { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
