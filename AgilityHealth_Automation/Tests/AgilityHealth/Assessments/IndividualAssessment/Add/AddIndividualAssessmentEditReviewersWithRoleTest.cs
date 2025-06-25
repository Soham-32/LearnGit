using System;
using System.Collections.Generic;
using System.Linq;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Account;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Common;
using AgilityHealth_Automation.PageObjects.AgilityHealth.Assessment.IndividualAssessment.Create;
using AtCommon.Api;
using AtCommon.Dtos.Assessments;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Assessments.IndividualAssessment.Add
{
    [TestClass]
    [TestCategory("TalentDevelopment"), TestCategory("Assessments")]
    public class AddIndividualAssessmentEditReviewersWithRoleTest : IndividualAssessmentBaseTest
    {
        private static bool _classInitFailed;
        private static TeamResponse _team;
        private static CreateReviewerRequest _reviewer;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            try
            {
                var setup = new SetupTeardownApi(TestEnvironment);
                _team = GetTeamForIndividualAssessment(setup, "IA");
                _reviewer = MemberFactory.GetReviewer();
                setup.CreateReviewer(_reviewer).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                _classInitFailed = true;
                throw;
            }
           
        }

        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader")]
        public void IndividualAssessment_EditReviewersWithRole()
        {
            VerifySetup(_classInitFailed);

            var login = new LoginPage(Driver, Log);
            var createIndividualAssessment1 = new CreateIndividualAssessment1CreateAssessmentPage(Driver, Log);
            var createIndividualAssessment2 = new CreateIndividualAssessment2AddReviewersPage(Driver, Log);
            var createIndividualAssessment3 = new CreateIndividualAssessment3InviteViewersPage(Driver, Log);
            var createIndividualAssessment4 = new CreateIndividualAssessment4AddReviewAndPublishPage(Driver, Log);
            var addReviewerModal = new AddReviewerModal(Driver, Log);

            login.NavigateToPage();
            login.LoginToApplication(User.Username, User.Password);

            createIndividualAssessment1.NavigateToPage(Company.Id, _team.Uid);

            var assessment = IndividualAssessmentFactory.GetUiIndividualAssessment($"EditCreateIA_{Guid.NewGuid()}");
            assessment.Members = _team.Members.Select(m => m.ToAddIndividualMemberRequest()).ToList();

            createIndividualAssessment1.WaitUntilLoaded();
            createIndividualAssessment1.FillInIndividualAssessmentInfo(assessment,
                SharedConstants.IndividualAssessmentType);
            createIndividualAssessment1.ClickNextButton();

            createIndividualAssessment2.WaitUntilLoaded();
            createIndividualAssessment2.ExpandCollapseParticipantsAndReviewers();
            createIndividualAssessment2.ClickAddReviewer(_team.Members[0].Email);
            addReviewerModal.WaitUntilLoaded();

            addReviewerModal.AddReviewersBySearchingInModal(_reviewer);
            createIndividualAssessment2.EditReviewer(_reviewer.Email);
            var email = Guid.NewGuid() + "@example.com";
            var firstName = Guid.NewGuid().ToString();
            const string lastName = "Name";
            var roles = new List<RoleResponse>
            {
                new RoleResponse
                {
                    Tags = new List<TagRoleResponse>
                    {
                        new TagRoleResponse
                        {
                            Id = 1,
                            Name = "Customer"
                        },
                        new TagRoleResponse
                        {
                            Id = 1,
                            Name = "Reviewer"
                        }
                    }
                }
            };

            createIndividualAssessment2.UpdateMember(email, firstName, lastName, roles[0].Tags);

            Log.Info("Assert: Verify that edited reviewer with roles displays in reviewers list");
            Assert.IsTrue(createIndividualAssessment2.DoesReviewerDisplayCreateIaAddReviewerList($"{firstName} {lastName}", email, roles[0].Tags),
                $"Edited reviewer {email} with roles doesn't display in reviewers list");

            createIndividualAssessment2.ClickNextButton();

            createIndividualAssessment3.WaitUntilLoaded();
            createIndividualAssessment3.ClickNextButton();

            //publish
            createIndividualAssessment4.WaitUntilLoaded();

            Log.Info("Assert: Verify that edited reviewer with roles displays in reviewers list in review screen");
            Assert.IsTrue(createIndividualAssessment4.DoesReviewerDisplayCreateIaPublishReviewerList($"{firstName} {lastName}", email, roles[0].Tags),
                $"Edited reviewer {email} with roles doesn't display in reviewers list in review screen");
        }
    }
}
