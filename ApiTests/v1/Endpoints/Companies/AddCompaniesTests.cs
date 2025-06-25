using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Companies
{
    [TestClass]
    [TestCategory("Companies")]
    public class AddCompaniesTests :BaseV1Test
    {
        private static readonly List<AddCompanyRequest> Companies = new List<AddCompanyRequest>();

        // 201
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Post_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            Companies.Add(company);
            company.Name = $"ZZZ_Add{Guid.NewGuid()}";

            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));

            // act
            var response = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);

            // assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status code does not match");
            Assert.AreEqual(company.Name, response.Dto.Name, "Name does not match.");
            Assert.AreEqual(company.CompanyType, response.Dto.Type, "CompanyType does not match.");
            Assert.AreEqual(company.LifeCycleStage, response.Dto.LifeCycleStage,
                "LifeCycleStage does not match.");
            Assert.AreEqual(company.ReferralType, response.Dto.ReferralPartner,
                "ReferralPartner does not match.");
            Assert.AreEqual(company.SubscriptionType, response.Dto.SubscriptionType,
                "SubscriptionType does not match.");
            Assert.IsNull(response.Dto.LastAssessmentDate, "LastAssessmentDate is not null");
            Assert.AreEqual(0, response.Dto.NumberOfAssessments,
                "NumberOfAssessments does not match");
            Assert.AreEqual(0, response.Dto.NumberOfTeams,
                "NumberOfTeams does not match");
            Assert.AreEqual(company.Industry, response.Dto.Industry, "Industry does not match.");
            Assert.AreEqual(company.Size, response.Dto.Size, "Size does not match.");
            Assert.AreEqual(company.Country, response.Dto.Country, "Country does not match.");
            Assert.AreEqual(company.AccountManagerFirstName, response.Dto.AccountManagerFirstName,
                "AccountManagerFirstName does not match.");
            Assert.AreEqual(company.AccountManagerLastName, response.Dto.AccountManagerLastName,
                "AccountManagerLastName does not match.");
            Assert.AreEqual(company.AccountManagerEmail, response.Dto.AccountManagerEmail,
                "AccountManagerEmail does not match.");
            Assert.AreEqual(company.AccountManagerPhone, response.Dto.AccountManagerPhone,
                "AccountManagerPhone does not match.");
            Assert.AreEqual(company.IndustryId, response.Dto.IndustryId, "IndustryId does not match.");
            Assert.AreEqual(company.CompanyPartnerReferral, response.Dto.CompanyPartnerReferral,
                "CompanyPartnerReferral does not match.");
            Assert.AreEqual(company.IsoLanguageCode,response.Dto.IsoLanguageCode,"IsoLanguage code does not match");
            Assert.IsNull(response.Dto.IndividualPartnerReferral, "IndividualPartnerReferral is not null");
            Assert.AreEqual(company.WatchList, response.Dto.WatchList, "WatchList does not match.");
            Assert.AreEqual(company.ReferralType, response.Dto.ReferralType, "ReferralType does not match.");
            Assert.AreEqual(company.TimeZoneInfoId, response.Dto.TimeZoneInfoId,
                "TimeZoneInfoId does not match.");
            Assert.AreEqual(company.IsDraft, response.Dto.IsDraft,
                "IsDraft does not match.");
            Assert.IsNull(response.Dto.Children,
                "Children is not null");
            Assert.AreEqual(company.DateContractSigned?.ToString("s"), response.Dto.DateContractSigned?.ToString("s"),
                "DateContractSigned does not match.");
            Assert.AreEqual(company.ContractEndDate?.ToString("s"), response.Dto.ContractEndDate?.ToString("s"),
                "ContractEndDate does not match.");
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.Dto.SubscriptionHistory),
                "SubscriptionHistory is empty.");
            Assert.AreEqual(company.SessionTimeout, response.Dto.SessionTimeout,
                "SessionTimeout does not match.");
            Assert.AreEqual(company.MaxSessionLength,
                response.Dto.MaxSessionLength,"MaxSessionLength does not match.");
            Assert.AreEqual(company.ForcePasswordUpdate, response.Dto.ForcePasswordUpdate,
                "ForcePasswordUpdate does not match.");
            Assert.AreEqual(company.RequireSecurityQuestions, response.Dto.RequireSecurityQuestions,
                "RequireSecurityQuestions does not match.");
            Assert.AreEqual(company.TwoFactorBuAdmin, response.Dto.TwoFactorBuAdmin,
                "TwoFactorBLAdmin does not match.");
            Assert.AreEqual(company.TwoFactorCompanyAdmin, response.Dto.TwoFactorCompanyAdmin,
                "TwoFactorCompanyAdmin does not match.");
            Assert.AreEqual(company.TwoFactorOrgLeader, response.Dto.TwoFactorOrgLeader,
                "TwoFactorOrgLeader does not match.");
            Assert.AreEqual(company.TwoFactorTeamAdmin, response.Dto.TwoFactorTeamAdmin,
                "TwoFactorTeamAdmin does not match.");
            Assert.AreEqual(company.LogoutUrl, response.Dto.LogoutUrl, "LogoutUrl does not match.");
            Assert.AreEqual(company.AuditTrailRetentionPeriod, response.Dto.AuditTrailRetentionPeriod,
                "AuditTrailRetentionPeriod does not match.");
            Assert.AreEqual(company.TeamsLimit, response.Dto.TeamsLimit, "TeamsLimit does not match.");
            Assert.AreEqual(company.AssessmentsLimit, response.Dto.AssessmentsLimit,
                "AssessmentsLimit does not match.");
            Assert.AreEqual(company.ManagedSubscription, response.Dto.ManagedSubscription,
                "ManagedSubscription does not match.");
            Assert.AreEqual(company.PhotoPath, response.Dto.PhotoPath,
                "PhotoPath does not match");
            Assert.IsTrue(response.Dto.Logourl.Contains("/companylogos/"), "Logourl does not contain '/companylogos/' ");
            Assert.AreEqual(0, response.Dto.CompanyLicenses.First().Used, "License used quantity is not matched");
            Assert.IsTrue(!string.IsNullOrEmpty(response.Dto.CompanyLicenses.First().CompanyLicenseId.ToString()), "Company license id is invalid");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Post_Draft_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = new AddCompanyRequest
            {
                Name = $"ZZZ_Add{Guid.NewGuid()}",
                IsDraft = true,
                CompanyLicenses = new List<CompanyLicenseDto>
                {
                    new CompanyLicenseDto
                    {
                        Origin = "Automation",
                        Key = "Key 1",
                        Quantity = 1
                    }
                }
            };
            Companies.Add(company);
            // act
            var response = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);

            // assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Status code does not match");
            Assert.AreEqual(company.Name, response.Dto.Name, "Name does not match.");
            Assert.AreEqual(company.IsDraft, response.Dto.IsDraft, "IsDraft does not match");
            Assert.IsTrue(response.Dto.CompanyLicenses.First().CompanyLicenseId > 0, "Company license id is invalid");
        }

        // 400
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Post_MissingName_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();

            company.Name = null;

            // act
            var response = await client.PostAsync(RequestUris.Companies(), company.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");

        }

        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Post_LicenseKey_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_AddLicenseKey{Guid.NewGuid()}";

            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));

            company.CompanyLicenses = new List<CompanyLicenseDto>
            {
                new CompanyLicenseDto
                {
                    Origin = "",
                    Key = "",
                    Quantity = -1
                }
            };

            var errorList = new List<string>
            {
                "Key should not be empty",
                "Quantity must be greater than 0"
            };

            // act
            var response = await client.PostAsync<IList<string>>(RequestUris.Companies(), company);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Status code does not match");
            Assert.That.ListsAreEqual(errorList, response.Dto.ToList(), "Error lists do not match");
        }

        // 401
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Post_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_Add{Guid.NewGuid()}";

            // act
            var response = await client.PostAsync(RequestUris.Companies(), company.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code does not match");
        }

        // 403
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Companies_Post_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_Add{Guid.NewGuid()}";

            // act
            var response = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status code does not match");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);

            foreach (var company in Companies)
            {
                setup.DeleteCompany(company.Name).GetAwaiter().GetResult(); 
            }
        }
    }
}
