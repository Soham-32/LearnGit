using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.CampaignsV2
{
    public class FacilitatorInfoResponse
    {
        public List<FacilitatorInfo> FacilitatorInfo { get; set; }
        public Paging Paging { get; set; }
    }

    public class AdjustmentRule
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string DaylightDelta { get; set; }
        public DaylightTransitionStart DaylightTransitionStart { get; set; }
        public DaylightTransitionEnd DaylightTransitionEnd { get; set; }
        public string BaseUtcOffsetDelta { get; set; }
        public bool NoDaylightTransitions { get; set; }
    }

    public class DaylightTransitionEnd
    {
        public DateTime TimeOfDay { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsFixedDateRule { get; set; }
    }

    public class DaylightTransitionStart
    {
        public DateTime TimeOfDay { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsFixedDateRule { get; set; }
    }

    public class FacilitatorInfo
    {
        public string FacilitatorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object FacilitatorTags { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
        public string Email { get; set; }
        public int NumberOfFacilitation { get; set; }
        public bool IsFirst { get; set; }
    }

    public class Paging
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int FirstRowOnPage { get; set; }
        public int LastRowOnPage { get; set; }
    }

    public class TimeZoneInfo
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string StandardName { get; set; }
        public string DaylightName { get; set; }
        public string BaseUtcOffset { get; set; }
        public List<AdjustmentRule> AdjustmentRules { get; set; }
        public bool SupportsDaylightSavingTime { get; set; }
    }
}
