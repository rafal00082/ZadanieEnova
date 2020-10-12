using GitHubApi.Models.Branch;
using GitHubApi.Models.Commit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubApi.Api
{
    public class GitHubClient
    {
        private readonly HttpClient _httpClient;
        private readonly string user;
        private readonly string repo;


        public GitHubClient(string url, string user, string repo)
        {
            this.user = user;
            this.repo = repo;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("anything", "1"));
        }

        public async Task<List<Branch>> GetBranches()
        {
            var response = await _httpClient.GetStringAsync($"repos/{user}/{repo}/branches");
            var result = JsonConvert.DeserializeObject<List<Branch>>(response);
            return result;
        }

        public async Task<CommitInfo[]> GetCommits(string sha)
        {
            var response = await _httpClient.GetStringAsync($"repos/{user}/{repo}/commits?per_page=100&sha={sha}");
            var result = JsonConvert.DeserializeObject<CommitInfo[]>(response);
            return result;
        }
    }
}
