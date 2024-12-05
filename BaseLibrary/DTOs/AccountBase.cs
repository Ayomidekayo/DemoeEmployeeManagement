
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class AccountBase
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public String? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String? Password { get; set; }
    }
}
