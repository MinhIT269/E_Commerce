using System.ComponentModel.DataAnnotations;

namespace E_Commerce.UI.Models.Requests
{
    public class LoginRequestDto
    {
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
