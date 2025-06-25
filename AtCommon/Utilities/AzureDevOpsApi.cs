using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Analytics.Custom;
using Newtonsoft.Json;

namespace AtCommon.Utilities
{
    public class AzureDevOpsApi
    {
        private const string ApiUrl = "https://dev.azure.com/AgilityHealth-Net/Agility%20Health/_apis";

        public static async Task<Dictionary<string, string>> GetConfigurationValues(string configurationId, string personalAccessToken)
        {

            string responseString;
            using (var client = GetClient(personalAccessToken))
            {
                
                using var response = await client.GetAsync(
                    $"test/configurations/{configurationId}?api-version=5.0-preview.2");
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
            }
            
            dynamic responseJson = JsonConvert.DeserializeObject(responseString);
            var dict = new Dictionary<string, string>();

            if (responseJson == null) return dict;
            foreach (var item in responseJson.values)
            {
                dict.Add(item.name.ToString(), item.value.ToString());
            }

            return dict;
        }

        public static async Task<List<ProductionEnvironment>> GetProductionEnvironments(string personalAccessToken)
        {
            using var client = GetClient(personalAccessToken);
            var response = await client.GetAsync<WikiResponse>("_apis/wiki/wikis/Agility%20Health.Wiki/pages/148?includeContent=true");
            response.EnsureSuccess();
            var environments = response.Dto.Content.SplitLines().Skip(2).Select(e => ProductionEnvironment.Parse(e.ToString())).ToList();
            
            return environments;
        }

        private static HttpClient GetClient(string personalAccessToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(ApiUrl)
            };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

            return client;
        }

    }

    internal class WikiResponse
    {
        public string Content { get; set; }
    }
}
