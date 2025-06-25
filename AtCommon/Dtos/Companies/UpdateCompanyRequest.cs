using System;
using System.Collections.Generic;

namespace AtCommon.Dtos.Companies
{
    public class UpdateCompanyRequest
    {
        public string Name { get; set; }
        public string CompanyType { get; set; }
        public string LifeCycleStage { get; set; }
        public string ReferralPartner { get; set; }
        public string SubscriptionType { get; set; }    
        public string Industry { get; set; }
        public string Size { get; set; }
        public string Country { get; set; }
        public string AccountManagerFirstName { get; set; }
        public string AccountManagerLastName { get; set; }
        public string AccountManagerEmail { get; set; }
        public int? IndustryId { get; set; }
        public string CompanyPartnerReferral { get; set; }
        public string IndividualPartnerReferral { get; set; }
        public bool WatchList { get; set; }
        public string ReferralType { get; set; }
        public string TimeZoneInfoId { get; set; }
        public DateTime? DateContractSigned { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string SubscriptionHistory { get; set; }
        public int SessionTimeout { get; set; }
        public int MaxSessionLength { get; set; }
        public int ForcePasswordUpdate { get; set; }
        public bool RequireSecurityQuestions { get; set; }
        public bool TwoFactorCompanyAdmin { get; set; }
        public bool TwoFactorTeamAdmin { get; set; }
        public bool TwoFactorOrgLeader { get; set; }
        public bool TwoFactorBuAdmin { get; set; }
        public string LogoutUrl { get; set; }
        public int? AuditTrailRetentionPeriod { get; set; }
        public bool IsDraft { get; set; }
        public int TeamsLimit { get; set; }
        public int AssessmentsLimit { get; set; }
        public bool ManagedSubscription { get; set; }
        public string AccountManagerPhone { get; set; }
        public string CompanyAdminFirstName { get; set; }
        public string CompanyAdminLastName { get; set; }
        public string CompanyAdminEmail { get; set; }
        public string PhotoPath { get; set; }
        public string Logourl { get; set; }
        public string IsoLanguageCode { get; set; }
        public string PreviousLogurl { get; set; }
        public List<CompanyLicenseDto> CompanyLicenses{ get; set; }
    }
}
