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
        private readonly IConverterGroup _converter;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;
        private readonly IOneTokenService _oneTokenService;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1);



        public FileOperations(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IStorageService storageService
                        , IConverterGroup converter, IOneTokenService oneTokenService)
        {
            _oneTokenService = oneTokenService;
            _converter = converter;
            _storageService = storageService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpGet("data")]
        //public async Task<IActionResult> GetData()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (userId == null)
        //    {
        //        return BadRequest("Пользователь не найден");
        //    }
           

        //    return Ok(new { message = $"Here {userId}" });
        //}

      
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

        [HttpGet("downloadfile")]
        public async Task<IActionResult> DowloadFile(string groupName,string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var tupleResult = await _storageService.DownloadFile(groupName, fileName, userId);
                return File(tupleResult.Item1, tupleResult.Item2, tupleResult.Item3);
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
                var tupleResult = await _converter.ConvetGrTupleAsync(groupTuples);
                return File(tupleResult.Item1, tupleResult.Item2,tupleResult.Item3);
                
                //
                //return filesGroup;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("onelinkfile")]
        public async Task<IActionResult> GetLinkFile(string groupName, string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var tokenGet = await _oneTokenService.CreateOneFileTokenAsync(groupName,fileName,userId);
                string link = Url.Action("DownloadFileLink", "FileOperations", new {token = tokenGet }, Request.Scheme);
                return Ok(link);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("onelinkgroup")]
        public async Task<IActionResult> GetLinkGroup(string groupName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var tokenGet = await _oneTokenService.CreateOneGroupTokenAsync(groupName, userId);
                string link = Url.Action("DownloadFileLink", "FileOperations", new { token = tokenGet }, Request.Scheme);
                return Ok(link);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [AllowAnonymous]
        [HttpGet("dowlowdanon")]
        public async Task<IActionResult> DownloadFileLink(string token)
        {
            try
            {
                await _tokenLock.WaitAsync();

                int countToken = await _oneTokenService.TokenCheck(token);
                if (countToken == 1)
                {
                    var fileTuple = await _oneTokenService.DownloadFileToken(token);
                    return File(fileTuple.Item1, fileTuple.Item2, fileTuple.Item3);
                }
                else
                {
                    var groupTuples = await _oneTokenService.DownLoadFileGroupAsync(token);
                    var tupleResult = await _converter.ConvetGrTupleAsync(groupTuples);
                    return File(tupleResult.Item1, tupleResult.Item2, tupleResult.Item3);
                }
                
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            finally
            {
                // Важно освободить мьютекс, чтобы другие запросы могли получить доступ
                _tokenLock.Release();
            }
        }


        [HttpGet("checkload")]
        public async Task<IActionResult> CheckLoadFile(string groupName, string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                int percatage= await _storageService.LoadPercAsync(groupName, fileName, userId);
                string percStr = $"{percatage} upload {fileName}";
                return Ok(percStr);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("checkgroupload")]
        public async Task<IActionResult> CheckLoadGroup(string groupName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                int percatage = await _storageService.LoadPercGroupAsync(groupName , userId);
                string percStr = $"{percatage} upload {groupName}";
                return Ok(percStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
