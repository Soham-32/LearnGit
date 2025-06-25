using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.EnterpriseTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("EnterpriseTeam")]
    public class EnterpriseTeamUploadMembersTests : TeamsBaseTest
    {
        public static bool ClassInitFailed;
        private static int _enterpriseId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                var enterpriseTeam = TeamFactory.GetEnterpriseTeam("EnterpriseTeam");
                var enterpriseTeamResponse = setup.CreateTeam(enterpriseTeam).GetAwaiter().GetResult();
                _enterpriseId = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(enterpriseTeamResponse.Name).TeamId;
            }
            catch (Exception)
            {
                ClassInitFailed = true;
                throw;
            }
        }

        [TestMethod]
        [TestCategory("CompanyAdmin")]
        public void EnterpriseTeam_Upload_TeamMembers_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadTeamMembersViaExcelAndVerify(_enterpriseId);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug Id : 51417
        [TestCategory("CompanyAdmin")]
        public void Enterprise_Upload_Stakeholders_ViaExcelFile()
        {
            VerifySetup(ClassInitFailed);

            UploadStakeholdersViaExcelAndVerify(_enterpriseId);
        }
    }
}