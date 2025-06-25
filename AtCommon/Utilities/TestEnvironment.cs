using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Net;

namespace AtCommon.Utilities
{
    public class TestEnvironment
    {

        public IDictionary Parameters { get; set; }
        public UserConfig UserConfig { get; set; }
        public string EnvironmentName { get; set; }
        public string UserCode { get; set; }
        public string Browser { get; set; }
        public string DatabaseConnectionString { get; set; }
        public TimeSpan JsTimeout { get; set; }
        public bool UseOauth { get; set;  }
        public bool NewNav { get; set; }
        public string ProdEmail { get; set; }
        public string ProdPassword { get; set; }
        public string PowerBiEmail { get; set; }
        public string PowerBiPassword { get; set; }
        public string ProdDatabaseConnectionString { get; set; }

        public TestEnvironment(TestContext context)
        {
            Parameters = context.Properties;
            string password =$"\"{WebUtility.HtmlDecode(Parameters["DatabasePassword"]?.ToString())}\"";


            // normal run - get the configuration from the .runsettings file
            UserCode = Parameters["User"].ToString().ToUpper();
            EnvironmentName = Parameters["Env"].ToString().ToLower();
            Browser = Parameters["Browser"].ToString().ToLower();
            JsTimeout = TimeSpan.FromSeconds(double.Parse(Parameters["JsTimeout"].ToString()));
            UseOauth = bool.TryParse(Parameters["UseOauth"].ToString().ToLower(), out var useOauth) && useOauth;
            NewNav = bool.TryParse(Parameters["NewNav"].ToString().ToLower(), out var newNav) && newNav;

            UserConfig = new UserConfig(UserCode, NewNav);
            DatabaseConnectionString = DataBaseUtil.GetDatabaseConnectionString(EnvironmentName,
                password.ToString());
            ProdEmail = Parameters["ProdEmail"]?.ToString();
            ProdPassword = Parameters["ProdPassword"]?.ToString();
            PowerBiEmail = Parameters["PowerBiEmail"]?.ToString();
            PowerBiPassword = Parameters["PowerBiPassword"]?.ToString();
            ProdDatabaseConnectionString = Parameters["DatabasePassword"].ToString();
        }

    }
}