using System.ComponentModel.DataAnnotations;

namespace ApiService.Models
{
    public class Functie
    {
        [Key]
        public string Functiecode { get; set; } = null!;

        public string Functienaam { get; set; } = null!;

        public string? Schaall { get; set; }

        // Navigation property
        public ICollection<Dienstverband> Dienstverbanden { get; set; } = new List<Dienstverband>();
    }
}
