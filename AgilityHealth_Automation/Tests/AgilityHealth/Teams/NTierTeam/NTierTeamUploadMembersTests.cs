using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Dashboard.Teams;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.N_Tier;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Team.Create;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams.NTierTeam
{
    [TestClass]
    [TestCategory("Teams"), TestCategory("NTier")]
    public class NTierTeamUploadMembersTests : TeamsBaseTest
    {
        private static int _nTierTeamId;
        protected new static readonly User NTierUser = TestEnvironment.UserConfig.GetUserByDescription("ntier user");

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        public void NTier_TeamUpload_TeamMembers_ViaExcelFile()
        {
            var login = new LoginPage(Driver, Log);
            var dashBoardPage = new TeamDashboardPage(Driver, Log);
            var createNTierPage = new CreateNTierPage(Driver, Log);
            var addStakeHolderPage = new AddStakeHolderPage(Driver, Log);
            var finishAndReviewPage = new FinishAndReviewPage(Driver, Log);
            var setup = new SetupTeardownApi(TestEnvironment);
            var topNav = new TopNavigation(Driver, Log); 
            var nTierTeamName = "N-TierTeam_" + RandomDataUtil.GetTeamName();

            login.NavigateToPage();
            login.LoginToApplication(NTierUser.Username, NTierUser.Password);
            dashBoardPage.CloseDeploymentPopup();

            //Add N-Tier Team
            Log.Info("Create an N-Tier team & Add team member via excel file.");
            createNTierPage.CreateNTierTeamWithSubTeam(nTierTeamName);
            addStakeHolderPage.ClickReviewAndFinishButton();
            finishAndReviewPage.ClickOnGoToTeamDashboard();
            topNav.LogOut();
            _nTierTeamId = setup.GetCompanyHierarchy(Company.NtierId, NTierUser).GetTeamByName(nTierTeamName).TeamId;
            UploadTeamMembersViaExcelAndVerify(_nTierTeamId,true);
        }
    }
}
