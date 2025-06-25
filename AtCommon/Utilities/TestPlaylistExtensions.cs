using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtCommon.Utilities
{

    [TestClass]
    public class TestPlaylistExtensions
    {
        private const string Pat = ""; // Enter Your Personal Access Token
        private const string Organization = "agilityhealth-net";
        private const string Project = "Agility Health";
        private readonly HttpClient Client;

        public TestPlaylistExtensions()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{Pat}")));
        }

        private async Task<List<(int Id, string Name)>> GetTestRunsAsync(string buildId)
        {
            var buildUri = $"vstfs:///Build/Build/{buildId}";
            var url = $"https://dev.azure.com/{Organization}/{Uri.EscapeDataString(Project)}/_apis/test/runs?buildUri={Uri.EscapeDataString(buildUri)}&api-version=7.0";
            var response = await Client.GetStringAsync(url);
            var doc = JsonDocument.Parse(response);

            return doc.RootElement.GetProperty("value")
                .EnumerateArray()
                .Select(run => (
                    run.GetProperty("id").GetInt32(),
                    run.GetProperty("name").GetString()))
                .ToList();
        }

        private async Task<List<string>> GetFailedTestsAsync(int runId)
        {
            var url = $"https://dev.azure.com/{Organization}/{Uri.EscapeDataString(Project)}/_apis/test/Runs/{runId}/results?api-version=7.0";
            var response = await Client.GetStringAsync(url);
            var doc = JsonDocument.Parse(response);
            var list = new List<string>();

            foreach (var item in doc.RootElement.GetProperty("value").EnumerateArray())
            {
                var outcome = item.GetProperty("outcome").GetString();
                var name = item.GetProperty("automatedTestName").GetString();
                var storage = item.GetProperty("automatedTestStorage").GetString();

                if (outcome?.ToLower() == "failed" &&
                    !string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(storage))
                {
                    list.Add(name);
                }
            }

            return list;
        }

        private string GeneratePlaylist(List<string> testNames, string fileName = "FailedTests.playlist")
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Playlist Version=\"1.0\">");

            foreach (var test in testNames.Distinct())
            {
                sb.AppendLine($"      <Add Test=\"{test}\" />");
            }

            sb.AppendLine("</Playlist>");
            File.WriteAllText(fileName, sb.ToString());

            return Path.GetFullPath(fileName);
        }

        private void WriteFailedTestsToFile(Dictionary<string, List<string>> failedTestsByRun, string fileName = "FailedCases.txt")
        {
            var sb = new StringBuilder();

            foreach (var entry in failedTestsByRun.OrderBy(kv => kv.Key))
            {
                sb.AppendLine($"[{entry.Key}]");

                var methodNames = entry.Value
                    .Select(name => name.Split('.').Last())
                    .Distinct();

                foreach (var method in methodNames)
                {
                    sb.AppendLine(method);
                }

                sb.AppendLine();
            }

            File.WriteAllText(fileName, sb.ToString());
            var fullPath = Path.GetFullPath(fileName);

            Console.WriteLine($"Failed test methods written to: {fullPath}");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not open the file automatically: {ex.Message}");
            }
        }

        public async Task GenerateFailedTestPlaylistAsync(string buildId)
        {
            Console.WriteLine("Getting test runs for build ID: " + buildId);
            var testRuns = await GetTestRunsAsync(buildId);

            if (!testRuns.Any())
            {
                Console.WriteLine("No test runs found for this build.");
                return;
            }

            var allFailedTests = new HashSet<string>();
            var failedTestsByRunName = new Dictionary<string, List<string>>();

            foreach (var (id, name) in testRuns)
            {
                Console.WriteLine($"Checking run: {name} (ID: {id})");
                var failedTests = await GetFailedTestsAsync(id);

                if (failedTests.Any())
                {
                    failedTestsByRunName[name] = failedTests;

                    foreach (var test in failedTests)
                        allFailedTests.Add(test);
                }
            }

            if (!allFailedTests.Any())
            {
                Console.WriteLine("No failed tests found.");
                return;
            }

            Console.WriteLine($"Total unique failed tests: {allFailedTests.Count}");
            var playlistPath = GeneratePlaylist(allFailedTests.ToList());
            WriteFailedTestsToFile(failedTestsByRunName);
            Console.WriteLine($"Playlist generated: {playlistPath}");
        }

        [TestMethod]
        public async Task GenerateFailedTestPlaylist()
        {
            const string buildId = ""; // Enter Your Build ID
            await GenerateFailedTestPlaylistAsync(buildId);
        }
    }
}
