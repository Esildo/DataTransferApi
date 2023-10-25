using DataTransferApi.Entities;

namespace DataTransferApi.Db
{
    public interface ITransferRepository
    {
        public IQueryable<SavedFile> SavedFiles { get; set; }
        public IQueryable<FileGroup> FileGroups { get; set; }
        public IQueryable<User> Users { get; set; }
        public IQueryable<TokenLink> TokenLinks { get; set; }
    }
}
