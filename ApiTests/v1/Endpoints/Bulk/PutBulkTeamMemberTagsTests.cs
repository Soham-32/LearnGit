using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Bulk;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    [TestCategory("Bulk"), TestCategory("Public")]
    public class PutBulkTeamMemberTagsTests : BulkBaseTest
    {
        
        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_TeamMemberTags_Put_Created()
        {
            var caClient = GetCaClient();
            var teamRequest = TeamFactory.GetNormalTeam("bulk", 1);
            var teamResponse = GetTeamForBulk(caClient, teamRequest);

            // given
            var client = await GetAuthenticatedClient();
            var updatedTag = new List<MemberTagRequest>
            {
                MemberFactory.GetMemberTagRequest(teamResponse.Members.First().Email,
                    teamRequest.ExternalIdentifier)
            };
            // when
            var response = await client.PutAsync(RequestUris.BulkTeamMemberTags(Company.Id),
                updatedTag.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");

            var memberResponse = GetTeamMembers(caClient, teamResponse.Uid);
            var actualTags = memberResponse.First().Tags.First();
            Assert.AreEqual(updatedTag.First().Category, actualTags.Key);
            Assert.That.ListsAreEqual(updatedTag.First().Tags, actualTags.Value);
        }

        // 400
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 39379
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_TeamMemberTags_Put_EmptyValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var updatedTag = new List<MemberTagRequest>
            {
                new MemberTagRequest
                {
                    Email = "",
                    TeamExternalIdentifier = "",
                    Category = "",
                    Tags = new List<string> { "" }
                }
            };

            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkTeamMemberTags(Company.Id),
                updatedTag);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            var expectedErrorList = new List<string>
            {
                "'Team External Identifier' must not be empty.",
                "'Email' must not be empty.",
                "The specified condition was not met for 'Category'.",
                "'Tags' must not be empty."
            };
            Assert.That.ListsAreEqual(expectedErrorList, response.Dto);
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 39379
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_TeamMemberTags_Put_NullValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var updatedTag = new List<MemberTagRequest> { new MemberTagRequest() };

            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkTeamMemberTags(Company.Id),
                updatedTag);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            var expectedErrorList = new List<string>
            {
                "'Team External Identifier' must not be empty.",
                "'Email' must not be empty.",
                "'Category' must not be empty.",
                "The specified condition was not met for 'Category'."
            };
            Assert.That.ListsAreEqual(expectedErrorList, response.Dto);
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_TeamMemberTags_Put_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();

            // when
            var response = await client.PutAsync(RequestUris.BulkTeamMemberTags(Company.Id), 
                "".ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_TeamMemberTags_Put_Permission_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PutAsync(RequestUris.BulkTeamMemberTags(2),
                "".ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_TeamMemberTags_Put_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();

            // when
            var response = await client.PutAsync(RequestUris.BulkTeamMemberTags(Company.Id),
                "".ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_TeamMemberTags_Put_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var updatedTags = MemberFactory.GetMemberTagRequest("",
                Guid.NewGuid().ToString("D"));
            // when
            var response = await client.PutAsync(RequestUris.BulkTeamMemberTags(SharedConstants.FakeCompanyId),
                updatedTags.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
                "Status code doesn't match.");
        }
    }
}