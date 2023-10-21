namespace DataTransferApi.Services
{
    public interface IStorageService
    {
        public Task SaveToStorageAsync(List<IFormFile> file, string userName, string userId);
    }
}
