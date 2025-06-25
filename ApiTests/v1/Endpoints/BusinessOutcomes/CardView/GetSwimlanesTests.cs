using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes;
using AtCommon.Dtos.BusinessOutcomes.Custom;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.CardView
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class GetSwimlanesTests : BaseV1Test
    {
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcomes_Swimlanes_Get_Success()
        {
            //arrange
            var client = await GetAuthenticatedClient();
            var setup = new SetupTeardownApi(TestEnvironment);
            var team = setup.GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.MultiTeam);
            var expectedSwimLaneCounts = team.Children.Count + 4; 

            //creating a business outcome
            var expectedBo = BusinessOutcomesFactory.GetValidPostBusinessOutcome(Company, 0);
            var boResponse = await client.PostAsync<CustomBusinessOutcomeRequest>(
                RequestUris.BusinessOutcomePost(), expectedBo);
            boResponse.EnsureSuccess();
            
            var swimlanesResponse = await client.GetAsync<IList<BusinessOutcomeSwimlaneResponse>>(
                RequestUris.BusinessOutcomeAllSwimlanes(Company.Id).AddQueryParameter("teamId", team.TeamId));
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, swimlanesResponse.StatusCode,"Status code didn't match'");
            Assert.AreEqual(expectedSwimLaneCounts, swimlanesResponse.Dto.Count, "Swimlanes count didn't match'");

            //Swimlane #0
            Assert.AreEqual(Company.Id, swimlanesResponse.Dto[0].CompanyId, "Swimlanes #0 company id didn't match");
            Assert.AreEqual(team.TeamId, swimlanesResponse.Dto[0].TeamId, "Swimlane #0 team id didn't match");
            Assert.AreEqual("3 Year Outcomes", swimlanesResponse.Dto[0].Title, "Swimlane #0 Title didn't match");
            
            var actualBo = swimlanesResponse.Dto[0].BusinessOutcomes.FirstOrDefault(b => b.Title == expectedBo.Title)
                .CheckForNull($"{expectedBo.Title} not found");
            Assert.AreEqual(expectedBo.Title, actualBo.Title, "BO Item #0 Title doesn't match");
            Assert.AreEqual(expectedBo.Description, actualBo.Description, "BO Item #0 Description doesn't match");
            Assert.AreEqual(expectedBo.OverallProgress, actualBo.OverallProgress, "BO Item #0 OverallProgress doesn't match");
            Assert.AreEqual(expectedBo.CardColor, actualBo.CardColor, "BO Item #0 CardColor doesn't match");
            Assert.AreEqual(User.FullName, actualBo.Owner, "BO Item #0 Owner doesn't match");
            Assert.AreEqual(expectedBo.TeamId, actualBo.TeamId, "BO Item #0 TeamId doesn't match");
            Assert.AreEqual(expectedBo.CompanyId, actualBo.CompanyId, "BO Item #0 CompanyId doesn't match");
            Assert.AreEqual(expectedBo.SwimlaneType, (int)actualBo.SwimlaneType, "BO Item #0 SwimlaneType doesn't match");
            Assert.AreEqual(expectedBo.IsDeleted, actualBo.IsDeleted, "BO Item #0 IsDeleted doesn't match");
            Assert.AreEqual(expectedBo.Tags.Count, actualBo.Tags.Count, "BO Item #0 Tags count doesn't match");
            Assert.AreEqual(expectedBo.Comments.Count, actualBo.Comments.Count, "BO Item #0 Comments count doesn't match");
            Assert.AreEqual(expectedBo.KeyResults.Count, actualBo.KeyResults.Count, "BO Item #0 KeyResults count doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Title, actualBo.KeyResults[0].Title, "BO Item #0 Key Result Title doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].IsImpact, actualBo.KeyResults[0].IsImpact, "BO Item #0 Key Result IsImpact doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Start, actualBo.KeyResults[0].Start, "BO Item #0 Key Result Start doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Target, actualBo.KeyResults[0].Target, "BO Item #0 Key Result Target doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Progress, actualBo.KeyResults[0].Progress.ToString("F1"), "BO Item #0 Key Result Progress doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].IsDeleted, actualBo.KeyResults[0].IsDeleted, "BO Item #0 Key Result IsDeleted doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Metric.Name, actualBo.KeyResults[0].Metric.Name, "BO Item #0 Key Result Metric Name doesn't match");
            Assert.AreEqual(expectedBo.KeyResults[0].Metric.TypeId, actualBo.KeyResults[0].Metric.TypeId, "BO Item #0 Key Result Metric TypeId doesn't match");

            //Swimlane #1
            Assert.AreEqual(Company.Id, swimlanesResponse.Dto[1].CompanyId, "Swimlanes #1 company id didn't match");
            Assert.AreEqual(team.TeamId, swimlanesResponse.Dto[1].TeamId, "Swimlane #1 team id didn't match");
            Assert.AreEqual("1 Year Outcomes", swimlanesResponse.Dto[1].Title, "Swimlane #1 Title didn't match");
            
            //Swimlane #2
            Assert.AreEqual(Company.Id, swimlanesResponse.Dto[2].CompanyId, "Swimlanes #2 company id didn't match");
            Assert.AreEqual(team.TeamId, swimlanesResponse.Dto[2].TeamId, "Swimlane #2 team id didn't match");
            Assert.AreEqual("Quarterly Outcomes", swimlanesResponse.Dto[2].Title, "Swimlane #2 Title didn't match");
            
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader"), TestCategory("BLAdmin"), TestCategory("Member")]
        public async Task BusinessOutcomes_Swimlanes_Get_Unauthorized()
        {
            //arrange
            var client = GetUnauthenticatedClient();

            // act
            var swimlanesResponse = await client.GetAsync<IList<BusinessOutcomeSwimlaneResponse>>(
                RequestUris.BusinessOutcomeAllSwimlanes(Company.Id).AddQueryParameter("teamId", Company.TeamId1));

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, swimlanesResponse.StatusCode, "Status code didn't match'");
        }
    }
}
