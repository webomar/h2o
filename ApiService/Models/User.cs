using System.ComponentModel.DataAnnotations;

namespace ApiService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; } = null!;
    }
}
