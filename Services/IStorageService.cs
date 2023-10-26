using DataTransferApi.Dtos;
using DataTransferApi.Entities;

namespace DataTransferApi.Services
{
    public interface IStorageService
    {
        public Task SaveToStorageAsync(List<IFormFile> file, string userName, string userId);

        public Task<IEnumerable<string>> SearchGroupsAsync(string userId);

        public Task<IEnumerable<FileRequest>> SearchFilesAsync(string userId);

        public Task<(byte[], string, string)>DownloadFile(string groupName, string fileName, string userId);

        public Task<IEnumerable<(byte[], string, string)>> DownloadFileGroup(string groupName, string userId);

        public Task<int> LoadPercAsync(string groupName, string fileName, string userId);

        public Task<int> LoadPercGroupAsync(string groupName, string userId);
    }
}
