namespace AtCommon.Dtos.Radars
{
    public class TagInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? MasterTagId { get; set; }
    }
}
