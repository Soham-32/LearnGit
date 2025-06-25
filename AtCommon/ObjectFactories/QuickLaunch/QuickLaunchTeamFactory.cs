using AtCommon.Dtos.Teams.Custom.QuickLaunch;
using AtCommon.Utilities;
using System;

namespace AtCommon.ObjectFactories.QuickLaunch
{
    public static class QuickLaunchTeamFactory
    {
        public static QuickLaunchTeam GetValidQuickLaunchTeamInfo() => new QuickLaunchTeam
        {
            TeamName = $"Quick_Launch{RandomDataUtil.GetTeamName()}",
            WorkType = SharedConstants.NewTeamWorkType
        };
    }
}