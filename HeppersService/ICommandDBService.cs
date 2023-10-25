using DataTransferApi.Entities;

namespace DataTransferApi.HeppersService
{
    public interface ICommandDBService
    {
        public Task<(byte[], string, string)> ReturnFileTupleAsync(string filePath);
        public Task<string> GetFilePathByNameAsync(string groupName, string fileName, string userId);
        public Task<IEnumerable<SavedFile>> GetFileGroupAsync(string groupName, string userId);
        public Task<IEnumerable<string>> GetFileNameGroupAsync(string groupName, string userId);
        public Task<SavedFile> FileByNameAsync(string groupName, string fileName, string userId);
    }
}
