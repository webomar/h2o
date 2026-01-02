namespace WorkerService.Suppliers.ErpX.Records
{
    public sealed class ErpXBegrotingCsvRecord
    {
        public int PersNr { get; set; }

        public int BegrotingJaar { get; set; }

        public string KostenplaatsCode { get; set; } = null!;

        public decimal Bedrag { get; set; }

        public string? Kostensoort { get; set; }
    }
}
