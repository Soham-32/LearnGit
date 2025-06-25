namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeAttachmentRequest
    {
        public int Id{ get; set; }
        public string FileName { get; set; }
        public string LinkUrl { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLink { get; set; }
    }


}
