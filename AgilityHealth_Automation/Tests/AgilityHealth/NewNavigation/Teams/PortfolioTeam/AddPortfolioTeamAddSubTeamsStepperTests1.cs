﻿using AgilityHealth_Automation.Enum.NewNavigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.PortfolioTeam
{
    [TestClass]
    [TestCategory("Multi-Team"), TestCategory("NewNavigation")]
    public class AddPortfolioTeamAddSubTeamsStepperTests1 : NewNavBaseTest
    {
        [TestMethod]
        [TestCategory("Critical")]
        [TestCategory("KnownDefectAsTA")] //Bug Id: 53523
        [TestCategory("KnownDefectAsBL")] //Bug Id: 53523
        [TestCategory("KnownDefectAsOL")] //Bug Id: 53523
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void PortfolioTeam_Add_Sub_Teams()
        {
            Verify_PortfolioMultiTeam_AddSubTeamsSteppers_AddSubTeams(TeamType.PortfolioTeam);
        }
    }
}
