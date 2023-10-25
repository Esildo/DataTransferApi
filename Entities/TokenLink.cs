using System.ComponentModel.DataAnnotations;

namespace DataTransferApi.Entities
{
    public class TokenLink
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public string Path { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
