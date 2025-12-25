using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class Inhuurkosten
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Periode))]
        public int PeriodeId { get; set; }

        [ForeignKey(nameof(Kostenplaats))]
        public string KostenplaatsCode { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        // Navigation properties
        public Periode Periode { get; set; } = null!;
        public Kostenplaats Kostenplaats { get; set; } = null!;
    }
}

