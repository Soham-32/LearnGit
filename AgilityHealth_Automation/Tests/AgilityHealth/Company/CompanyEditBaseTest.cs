using AgilityHealth_Automation.Base;
using AtCommon.Api;
using AtCommon.Dtos.Companies;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.Tests.AgilityHealth.Company
{
    [TestClass]
    public class CompanyEditBaseTest : BaseTest
    {
        
        protected static AddCompanyRequest CreateCompany(bool isDraft = false)
        {
            var companyRequest = CompanyFactory.GetValidPostCompany();
            companyRequest.IsDraft = isDraft;
            var setUp = new SetupTeardownApi(TestEnvironment);
            setUp.CreateCompany(companyRequest).GetAwaiter().GetResult();
            return companyRequest;
        }

        //[ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void ClassTearDown(string companyName)
        {
            if (!User.IsSiteAdmin() && !User.IsPartnerAdmin()) return;
            var setup = new SetupTeardownApi(TestEnvironment);
            setup.DeleteCompany(companyName).GetAwaiter().GetResult();
        }

    }
}