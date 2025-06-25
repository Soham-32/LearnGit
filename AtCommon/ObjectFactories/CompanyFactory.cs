using System;
using System.Collections.Generic;
using AtCommon.Dtos.Companies;
using AtCommon.Utilities;

namespace AtCommon.ObjectFactories
{
    public static class CompanyFactory
    {
        public static AddCompanyRequest GetCompany(string companyNamePrefix = "ZZZ", string partnerReferralCompany = "", int licenses = 1)
        {
            var guid = Guid.NewGuid();
            var companyLicenses = new List<CompanyLicenseDto>();

            for (int i = 0; i < licenses; i++)
            {
                companyLicenses.Add(
                    new CompanyLicenseDto
                    {
                        Key = Guid.NewGuid().ToString(),
                        Origin = $"Team Create Automation Test {i + 1}",
                        Quantity = 1
                    });
            }

            return new AddCompanyRequest
            {
                Name = $"{companyNamePrefix}{RandomDataUtil.GetCompanyName()}",
                Country = "Angola",
                Size = "< 1K",
                Industry = "Finance",
                IndustryId = 6,
                TimeZoneInfoId = "UTC",
                LifeCycleStage = "Prospect",
                AccountManagerFirstName = "Account",
                AccountManagerLastName = "Manager",
                AccountManagerEmail = $"ah_automation+AccountManager{guid}@agiletransformation.com",
                AccountManagerPhone = "402-555-1234",
                CompanyType = "Customer",
                WatchList = false,
                ReferralType = "Company",
                CompanyPartnerReferral = partnerReferralCompany,
                SubscriptionType = "Annual",
                AssessmentsLimit = 100,
                TeamsLimit = 10,
                DateContractSigned = DateTime.Today,
                ContractEndDate = DateTime.Today.AddDays(7),
                SessionTimeout = 10,
                MaxSessionLength = 60,
                ForcePasswordUpdate = 0,
                RequireSecurityQuestions = true,
                TwoFactorCompanyAdmin = true,
                TwoFactorOrgLeader = true,
                TwoFactorTeamAdmin = true,
                TwoFactorBuAdmin = true,
                LogoutUrl = "www.test.us",
                AuditTrailRetentionPeriod = 30,
                CompanyAdminFirstName = $"AT{guid}",
                CompanyAdminLastName = SharedConstants.TeamMemberLastName,
                CompanyAdminEmail = $"ah_automation+CA{guid}@agiletransformation.com",
                IsDraft = false,
                ManagedSubscription = true,
                PhotoPath = "https://test.us/photo.jpeg",
                Logourl = "",
                IsoLanguageCode ="English",
                CompanyLicenses = companyLicenses
            };
        }

        public static AddCompanyRequest GetCompanyForUpdate(string companyNamePrefix = "ZZZ")
        {
            var guid = Guid.NewGuid();

            return new AddCompanyRequest
            {
                Name = $"{companyNamePrefix}{RandomDataUtil.GetCompanyName()}",
                SubscriptionType = "Pilot",
                IndustryId = 8,
                CompanyPartnerReferral = null,
                IndividualPartnerReferral = "Larry Tate",
                WatchList = true,
                ReferralType = "individual",
                TimeZoneInfoId = "Central Standard Time",
                Country = "Antarctica",
                Size = "1K - 5K",
                Industry = "Healthcare",
                LifeCycleStage = "Pilot",
                AccountManagerFirstName = "Auto1",
                AccountManagerLastName = "Tester1",
                AccountManagerEmail = "auto.tester@fake.us",
                AccountManagerPhone = "402-555-6789",
                CompanyType = "Partner",
                AssessmentsLimit = 500,
                TeamsLimit = 25,
                DateContractSigned = DateTime.Today.AddDays(-7),
                ContractEndDate = DateTime.Today.AddDays(14),
                SessionTimeout = 30,
                MaxSessionLength = 120,
                ForcePasswordUpdate = 0,
                RequireSecurityQuestions = false,
                TwoFactorCompanyAdmin = false,
                TwoFactorOrgLeader = false,
                TwoFactorTeamAdmin = false,
                TwoFactorBuAdmin = false,
                LogoutUrl = "www.agilityinsights.ai",
                AuditTrailRetentionPeriod = 10,
                CompanyAdminFirstName = $"AT{RandomDataUtil.GetFirstName()}",
                CompanyAdminLastName = SharedConstants.TeamMemberLastName,
                CompanyAdminEmail = $"ah_automation+CA{guid}@agiletransformation.com",
                ManagedSubscription = false,
                IsDraft = false,
                PhotoPath = "https://test.us/photo2.jpeg",
                Logourl = "",
                IsoLanguageCode = "Spanish",
                CompanyLicenses = new List<CompanyLicenseDto>
                {
                    new CompanyLicenseDto
                    {
                        Key = Guid.NewGuid().ToString(),
                        Origin = "Update Automation Test",
                        Quantity = 2
                    }
                }

            };
        }


        public static AddCompanyRequest GetValidPostCompany()
        {
            return new AddCompanyRequest
            {
                Name = $"ZZZPost{RandomDataUtil.GetCompanyName()}",
                CompanyType = "Customer",
                LifeCycleStage = "Prospect",
                SubscriptionType = "Pilot",
                Industry = "Finance",
                Size = "< 1K",
                Country = "United States",
                AccountManagerFirstName = "Auto",
                AccountManagerLastName = "Tester",
                AccountManagerEmail = "auto.tester@fake.us",
                AccountManagerPhone = "402-123-5555",
                IndustryId = 6,
                CompanyPartnerReferral = "dave",
                WatchList = true,
                ReferralType = "Company",
                TimeZoneInfoId = "Central Standard Time",
                TeamsLimit = 1,
                DateContractSigned = DateTime.UtcNow,
                ContractEndDate = DateTime.UtcNow.AddDays(7),
                SessionTimeout = 60,
                MaxSessionLength = 15,
                ForcePasswordUpdate = 0,
                RequireSecurityQuestions = true,
                TwoFactorBuAdmin = true,
                TwoFactorCompanyAdmin = true,
                TwoFactorOrgLeader = true,
                TwoFactorTeamAdmin = true,
                LogoutUrl = "https://test.com",
                AuditTrailRetentionPeriod = 2,
                IsDraft = false,
                AssessmentsLimit = 15,
                ManagedSubscription = false,
                CompanyAdminFirstName = RandomDataUtil.GetFirstName(),
                CompanyAdminLastName = SharedConstants.TeamMemberLastName,
                CompanyAdminEmail = $"ca_{RandomDataUtil.GetFirstName():N}@test.us",
                PhotoPath = "photopath",
                Logourl = "",
                IsoLanguageCode = "eng",
                CompanyLicenses = new List<CompanyLicenseDto>
                {
                    new CompanyLicenseDto
                    {
                        Key = Guid.NewGuid().ToString(),
                        Origin = "Automation Test",
                        Quantity = 1
                    }
                }
            };
        }

        public static UpdateCompanyRequest GetValidPutCompany()
        {
            return new UpdateCompanyRequest
            {
                Name = $"ZZZPut{RandomDataUtil.GetCompanyName()}",
                CompanyType = "Partner",
                LifeCycleStage = "Active",
                SubscriptionType = "Pilot",
                Industry = "Agile Consulting",
                Size = "10K - 50K",
                Country = "United States",
                AccountManagerFirstName = "Derederick",
                AccountManagerLastName = "Tatum",
                AccountManagerEmail = "d.tatum@fake.us",
                IndustryId = 18,
                IndividualPartnerReferral = "Some Guy",
                WatchList = false,
                ReferralType = "Individual",
                TimeZoneInfoId = "Eastern Standard Time",
                AssessmentsLimit = 50,
                DateContractSigned = DateTime.UtcNow,
                ContractEndDate = DateTime.UtcNow.AddDays(7),
                SessionTimeout = 30,
                MaxSessionLength = 60,
                ForcePasswordUpdate = 0,
                RequireSecurityQuestions = true,
                TwoFactorBuAdmin = true,
                TwoFactorCompanyAdmin = true,
                TwoFactorOrgLeader = true,
                TwoFactorTeamAdmin = true,
                LogoutUrl = "https://test2.com",
                AuditTrailRetentionPeriod = 5,
                IsDraft = false,
                TeamsLimit = 15,
                ManagedSubscription = true,
                CompanyAdminFirstName = $"CA{RandomDataUtil.GetFirstName():N}",
                CompanyAdminLastName = SharedConstants.TeamMemberLastName,
                CompanyAdminEmail = $"ca_{RandomDataUtil.GetFirstName():N}@test.us",
                PhotoPath = "photopath2",
                Logourl = "",
                IsoLanguageCode = "dut",
                CompanyLicenses = new List<CompanyLicenseDto>
                {
                    new CompanyLicenseDto
                    {
                        Key = Guid.NewGuid().ToString(),
                        Origin = "Update Automation Test",
                        Quantity = 1
                    }
                }
            };
        }

        public static GetCompanyFeaturesRequest GetValidGetCompanyFeaturesRequest()
        {
            return new GetCompanyFeaturesRequest
            {
                FeatureIds = new List<int> { 80, 81, 82, 83 }
            };
        }
    }
}