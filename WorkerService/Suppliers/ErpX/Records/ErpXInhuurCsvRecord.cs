namespace WorkerService.Suppliers.ErpX.Records
{
    public sealed class ErpXInhuurCsvRecord
    {
        public int PersNr { get; set; }

        public int Jaar { get; set; }

        public int Maand { get; set; }

        public string KostenplaatsCode { get; set; } = null!;

        public decimal Bedrag { get; set; }
    }
}
