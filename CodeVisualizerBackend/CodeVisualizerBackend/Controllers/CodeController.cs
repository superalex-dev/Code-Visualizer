using Microsoft.AspNetCore.Mvc;
using Code_Visualizer_Backend.Services;

namespace Code_Visualizer_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public CodeController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("list-files")]
        public async Task<IActionResult> ListFilesFromGitHub(string repoUrl)
        {
            var result = await _gitHubService.GetRepositoryFiles(repoUrl);
            if (result != null && result.Count > 0)
            {
                return Ok(result); // The result is now a List<Dictionary<string, string>>
            }
            return NotFound("No files found or an error occurred.");
        }

        [HttpGet("get-code")]
        public async Task<IActionResult> GetCodeFromGitHub(string fileUrl)
        {
            var content = await _gitHubService.GetFileContent(fileUrl);
            return content != null ? Ok(new { content }) : BadRequest("Error fetching file content.");
        }
    }
}