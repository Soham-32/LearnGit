using System;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.Dtos.Tags;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Tags
{
    [TestClass]
    [TestCategory("Tags")]
    public class UpdateTagsAddMasterTagsToCompanyTests : BaseV1Test
    {
        private static AddCompanyRequest _companyRequest;
        private const int FakeCompanyId = 999999999;

        // 401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Tags_AddMasterTagsToCompany_Put_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();

            // act
            var response = await client.PutAsync(
                RequestUris.TagsAddMasterTagsToCompany(FakeCompanyId), string.Empty.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Tags_AddMasterTagsToCompany_Put_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            // act
            var response = await client.PutAsync(
                RequestUris.TagsAddMasterTagsToCompany(FakeCompanyId), string.Empty.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        // 404
        [TestMethod]
        [TestCategory("SiteAdmin")]
        public async Task Tags_AddMasterTagsToCompany_Put_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            
            // act
            var response = await client.PutAsync(
                RequestUris.TagsAddMasterTagsToCompany(FakeCompanyId), string.Empty.ToStringContent());


            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code does not match.");
        }

        // 409
        [TestMethod]
        [TestCategory("KnownDefect")]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Tags_AddMasterTagsToCompany_Put_Conflict()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            _companyRequest = CompanyFactory.GetValidPostCompany();
            _companyRequest.Name = $"ZZZ_UpdateTags{Guid.NewGuid()}";

            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), _companyRequest);
            postCompanyResponse.EnsureSuccess();
            var companyId = postCompanyResponse.Dto.Id;

            var response = await client.PutAsync<CopyMasterTagsResponse>(
                RequestUris.TagsAddMasterTagsToCompany(companyId), string.Empty.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode, "Status Code does not match.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (User.Type != UserType.SiteAdmin && User.Type != UserType.PartnerAdmin) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(_companyRequest.Name).GetAwaiter().GetResult();
        }

    }
}
