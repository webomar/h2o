namespace WorkerService.Suppliers.ErpX
{
    public enum ErpXRecordType
    {
        CONTRACT,
        BEGROTING,
        INHUUR,
        TRANSACTIE
    }

    public class ErpXCsvRecord
    {
        public ErpXRecordType RecordType { get; set; }
        public int PersNr { get; set; }

        // Contract
        public string? Crediteur { get; set; }
        public string? Rekening { get; set; }
        public string? OrganisatorischeEenheidCode { get; set; }

        // Begroting
        public int? BegrotingJaar { get; set; }
        public string? KostenplaatsCode { get; set; }
        public decimal? Bedrag { get; set; }
        public string? Kostensoort { get; set; }

        // Inhuur
        public int? InhuurJaar { get; set; }
        public int? InhuurMaand { get; set; }
        public decimal? InhuurBedrag { get; set; }

        // Transactie
        public DateTime? TransactieDatum { get; set; }
        public decimal? TransactieBedrag { get; set; }
    }
}
