using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Edit;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.IndividualAssessments;
using AtCommon.Dtos.Reports;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Edit
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class EditIndividualAssessmentAddReviewerWithRoleTests : IndividualAssessmentBaseTest
    {
        private static CreateIndividualAssessmentRequest _assessmentRequest;
        private static IndividualAssessmentResponse _assessment;
        private static ReviewerResponse _reviewer;
        private static TeamResponse _team;
        private static bool _classInitFailed;
        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);

                _team = GetTeamForBatchEdit(setup, "IA");
                var individualDataResponse = GetIndividualAssessment(setup, _team, "ReviewRoleEditIASection_");
                _assessmentRequest = individualDataResponse.Item2;
                _assessmentRequest.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();
                
                _reviewer = setup.CreateReviewer(MemberFactory.GetReviewer()).GetAwaiter().GetResult();
                _assessmentRequest.Members.First().Reviewers.Add(_reviewer.ToAddIndividualMemberRequest());
                
                _assessment = setup.CreateIndividualAssessment(_assessmentRequest, SharedConstants.IndividualAssessmentType).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
        }
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_Edit_Reviewer_Add_Role()
        {
            VerifySetup(_classInitFailed);
            
            var login = new LoginPage(Driver, Log);
            var iaEditPage = new IaEdit(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);
            
            var assessmentUid = _assessment.AssessmentList.FirstOrDefault().CheckForNull().AssessmentUid;
            iaEditPage.NavigateToPage(Company.Id, _team.Uid, assessmentUid);
            
            Log.Info("Go to created Individual assessment and edit reviewer with Role");
            iaEditPage.ClickOnEditReviewer(_reviewer.Email);
            var roles = new List<string>
            {
                "Customer",
                "Reviewer"
            };
            iaEditPage.EditRoles(roles);
            var updatedRole = iaEditPage.GetRolesOfReviewer(_reviewer.Email).StringToList();

            Assert.That.ListsAreEqual(updatedRole, roles, "Added Roles are not matching");

            Log.Info($"Remove {roles.First()} from the reviewer");
            iaEditPage.ClickOnEditReviewer(_reviewer.Email);
            iaEditPage.RemoveRoleFromReviewer(roles.First());

            Assert.IsFalse(iaEditPage.DoesRoleOfReviewerDisplay(roles.First()), "Role does exist");
            
            Log.Info($"Verify remaining role- {roles.Last()} in grid ");
            var rolesOfReviewer = iaEditPage.GetRolesOfReviewer(_reviewer.Email);
            Assert.AreEqual(roles.Last(), rolesOfReviewer, "Role does not exist");
        }
    }
}