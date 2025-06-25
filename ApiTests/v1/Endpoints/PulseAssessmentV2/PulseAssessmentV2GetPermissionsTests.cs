using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Assessments.PulseV2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.PulseAssessmentV2
{
    [TestClass]
    [TestCategory("PulseAssessmentsV2")]
    public class PulseAssessmentV2GetPermissionsTests : PulseApiTestBase
    {
        //200
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_Permissions_Success()
        {
            //Given
            var client = await GetAuthenticatedClient();

            //When
            var response = await client.GetAsync<PulseAssessmentPermissionsV2Response>(RequestUris.PulseAssessmentV2Permissions());

            //Then
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code does not match");
            Assert.IsTrue(response.Dto.CanView, "User does not have 'view pulse assessment' permission");

            if (User.IsMember())
            {
                Assert.IsFalse(response.Dto.CanCreate, "User does have 'create pulse assessment' permission");
                Assert.IsFalse(response.Dto.CanEdit, "User does have 'edit pulse assessment' permission");
                Assert.IsFalse(response.Dto.CanDelete, "User does have 'delete pulse assessment' permission");
            }
            else if (User.IsOrganizationalLeader())
            {
                Assert.IsTrue(response.Dto.CanCreate, "User does not have 'create pulse assessment' permission");
                Assert.IsFalse(response.Dto.CanEdit, "User does have 'edit pulse assessment' permission");
                Assert.IsFalse(response.Dto.CanDelete, "User does have 'delete pulse assessment' permission");
            }
            else
            {
                Assert.IsTrue(response.Dto.CanCreate, "User does not have 'create pulse assessment' permission");
                Assert.IsTrue(response.Dto.CanEdit, "User does not have 'edit pulse assessment' permission");
                Assert.IsTrue(response.Dto.CanDelete, "User does not have 'delete pulse assessment' permission");
            }
        }

        //401
        [TestMethod]
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        [TestCategory("CompanyAdmin"), TestCategory("TeamAdmin"), TestCategory("BLAdmin"), TestCategory("OrgLeader"), TestCategory("Member")]
        public async Task PulseAssessmentV2_Get_Permissions_Unauthorized()
        {
            //Given
            var client = GetUnauthenticatedClient();

            //When
            var response = await client.GetAsync<PulseAssessmentPermissionsV2Response>(RequestUris.PulseAssessmentV2Permissions());

            //Then
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }
    }
}
