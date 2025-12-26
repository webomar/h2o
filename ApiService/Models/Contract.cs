using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        public string Crediteur { get; set; } = null!;

        [ForeignKey(nameof(Medewerker))]
        public int? MedewerkerNummer { get; set; }

        [ForeignKey(nameof(OrganisatorischeEenheid))]
        public string? OrganisatorischeEenheidCode { get; set; }

        public string? Rekening { get; set; }

        // Navigation properties
        public Medewerker? Medewerker { get; set; }
        public OrganisatorischeEenheid? OrganisatorischeEenheid { get; set; }
        public ICollection<Transactie> Transacties { get; set; } = new List<Transactie>();
    }
}

