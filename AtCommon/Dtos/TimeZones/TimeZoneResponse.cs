
namespace AtCommon.Dtos.TimeZones
{
    public class TimeZoneResponse
    {
        public string Id { get; set; }
        public string StandardName { get; set; }
        public string DisplayName { get; set; }
        public string DaylightName { get; set; }
        public bool SupportsDaylightSavingTime { get; set; }
    }
}
