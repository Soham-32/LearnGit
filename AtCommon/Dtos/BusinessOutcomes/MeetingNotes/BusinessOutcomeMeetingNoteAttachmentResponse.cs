using System;

namespace AtCommon.Dtos.BusinessOutcomes.MeetingNotes
{
    public class BusinessOutcomeMeetingNoteAttachmentResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLink { get; set; }
        public string LinkUrl { get; set; }
    }
}
