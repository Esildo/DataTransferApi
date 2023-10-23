using DataTransferApi.Db;
using DataTransferApi.Entities;
using DataTransferApi.HeppersService;
using DataTransferApi.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace DataTransferApi.Heppers
{
    public class EFCommandHelperService : ICommandDBService
    {
        private readonly AppDbContext _appDbContext;
        public EFCommandHelperService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<SavedFile>> GetFileGroupAsync(string groupName, string userId)
        {
            var fileArray = await _appDbContext.SavedFiles.Where(g => g.UserId == userId
                                                        && g.FileGroup.Name == groupName)
                                                        .ToArrayAsync();
            return fileArray;
        }

        public async Task<IEnumerable<string>> GetFileNameGroupAsync(string groupName, string userId)
        {
            var fileArray = await _appDbContext.SavedFiles.Where(g => g.UserId == userId
                                                        && g.FileGroup.Name == groupName)
                                                        .Select(f => f.SavedFileName)
                                                        .ToArrayAsync();
            return fileArray;
        }

        //Select filePath from db 
        public async Task<string> GetFilePathByNameAsync(string groupName, string fileName, string userId)
        {
            var path = await _appDbContext.SavedFiles.Where(file => file.UserId == userId
                && file.FileGroup.Name == groupName
                && file.SavedFileName == fileName)
                    .Select(s => s.SavedFilePath)
                    .SingleOrDefaultAsync();
            if (path == null)
            {
                //handle
                throw new Exception("null path");
            }
            return path;
        }

        //Method to return file tuple
        public async Task<(byte[], string, string)> ReturnFileTupleAsync(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var _ContentType))
            {
                _ContentType = "application/octet-stream";
            }
            var byteFile = await File.ReadAllBytesAsync(filePath);
            return (byteFile, _ContentType, Path.GetFileName(filePath));
        }

        public async Task<SavedFile> FileByNameAsync(string groupName, string fileName, string userId)
        {
            var savedFile = await _appDbContext.SavedFiles.Where(f => f.FileGroup.Name == groupName
                                                            && f.UserId == userId
                                                            && f.SavedFileName == fileName)
                                                            .SingleOrDefaultAsync();
            if (savedFile == null)
            {
                //handle
                throw new Exception("no file");
            }
            return savedFile;
        }
    }
}
