using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Teams
{
    [TestClass]
    public class TeamSubteamsBase : BaseV1Test
    {
        protected static async Task<TeamResponse> CreateMultiteam(HttpClient client = null)
        {
            client ??= GetCaClient();
            
            var multiTeamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), TeamFactory.GetMultiTeam("AddSubteams"));
            multiTeamResponse.EnsureSuccess();

            return multiTeamResponse.Dto;
        }

        protected static async Task<List<string>> GetTeamUidsForSubteams(int numberOfTeams, HttpClient client = null)
        {
            client ??= GetCaClient();
            var teamUids = new List<string>();

            for (var i = 0; i < numberOfTeams; i++)
            {
                var teamResponse = await client.PostAsync<TeamResponse>(RequestUris.Teams(), TeamFactory.GetNormalTeam("Subteam"));
                teamResponse.EnsureSuccess();
                teamUids.Add(teamResponse.Dto.Uid.ToString("D"));
            }
            
            return teamUids;
        }
    }
}