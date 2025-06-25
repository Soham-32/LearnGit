using System;

namespace AtCommon.Dtos.BusinessOutcomes.MeetingNotes
{
    public class FileContentResult
    {
        public byte[] FileContents { get; set; }

        public string ContentType { get; set; }

        public string FileDownloadName { get; set; }

        public DateTime LastModified { get; set; }

        public EntityTagHeaderValueWrapper EntityTag { get; set; }

        public bool EnableRangeProcessing { get; set; }
    }

    public class EntityTagHeaderValueWrapper
    {
        public StringSegmentWrapper Tag { get; set; }

        public bool IsWeak { get; set; }

        public bool ReadOnly { get; set; }
    }

    public class StringSegmentWrapper
    {
        public string Value { get; set; }
    }
}
