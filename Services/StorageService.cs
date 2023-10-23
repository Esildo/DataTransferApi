using DataTransferApi.Db;
using DataTransferApi.Heppers;
using DataTransferApi.Entities;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Web;
using static Azure.Core.HttpHeader;
using DataTransferApi.HeppersService;

namespace DataTransferApi.Services
{

    //add saveChanges async
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _appDbContext;
        private readonly ICommandDBService _efHelper;

        public StorageService(IWebHostEnvironment env, AppDbContext appDbContext, ICommandDBService efHelper)
        {
            _env = env;
            _appDbContext = appDbContext;
            _efHelper = efHelper;
        }



        //Save group of files in file directory
        public async Task SaveToStorageAsync(List<IFormFile> files, string userName, string userId)
        {
            try
            {
                string fileStoragePath = Path.Combine(_env.ContentRootPath, "FileStorage");


                if (!Directory.Exists(fileStoragePath))
                {
                    Directory.CreateDirectory(fileStoragePath);
                }

                string userDirectory = Path.Combine(fileStoragePath, userName);
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }
                int groupNumber = GetNextGroupNumber(userDirectory);

                string groupDirectory = Path.Combine(userDirectory, $"group{groupNumber}");
                if (!Directory.Exists(groupDirectory))
                {
                    Directory.CreateDirectory(groupDirectory);
                }

                var fileGroup = new FileGroup()
                {
                    Name = $"group{groupNumber}"
                };
                _appDbContext.Add(fileGroup);
                _appDbContext.SaveChanges();

                foreach (var file in files)
                {
                    string fullPath = Path.Combine(groupDirectory, file.FileName);

                    //Need to change if filename exists 
                    fullPath = GetFullPath(groupDirectory, fullPath, file, out string correctName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var saveFile = new SavedFile
                    {
                        SavedFileName = correctName,
                        SavedFilePath = fullPath,
                        ExpectedSize = file.Length,
                        UserId = userId,
                        FileGroupId = fileGroup.Id
                    };
                    _appDbContext.Add(saveFile);
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }

        //Search all user files 
        public async Task<IEnumerable<SavedFile>> SearchFilesAsync(string userId)
        {
            var userFiles = await _appDbContext.SavedFiles.Where(u => u.UserId == userId).ToArrayAsync();

            return userFiles;
        }

        //Search all user groups
        public async Task<IEnumerable<FileGroup>> SearchGroupsAsync(string userId)
        {
            var groups = await _appDbContext.SavedFiles.Where(file => file.UserId == userId)
                                                            .Select(f => f.FileGroup)
                                                            .Distinct()
                                                            .ToArrayAsync();

            return groups;
        }

        //Dowload group of files
        public async Task<IEnumerable<(byte[], string, string)>> DownloadFileGroup(string groupName, string userId)
        {
            try
            {
                var fileArray = await _efHelper.GetFileGroupAsync(groupName, userId);
                if(!fileArray.Any())
                {
                    throw new Exception($"There are not {groupName}");
                }

                foreach(var file in fileArray)
                {
                    if(CheckLoad(file) != 100)
                    {
                        //handle
                        throw new Exception("not full load");
                    }
                }

                List<(byte[], string, string)> listFileTuples = new List<(byte[], string, string)>();
                foreach (var file in fileArray)
                {
                    var fileTuple = await _efHelper.ReturnFileTupleAsync(file.SavedFilePath);
                    listFileTuples.Add(fileTuple);
                }
                return listFileTuples;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }


        //Download single file
        public async Task<(byte[], string, string)> DownloadFile(string groupName, string fileName, string userId)
        {
            try
            {
                //Select file from db
                var savedFile = await _efHelper.FileByNameAsync(groupName, fileName, userId);
                if(CheckLoad(savedFile) != 100)
                {
                    //handle
                    throw new Exception("Not full file");
                }


                //Need to handle 
                if (savedFile.SavedFilePath == null)
                {
                    throw (new Exception("file not found"));
                }
                
                var fileTuple = await _efHelper.ReturnFileTupleAsync(savedFile.SavedFilePath);
                return fileTuple;            
            }
            //handle
            catch(Exception ex) 
            {
                throw;
            }
        }
            
        
        public async Task<int> LoadPercGroupAsync(string groupName,string userId)
        {
            int i;
            int sum;

            i = 0;
            sum = 0;
            var fileGroup = await _efHelper.GetFileGroupAsync(groupName, userId);

            foreach (var file in fileGroup) 
            {
                sum += CheckLoad(file);
                i++;
            }
            return sum / i;
        }

        
        public async Task<int> LoadPercAsync(string groupName,string fileName, string userId)
        {

            try
            {
                var savedFile = await _efHelper.FileByNameAsync(groupName, fileName, userId);

                return CheckLoad(savedFile);

            }
            //handle
            catch(Exception ex)
            {
                throw;
            }
        }

            
        private int CheckLoad(SavedFile savedFile) 
        {
            if (File.Exists(savedFile.SavedFilePath))
            {
                FileInfo fileInfo = new FileInfo(savedFile.SavedFilePath);
                long fileLength = fileInfo.Length;

                return (int)((fileLength / savedFile.ExpectedSize) * 100);
            }
            else
            {
                //handle
                throw new Exception("File Path not found");
            }
        }



        //Method to create puth for group  
        private int GetNextGroupNumber(string userDirectory)
        {
            int groupNumber = 1;
            while (Directory.Exists(Path.Combine(userDirectory, $"group{groupNumber}")))
            {
                groupNumber++;
            }
            return groupNumber;
        }

   
        //Method to create fullpath for file and return  new file name 
        private string GetFullPath(string groupDerictory, string curPath,IFormFile file, out string newFileName)
        {
            int count = 1;
            //for safety GetFileName
            newFileName = Path.GetFileName(file.FileName);
            while(File.Exists(curPath))
            {
                newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}({count}){Path.GetExtension(file.FileName)}";
                curPath = Path.Combine(groupDerictory, newFileName);
                count++;
            }
            return curPath;
        }
    }

}
