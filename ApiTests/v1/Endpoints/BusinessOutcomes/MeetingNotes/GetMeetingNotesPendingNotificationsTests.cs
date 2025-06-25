using AtCommon.Api;
using AtCommon.Dtos.BusinessOutcomes.MeetingNotes;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories.BusinessOutcomes.MeetingNotes;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ApiTests.v1.Endpoints.BusinessOutcomes.MeetingNotes
{
    [TestClass]
    [TestCategory("BusinessOutcomes")]
    public class GetMeetingNotesPendingNotificationsTests : BaseV1Test
    {
        private static int _teamId;
        private static Dictionary<string, object> _queries;
        private static BusinessOutcomesMeetingNotesResponse _meetingCard;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            _teamId = new SetupTeardownApi(TestEnvironment).GetCompanyHierarchy(Company.Id).GetTeamByName(SharedConstants.RadarTeam).TeamId;
            _queries = new Dictionary<string, object>
            {
               {"companyId", Company.Id },
               {"teamId",_teamId}
            };
        
            var request = BusinessOutcomesMeetingNotesFactory.GetValidMeetingNotesRequest(Company.Id, _teamId, User);
            _meetingCard= new SetupTeardownApi(TestEnvironment).CreateBusinessOutcomesMeetingNotes(request, User);
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNotePendingNotifications_Get_Success()
        {
            var client = await  GetAuthenticatedClient();

            var response = await client.GetAsync<bool>(RequestUris.BusinessOutcomeGetMeetingNotePendingNotifications.AddQueryParameter(_queries));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode,"Status code didn't match");
            Assert.IsTrue(response.Dto,"No notifications are present");
        }

        [TestMethod]
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNotePendingNotifications_Get_UnAuthenticated()
        {
            var client = GetUnauthenticatedClient(); 

            var response = await client.GetAsync<string>(RequestUris.BusinessOutcomeGetMeetingNotePendingNotifications.AddQueryParameter(_queries));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code didn't match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53942
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNotePendingNotifications_Get_BadRequest_BadTeamId()
        {
            var client = await GetAuthenticatedClient();

            var incompleteQuery = new Dictionary<string, object> { { "teamId", "FakeData" } }; 
            var response = await client.GetAsync<string>(RequestUris.BusinessOutcomeGetMeetingNotePendingNotifications.AddQueryParameter(incompleteQuery));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
        [TestCategory("KnownDefect")] //Bug Id: 53942
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNotePendingNotifications_Get_BadRequest_BadCompanyData()
        {
            var client = await GetAuthenticatedClient();

            var incompleteQuery = new Dictionary<string, object> { { "companyId", "CompanyId" } }; 
            var response = await client.GetAsync<string>(RequestUris.BusinessOutcomeGetMeetingNotePendingNotifications.AddQueryParameter(incompleteQuery));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
        [TestMethod]
        [TestCategory("KnownDefect")] //Bug Id: 53942
        [TestCategory("TeamAdmin"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("SiteAdmin"), TestCategory("Member"), TestCategory("PartnerAdmin")]
        public async Task BusinessOutcome_MeetingNotePendingNotifications_Get_BadRequest_NoData()
        {
            var client = await GetAuthenticatedClient();

            var incompleteQuery = new Dictionary<string, object>();
            var response = await client.GetAsync<string>(RequestUris.BusinessOutcomeGetMeetingNotePendingNotifications.AddQueryParameter(incompleteQuery));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code didn't match");
        }
    }
}
