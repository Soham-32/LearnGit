using System;
using System.Collections.Generic;
using System.Net.Http;
using AtCommon.Api;
using AtCommon.Dtos.Teams;
using AtCommon.ObjectFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiTests.v1.Endpoints.Bulk
{
    [TestClass]
    public class BulkBaseTest : BaseV1Test
    {

        protected static TeamResponse GetTeamForBulk(HttpClient client, AddTeamWithMemberRequest request)
        {
            var createTeamResponse = client.PostAsync<TeamResponse>(
                RequestUris.Teams(), request).GetAwaiter().GetResult();
            createTeamResponse.EnsureSuccess();

            return createTeamResponse.Dto;
        }

        protected static IList<TeamMemberResponse> GetTeamMembers(HttpClient client, Guid teamUid)
        {
            var memberResponse = client.GetAsync<IList<TeamMemberResponse>>(
                RequestUris.TeamMembers(teamUid)).GetAwaiter().GetResult();
            memberResponse.EnsureSuccess();
            return memberResponse.Dto;
        }

        protected static TeamResponse CreateMultiteam(HttpClient client = null)
        {
            client ??= GetCaClient();
            
            var multiTeamResponse = client.PostAsync<TeamResponse>(RequestUris.Teams(), TeamFactory.GetMultiTeam("AddSubteams")).GetAwaiter().GetResult();
            multiTeamResponse.EnsureSuccess();

            return multiTeamResponse.Dto;
        }
    }
}