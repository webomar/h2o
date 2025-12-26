using System.ComponentModel.DataAnnotations;

namespace ApiService.Models
{
    public class Periode
    {
        [Key]
        public int Id { get; set; }

        public int Jaar { get; set; }

        public int Maand { get; set; }

        public bool Verwerking { get; set; }

        public string? Label { get; set; }

        // Navigation property
        public ICollection<Inhuurkosten> Inhuurkosten { get; set; } = new List<Inhuurkosten>();
    }
}

