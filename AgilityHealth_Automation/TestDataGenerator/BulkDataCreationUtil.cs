using System;
using AtCommon.Api;
using AtCommon.Dtos;
using AtCommon.ObjectFactories;
using System.Collections.Generic;
using AgilityHealth_Automation.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgilityHealth_Automation.TestDataGenerator
{
    [TestClass]
    public class BulkDataCreationUtil : BaseTest 
    {
        private static SetupTeardownApi _setupApi;

        /*
         * This test case dynamically generates bulk test data by creating multiple Teams, MultiTeams, and EnterpriseTeams using the API.
         * It sets up a hierarchical structure by adding subteams under MultiTeams and sub MultiTeams under EnterpriseTeams to facilitate comprehensive testing scenarios.
         * Do configure configuration object as per your need.
         * Note: Adjust the 'TestTimeout' in the "testrunsettings" file according to your requirements, If you need to create more than 100 teams. 
         */

        // Enable this as needed when creating multiple sets of test data.
        //[TestMethod] 
        public void GenerateTestData()                         
        {
                var configuration = new BulkDataDetailsDto()
                {
                    // Set number of Teams
                    NoOfTeams = 1,

                    // Set number of MultiTeams
                    NoOfMt = 1,

                    // Set number of EnterpriseTeams
                    NoOfEt = 1,

                    // Set starting number of Team e.g. you may want to start from 50 e.g. Team50 then put 50 as value.
                    TeamStartingIndexNumber = 1,

                    // Set starting number of MultiTeam e.g. you may want to start from 50 e.g. MultiTeam50 then put 50 as value.
                    MtStartingIndexNumber = 1,

                    // Set starting number of MultiTeam e.g. you may want to start from 50 e.g. EnterpriseTeam50 then put 50 as value.
                    EtStartingIndexNumber = 1,

                    //Set number of team members per team
                    NoOfTeamMembersPerTeam = 0,

                    // Add CA user detail for the company you want to create test data
                    User = new User("", ""),

                    //Set prefixes for the 'Team', 'MultiTeam' and 'EnterpriseTeam' as per need.
                    TeamPrefix = "TestDataTeam ",
                    MultiTeamPrefix = "TestDataMultiTeam ",
                    EnterpriseTeamPrefix = "TestDataEnterpriseTeam ",
                };

                // Initialize lists to store the unique identifiers (UIDs) of created Teams and MultiTeams
                var teamList = new List<Guid>();
                var multiTeamList = new List<Guid>();

                // Variables to track the index for fetching UIDs from the lists.
                int teamNo = 0;
                int mtNo = 0;

                // Set the number of teams, multi teams, enterprise teams, and subteams to be created.
                // These counts can be adjusted as needed for the bulk data hierarchy.
                int subteamCount = configuration.NoOfTeams/ configuration.NoOfMt;  //subTeamPerMt
                int subMtCount = configuration.NoOfMt/configuration.NoOfEt;   // subMtPerEt

                // Initialize the API setup class with the test environment.
                _setupApi = new SetupTeardownApi(TestEnvironment);
            
                // Create the specified number of teams (with team members and stakeholders).
                for (int i = 0; i < configuration.NoOfTeams; i++)
                {
                    // Create a new team with a unique name specified number of team members and stakeholders.
                    var team = TeamFactory.GetNormalTeam(configuration.TeamPrefix + configuration.TeamStartingIndexNumber + " ", configuration.NoOfTeamMembersPerTeam);

                    // Call the API to create the team using the provided user credentials and wait for the result.
                    var teamResponse = _setupApi.CreateTeam(team, configuration.User).GetAwaiter().GetResult();

                    // Add the newly created team's unique identifier (UID) to the list of teams.
                    teamList.Add(teamResponse.Uid);

                    // Increment the team index to ensure that the next team has a unique number in its name.
                    configuration.TeamStartingIndexNumber = configuration.TeamStartingIndexNumber + 1; 
                }

                // Create the specified number of MultiTeams and add subteams to them.
                if (teamList.Count == configuration.NoOfTeams)
                {
                    for (int j = 0; j < configuration.NoOfMt; j++)
                    {
                        // Create a new MultiTeam with a unique name.
                        var multiTeam1 = TeamFactory.GetMultiTeam(configuration.MultiTeamPrefix + configuration.MtStartingIndexNumber + " ");

                        // Call the API to create the MultiTeam using the provided user credentials, and wait for the result. 
                        var multiTeamResponse1 = _setupApi.CreateTeam(multiTeam1, configuration.User).GetAwaiter().GetResult();

                        // Add the newly created MultiTeam's unique identifier (UID) to the list of MultiTeams.
                        multiTeamList.Add(multiTeamResponse1.Uid);

                        // Add the specified number of subteams to each MultiTeam.
                        for (int k = 0; k < subteamCount; k++)
                        {
                            // Assign a subteam to the current MultiTeam by calling the API with the team's UID from teamList.
                            _setupApi.AddSubteams(multiTeamResponse1.Uid, new List<Guid> { teamList[teamNo] }, configuration.User).GetAwaiter().GetResult();

                            // Increment the team index to assign the next team as a subteam in the next iteration.
                            teamNo++; 
                        }

                        // Increment the MultiTeam index to ensure that the next MultiTeam gets a unique name.
                        configuration.MtStartingIndexNumber = configuration.MtStartingIndexNumber + 1; 
                    }
                }

                // Create the specified number of EnterpriseTeams and add sub MultiTeams to them
                if (multiTeamList.Count == configuration.NoOfMt)
                {
                    for (int x = 0; x < configuration.NoOfEt; x++)
                    {
                        // Create a new EnterpriseTeam with a unique name.
                        var enterpriseTeam = TeamFactory.GetEnterpriseTeam(configuration.EnterpriseTeamPrefix + configuration.EtStartingIndexNumber + " ");

                        // Call the API to create the EnterpriseTeam using the provided user credentials, and wait for the result 
                        var enterpriseTeamResponse = _setupApi.CreateTeam(enterpriseTeam, configuration.User).GetAwaiter().GetResult(); 

                        // Add the specified number of sub MultiTeams to each EnterpriseTeam.
                        for (int y = 0; y < subMtCount; y++)
                        {
                            // Assign a sub MultiTeam to the current EnterpriseTeam by calling the API with the MultiTeam's UID from multiTeamList.
                            _setupApi.AddSubteams(enterpriseTeamResponse.Uid,
                                    new List<Guid> { multiTeamList[mtNo] }, configuration.User).GetAwaiter().GetResult();

                            // Increment the MultiTeam index to assign the next MultiTeam as a subteam in the next iteration.
                            mtNo++; 
                        }

                        // Increment the EnterpriseTeam index to ensure that the next EnterpriseTeam gets a unique name.
                        configuration.EtStartingIndexNumber = configuration.EtStartingIndexNumber + 1; 
                    }
                }
        }
    }
    
}



