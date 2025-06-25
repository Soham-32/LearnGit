using System.Collections.Generic;
using System.Linq;

namespace AtCommon.Dtos.Analytics
{
    public class BaseTeamIdentityAnalyticsPageableModel
    {
        public IEnumerable<int> TeamIds { get; set; }
        public IEnumerable<int> HydratedTeamIds { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string FilterBy { get; set; }
        public string SortBy { get; set; }
        public string SortDir { get; set; }
        public bool UseRootTeam { get { return this.TeamIds != null && this.TeamIds.Any(id => id == 0); } }
        public virtual bool HydrateTeamIds { get { return false; } }
    }
}
