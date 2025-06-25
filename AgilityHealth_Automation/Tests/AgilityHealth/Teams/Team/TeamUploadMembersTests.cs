using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.Team
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("Team")]
    public class TeamUploadMembersTests : TeamsBaseTest
    {
        public static bool ClassInitFailed;
        private static int _teamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var team = TeamFactory.GetNormalTeam("Team");
                var teamResponse = setup.CreateTeam(team).GetAwaiter().GetResult();
                _teamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(teamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Team_Upload_TeamMembers_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadTeamMembersViaExcelAndVerify(_teamId);
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void Team_Upload_Stakeholders_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadStakeholdersViaExcelAndVerify(_teamId);

        }
    }
}
