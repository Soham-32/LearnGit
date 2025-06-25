using System.Collections.Generic;
using System.Linq;
using AtCommon.Dtos.OAuth;
using AtCommon.Utilities;

namespace AtCommon.Dtos
{
    public class Company
    {
        public string Environment { get; set; }
        public int Id { get; set; }
        public string TeamId1 { get; set; }
        public string ScimToken { get; set; }
        public int InsightsId => 121;
        public string InsightsCompany => "Automation_Insights (DO NOT USE)";
        public int NtierId => 773;
        public List<AddAppRegistrationResponse> OauthApps { get; set; }
        
        public AddAppRegistrationResponse GetOauthApp(string appName)
        {
            return OauthApps.FirstOrDefault(a => a.AppName == appName).CheckForNull($"AppName <{appName}> not found.");
        }
    }

}
