using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class FaultColletor
    {
        static FaultColletor _instance;
        public static FaultColletor Instance => _instance ??= new FaultColletor();

        readonly List<(string message, Exception ex)> faults;
        readonly HashSet<string> labels;

        private string ghToken;
        private string userName;
        private string repoName;

        FaultColletor()
        {
            faults = new List<(string, Exception)>();
            labels = new HashSet<string>(new[] { "bug" });
        }

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

                int faultCount = 0;
                foreach (var fault in faults)
                {
                    bodyBuilder.AppendLine($"<b>{++faultCount})</b> {fault.message}");
                    bodyBuilder.AppendLine($"```{fault.ex}\n```");
                    Console.WriteLine();
                }

                var postData = new
                {
                    title = $"Fuel-Georgia-Parser faulted with {faults.Count} error{(faults.Count > 1 ? "s" : "")}",
                    labels = labels,
                    body = bodyBuilder.ToString(),
                };

                var postJson = System.Text.Json.JsonSerializer.Serialize(postData);

                var resp = await client.PostAsync($"https://api.github.com/repos/{userName}/{repoName}/issues", new StringContent(postJson, Encoding.UTF8, "application/json"));
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
