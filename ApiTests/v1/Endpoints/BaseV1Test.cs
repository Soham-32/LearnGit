using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.Dtos.Radars;
using AtCommon.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints
{
    [TestClass]
    public class BaseV1Test
    {
        protected static TestEnvironment TestEnvironment;
        protected static User User;
        protected static User InsightsUser;
        protected static Company Company;
        private static int _individualAssessmentSurveyId;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            TestEnvironment = new TestEnvironment(context);
            User = TestEnvironment.UserConfig.GetUserByDescription("user 1");

            if (!User.IsCompanyAdmin() && TestEnvironment.UseOauth) throw new ArgumentException("Oauth can only be used with user 'CA'");
            
            //SiteAdmin has one user "user 1"
            InsightsUser = User.IsSiteAdmin() || User.IsPartnerAdmin()
                ? User : TestEnvironment.UserConfig.GetUserByDescription("insights user");
            Company = TestEnvironment.UserConfig.GetCompanyByEnvironment(TestEnvironment.EnvironmentName);

            //Clearing up Environment
            if (context.Properties["CleanCompany"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                //Database clean up
                try
                {
                    DataBaseUtil.CleanCompanyForAutomation(Company.Id, TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Database clean up failed. Exception - " + e.Message);
                }
            }

            if (TestEnvironment.Parameters["DeleteTestCompanies"].ToString().ToLower() == "true"
                && !string.IsNullOrEmpty(TestEnvironment.DatabaseConnectionString))
            {
                try
                {
                    DataBaseUtil.DeleteTestCompanies(TestEnvironment.DatabaseConnectionString);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Database test company clean up failed. Exception - " + e.Message);
                }
            }

            // try to get an api token
            try
            {
                if (TestEnvironment.UseOauth)
                {
                    ClientFactory
                        .GetOauthClient(Company.GetOauthApp("Automation"), TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
                }
                else
                {
                    ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName)
                        .GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Unable to get token for first login");
            }

        }

        protected static void VerifySetup(bool classInitializationFailed)
        {
            if (classInitializationFailed) throw new Exception("ClassInitialize was not successful. Aborting test");
        }

        protected int GetIndividualSurveyId()
        {
            if (_individualAssessmentSurveyId != 0) return _individualAssessmentSurveyId;
            var client = GetAuthenticatedClient().GetAwaiter().GetResult();
            var surveyResponse = client.GetAsync<IList<RadarDetailResponse>>(RequestUris.RadarsDetails(Company.Id)).GetAwaiter().GetResult();
            surveyResponse.EnsureSuccess();
            _individualAssessmentSurveyId = surveyResponse.Dto.First(r => r.Name == SharedConstants.IndividualAssessmentType).CheckForNull().SurveyId;
            return _individualAssessmentSurveyId;
        }

        #region Auth Clients
        public async Task<HttpClient> GetAuthenticatedClient()
        {
            if (TestEnvironment.UseOauth)
            {
                var oauthApp = Company.GetOauthApp("Automation");
                return ClientFactory.GetOauthClient(oauthApp, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            }
            return await ClientFactory.GetAuthenticatedClient(User.Username, User.Password, TestEnvironment.EnvironmentName);
        }

        public async Task<HttpClient> GetInsightsAuthenticatedClient()
        {
            if (TestEnvironment.UseOauth)
            {
                var oauthApp = Company.GetOauthApp("Automation_Insights");
                return ClientFactory.GetOauthClient(oauthApp, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            }
            return await ClientFactory.GetAuthenticatedClient(InsightsUser.Username, InsightsUser.Password, TestEnvironment.EnvironmentName);
        }

        protected HttpClient GetUnauthenticatedClient()
        {
            return ClientFactory.GetUnauthenticatedClient(TestEnvironment.EnvironmentName);
        }

        public HttpClient GetAuthenticatedScimClient()
        {
            
            return ClientFactory.GetAuthenticatedScimClient(Company.ScimToken, Company.Environment);
        }

        public HttpClient GetUnauthenticatedScimClient()
        {

            return ClientFactory.GetAuthenticatedScimClient("invalidToken", Company.Environment);
        }

        protected static HttpClient GetCaClient()
        {
            if (TestEnvironment.UseOauth)
            {
                var oauthApp = Company.GetOauthApp("Automation");
                return ClientFactory.GetOauthClient(oauthApp, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
            }
            var companyAdmin = new UserConfig("CA").GetUserByDescription("user 1");
            return ClientFactory.GetAuthenticatedClient(companyAdmin.Username, 
                companyAdmin.Password, TestEnvironment.EnvironmentName).GetAwaiter().GetResult();
        }
        #endregion

    }
}