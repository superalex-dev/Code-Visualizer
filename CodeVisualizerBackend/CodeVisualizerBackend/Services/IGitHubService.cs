using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Code_Visualizer_Backend.Services;

public interface IGitHubService
{
    Task<List<Dictionary<string, string>>> GetRepositoryFiles(string repoUrl, string path = "");
    Task<string> GetFileContent(string fileUrl);
}