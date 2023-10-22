using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DataTransferApi.Entities
{
    public class SavedFile
    {
        [Key]
        public int Id {  get; set; }
        public string SavedFileName { get; set; }
        public string SavedFilePath { get; set; }

        public string UserId { get; set; }
        public User User { get; set; } 

        public int FileGroupId {  get; set; }
        public FileGroup FileGroup { get; set; }
        
    }
}
