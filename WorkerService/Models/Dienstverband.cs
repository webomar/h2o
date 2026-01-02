using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class Dienstverband
    {
        [Key]
        public int Nummer { get; set; }

        public int ExternalNummer { get; set; } // Youforce DienstverbandNummer

        [ForeignKey(nameof(Medewerker))]
        public int MedewerkerNummer { get; set; }

        [ForeignKey(nameof(Functie))]
        public string Functiecode { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime? DatumInDienst { get; set; }

        public DateTime? DatumUitDienst { get; set; }

        public int? Ancienniteit { get; set; }

        // Navigation properties
        public Medewerker Medewerker { get; set; } = null!;
        public Functie Functie { get; set; } = null!;
    }
}

