using DataTransferApi.Entities;
using DataTransferApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataTransferApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class FileOperations : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;

        public FileOperations(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IStorageService storageService)
        {
            _storageService = storageService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return BadRequest("Пользователь не найден");
            }
           

            return Ok(new { message = $"Here {userId}" });
        }

        [Authorize]
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(files == null)
            {
                return BadRequest();
            }
            try 
            {
                await _storageService.SaveToStorageAsync(files, userName, userId);
                return Ok();
            }
            catch (Exception ex) 
            {
                throw;
            }

        }

    }
}
