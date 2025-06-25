using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes.MeetingNotes
{
    public class BusinessOutcomesMeetingNotesRequest
    {
        public int CompanyId { get; set; }
        public int TeamId { get; set; }
        public bool SendEmail { get; set; }
        public string RedirectUri { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string DecisionsDescription { get; set; }
        public string AuthorName { get; set; }
        public bool IsPrivate { get; set; }
        public int MeetingNoteType { get; set; }
        public DateTime ScheduledAt { get; set; }
        public List<MemberUser> MemberUsers { get; set; }
        public List<ActionItem> ActionItems { get; set; }
        public List<Attachment> Attachments { get; set; }

        public class ActionItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string OwnerId { get; set; }
            public int MeetingNoteId { get; set; }
            public DateTime DueBy { get; set; }
            public bool IsCompleted { get; set; }
        }

        public class Attachment
        {
            public int MeetingNoteAttachmentId { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsLink { get; set; }
            public string LinkTitle { get; set; }
            public string LinkUrl { get; set; }
            public string AuthorName { get; set; }
        }

        public class MemberUser
        {
            public string MemberId { get; set; }
        }
    }
}