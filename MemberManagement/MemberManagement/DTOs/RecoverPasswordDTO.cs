using System.ComponentModel.DataAnnotations;

namespace Project5.DTOs
{
    public class RecoverPasswordDTO
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        public string? Email { get; set; }
    }
}
