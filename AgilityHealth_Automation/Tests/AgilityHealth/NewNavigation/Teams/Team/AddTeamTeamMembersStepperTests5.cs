using AgilityHealth_Automation.Enum.NewNavigation;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.NewNavigation.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team"), TestCategory("NewNavigation")]
    public class AddTeamTeamMembersStepperTests5 : NewNavBaseTest
    {
        public static bool ClassInitFailed;
        private static TeamResponse _expectedTeamResponse;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                // Getting team details from existing team
                _expectedTeamResponse = setup.GetTeamResponse(SharedConstants.Team);
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_AddFromDirectory_AddMember_Successfully()
        {
            VerifySetup(ClassInitFailed);

            Verify_Team_AddTeamMembersLeadersStepper_AddFromDirectory_AddMemberLeader_Successfully(TeamType.FullWizardTeam, _expectedTeamResponse);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 51142
        [TestCategory("Critical")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin")]
        public void Team_FullWizard_Verify_AddFromDirectory_AddTeam_Successfully()
        {
            VerifySetup(ClassInitFailed);

            Verify_Team_AddTeamMembersLeaders_AddFromDirectory_AddTeam_Successfully(TeamType.FullWizardTeam, _expectedTeamResponse);
        }
    }
}