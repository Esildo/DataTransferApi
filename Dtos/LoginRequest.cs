using System.ComponentModel.DataAnnotations;

namespace DataTransferApi.Dtos
{
    public class LoginRequest
    {
        #nullable enable
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
        #nullable disable
    }
}
