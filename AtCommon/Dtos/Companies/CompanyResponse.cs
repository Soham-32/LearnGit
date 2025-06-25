using System;
using System.Collections.Generic;
using AtCommon.Dtos.Teams;

namespace AtCommon.Dtos.Companies
{
    public class CompanyResponse : BaseCompanyResponse
    {

        public string Type { get; set; }
        public string AccountManager { get; set; }
        public string LifeCycleStage { get; set; }
        public string ReferralPartner { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime? LastAssessmentDate { get; set; }
        public int NumberOfAssessments { get; set; }
        public int NumberOfTeams { get; set; }
        public string Industry { get; set; }
        public string Size { get; set; }
        public string Country { get; set; }
        public string AccountManagerFirstName { get; set; }
        public string AccountManagerLastName { get; set; }
        public string AccountManagerEmail { get; set; }
        public string AccountManagerPhone { get; set; }
        public int? IndustryId { get; set; }
        public string CompanyPartnerReferral { get; set; }
        public string IndividualPartnerReferral { get; set; }
        public bool WatchList { get; set; }
        public string ReferralType { get; set; }
        public string TimeZoneInfoId { get; set; }
        public bool IsDraft { get; set; }
        public ICollection<TeamResponse> Children { get; set; }
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
        public int TeamsLimit { get; set; }
        public int AssessmentsLimit { get; set; }
        public bool ManagedSubscription { get; set; }
        public string PhotoPath { get; set; }
        public string Logourl { get; set; }
        public string IsoLanguageCode { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public string CompanyAdmin { get; set; }
        public List<CompanyLicenseResponse> CompanyLicenses { get; set; }
    }
}