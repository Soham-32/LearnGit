using AtCommon.Dtos;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using System;
using System.Collections.Generic;

namespace AtCommon.ObjectFactories.BusinessOutcomes.MeetingNotes
{
    public static class BusinessOutcomesMeetingNotesFactory
    {
        public static BusinessOutcomesMeetingNotesRequest GetValidMeetingNotesRequest(int companyId, int teamId, User user)
        {
            return new BusinessOutcomesMeetingNotesRequest
            {
                CompanyId = companyId,
                TeamId = teamId,
                SendEmail = true,
                RedirectUri = "https://demo.agilityhealth.com/redirect",
                Id = 0,
                Title = RandomDataUtil.GetBusinessOutcomeTitle(),
                DecisionsDescription = RandomDataUtil.GetTeamDescription(),
                AuthorName = user.FullName,
                IsPrivate = false,
                MeetingNoteType = 1,
                ScheduledAt = DateTime.Now,

                MemberUsers = new List<BusinessOutcomesMeetingNotesRequest.MemberUser>
                {
                    new BusinessOutcomesMeetingNotesRequest.MemberUser
                    {
                        MemberId = "04b3893e-f884-4cd7-8966-afcfcb0f8564"
                    }
                },

                ActionItems = new List<BusinessOutcomesMeetingNotesRequest.ActionItem>
                {
                    new BusinessOutcomesMeetingNotesRequest.ActionItem
                    {
                        Id = 0,
                        Description = RandomDataUtil.GetTeamDescription(),
                        OwnerId = "04b3893e-f884-4cd7-8966-afcfcb0f8564",
                        MeetingNoteId = 0,
                        DueBy = DateTime.UtcNow.AddDays(7),
                        IsCompleted = false
                    }
                },

                Attachments = new List<BusinessOutcomesMeetingNotesRequest.Attachment>
                {
                    new BusinessOutcomesMeetingNotesRequest.Attachment
                    {
                        MeetingNoteAttachmentId = 0,
                        IsDeleted = false,
                        IsLink = true,
                        LinkTitle = RandomDataUtil.GetTeamDescription(),
                        LinkUrl = null,
                        AuthorName = user.FullName
                    }
                }
            };
        }

        public static BusinessOutcomesMeetingNotesRequest GetValidUpdatedMeetingNotesRequest(int companyId, int teamId, User user)
        {
            return new BusinessOutcomesMeetingNotesRequest
            {
                CompanyId = companyId,
                TeamId = teamId,
                SendEmail = true,
                RedirectUri = "https://demo.agilityhealth.com/redirect",
                Id = 0,
                Title = "Updated" + RandomDataUtil.GetBusinessOutcomeTitle(),
                DecisionsDescription = "Updated" + RandomDataUtil.GetTeamDescription(),
                AuthorName = user.FullName,
                IsPrivate = false,
                MeetingNoteType = 1,
                ScheduledAt = DateTime.Now.AddDays(2),

                MemberUsers = new List<BusinessOutcomesMeetingNotesRequest.MemberUser>
                {
                    new BusinessOutcomesMeetingNotesRequest.MemberUser
                    {
                        MemberId = "04b3893e-f884-4cd7-8966-afcfcb0f8564"//
                    }
                },

                ActionItems = new List<BusinessOutcomesMeetingNotesRequest.ActionItem>
                {
                    new BusinessOutcomesMeetingNotesRequest.ActionItem
                    {
                        Id = 0,
                        Description = "updated" + RandomDataUtil.GetTeamDescription(),
                        OwnerId = "04b3893e-f884-4cd7-8966-afcfcb0f8564",
                        MeetingNoteId = 0,
                        DueBy = DateTime.UtcNow.AddDays(10),
                        IsCompleted = false
                    }
                },

                Attachments = new List<BusinessOutcomesMeetingNotesRequest.Attachment>
                {
                    new BusinessOutcomesMeetingNotesRequest.Attachment
                    {
                        MeetingNoteAttachmentId = 0,
                        IsDeleted = false,
                        IsLink = true,
                        LinkTitle = "Updated" + RandomDataUtil.GetBusinessOutcomeTitle(),
                        LinkUrl = null,
                        AuthorName = user.FullName
                    }
                }
            };
        }
    }
}




