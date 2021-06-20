using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    internal class FaultColletor
    {
        private static FaultColletor _instance;

        private readonly List<(string message, Exception ex)> faults;
        private readonly HashSet<string> labels;

        private string ghToken;
        private string repoName;
        private string userName;

        private FaultColletor()
        {
            faults = new List<(string, Exception)>();
            labels = new HashSet<string>(new[] {"bug"});
        }

        public static FaultColletor Instance => _instance ??= new FaultColletor();

        public void Register(string message, Exception ex, params string[] labels)
        {
            lock (this)
            {
                faults.Add((message, ex));
                foreach (var label in labels)
                    this.labels.Add(label);
            }
        }


        public void Init(string ghToken, string userName, string repoName)
        {
            this.ghToken = ghToken;
            this.userName = userName;
            this.repoName = repoName;
        }

        public async Task UploadAsync()
        {
            if (faults.Any())
            {
                Console.WriteLine($"Uploading {faults.Count} faults");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"token {ghToken}");
                client.DefaultRequestHeaders.Add("User-Agent", "GitHubAction.Net5");

                StringBuilder bodyBuilder = new();

                var faultCount = 0;
                foreach (var fault in faults)
                {
                    bodyBuilder.AppendLine($"<b>{++faultCount})</b> {fault.message}");
                    bodyBuilder.AppendLine($"```{fault.ex}\n```");
                    Console.WriteLine();
                }

                var postData = new
                {
                    title = $"Fuel-Georgia-Parser faulted with {faults.Count} error{(faults.Count > 1 ? "s" : "")}",
                    labels,
                    body = bodyBuilder.ToString()
                };

                var postJson = JsonSerializer.Serialize(postData);

                var resp = await client.PostAsync($"https://api.github.com/repos/{userName}/{repoName}/issues",
                    new StringContent(postJson, Encoding.UTF8, "application/json"));
                resp.EnsureSuccessStatusCode();
                Console.WriteLine("Faults uploaded");
            }
            else
            {
                Console.WriteLine("No faults registered");
            }
        }
    }
}