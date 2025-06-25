using System;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CommentsResponse : BaseBusinessOutcomesDto
    {
        public Guid? ReplyId { get; set; }
        public Guid ItemId { get; set; }
        public string Content { get; set; }
        public string Commenter { get; set; }
        public DateTime CommentDate { get; set; }
        public string Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}