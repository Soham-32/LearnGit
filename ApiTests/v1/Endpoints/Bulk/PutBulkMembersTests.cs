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
    public class PutBulkMembersTests : BulkBaseTest
    {
        private static readonly List<string> ExpectedErrorList = new List<string>
        {
            "'Email' must not be empty.",
            "'First Name' must not be empty.",
            "'Last Name' must not be empty.",
            "'Team External Identifier' must not be empty."
        };

        // 200-201
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_NewMember_Created()
        {
            var caClient = GetCaClient();
            var teamRequest = TeamFactory.GetNormalTeam("bulk");
            var teamResponse = GetTeamForBulk(caClient, teamRequest);

            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers> { MemberFactory.GetMemberForBulkImport(teamRequest.ExternalIdentifier) };

            // when
            var response = await client.PutAsync(RequestUris.BulkMembers(Company.Id), 
                members.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");

            var memberResponse = GetTeamMembers(caClient, teamResponse.Uid);
            Assert.AreEqual(members.First().FirstName, memberResponse.First().FirstName,
                "FirstName does not match.");
            Assert.AreEqual(members.First().LastName, memberResponse.First().LastName,
                "LastName does not match.");
            Assert.AreEqual(members.First().Email, memberResponse.First().Email,
                "Email does not match.");
            Assert.AreEqual(members.First().ExternalIdentifier, memberResponse.First().ExternalIdentifier,
                "ExternalIdentifier does not match.");
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_ExistingMember_Created()
        {
            var caClient = GetCaClient();
            var teamRequest = TeamFactory.GetNormalTeam("bulk");
            var teamResponse = GetTeamForBulk(caClient, teamRequest);

            // given
            var client = await GetAuthenticatedClient();
            var newMember = MemberFactory.GetMemberForBulkImport(teamRequest.ExternalIdentifier);
            
            // when
            var createResponse = await client.PutAsync(RequestUris.BulkMembers(Company.Id), 
                new List<AddMembers> { newMember }.ToStringContent());
            createResponse.EnsureSuccessStatusCode();
            var updatedMember = new AddMembers
            {
                FirstName = newMember.FirstName + "Updated",
                LastName = newMember.LastName + "Updated",
                Email = newMember.Email,
                Roles = new List<string> { "Developer" },
                ParticipantGroups = new List<string>(),
                Hash = newMember.Hash,
                TeamExternalIdentifier = newMember.TeamExternalIdentifier
            };

            var response = await client.PutAsync(RequestUris.BulkMembers(Company.Id), 
                new List<AddMembers> { updatedMember }.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, 
                "Status code doesn't match.");

            var memberResponse = GetTeamMembers(caClient, teamResponse.Uid);
            Assert.AreEqual(updatedMember.FirstName, memberResponse.First().FirstName,
                "FirstName does not match.");
            Assert.AreEqual(updatedMember.LastName, memberResponse.First().LastName,
                "LastName does not match.");
            Assert.AreEqual(updatedMember.Email, memberResponse.First().Email,
                "Email does not match.");
            Assert.AreEqual(updatedMember.ExternalIdentifier, memberResponse.First().ExternalIdentifier,
                "ExternalIdentifier does not match.");
        }

        // 400
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_EmptyValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers>
            {
                new AddMembers
                {
                    FirstName = "",
                    LastName = "",
                    Email = "",
                    TeamExternalIdentifier = ""
                }
            };

            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkMembers(Company.Id), 
                members);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            Assert.That.ListsAreEqual(ExpectedErrorList, response.Dto);
        }

        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_NullValues_BadRequest()
        {
            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers> { new AddMembers() };

            // when
            var response = await client.PutAsync<List<string>>(RequestUris.BulkMembers(Company.Id), 
                members);

            // then
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, 
                "Status code doesn't match.");
            Assert.That.ListsAreEqual(ExpectedErrorList, response.Dto);
        }

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_Unauthorized()
        {
            // given
            var client = GetUnauthenticatedClient();
            var members = new List<AddMembers> { MemberFactory.GetMemberForBulkImport(
                Guid.NewGuid().ToString("D")) };

            // when
            var response = await client.PutAsync(RequestUris.BulkMembers(Company.Id), 
                members.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 403
        [TestMethod]
        [TestCategory("BLAdmin"), TestCategory("TeamAdmin"), TestCategory("OrgLeader")]
        [TestCategory("Member")]
        public async Task Bulk_Members_Put_UserRole_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers> { MemberFactory.GetMemberForBulkImport(
                Guid.NewGuid().ToString("D")) };

            // when
            var response = await client.PutAsync(RequestUris.BulkMembers(Company.Id), 
                members.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        [TestMethod]
        [TestCategory("PartnerAdmin"), TestCategory("CompanyAdmin")]
        public async Task Bulk_Members_Put_Permission_Forbidden()
        {
            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers> { MemberFactory.GetMemberForBulkImport(
                Guid.NewGuid().ToString("D")) };

            // when
            var response = await client.PutAsync(RequestUris.BulkMembers(2), 
                members.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, 
                "Status code doesn't match.");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Bulk_Members_Put_NotFound()
        {
            // given
            var client = await GetAuthenticatedClient();
            var members = new List<AddMembers> { MemberFactory.GetMemberForBulkImport(
                Guid.NewGuid().ToString("D")) };

            // when
            var response = await client.PutAsync(
                RequestUris.BulkMembers(SharedConstants.FakeCompanyId), 
                members.ToStringContent());

            // then
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
                "Status code doesn't match.");
        }
    }
}