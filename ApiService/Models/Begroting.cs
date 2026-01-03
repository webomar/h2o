using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public enum BegrotingStatus
    {
        Primair,
        Gewijzigd
    }

    public class Begroting
    {
        [Key]
        public int Id { get; set; }

        public int Jaar { get; set; }

        public BegrotingStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Totaalbedrag { get; set; }

        // Navigation property
        public ICollection<Begrotingsregel> Begrotingsregels { get; set; } = new List<Begrotingsregel>();
    }
}

