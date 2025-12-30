using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class Transactie
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Contract))]
        public int ContractId { get; set; }

        public DateTime Datum { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        // Navigation property
        public Contract Contract { get; set; } = null!;
    }
}

