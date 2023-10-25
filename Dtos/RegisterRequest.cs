using System.ComponentModel.DataAnnotations;

namespace DataTransferApi.Dtos
{
    public class RegisterRequest
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Email {  get; set; }

        [Required]
        public string? Password {  get; set; }
    }
}
