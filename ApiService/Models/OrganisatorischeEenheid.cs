using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class OrganisatorischeEenheid
    {
        [Key]
        public string Code { get; set; } = null!;

        public string Omschrijving { get; set; } = null!;

        [ForeignKey(nameof(Parent))]
        public string? ParentCode { get; set; }

        public int? MunicipalityId { get; set; }

        // Navigation properties
        public OrganisatorischeEenheid? Parent { get; set; }
        public ICollection<OrganisatorischeEenheid> Children { get; set; } = new List<OrganisatorischeEenheid>();
        public ICollection<Kostenplaats> Kostenplaatsen { get; set; } = new List<Kostenplaats>();
        public ICollection<Contract> Contracten { get; set; } = new List<Contract>();
    }
}

