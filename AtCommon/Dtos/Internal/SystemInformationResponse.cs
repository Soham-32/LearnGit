namespace AtCommon.Dtos.Internal
{
    public class SystemInformationResponse
    {
        public PendoInformationResponse PendoInformation { get; set; }
        public SplitUserInformationResponse SplitUserInformation { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
    }
}
