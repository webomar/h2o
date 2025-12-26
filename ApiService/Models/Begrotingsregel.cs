using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public enum Kostensoort
    {
        Lasten,
        Baten
    }

    public class Begrotingsregel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Begroting))]
        public int BegrotingJaar { get; set; }

        [ForeignKey(nameof(Medewerker))]
        public int? MedewerkerNummer { get; set; }

        [ForeignKey(nameof(Kostenplaats))]
        public string? KostenplaatsCode { get; set; }

        public Kostensoort Kostensoort { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        // Navigation properties
        public Begroting Begroting { get; set; } = null!;
        public Medewerker? Medewerker { get; set; }
        public Kostenplaats? Kostenplaats { get; set; }
    }
}

