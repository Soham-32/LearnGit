using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.MultiTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("MultiTeam")]
    public class MultiTeamUploadMembersTests : TeamsBaseTest

    {
        public static bool ClassInitFailed;
        private static int _multiTeamId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var multiTeam = TeamFactory.GetMultiTeam("MultiTeam");
                var multiTeamResponse = setup.CreateTeam(multiTeam).GetAwaiter().GetResult();
                _multiTeamId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(multiTeamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void MultiTeam_Upload_TeamMembers_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadTeamMembersViaExcelAndVerify(_multiTeamId);

        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void MultiTeam_Upload_Stakeholders_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadStakeholdersViaExcelAndVerify(_multiTeamId);
        }
    }
}
