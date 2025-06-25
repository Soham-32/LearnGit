using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeAttachmentResponse
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLink { get; set; }
        public string LinkUrl { get; set; }
    }


}