using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Analytics
{
    public class PagedBaseResponse
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    public class PagedResponse<T> : PagedBaseResponse where T : class
    {
        public List<T> Results { get; set; }

        public PagedResponse()
        {
            Results = new List<T>();
        }
    }
}
