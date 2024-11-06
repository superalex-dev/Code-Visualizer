using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Code_Visualizer_Backend.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubService> _logger;

        public GitHubService(string token, ILogger<GitHubService> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            _logger = logger;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<Dictionary<string, string>>> GetRepositoryFiles(string repoUrl, string path = "")
        {
            try
            {
                string apiUrl = $"https://api.github.com/repos/{GetOwnerAndRepo(repoUrl)}/contents/{path}";
                _logger.LogInformation("Fetching repository contents from {ApiUrl}", apiUrl);

                var response = await _httpClient.GetStringAsync(apiUrl);
                _logger.LogInformation("Raw response: {Response}", response);

                var parsedResponse = JArray.Parse(response);
                var fileList = new List<Dictionary<string, string>>();

                foreach (var item in parsedResponse)
                {
                    if (item != null)
                    {
                        var type = item["type"]?.ToString();
                        var fileEntry = new Dictionary<string, string>
                        {
                            { "name", item["name"]?.ToString() },
                            { "path", item["path"]?.ToString() },
                            { "type", type },
                            { "download_url", item["download_url"]?.ToString() ?? "" }
                        };

                        fileList.Add(fileEntry);

                        // If the item is a directory, fetch its contents recursively
                        if (type == "dir")
                        {
                            var subDirContents = await GetRepositoryFiles(repoUrl, item["path"]?.ToString());
                            if (subDirContents != null)
                            {
                                fileList.AddRange(subDirContents);
                            }
                        }
                    }
                }

                return fileList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching repository files from {RepoUrl}", repoUrl);
                return null;
            }
        }

        private string GetOwnerAndRepo(string repoUrl)
        {
            var segments = repoUrl.TrimEnd('/').Split('/');
            var owner = segments[^2];
            var repo = segments[^1];
            return $"{owner}/{repo}";
        }


        public async Task<string> GetFileContent(string fileUrl)
        {
            try
            {
                _logger.LogInformation("Fetching file content from {FileUrl}", fileUrl);
                var response = await _httpClient.GetStringAsync(fileUrl);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching file content from {FileUrl}", fileUrl);
                return null;
            }
        }
    }
}