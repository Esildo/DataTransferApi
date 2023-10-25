using Microsoft.AspNetCore.Identity;

namespace DataTransferApi.Entities
{
    public class User : IdentityUser
    {
        public ICollection<SavedFile> SavedFiles { get; set; } = new List<SavedFile>();
    }
}
