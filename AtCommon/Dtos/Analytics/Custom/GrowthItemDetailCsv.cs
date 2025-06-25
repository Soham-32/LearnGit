using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Text;
using AtCommon.Utilities;

namespace AtCommon.Dtos.Analytics.Custom
{
    public class GrowthItemDetailCsv
    {
        public string Title { get; set; }
        public string Priority { get; set; }
        public string RequestedBy { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string GroupBy { get; set; }
        public string Type { get; set; }

        public static List<GrowthItemDetailCsv> ParseCsv(string content)
        {
            // split the string by line returns to get the rows
            var rows = content.SplitLines();
            // get the columns from the first row
            var columns = rows[0].SplitCommas().ToList();
            // remove the columns row
            rows.RemoveAt(0);
            // for each remaining row convert it to an object
            return rows.Select(row => row.SplitCommas())
                .Select(parsedRow => new GrowthItemDetailCsv
                {
                    Title = parsedRow[columns.FindIndex(x => x == "Title")],
                    Priority = parsedRow[columns.FindIndex(x => x == "Priority")],
                    RequestedBy = parsedRow[columns.FindIndex(x => x == "Requested By")],
                    Created = DateTime.ParseExact(parsedRow[columns.FindIndex(x => x == "Created")], "ddd MMM dd yyyy", CultureInfo.InvariantCulture),
                    Description = parsedRow[columns.FindIndex(x => x == "Description")].Replace("\"", "").Trim(),
                    Status = parsedRow[columns.FindIndex(x => x == "Status")],
                    GroupBy = parsedRow[columns.FindIndex(x => x == "Group By")],
                    Type = parsedRow[columns.FindIndex(x => x == "GI Type")]
                })
                .ToList();
        }
        
    }
}