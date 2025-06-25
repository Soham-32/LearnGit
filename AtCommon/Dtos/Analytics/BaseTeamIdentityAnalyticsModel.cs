using System.Collections.Generic;
using System.Linq;

namespace AtCommon.Dtos.Analytics
{
    public class BaseTeamIdentityAnalyticsModel
    {
        public IEnumerable<int> TeamIds { get; set; }
        public string SelectedTeamParents { get; set; }
        public IEnumerable<int> HydratedTeamIds { get; set; }
        public bool UseRootTeam { get { return this.TeamIds != null && this.TeamIds.Any(id => id == 0); } }
        public virtual bool HydrateTeamIds { get { return false; } }
        public int SelectedTeamId { get; set; }
    }
}
