using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class Kostenplaats
    {
        [Key]
        public string Code { get; set; } = null!;

        public string Omschrijving { get; set; } = null!;

        [ForeignKey(nameof(OrganisatorischeEenheid))]
        public string OrganisatorischeEenheidCode { get; set; } = null!;

        // Navigation properties
        public OrganisatorischeEenheid OrganisatorischeEenheid { get; set; } = null!;
        public ICollection<Begrotingsregel> Begrotingsregels { get; set; } = new List<Begrotingsregel>();
        public ICollection<Inhuurkosten> Inhuurkosten { get; set; } = new List<Inhuurkosten>();
    }
}

