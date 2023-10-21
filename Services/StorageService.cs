using DataTransferApi.Db;
using DataTransferApi.Entities;
using System.IO;
using System.Net.WebSockets;
using System.Web;
namespace DataTransferApi.Services
{
    public class StorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _appDbContext;

        public StorageService(IWebHostEnvironment env, AppDbContext appDbContext)
        {
            _env = env;
            _appDbContext = appDbContext;
        }

        public async Task SaveToStorageAsync(List<IFormFile> files,string userName, string userId)
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


                foreach (var file in files)
                {
                    string fullpath = Path.Combine(userDirectory, file.FileName);
                    //Need to change if filename exists 
                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var saveFile = new SavedFile
                    {
                        SavedFileName = file.FileName,
                        SavedFilePath = fullpath,
                        UserId = userId
                    };
                    _appDbContext.Add(saveFile);
                    _appDbContext.SaveChanges();
                }
            }
            catch(Exception ex) 
            { }
        }
    }
}
