namespace WorkerService.Suppliers.ErpX
{
    public class ErpXCsvRecord
    {
        public int PersNr { get; set; }

        public string Crediteur { get; set; } = null!;

        public string? Rekening { get; set; }

        public string? OrganisatorischeEenheidCode { get; set; }

        public int? BegrotingJaar { get; set; }

        public string? KostenplaatsCode { get; set; }

        public decimal Bedrag { get; set; }

        public string Kostensoort { get; set; } = null!;
    }
}
