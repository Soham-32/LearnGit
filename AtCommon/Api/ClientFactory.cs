using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AtCommon.Dtos.Account;
using AtCommon.Dtos.OAuth;

namespace AtCommon.Api
{
    public class ClientFactory
    {
        private const string BaseV1Uri = "https://api-{0}.agilityinsights.ai/";
        private const string BaseScimUri = "https://scim-{0}.agilityinsights.ai/";
        private static readonly Dictionary<string, string> TokensDictionary = new Dictionary<string, string>();

        public static async Task<HttpClient> GetAuthenticatedClient(string userName, string password, string environment)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(BaseV1Uri, environment))
            };

            // get the user's token from the dictionary. if this user doesn't have a key in the dictionary,
            // or if there is a key but no value, get a new token
            if (!TokensDictionary.TryGetValue(userName, out var token) || string.IsNullOrWhiteSpace(token))
            {
                var credentials = new LoginDto
                {
                    Email = userName,
                    Password = password
                };

                var response = await client.PostAsync<string>(RequestUris.Login(), credentials);
                response.EnsureSuccess();
                token = response.Dto;
                
                // add the new token to the dictionary
                TokensDictionary[userName] = token;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return client;
        }

        public static HttpClient GetAuthenticatedScimClient(string token, string environment)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(BaseScimUri, environment))
            };

            client.DefaultRequestHeaders.Add("X-Agility-Forwarded-Host", $"{environment}agilityinsights.ai");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public static async Task<HttpClient> GetOauthClient(AddAppRegistrationResponse oauthApp, string environmentName)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(BaseV1Uri, environmentName))
            };

            // get the user's token from the dictionary. if this user doesn't have a key in the dictionary,
            // or if there is a key but no value, get a new token
            if (!TokensDictionary.TryGetValue(oauthApp.ClientId, out var token) || string.IsNullOrWhiteSpace(token))
            {
                
                var query = new Dictionary<string, object>
                {
                    { "clientId", oauthApp.ClientId },
                    { "secret", oauthApp.Secret }
                };
                
                var response = await client.PostAsync<OauthTokenResponse>(RequestUris.OauthToken().AddQueryParameter(query));
                response.EnsureSuccess();
                token = response.Dto.Token;
                
                // add the new token to the dictionary
                TokensDictionary[oauthApp.ClientId] = token;
            }
            client.DefaultRequestHeaders.Add("X-App-Registration", oauthApp.AppName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return client;
        }
        
        public static async Task<HttpClient> GetAuthenticatedClientWithNewToken(string userName, string password, string environment)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(BaseV1Uri, environment))
            };

            var creds = new LoginDto
            {
                Email = userName,
                Password = password
            };

            var response = await client.PostAsync<string>(RequestUris.Login(), creds);
            response.EnsureSuccess();
            var token = response.Dto;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public static HttpClient GetUnauthenticatedClient(string environment)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(string.Format(BaseV1Uri, environment))
            };
        }
    }
}