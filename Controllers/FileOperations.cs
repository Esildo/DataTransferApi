using DataTransferApi.Entities;
using DataTransferApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO.Compression;
using System.Security.Claims;

namespace DataTransferApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class FileOperations : ControllerBase
    {
        private readonly IConverter _converter;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;
        

        public FileOperations(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IStorageService storageService, IConverter converter)
        {
            _converter = converter;
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

      
        [HttpPost("upload")]
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

        //Get groups of uploaded files
        [HttpGet("searchgroups")]
        public async Task<IActionResult> SearchUserFileGroups()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var userFileGroups = await _storageService.SearchGroupsAsync(userId);
                return Ok(userFileGroups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        //Get list of all files
        [HttpGet("searchfiles")]
        public async Task<IActionResult> SearchUserFiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var userFileGroups = await _storageService.SearchFilesAsync(userId);
                return Ok(userFileGroups);
            }
            catch (Exception ex)
            {
                // Обработка исключения
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("downloadfile")]
        public async Task<IActionResult> DowloadFile(string groupName,string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var fileResult = await _storageService.DownloadFile(groupName, fileName, userId);
                return File(fileResult.Item1, fileResult.Item2, fileResult.Item3);
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        [HttpGet("downloadgroup")]
        public async Task<IActionResult> DownloadFileGroup(string groupName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var groupTuples = await _storageService.DownloadFileGroup(groupName, userId);
                var zipBytes = await _converter.ConvetGrTupZipAsync(groupTuples);
                return File(zipBytes, "application/zip", "group.zip");
                
                //
                //return filesGroup;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
