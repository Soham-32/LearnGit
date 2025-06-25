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
    public class UpdateCompaniesTests : BaseV1Test
    {
        private static readonly List<string> Companies = new List<string>();
        // 200
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_Update{Guid.NewGuid()}";
            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));
            
            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(
                RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();

            var companyId = postCompanyResponse.Dto.Id;

            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"));
            updatedCompany.Name = $"ZZZ_Update{Guid.NewGuid()}";
            var putCompanyResponse = await client.PutAsync<CompanyResponse>(RequestUris.CompanyDetails(companyId), 
                updatedCompany);
            Companies.Add(updatedCompany.Name);
            // assert
            Assert.AreEqual(HttpStatusCode.OK, putCompanyResponse.StatusCode, 
                "Status Code does not match.");
            Assert.AreEqual(updatedCompany.Name, putCompanyResponse.Dto.Name, 
                "Name does not match.");
            Assert.AreEqual(updatedCompany.CompanyType, putCompanyResponse.Dto.Type, 
                "CompanyType does not match.");
            Assert.AreEqual($"{updatedCompany.AccountManagerFirstName} {updatedCompany.AccountManagerLastName}",
                putCompanyResponse.Dto.AccountManager, "AccountManager does not match");
            Assert.AreEqual(updatedCompany.LifeCycleStage, putCompanyResponse.Dto.LifeCycleStage, 
                "LifeCycleStage does not match.");
            Assert.AreEqual(updatedCompany.ReferralType, putCompanyResponse.Dto.ReferralPartner, 
                "ReferralPartner does not match.");
            Assert.AreEqual(updatedCompany.SubscriptionType, putCompanyResponse.Dto.SubscriptionType, 
                "SubscriptionType does not match.");
            Assert.IsNull(putCompanyResponse.Dto.LastAssessmentDate, "LastAssessmentDate is not null");
            Assert.AreEqual(0, putCompanyResponse.Dto.NumberOfAssessments, 
                "NumberOfAssessments does not match");
            Assert.AreEqual(0, putCompanyResponse.Dto.NumberOfTeams, 
                "NumberOfTeams does not match");
            Assert.AreEqual(updatedCompany.Industry, putCompanyResponse.Dto.Industry, 
                "Industry does not match.");
            Assert.AreEqual(updatedCompany.Size, putCompanyResponse.Dto.Size, 
                "Size does not match.");
            Assert.AreEqual(updatedCompany.Country, putCompanyResponse.Dto.Country, 
                "Country does not match.");
            Assert.AreEqual(updatedCompany.IsoLanguageCode, putCompanyResponse.Dto.IsoLanguageCode,
                "IsoLanguage Code does not match.");
            Assert.AreEqual(updatedCompany.AccountManagerFirstName, 
                putCompanyResponse.Dto.AccountManagerFirstName, "AccountManagerFirstName does not match.");
            Assert.AreEqual(updatedCompany.AccountManagerLastName, 
                putCompanyResponse.Dto.AccountManagerLastName, "AccountManagerLastName does not match.");
            Assert.AreEqual(updatedCompany.AccountManagerEmail, 
                putCompanyResponse.Dto.AccountManagerEmail, "AccountManagerEmail does not match.");
            Assert.AreEqual(updatedCompany.AccountManagerPhone, 
                putCompanyResponse.Dto.AccountManagerPhone, "AccountManagerPhone does not match.");
            Assert.AreEqual(updatedCompany.IndustryId, putCompanyResponse.Dto.IndustryId, 
                "IndustryId does not match.");
            Assert.IsNull(putCompanyResponse.Dto.CompanyPartnerReferral, 
                "CompanyPartnerReferral is not null");
            Assert.AreEqual(updatedCompany.IndividualPartnerReferral, 
                putCompanyResponse.Dto.IndividualPartnerReferral, 
                "IndividualPartnerReferral does not match.");
            Assert.AreEqual(updatedCompany.WatchList, putCompanyResponse.Dto.WatchList, 
                "WatchList does not match.");
            Assert.AreEqual(updatedCompany.ReferralType, putCompanyResponse.Dto.ReferralType, 
                "ReferralType does not match.");
            Assert.AreEqual(updatedCompany.TimeZoneInfoId, putCompanyResponse.Dto.TimeZoneInfoId, 
                "TimeZoneInfoId does not match.");
            Assert.AreEqual(updatedCompany.IsDraft, putCompanyResponse.Dto.IsDraft, 
                "IsDraft does not match.");
            Assert.IsNull(putCompanyResponse.Dto.Children, 
                "Children is not null");
            Assert.AreEqual(updatedCompany.DateContractSigned?.ToString("s"), 
                putCompanyResponse.Dto.DateContractSigned?.ToString("s"), 
                "DateContractSigned does not match.");
            Assert.AreEqual(updatedCompany.ContractEndDate?.ToString("s"), 
                putCompanyResponse.Dto.ContractEndDate?.ToString("s"), 
                "ContractEndDate does not match.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(putCompanyResponse.Dto.SubscriptionHistory),
                "SubscriptionHistory is empty.");
            Assert.AreEqual(updatedCompany.SessionTimeout, putCompanyResponse.Dto.SessionTimeout, 
                "SessionTimeout does not match.");
            Assert.AreEqual(updatedCompany.MaxSessionLength, 
                putCompanyResponse.Dto.MaxSessionLength,"MaxSessionLength does not match.");
            Assert.AreEqual(updatedCompany.ForcePasswordUpdate, 
                putCompanyResponse.Dto.ForcePasswordUpdate, "ForcePasswordUpdate does not match.");
            Assert.AreEqual(updatedCompany.RequireSecurityQuestions, 
                putCompanyResponse.Dto.RequireSecurityQuestions, 
                "RequireSecurityQuestions does not match.");
            Assert.AreEqual(updatedCompany.TwoFactorBuAdmin, putCompanyResponse.Dto.TwoFactorBuAdmin, 
                "TwoFactorBLAdmin does not match.");
            Assert.AreEqual(updatedCompany.TwoFactorCompanyAdmin, 
                putCompanyResponse.Dto.TwoFactorCompanyAdmin, "TwoFactorCompanyAdmin does not match.");
            Assert.AreEqual(updatedCompany.TwoFactorOrgLeader, 
                putCompanyResponse.Dto.TwoFactorOrgLeader, "TwoFactorOrgLeader does not match.");
            Assert.AreEqual(updatedCompany.TwoFactorTeamAdmin, 
                putCompanyResponse.Dto.TwoFactorTeamAdmin, "TwoFactorTeamAdmin does not match.");
            Assert.AreEqual(updatedCompany.LogoutUrl, putCompanyResponse.Dto.LogoutUrl, 
                "LogoutUrl does not match.");
            Assert.AreEqual(updatedCompany.AuditTrailRetentionPeriod, 
                putCompanyResponse.Dto.AuditTrailRetentionPeriod, 
                "AuditTrailRetentionPeriod does not match.");
            Assert.AreEqual(updatedCompany.TeamsLimit, putCompanyResponse.Dto.TeamsLimit,
                "TeamsLimit does not match.");
            Assert.AreEqual(updatedCompany.AssessmentsLimit, putCompanyResponse.Dto.AssessmentsLimit,
                "AssessmentsLimit does not match.");
            Assert.AreEqual(updatedCompany.ManagedSubscription, 
                putCompanyResponse.Dto.ManagedSubscription,"ManagedSubscription does not match.");
            Assert.AreEqual(updatedCompany.PhotoPath, putCompanyResponse.Dto.PhotoPath, 
                "PhotoPath does not match");
            Assert.IsTrue(putCompanyResponse.Dto.Logourl.Contains("/companylogos/"), "Logourl does not contain '/companylogos/' ");
            Assert.AreEqual(0, putCompanyResponse.Dto.CompanyLicenses.First().Used, "License used quantity doesn't match");
            Assert.IsTrue(putCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId > 0, "Company license id is invalid");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId, putCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId, "License keys do not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_Draft_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = new AddCompanyRequest
            {
                Name = $"ZZZ_Update{Guid.NewGuid()}",
                IsDraft = true
            };
            
            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();

            var companyId = postCompanyResponse.Dto.Id;
            var updatedCompany = new UpdateCompanyRequest
            {
                Name = company.Name,
                Country = "Angola",
                IsDraft = true
            };
            Companies.Add(updatedCompany.Name);
            var putCompanyResponse = await client.PutAsync<CompanyResponse>(RequestUris.CompanyDetails(companyId), 
                updatedCompany);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, putCompanyResponse.StatusCode, "Status Code does not match.");
            Assert.AreEqual(updatedCompany.Name, putCompanyResponse.Dto.Name, "Name does not match.");
            Assert.AreEqual(updatedCompany.Country, putCompanyResponse.Dto.Country, "Country does not match.");
            Assert.AreEqual(updatedCompany.IsDraft, putCompanyResponse.Dto.IsDraft, 
                "IsDraft does not match.");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_UpdateLicenseKey_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_UpdateLicenseKey{Guid.NewGuid()}";
            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));

            var postCompanyResponse = await client.PostAsync<CompanyResponse>(
                RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();
            
            var companyId = postCompanyResponse.Dto.Id;

            // act
            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.CompanyLicenses.First().CompanyLicenseId = postCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId;
            updatedCompany.CompanyLicenses.First().Key = postCompanyResponse.Dto.CompanyLicenses.First().Key;
            var putCompanyResponse = await client.PutAsync<CompanyResponse>(RequestUris.CompanyDetails(companyId),
                updatedCompany);
            Companies.Add(updatedCompany.Name);
            // assert
            Assert.AreEqual(HttpStatusCode.OK, putCompanyResponse.StatusCode,
                "Status Code does not match.");
            Assert.AreEqual(1, putCompanyResponse.Dto.CompanyLicenses.Count, "Company License count doesn't match");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Key, putCompanyResponse.Dto.CompanyLicenses.First().Key, "License keys do not match");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Quantity, putCompanyResponse.Dto.CompanyLicenses.First().Quantity, "License quantities do not match");
            Assert.AreEqual(updatedCompany.CompanyLicenses.First().Origin, putCompanyResponse.Dto.CompanyLicenses.First().Origin, "License origins do not match");
            Assert.IsTrue(putCompanyResponse.Dto.CompanyLicenses.First().Used >= 0, "License used quantity incorrect");
            Assert.IsTrue(putCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId > 0, "Company license id is invalid");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Key, putCompanyResponse.Dto.CompanyLicenses.First().Key, "License keys do not match");
        }

        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_ExistingCompany_UpdateLicenseKey_Success()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_UpdateLicenseKey{Guid.NewGuid()}";
            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));

            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(
                RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();

            var companyId = postCompanyResponse.Dto.Id;

            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.CompanyLicenses.First().CompanyLicenseId = postCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId;
            updatedCompany.CompanyLicenses.First().Key = postCompanyResponse.Dto.CompanyLicenses.First().Key;
            var putCompanyResponse = await client.PutAsync<CompanyResponse>(RequestUris.CompanyDetails(companyId),
                updatedCompany);
            Companies.Add(updatedCompany.Name);
            // assert
            Assert.AreEqual(HttpStatusCode.OK, putCompanyResponse.StatusCode,
                "Status Code does not match.");
            Assert.AreEqual(1, putCompanyResponse.Dto.CompanyLicenses.Count, "Company License count doesn't match");
            Assert.IsTrue(putCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId > 0, "Company license id is invalid");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Key, putCompanyResponse.Dto.CompanyLicenses.First().Key, "License keys do not match");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Quantity, putCompanyResponse.Dto.CompanyLicenses.First().Quantity, "License quantities do not match");
            Assert.AreEqual(updatedCompany.CompanyLicenses.First().Origin, putCompanyResponse.Dto.CompanyLicenses.First().Origin, "License origins do not match");
            Assert.IsTrue(putCompanyResponse.Dto.CompanyLicenses.First().Used >= 0, "License used quantity incorrect");
            Assert.AreEqual(postCompanyResponse.Dto.CompanyLicenses.First().Key, putCompanyResponse.Dto.CompanyLicenses.First().Key, "License keys do not match");
        }

        // 400
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_MissingName_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_Update{Guid.NewGuid()}";

            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();
            Companies.Add(postCompanyResponse.Dto.Name);
            var companyId = postCompanyResponse.Dto.Id;

            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.Name = null;
            var putCompanyResponse = await client.PutAsync<IList<string>>(RequestUris.CompanyDetails(companyId), updatedCompany);

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putCompanyResponse.StatusCode, "Statuc Code does not match.");
            Assert.AreEqual("'Name' must not be empty.", putCompanyResponse.Dto.FirstOrDefault(), "response error message does not match");
        }

        // 400
        [TestMethod]
        [TestCategory("KnownDefect")] // Bug : 48243
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_MissingLicenseKey_BadRequest()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var company = CompanyFactory.GetValidPostCompany();
            company.Name = $"ZZZ_Update{Guid.NewGuid()}";
            var cSharpHelper = new CSharpHelpers();
            company.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\1.jpg"));

            // act
            var postCompanyResponse = await client.PostAsync<CompanyResponse>(
                RequestUris.Companies(), company);
            postCompanyResponse.EnsureSuccess();
            Companies.Add(postCompanyResponse.Dto.Name);
            var companyId = postCompanyResponse.Dto.Id;

            var getCompanyResponse = await client.GetAsync<CompanyResponse>(RequestUris.CompanyDetails(postCompanyResponse.Dto.Id));
            getCompanyResponse.EnsureSuccess();

            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.Logourl = cSharpHelper.ImageToBase64(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestData\\2.jpg"));
            updatedCompany.CompanyLicenses.First().CompanyLicenseId = getCompanyResponse.Dto.CompanyLicenses.First().CompanyLicenseId;
            updatedCompany.CompanyLicenses = new List<CompanyLicenseDto>
            {
                new CompanyLicenseDto
                {
                    Origin = "",
                    Key = "",
                    Quantity = -1
                }
            };
            var putCompanyResponse = await client.PutAsync<IList<string>>(RequestUris.CompanyDetails(companyId), 
                updatedCompany);

            var errorResponse = new List<string>
            {
                "Key should not be empty",
                "Quantity must be greater than 0"
            };

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putCompanyResponse.StatusCode, "Status Code does not match.");
            Assert.That.ListsAreEqual(errorResponse, putCompanyResponse.Dto.ToList());
        } 

        // 401
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("SiteAdmin"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_Unauthorized()
        {
            // arrange
            var client = GetUnauthenticatedClient();
            const int companyId = 150;
            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.Name = $"ZZZ_Update{Guid.NewGuid()}";
            //act
            var response =
                await client.PutAsync(RequestUris.CompanyDetails(companyId), updatedCompany.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status Code does not match.");
        }

        // 403
        [TestMethod]
        [TestCategory("Rocket"), TestCategory("CompanyAdmin"), TestCategory("BLAdmin")]
        [TestCategory("Member"), TestCategory("OrgLeader"), TestCategory("TeamAdmin")]
        public async Task Companies_Put_UserRole_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();
            var companiesResponse = await client.GetAsync<IList<BaseCompanyResponse>>(RequestUris.CompaniesListCompanies());
            var company = companiesResponse.Dto.First();
            var updatedCompany = new UpdateCompanyRequest {Name = $"{company.Name}Updated"};

            // act
            var response =
                await client.PutAsync(RequestUris.CompanyDetails(company.Id), updatedCompany.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");

        }

        [TestMethod]
        [TestCategory("Rocket"), TestCategory("PartnerAdmin")]
        public async Task Companies_Put_NoCompanyAccess_Forbidden()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            const int companyId = 2;
            var updatedCompany = new UpdateCompanyRequest {AccountManagerFirstName = "Tester"};

            // act
            var response =
                await client.PutAsync(RequestUris.CompanyDetails(companyId), updatedCompany.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "Status Code does not match.");
        }

        // 404
        // BUG 28983
        //[TestMethod]
        //[TestCategory("Rocket"), TestCategory("SiteAdmin")]
        public async Task Companies_Put_NotFound()
        {
            // arrange
            var client = await GetAuthenticatedClient();

            const int companyId = 999999999;
            var updatedCompany = CompanyFactory.GetValidPutCompany();
            updatedCompany.Name = $"ZZZ_Update{Guid.NewGuid()}";

            // act
            var response = await client.PutAsync(RequestUris.CompanyDetails(companyId), updatedCompany.ToStringContent());

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Status Code does not match.");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);

            foreach (var company in Companies)
            {
                setup.DeleteCompany(company).GetAwaiter().GetResult(); 
            }
        }
    }
}
