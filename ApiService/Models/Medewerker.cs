using System.ComponentModel.DataAnnotations;

namespace ApiService.Models
{
    public class Medewerker
    {
        [Key]
        public int Nummer { get; set; }
        public int ExternalNummer { get; set; } // Youforce / ERP-X ID
        public string Achternaam { get; set; } = null!;

        // Navigation properties
        public Dienstverband? Dienstverband { get; set; }
        public ICollection<Begrotingsregel> Begrotingsregels { get; set; } = new List<Begrotingsregel>();
        public ICollection<Contract> Contracten { get; set; } = new List<Contract>();
    }
}

