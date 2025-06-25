using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class CommentRequest : BaseBusinessOutcomesDto
    {
        public Guid? ReplyId { get; set; }
        public Guid ItemId { get; set; }
        public string Content { get; set; }
        public string Commenter { get; set; }
        public string Avatar { get; set; }
        public DateTime? CommentDate { get; set; }
        public IEnumerable<string> MentionedUserIds { get; set; } = new List<string>();

        public bool IsNew { get; set; }
        public void SetAvatar(string avatar)
        {
            this.Avatar = avatar;
        }
    }
}