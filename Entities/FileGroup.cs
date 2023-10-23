using System.ComponentModel.DataAnnotations;
namespace DataTransferApi.Entities
{
    public class FileGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set;}

        public ICollection<SavedFile> SavedFiles { get; set; } = new List<SavedFile>();
    }
}
