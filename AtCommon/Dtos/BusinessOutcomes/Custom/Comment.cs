using System;

namespace AtCommon.Dtos.BusinessOutcomes.Custom
{
    public class Comment
    {
        public Guid ItemId { get; set; }
        public string Content { get; set; }
        public string Commenter { get; set; }
        public DateTime CommentDate { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string Owner { get; set; }
        public Guid Uid { get; set; }
    }
}