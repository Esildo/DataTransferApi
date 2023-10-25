namespace DataTransferApi.Services
{
    public interface IOneTokenService
    {
        public Task<string> CreateOneFileTokenAsync(string groupName, string userName, string userId);
        public Task<string> CreateOneGroupTokenAsync(string groupName, string userId);
        public Task<(byte[], string, string)> DownloadFileToken(string token);
        public Task<IEnumerable<(byte[], string, string)>> DownLoadFileGroupAsync(string token);
        public Task<int> TokenCheck(string tokenCheck);
        public bool CheckDataAsync(string token);
    }
}
