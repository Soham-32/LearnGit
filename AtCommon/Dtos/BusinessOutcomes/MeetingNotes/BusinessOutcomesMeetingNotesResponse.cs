using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.BusinessOutcomes.MeetingNotes
{
    public class BusinessOutcomesMeetingNotesResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DecisionsDescription { get; set; }
        public string AuthorName { get; set; }
        public string EditorName { get; set; }
        public string CreatedBy { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public bool IsPrivate { get; set; }
        public int MeetingNoteType { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool MeetingNoteAuthor { get; set; }
        public bool HasSeen { get; set; }
        public bool Deleted { get; set; }

        public List<MemberUser> MemberUsers { get; set; }
        public List<ActionItem> ActionItems { get; set; }
        public List<Attachment> Attachments { get; set; }

        public class MemberUser
        {
            public int Id { get; set; }
            public string MemberUserId { get; set; }
            public string Name { get; set; }
            public string Avatar { get; set; }
            public string Email { get; set; }
        }

        public class ActionItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string OwnerUserId { get; set; }
            public string OwnerUserName { get; set; }
            public int MeetingNoteId { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsActionItemAuthor { get; set; }
            public DateTime DueDateBy { get; set; }
            public bool IsCompleted { get; set; }
        }

        public class Attachment
        {
            public int Id { get; set; }
            public string Filename { get; set; }
            public string FileExtension { get; set; }
            public int FileSize { get; set; }
            public string AuthorName { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsLink { get; set; }
            public string LinkUrl { get; set; }
        }
    }
}
