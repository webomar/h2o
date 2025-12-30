namespace ApiService.Models.DTOs
{
    public class PersonnelCostsDto
    {
        public int Jaar { get; set; }
        public int? Kwartaal { get; set; }
        public int? Maand { get; set; }
        public string? Functiecode { get; set; }
        public string? Functienaam { get; set; }
        public string? OrganisatorischeEenheidCode { get; set; }
        public string? OrganisatorischeEenheidOmschrijving { get; set; }
        public string? KostenplaatsCode { get; set; }
        public string? KostenplaatsOmschrijving { get; set; }
        public decimal BegrootBedrag { get; set; }
        public decimal GerealiseerdBedrag { get; set; }
        public decimal Verschil { get; set; }
        public decimal VerschilPercentage { get; set; }
    }

    public class PersonnelCostsSummaryDto
    {
        public int Jaar { get; set; }
        public int? Kwartaal { get; set; }
        public int? Maand { get; set; }
        public string? Functiecode { get; set; }
        public string? Functienaam { get; set; }
        public string? OrganisatorischeEenheidCode { get; set; }
        public string? OrganisatorischeEenheidOmschrijving { get; set; }
        public string? KostenplaatsCode { get; set; }
        public string? KostenplaatsOmschrijving { get; set; }
        public decimal TotaalBegroot { get; set; }
        public decimal TotaalGerealiseerd { get; set; }
        public decimal TotaalVerschil { get; set; }
        public decimal TotaalVerschilPercentage { get; set; }
    }
}

