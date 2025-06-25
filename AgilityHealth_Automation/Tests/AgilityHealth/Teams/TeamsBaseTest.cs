using System;
using System.Data;
using System.IO;
using System.Linq;
using AgilityHealth_Automation.Base;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Teams.Common.Create;
using AgilityHealth_Automation.Utilities;
using AtCommon.Dtos;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Teams
{
    [TestClass]
    public class TeamsBaseTest : BaseTest
    {
        protected static readonly User NTierUser = TestEnvironment.UserConfig.GetUserByDescription("ntier user");
        private const string ValidationMessageForInvalidEmail = "Please enter a valid email";
        private const string ValidationMessageForBlankEmail = "Please enter the email";

        protected internal void UploadTeamMembersViaExcelAndVerify(int teamId, bool isNTierTeam = false)
        {
            var login = new LoginPage(Driver, Log);
            var addTeamMemberPage = new AddTeamMemberPage(Driver, Log);

            Log.Info($"Login as {User} user");
            Driver.NavigateToPage(ApplicationUrl);
            if (isNTierTeam)
            {
                login.LoginToApplication(NTierUser.Username,NTierUser.Password);
            }
            else
            {
                login.LoginToApplication(User.Username, User.Password);
            }
            addTeamMemberPage.NavigateToPage(teamId);

            Log.Info("Click on 'Upload Team Members' button and upload the excel file which contains fresh members");
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\TeamMembersImport.xlsx");
            var tbl = ExcelUtil.GetExcelData(filePath, "Sheet1");
            addTeamMemberPage.UploadMembersFromExcelFile(filePath);
            addTeamMemberPage.ImportCompletePopupClickOnCloseButton();

            Log.Info("Verify that uploaded members are exist on the grid.");
            foreach (DataRow row in tbl.Rows.Cast<DataRow>().Skip(2).Take(2))
            {
                var userImagePath = addTeamMemberPage.GetAvatarFromMembersGrid(row["Email Address"].ToString());
                Assert.IsTrue(addTeamMemberPage.DoesMemberExist(userImagePath, row["FirstName"].ToString(), row["LastName"].ToString(), row["Email Address"].ToString()),
                    $"Team Member with First name: {row["FirstName"]} and Last name: {row["LastName"]} does not exist");
            }

            Log.Info("Click on 'Upload Team Members' button and upload the excel file which contains existing member, member with invalid email and member with blank email");
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\TeamMembersImportWithExistingMemberAndInvlidAndBlankEmail.xlsx");
            addTeamMemberPage.UploadMembersFromExcelFile(filePath);

            // Verify validation message for existing member
            Assert.AreEqual("The team member already exists for this team.", addTeamMemberPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for existing member");
            addTeamMemberPage.InvalidMemberPopupClickOnSkipTeamMemberButton();

            // Verify validation message for invalid email
            Assert.AreEqual(ValidationMessageForInvalidEmail, addTeamMemberPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for invalid email");
            addTeamMemberPage.InvalidMemberPopupClickOnSkipTeamMemberButton();

            // Verify validation message for blank email
            Assert.AreEqual(ValidationMessageForBlankEmail, addTeamMemberPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for blank email");

            // Add member on 'invalid member' popup
            Log.Info("Add valid member details on 'invalid member' popup and update then verify newly added member");
            var teamMemberInfo = SharedConstants.TeamMember1;
            teamMemberInfo.Email = "ah_automation" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain;
            addTeamMemberPage.FillMemberInfo(teamMemberInfo);
            addTeamMemberPage.InvalidMemberPopupClickOnUpdateButton();
            addTeamMemberPage.ImportCompletePopupClickOnCloseButton();
            Assert.IsTrue(addTeamMemberPage.IsTeamMemberDisplayed(teamMemberInfo.Email),
                $"Team Member with First name: {teamMemberInfo.FirstName} and Last name: {teamMemberInfo.LastName} does not exist");

            Log.Info("Click on 'Upload Team Members' button and upload the blank excel file then verify");
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\MembersImportWithBlankExcel.xlsx");
            addTeamMemberPage.UploadMembersFromExcelFile(filePath);
            Assert.IsTrue(addTeamMemberPage.BlankExcelImportPopupGetPopupMessage().Contains("No team member's found to Upload Please Add team members and try again"), "Validation message is not exist for blank excel file");

        }

        protected void UploadStakeholdersViaExcelAndVerify(int teamId)
        {
            var login = new LoginPage(Driver, Log);
            var addStakeHoldersPage = new AddStakeHolderPage(Driver, Log);

            Log.Info($"Login as {User} user");
            Driver.NavigateToPage(ApplicationUrl);
            login.LoginToApplication(User.Username, User.Password);
            addStakeHoldersPage.NavigateToPage(teamId);


            Log.Info("Click on 'Upload Stakeholders' button and upload the excel file which contains fresh stakeholders");
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\StakeholdersImportData.xlsx");
            var tbl = ExcelUtil.GetExcelData(filePath, "Sheet1");
            addStakeHoldersPage.UploadMembersFromExcelFile(filePath);
            addStakeHoldersPage.ImportCompletePopupClickOnCloseButton();

            Log.Info("Verify that uploaded stakeholders are exist on the grid.");
            foreach (DataRow row in tbl.Rows.Cast<DataRow>().Skip(2).Take(2))
            {
                var userImagePath = addStakeHoldersPage.GetAvatarFromMembersGrid(row["Email Address"].ToString());
                Assert.IsTrue(addStakeHoldersPage.DoesMemberExist(userImagePath, row["FirstName"].ToString(), row["LastName"].ToString(), row["Email Address"].ToString()),
                    $"Stakeholders with First name: {row["FirstName"]} and Last name: {row["LastName"]} does not exist");
            }

            Log.Info("Click on 'Upload Stakeholders' button and upload the excel file which contains existing stakeholders, stakeholders with invalid email and stakeholders with blank email");
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\StakeholdersImportWithExistingMemberAndInvlidEmail.xlsx");
            addStakeHoldersPage.UploadMembersFromExcelFile(filePath);

            // Verify validation message for existing stakeholder
            Assert.AreEqual("The stakeholder already exists for this team.", addStakeHoldersPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for existing stakeholder");
            addStakeHoldersPage.InvalidMemberPopupClickOnSkipTeamMemberButton();

            // Verify validation message for invalid email
            Assert.AreEqual(ValidationMessageForInvalidEmail, addStakeHoldersPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for invalid email");
            for (int i = 0; i < 2; i++)
            {
                addStakeHoldersPage.InvalidMemberPopupClickOnSkipTeamMemberButton();
            }
            addStakeHoldersPage.ImportCompletePopupClickOnCloseButton();

            // Verify validation message for blank email
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\StakeholdersImportWithBlankEmail.xlsx");
            addStakeHoldersPage.UploadMembersFromExcelFile(filePath);
            Assert.AreEqual(ValidationMessageForBlankEmail, addStakeHoldersPage.InvalidMemberPopupGetErrorMessage(), "Validation message is not matched for blank email");

            // Add member on 'invalid stakeholder' popup
            Log.Info("Add valid stakeholder details on 'invalid member' popup and update then verify newly added stakeholder");
            var stakeHolderInfo = SharedConstants.Stakeholder1;
            stakeHolderInfo.Email = "ah_automation" + CSharpHelpers.RandomNumber() + Constants.UserEmailDomain;
            addStakeHoldersPage.FillMemberInfo(stakeHolderInfo);
            addStakeHoldersPage.InvalidMemberPopupClickOnUpdateButton();
            addStakeHoldersPage.ImportCompletePopupClickOnCloseButton();
            Assert.IsTrue(addStakeHoldersPage.IsTeamMemberDisplayed(stakeHolderInfo.Email),
                $"StakeHolder with First name: {stakeHolderInfo.FirstName} and Last name: {stakeHolderInfo.LastName} does not exist");

            Log.Info("Click on 'Upload StakeHolder' button and upload the blank excel file then verify");
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\TeamMembers\\MembersImportWithBlankExcel.xlsx");
            addStakeHoldersPage.UploadMembersFromExcelFile(filePath);
            Assert.IsTrue(addStakeHoldersPage.BlankExcelImportPopupGetPopupMessage().Contains("No stakeholder's found to Upload Please Add stakeholders and try again"), "Validation message is not exist for blank excel file");

        }

    }
}